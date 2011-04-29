namespace SceneStudioApp
{
    partial class HelpScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpScreen));
            this.nameLabel = new System.Windows.Forms.Label();
            this.helpTextBox = new System.Windows.Forms.RichTextBox();
            this.iconBox = new System.Windows.Forms.PictureBox();
            this.helpExplLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.iconBox)).BeginInit();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameLabel.Location = new System.Drawing.Point(58, 22);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(247, 19);
            this.nameLabel.TabIndex = 1;
            this.nameLabel.Text = "Hover over a button for more info.";
            // 
            // helpTextBox
            // 
            this.helpTextBox.BackColor = System.Drawing.Color.White;
            this.helpTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.helpTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpTextBox.Location = new System.Drawing.Point(12, 59);
            this.helpTextBox.Name = "helpTextBox";
            this.helpTextBox.ReadOnly = true;
            this.helpTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.helpTextBox.Size = new System.Drawing.Size(300, 276);
            this.helpTextBox.TabIndex = 4;
            this.helpTextBox.Text = "";
            this.helpTextBox.Enter += new System.EventHandler(this.helpTextBox_Enter);
            // 
            // iconBox
            // 
            this.iconBox.Location = new System.Drawing.Point(12, 12);
            this.iconBox.Name = "iconBox";
            this.iconBox.Size = new System.Drawing.Size(40, 40);
            this.iconBox.TabIndex = 0;
            this.iconBox.TabStop = false;
            // 
            // helpExplLabel
            // 
            this.helpExplLabel.AutoSize = true;
            this.helpExplLabel.Location = new System.Drawing.Point(12, 341);
            this.helpExplLabel.Name = "helpExplLabel";
            this.helpExplLabel.Size = new System.Drawing.Size(271, 13);
            this.helpExplLabel.TabIndex = 5;
            this.helpExplLabel.Text = "Mouse over a toolbar button to display help for that item.";
            // 
            // HelpScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 363);
            this.Controls.Add(this.helpExplLabel);
            this.Controls.Add(this.helpTextBox);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.iconBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HelpScreen";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Help";
            ((System.ComponentModel.ISupportInitialize)(this.iconBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox iconBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.RichTextBox helpTextBox;
        private System.Windows.Forms.Label helpExplLabel;
    }
}