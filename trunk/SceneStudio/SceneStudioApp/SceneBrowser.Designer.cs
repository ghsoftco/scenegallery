namespace SceneStudioApp
{
    partial class SceneBrowser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.keywordSearchTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.AllowWebBrowserDrop = false;
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.Location = new System.Drawing.Point(0, 37);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(1284, 825);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser_Navigating);
            this.webBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.webBrowser_PreviewKeyDown);
            // 
            // keywordSearchTextBox
            // 
            this.keywordSearchTextBox.Location = new System.Drawing.Point(10, 11);
            this.keywordSearchTextBox.Name = "keywordSearchTextBox";
            this.keywordSearchTextBox.Size = new System.Drawing.Size(300, 20);
            this.keywordSearchTextBox.TabIndex = 1;
            this.keywordSearchTextBox.Text = "Search by keyword";
            this.keywordSearchTextBox.TextChanged += new System.EventHandler(this.keywordSearchTextBox_TextChanged);
            this.keywordSearchTextBox.Enter += new System.EventHandler(this.keywordSearchTextBox_Enter);
            this.keywordSearchTextBox.Leave += new System.EventHandler(this.keywordSearchTextBox_Leave);
            this.keywordSearchTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keywordSearchTextBox_MouseUp);
            // 
            // SceneBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 862);
            this.Controls.Add(this.keywordSearchTextBox);
            this.Controls.Add(this.webBrowser);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SceneBrowser";
            this.Text = "ExemplarBrowser";
            this.Load += new System.EventHandler(this.browser_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.TextBox keywordSearchTextBox;
    }
}