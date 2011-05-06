﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Net;

namespace SceneStudioApp
{
    public static class Utility
    {
        public static void ReportCriticalError(String s)
        {
            //
            // Application.Exit() doesn't appear to actually exit the application.
            //
            //MessageBox.Show(s + "\nThe application will now exit.", "Fatal Error Encountered");
            //Application.Exit();

            MessageBox.Show(s, "Error");
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
            isExemplar = false;
        }
        public SceneEntry(string _filename, List<string> _modelHashes)
        {
            hash = Path.GetFileNameWithoutExtension(_filename);
            name = Path.GetFullPath(_filename);
            image = Path.ChangeExtension(Path.GetFullPath(_filename), ".png");
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
    public class Database
    {
        private Dictionary<string, SceneEntry> scenes;
        private Dictionary<string, SceneEntry> exemplars;
        private List<ArchitectureEntry> architectures;
        private WebClient webClient;
        private CacheDownloader cacheDownloader;

        public Database()
        {
        }
        public void Init(WebClient _webClient, CacheDownloader _cacheDownloader)
        {
            webClient = _webClient;
            cacheDownloader = _cacheDownloader;
            scenes = new Dictionary<string, SceneEntry>();
            exemplars = new Dictionary<string, SceneEntry>();
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
                    results.Add(getScene(hash));
            }
            return results;
        }
        public void addArchitecture(string name)
        {
            architectures.Add(new ArchitectureEntry(name));
            // Architecture does not have a name, but still needs a SceneEntry
            scenes.Add(name, new SceneEntry(name, "Architecture"));
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
        public void addExemplar(string filename, SceneEntry exemplar)
        {
            exemplars.Add(filename, exemplar);
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