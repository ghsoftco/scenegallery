#include "Main.h"

void App::ReleaseAssets()
{
    _state.globalAssets.pickingSurfaceRGB.ReleaseMemory();
    _state.globalAssets.pickingSurfaceUV.ReleaseMemory();
    _state.globalAssets.scratchSurface.ReleaseMemory();
    _state.globalAssets.debugFont.ReleaseMemory();
}

void App::ResetAssets()
{
    _state.globalAssets.pickingSurfaceRGB.Reset(_state.GD);
    _state.globalAssets.pickingSurfaceUV.Reset(_state.GD);
    _state.globalAssets.scratchSurface.Reset(_state.GD);
    _state.globalAssets.debugFont.Reset(_state.GD);
}

void App::InitD3D(HWND window)
{
    if (DebuggingMode)
        AllocConsole();

    _state.parameters.Init("Parameters.txt");
    _state.window.Init(window);
    _state.dimensions = _state.window.Dimensions();

    GraphicsDeviceParameters parameters;
    parameters.ScreenColor = RGBColor::White;
    parameters.WaitForVSync = true;
    parameters.MultisampleCount = _state.parameters.multisampleCount;
    _state.GD.Init(parameters, window, _state.dimensions.x, _state.dimensions.y);
    _state.device = _state.GD.GetDevice();

    _state.device->Clear( 0, NULL, D3DCLEAR_ZBUFFER | D3DCLEAR_TARGET, D3DCOLOR_ARGB(parameters.ScreenColor.a, parameters.ScreenColor.r, parameters.ScreenColor.g, parameters.ScreenColor.b), 1.0f, 0 );

    _state.device->BeginScene();
    _state.device->EndScene();

    _state.device->Present( NULL, NULL, NULL, NULL );

    _state.device->SetSamplerState(0, D3DSAMP_MAGFILTER, D3DTEXF_LINEAR);
    _state.device->SetSamplerState(0, D3DSAMP_MINFILTER, D3DTEXF_LINEAR);
    _state.device->SetSamplerState(0, D3DSAMP_MIPFILTER, D3DTEXF_LINEAR);

    _state.modelDatabase.Init(_state);

    _state.assets.Init(_state);

    _state.scene.Reset(_state, "room01");

    _state.loadedCamera = false;
    ResetCamera();

    _state.globalAssets.PixelShaders.model.Init    (_state.GD, _state.parameters.shaderDirectory + String("model.ps"));
    _state.globalAssets.VertexShaders.model.Init   (_state.GD, _state.parameters.shaderDirectory + String("model.vs"));
    _state.globalAssets.PixelShaders.solid.Init    (_state.GD, _state.parameters.shaderDirectory + String("solid.ps"));
    _state.globalAssets.VertexShaders.solid.Init   (_state.GD, _state.parameters.shaderDirectory + String("solid.vs"));
    _state.globalAssets.PixelShaders.pickingA.Init (_state.GD, _state.parameters.shaderDirectory + String("pickingA.ps"));
    _state.globalAssets.VertexShaders.pickingA.Init(_state.GD, _state.parameters.shaderDirectory + String("pickingA.vs"));
    _state.globalAssets.PixelShaders.pickingB.Init (_state.GD, _state.parameters.shaderDirectory + String("pickingB.ps"));
    _state.globalAssets.VertexShaders.pickingB.Init(_state.GD, _state.parameters.shaderDirectory + String("pickingB.vs"));

    _state.globalAssets.whiteTexture.Init(_state.GD);

    _state.globalAssets.debugFont.Init(_state.GD, "Calibri", 20, FW_BOLD);

    Bitmap bmp(2, 2);
    bmp.Clear(RGBColor::White);
    _state.globalAssets.whiteTexture.Load(bmp);

    _state.globalAssets.selectionSphere.SetGD(_state.GD);
    _state.globalAssets.selectionSphere.CreateSphere(1.0f, 3);

    _state.selectedGeometryIndex = 0xFFFFFFFF;
    SetMode(ModeNormal);
    _state.selectedModel = NULL;

    _state.chosenModel = NULL;
    _state.chosenSecondTransform = Matrix4::Identity();

    _state.copiedModel = NULL;

    _state.sceneDirty = false;

    // Initialize sampling spheres
    _state.showSurfaceSamples = false;
    _state.globalAssets.samplingSpheres.Allocate(samplingSpheresToRender);
    for (UINT samplingSphereIndex = 0; samplingSphereIndex < _state.globalAssets.samplingSpheres.Length(); samplingSphereIndex++)
    {
        _state.globalAssets.samplingSpheres[samplingSphereIndex].first.SetGD(_state.GD);
        _state.globalAssets.samplingSpheres[samplingSphereIndex].first.CreateSphere(1.0f, 0);
    }
}

void App::RenderFrame()
{
    Vec2i curDimensions = _state.window.Dimensions();
    if(curDimensions.x > 0 && curDimensions.y > 0 && curDimensions != _state.dimensions)
    {
        GraphicsDeviceParameters parameters;
        parameters.ScreenColor = RGBColor::White;
        parameters.WaitForVSync = true;
        _state.GD.ReSize(_state.window, *this);
        _state.device = _state.GD.GetDevice();

        _state.dimensions = curDimensions;

        UpdateCamera();
    }

    _state.device->Clear( 0, NULL, D3DCLEAR_ZBUFFER | D3DCLEAR_TARGET, RGBColor(255, 255, 255), 1.0f, 0 );

    _state.device->BeginScene();

    _state.scene.RenderNormal(_state, _state.mode == ModeInsert);

    //if(_state.mode != ModeInsert)
    //{
    //    _state.scene.RenderSelectionSphere(_state);
    //}

    if(_state.showSurfaceSamples)
    {
        _state.scene.RenderSamplingSpheres(_state);
    }

    RenderStatusText();

    _state.device->EndScene();

    _state.device->Present( NULL, NULL, NULL, NULL );
}

void App::RenderStatusText()
{
    _consoleY = 2;
    if(DebuggingStatusText)
    {
        //Status(String("selected geometry: ") + String(_state.selectedGeometryIndex), RGBColor::Red);
        //Status(String("selected face: ") + String(_state.selectedFaceIndex), RGBColor::Red);
        //Status(String("selected UV: ") + _state.selectedUV.CommaSeparatedString(), RGBColor::Red);

        _state.undoStack.Report(_state, *this);
    }
}

void App::Status(const String &s, RGBColor c)
{
    _state.globalAssets.debugFont.DrawString(s, Vec2i(2, _consoleY), c);
    _consoleY += 20;
}

void App::ReportString(const String& s)
{
    if (DebuggingMode)
    {
        Console::WriteLine(s);
        //HANDLE cons_out = GetStdHandle(STD_OUTPUT_HANDLE);
        //LPDWORD written;
        //WriteConsole(cons_out, s.CString(), s.Length(), written, NULL);
    }
}

void App::ReportMode()
{
    String mode;
    switch (_state.mode)
    {
    case ModeNormal:
        mode = "Normal"; break;
    case ModeLooking:
        mode = "Looking "; break;
    case ModeSelecting:
        mode = "Selecting"; break;
    case ModeMoveSurfaces:
        mode = "MoveSurfaces"; break;
    case ModeMovePlane:
        mode = "MovePlane"; break;
    case ModeLevitate:
        mode = "Levitate"; break;
    case ModeInsert:
        mode = "Insert"; break;
    default:
        mode = "MODE UNKNOWN!!!"; break;
    }
    mode += "\n";
    ReportString(mode);
}

void App::MouseDown(UINT button, int x, int y)
{
    //ReportString("MouseDown\n");

    bool shift = (GetAsyncKeyState(VK_SHIFT) != 0);
    bool ctrl = (GetAsyncKeyState(VK_CONTROL) != 0);
    bool alt = (GetAsyncKeyState(VK_MENU) != 0);

    //String mods = "Shift: " + String(shift) + ", Ctrl: " + String(ctrl) + ", Alt: " + String(alt) + "\n";
    //ReportString(mods);

    // Quasi-Mode transitions
    if (_state.mode == ModeNormal)
    {
        if (button == 1 && !alt && !shift && !ctrl)
        {
            SetMode(ModeSelecting);

            _state.device->BeginScene();
            _state.picker.PickGeometry(_state, x, y);
            _state.picker.PickTriangle(_state, x, y);
            _state.device->EndScene();

            _state.selectedModel = _state.scene.GetSelectedModel(_state);
        }
        else if ((button == 1 || button == 2) && alt && !shift && !ctrl)
            SetMode(ModeLooking);
        else if (button == 2 && !alt && !shift && !ctrl)
            SetMode(ModeMoveSurfaces);
        else if (button == 2 && !alt && !ctrl && shift)
            SetMode(ModeMovePlane);
        else if (button == 2 && !alt && !shift && ctrl)
            SetMode(ModeLevitate);
    }
    else if (_state.mode == ModeInsert && (button == 1 || button == 2) && alt)
    {
        SetMode(ModeLooking);
    }

    // Object insertion
    else if (_state.mode == ModeInsert && button == 1 && !alt)
    {
        InsertModel();
    }
}

void App::InsertModel()
{
    ModelInstance* insertedModel;
    if(_state.scene.InsertChosenModel(_state, insertedModel))
    {
        _state.selectedModel = insertedModel;
        SetMode(ModeNormal);
        _state.selectedFaceIndex = 0xFFFFFFFF;
        _state.selectedGeometryIndex = 0xFFFFFFFF;
    }
}

void App::MouseUp(UINT button)
{
    //ReportString("MouseUp\n");

    // Quasi-mode transitions
    if (_state.mode == ModeSelecting && button == 1)
        SetMode(ModeNormal);
    else if (_state.mode == ModeMoveSurfaces && button == 2)
        SetMode(ModeNormal);
    else if (_state.mode == ModeLooking && (button == 1 || button == 2))
        SetMode(ModeNormal);
    else if (_state.mode == ModeMovePlane && button == 2)
        SetMode(ModeNormal);
    else if (_state.mode == ModeLevitate && button == 2)
        SetMode(ModeNormal);

    //ReportMode();
}

void App::MouseMove(UINT button, int x, int y, int xDelta, int yDelta)
{
    //ReportString("MouseMove\n");

    bool shift = (GetAsyncKeyState(VK_SHIFT) != 0);
    bool ctrl = (GetAsyncKeyState(VK_CONTROL) != 0);
    bool alt = (GetAsyncKeyState(VK_MENU) != 0);

    // Quasi-mode transitions
    if (_state.mode == ModeLooking && !alt)
        SetMode(ModeNormal);
    if (_state.mode == ModeMovePlane && !shift)
        SetMode(ModeNormal);
    if (_state.mode == ModeLevitate && !ctrl)
        SetMode(ModeNormal);

    //ReportMode();

    // Looking
    if(_state.mode == ModeLooking)
    {
        const float movementScale = 0.3f;
        const float angleScale = 0.0075f;
        if(button == 2)
        {
            //if(shift)
            //{
            //    _state.camera.Move(yDelta * movementScale);
            //}
            //else
            {
                _state.camera.Strafe(xDelta * movementScale);
                _state.camera.Jump(yDelta * movementScale);
            }
        }
        else if(button == 1)
        {
            if(shift)
            {
                _state.camera.LookRight(-xDelta * angleScale * 0.25f);
                _state.camera.LookUp(yDelta * angleScale * 0.25f);
            }
            else
            {
                _state.camera.LookRight(-xDelta * angleScale);
                _state.camera.LookUp(yDelta * angleScale);
            }
        }
        _state.camera.Update();
        _state.viewProjection = _state.camera.ViewMatrix() * _state.projection;
    }
    // Selecting
    else if(_state.mode == ModeSelecting)
    {
        _state.device->BeginScene();
        _state.picker.PickGeometry(_state, x, y);
        _state.picker.PickTriangle(_state, x, y);
        _state.device->EndScene();

        _state.selectedModel = _state.scene.GetSelectedModel(_state);
        //_state.picker.SaveDebugBitmaps();
    }
    // Moving on surfaces
    else if(_state.mode == ModeMoveSurfaces)
    {
        _state.device->BeginScene();
        _state.picker.PickGeometry(_state, x, y);
        _state.picker.PickTriangle(_state, x, y);
        _state.device->EndScene();

        if(_state.selectedModel != NULL)
        {
            _state.undoStack.RecordAction(_state, ActionMove, _state.selectedModel);

            _state.sceneDirty = true;
            _state.selectedModel->parentOffset = Vec3f::Origin;
            _state.scene.RelocateSelectedModel(_state);
        }
    }
    // Modified move modes
    else if(_state.mode == ModeMovePlane || _state.mode == ModeLevitate)
    {
        if(_state.selectedModel != NULL)
        {
            _state.undoStack.RecordAction(_state, ActionMove, _state.selectedModel);

            const float movementScale = 0.4f;
            Vec3f translation;
            // Levitate
            if (_state.mode == ModeLevitate)
            {
                translation = Vec3f(0.0f, 0.0f, -yDelta * movementScale);
            }
            // Move in plane
            else if (_state.mode == ModeMovePlane)
            {
                Vec3f a = _state.camera.VecLookDir();
                Vec3f b = _state.camera.VecRight();
                a.z = 0.0f;
                b.z = 0.0f;
                translation = a * float(yDelta) * movementScale + b * -float(xDelta) * movementScale;
            }

            _state.undoStack.RecordAction(_state, ActionMove, _state.selectedModel);
            _state.sceneDirty = true;
            _state.selectedModel->parentOffset += translation;
            _state.selectedModel->UpdateTransform();
        }
    }
    // Insert object
    else if(_state.mode == ModeInsert)
    {
        _state.device->BeginScene();
        _state.picker.PickGeometry(_state, x, y);
        _state.picker.PickTriangle(_state, x, y);
        _state.device->EndScene();
    }
}

void App::MouseWheel(int delta)
{
    DollyCamera(-delta * wheelScale);
}

void App::ExportAllScenes()
{
    _state.chosenModel = NULL;
    _state.selectedModel = NULL;

    const String &inputDirectory = "C:\\SceneModeling\\Scenes";
    const String &outputDirectory = "C:\\SceneModeling\\Graphs\\";

    Directory sceneDirectory(inputDirectory);
    for(UINT fileIndex = 0; fileIndex < sceneDirectory.Files().Length(); fileIndex++)
    {
        const String &curFile = sceneDirectory.Files()[fileIndex];
        if(curFile.EndsWith(".scs"))
        {
            Console::WriteLine("Exporting " + curFile);
            _state.scene.FreeMemory();
            _state.assets.FreeMemory();
            _state.scene.LoadSceneASCII(_state, sceneDirectory.DirectoryPath() + curFile);
            _state.scene.SaveRelationshipGraphASCII(_state, outputDirectory + curFile.RemoveSuffix(".scs") + String(".txt"));
        }
    }
    Console::WriteLine("Done");
}

void App::CreateSceneThumbnails(const String &sceneDirectory, const Vec2i &dim)
{
    _state.chosenModel = NULL;
    _state.selectedModel = NULL;

    Directory dir(sceneDirectory);
    for(UINT fileIndex = 0; fileIndex < dir.Files().Length(); fileIndex++)
    {
        const String &curFile = dir.Files()[fileIndex];
        if(curFile.EndsWith(".scs"))
        {
            _state.scene.FreeMemory();
            _state.assets.FreeMemory();
            SaveSceneThumbnail(dir.DirectoryPath() + curFile, dim);
        }
    }
}

void App::KeyPress(int key, bool shift, bool ctrl)
{
    if(DebuggingMode)
    {
        if(key == KEY_V)
        {
            Console::WriteLine(_state.scene.BaseModelName() + " " + _state.camera.ToString());
        }
        if(key == KEY_G)
        {
            ExportAllScenes();
        }
        if(key == KEY_F)
        {
            _state.GD.ToggleWireframe();
        }
    }

    if(key == KEY_ESCAPE)
    {
        ClearSelection();
    }

    if(key == KEY_R && ctrl)
    {
        ResetCamera();
    }

    if(key == KEY_B)
    {
        ResetRotation();
    }

    if(key == KEY_N)
    {
        FlipFace(-1);
    }

    if(key == KEY_M)
    {
        FlipFace(1);
    }

    if(shift)
    {
        if(key == KEY_UP) Scale(1.0f + scaleDeltaSmall);
        if(key == KEY_DOWN) Scale(1.0f / (1.0f + scaleDeltaSmall));
        if(key == KEY_LEFT) Rotate(rotateDeltaSmall);
        if(key == KEY_RIGHT) Rotate(-rotateDeltaSmall);
    }
    else
    {
        if(key == KEY_UP) Scale(1.0f + scaleDelta);
        if(key == KEY_DOWN) Scale(1.0f / (1.0f + scaleDelta));
        if(key == KEY_LEFT) Rotate(rotateDelta);
        if(key == KEY_RIGHT) Rotate(-rotateDelta);
    }

    if(key == KEY_DELETE || key == KEY_BACKSPACE)
    {
        DeleteSelectedModel();
    }

    if (ctrl && key == KEY_C)
    {
        Copy();
    }

    if (ctrl && key == KEY_V)
    {
        Paste();
    }

    if (ctrl && key == KEY_Z)
    {
        Undo();
    }
    if (ctrl)
    {
        String filename;
        if (!_state.scene.Filename().EndsWith(".scs"))
        {
            filename = String("screenshot");
        }
        else
        {
            filename = _state.scene.Filename().RemoveSuffix(".scs");
        }

        // Save model hash map
        if (key == KEY_D)
        {
            _state.picker.SaveModelNameGrid(_state, filename + String(".txt"), screenshotDim);
        }

        // Screenshot
        if (key == KEY_P)
        {
            Screenshot(filename + String(".png"), screenshotDim);
        }

        // Create Thumbnails
        if (key == KEY_T)
        {
            CreateSceneThumbnails(exemplarsDirectory, screenshotDim);
        }

        // Re-Root
        if (key == KEY_R)
        {
            _state.scene.SetSelectedModelAsRoot(_state);
        }

        // Test MeshSampler
        if (key == KEY_Q)
        {
            if (!_state.showSurfaceSamples)
                CreateSurfaceSamplingSpheres();
            _state.showSurfaceSamples = !_state.showSurfaceSamples;
        }
    }
}

void App::CreateSurfaceSamplingSpheres()
{
    Vector<MeshSample> samples;
    ModelInstance *m = _state.selectedModel;
    if (m == NULL) return;
    MeshSampler::Sample(m->model->geometry(), samplingSpheresToRender, samples);
    for (UINT i = 0; i < samples.Length(); i++)
    {
        if (samples[i].normal.z > minZforSampledNormal)
            _state.globalAssets.samplingSpheres[i].second = m->transform.TransformPoint(samples[i].pos);
    }
}

void App::ModelChosen(const char *model)
{
    String modelName = String(model);

    if(modelName.Length() == 0)
    {
        _state.chosenModel = NULL;
    }
    else
    {
        _state.chosenModel = _state.assets.GetModel(modelName);
        _state.scene.LogUIEvent(UIEventModelChosen, modelName);
    }
    if(_state.chosenModel != NULL)
    {
        const Rectangle3f &bbox = _state.chosenModel->BoundingBox();

        const float sceneBBoxLength = (_state.scene.BoundingBox().Dimensions().x + _state.scene.BoundingBox().Dimensions().y + _state.scene.BoundingBox().Dimensions().z) / 3.0f;
        const float modelBBoxLength = Math::Max(bbox.Dimensions().x, bbox.Dimensions().y, bbox.Dimensions().z);

        float scaleFactor = 1.0f;
        if(modelBBoxLength < 0.05f * sceneBBoxLength)
        {
            scaleFactor = 0.05f * sceneBBoxLength / modelBBoxLength;
        }
        if(modelBBoxLength > 0.3f * sceneBBoxLength)
        {
            scaleFactor = 0.3f * sceneBBoxLength / modelBBoxLength;
        }

        //Vec3f bboxBottom = Vec3f(bbox.Center().x, bbox.Center().y, bbox.Min.z);

        _state.chosenFace = 0;
        _state.chosenRotation = 0.0f;
        _state.chosenScale = scaleFactor * 1.5f;
        _state.chosenSecondTransform = Matrix4::Identity();

    }
}

const char* App::QueryString(QueryType q)
{
    if(q == QuerySelectedModelName)
    {
        if(_state.selectedModel == NULL)
        {
            return NULL;
        }
        return _state.selectedModel->model->name().CString();
    }
    else if(q == QuerySceneFilename)
    {
        return _state.scene.Filename().CString();
    }
    else if(q == QueryModelList)
    {
        return _sceneModelList.CString();
    }
    else if(q == QuerySearchResults)
    {
        return _searchResults.CString();
    }
    return NULL;
}

int App::QueryInteger(QueryType q)
{
    if(q == QueryIsSceneDirty)
    {
        if(_state.sceneDirty)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    else if (q == QueryIsInsertMode)
    {
        return _state.mode == ModeInsert;
    }
    return 0;
}

UINT App::ProcessCommand(const char *command)
{
    const String &s = command;
    Vector<String> words;
    s.Partition('\t', words);
    if(words[0] == "save")
    {
        if(words.Length() != 2)
        {
            Utility::MessageBox("Improperly formatted save command");
            return 1;
        }
        return SaveScene(words[1]);
    }
    else if(words[0] == "new")
    {
        if(words.Length() != 2)
        {
            Utility::MessageBox("Improperly formatted new command");
            return 1;
        }
        return NewScene(words[1]);
    }
    else if(words[0] == "load")
    {
        if(words.Length() != 2)
        {
            Utility::MessageBox("Improperly formatted load command");
            return 1;
        }
        return LoadScene(words[1]);
    }
    else if(words[0] == "loadModelList")
    {

        if(words.Length() != 2)
        {
            Utility::MessageBox("Improperly formatted loadModelList command");
            return 1;
        }
        return LoadModelList(words[1]);
    }
    else if(words[0] == "resetCamera")
    {
        ResetCamera();
        return 0;
    }
    else if (words[0] == "enterInsertMode")
    {
        EnterInsertMode();
        return 0;
    }
    else if(words[0] == "shape")
    {
        if(words.Length() != 2)
        {
            Utility::MessageBox("Improperly formatted shape command");
            return 1;
        }
        ShapeSearch(words[1]);
        return 0;
    }
    else if (words[0] == "textSearch")
    {
        if(words.Length() != 2)
        {
            Utility::MessageBox("Improperly formatted text search command");
            return 1;
        }
        TextSearch(words[1]);
        return 0;
    }
    else if(words[0] == "setChosenModelScale")
    {
        if(words.Length() != 2)
        {
            Utility::MessageBox("Improperly formatted set chosen model scale command");
            return 1;
        }
        SetChosenModelScale(words[1].ConvertToFloat());
        return 0;
    }
    else if(words[0] == "SetChosenModelTransform")
    {
        if(words.Length() != 2)
        {
            Utility::MessageBox("Improperly formatted set chosen model transform from file command");
            return 1;
        }
        return SetChosenModelTransformFromFile(words[1]);
    }
    else if(words[0] == "LogUIEvent")
    {
        if(words.Length() != 3)
        {
            Utility::MessageBox("Improperly formatted log event command");
            return 1;
        }
        _state.scene.LogUIEvent(UIEvent::tagToType(words[1]), words[2]);
        return 0;
    }
    else
    {
        Utility::MessageBox("Unrecognized command");
        return 1;
    }
}

void App::ResetCamera()
{
    // If camera loaded from saved scene, update and return
    if (_state.loadedCamera)
    {
        UpdateCamera();
        _state.loadedCamera = false;
        return;
    }

    _state.scene.LogUIEvent(UIEventResetCamera, _state.camera.ToString());
    ArchitectureInfo *curArchitecture = _state.modelDatabase.GetArchitectureInfo(_state.scene.BaseModelName());
    if(curArchitecture == NULL)
    {
        _state.camera = SceneCamera("125.929,-215.176,178.166,0.008615,-0.926567,0.376028,-0.0034961,0.376012,0.926608,-1,0,0,0,0,1");
    }
    else
    {
        _state.camera = SceneCamera(curArchitecture->camera);
    }
    UpdateCamera();
}

void App::UpdateCamera()
{
    _state.projection = Matrix4::PerspectiveFov(60.0f * Math::PIf / 180.0f, float(_state.dimensions.x) / float(_state.dimensions.y), 2.0f, 750.0f);
    _state.camera.Update();
    _state.viewProjection = _state.camera.ViewMatrix() * _state.projection;
}

void App::DollyCamera(float dist)
{
    _state.scene.LogUIEvent(UIEventDollyCamera, String(dist));
    //if(_state.mode == ModeNormal || _state.mode == ModeLooking)
    {
        _state.camera.Move(dist);
        _state.camera.Update();
        _state.viewProjection = _state.camera.ViewMatrix() * _state.projection;
    }
}

void App::ClearSelection()
{
    _state.scene.LogUIEvent(UIEventClearSelection, "");
    SetMode(ModeNormal);
    _state.selectedFaceIndex = 0xFFFFFFFF;
    _state.selectedGeometryIndex = 0xFFFFFFFF;
    _state.selectedModel = NULL;
}

void App::EnterInsertMode()
{
    _state.selectedModel = NULL;
    SetMode(ModeInsert);
}

void App::ResetRotation()
{

    if(_state.selectedModel != NULL && _state.selectedModel->parent != NULL)
    {
        _state.scene.LogUIEvent(UIEventResetRotation, _state.selectedModel->model->name());
        _state.undoStack.RecordAction(_state, ActionResetRotation, NULL);

        //
        // evalute whether face should also be reset here
        //
        _state.selectedModel->cubeFace = 0;

        _state.selectedModel->CascadingReset();
        _state.selectedGeometryIndex = _state.selectedModel->parentGeometryIndex + _state.selectedModel->parent->baseId;
        _state.selectedFaceIndex = _state.selectedModel->parentTriangleIndex;
        _state.selectedUV = _state.selectedModel->parentTriangleUV;
        _state.scene.RelocateSelectedModel(_state);
    }
}

void App::Copy()
{
    if(_state.selectedModel != NULL && _state.selectedModel->parent != NULL)
    {
        _state.scene.LogUIEvent(UIEventCopy, _state.selectedModel->model->name());
        _state.copiedModel = _state.selectedModel->model;
        _state.copiedFace = _state.selectedModel->cubeFace;
        _state.copiedRotation = 0.0f;
        _state.copiedScale = _state.selectedModel->scale;
        _state.copiedSecondTransform = _state.selectedModel->secondTransform;
    }
}

void App::Paste()
{
    if(_state.copiedModel != NULL)
    {
        _state.scene.LogUIEvent(UIEventPaste, _state.copiedModel->name());
        _state.chosenModel = _state.copiedModel;
        _state.chosenFace = _state.copiedFace;
        _state.chosenRotation = _state.copiedRotation;
        _state.chosenScale = _state.copiedScale;
        _state.chosenSecondTransform = _state.copiedSecondTransform;

        EnterInsertMode();
    }
}

void App::Undo()
{    _state.scene.LogUIEvent(UIEventUndo, "");
_state.undoStack.Undo(_state);
}

void App::DeleteSelectedModel()
{
    if(_state.selectedModel != NULL)
    {
        _state.scene.LogUIEvent(UIEventDelete, _state.selectedModel->model->name());
        _state.undoStack.RecordAction(_state, ActionDelete, NULL);

        _state.scene.RemoveSelectedModel(_state, true);
        _state.selectedModel = NULL;
        _state.selectedGeometryIndex = 0xFFFFFFFF;
        _state.selectedFaceIndex = 0xFFFFFFFF;
    }
}

void App::FlipFace(int faceDelta)
{
    _state.scene.LogUIEvent(UIEventFlipFace, String(faceDelta));
    if(_state.mode == ModeInsert)
    {
        _state.chosenFace = Math::Mod(int(_state.chosenFace) + faceDelta, 6);
    }
    else if(_state.selectedModel != NULL && _state.selectedModel->parent != NULL)
    {
        _state.undoStack.RecordAction(_state, ActionChangeFace, NULL);

        _state.selectedModel->cubeFace = Math::Mod(int(_state.selectedModel->cubeFace) + faceDelta, 6);
        _state.selectedGeometryIndex = _state.selectedModel->parentGeometryIndex + _state.selectedModel->parent->baseId;
        _state.selectedFaceIndex = _state.selectedModel->parentTriangleIndex;
        _state.selectedUV = _state.selectedModel->parentTriangleUV;
        _state.scene.RelocateSelectedModel(_state);
    }
}

void App::Rotate(float rotate)
{
    _state.scene.LogUIEvent(UIEventRotate, String(rotate));
    if(_state.mode == ModeInsert)
    {
        _state.chosenRotation += rotate;
    }
    else if(_state.selectedModel != NULL && _state.selectedModel->parent != NULL)
    {

        _state.undoStack.RecordAction(_state, ActionRotate, _state.selectedModel);

        Vec3f pos, normal;
        _state.selectedModel->parent->GetPos(_state.selectedModel->parentGeometryIndex, _state.selectedModel->parentTriangleIndex, _state.selectedModel->parentTriangleUV, pos, normal);
        _state.selectedModel->ApplyCascadingTransform(Matrix4::Identity(), Matrix4::Rotation(normal, rotate));

        _state.sceneDirty = true;
        _state.selectedModel->UpdateTransform();
    }
}

void App::Scale(float scale)
{
    _state.scene.LogUIEvent(UIEventScale, String(scale));
    if(_state.mode == ModeInsert)
    {
        _state.chosenScale *= scale;
    }
    else if(_state.selectedModel != NULL && _state.selectedModel->parent != NULL)
    {
        _state.undoStack.RecordAction(_state, ActionScale, _state.selectedModel);
        _state.selectedModel->CascadingScale(scale);

        _state.sceneDirty = true;
        _state.selectedModel->UpdateTransform();
    }
}

void App::SetChosenModelScale(float scale)
{
    if(_state.mode == ModeInsert)
    {
        _state.chosenScale = scale;
    }
}

UINT App::SetChosenModelTransformFromFile(const String &filename)
{
    if (_state.chosenModel == NULL) return -1;

    ifstream file(filename.CString());
    if(file.fail())
    {
        String s = "Failed to open " + filename;
        MessageBox(NULL, s.CString(), "SceneStudio", MB_OK);
        return 1;
    }

    Vector<String> lines;
    Utility::GetFileLines(filename, lines);
    if(lines.Length() < 4 || lines[0] != "SceneStudio A" || lines[1] != "v 1")
    {
        return 2;
    }
    
    Vector<String> words;
    bool foundModel = false;

    UINT lineIndex = 3;
    for(; lineIndex < lines.Length(); lineIndex++)
    {
        const String &curLine = lines[lineIndex];
        if(curLine.Length() >= 2)
        {
            curLine.Partition(' ', words);
            const char c0 = curLine[0];
            const char c1 = curLine[1];
            if(c0 == 'n')
            {
                if (!foundModel && (words[1] == _state.chosenModel->name()))
                {
                    foundModel = true;
                    break;
                }
            }
        }
    }

    if (foundModel)
    {
        // Stupidly assume f and s immediately follow n
        const String &fLine = lines[lineIndex+1];
        const String &sLine = lines[lineIndex+2];
        fLine.Partition(' ', words);
        _state.chosenFace = words[1].ConvertToUnsignedInteger();
        sLine.Partition(' ', words);
        String::HexStringToByteStream(words[1], (BYTE *)&(_state.chosenScale));
    }

    return 0;
}

UINT App::LoadScene(const String &filename)
{
    UINT result = _state.scene.LoadSceneASCII(_state, filename);
    if(result == 0)
    {
        _state.undoStack.FreeMemory();
        _state.sceneDirty = false;
        _state.selectedModel = NULL;
        ResetCamera();

        //
        // After a large number of textures are loaded into memory, the UI will be very laggy until they are used a few times.
        // By repeatedly rendering the scene a few times we can force all this high latency time to be integrated into the
        // scene loading time.
        //
        for(UINT iteration = 0; iteration < 10; iteration++)
        {
            _state.device->BeginScene();
            _state.picker.PickGeometry(_state, 1, 1);
            _state.picker.PickTriangle(_state, 1, 1);
            _state.device->EndScene();
        }
    }
    else if(result == 2)
    {
        MessageBox(NULL, "This is not a valid SceneStudio file.", "SceneStudio", MB_OK);
    }
    return result;
}

UINT App::SaveScene(const String &filename)
{
    _state.scene.LogUIEvent(UIEventSaveScene, filename);
    return _state.scene.SaveSceneASCII(_state, filename);
}

UINT App::LoadModelList(const String &filename)
{
    _sceneModelList.FreeMemory();
    Vector<String> models;
    UINT result = Scene::MakeModelList(_state, filename, models);
    if(result == 0)
    {
        for(UINT modelIndex = 0; modelIndex < models.Length(); modelIndex++)
        {
            const String &curModel = models[modelIndex];
            _sceneModelList += models[modelIndex];
            if(modelIndex != models.Length() - 1)
            {
                _sceneModelList.PushEnd(' ');
            }
        }
    }
    else if(result == 2)
    {
        MessageBox(NULL, "This is not a valid SceneStudio file.", "SceneStudio", MB_OK);
    }
    return result;
}

UINT App::NewScene(const String &scene)
{
    _state.undoStack.FreeMemory();
    _state.scene.Reset(_state, scene);
    ResetCamera();
    _state.sceneDirty = false;
    return 0;
}

void App::TextSearch(const String &query)
{
    _state.scene.LogUIEvent(UIEventTextSearch, query);
    _state.modelDatabase.TextSearch(query, _searchResults);
    _state.scene.AddSearchTerm(query);
}

void App::ShapeSearch(const String &query)
{
    _state.scene.LogUIEvent(UIEventShapeSearch, query);
    _state.modelDatabase.ShapeSearch(query, _searchResults);
}

ModeType App::SetMode(ModeType m)
{
    ModeType previousMode = _state.mode;
    _state.mode = m;

    UIEventType e = UIEventNULL;
    String modifiers = "";
    if      (m == ModeNormal)           e = UIEventEnterNormalMode;
    else if (m == ModeSelecting)        e = UIEventEnterSelectingMode;
    else if (m == ModeLooking)          e = UIEventEnterLookingMode;
    else if (m == ModeMoveSurfaces)     e = UIEventEnterMoveSurfacesMode;
    else if (m == ModeMovePlane)        e = UIEventEnterMovePlaneMode;
    else if (m == ModeLevitate)         e = UIEventEnterMoveLevitateMode;
    else if (m == ModeInsert)           e = UIEventEnterInsertMode;
    else SignalError("Unknown Mode encountered.");

    if (e == UIEventEnterSelectingMode && _state.selectedModel != NULL)
    {
        modifiers = _state.selectedModel->model->name();
    }
    else if ((e == UIEventEnterMoveSurfacesMode || e == UIEventEnterMovePlaneMode || e == UIEventEnterMoveLevitateMode)
        && _state.selectedModel != NULL)
    {
        modifiers = _state.selectedModel->model->name();
        // TODO: Obtain position and pass as modifier
    }

    _state.scene.LogUIEvent(e, modifiers);

    return previousMode;
}

void App::Screenshot(const String &filename, const Vec2i &dim)
{
    UINT upscaleMult = 4;
    UINT width = dim.x;
    UINT height = dim.y;

    ClearSelection();

    D3D9ProtectRenderTarget protector(_state.device, true, true);
    D3D9RenderTargetSurface &surface = _state.globalAssets.scratchSurface;
    surface.Init(_state.GD, D3DFMT_A8R8G8B8, upscaleMult*width, upscaleMult*height);
    surface.SetAsRenderTarget(_state.GD);
    _state.device->Clear( 0, NULL, D3DCLEAR_ZBUFFER | D3DCLEAR_TARGET, RGBColor(255, 255, 255, 255), 1.0f, 0 );
    _state.scene.RenderNormal(_state,false);
    surface.CopySurfaceToOffScreen(_state.GD);
    auto capturedSurface = surface.OffScreenPlainSurface();

    Bitmap highRes;
    highRes.LoadFromSurface(capturedSurface);
    highRes.FlipBlueAndRed();

    Bitmap out(width, height);
    Rectangle2i sourceRect = Rectangle2i(0, 0, highRes.Width(), highRes.Height());
    Rectangle2i targetRect = Rectangle2i(0, 0, out.Width(), out.Height());
    highRes.StretchBltTo(out, targetRect, sourceRect, Bitmap::SamplingLinear);

    out.SavePNG(filename);
}

void App::SaveSceneThumbnail(const String &filename, const Vec2i &dim)
{
    LoadScene(filename);
    String basename = filename.RemoveSuffix(".scs");
    Screenshot(basename + String(".png"), dim);
    _state.picker.SaveModelNameGrid(_state, basename + String(".txt"), dim);
}