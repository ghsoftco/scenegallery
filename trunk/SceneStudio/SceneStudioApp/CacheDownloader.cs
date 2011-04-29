using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Windows.Forms;

namespace SceneStudioApp
{
    class CacheDownloader
    {
        public enum DownloadStatus
        {
            InProgress,
            Done,
            Failed,
        };

        private enum FileState
        {
            Unknown,
            Ready,
            Pending,
            Downloading,
            Failed
        };
        
        private class FileEntry
        {
            public FileEntry(String _name)
            {
                name = _name;
                state = FileState.Unknown;
            }
            public String name;
            public FileState state;
        };
        
        public void NewScene(SceneEntry _scene)
        {
            scene = _scene;
            files = new List<FileEntry>();
            files.Add(new FileEntry("Models/" + scene.hash));
            files.Add(new FileEntry("Geometry/" + scene.hash));
            if (scene.textures != null)
            {
                foreach (String textureName in scene.textures)
                {
                    files.Add(new FileEntry("Textures_jpg/" + textureName + ".jpg"));
                }
            }
        }

        public void DownloadFilesImmediate(WebClient client)
        {
            foreach (FileEntry entry in files)
            {
                if (entry.state == FileState.Unknown)
                {
                    entry.state = QueryFileState(entry.name);
                }
                if (entry.state == FileState.Pending)
                {
                    try
                    {
                        client.DownloadFile(Constants.baseWebAddress + "Assets/" + entry.name, Constants.cacheBaseDirectory + entry.name);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to download a required file from the server: " + Constants.baseWebAddress + "Assets/" + entry.name + "\n" + ex.ToString(), "Error");
                        return;
                    }
                    entry.state = FileState.Ready;
                }
            }
        }

        public void DownloadFiles(WebClient client)
        {
            if (client.IsBusy)
            {
                return;
            }
            foreach (FileEntry entry in files)
            {
                if (entry.state == FileState.Unknown)
                {
                    entry.state = QueryFileState(entry.name);
                }
                if(entry.state == FileState.Pending)
                {
                    client.CancelAsync();
                    client.DownloadFileCompleted += (sender, args) => DownloadFileCompleted(client, args);
                    client.DownloadFileAsync(new Uri(Constants.baseWebAddress + "Assets/" + entry.name), Constants.cacheBaseDirectory + entry.name, entry);
                    entry.state = FileState.Downloading;
                    return;
                }
            }
        }

        public DownloadStatus Status(out String description)
        {
            description = null;
            foreach (FileEntry entry in files)
            {
                if (entry.state == FileState.Unknown || entry.state == FileState.Failed)
                {
                    return DownloadStatus.Failed;
                }
                if (entry.state == FileState.Pending || entry.state == FileState.Downloading)
                {
                    description = "Downloading " + entry.name + "...";
                    return DownloadStatus.InProgress;
                }
            }
            return DownloadStatus.Done;
        }

        private void DownloadFileCompleted(WebClient client, AsyncCompletedEventArgs args)
        {
            FileEntry entry = (FileEntry)args.UserState;
            if (args.Cancelled || args.Error != null)
            {
                entry.state = FileState.Failed;
            }
            else
            {
                entry.state = FileState.Ready;
                DownloadFiles(client);
            }
        }

        private FileState QueryFileState(String name)
        {
            String fullFilename = Constants.cacheBaseDirectory + name;
            if (File.Exists(fullFilename))
            {
                FileInfo info = new FileInfo(fullFilename);
                if (info.Length > 0)
                {
                    return FileState.Ready;
                }
            }
            return FileState.Pending;
        }

        public SceneEntry scene;
        private List<FileEntry> files;
    }
}
