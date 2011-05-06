using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace SceneStudioApp
{
    class Constants
    {
        public static void Init()
        {
            cacheBaseDirectory = Path.GetDirectoryName(Application.ExecutablePath) + @"\cache\";
            cacheSceneNamesFilename = cacheBaseDirectory + "SceneNames.txt";
            cacheSceneTexturesFilename = cacheBaseDirectory + "SceneTextures.txt";
            cacheArchitectureListFilename = cacheBaseDirectory + "ArchitectureList.txt";
            sceneImgDim = new Dim(168, 126);
            exemplarImgDim = new Dim(400, 400);
        }

        public const string baseWebAddress = "http://graphics.stanford.edu/projects/SceneStudio/SceneModeling/";
        public const string localSceneImagesDirectory = "C:/SceneModeling/Database/SceneImagesB/";
        public const string webSceneImagesDirectory = baseWebAddress + "Database/SceneImagesB/";
        public const string localExemplarsDirectory =  "../../Scenes/exemplars/";

        public const string localSceneNamesFilename = "C:/SceneModeling/Assets/SceneNames.txt";
        public const string localSceneTexturesFilename = "C:/SceneModeling/Assets/SceneTextures.txt";

        public static string cacheBaseDirectory;
        public static string cacheSceneNamesFilename;
        public static string cacheSceneTexturesFilename;
        public static string cacheArchitectureListFilename;

        public static Dim sceneImgDim, exemplarImgDim;
    }
}
