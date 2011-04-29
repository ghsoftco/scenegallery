class Scene
{
public:
    Scene()
    {
        _root = NULL;
    }
    void FreeMemory();
    void Reset(AppState &state, const String &baseModelName);

    void SaveRelationshipGraphASCII(AppState &state, const String &filename);
    
    UINT SaveSceneASCII(AppState &state, const String &filename);
    UINT LoadSceneASCII(AppState &state, const String &filename);

    void SaveSceneBinary(AppState &state, OutputDataStream &stream);
    void LoadSceneBinary(AppState &state, InputDataStream &stream);
    
    void UpdateModelIDs(AppState &state);
    
    void RenderNormal(AppState &state, bool renderChosen);
    void RenderPickingA(AppState &state);
    void RenderPickingB(AppState &state);
    void RenderSphere(AppState &state, const Vec3f &pos, float radius);
    void RenderSelectionSphere(AppState &state);
    
    Matrix4 MakeFirstTransform(AppState &state, const Model &model, UINT face, const Vec3f &pos, const Vec3f &normal);
    
    bool RelocateSelectedModel(AppState &state);
    bool InsertChosenModel(AppState &state, ModelInstance*& insertedModel);
    bool RemoveSelectedModel(AppState &state, bool deleteModel);
    ModelInstance* GetSelectedModel(AppState &state);

    void RenderDebugBBox(AppState &state);
    
    __forceinline const Rectangle3f& BoundingBox() const
    {
        return _bbox;
    }
    __forceinline const String& Filename() const
    {
        return _filename;
    }
    __forceinline const String& BaseModelName() const
    {
        return _root->model->name();
    }
    __forceinline void AddSearchTerm(const String &s)
    {
        _searchTerms.PushEnd(s);
    }
    __forceinline void LogUIEvent(UIEventType eventType, String eventData)
    {
        _uiEventLog.Log(eventType, eventData);
    }

    static UINT MakeModelList(AppState &state, const String &filename, Vector<String> &models);

private:
    void Validate();

    bool GetSelectedPos(AppState &state, Vec3f &pos, Vec3f &normal);

    friend struct ModelInstance;

    String _filename;
    Rectangle3f _bbox;
    ModelInstance *_root;
    Vector<ModelInstance*> _modelList;
    Vector<ModelInstance*> _modelFromGeometryIndex;

    Vector<String> _searchTerms;
    UIEventLogger _uiEventLog;
};
