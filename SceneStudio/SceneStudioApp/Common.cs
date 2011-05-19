using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Web.UI;
using System.Drawing;

namespace SceneStudioApp
{
    public static class Utility
    {
        public static void showThumbnails(WebBrowser webBrowser, List<SceneEntry> entries, Dim dim)
        {
            StringWriter sw = new StringWriter();
            sw.Write(Constants.HtmlHeaderExemplarBrowser);

            if (entries.Count == 0)
            {
                sw.WriteLine("<h1>No results found.</h1>");
            }
            else
            {
                using (HtmlTextWriter writer = new HtmlTextWriter(sw))
                {
                    foreach (SceneEntry e in entries)
                    {
                        e.Render(writer, dim);
                    }

                }
            }

            sw.WriteLine("</body></html>");
            webBrowser.DocumentText = sw.ToString();
        }
        public static Image CaptureWindow(Form form)
        {
            Rectangle bounds = form.Bounds;
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                }
                return bitmap;
            }
        }
        public static void ReportCriticalError(String s)
        {
            //
            // Application.Exit() doesn't appear to actually exit the application.
            //
            //MessageBox.Show(s + "\nThe application will now exit.", "Fatal Error Encountered");
            //Application.Exit();

            MessageBox.Show(s, "Error");
        }
        public static string UIEventTypeToString(UIEventType input)
        {
            switch (input)
            {
                case UIEventType.UIEventClickExemplar:
                    return "Ce";
                case UIEventType.UIEventClickModel:
                    return "Cm";
                case UIEventType.UIEventKeywordSearchExemplar:
                    return "Se";
                case UIEventType.UIEventKeywordSearchModel:
                    return "Sm";
                default:
                    return "Unknown";
            }
        }
    }
    public struct Dim
    {
        public Dim(int _w, int _h)
        {
            w = _w;
            h = _h;
        }
        public int w;
        public int h;
    }
    public class SceneEntry
    {
        public SceneEntry(string _hash, string _name)
        {
            hash = _hash;
            name = _name;
            image = Constants.webSceneImagesDirectory + _hash + ".jpg";
            textures = null;
            modelHashes = null;
            tags = _name;
            hashMap = null;
            isExemplar = false;
        }
        public SceneEntry(string _filename, List<string> _modelHashes, string _tags, SceneHashMap _hashMap)
        {
            hash = Path.GetFileNameWithoutExtension(_filename);
            name = Path.GetFullPath(_filename);
            image = Path.ChangeExtension(Path.GetFullPath(_filename), ".png");
            modelHashes = _modelHashes;
            tags = _tags;
            hashMap = _hashMap;
            isExemplar = true;
        }
        public void Render(HtmlTextWriter writer, Dim dim)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "box");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);   // Begin Div

            writer.AddAttribute(HtmlTextWriterAttribute.Href, hash);
            writer.RenderBeginTag(HtmlTextWriterTag.A);     // Begin A

            writer.AddAttribute(HtmlTextWriterAttribute.Src, image);
            writer.AddAttribute(HtmlTextWriterAttribute.Width, dim.w.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Height, dim.h.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Alt, "");

            writer.RenderBeginTag(HtmlTextWriterTag.Img);   // Begin Img
            writer.RenderEndTag(); // End Img

            //writer.AddAttribute(HtmlTextWriterAttribute.Id, "clickPosition");
            //writer.RenderBeginTag(HtmlTextWriterTag.P); // Begin P
            //writer.RenderEndTag(); // End P

            writer.RenderEndTag(); // End A

            writer.RenderEndTag(); // End Div
        }

        public string hash;
        public string name;
        public string image;
        public List<string> textures;
        public List<string> modelHashes;
        public string tags; // comma separated list of tags of all models in scene
        public SceneHashMap hashMap;
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
    public class Database
    {
        Dictionary<string, SceneEntry> scenes;
        Dictionary<string, SceneEntry> exemplars;
        Dictionary<string, SceneEntry> exemplarModels;
        Dictionary<string, List<SceneEntry>> exemplarModelToInstanceScene;
        List<ArchitectureEntry> architectures;
        WebClient webClient;
        CacheDownloader cacheDownloader;

        public Database()
        {
        }
        public void Init(WebClient _webClient, CacheDownloader _cacheDownloader)
        {
            webClient = _webClient;
            cacheDownloader = _cacheDownloader;
            scenes = new Dictionary<string, SceneEntry>();
            exemplars = new Dictionary<string, SceneEntry>();
            exemplarModels = new Dictionary<string, SceneEntry>();
            exemplarModelToInstanceScene = new Dictionary<string, List<SceneEntry>>();
            architectures = new List<ArchitectureEntry>();

            PopulateCache();
            LoadSceneInfo();
            PopulateCacheArchitecture();
        }

        public bool containsSceneHash(string hash)
        {
            return scenes.ContainsKey(hash);
        }
        public SceneEntry getScene(string hash)
        {
            SceneEntry scene;
            if (!scenes.TryGetValue(hash, out scene))
            {
                throw new Exception("Unknown scene hash");
            }
            return scene;
        }
        public List<SceneEntry> getScenes()
        {
            return new List<SceneEntry>(scenes.Values);
        }
        public List<SceneEntry> filterScenesByKeyword(string keyword)
        {
            List<SceneEntry> matching = new List<SceneEntry>();
            foreach (SceneEntry scene in scenes.Values)
            {
                if (scene.tags.Contains(keyword)) matching.Add(scene);
            }
            return matching;
        }
        public List<SceneEntry> filterScenesByKeyword(string keyword, char separator)
        {
            List<SceneEntry> matching = new List<SceneEntry>();
            if (keyword == null) return matching;
            foreach (string token in keyword.Split(separator))
            {
                if (Constants.stopwordList.Contains(token)) continue;
                foreach (SceneEntry scene in scenes.Values)
                {
                    if (!matching.Contains(scene) && scene.tags.Contains(token))
                        matching.Add(scene);
                }
            }
            return matching;
        }
        public void addScene(string hash, SceneEntry scene)
        {
            scenes.Add(hash, scene);
        }
        public bool removeScene(string hash)
        {
            return scenes.Remove(hash);
        }
        public List<SceneEntry> getScenesFromHashes(List<string> hashes)
        {
            List<SceneEntry> results = new List<SceneEntry>();
            if (hashes.Count != 0) foreach (string hash in hashes)
            {
                SceneEntry scene = getScene(hash);
                if (!(scene.name == Constants.architectureNameTag))
                {
                    results.Add(scene);
                }
            }
            return results;
        }
        public void addArchitecture(string name)
        {
            architectures.Add(new ArchitectureEntry(name));
            // Architecture does not have a name, but still needs a SceneEntry
            scenes.Add(name, new SceneEntry(name, Constants.architectureNameTag));
        }
        public SceneEntry getExemplar(string filename)
        {
            SceneEntry exemplar;
            if (!exemplars.TryGetValue(filename, out exemplar))
            {
                throw new Exception("Unknown exemplar filename");
            }
            return exemplar;
        }
        public List<SceneEntry> getExemplars()
        {
            return new List<SceneEntry>(exemplars.Values);
        }
        public List<SceneEntry> getExemplarModels()
        {
            return new List<SceneEntry>(exemplarModels.Values);
        }
        public List<SceneEntry> getInstanceScenesOfModel(string hash)
        {
            return exemplarModelToInstanceScene[hash];
        }
        public List<SceneEntry> filterExemplarModelsByKeyword(string keyword)
        {
            List<SceneEntry> matching = new List<SceneEntry>();
            foreach (SceneEntry scene in exemplarModels.Values)
            {
                if (scene.name.Contains(keyword)) matching.Add(scene);
            }
            return matching;
        }
        public List<SceneEntry> filterExemplarModelsByKeyword(string keyword, char separator)
        {
            List<SceneEntry> matching = new List<SceneEntry>();
            if (keyword == null) return matching;

            foreach (string token in keyword.Split(separator))
            {
                if (Constants.stopwordList.Contains(token)) continue;
                foreach (SceneEntry scene in exemplarModels.Values)
                {
                    if (!matching.Contains(scene) && scene.name.Contains(keyword)) matching.Add(scene);
                }
            }
            return matching;
        }
        public List<SceneEntry> filterExemplarsByKeyword(string keyword)
        {
            List<SceneEntry> matching = new List<SceneEntry>();
            foreach (SceneEntry scene in exemplars.Values)
            {
                if (scene.tags.Contains(keyword)) matching.Add(scene);
            }
            return matching;
        }
        public void addExemplar(string filename, SceneEntry exemplar)
        {
            exemplars.Add(filename, exemplar);
            List<SceneEntry> models = getScenesFromHashes(exemplar.modelHashes);
            foreach (SceneEntry model in models)
            {
                if (model.name != Constants.architectureNameTag && !exemplarModels.ContainsKey(model.hash))
                {
                    exemplarModels.Add(model.hash, model);
                    if (exemplarModelToInstanceScene.ContainsKey(model.hash))
                    {
                        exemplarModelToInstanceScene[model.hash].Add(exemplar);
                    }
                    else
                    {
                        List<SceneEntry> list = new List<SceneEntry>();
                        list.Add(exemplar);
                        exemplarModelToInstanceScene.Add(model.hash, list);
                    }
                }
            }
        }

        private void LoadSceneInfo()
        {
            foreach (string line in File.ReadAllLines(Constants.cacheSceneNamesFilename))
            {
                String[] curWords = line.Split('\t');
                if (curWords.Length == 2)
                {
                    addScene(curWords[0], new SceneEntry(curWords[0], curWords[1]));
                }
            }

            foreach (string line in File.ReadAllLines(Constants.cacheArchitectureListFilename))
            {
                String[] curWords = line.Split('\t');
                if (curWords.Length == 2)
                {
                    addArchitecture(curWords[0]);
                }
            }

            // Load textures into SceneEntries
            foreach (string line in File.ReadAllLines(Constants.cacheSceneTexturesFilename))
            {
                string[] curWords = line.Split(' ');
                if (curWords.Length >= 2 && containsSceneHash(curWords[0]))
                {
                    SceneEntry curEntry = getScene(curWords[0]);
                    curEntry.textures = new List<String>();
                    for (int textureIndex = 1; textureIndex < curWords.Length; textureIndex++)
                    {
                        curEntry.textures.Add(curWords[textureIndex]);
                    }
                }
            }
        }
        private void PopulateCache()
        {
            try
            {
                Directory.CreateDirectory(Constants.cacheBaseDirectory);
                Directory.CreateDirectory(Constants.cacheBaseDirectory + "Models");
                Directory.CreateDirectory(Constants.cacheBaseDirectory + "Geometry");
                Directory.CreateDirectory(Constants.cacheBaseDirectory + "Textures_jpg");
            }
            catch (Exception e)
            {
                Utility.ReportCriticalError("Failed to create cache directories.\n" + e.ToString());
                return;
            }
            try
            {
                webClient.DownloadFile(Constants.baseWebAddress + "Assets/SceneNames.txt", Constants.cacheBaseDirectory + "SceneNames.txt");
                webClient.DownloadFile(Constants.baseWebAddress + "Assets/SceneTags.txt", Constants.cacheBaseDirectory + "SceneTags.txt");
                webClient.DownloadFile(Constants.baseWebAddress + "Assets/SceneTextures.txt", Constants.cacheBaseDirectory + "SceneTextures.txt");
                webClient.DownloadFile(Constants.baseWebAddress + "Assets/ArchitectureList.txt", Constants.cacheBaseDirectory + "ArchitectureList.txt");
                webClient.DownloadFile(Constants.baseWebAddress + "Assets/DescriptorSet.dat", Constants.cacheBaseDirectory + "DescriptorSet.dat");
            }
            catch (Exception e)
            {
                Utility.ReportCriticalError("Failed to download scene information files. Make sure you have a working internet connection.\n" + e.ToString());
            }
        }
        private void PopulateCacheArchitecture()
        {
            try
            {
                foreach (ArchitectureEntry architecture in architectures)
                {
                    cacheDownloader.NewScene(getScene(architecture.name));
                    cacheDownloader.DownloadFilesImmediate(webClient);
                }
            }
            catch (Exception e)
            {
                Utility.ReportCriticalError("Failed to download scene architecture files. Make sure you have a working internet connection.\n" + e.ToString());
            }
        }
    };
    public class SceneHashMap
    {
        public SceneHashMap(string filename, Database database)
        {
            _sceneMap = LoadSceneHashMap(filename, database);
        }

        public int GetLength(int dimension)
        {
            return _sceneMap.GetLength(dimension);
        }

        public SceneEntry this[int iRow, int iCol]
        {
            get
            {
                return _sceneMap[iRow, iCol];
            }
            set
            {
                _sceneMap[iRow, iCol] = value;
            }
        }

        private static SceneEntry[,] LoadSceneHashMap(string filename, Database database)
        {
            SceneEntry[,] map;

            using (TextReader rdr = new StreamReader(filename))
            {
                string line;
                line = rdr.ReadLine();
                string[] dim = line.Split('\t');
                uint rows = uint.Parse(dim[0]);
                uint cols = uint.Parse(dim[1]);
                map = new SceneEntry[rows, cols];

                for (int iRow = 0; iRow < rows; iRow++)
                {
                    line = rdr.ReadLine().Trim();
                    if (line == null && iRow < rows-1) throw new Exception("Number of rows less than stated.");

                    string[] row = line.Split('\t');
                    if (row.Length != cols) throw new Exception("Row length does not match number of columns.");
                    for (int iCol = 0; iCol < cols; iCol++)
                    {
                        string hash = row[iCol];
                        if (database.containsSceneHash(hash))
                        {
                            map[iRow,iCol] = database.getScene(hash);
                        }
                    }
                }

            }

            return map;
        }

        private SceneEntry[,] _sceneMap;
    }
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
    public enum BrowserMode
    {
        ExemplarsAvailable = 0,
        ExemplarsNotAvailable = 1,
    };

    // Must match the definitions in UIEvent.h
    public enum UIEventType
    {
        UIEventKeywordSearchExemplar = 0,
        UIEventKeywordSearchModel = 1,
        UIEventClickModel = 2,
        UIEventClickExemplar = 3,
    };
}