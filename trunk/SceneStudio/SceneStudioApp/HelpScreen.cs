using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SceneStudioApp
{
    public partial class HelpScreen : Form
    {
        public HelpScreen()
        {
            InitializeComponent();
        }

        public void UpdateContents(ToolStripButton button)
        {
            this.nameLabel.Text = button.ToolTipText;
            this.iconBox.Image = button.Image;

            string propName = button.Name + "_help";
            this.helpTextBox.Text = SceneStudioApp.Properties.Resources.ResourceManager.GetString(propName);
        }

        private void helpTextBox_Enter(object sender, EventArgs e)
        {
            // Kill focus so we don't get the blinky text cursor
            // that indicates an editable text box.
            this.iconBox.Focus();
        }
    }
}
