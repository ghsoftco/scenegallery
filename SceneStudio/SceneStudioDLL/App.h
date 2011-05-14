enum ModeType
{
    ModeNormal          = 0,
    ModeSelecting       = 1,
    ModeLooking         = 2,
    ModeMoveSurfaces    = 3,
    ModeMovePlane       = 4,
    ModeLevitate        = 5,
    ModeInsert          = 6,
};

enum QueryType
{
    QueryIsSceneDirty           = 0,
    QueryIsInsertMode           = 1,
    QuerySelectedModelName      = 2,
    QuerySceneFilename          = 3,
    QueryModelList              = 4,
    QuerySearchResults          = 5,
};

struct GlobalAssets
{
    //
    // Textures
    //
    D3D9Texture             whiteTexture;
    
    //
    // Render Target Surfaces
    //
    D3D9RenderTargetSurface pickingSurfaceRGB;
    D3D9RenderTargetSurface pickingSurfaceUV;
    D3D9RenderTargetSurface scratchSurface;

    //
    // Meshes
    //
    D3D9Mesh                        selectionSphere;
    Vector<pair<D3D9Mesh, Vec3f> >  samplingSpheres;

    //
    // Shaders
    //
    struct
    {
        D3D9VertexShader    model;
        D3D9VertexShader    pickingA;
        D3D9VertexShader    pickingB;
        D3D9VertexShader    solid;
    } VertexShaders;
    struct
    {
        D3D9PixelShader     model;
        D3D9PixelShader     pickingA;
        D3D9PixelShader     pickingB;
        D3D9PixelShader     solid;
    } PixelShaders;

    //
    // Fonts
    //
    D3D9Font                debugFont;
};

struct AppState
{
    AppParameters parameters;
    
    D3D9GraphicsDevice GD;
    LPDIRECT3DDEVICE9 device;
    
    ApplicationWindow window;
    Vec2i dimensions;
    
    Scene scene;
    
    Picker picker;

    ModelDatabase modelDatabase;

    AssetManager assets;
    GlobalAssets globalAssets;

    UndoStack undoStack;

    SceneCamera camera;
    Matrix4 projection;
    Matrix4 viewProjection;

    ModeType mode;

    ModelInstance *selectedModel;
    UINT selectedGeometryIndex;
    UINT selectedFaceIndex;
    Vec2f selectedUV;

    Model *chosenModel;
    float chosenScale;
    float chosenRotation;
    UINT chosenFace;
    Matrix4 chosenSecondTransform;

    Model *copiedModel;
    float copiedScale;
    float copiedRotation;
    UINT copiedFace;
    Matrix4 copiedSecondTransform;

    bool sceneDirty;
    bool loadedCamera;
    bool showSurfaceSamples;
};

class App : public GraphicsDeviceCallbacks
{
public:
    void ReleaseAssets();
    void ResetAssets();

    void InitD3D(HWND window);
    void RenderFrame();
    void MouseMove(UINT button, int x, int y, int xDelta, int yDelta);
    void MouseDown(UINT button, int x, int y);
    void MouseUp(UINT button);
    void MouseWheel(int delta);
    void KeyPress(int key, bool shift, bool ctrl);
    void ModelChosen(const char *model);
    UINT ProcessCommand(const char *command);
    
    const char* QueryString(QueryType q);
    int QueryInteger(QueryType q);

    __forceinline Mutex& mutex()
    {
        return _mutex;
    }

    void Status(const String &s, RGBColor c);

private:
    void RenderStatusText();

    void ResetCamera();
    void UpdateCamera();
    void DollyCamera(float dist);
    
    void ClearSelection();
    void EnterInsertMode();
    void FlipFace(int faceDelta);
    void Rotate(float rotate);
    void Scale(float scale);
    void ResetRotation();
    void Copy();
    void Paste();
    void Undo();
    void DeleteSelectedModel();
    void InsertModel();

    void TextSearch(const String &query);
    void ShapeSearch(const String &query);

    UINT LoadScene(const String &filename);
    UINT SaveScene(const String &filename);
    UINT LoadModelList(const String &filename);
    UINT NewScene(const String &scene);

    void Screenshot(const String &filename, const Vec2i &dim);
    void SaveSceneThumbnail(const String &filename, const Vec2i &dim);

    ModeType SetMode(ModeType m);

    void ExportAllScenes();
    void CreateSceneThumbnails(const String &sceneDirectory, const Vec2i &dim);
    
    // Debugging
    void ReportString(const String& s);
    void ReportMode();
    void CreateSurfaceSamplingSpheres();

    Mutex _mutex;
    AppState _state;
    UINT _consoleY;
    String _searchResults;
    String _sceneModelList;
    Vector<BYTE> _debugDataStream;
};
