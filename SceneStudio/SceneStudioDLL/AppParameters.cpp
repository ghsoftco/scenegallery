#include "Main.h"

void AppParameters::Init(const String &parameterFilename)
{
    ParameterFile file(parameterFilename);

    baseAssetDirectory = file.GetRequiredString("baseAssetDirectory");
    if(runningLocalDirectoryTasks)
    {
        baseAssetDirectory = "C:\\SceneModeling\\Assets\\";
    }

    multisampleCount = file.GetUnsignedInteger("multisampleCount");

    textureDirectory  = baseAssetDirectory + String("textures_jpg/");
    modelDirectory    = baseAssetDirectory + String("models/");
    geometryDirectory = baseAssetDirectory + String("geometry/");
    shaderDirectory   = String("shaders/");
}
