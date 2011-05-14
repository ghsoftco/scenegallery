#include "Main.h"

void MeshSampler::Sample(const Mesh &m, UINT sampleCount, Vector<MeshSample> &samples)
{
    float totalArea = m.SurfaceArea();
    float areaScale = 1.0f / totalArea;
    if(totalArea == 0.0f)
    {
        samples.FreeMemory();
        return;
    }

    samples.Allocate(sampleCount);

    UINT samplingTriangleIndex = 0;
    float samplingTriangleAreaRatio = GetTriangleArea(m, samplingTriangleIndex) * areaScale;
    float accumulatedAreaRatio = samplingTriangleAreaRatio;

    for(UINT sampleIndex = 0; sampleIndex < sampleCount; sampleIndex++)
    {
        float intervalMin = float(sampleIndex) / float(sampleCount);
        float intervalMax = float(sampleIndex + 1) / float(sampleCount);
        float sampleValue = intervalMin + rnd() * (intervalMax - intervalMin);

        while(accumulatedAreaRatio < sampleValue && samplingTriangleIndex < m.FaceCount() - 1)
        {
            samplingTriangleIndex++;
            samplingTriangleAreaRatio = GetTriangleArea(m, samplingTriangleIndex) * areaScale;
            accumulatedAreaRatio += samplingTriangleAreaRatio;
        }

        float triangleValue = Utility::Bound( Math::LinearMap(accumulatedAreaRatio - samplingTriangleAreaRatio, accumulatedAreaRatio, 0.0f, 1.0f, sampleValue), 0.0f, 1.0f );
        samples[sampleIndex] = SampleTriangle(m, samplingTriangleIndex, triangleValue);
    }
}

void MeshSampler::Sample(const Vector<ModelGeometryEntry> &geometry, UINT sampleCount, Vector<MeshSample> &samples)
{
    Vector< pair<const BaseMesh *, Matrix4> > meshList;
    for(UINT geometryIndex = 0; geometryIndex < geometry.Length(); geometryIndex++)
    {
        meshList.PushEnd(pair<const BaseMesh *, Matrix4>(geometry[geometryIndex].mesh, geometry[geometryIndex].transform));
    }

    Mesh allMeshes;
    allMeshes.LoadMeshList(meshList);

    Sample(allMeshes, sampleCount, samples);
}

float MeshSampler::GetTriangleArea(const Mesh &m, UINT triangleIndex)
{
    const DWORD *indices = m.Indices();
    const MeshVertex *vertices = m.Vertices();

    Vec3f V[3];
    for(UINT vertexIndex = 0; vertexIndex < 3; vertexIndex++)
    {
        V[vertexIndex] = vertices[indices[triangleIndex * 3 + vertexIndex]].Pos;
    }
    return Math::TriangleArea(V[0], V[1], V[2]);
}

MeshSample MeshSampler::SampleTriangle(const Mesh &m, UINT triangleIndex, float sampleValue)
{
    const DWORD *indices = m.Indices();
    const MeshVertex *vertices = m.Vertices();

    Vec3f V[3];
    for(UINT vertexIndex = 0; vertexIndex < 3; vertexIndex++)
    {
        V[vertexIndex] = vertices[indices[triangleIndex * 3 + vertexIndex]].Pos;
    }

    MeshSample result;
    
    result.normal = Math::TriangleNormal(V[0], V[1], V[2]);

    Vec2f uv = StratifiedSample2D(sampleValue);
    if(uv.x + uv.y > 1.0f)
    {
        uv = Vec2f(1.0f - uv.y, 1.0f - uv.x);
    }

    result.pos = V[0] + (V[1] - V[0]) * uv.x + (V[2] - V[0]) * uv.y;

    return result;
}

//
// ask Justin what this is actually doing
//
Vec2f MeshSampler::StratifiedSample2D(float s, UINT depth)
{
    if(depth == 10)
    {
        return Vec2f(rnd(), rnd());
    }

    Vec2f basePoint;
    float baseValue;
    if(s < 0.25f)
    {
        baseValue = 0.0f;
        basePoint = Vec2f(0.0f, 0.0f);
    }
    else if(s < 0.5f)
    {
        baseValue = 0.25f;
        basePoint = Vec2f(0.5f, 0.0f);
    }
    else if(s < 0.75f)
    {
        baseValue = 0.5f;
        basePoint = Vec2f(0.0f, 0.5f);
    }
    else
    {
        baseValue = 0.75f;
        basePoint = Vec2f(0.5f, 0.5f);
    }

    return basePoint + StratifiedSample2D((s - baseValue) * 4.0f, depth + 1) * 0.5f;
}
