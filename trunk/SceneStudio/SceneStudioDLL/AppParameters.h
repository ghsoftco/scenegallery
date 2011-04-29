struct AppParameters
{
    void Init(const String &parameterFilename);

    UINT multisampleCount;
    String baseAssetDirectory;
    String textureDirectory;
    String modelDirectory;
    String geometryDirectory;
    String shaderDirectory;
};
