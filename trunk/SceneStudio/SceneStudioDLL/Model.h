struct RenderParameters
{
    RenderParameters()
    {
        vs = NULL;
        ps = NULL;
        transparencyStage = 0;
        pickerIDBase = 0;
        selectedBase = false;
        selectedChild = false;
    }

    D3D9VertexShader *vs;
    D3D9PixelShader  *ps;
    UINT transparencyStage;
    UINT pickerIDBase;
    bool selectedBase;
    bool selectedChild;
};

struct ModelGeometryEntry
{
    D3D9Mesh *mesh;
    D3D9Mesh *meshPick;
    D3D9Texture *texture;
    bool isLineMesh;
    Matrix4 transform;
    Vec4f transparent;
    Vec4f diffuse;
    float transparency;
};

struct AppState;

class Model
{
public:
    void Render(AppState &state, const Matrix4 &transform, RenderParameters &parameters) const;
    void LoadBinary(AppState &state, const String &modelName);
    void LoadDebugSquare(AppState &state);
    
    __forceinline Vector<ModelGeometryEntry>& geometry()
    {
        return _geometry;
    }
    __forceinline const Rectangle3f& BoundingBox() const
    {
        return _bbox;
    }
    __forceinline const String& name() const
    {
        return _name;
    }

    static void MakeTextureList(AppState &state, const String &modelName, Vector<UINT64> &textures);

private:
    Rectangle3f ComputeBoundingBox() const;

    String _name;
    Vector<ModelGeometryEntry> _geometry;
    Rectangle3f _bbox;
};
