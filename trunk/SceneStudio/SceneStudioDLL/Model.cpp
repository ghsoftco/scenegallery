#include "Main.h"

Rectangle3f Model::ComputeBoundingBox() const
{
    Rectangle3f result(Vec3f::Origin, Vec3f::Origin);
    bool bboxInitialized = false;
    for(UINT geometryIndex = 0; geometryIndex < _geometry.Length(); geometryIndex++)
    {
        const ModelGeometryEntry &curGeometry = _geometry[geometryIndex];
        if(!curGeometry.isLineMesh)
        {
            Rectangle3f curBBox = curGeometry.mesh->BoundingBox(curGeometry.transform);
            if(!bboxInitialized)
            {
                bboxInitialized = true;
                result = curBBox;
            }
            else
            {
                result.ExpandBoundingBox(curBBox.Min);
                result.ExpandBoundingBox(curBBox.Max);
            }
        }
    }
    return result;
}

void Model::LoadDebugSquare(AppState &state)
{
    _geometry.Allocate(1);

    auto &g = _geometry[0];
    g.diffuse = Vec4f(1.0f, 0.5f, 0.5f, 1.0f);
    g.isLineMesh = false;
    
    g.mesh = new D3D9Mesh(state.GD);
    g.mesh->CreateBox(1.0f, 1.0f, 1.0f, 0);
    state.assets.AddMesh("square", g.mesh);
    g.meshPick = state.assets.GetPickMesh("square");

    g.texture = NULL;
    g.transform = Matrix4::Identity();
    g.transparency = 0.0f;
    g.transparent = Vec4f::Origin;
}

void Model::MakeTextureList(AppState &state, const String &modelName, Vector<UINT64> &textures)
{
    FILE *file = Utility::CheckedFOpen((state.parameters.modelDirectory + modelName).CString(), "rb");
    UINT geometryCount = 0;
    Utility::CheckedFRead(&geometryCount, sizeof(UINT), 1, file);

    textures.FreeMemory();

    for(UINT geometryIndex = 0; geometryIndex < geometryCount; geometryIndex++)
    {
        ModelGeometryEntry curGeometry;
        Utility::CheckedFRead(&curGeometry.transform, sizeof(Matrix4), 1, file);

        Utility::CheckedFRead(&curGeometry.diffuse, sizeof(Vec4f), 1, file);
        Utility::CheckedFRead(&curGeometry.transparent, sizeof(Vec4f), 1, file);
        Utility::CheckedFRead(&curGeometry.transparency, sizeof(float), 1, file);

        UINT64 materialValue = 0;
        Utility::CheckedFRead(&materialValue, sizeof(UINT64), 1, file);
        if(materialValue == 0)
        {
            // lines
        }
        else if(materialValue == 1)
        {
            // no texture
        }
        else
        {
            if(!textures.Contains(materialValue))
            {
                textures.PushEnd(materialValue);
            }
        }

        UINT64 geometryHash = 0;
        Utility::CheckedFRead(&geometryHash, sizeof(UINT64), 1, file);
    }

    fclose(file);
}

void Model::LoadBinary(AppState &state, const String &modelName)
{
    _name = modelName;

    FILE *file = Utility::CheckedFOpen((state.parameters.modelDirectory + modelName).CString(), "rb");
    UINT geometryCount = 0;
    Utility::CheckedFRead(&geometryCount, sizeof(UINT), 1, file);

    _geometry.Allocate(geometryCount);

    FileCollection meshes;
    meshes.LoadCompressed(state.parameters.geometryDirectory + modelName);
    
    for(UINT geometryIndex = 0; geometryIndex < geometryCount; geometryIndex++)
    {
        ModelGeometryEntry &curGeometry = _geometry[geometryIndex];
        Utility::CheckedFRead(&curGeometry.transform, sizeof(Matrix4), 1, file);

        Utility::CheckedFRead(&curGeometry.diffuse, sizeof(Vec4f), 1, file);
        Utility::CheckedFRead(&curGeometry.transparent, sizeof(Vec4f), 1, file);
        Utility::CheckedFRead(&curGeometry.transparency, sizeof(float), 1, file);

        UINT64 materialValue = 0;
        Utility::CheckedFRead(&materialValue, sizeof(UINT64), 1, file);
        curGeometry.texture = NULL;
        curGeometry.isLineMesh = false;
        if(materialValue == 0)
        {
            curGeometry.isLineMesh = true;
            // lines
        }
        else if(materialValue == 1)
        {
            // no texture
        }
        else
        {
            curGeometry.texture = state.assets.GetTexture(String::Hash64ToHexString(materialValue) + String(".jpg"));
        }

        UINT64 geometryHash = 0;
        Utility::CheckedFRead(&geometryHash, sizeof(UINT64), 1, file);
        const String &meshName = String::Hash64ToHexString(geometryHash);
        curGeometry.mesh = state.assets.GetMesh(meshName, meshes);
        Assert(curGeometry.mesh->VertexCount() > 0, "No vertices in mesh");
        if(curGeometry.isLineMesh)
        {
            curGeometry.meshPick = NULL;
        }
        else
        {
            curGeometry.meshPick = state.assets.GetPickMesh(meshName);
        }
    }

    fclose(file);

    _bbox = ComputeBoundingBox();
}

void Model::Render(AppState &state, const Matrix4 &transform, RenderParameters &parameters) const
{
    auto device = state.GD.GetDevice();

    device->SetRenderState(D3DRS_SRCBLEND, D3DBLEND_BLENDFACTOR);
    device->SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVBLENDFACTOR);
    device->SetRenderState(D3DRS_CULLMODE, D3DCULL_CW);

    parameters.ps->Set();
    parameters.vs->Set();

    bool isPickingA = (parameters.ps == &state.globalAssets.PixelShaders.pickingA);
    bool isPickingB = (parameters.ps == &state.globalAssets.PixelShaders.pickingB);
    bool isPicking = (isPickingA || isPickingB);

    const Matrix4 transformViewProjection = transform * state.viewProjection;

    for(UINT geometryIndex = 0; geometryIndex < _geometry.Length(); geometryIndex++)
    {
        const ModelGeometryEntry &curGeometry = _geometry[geometryIndex];

        bool isTransparent = (curGeometry.transparency != 0.0f);
        bool shouldRender = isPicking || (!isTransparent && parameters.transparencyStage == 0) || (isTransparent && parameters.transparencyStage == 1);
        if(isPicking && curGeometry.isLineMesh)
        {
            shouldRender = false;
        }

        if(shouldRender)
        {
            if(curGeometry.texture == NULL)
            {
                state.globalAssets.whiteTexture.Set(0);
            }
            else
            {
                curGeometry.texture->Set(0);
            }

            parameters.vs->SetMatrix("WorldViewProj", curGeometry.transform * transformViewProjection);
            if(isPickingA)
            {
                UINT32 modelID = parameters.pickerIDBase + geometryIndex;
                BYTE *c = (BYTE *)&modelID;
                
                //
                // Correct byte ordering determined by mastermind algorithm
                //
                Vec4f value = Vec4f(c[2] / 255.0f, c[1] / 255.0f, c[0] / 255.0f, c[3] / 255.0f);

                parameters.ps->SetVec4("ModelId", value);
            }
            else if(isPickingB)
            {

            }
            else
            {
                Vec4f color = curGeometry.diffuse;
                Vec3f colorShift = Vec3f::Origin;
                if(parameters.selectedBase)
                {
                    colorShift.x = -0.25f;
                    colorShift.y = -0.25f;
                    colorShift.z = 0.5f;
                }
                else if(parameters.selectedChild)
                {
                    colorShift.x = 0.5f;
                    colorShift.y = -0.25f;
                    colorShift.z = -0.25f;
                }
                parameters.ps->SetVec4("Color", color);
                parameters.ps->SetVec3("ColorShift", colorShift);
                parameters.vs->SetMatrix("World", curGeometry.transform);

                //
                // This is a bug in SketchUp's COLLADA exporter.  Sometimes it will make surfaces that should be opaque extremely transparent.
                //
                bool falseTransparency = (curGeometry.transparent.x <= 0.05f && curGeometry.transparent.y <= 0.05f && curGeometry.transparent.z <= 0.05f);

                if(curGeometry.transparency == 0.0f || falseTransparency)
                {
                    device->SetRenderState(D3DRS_ALPHABLENDENABLE, FALSE);
                }
                else
                {
                    D3DCOLOR Color = RGBColor(curGeometry.transparency * curGeometry.transparent);
                    device->SetRenderState(D3DRS_ALPHABLENDENABLE, TRUE);
                    device->SetRenderState(D3DRS_BLENDFACTOR, Color);
                }
                //device->SetRenderState(D3DRS_ALPHABLENDENABLE, FALSE);
            }

            if(isPickingB)
            {
                curGeometry.meshPick->Render();
            }
            else
            {
                curGeometry.mesh->Render();
            }
        }
    }

    device->SetRenderState(D3DRS_ALPHABLENDENABLE, FALSE);
}
