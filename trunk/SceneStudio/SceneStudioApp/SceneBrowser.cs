using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Web.UI;
using System.Drawing.Imaging;

namespace SceneStudioApp
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class SceneBrowser : Form
    {
        private Database database = null;
        private MainWindow main = null;
        private BrowserMode mode;
        private List<SceneEntry> pickedModels;
        private SceneEntry clickedExemplar = null;
        private bool keywordSearchTextBoxFocused = false;

        public SceneBrowser(MainWindow _main, Database _database)
        {
            main = _main;
            database = _database;
            pickedModels = new List<SceneEntry>();
            InitializeComponent();
        }

        public void ReportClick(string hash, int x, int y)
        {
            string msg = hash.ToString() + "," + x.ToString() + "," + y.ToString();
            SceneEntry exemplar = database.getExemplar(hash);
            SceneEntry clickedModel = exemplar.hashMap[y, x];
            pickedModels.Add(clickedModel);
            //MessageBox.Show(msg, "client code");
        }

        private void browser_Load(object sender, EventArgs e)
        {
            webBrowser.ObjectForScripting = this;
            mode = BrowserMode.ExemplarsAvailable;
            Utility.showThumbnails(webBrowser, database.getExemplars(), Constants.exemplarImgDim);
        }

        private void webBrowser_Focus(object sender, EventArgs e)
        {
            webBrowser.Focus();
            webBrowser.Select();
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.AbsolutePath == "blank") return;

            string tag = e.Url.AbsolutePath;

            if (mode == BrowserMode.ExemplarsAvailable)
            {
                exemplarClicked(tag);
            }
            else if (mode == BrowserMode.ExemplarsNotAvailable)
            {
                modelClicked(tag);
            }

            e.Cancel = true;
        }

        private void exemplarClicked(string tag)
        {
            SceneEntry exemplar = database.getExemplar(tag);
            clickedExemplar = exemplar;
            Utility.showThumbnails(webBrowser, database.getScenesFromHashes(exemplar.modelHashes), Constants.sceneImgDim);
            mode = BrowserMode.ExemplarsNotAvailable;
        }
        private void modelClicked(string tag)
        {
            SceneEntry model = database.getScene(tag);
            pickedModels.Add(model);
            //showThumbnails(database.getExemplars(), Constants.exemplarImgDim);
        }

        private void webBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                Utility.showThumbnails(webBrowser, database.getExemplars(), Constants.exemplarImgDim);
                mode = BrowserMode.ExemplarsAvailable;
            }
            if (e.KeyCode == Keys.Enter)
            {
                Utility.showThumbnails(webBrowser, pickedModels, Constants.sceneImgDim);
                mode = BrowserMode.ExemplarsNotAvailable;
            }
            if (e.KeyCode == Keys.O && mode == BrowserMode.ExemplarsAvailable)
            {
                main.OpenScene(clickedExemplar.name);
            }
        }

        private void keywordSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            string keyword = keywordSearchTextBox.Text;
            Utility.showThumbnails(webBrowser, database.filterExemplarsByKeyword(keyword), Constants.exemplarImgDim);
        }
        private void keywordSearchTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!keywordSearchTextBoxFocused && keywordSearchTextBox.SelectionLength == 0)
            {
                keywordSearchTextBoxFocused = true;
                keywordSearchTextBox.SelectAll();
            }
        }
        private void keywordSearchTextBox_Leave(object sender, EventArgs e)
        {
            keywordSearchTextBoxFocused = false;
        }
        private void keywordSearchTextBox_Enter(object sender, EventArgs e)
        {
            if (MouseButtons == MouseButtons.None)
            {
                keywordSearchTextBox.SelectAll();
                keywordSearchTextBoxFocused = true;
            }
        }
    }
}
