using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SceneStudioApp
{
    public enum BrowserMode
    {
        ExemplarBrowsing    = 0,
        ModelBrowsing       = 1,
    };

    public partial class SceneBrowser : Form
    {
        private Database database = null;
        private MainWindow main = null;
        private BrowserMode mode;
        private List<SceneEntry> pickedModels;
        private SceneEntry clickedExemplar = null;

        public SceneBrowser(MainWindow _main, Database _database)
        {
            main = _main;
            database = _database;
            pickedModels = new List<SceneEntry>();
            InitializeComponent();
        }

        public static string makeHTMLgallery(List<SceneEntry> entries, Dim dim)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<html><head></head><body>");

            if (entries.Count == 0)
            {
                builder.AppendLine("<h1>No results found.</h1>");
            }
            else
            {
                foreach (SceneEntry e in entries)
                {
                    //builder.AppendLine("<div id=\"content\" align=\"center\" >");
                    builder.AppendLine("<a href=\"" + e.hash + "\"><img width=" + dim.w + " height=" + dim.h + " src=\"" + e.image + "\" /></a>");
                    //builder.AppendLine("<p><button type=\"button\">Click Me!</button></p>");
                    //builder.AppendLine("</div>");
                }
            }

            builder.AppendLine("</body></html>");
            return builder.ToString();
        }

        public void showThumbnails(List<SceneEntry> scenes, Dim dim)
        {
            webBrowser.DocumentText = makeHTMLgallery(scenes, dim);
        }

        private void browser_Load(object sender, EventArgs e)
        {
            mode = BrowserMode.ExemplarBrowsing;
            showThumbnails(database.getExemplars(), Constants.exemplarImgDim);
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
    }
}
