#include "Main.h"

void Scene::FreeMemory()
{
    //_filename.FreeMemory();
    _modelList.FreeMemory();
    if(_root)
    {
        delete _root;
        _root = NULL;
    }
    _modelFromGeometryIndex.FreeMemory();
}

void Scene::Reset(AppState &state, const String &baseModelName)
{
    FreeMemory();

    //
    // This is the only place asset memory can be freed because this is the only place where there should be no active models.
    // To call this safetly we must also clear the chosen model.
    //
    //state.assets.FreeMemory();

    //Model *model0 = new Model;
    //model0->LoadDebugSquare(state);

    _root = new ModelInstance(NULL, state.assets.GetModel(baseModelName));
    PersistentAssert(_root->model != NULL, "Required model file not found.");
    _root->UpdateTransform();

    _bbox = _root->model->BoundingBox();

    //_root->children.PushEnd(new ModelInstance(state.assets.GetModel("14f4027490a165a6c017de3427505dac"), Matrix4::Translation(Vec3f(5.0f, 5.0f, 5.0f))));
    
    UpdateModelIDs(state);

    _filename = "Untitled";
    _searchTerms.FreeMemory();

    _uiEventLog.Reset();
}

UINT Scene::MakeModelList(AppState &state, const String &filename, Vector<String> &models)
{
    {
        ifstream file(filename.CString());
        if(file.fail())
        {
            String s = "Failed to open " + filename;
            MessageBox(NULL, s.CString(), "SceneStudio", MB_OK);
            return 1;
        }
    }
    
    Vector<String> lines;
    Utility::GetFileLines(filename, lines);
    if(lines.Length() < 4 || lines[0] != "SceneStudio A" || lines[1] != "v 1")
    {
        return 2;
    }
    Vector<String> words;
    lines[2].Partition(' ', words);
    if(words.Length() != 2 || words[0] != "modelCount")
    {
        return 2;
    }

    const UINT modelCount = words[1].ConvertToUnsignedInteger();
    if(modelCount == 0)
    {
        return 2;
    }

    models.FreeMemory();
    
    for(UINT lineIndex = 3; lineIndex < lines.Length(); lineIndex++)
    {
        const String &curLine = lines[lineIndex];
        if(curLine.Length() >= 2)
        {
            curLine.Partition(' ', words);
            const char c0 = curLine[0];
            if(c0 == 'n' && !models.Contains(words[1]))
            {
                models.PushEnd(words[1]);
            }
        }
    }

    return 0;
}

UINT Scene::SaveSceneASCII(AppState &state, const String &filename)
{
    if(filename != "*")
    {
        _filename = filename;
    }

    ofstream file(_filename.CString());
    if(file.fail())
    {
        String s = "Failed to open " + filename;
        MessageBox(NULL, s.CString(), "SceneStudio", MB_OK);
        return 1;
    }
    
    file << "SceneStudio A\n";
    file << "v 1\n";
    file << "modelCount " << _modelList.Length() << '\n';
    
    for(UINT searchTermIndex = 0; searchTermIndex < _searchTerms.Length(); searchTermIndex++)
    {
        file << "z " << _searchTerms[searchTermIndex].FindAndReplace(String(" "), String("_")) << '\n';
    }
    
    // Save Camera
    file << "C " << state.camera.ToString() << endl;

    _root->SaveASCII(file);
    _uiEventLog.SaveASCII(file);
    state.sceneDirty = false;
    return 0;
}

UINT Scene::LoadSceneASCII(AppState &state, const String &filename)
{
    {
        ifstream file(filename.CString());
        if(file.fail())
        {
            String s = "Failed to open " + filename;
            MessageBox(NULL, s.CString(), "SceneStudio", MB_OK);
            return 1;
        }
    }
    
    Vector<String> lines;
    Utility::GetFileLines(filename, lines);
    if(lines.Length() < 4 || lines[0] != "SceneStudio A" || lines[1] != "v 1")
    {
        return 2;
    }
    Vector<String> words;
    lines[2].Partition(' ', words);
    if(words.Length() != 2 || words[0] != "modelCount")
    {
        return 2;
    }

    const UINT modelCount = words[1].ConvertToUnsignedInteger();
    if(modelCount == 0)
    {
        return 2;
    }

    FreeMemory();

    _modelList.Allocate(modelCount);
    for(UINT modelIndex = 0; modelIndex < modelCount; modelIndex++)
    {
        _modelList[modelIndex] = new ModelInstance(NULL, NULL);
    }
    _root = _modelList[0];

    ModelInstance *activeModel = _modelList[0];
    for(UINT lineIndex = 3; lineIndex < lines.Length(); lineIndex++)
    {
        const String &curLine = lines[lineIndex];
        if(curLine.Length() >= 2)
        {
            curLine.Partition(' ', words);
            const char c0 = curLine[0];
            const char c1 = curLine[1];
            if(c0 == 'm')
            {
                UINT modelIndex = words[1].ConvertToUnsignedInteger();
                activeModel = _modelList[modelIndex];

                //
                // Only the root object has no parent
                //
                if(modelIndex != 0)
                {
                    activeModel->parent = _modelList[words[2].ConvertToUnsignedInteger()];
                }
            }
            else if(c0 == 'z')
            {
                if(words.Length() >= 2)
                {
                    _searchTerms.PushEnd(words[1]);
                }
            }
            else if(c0 == 'C') // Parse saved SceneCamera
            {
                if(words.Length() >= 2)
                {
                    state.camera = SceneCamera(words[1]);
                    state.loadedCamera = true;
                }
            }
            else if(c0 == 'c')
            {
                for(UINT childIndex = 0; childIndex < words.Length() - 1; childIndex++)
                {
                    activeModel->children.PushEnd(_modelList[words[childIndex + 1].ConvertToUnsignedInteger()]);
                }
            }
            else if(c0 == 'n')
            {
                activeModel->model = state.assets.GetModel(words[1]);
                if(activeModel->model == NULL)
                {
                    SignalError("Model in file not found in database");
                    return 2;
                }
            }
            else if(c0 == 'f')
            {
                activeModel->cubeFace = words[1].ConvertToUnsignedInteger();
            }
            else if(c0 == 's')
            {
                if(words[1].Length() != 8)
                {
                    SignalError("Invalid scale");
                    return 2;
                }
                String::HexStringToByteStream(words[1], (BYTE *)&activeModel->scale);
            }
            else if(c0 == 'p' && c1 == 'g')
            {
                activeModel->parentGeometryIndex = words[1].ConvertToUnsignedInteger();
            }
            else if(c0 == 'p' && c1 == 't')
            {
                activeModel->parentTriangleIndex = words[1].ConvertToUnsignedInteger();
            }
            else if(c0 == 'p' && c1 == 'u')
            {
                if(words[1].Length() != 16)
                {
                    SignalError("Invalid UV");
                    return 2;
                }
                String::HexStringToByteStream(words[1], (BYTE *)&activeModel->parentTriangleUV);
            }
            else if(c0 == 'p' && c1 == 'o')
            {
                if(words[1].Length() != 24)
                {
                    SignalError("Invalid offset");
                    return 2;
                }
                String::HexStringToByteStream(words[1], (BYTE *)&activeModel->parentOffset);
            }
            else if(c0 == 't' && c1 == 'a')
            {
                if(words[1].Length() != 8 * 16)
                {
                    SignalError("Invalid transform");
                    return 2;
                }
                String::HexStringToByteStream(words[1], (BYTE *)&activeModel->firstTransform);
            }
            else if(c0 == 't' && c1 == 'b')
            {
                if(words[1].Length() != 8 * 16)
                {
                    SignalError("Invalid transform");
                    return 2;
                }
                String::HexStringToByteStream(words[1], (BYTE *)&activeModel->secondTransform);
            }
            else if(c0 == 'q')
            {
                // Skip UIEventLogger lines for now and independently parse file to allow for interleaving
            }
            else
            {
                SignalError("Invalid line");
            }
        }
    }

    UpdateModelIDs(state);
    _root->UpdateTransform();
    state.sceneDirty = false;
    _filename = filename;

    _uiEventLog.LoadASCII(filename);

    return 0;
}

void Scene::SaveSceneBinary(AppState &state, OutputDataStream &stream)
{
    //
    // The ModelIDs should already be updated. But without this call sometimes
    // undo causes an incorrect model ordering. Trying to reproduce this bug
    // is important since Undo crashing the program is very bad.
    //
    UpdateModelIDs(state);

    stream << _modelList.Length();
    _root->SaveBinary(stream);
}

void Scene::LoadSceneBinary(AppState &state, InputDataStream &stream)
{
    UINT modelCount;
    stream >> modelCount;
    
    FreeMemory();

    _modelList.Allocate(modelCount);
    for(UINT modelIndex = 0; modelIndex < modelCount; modelIndex++)
    {
        _modelList[modelIndex] = new ModelInstance(NULL, NULL);
    }
    _root = _modelList[0];

    for(UINT modelIndex = 0; modelIndex < modelCount; modelIndex++)
    {
        UINT savedModelIndex;
        stream >> savedModelIndex;
        PersistentAssert(savedModelIndex == modelIndex, "Index mismatch");
        ModelInstance &curModel = *_modelList[savedModelIndex];

        UINT parentIndex;
        stream >> parentIndex;
        if(parentIndex != 0xFFFFFFFF)
        {
            curModel.parent = _modelList[parentIndex];
        }
        UINT childCount;
        stream >> childCount;
        for(UINT childIndex = 0; childIndex < childCount; childIndex++)
        {
            UINT childModelIndex;
            stream >> childModelIndex;
            curModel.children.PushEnd(_modelList[childModelIndex]);
        }
        String modelName;
        stream >> modelName;
        curModel.model = state.assets.GetModel(modelName);
        PersistentAssert(curModel.model != NULL, "Model not found");
        stream >> curModel.cubeFace;
        stream >> curModel.scale;
        stream >> curModel.parentGeometryIndex;
        stream >> curModel.parentTriangleIndex;
        stream.ReadData(curModel.parentTriangleUV);
        stream.ReadData(curModel.parentOffset);
        stream.ReadData(curModel.firstTransform);
        stream.ReadData(curModel.secondTransform);
    }

    UpdateModelIDs(state);
    _root->UpdateTransform();
    state.sceneDirty = true;

    //Validate();
}

void Scene::Validate()
{
    for(UINT modelIndex = 0; modelIndex < _modelList.Length(); modelIndex++)
    {
        const ModelInstance &curModel = *_modelList[modelIndex];
        PersistentAssert(curModel.model != NULL, "ModelInstance with no model");
    }
}

void Scene::SaveRelationshipGraphASCII(AppState &state, const String &filename)
{
    ofstream file(filename.CString());

    UpdateModelIDs(state);

    Vector<ModelRelationshipEdge> edgeList;
    _root->MakeRelationshipEdgeList(edgeList);

    const UINT nodeCount = _modelList.Length();
    const UINT edgeCount = edgeList.Length();

    file << "GraphCount 1" << endl;
    file << "g 0 NodeCount " << nodeCount << endl;
    file << "g 0 EdgeCount " << edgeCount << endl;
    for(UINT nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
    {
        // Hash
        ModelInstance &curNode = *_modelList[nodeIndex];
        const String NodeHeader = String("n ") + String(nodeIndex) + String(" ");
        file << NodeHeader << "ModelHash " << curNode.model->name() << endl;

        // Name
        ModelInfo *info = state.modelDatabase.GetModelInfo(curNode.model->name());
        if(info != NULL)
        {
            file << NodeHeader << "ModelName " << info->name << endl;
        }

        // Model bbox
        Rectangle3f BBox = curNode.model->BoundingBox();
        file << NodeHeader << "BBox " << BBox.Min.x << "," << BBox.Min.y << "," << BBox.Min.z << "," <<
                                         BBox.Max.x << "," << BBox.Max.y << "," << BBox.Max.z << endl;

        // Transform
        curNode.UpdateTransform();
        Matrix4 xform = curNode.transform;
        file << NodeHeader << "Transform " << xform[0][0] << " " << xform[0][1] << " " << xform[0][2] << " " << xform[0][3] << " "
                                           << xform[1][0] << " " << xform[1][1] << " " << xform[1][2] << " " << xform[1][3] << " "
                                           << xform[2][0] << " " << xform[2][1] << " " << xform[2][2] << " " << xform[2][3] << " "
                                           << xform[3][0] << " " << xform[3][1] << " " << xform[3][2] << " " << xform[3][3] << endl;

        // Path
        Vector<ModelInstance*> path;
        curNode.MakePath(path);
        file << NodeHeader << "Path _";
        for(UINT PathIndex = 0; PathIndex < path.Length(); PathIndex++)
        {
            file << path[PathIndex]->index;
            if(PathIndex != path.Length() - 1)
            {
                file << "_";
            }
        }
        file << endl;
    }
    for(UINT edgeIndex = 0; edgeIndex < edgeCount; edgeIndex++)
    {
        const ModelRelationshipEdge &curEdge = edgeList[edgeIndex];
        const String EdgeHeader = String("e ") + String(edgeIndex) + String(" ");
        file << EdgeHeader << "Nodes " << curEdge.n0->index << ',' << curEdge.n1->index << endl;
        file << EdgeHeader << "Normal " << curEdge.normal.ToString(',') << endl;
    }
}

void Scene::UpdateModelIDs(AppState &state)
{
    _modelList.FreeMemory();
    _root->MakeModelList(_modelList);

    _modelFromGeometryIndex.FreeMemory();
    const UINT modelCount = _modelList.Length();
    for(UINT modelIndex = 0; modelIndex < modelCount; modelIndex++)
    {
        ModelInstance *curModel = _modelList[modelIndex];
        curModel->index = modelIndex;
        curModel->baseId = _modelFromGeometryIndex.Length();
        const UINT geometryCount = curModel->model->geometry().Length();
        for(UINT geometryIndex = 0; geometryIndex < geometryCount; geometryIndex++)
        {
            _modelFromGeometryIndex.PushEnd(curModel);
        }
    }
}

void Scene::RenderNormal(AppState &state, bool renderChosen)
{
    //Validate();

    RenderParameters parameters;
    parameters.vs = &state.globalAssets.VertexShaders.model;
    parameters.ps = &state.globalAssets.PixelShaders.model;
    
    parameters.transparencyStage = 0;
    _root->Render(state, parameters);

    Vec3f pos, normal;
    Matrix4 chosenTransform;
    if(renderChosen && state.chosenModel != NULL)
    {
        if(GetSelectedPos(state, pos, normal))
        {
            Vec3f epsilonOffset = epsilonNormalDisplacement * normal;
            chosenTransform = MakeFirstTransform(state, *state.chosenModel, state.chosenFace, pos, normal) * state.chosenSecondTransform * Matrix4::Rotation(normal, state.chosenRotation) * Matrix4::Scaling(state.chosenScale) * Matrix4::Translation(pos + epsilonOffset);

            parameters.selectedBase = false;
            parameters.selectedChild = false;
            state.chosenModel->Render(state, chosenTransform, parameters);
        }
        else
        {
            renderChosen = false;
        }
    }
    
    parameters.transparencyStage = 1;
    _root->Render(state, parameters);

    if(renderChosen && state.chosenModel != NULL)
    {
        parameters.selectedBase = false;
        parameters.selectedChild = false;
        state.chosenModel->Render(state, chosenTransform, parameters);
    }
    //RenderDebugBBox(state);
}

void Scene::RenderPickingA(AppState &state)
{
    RenderParameters parameters;
    parameters.vs = &state.globalAssets.VertexShaders.pickingA;
    parameters.ps = &state.globalAssets.PixelShaders.pickingA;

    _root->Render(state, parameters);
}

void Scene::RenderPickingB(AppState &state)
{
    RenderParameters parameters;
    parameters.vs = &state.globalAssets.VertexShaders.pickingB;
    parameters.ps = &state.globalAssets.PixelShaders.pickingB;

    if(state.selectedGeometryIndex < _modelFromGeometryIndex.Length())
    {
        const ModelInstance &instance = *_modelFromGeometryIndex[state.selectedGeometryIndex];
        instance.model->Render(state, instance.transform, parameters);
    }

    //_root->Render(state, parameters);
}

bool Scene::GetSelectedPos(AppState &state, Vec3f &pos, Vec3f &normal)
{
    if(state.selectedGeometryIndex >= _modelFromGeometryIndex.Length())
    {
        return false;
    }
    const ModelInstance &curModel = *_modelFromGeometryIndex[state.selectedGeometryIndex];

    return curModel.GetPos(state.selectedGeometryIndex - curModel.baseId, state.selectedFaceIndex, state.selectedUV, pos, normal);
}

bool Scene::RemoveSelectedModel(AppState &state, bool deleteModel)
{
    if(state.selectedModel == NULL)
    {
        return false;
    }
    ModelInstance *parent = state.selectedModel->parent;
    if(parent == NULL)
    {
        return false;
    }

    Assert(parent->children.Contains(state.selectedModel), "parent does not contain child");
    parent->children.RemoveSwap(parent->children.FindFirstIndex(state.selectedModel));
    if(deleteModel)
    {
        delete state.selectedModel;
        state.selectedModel = NULL;
    }
    state.sceneDirty = true;
    return true;
}

ModelInstance* Scene::GetSelectedModel(AppState &state)
{
    if(state.selectedGeometryIndex >= _modelFromGeometryIndex.Length())
    {
        return NULL;
    }
    return _modelFromGeometryIndex[state.selectedGeometryIndex];
}

Matrix4 Scene::MakeFirstTransform(AppState &state, const Model &model, UINT face, const Vec3f &pos, const Vec3f &normal)
{
    const Rectangle3f &bbox = model.BoundingBox();
    
    Matrix4 faceTranslation, finalRotation;
    
    Vec3f centralAxis;
    if(face == 0)
    {
        centralAxis = Vec3f::eZ;
        faceTranslation = Matrix4::Translation(Vec3f(-bbox.Center().x, -bbox.Center().y, -bbox.Min.z));
    }
    else if(face == 1)
    {
        centralAxis = Vec3f::eX;
        faceTranslation = Matrix4::Translation(Vec3f(-bbox.Min.x, -bbox.Center().y, -bbox.Center().z));
    }
    else if(face == 2)
    {
        centralAxis = Vec3f::eY;
        faceTranslation = Matrix4::Translation(Vec3f(-bbox.Center().x, -bbox.Min.y, -bbox.Center().z));
    }
    else if(face == 3)
    {
        centralAxis = -Vec3f::eZ;
        faceTranslation = Matrix4::Translation(Vec3f(-bbox.Center().x, -bbox.Center().y, -bbox.Max.z));
    }
    else if(face == 4)
    {
        centralAxis = -Vec3f::eX;
        faceTranslation = Matrix4::Translation(Vec3f(-bbox.Max.x, -bbox.Center().y, -bbox.Center().z));
    }
    else if(face == 5)
    {
        centralAxis = -Vec3f::eY;
        faceTranslation = Matrix4::Translation(Vec3f(-bbox.Center().x, -bbox.Max.y, -bbox.Center().z));
    }
    
    Vec3f diff = state.camera.VecEye() - pos;
    Vec3f fixedNormal = normal;
    if(Vec3f::Dot(diff, normal) < 0.0f)
    {
        fixedNormal *= -1.0f;
    }

    finalRotation = Matrix4::Face(centralAxis, fixedNormal);

    return faceTranslation * finalRotation;
}

bool Scene::RelocateSelectedModel(AppState &state)
{
    Vec3f pos, normal;
    if(state.selectedGeometryIndex >= _modelFromGeometryIndex.Length())
    {
        return false;
    }
    ModelInstance &newParentModel = *_modelFromGeometryIndex[state.selectedGeometryIndex];
    if(!newParentModel.GetPos(state.selectedGeometryIndex - newParentModel.baseId, state.selectedFaceIndex, state.selectedUV, pos, normal))
    {
        return false;
    }

    bool success = RemoveSelectedModel(state, false);
    if(!success)
    {
        return false;
    }
    
    ModelInstance &model = *state.selectedModel;
    model.parent = &newParentModel;
    model.parentGeometryIndex = state.selectedGeometryIndex - newParentModel.baseId;
    model.parentOffset = Vec3f::Origin;
    model.parentTriangleIndex = state.selectedFaceIndex;
    model.parentTriangleUV = state.selectedUV;
    model.firstTransform = MakeFirstTransform(state, *model.model, model.cubeFace, pos, normal);

    newParentModel.children.PushEnd(state.selectedModel);

    model.UpdateTransform();

    UpdateModelIDs(state);
    state.sceneDirty = true;
    return true;
}

bool Scene::InsertChosenModel(AppState &state, ModelInstance*& insertedModel)
{
    if(state.chosenModel == NULL)
    {
        return false;
    }
    Vec3f pos, normal;
    if(state.selectedGeometryIndex >= _modelFromGeometryIndex.Length())
    {
        return false;
    }
    ModelInstance &curModel = *_modelFromGeometryIndex[state.selectedGeometryIndex];
    if(!curModel.GetPos(state.selectedGeometryIndex - curModel.baseId, state.selectedFaceIndex, state.selectedUV, pos, normal))
    {
        return false;
    }
    
    state.undoStack.RecordAction(state, ActionInsert, NULL);

    ModelInstance *newModel = new ModelInstance(&curModel, state.chosenModel);
    insertedModel = newModel;
    
    newModel->firstTransform = MakeFirstTransform(state, *state.chosenModel, state.chosenFace, pos, normal);
    newModel->secondTransform = state.chosenSecondTransform * Matrix4::Rotation(normal, state.chosenRotation);

    newModel->parentGeometryIndex = state.selectedGeometryIndex - curModel.baseId;
    newModel->parentOffset = Vec3f::Origin;
    newModel->parentTriangleIndex = state.selectedFaceIndex;
    newModel->parentTriangleUV = state.selectedUV;
    newModel->cubeFace = state.chosenFace;
    newModel->scale = state.chosenScale;

    curModel.children.PushEnd(newModel);

    newModel->UpdateTransform();

    UpdateModelIDs(state);
    state.sceneDirty = true;
    return true;
}

void Scene::RenderDebugBBox(AppState &state)
{
    RenderSphere(state, Vec3f(_bbox.Min.x, _bbox.Min.y, _bbox.Min.z), 2.0f);
    RenderSphere(state, Vec3f(_bbox.Min.x, _bbox.Max.y, _bbox.Min.z), 2.0f);
    RenderSphere(state, Vec3f(_bbox.Max.x, _bbox.Min.y, _bbox.Min.z), 2.0f);
    RenderSphere(state, Vec3f(_bbox.Max.x, _bbox.Max.y, _bbox.Min.z), 2.0f);

    RenderSphere(state, Vec3f(_bbox.Min.x, _bbox.Min.y, _bbox.Max.z), 2.0f);
    RenderSphere(state, Vec3f(_bbox.Min.x, _bbox.Max.y, _bbox.Max.z), 2.0f);
    RenderSphere(state, Vec3f(_bbox.Max.x, _bbox.Min.y, _bbox.Max.z), 2.0f);
    RenderSphere(state, Vec3f(_bbox.Max.x, _bbox.Max.y, _bbox.Max.z), 2.0f);
}

void Scene::RenderSphere(AppState &state, const Vec3f &pos, float radius)
{
    auto device = state.GD.GetDevice();

    device->SetRenderState(D3DRS_CULLMODE, D3DCULL_NONE);
    device->SetRenderState(D3DRS_ALPHABLENDENABLE, FALSE);

    state.globalAssets.VertexShaders.solid.Set();
    state.globalAssets.PixelShaders.solid.Set();
    
    Matrix4 sphereTransform = Matrix4::Scaling(radius) * Matrix4::Translation(pos);
    state.globalAssets.VertexShaders.solid.SetMatrix("WorldViewProj", sphereTransform * state.viewProjection);
    state.globalAssets.PixelShaders.solid.SetVec4("Color", Vec4f(0.0f, 0.0f, 0.5f, 1.0f));
    
    state.globalAssets.selectionSphere.Render();
}

void Scene::RenderSelectionSphere(AppState &state)
{
    Vec3f pos, normal;
    if(!GetSelectedPos(state, pos, normal))
    {
        return;
    }

    RenderSphere(state, pos, 2.0f);
}
