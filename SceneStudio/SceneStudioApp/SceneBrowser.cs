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
    public enum BrowserMode
    {
        ExemplarBrowsing    = 0,
        ModelBrowsing       = 1,
    };

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

        public static string makeHTMLgallery(List<SceneEntry> entries, Dim dim)
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
            return sw.ToString();
        }

        public void showThumbnails(List<SceneEntry> scenes, Dim dim)
        {
            webBrowser.DocumentText = makeHTMLgallery(scenes, dim);
        }

        public void ReportClick(string hash, int x, int y)
        {
            string msg = hash.ToString() + "," + x.ToString() + "," + y.ToString();
            //MessageBox.Show(msg, "client code");
        }

        private void browser_Load(object sender, EventArgs e)
        {
            webBrowser.ObjectForScripting = this;
            mode = BrowserMode.ExemplarBrowsing;
            showThumbnails(database.getExemplars(), Constants.exemplarImgDim);
        }

        private Image CaptureWindow()
        {
            Rectangle bounds = this.Bounds;
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                }
                return bitmap;
            }
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

            if (mode == BrowserMode.ExemplarBrowsing)
            {
                exemplarClicked(tag);
            }
            else if (mode == BrowserMode.ModelBrowsing)
            {
                modelClicked(tag);
            }

            e.Cancel = true;
        }

        private void exemplarClicked(string tag)
        {
            SceneEntry exemplar = database.getExemplar(tag);
            clickedExemplar = exemplar;
            showThumbnails(database.getScenesFromHashes(exemplar.modelHashes), Constants.sceneImgDim);
            mode = BrowserMode.ModelBrowsing;
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
                showThumbnails(database.getExemplars(), Constants.exemplarImgDim);
                mode = BrowserMode.ExemplarBrowsing;
            }
            if (e.KeyCode == Keys.Enter)
            {
                showThumbnails(pickedModels, Constants.sceneImgDim);
                mode = BrowserMode.ModelBrowsing;
            }
            if (e.KeyCode == Keys.O && mode == BrowserMode.ExemplarBrowsing)
            {
                main.OpenScene(clickedExemplar.name);
            }
        }

        private void keywordSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            string keyword = keywordSearchTextBox.Text;
            showThumbnails(database.filterExemplarsByKeyword(keyword), Constants.exemplarImgDim);
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
