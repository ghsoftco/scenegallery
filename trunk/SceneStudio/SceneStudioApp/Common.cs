using System;
using System.Collections.Generic;
using System.Text;

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
        public SceneEntry(String _hash, String _name)
        {
            hash = _hash;
            name = _name;
            textures = null;
        }
        public String hash;
        public String name;
        public List<String> textures;
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
