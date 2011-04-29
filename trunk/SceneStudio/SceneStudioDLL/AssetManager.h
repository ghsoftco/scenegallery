struct AppState;

class AssetManager
{
public:
    void Init(AppState &state);

    void FreeMemory();
    void AddMesh(const String &name, D3D9Mesh *mesh);
    D3D9Mesh* GetMesh(const String &name, const FileCollection &meshes);
    D3D9Mesh* GetPickMesh(const String &name);
    Model* GetModel(const String &name);
    D3D9Texture* GetTexture(const String &name);

private:
    map<String, D3D9Mesh*,    String::LexicographicComparison> _meshes;
    map<String, D3D9Mesh*,    String::LexicographicComparison> _pickMeshes;
    map<String, D3D9Texture*, String::LexicographicComparison> _textures;
    map<String, Model*,       String::LexicographicComparison> _models;

    AppState *_state;
};
