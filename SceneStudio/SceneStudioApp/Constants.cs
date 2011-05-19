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
            sceneImgDim = new Dim(200, 150);
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

        public static string architectureNameTag = "Architecture";

        public static bool restrictModelSearchToExemplarContents = true;

        public static Dim sceneImgDim, exemplarImgDim;

        //     $('#clickPosition').text((e.clientX - offset.left) + "", "" + (e.clientY - offset.top));
        public static string HtmlHeaderExemplarBrowser = @"
<!DOCTYPE html>
<html>
<head>
<script class=""jsbin"" src=""http://ajax.googleapis.com/ajax/libs/jquery/1.5.2/jquery.min.js""></script>
<script type=""text/javascript"">
$(document).ready(function() {
  $('.box').click(function(e) {
    var offset = $(this).offset();
    var x = (e.pageX - offset.left);
    var y = (e.pageY - offset.top);
    var hash = $(this).find('a').attr('href');
    var index = $(this).index();
    window.external.ReportClick(hash, x, y, index);
  });
});
</script>
<meta charset=utf-8 />
<title>exemplarBrowser</title>
<script src=""http://html5shiv.googlecode.com/svn/trunk/html5.js""></script>
<style type=""text/css"">
  div { float: left; }
  div img { display: block; }
</style>
</head>
";
    }
}