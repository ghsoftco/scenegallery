using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SceneStudioApp
{
    public partial class RoomSelection : Form
    {
        public string result;
        public RoomSelection()
        {
            InitializeComponent();
        }

        private void SelectRoom()
        {
            if(sceneList.SelectedIndices.Count == 1)
            {
                result = images.Images.Keys[sceneList.SelectedIndices[0]];
                result = result.Replace(".png", "");
                if (result.Length > 0)
                {
                    this.Close();
                }
            }
        }

        private void sceneList_DoubleClick(object sender, EventArgs e)
        {
            SelectRoom();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectRoom();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            result = "";
            this.Close();
        }

        private void RoomSelection_Load(object sender, EventArgs e)
        {

        }
    }
}
