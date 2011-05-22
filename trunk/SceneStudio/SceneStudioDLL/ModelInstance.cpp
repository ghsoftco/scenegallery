#include "Main.h"

void ModelInstance::SaveASCII(ofstream &file) const
{
    if(parent == NULL)
    {
        file << "m " << index << '\n';
    }
    else
    {
        file << "m " << index << ' ' << parent->index << '\n';
    }
    if(children.Length() > 0)
    {
        file << "c ";
        for(UINT childIndex = 0; childIndex < children.Length(); childIndex++)
        {
            file << children[childIndex]->index;
            if(childIndex != children.Length())
            {
                file << " ";
            }
        }
        file << '\n';
    }
    file << "n " << model->name() << '\n';
    file << "f " << cubeFace << '\n';
    file << "s " << String::ByteStreamToHexString((const BYTE *)&scale, sizeof(float)) << '\n';
    file << "pg " << parentGeometryIndex << '\n';
    file << "pt " << parentTriangleIndex << '\n';
    file << "pu " << String::ByteStreamToHexString((const BYTE *)&parentTriangleUV, sizeof(Vec2f)) << '\n';
    file << "po " << String::ByteStreamToHexString((const BYTE *)&parentOffset, sizeof(Vec3f)) << '\n';
    file << "ta " << String::ByteStreamToHexString((const BYTE *)&firstTransform, sizeof(Matrix4)) << '\n';
    file << "tb " << String::ByteStreamToHexString((const BYTE *)&secondTransform, sizeof(Matrix4)) << '\n';
    
    for(UINT childIndex = 0; childIndex < children.Length(); childIndex++)
    {
        children[childIndex]->SaveASCII(file);
    }
}

void ModelInstance::SaveBinary(OutputDataStream &stream) const
{
    stream << index;
    if(parent == NULL)
    {
        stream << UINT(0xFFFFFFFF);
    }
    else
    {
        stream << parent->index;
    }
    stream << children.Length();
    for(UINT childIndex = 0; childIndex < children.Length(); childIndex++)
    {
        stream << children[childIndex]->index;
    }
    stream << model->name();
    stream << cubeFace;
    stream << scale;
    stream << parentGeometryIndex;
    stream << parentTriangleIndex;
    stream.WriteData(parentTriangleUV);
    stream.WriteData(parentOffset);
    stream.WriteData(firstTransform);
    stream.WriteData(secondTransform);

    for(UINT childIndex = 0; childIndex < children.Length(); childIndex++)
    {
        children[childIndex]->SaveBinary(stream);
    }
}

void ModelInstance::Render(AppState &state, RenderParameters &parameters) const
{
    parameters.pickerIDBase = baseId;

    //bool selected = (state.selectedGeometryIndex < state.scene._modelFromGeometryIndex.Length() && state.scene._modelFromGeometryIndex[state.selectedGeometryIndex] == this);
    bool selected = state.selectedModel == this;
    if (model != NULL && model->name().StartsWith("room") && model->name().Length() == 6)
    {
        selected = false;
    }

    if(selected)
    {
        bool moving = (state.mode == ModeMoveSurfaces || state.mode == ModeMovePlane || state.mode == ModeLevitate);
        if( (parameters.ps == &state.globalAssets.PixelShaders.pickingA || parameters.ps == &state.globalAssets.PixelShaders.pickingB) && moving)
        {
            return;
        }
        parameters.selectedBase = true;
    }
    model->Render(state, transform, parameters);
    
    if(selected)
    {
        parameters.selectedBase = false;
        parameters.selectedChild = true;
    }

    for(UINT childIndex = 0; childIndex < children.Length(); childIndex++)
    {
        children[childIndex]->Render(state, parameters);
    }

    if(selected)
    {
        parameters.selectedChild = false;
    }
}

void ModelInstance::MakePath(Vector<ModelInstance*> &path)
{
    if(parent != NULL)
    {
        parent->MakePath(path);
    }
    path.PushEnd(this);
}

void ModelInstance::MakeRelationshipEdgeList(Vector<ModelRelationshipEdge> &edgeList)
{
    for(UINT childIndex = 0; childIndex < children.Length(); childIndex++)
    {
        ModelInstance *curChild = children[childIndex];
        ModelRelationshipEdge newEdge;
        newEdge.n0 = this;
        newEdge.n1 = curChild;

        Vec3f pos, normal;
        GetPos(curChild->parentGeometryIndex, curChild->parentTriangleIndex, curChild->parentTriangleUV, pos, normal);
        newEdge.pos = pos;
        newEdge.normal = normal;

        edgeList.PushEnd(newEdge);
    }
    for(UINT childIndex = 0; childIndex < children.Length(); childIndex++)
    {
        children[childIndex]->MakeRelationshipEdgeList(edgeList);
    }
}

void ModelInstance::MakeModelList(Vector<ModelInstance*> &modelList)
{
    modelList.PushEnd(this);
    for(UINT childIndex = 0; childIndex < children.Length(); childIndex++)
    {
        children[childIndex]->MakeModelList(modelList);
    }
}

void ModelInstance::UpdateTransform()
{
    if(parent == NULL)
    {
        transform = firstTransform * secondTransform;
    }
    else
    {
        Vec3f pos, normal;
        parent->GetPos(parentGeometryIndex, parentTriangleIndex, parentTriangleUV, pos, normal);
    
        Vec3f epsilonOffset = epsilonNormalDisplacement * normal;
        transform = firstTransform * secondTransform * Matrix4::Scaling(scale) * Matrix4::Translation(pos + parentOffset + epsilonOffset);
    }

    for(UINT childIndex = 0; childIndex < children.Length(); childIndex++)
    {
        children[childIndex]->UpdateTransform();
    }
}

bool ModelInstance::GetPos(UINT geometryIndex, UINT faceIndex, const Vec2f &UV, Vec3f &pos, Vec3f &normal) const
{
    if(geometryIndex >= model->geometry().Length())
    {
        return false;
    }

    ModelGeometryEntry &curGeometry = model->geometry()[geometryIndex];
    if(curGeometry.meshPick == NULL)
    {
        return false;
    }
    UINT vertexCount = curGeometry.meshPick->VertexCount();
    if(faceIndex * 3 >= vertexCount)
    {
        return false;
    }

    Matrix4 m = curGeometry.transform * transform;

    const MeshVertex *V = curGeometry.meshPick->Vertices() + faceIndex * 3;
    Vec3f v0 = m.TransformPoint(V[0].Pos);
    Vec3f v1 = m.TransformPoint(V[1].Pos);
    Vec3f v2 = m.TransformPoint(V[2].Pos);

    pos = v0 + (v1 - v0) * UV.x + (v2 - v0) * UV.y;

    normal = Vec3f::Normalize(Vec3f::Cross(v1 - v0, v2 - v0));

    return true;
}

void ModelInstance::ApplyCascadingTransform(const Matrix4 &preMatrix, const Matrix4 &postMatrix)
{
    secondTransform = preMatrix * secondTransform * postMatrix;
    for(UINT childIndex = 0; childIndex < children.Length(); childIndex++)
    {
        children[childIndex]->ApplyCascadingTransform(Matrix4::Identity(), preMatrix * postMatrix);
    }
}

void ModelInstance::CascadingScale(float scaleFactor)
{
    scale *= scaleFactor;
    for(UINT childIndex = 0; childIndex < children.Length(); childIndex++)
    {
        children[childIndex]->CascadingScale(scaleFactor);
    }
}

void ModelInstance::CascadingReset()
{
    secondTransform = Matrix4::Identity();
    for(UINT childIndex = 0; childIndex < children.Length(); childIndex++)
    {
        children[childIndex]->CascadingReset();
    }
}
