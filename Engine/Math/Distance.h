/*
Distance.h
Written by Matthew Fisher

Code for computing geometric object distances.
http://www.geometrictools.com/LibMathematics/Distance/Distance.html
*/

namespace Math
{
    float DistancePointTriangleSq(const Vec3f &t0, const Vec3f &t1, const Vec3f &t2, const Vec3f &p);
    __forceinline float DistancePointTriangle(const Vec3f &t0, const Vec3f &t1, const Vec3f &t2, const Vec3f &p)
    {
        return sqrtf(DistancePointTriangleSq(t0, t1, t2, p));
    }
}
