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
        public static bool debugMode = true;

        public static Dim sceneImgDim, exemplarImgDim;

        public static string stopwordList = "3d 3D a about above across after again against all almost alone along already also although always among an and another any anybody anyone anything anywhere are area areas around as ask asked asking asks at away b back backed backing backs be became because become becomes been before began behind being beings best better between big both but by c came can cannot case cases certain certainly clear clearly come could d did differ different differently do does done down down downed downing downs during e each early either end ended ending ends enough even evenly ever every everybody everyone everything everywhere f face faces fact facts far felt few find finds first for four from full fully further furthered furthering furthers g gave general generally get gets give given gives go going good goods got great greater greatest group grouped grouping groups h had has have having he her here herself high high high higher highest him himself his how however i if important in interest interested interesting interests into is it its itself j just k keep keeps kind knew know known knows l large largely last later latest least less let lets like likely long longer longest m made make making man many may me member members men might more most mostly mr mrs much must my myself n necessary need needed needing needs never new new newer newest next no nobody non noone not nothing now nowhere number numbers o of off often old older oldest on once one only open opened opening opens or order ordered ordering orders other others our out over p part parted parting parts per perhaps place places point pointed pointing points possible present presented presenting presents problem problems put puts q quite r rather really right right room rooms s said same saw say says second seconds see seem seemed seeming seems sees several shall she should show showed showing shows side sides since small smaller smallest so some somebody someone something somewhere state states still still such sure t take taken than that the their them then there therefore these they thing things think thinks this those though thought thoughts three through thus to today together too took toward turn turned turning turns two u under until up upon us use used uses v very w want wanted wanting wants was way ways we well wells went were what when where whether which while who whole whose why will with within without work worked working works would x y year years yet you young younger youngest your yours z";
        
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