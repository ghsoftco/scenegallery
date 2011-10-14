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
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
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

        WebClient webClient = new WebClient();
        CacheDownloader cacheDownloader = new CacheDownloader();
        Database database = new Database();

        Int32 prevMouseX = Int32.MaxValue, prevMouseY = Int32.MaxValue;
        SceneEntry selectedSceneEntry;

        HelpScreen helpScreen = null;

        BrowserMode mode;
        List<SceneEntry> pickedModels = new List<SceneEntry>();
        SceneEntry clickedExemplar = null;
        SceneEntry clickedModelOriginatingExemplar = null;
        bool modelSearchBoxFocused = false;
        bool exemplarSearchBoxFocused = false;

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

        private void setChosenModelScale(float scale)
        {
            SSProcessCommand(d3dContext, "setChosenModelScale\t" + scale);
        }

        private void InitializeWebBrowsers()
        {
            modelBrowser.ObjectForScripting = this;
            exemplarBrowser.ObjectForScripting = this;
            Utility.showThumbnails(exemplarBrowser, database.getExemplars(), Constants.exemplarImgDim);
        }

        public MainWindow()
        {
            InitializeComponent();
            UpdatePanel2Components();

            modelSearchBox.MouseWheel += new MouseEventHandler(modelSearchTextBox_MouseWheel);
            exemplarSearchBox.MouseWheel += new MouseEventHandler(exemplarSearchTextBox_MouseWheel);
            splitContainer1.Panel2.MouseWheel += new MouseEventHandler(splitContainer1_Panel2_MouseMove);
            splitContainer1.Panel2.MouseMove += new MouseEventHandler(splitContainer1_Panel2_MouseMove);

            Constants.Init();
            database.Init(webClient, cacheDownloader);

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

            LoadExemplarInfo();

            InitializeWebBrowsers();
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
            if (!modelSearchBox.Focused || !exemplarSearchBox.Focused)
            {
                this.splitContainer1.Panel1.Focus();
            }
        }

        private void splitContainer1_Panel2_MouseMove(object sender, MouseEventArgs e)
        {
            Control ctrl = splitContainer1.Panel2.GetChildAtPoint(Cursor.Position);
            if (ctrl != null)
            {
                if (ctrl.GetType() == modelBrowser.GetType())
                {
                    WebBrowser browser = (WebBrowser)ctrl;
                    if (browser.Document != null) browser.Document.Focus();
                }
            }
        }

        private void splitContainer1_Panel1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (Constants.debugMode && e.Control && e.KeyCode == Keys.D1)
                SwitchExemplarVisibility();
            else if (Constants.debugMode && e.Control && e.KeyCode == Keys.D2)
                SwitchRestrictModelSearchToExemplars();

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
            UpdatePanel2Components();
        }

        private List<SceneEntry> processQueryResults(IntPtr result)
        {
            List<SceneEntry> queryResults = new List<SceneEntry>();
            queryResults.Clear();

            string searchResults = Marshal.PtrToStringAnsi(result);
            if (searchResults == null || searchResults.Length == 0)
            {
                return queryResults;
            }
            if (result == (IntPtr)0)
            {
                modelNameLabel.Text = "Search Failed";
            }
            else
            {
                List<string> hashes = new List<string>(searchResults.Split(' '));
                queryResults = database.getScenesFromHashes(hashes);
                if (Constants.restrictModelSearchToExemplarContents)
                {
                    List<SceneEntry> filteredResults = new List<SceneEntry>();
                    foreach (SceneEntry scene in queryResults)
                    {
                        if (database.getExemplarModels().Contains(scene))
                        {
                            filteredResults.Add(scene);
                        }
                    }
                    queryResults = filteredResults;
                }
            }
            return queryResults;
        }

        private List<SceneEntry> issueTextQuery(string keyword)
        {
            // Return no scenes for empty query
            if (keyword == null || keyword.Length == 0)
            {
                return new List<SceneEntry>();
            }
            uint success = SSProcessCommand(d3dContext, "textSearch\t" + keyword);
            IntPtr result = (IntPtr)0;
            if (success == 0)
            {
                result = SSQueryString(d3dContext, QueryType.QuerySearchResults);
            }
            return processQueryResults(result);
        }

        private List<SceneEntry> issueShapeQuery()
        {
            if (selectedSceneEntry != null)
            {
                uint success = SSProcessCommand(d3dContext, "shape\t" + selectedSceneEntry.hash);
                IntPtr result = (IntPtr)0;
                if (success == 0)
                {
                    result = SSQueryString(d3dContext, QueryType.QuerySearchResults);
                }
                return processQueryResults(result);
            }
            return null;
        }

        private void modelSearchButton_Click(object sender, EventArgs e)
        {
            Utility.showThumbnails(modelBrowser, issueTextQuery(modelSearchBox.Text), Constants.sceneImgDim);
            LogUIEvent(UIEventType.UIEventKeywordSearchModel, modelSearchButton.Text);
        }

        private void newSelectedModel(string sceneHash)
        {
            if (selectedSceneEntry != null && selectedSceneEntry.hash == sceneHash)
            {
                return;
            }
            SceneEntry entry = database.getScene(sceneHash);
            selectedSceneEntry = entry;
            modelNameLabel.Text = entry.name;
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
                if (Constants.restrictModelSearchToExemplarContents && clickedModelOriginatingExemplar != null)
                {
                    SSProcessCommand(d3dContext, "SetChosenModelTransform\t" + clickedModelOriginatingExemplar.name);
                }
            }
            else
            {
                if (description != null)
                {
                    this.statusText.Text = description;
                }
            }
        }

        private void modelBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            try
            {
                if (e.Url.AbsolutePath != "blank")
                {
                    string sceneHash = e.Url.AbsolutePath;
                    SceneEntry entry = database.getScene(sceneHash);
                    SSModelChosen(d3dContext, "");
                    cacheDownloader.NewScene(entry);
                    cacheDownloader.DownloadFiles(webClient);
                    timerDownloadCheck.Enabled = true;
                    newSelectedModel(sceneHash);
                    e.Cancel = true;
                    if (Constants.restrictModelSearchToExemplarContents)
                    {
                        List<SceneEntry> originatingScenes = database.getInstanceScenesOfModel(sceneHash);
                        Utility.showThumbnails(exemplarBrowser, originatingScenes, Constants.exemplarImgDim);
                        clickedModelOriginatingExemplar = originatingScenes[0];
                    }
                    else
                    {
                        clickedModelOriginatingExemplar = null;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
        }

        private void modelSearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                Utility.showThumbnails(modelBrowser, issueTextQuery(modelSearchBox.Text), Constants.sceneImgDim);
                LogUIEvent(UIEventType.UIEventKeywordSearchModel, modelSearchBox.Text);
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

        public void OpenScene(string filename)
        {
            this.UseWaitCursor = true;
            this.Text = Path.GetFileNameWithoutExtension(filename) + " - SceneStudio";

            List<string> words = ModelListFromSceneFile(filename);
            
            this.statusText.Text = "Downloading models...";
            this.toolStripProgressBar.Minimum = 0;
            this.toolStripProgressBar.Maximum = 100;
            this.toolStripProgressBar.Value = 33;
            Application.DoEvents();

            try
            {
                foreach (string curWord in words)
                {
                    cacheDownloader.NewScene(database.getScene(curWord));
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
            SSProcessCommand(d3dContext, "load\t" + filename);
            this.toolStripProgressBar.Value = 100;
            this.UseWaitCursor = false;
            this.statusText.Text = "Ready";
            timerRender.Enabled = true;
        }

        private void OpenScene()
        {
            if (SaveIfDirty())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "SceneStudio Models|*.scs";
                dialog.Title = "Open";
                dialog.ShowDialog();

                if (dialog.FileName.Length > 0) OpenScene(dialog.FileName);
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

        private void saveAsStripButton_Click(object sender, EventArgs e)
        {
            SaveSceneAs();
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

        private void webPageToolStripButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://graphics.stanford.edu/projects/SceneStudio");
        }

        public List<string> ModelListFromSceneFile(string filename)
        {
            SSProcessCommand(d3dContext, "loadModelList\t" + filename);
            IntPtr modelListPtr = SSQueryString(d3dContext, QueryType.QueryModelList);
            if (modelListPtr == (IntPtr)0)
            {
                MessageBox.Show("There was an error loading the scene.");
                return null;
            }
            string modelList = Marshal.PtrToStringAnsi(modelListPtr);
            string[] models = modelList.Split(' ');

            return new List<string>(models);
        }

        private void LoadExemplarInfo()
        {
            string[] files = Directory.GetFiles(Constants.localExemplarsDirectory);
            foreach (string file in files)
            {
                if (Path.GetExtension(file) == ".scs")
                {
                    string fullpath = Path.GetFullPath(file);
                    List<string> modelHashes = ModelListFromSceneFile(fullpath);
                    List<SceneEntry> models = database.getScenesFromHashes(modelHashes);
                    string tags = "";
                    foreach (SceneEntry model in models)
                    {
                        tags += "," + model.name.Trim();
                    }

                    string hashMapFile = Path.ChangeExtension(fullpath, ".txt");
                    SceneHashMap hashMap = null;
                    if (File.Exists(hashMapFile))
                    {
                        hashMap = new SceneHashMap(hashMapFile, database);
                    }

                    database.addExemplar(Path.GetFileNameWithoutExtension(file), new SceneEntry(file, modelHashes, tags, hashMap));
                }
            }
        }

        public void ReportClick(string hash, int x, int y, int index)
        {
            string msg;
            if (modelBrowser.Focused)
            {
                msg = hash + "," + ((float)x / Constants.sceneImgDim.w).ToString() + "," + ((float)y / Constants.sceneImgDim.h).ToString() + "," + index.ToString();
                LogUIEvent(UIEventType.UIEventClickModel, msg);
                return;
            }
            if (x < 0 || x > Constants.exemplarImgDim.w || y < 0 || y > Constants.exemplarImgDim.h)
            {
                return;
            }

            clickedExemplar = database.getExemplar(hash);
            clickedModelOriginatingExemplar = clickedExemplar;
            SceneEntry clickedModel = clickedExemplar.hashMap[y, x];

            msg = hash + "," + ((float)x / Constants.exemplarImgDim.w).ToString() + "," + ((float)y / Constants.exemplarImgDim.h).ToString() + "," + index.ToString();
            LogUIEvent(UIEventType.UIEventClickExemplar, msg + "," + clickedModel.hash);
            
            if (clickedModel.name == Constants.architectureNameTag)
            {
                return;
            }

            // Form and show model list
            pickedModels.Add(clickedModel);
            List<SceneEntry> models = issueTextQuery(clickedModel.name);
            if (models.Remove(clickedModel)) models.Insert(0, clickedModel);
            Utility.showThumbnails(modelBrowser, models, Constants.sceneImgDim);

            string clickedHash = clickedModel.hash;

            SSModelChosen(d3dContext, "");
            cacheDownloader.NewScene(clickedModel);
            cacheDownloader.DownloadFiles(webClient);
            timerDownloadCheck.Enabled = true;
            newSelectedModel(clickedModel.hash);
        }

        private void filterExemplars()
        {
            string keyword = exemplarSearchBox.Text;
            Utility.showThumbnails(exemplarBrowser, database.filterExemplarsByKeyword(keyword), Constants.exemplarImgDim);
            LogUIEvent(UIEventType.UIEventKeywordSearchExemplar, keyword);
        }

        private void exemplarSearchButton_Click(object sender, EventArgs e)
        {
            filterExemplars();
        }

        private void exemplarSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                filterExemplars();
            }
        }

        private void exemplarBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            try
            {
                if (e.Url.AbsolutePath != "blank")
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
        }

        private void exemplarSearchBox_MouseLeave(object sender, EventArgs e)
        {
            exemplarSearchBoxFocused = false;
        }
        private void exemplarSearchBox_MouseEnter(object sender, EventArgs e)
        {
            if (MouseButtons == MouseButtons.None)
            {
                exemplarSearchBox.SelectAll();
                exemplarSearchBoxFocused = true;
            }
        }
        private void exemplarSearchBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!exemplarSearchBoxFocused && exemplarSearchBox.SelectionLength == 0)
            {
                exemplarSearchBoxFocused = true;
                exemplarSearchBox.SelectAll();
            }
        }
        private void exemplarSearchBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (exemplarSearchBox.Text.Length != 0)
            {
                exemplarSearchBox.SelectionStart = 0;
                exemplarSearchBox.SelectionLength = exemplarSearchBox.Text.Length;
            }
        }

        private void modelSearchBox_MouseEnter(object sender, EventArgs e)
        {
            if (MouseButtons == MouseButtons.None)
            {
                modelSearchBox.SelectAll();
                modelSearchBoxFocused = true;
            }
        }
        private void modelSearchBox_MouseLeave(object sender, EventArgs e)
        {
            modelSearchBoxFocused = false;
        }
        private void modelSearchBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!modelSearchBoxFocused && modelSearchBox.SelectionLength == 0)
            {
                modelSearchBoxFocused = true;
                modelSearchBox.SelectAll();
            }
        }
        private void modelSearchBox_DoubleClick(object sender, EventArgs e)
        {
            if (modelSearchBox.Text.Length != 0)
            {
                modelSearchBox.SelectionStart = 0;
                modelSearchBox.SelectionLength = modelSearchBox.Text.Length;
            }
        }

        void modelSearchTextBox_MouseWheel(object sender, MouseEventArgs e)
        {
            //
            // Neither of these have the desired effect
            //
            //this.searchBrowser.Focus();
            //this.searchBrowser.Select();

            //searchBrowser.Document.Window.ScrollTo(0, y);
            if (modelBrowser.Document != null)
            {
                modelBrowser.Document.Focus();
            }
        }
        void exemplarSearchTextBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (exemplarBrowser.Document != null)
            {
                exemplarBrowser.Document.Focus();
            }
        }

        private void UpdatePanel2Components()
        {
            splitContainer1.Panel1MinSize = 250;
            splitContainer1.Panel2MinSize = 405;
            splitContainer1.SplitterDistance = splitContainer1.Width - 870;

            int panelW = splitContainer1.Panel2.Width;
            int panelH = splitContainer1.Panel2.Height;

            modelSearchButton.Width = modelSearchButton.PreferredSize.Width;
            modelSearchButton.Left = splitContainer1.Panel2.Width - (modelSearchButton.Width + 5);
            modelSearchBox.Left = modelSearchButton.Left - (modelSearchBox.Width + 5);
            modelNameLabel.Left = 5;
            modelNameLabel.Width = modelSearchBox.Left - 10;

            modelBrowser.Left = 5;
            modelBrowser.Top = modelSearchBox.Bottom;
            modelBrowser.Height = 300;
            modelBrowser.Width = panelW - 5;

            
            exemplarSearchButton.Top = modelBrowser.Bottom + 5;
            exemplarSearchButton.Left = splitContainer1.Panel2.Width - (exemplarSearchButton.Width + 5);
            exemplarSearchBox.Left = exemplarSearchButton.Left - (exemplarSearchBox.Width + 5);
            exemplarSearchBox.Top = exemplarSearchButton.Top;
            resetExemplarsButton.Left = 5;
            resetExemplarsButton.Top = exemplarSearchBox.Top;

            exemplarBrowser.Left = 5;
            exemplarBrowser.Top = exemplarSearchBox.Bottom;
            exemplarBrowser.Height = panelH - exemplarSearchBox.Bottom - 5;
            exemplarBrowser.Width = panelW - 5;

            if (mode == BrowserMode.ExemplarsNotAvailable)
            {
                exemplarSearchBox.Visible = false;
                exemplarSearchButton.Visible = false;
                exemplarBrowser.Visible = false;
                resetExemplarsButton.Visible = false;
                modelSearchBox.Visible = true;
                modelSearchButton.Visible = true;
                modelBrowser.Height = panelH - modelSearchBox.Height - 5;
            }
            else if (mode == BrowserMode.ExemplarsAvailable)
            {
                exemplarSearchBox.Visible = true;
                exemplarSearchButton.Visible = true;
                exemplarBrowser.Visible = true;
                resetExemplarsButton.Visible = true;
                modelSearchBox.Visible = false;
                modelSearchButton.Visible = false;
            }
        }

        private void LogUIEvent(UIEventType e, string data)
        {
            if (data == "") data = "_";
            if (SSProcessCommand(d3dContext, "LogUIEvent\t" + Utility.UIEventTypeToString(e) + "\t" + data) != 0)
            {
                throw new Exception("LogUIEvent failed.");
            }
        }

        private void SwitchExemplarVisibility()
        {
            if (mode == BrowserMode.ExemplarsAvailable)
            {
                mode = BrowserMode.ExemplarsNotAvailable;
            }
            else mode = BrowserMode.ExemplarsAvailable;

            Utility.showThumbnails(modelBrowser, new List<SceneEntry>(), Constants.sceneImgDim);
            Utility.showThumbnails(exemplarBrowser, database.getExemplars(), Constants.exemplarImgDim);
            UpdatePanel2Components();
        }

        private void SwitchRestrictModelSearchToExemplars()
        {
            Constants.restrictModelSearchToExemplarContents = !Constants.restrictModelSearchToExemplarContents;
        }

        private void resetExemplarsButton_Click(object sender, EventArgs e)
        {
            Utility.showThumbnails(exemplarBrowser, database.getExemplars(), Constants.exemplarImgDim);
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
