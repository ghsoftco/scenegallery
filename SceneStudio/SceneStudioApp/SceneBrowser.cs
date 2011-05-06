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
        private BrowserMode mode;
        private List<SceneEntry> pickedModels;

        public SceneBrowser(Database _database)
        {
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
            showThumbnails(database.getScenesFromHashes(database.getExemplar(tag).modelHashes), Constants.sceneImgDim);
            mode = BrowserMode.ModelBrowsing;
        }
        private void modelClicked(string tag)
        {
            SceneEntry model = database.getScene(tag);
            showThumbnails(database.getExemplars(), Constants.exemplarImgDim);
            mode = BrowserMode.ExemplarBrowsing;
        }
    }
}
