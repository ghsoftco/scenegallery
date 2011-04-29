#include "Main.h"

SceneCamera::SceneCamera()
{
    _VecEye = Vec3f(0.0f,0.0f,0.0f);
    _VecUp = Vec3f(0.0f,1.0f,0.0f);
    _VecRight = Vec3f(-1.0f,0.0f,0.0f);
    _VecLookDir = Vec3f(0.0f,0.0f,1.0f);
    _AbsoluteUp = Vec3f(0.0f,1.0f,0.0f);

    _VecLookAt = _VecEye - _VecLookDir;

    Update();
}

SceneCamera::SceneCamera(const String &s)
{
    Vector<String> v = s.Partition(',');
    PersistentAssert(v.Length() == 15, "Incorrect number of components");
    _VecEye     = Vec3f(v[0 ].ConvertToFloat(), v[1 ].ConvertToFloat(), v[2 ].ConvertToFloat());
    _VecLookDir = Vec3f(v[3 ].ConvertToFloat(), v[4 ].ConvertToFloat(), v[5 ].ConvertToFloat());
    _VecUp      = Vec3f(v[6 ].ConvertToFloat(), v[7 ].ConvertToFloat(), v[8 ].ConvertToFloat());
    _VecRight   = Vec3f(v[9 ].ConvertToFloat(), v[10].ConvertToFloat(), v[11].ConvertToFloat());
    _AbsoluteUp = Vec3f(v[12].ConvertToFloat(), v[13].ConvertToFloat(), v[14].ConvertToFloat());

    _VecLookAt = _VecEye - _VecLookDir;
}

String SceneCamera::ToString()
{
    return _VecEye.ToString(',') + "," +
           _VecLookDir.ToString(',') + "," +
           _VecUp.ToString(',') + "," +
           _VecRight.ToString(',') + "," +
           _AbsoluteUp.ToString(',');
}

void SceneCamera::Reset(const Vec3f &Eye, const Vec3f &Up, const Vec3f &VecLookAt)
{
    _VecEye = Eye;
    _AbsoluteUp = Up;
    _VecLookDir = Vec3f::Normalize(_VecEye - VecLookAt);
    _VecRight = Vec3f::Normalize(Vec3f::Cross(_VecLookDir, _AbsoluteUp));
    _VecUp = Vec3f::Normalize(Vec3f::Cross(_VecRight, _VecLookDir));

    _VecLookAt = _VecEye - _VecLookDir;

    Update();
}

void SceneCamera::Update()
{
    _ViewMatrix = Matrix4::Camera(_VecEye, _VecLookDir, _VecUp, -_VecRight);
}

void SceneCamera::LookUp(float Theta)
{
    Matrix4 Rotation = Matrix4::Rotation(_VecRight, Theta);

    Vec3f lookdir = _VecEye - _VecLookAt;
    lookdir = Rotation.TransformPoint(lookdir); 
    _VecEye = _VecLookAt + lookdir;
    _VecLookDir = Vec3f::Normalize(lookdir);

    _VecUp = Rotation.TransformPoint(_VecUp);
    _VecRight = Rotation.TransformPoint(_VecRight);
    //_VecLookDir = Rotation.TransformPoint(_VecLookDir);
}

void SceneCamera::LookRight(float Theta)
{
    Matrix4 Rotation = Matrix4::Rotation(_AbsoluteUp, Theta);

    Vec3f lookdir = _VecEye - _VecLookAt;
    lookdir = Rotation.TransformPoint(lookdir); 
    _VecEye = _VecLookAt + lookdir;
    _VecLookDir = Vec3f::Normalize(lookdir);

    _VecUp = Rotation.TransformPoint(_VecUp);
    _VecRight = Rotation.TransformPoint(_VecRight);
    //_VecLookDir = Rotation.TransformPoint(_VecLookDir);
}

void SceneCamera::Move(float Distance)
{
    Vec3f disp = _VecLookDir * Distance;
    Vec3f oldeye = _VecEye;
    _VecEye += disp;

    // Check if eye moved past lookat
    // (Line segment containtment)
    Vec3f a, b, c;
    a = oldeye;
    b = _VecLookAt;
    c = _VecEye;
    float t = (c.x - a.x) / (b.x - a.x);
    if (t > 1)
    {
        // We moved past the lookat point, so it must be adjusted
        _VecLookAt = _VecEye + (_VecEye - _VecLookAt);
    }
}

void SceneCamera::Strafe(float Distance)
{
    _VecEye += _VecRight * Distance;
    _VecLookAt += _VecRight * Distance;
}

void SceneCamera::Jump(float Distance)
{
    _VecEye += _VecUp * Distance;
    _VecLookAt += _VecUp * Distance;
}
