/*
Distance.cpp
Written by Matthew Fisher

Code for computing geometric object distances.
*/

float Math::DistancePointTriangleSq(const Vec3f &t0, const Vec3f &t1, const Vec3f &t2, const Vec3f &p)
{
    Vec3f diff = t0 - p;
    Vec3f edge0 = t1 - t0;
    Vec3f edge1 = t2 - t0;
    float a00 = edge0.LengthSq();
    float a01 = Vec3f::Dot(edge0, edge1);
    float a11 = edge1.LengthSq();
    float b0 = Vec3f::Dot(diff, edge0);
    float b1 = Vec3f::Dot(diff, edge1);
    float c = diff.LengthSq();
    float det = Math::Abs(a00*a11 - a01*a01);
    float s = a01*b1 - a11*b0;
    float t = a01*b0 - a00*b1;
    float sqrDistance;

    if (s + t <= det)
    {
        if (s < (float)0)
        {
            if (t < (float)0)  // region 4
            {
                if (b0 < (float)0)
                {
                    t = (float)0;
                    if (-b0 >= a00)
                    {
                        s = (float)1;
                        sqrDistance = a00 + ((float)2)*b0 + c;
                    }
                    else
                    {
                        s = -b0/a00;
                        sqrDistance = b0*s + c;
                    }
                }
                else
                {
                    s = (float)0;
                    if (b1 >= (float)0)
                    {
                        t = (float)0;
                        sqrDistance = c;
                    }
                    else if (-b1 >= a11)
                    {
                        t = (float)1;
                        sqrDistance = a11 + ((float)2)*b1 + c;
                    }
                    else
                    {
                        t = -b1/a11;
                        sqrDistance = b1*t + c;
                    }
                }
            }
            else  // region 3
            {
                s = (float)0;
                if (b1 >= (float)0)
                {
                    t = (float)0;
                    sqrDistance = c;
                }
                else if (-b1 >= a11)
                {
                    t = (float)1;
                    sqrDistance = a11 + ((float)2)*b1 + c;
                }
                else
                {
                    t = -b1/a11;
                    sqrDistance = b1*t + c;
                }
            }
        }
        else if (t < (float)0)  // region 5
        {
            t = (float)0;
            if (b0 >= (float)0)
            {
                s = (float)0;
                sqrDistance = c;
            }
            else if (-b0 >= a00)
            {
                s = (float)1;
                sqrDistance = a00 + ((float)2)*b0 + c;
            }
            else
            {
                s = -b0/a00;
                sqrDistance = b0*s + c;
            }
        }
        else  // region 0
        {
            // minimum at interior point
            float invDet = ((float)1)/det;
            s *= invDet;
            t *= invDet;
            sqrDistance = s*(a00*s + a01*t + ((float)2)*b0) +
                t*(a01*s + a11*t + ((float)2)*b1) + c;
        }
    }
    else
    {
        float tmp0, tmp1, numer, denom;

        if (s < (float)0)  // region 2
        {
            tmp0 = a01 + b0;
            tmp1 = a11 + b1;
            if (tmp1 > tmp0)
            {
                numer = tmp1 - tmp0;
                denom = a00 - ((float)2)*a01 + a11;
                if (numer >= denom)
                {
                    s = (float)1;
                    t = (float)0;
                    sqrDistance = a00 + ((float)2)*b0 + c;
                }
                else
                {
                    s = numer/denom;
                    t = (float)1 - s;
                    sqrDistance = s*(a00*s + a01*t + ((float)2)*b0) +
                        t*(a01*s + a11*t + ((float)2)*b1) + c;
                }
            }
            else
            {
                s = (float)0;
                if (tmp1 <= (float)0)
                {
                    t = (float)1;
                    sqrDistance = a11 + ((float)2)*b1 + c;
                }
                else if (b1 >= (float)0)
                {
                    t = (float)0;
                    sqrDistance = c;
                }
                else
                {
                    t = -b1/a11;
                    sqrDistance = b1*t + c;
                }
            }
        }
        else if (t < (float)0)  // region 6
        {
            tmp0 = a01 + b1;
            tmp1 = a00 + b0;
            if (tmp1 > tmp0)
            {
                numer = tmp1 - tmp0;
                denom = a00 - ((float)2)*a01 + a11;
                if (numer >= denom)
                {
                    t = (float)1;
                    s = (float)0;
                    sqrDistance = a11 + ((float)2)*b1 + c;
                }
                else
                {
                    t = numer/denom;
                    s = (float)1 - t;
                    sqrDistance = s*(a00*s + a01*t + ((float)2)*b0) +
                        t*(a01*s + a11*t + ((float)2)*b1) + c;
                }
            }
            else
            {
                t = (float)0;
                if (tmp1 <= (float)0)
                {
                    s = (float)1;
                    sqrDistance = a00 + ((float)2)*b0 + c;
                }
                else if (b0 >= (float)0)
                {
                    s = (float)0;
                    sqrDistance = c;
                }
                else
                {
                    s = -b0/a00;
                    sqrDistance = b0*s + c;
                }
            }
        }
        else  // region 1
        {
            numer = a11 + b1 - a01 - b0;
            if (numer <= (float)0)
            {
                s = (float)0;
                t = (float)1;
                sqrDistance = a11 + ((float)2)*b1 + c;
            }
            else
            {
                denom = a00 - ((float)2)*a01 + a11;
                if (numer >= denom)
                {
                    s = (float)1;
                    t = (float)0;
                    sqrDistance = a00 + ((float)2)*b0 + c;
                }
                else
                {
                    s = numer/denom;
                    t = (float)1 - s;
                    sqrDistance = s*(a00*s + a01*t + ((float)2)*b0) +
                        t*(a01*s + a11*t + ((float)2)*b1) + c;
                }
            }
        }
    }

    // Account for numerical round-off error.
    if (sqrDistance < 0.0f)
    {
        sqrDistance = 0.0f;
    }

    //mClosestPoint0 = *mPoint;
    //mClosestPoint1 = mTriangle->V[0] + s*edge0 + t*edge1;
    //mTriangleBary[1] = s;
    //mTriangleBary[2] = t;
    //mTriangleBary[0] = (float)1 - s - t;
    return sqrDistance;
}
