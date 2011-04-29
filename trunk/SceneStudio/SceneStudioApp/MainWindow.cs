using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;

//
// Marshalling tutorial
//
//http://msdn.microsoft.com/en-us/library/aa288468%28v=vs.71%29.aspx#pinvoke_custommarshaling

namespace SceneStudioApp
{
    public partial class MainWindow : Form
    {
        const string SceneStudioDLL = "SceneStudio.dll";
        [DllImport(SceneStudioDLL)] private static extern IntPtr SSInitD3D(IntPtr windowHandle);
        [DllImport(SceneStudioDLL)] private static extern UInt32 SSRenderFrame(IntPtr d3dContext);
        [DllImport(SceneStudioDLL)] private static extern UInt32 SSMouseMove(IntPtr d3dContext, UInt32 button, Int32 x, Int32 y, Int32 xDelta, Int32 yDelta);
        [DllImport(SceneStudioDLL)] private static extern UInt32 SSMouseDown(IntPtr d3dContext, UInt32 button, Int32 x, Int32 y);
        [DllImport(SceneStudioDLL)] private static extern UInt32 SSMouseUp(IntPtr d3dContext, UInt32 button);
        [DllImport(SceneStudioDLL)] private static extern UInt32 SSMouseWheel(IntPtr d3dContext, Int32 delta);
        [DllImport(SceneStudioDLL)] private static extern UInt32 SSKeyPress(IntPtr d3dContext, Int32 mode, bool shift, bool ctrl);
        [DllImport(SceneStudioDLL)] private static extern IntPtr SSQueryString(IntPtr d3dContext, QueryType q);
        [DllImport(SceneStudioDLL)] private static extern Int32 SSQueryInteger(IntPtr d3dContext, QueryType q);
        [DllImport(SceneStudioDLL)] private static extern UInt32 SSModelChosen(IntPtr d3dContext, [In, MarshalAs(UnmanagedType.LPStr)] String model);
        [DllImport(SceneStudioDLL)] private static extern UInt32 SSProcessCommand(IntPtr d3dContext, [In, MarshalAs(UnmanagedType.LPStr)] String command);

        IntPtr d3dContext = (IntPtr)0;

        Dictionary<String, SceneEntry> sceneDatabase;
        List<ArchitectureEntry> architectureDatabase;
        WebClient webClient = new WebClient();
        CacheDownloader cacheDownloader = new CacheDownloader();

        Int32 prevMouseX = Int32.MaxValue, prevMouseY = Int32.MaxValue;
        SceneEntry selectedSceneEntry;

        HelpScreen helpScreen = null;

        private void ReportCriticalError(String s)
        {
            //
            // Application.Exit() doesn't appear to actually exit the application.
            //
            //MessageBox.Show(s + "\nThe application will now exit.", "Fatal Error Encountered");
            //Application.Exit();

            MessageBox.Show(s, "Error");
        }

        private bool isInsertMode()
        {
            int queryresult = SSQueryInteger(d3dContext, QueryType.QueryIsInsertMode);
            return queryresult == 1;
        }

        private void verifyMode()
        {
            bool insertmode = isInsertMode();
            updateModeUI(insertmode);
        }

        private void updateModeUI(bool insertmode)
        {
            if (insertmode)
            {
                splitContainer1.Panel1.Cursor = Cursors.UpArrow;
            }
            else
            {
                splitContainer1.Panel1.Cursor = Cursors.Arrow;
            }
        }

        private void enterInsertMode()
        {
            SSProcessCommand(d3dContext, "enterInsertMode");
            updateModeUI(true);
        }
        
        private void LoadSceneInfo()
        {
            sceneDatabase = new Dictionary<string, SceneEntry>();
            string[] allLines = File.ReadAllLines(Constants.cacheSceneNamesFilename);
            for (int lineIndex = 0; lineIndex < allLines.Length; lineIndex++)
            {
                String curLine = allLines[lineIndex];
                String[] curWords = curLine.Split('\t');
                if (curWords.Length == 2)
                {
                    sceneDatabase.Add(curWords[0], new SceneEntry(curWords[0], curWords[1]));
                }
            }

            architectureDatabase = new List<ArchitectureEntry>();
            allLines = File.ReadAllLines(Constants.cacheArchitectureListFilename);
            for (int lineIndex = 0; lineIndex < allLines.Length; lineIndex++)
            {
                String curLine = allLines[lineIndex];
                String[] curWords = curLine.Split('\t');
                if (curWords.Length == 2)
                {
                    architectureDatabase.Add(new ArchitectureEntry(curWords[0]));
                }
            }

            //
            // Architecture does not have a name, but still needs a SceneEntry
            //
            foreach (ArchitectureEntry architecture in architectureDatabase)
            {
                sceneDatabase.Add(architecture.name, new SceneEntry(architecture.name, "Architecture"));
            }

            allLines = File.ReadAllLines(Constants.cacheSceneTexturesFilename);
            for (int lineIndex = 0; lineIndex < allLines.Length; lineIndex++)
            {
                String curLine = allLines[lineIndex];
                String[] curWords = curLine.Split(' ');
                if (curWords.Length >= 2 && sceneDatabase.ContainsKey(curWords[0]))
                {
                    SceneEntry curEntry = sceneDatabase[curWords[0]];
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
            catch(Exception e)
            {
                ReportCriticalError("Failed to create cache directories.\n" + e.ToString());
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
            catch(Exception e)
            {
                ReportCriticalError("Failed to download scene information files. Make sure you have a working internet connection.\n" + e.ToString());
            }
        }

        private void PopulateCacheArchitecture()
        {
            try
            {
                foreach (ArchitectureEntry architecture in architectureDatabase)
                {
                    cacheDownloader.NewScene(sceneDatabase[architecture.name]);
                    cacheDownloader.DownloadFilesImmediate(webClient);
                }
            }
            catch (Exception e)
            {
                ReportCriticalError("Failed to download scene architecture files. Make sure you have a working internet connection.\n" + e.ToString());
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            splitContainer1.Panel1MinSize = 250;
            splitContainer1.Panel2MinSize = 415;
            splitContainer1.SplitterDistance = 875;

            queryBox.MouseWheel += new MouseEventHandler(queryBox_MouseWheel);

            Constants.Init();
            
            PopulateCache();

            LoadSceneInfo();

            PopulateCacheArchitecture();

            if (d3dContext == (IntPtr)0)
            {
                IntPtr windowHandle = splitContainer1.Panel1.Handle;
                try
                {
                    d3dContext = SSInitD3D(windowHandle);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("A necessary application component failed to load. This error usually occurs because the latest version of DirectX is not installed. " +
                                    "SceneStudio will now take you to Microsoft's DirectX installer webpage, which should fix this problem.  For most users, this does not require the computer to restart. If this problem persists even after this is installed, please let us know." +
                                    "Details: " + ex.ToString(), "Error");
                    System.Diagnostics.Process.Start("http://www.microsoft.com/downloads/en/details.aspx?FamilyID=2da43d38-db71-4c1b-bc6a-9b6652cd92a3");
                    throw new SystemException(ex.ToString());
                }
            }
        }

        private void splitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            UInt32 button = 0;
            if (e.Button == MouseButtons.Left)
                button = 1;
            if (e.Button == MouseButtons.Right)
                button = 2;
            SSMouseDown(d3dContext, button, e.X, e.Y);
            prevMouseX = e.X;
            prevMouseY = e.Y;

            // Ensure the UI is in the correct state after all mouse events
            verifyMode();
            updateHelp();
        }

        private void splitContainer1_Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            UInt32 button = 0;
            if (e.Button == MouseButtons.Left)
                button = 1;
            if (e.Button == MouseButtons.Right)
                button = 2;
            SSMouseUp(d3dContext, button);

            // Ensure the UI is in the correct state after all mouse events
            verifyMode();
            updateHelp();
        }

        private void splitContainer1_Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (timerRender.Enabled)
            {
                return;
            }
            if (e.Delta == 0)
            {
                UInt32 button = 0;
                if (e.Button == MouseButtons.Left)
                    button = 1;
                if (e.Button == MouseButtons.Right)
                    button = 2;

                if (e.Button != MouseButtons.None)
                {
                    this.splitContainer1.Panel1.Focus();
                }

                if (prevMouseX != Int32.MaxValue && prevMouseY != Int32.MaxValue)
                {
                    SSMouseMove(d3dContext, button, e.X, e.Y, e.X - prevMouseX, e.Y - prevMouseY);
                }
                prevMouseX = e.X;
                prevMouseY = e.Y;
            }
            else
            {
                SSMouseWheel(d3dContext, e.Delta);
            }
            SSRenderFrame(d3dContext);

            IntPtr selectedModelStringPtr = SSQueryString(d3dContext, QueryType.QuerySelectedModelName);
            if (selectedModelStringPtr != (IntPtr)0)
            {
                string selectedModelString = Marshal.PtrToStringAnsi(selectedModelStringPtr);
                newSelectedModel(selectedModelString);
            }

            // Ensure the UI is in the correct state after all mouse events
            verifyMode();
            updateHelp();
        }

        private void splitContainer1_Panel1_MouseLeave(object sender, EventArgs e)
        {
            prevMouseX = Int32.MaxValue;
            prevMouseY = Int32.MaxValue;
        }

        private void splitContainer1_Panel1_MouseEnter(object sender, EventArgs e)
        {
            //
            // Focus is needed to receive all mouse commands
            //
            prevMouseX = Int32.MaxValue;
            prevMouseY = Int32.MaxValue;
            if (!queryBox.Focused)
            {
                this.splitContainer1.Panel1.Focus();
            }
        }

        private void splitContainer1_Panel2_MouseEnter(object sender, EventArgs e)
        {
            if (splitContainer1.Focused)
            {
                //queryBox.Focus();
            }
        }

        private void splitContainer1_Panel1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //
            // Only re-render the display on actual keys
            //
            if ((e.KeyValue >= 65 && e.KeyValue <= 90) ||
                (e.KeyValue >= 37 && e.KeyValue <= 40) ||
                e.KeyCode == Keys.Escape || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                SSKeyPress(d3dContext, (int)e.KeyCode, e.Shift, e.Control);
                SSRenderFrame(d3dContext);

                // Ensure the UI is in the correct state after all keyboard events
                verifyMode();
                updateHelp();
            }
        }

        private void splitContainer1_Panel1_Resize(object sender, EventArgs e)
        {
            timerRender.Enabled = true;
        }

        private void timerRender_Tick(object sender, EventArgs e)
        {
            if ((int)d3dContext != 0)
            {
                SSRenderFrame(d3dContext);
                timerRender.Enabled = false;
            }
        }

        private void splitContainer1_Panel2_Resize(object sender, EventArgs e)
        {
            queryButton.Left = splitContainer1.Panel2.Width - queryButton.Width - 10;
            queryBox.Left = 7;
            queryBox.Width = queryButton.Left - 14;

            int browserHeight = splitContainer1.Panel2.Height - modelNameLabel.Bottom - 9;
            if (browserHeight > 9)
            {
                this.searchBrowser.Height = browserHeight;
            }
            this.searchBrowser.Left = 0;
            this.searchBrowser.Width = splitContainer1.Panel2.Width - 5;
        }

        private void processQueryResults(IntPtr result)
        {
            string searchResults = Marshal.PtrToStringAnsi(result);

            if (result == (IntPtr)0)
            {
                modelNameLabel.Text = "Search Failed";
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("<html><head></head><body>");

                if (searchResults.Length == 0)
                {
                    builder.AppendLine("<h1>No results found.</h1>");
                }
                else
                {
                    string[] results = searchResults.Split(' ');

                    foreach (string curString in results)
                    {
                        string imageSrc = Constants.webSceneImagesDirectory + curString + ".jpg";
                        builder.AppendLine("<a href=\"" + curString + "\"><img width=168 height=126 src=\"" + imageSrc + "\" /></a>");
                    }
                }

                builder.AppendLine("</body></html>");
                searchBrowser.DocumentText = builder.ToString();
            }
        }

        private void issueTextQuery()
        {
            uint success = SSProcessCommand(d3dContext, "textSearch\t" + queryBox.Text);
            IntPtr result = (IntPtr)0;
            if (success == 0)
            {
                result = SSQueryString(d3dContext, QueryType.QuerySearchResults);
            }
            processQueryResults(result);
        }

        private void issueShapeQuery()
        {
            if (selectedSceneEntry != null)
            {
                uint success = SSProcessCommand(d3dContext, "shape\t" + selectedSceneEntry.hash);
                IntPtr result = (IntPtr)0;
                if (success == 0)
                {
                    result = SSQueryString(d3dContext, QueryType.QuerySearchResults);
                }
                processQueryResults(result);
            }
        }

        private void queryButton_Click(object sender, EventArgs e)
        {
            issueTextQuery();
        }

        private void newSelectedModel(string sceneHash)
        {
            if (selectedSceneEntry != null && selectedSceneEntry.hash == sceneHash)
            {
                return;
            }
            SceneEntry entry;
            bool success = sceneDatabase.TryGetValue(sceneHash, out entry);
            if (entry == null)
            {
                //modelNameLabel.Text = "<Model name not found>";
            }
            else
            {
                selectedSceneEntry = entry;
                modelNameLabel.Text = entry.name;
                try
                {
                    selectedModelPicture.ImageLocation = Constants.webSceneImagesDirectory + sceneHash + ".jpg";
                }
                catch
                {

                }
            }
            Application.DoEvents();
        }

        private void timerDownloadCheck_Tick(object sender, EventArgs e)
        {
            string description;
            CacheDownloader.DownloadStatus status = cacheDownloader.Status(out description);
            if (status == CacheDownloader.DownloadStatus.Failed)
            {
                this.statusText.Text = "Download failed";
                timerDownloadCheck.Enabled = false;
            }
            else if (status == CacheDownloader.DownloadStatus.Done)
            {
                this.statusText.Text = "Ready";
                timerDownloadCheck.Enabled = false;
                SSModelChosen(d3dContext, cacheDownloader.scene.hash);
                newSelectedModel(cacheDownloader.scene.hash);
                enterInsertMode();
            }
            else
            {
                if (description != null)
                {
                    this.statusText.Text = description;
                }
            }
        }

        private void searchBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            try
            {
                if (e.Url.AbsolutePath != "blank")
                {
                    string sceneHash = e.Url.AbsolutePath;

                    SceneEntry entry;
                    bool success = sceneDatabase.TryGetValue(sceneHash, out entry);
                    if (success)
                    {
                        SSModelChosen(d3dContext, "");
                        cacheDownloader.NewScene(entry);
                        cacheDownloader.DownloadFiles(webClient);
                        timerDownloadCheck.Enabled = true;
                        newSelectedModel(sceneHash);
                    }
                    e.Cancel = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
        }

        private void queryBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                issueTextQuery();
            }
        }

        void queryBox_MouseWheel(object sender, MouseEventArgs e)
        {
            //
            // Neither of these have the desired effect
            //
            //this.searchBrowser.Focus();
            //this.searchBrowser.Select();

            //searchBrowser.Document.Window.ScrollTo(0, y);
            if (searchBrowser.Document != null)
            {
                searchBrowser.Document.Focus();
            }
        }
        
        private void modelNameLabel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (selectedSceneEntry != null)
            {
                string modelURL = "http://sketchup.google.com/3dwarehouse/details?mid=" + selectedSceneEntry.hash;
                System.Diagnostics.Process.Start(modelURL);
            }
        }

        private void queryBox_DoubleClick(object sender, EventArgs e)
        {
            //if (queryBox.SelectedText.Length == 0)
            {
                queryBox.SelectionStart = 0;
                queryBox.SelectionLength = queryBox.Text.Length;
            }
        }

        private bool OnAppExit()
        {
            return SaveIfDirty();
        }

        private bool SaveIfDirty()
        {
            if (SSQueryInteger(d3dContext, QueryType.QueryIsSceneDirty) == 1)
            {
                //
                // Current scene is dirty
                //
                String filename = "Untitled";
                IntPtr filenameStringPtr = SSQueryString(d3dContext, QueryType.QuerySceneFilename);
                if (filenameStringPtr != (IntPtr)0)
                {
                    filename = Marshal.PtrToStringAnsi(filenameStringPtr);
                }
                DialogResult result = MessageBox.Show("Save changes to " + filename + "?", "SceneStudio", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                {
                    return false;
                }
                else if (result == DialogResult.Yes)
                {
                    SaveScene();
                }
            }
            return true;
        }

        private void SaveScene()
        {
            IntPtr filenameStringPtr = SSQueryString(d3dContext, QueryType.QuerySceneFilename);
            if (filenameStringPtr == (IntPtr)0 || Marshal.PtrToStringAnsi(filenameStringPtr) == "Untitled")
            {
                SaveSceneAs();
            }
            else
            {
                SSProcessCommand(d3dContext, "save\t*");
            }
        }

        private void SaveSceneAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "SceneStudio Models|*.scs";
            dialog.Title = "Save As";
            dialog.ShowDialog();

            if (dialog.FileName.Length > 0)
            {
                string[] parts = dialog.FileName.Split('\\');
                this.Text = parts[parts.Length - 1] + " - SceneStudio";
                SSProcessCommand(d3dContext, "save\t" + dialog.FileName);
                timerRender.Enabled = true;
            }
        }

        private void NewScene()
        {
            if (SaveIfDirty())
            {
                RoomSelection roomSelectionForm = new RoomSelection();
                roomSelectionForm.ShowDialog();
                if (roomSelectionForm.result != null && roomSelectionForm.result.Length > 0)
                {
                    SSProcessCommand(d3dContext, "new\t" + roomSelectionForm.result);
                }
                timerRender.Enabled = true;
                this.Text = "Untitled - SceneStudio";
            }
        }

        private void OpenScene()
        {
            if (SaveIfDirty())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "SceneStudio Models|*.scs";
                dialog.Title = "Open";
                dialog.ShowDialog();

                if (dialog.FileName.Length > 0)
                {
                    this.UseWaitCursor = true;

                    string[] parts = dialog.FileName.Split('\\');
                    this.Text = parts[parts.Length - 1] + " - SceneStudio";
                    
                    SSProcessCommand(d3dContext, "loadModelList\t" + dialog.FileName);
                    IntPtr modelListPtr = SSQueryString(d3dContext, QueryType.QueryModelList);
                    if (modelListPtr == (IntPtr)0)
                    {
                        MessageBox.Show("There was an error loading the scene.");
                        return;
                    }
                    string modelList = Marshal.PtrToStringAnsi(modelListPtr);
                    string[] words = modelList.Split(' ');

                    this.statusText.Text = "Downloading models...";
                    this.toolStripProgressBar.Minimum = 0;
                    this.toolStripProgressBar.Maximum = 100;
                    this.toolStripProgressBar.Value = 33;
                    Application.DoEvents();

                    try
                    {
                        foreach (string curWord in words)
                        {
                            cacheDownloader.NewScene(sceneDatabase[curWord]);
                            cacheDownloader.DownloadFilesImmediate(webClient);
                            //
                            // Avoid running the message loop in here.  Users might be able to enter file loading functions twice then.
                            //
                            //Application.DoEvents();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("There was an error downloading the scene models.\n" + ex.ToString());
                        this.UseWaitCursor = false;
                        this.statusText.Text = "Ready";
                        return;
                    }

                    this.statusText.Text = "Loading models...";
                    this.toolStripProgressBar.Value = 66;
                    Application.DoEvents();
                    SSProcessCommand(d3dContext, "load\t" + dialog.FileName);
                    this.toolStripProgressBar.Value = 100;
                    this.UseWaitCursor = false;
                    this.statusText.Text = "Ready";
                    timerRender.Enabled = true;
                }
            }
        }

        private void MenuSave_Click(object sender, EventArgs e)
        {
            SaveScene();
        }

        private void MenuNew_Click(object sender, EventArgs e)
        {
            NewScene();
        }

        private void MenuSaveAs_Click(object sender, EventArgs e)
        {
            SaveSceneAs();
        }

        private void MenuOpen_Click(object sender, EventArgs e)
        {
            OpenScene();
        }

        private void MenuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!OnAppExit())
            {
                e.Cancel = true;
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SSProcessCommand(d3dContext, "resetCamera");
            timerRender.Enabled = true;
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            NewScene();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenScene();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveScene();
        }

        private void deleteToolStripButton_Click(object sender, EventArgs e)
        {
            SSKeyPress(d3dContext, (int)Keys.Delete, false, false);
            SSRenderFrame(d3dContext);
        }

        private void resetCameraToolStripButton_Click(object sender, EventArgs e)
        {
            SSProcessCommand(d3dContext, "resetCamera");
            timerRender.Enabled = true;
        }

        private void rotateCCWToolStripButton_Click(object sender, EventArgs e)
        {
            SSKeyPress(d3dContext, (int)Keys.Left, Control.ModifierKeys == Keys.Shift, Control.ModifierKeys == Keys.Control);
            SSRenderFrame(d3dContext);
        }

        private void rotateCWToolStripButton_Click(object sender, EventArgs e)
        {
            SSKeyPress(d3dContext, (int)Keys.Right, Control.ModifierKeys == Keys.Shift, Control.ModifierKeys == Keys.Control);
            SSRenderFrame(d3dContext);
        }

        private void scaleUpToolStripButton_Click(object sender, EventArgs e)
        {
            SSKeyPress(d3dContext, (int)Keys.Up, Control.ModifierKeys == Keys.Shift, Control.ModifierKeys == Keys.Control);
            SSRenderFrame(d3dContext);
        }

        private void scaleDownToolStripButton_Click(object sender, EventArgs e)
        {
            SSKeyPress(d3dContext, (int)Keys.Down, Control.ModifierKeys == Keys.Shift, Control.ModifierKeys == Keys.Control);
            SSRenderFrame(d3dContext);
        }

        private void flipNToolStripButton_Click(object sender, EventArgs e)
        {
            SSKeyPress(d3dContext, (int)Keys.N, Control.ModifierKeys == Keys.Shift, Control.ModifierKeys == Keys.Control);
            SSRenderFrame(d3dContext);
        }

        private void flipMToolStripButton_Click(object sender, EventArgs e)
        {
            SSKeyPress(d3dContext, (int)Keys.M, Control.ModifierKeys == Keys.Shift, Control.ModifierKeys == Keys.Control);
            SSRenderFrame(d3dContext);
        }

        private void revertObjectToolStripButton_Click(object sender, EventArgs e)
        {
            SSKeyPress(d3dContext, (int)Keys.B, false, false);
            SSRenderFrame(d3dContext);
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            SSKeyPress(d3dContext, (int)Keys.C, false, true);
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            SSKeyPress(d3dContext, (int)Keys.V, false, true);
        }

        private void undoToolStripButton_Click(object sender, EventArgs e)
        {
            SSKeyPress(d3dContext, (int)Keys.Z, false, true);
            timerRender.Enabled = true;
        }

        private void uploadToolStripButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Press OK to upload this scene to our server. This will save your current file. You can upload the same scene multiple times and the latest version of each unique filename will be maintained.", "Confirm Upload", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                try
                {
                    SaveScene();
                    IntPtr filenameStringPtr = SSQueryString(d3dContext, QueryType.QuerySceneFilename);
                    string filenameString = null;
                    if (filenameStringPtr != (IntPtr)0)
                    {
                        filenameString = Marshal.PtrToStringAnsi(filenameStringPtr);
                    }
                    if (filenameStringPtr == (IntPtr)0 || filenameString.Length < 2 || filenameString == "Untitled")
                    {
                        MessageBox.Show("You must save the scene before it can be uploaded.");
                    }
                    else
                    {
                        string[] parts = filenameString.Split('\\');
                        string data = File.ReadAllText(filenameString);
                        SceneUploader.UploadScene(data, parts[parts.Length - 1]);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was an error uploading the scene.\n" + ex.ToString());
                }
            }
        }

        private void clearToolStripButton_Click(object sender, EventArgs e)
        {
            SSKeyPress(d3dContext, (int)Keys.Escape, false, false);
        }

        private void showHelp()
        {
            // Do nothing if we have an open help screen already
            if (helpScreen != null && helpScreen.Visible)
                return;

            helpScreen = new HelpScreen();
            helpScreen.TopMost = true;
            helpScreen.Show();
            updateHelp();
        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            showHelp();
        }

        private void toolStripButtons_MouseEnter(object sender, EventArgs e)
        {
            if (helpScreen != null)
                helpScreen.UpdateContents((ToolStripButton)sender);
        }

        private void updateHelp()
        {
            if (helpScreen != null)
            {
                ToolStripButton modeBtn = new ToolStripButton();
                bool insertmode = isInsertMode();

                // Insert mode
                if (insertmode)
                {
                    modeBtn.ToolTipText = "Insert Object";
                    modeBtn.Name = "insertObject";
                    modeBtn.Image = Properties.Resources.Arrow;
                }
                // Normal mode
                else
                {
                    modeBtn.ToolTipText = "View and Interact with Objects";
                    modeBtn.Name = "normalMode";
                    modeBtn.Image = Properties.Resources.Mouse_pointer;
                }

                helpScreen.UpdateContents(modeBtn);
            }
        }

        private void toolStripButtons_MouseLeave(object sender, EventArgs e)
        {
            updateHelp();
        }

        private void shapeSearchButton_Click(object sender, EventArgs e)
        {
            issueShapeQuery();
        }

        private void webPageToolStripButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://graphics.stanford.edu/projects/SceneStudio");
        }
    }
}
