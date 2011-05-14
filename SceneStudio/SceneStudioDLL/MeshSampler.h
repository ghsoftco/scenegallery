struct MeshSample
{
    Vec3f pos;
    Vec3f normal;
};

class MeshSampler
{
public:
    static void Sample(const Mesh &m, UINT sampleCount, Vector<MeshSample> &samples);
    static void Sample(const Vector<ModelGeometryEntry> &geometry, UINT sampleCount, Vector<MeshSample> &samples);

private:
    static float GetTriangleArea(const Mesh &m, UINT triangleIndex);
    static MeshSample SampleTriangle(const Mesh &m, UINT triangleIndex, float sampleValue);
    static Vec2f StratifiedSample2D(float s, UINT depth = 0);
};