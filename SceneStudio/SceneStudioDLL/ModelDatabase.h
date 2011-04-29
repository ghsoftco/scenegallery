struct AppState;

struct ModelInfo
{
    ModelInfo(const String &_hash)
    {
        hash = _hash;
    }
    String hash;
    String name;
    Vector<String> nameWords;
    Vector<String> tags;
    Vector<ModelInfo*> similarGeometry;
};

struct ArchitectureInfo
{
    String name;
    String camera;
};

class ModelDatabase
{
public:
    void Init(AppState &state);
    
    ModelInfo* GetModelInfo(const String &scene) const;
    ArchitectureInfo* GetArchitectureInfo(const String &scene) const;

    void TextSearch(const String &query, String &result);
    void ShapeSearch(const String &query, String &result);

    void GenerateModelTextureList(AppState &state) const;

private:
    void AddModelTextureList(AppState &state, const String &modelName, ofstream &file) const;

    void LoadArchitecture(AppState &state);
    void LoadValidScenes(AppState &state);
    void LoadSceneNames(AppState &state);
    void LoadSceneTags(AppState &state);
    void LoadDescriptorSet(AppState &state);

    Vector<ArchitectureInfo*> _architecture;
    set<String, String::LexicographicComparison> _validScenes;
    map<String, ModelInfo*, String::LexicographicComparison> _database;
};
