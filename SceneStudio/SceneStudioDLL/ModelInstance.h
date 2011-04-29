
struct ModelInstance;
struct ModelRelationshipEdge
{
    ModelInstance *n0, *n1;
    Vec3f pos;
    Vec3f normal;
};

struct ModelInstance
{
    ModelInstance(ModelInstance *_parent, Model *_model)
    {
        ZeroMemory(this, sizeof(ModelInstance));
        parent = _parent;
        model = _model;
        scale = 1.0f;
        firstTransform = Matrix4::Identity();
        secondTransform = Matrix4::Identity();
        transform = Matrix4::Identity();
    }
    ~ModelInstance()
    {
        for(UINT childIndex = 0; childIndex < children.Length(); childIndex++)
        {
            delete children[childIndex];
        }
        children.FreeMemory();
    }
    
    void Render(AppState &state, RenderParameters &parameters) const;
    
    void SaveASCII(ofstream &file) const;
    void SaveBinary(OutputDataStream &stream) const;

    void MakeModelList(Vector<ModelInstance*> &modelList);
    void MakeRelationshipEdgeList(Vector<ModelRelationshipEdge> &edgeList);
    void MakePath(Vector<ModelInstance*> &path);

    bool GetPos(UINT geometryIndex, UINT faceIndex, const Vec2f &UV, Vec3f &pos, Vec3f &normal) const;

    void UpdateTransform();
    void ApplyCascadingTransform(const Matrix4 &preMatrix, const Matrix4 &postMatrix);
    void CascadingScale(float scaleFactor);
    void CascadingReset();

    Model *model;
    Matrix4 transform;
    Vector<ModelInstance*> children;

    UINT index;
    UINT baseId;
    ModelInstance *parent;

    UINT parentGeometryIndex;
    UINT parentTriangleIndex;
    Vec2f parentTriangleUV;
    Vec3f parentOffset;
    UINT cubeFace;
    float scale;
    Matrix4 firstTransform;
    Matrix4 secondTransform;
};
