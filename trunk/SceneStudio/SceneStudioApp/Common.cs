using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SceneStudioApp
{
    //
    // This struct must always match the version in SceneStudioDLL.h
    //
    //[StructLayout(LayoutKind.Sequential)]
    //public class SSState
    //{
    //    public UInt64 selectedModel;
    //}
    //[DllImport(SceneStudioDLL)] public static extern UInt32 SSQueryState(IntPtr d3dContext, [Out, MarshalAs(UnmanagedType.LPStruct)] SSState state);
    //SSState d3dState = new SSState();

    public class SceneEntry
    {
        public SceneEntry(string _hash, string _name)
        {
            hash = _hash;
            name = _name;
            image = Constants.webSceneImagesDirectory + _hash + ".jpg";
            textures = null;
            modelHashes = null;
            isExemplar = false;
        }
        public SceneEntry(string _filename, List<string> _modelHashes)
        {
            hash = Path.GetFullPath(_filename);
            name = Path.GetFileNameWithoutExtension(_filename);
            image = Path.ChangeExtension(hash, ".png");
            modelHashes = _modelHashes;
            isExemplar = true;
        }
        public string hash;
        public string name;
        public string image;
        public List<string> textures;
        public List<string> modelHashes;
        public bool isExemplar;
    };

    public class ArchitectureEntry
    {
        public ArchitectureEntry(String _name)
        {
            name = _name;
        }
        public String name;
    };

    //
    // This must always match the QueryType enum in App.h
    //
    public enum QueryType
    {
        QueryIsSceneDirty       = 0,
        QueryIsInsertMode       = 1,
        QuerySelectedModelName  = 2,
        QuerySceneFilename      = 3,
        QueryModelList          = 4,
        QuerySearchResults      = 5,
    };
}