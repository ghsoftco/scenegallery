class SceneCamera
{
public:
    SceneCamera();
    SceneCamera(const String &s);
    void Reset(const Vec3f &Eye, const Vec3f &Up, const Vec3f &VecLookAt);    //resets tha camera to the given configuration
    
    void Update();

    void LookUp(float Theta);
    void LookRight(float Theta);
    void Move(float Distance);
    void Strafe(float Distance);
    void Jump(float Distance);

    String ToString();

    __forceinline Matrix4& ViewMatrix()
    {
        return _ViewMatrix;
    }
    __forceinline const Matrix4& ViewMatrix() const
    {
        return _ViewMatrix;
    }
    __forceinline Vec3f& VecEye()
    {
        return _VecEye;
    }
    __forceinline const Vec3f& VecEye() const
    {
        return _VecEye;
    }
    __forceinline Vec3f& VecLookDir()
    {
        return _VecLookDir;
    }
    __forceinline const Vec3f& VecLookDir() const
    {
        return _VecLookDir;
    }
    __forceinline Vec3f& VecUp()
    {
        return _VecUp;
    }
    __forceinline const Vec3f& VecUp() const
    {
        return _VecUp;
    }
    __forceinline Vec3f& VecRight()
    {
        return _VecRight;
    }
    __forceinline const Vec3f& VecRight() const
    {
        return _VecRight;
    }
    __forceinline Vec3f& AbsoluteUp()
    {
        return _AbsoluteUp;
    }
    __forceinline const Vec3f& AbsoluteUp() const
    {
        return _AbsoluteUp;
    }

private:
    Vec3f _VecEye, _VecLookDir, _VecUp, _VecRight, _AbsoluteUp;
    Matrix4 _ViewMatrix;
    Vec3f _VecLookAt;
};
