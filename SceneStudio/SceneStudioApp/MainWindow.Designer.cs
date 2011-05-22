namespace SceneStudioApp
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            this.toolBar = new System.Windows.Forms.ToolStrip();
            this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveAsStripButton = new System.Windows.Forms.ToolStripButton();
            this.uploadToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.clearToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.rotateCCWToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.rotateCWToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.scaleUpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.scaleDownToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.flipNToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.flipMToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.revertObjectToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.deleteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.undoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.resetCameraToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.webPageToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.resetExemplarsButton = new System.Windows.Forms.Button();
            this.modelNameLabel = new System.Windows.Forms.Label();
            this.exemplarSearchButton = new System.Windows.Forms.Button();
            this.exemplarBrowser = new System.Windows.Forms.WebBrowser();
            this.exemplarSearchBox = new System.Windows.Forms.TextBox();
            this.modelBrowser = new System.Windows.Forms.WebBrowser();
            this.modelSearchButton = new System.Windows.Forms.Button();
            this.modelSearchBox = new System.Windows.Forms.TextBox();
            this.timerRender = new System.Windows.Forms.Timer(this.components);
            this.menuBar = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.timerDownloadCheck = new System.Windows.Forms.Timer(this.components);
            this.toolBar.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.menuBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBar
            // 
            this.toolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.saveAsStripButton,
            this.uploadToolStripButton,
            this.toolStripSeparator,
            this.clearToolStripButton,
            this.rotateCCWToolStripButton,
            this.rotateCWToolStripButton,
            this.scaleUpToolStripButton,
            this.scaleDownToolStripButton,
            this.flipNToolStripButton,
            this.flipMToolStripButton,
            this.revertObjectToolStripButton,
            this.toolStripSeparator4,
            this.copyToolStripButton,
            this.pasteToolStripButton,
            this.deleteToolStripButton,
            this.undoToolStripButton,
            this.toolStripSeparator1,
            this.resetCameraToolStripButton,
            this.helpToolStripButton,
            this.webPageToolStripButton});
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(1426, 43);
            this.toolBar.TabIndex = 0;
            this.toolBar.Text = "toolStrip1";
            // 
            // newToolStripButton
            // 
            this.newToolStripButton.AutoSize = false;
            this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newToolStripButton.Image = global::SceneStudioApp.Properties.Resources.New_file;
            this.newToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.newToolStripButton.Name = "newToolStripButton";
            this.newToolStripButton.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.newToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.newToolStripButton.Text = "&New";
            this.newToolStripButton.ToolTipText = "New (Ctrl+N)";
            this.newToolStripButton.Click += new System.EventHandler(this.newToolStripButton_Click);
            this.newToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.newToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.AutoSize = false;
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Open_file;
            this.openToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.openToolStripButton.Text = "&Open";
            this.openToolStripButton.ToolTipText = "Open (Ctrl+O)";
            this.openToolStripButton.Click += new System.EventHandler(this.openToolStripButton_Click);
            this.openToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.openToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.AutoSize = false;
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Save;
            this.saveToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.saveToolStripButton.Text = "&Save";
            this.saveToolStripButton.ToolTipText = "Save (Ctrl+S)";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            this.saveToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.saveToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // saveAsStripButton
            // 
            this.saveAsStripButton.AutoSize = false;
            this.saveAsStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveAsStripButton.Image = global::SceneStudioApp.Properties.Resources.SaveAs;
            this.saveAsStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.saveAsStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.saveAsStripButton.Name = "saveAsStripButton";
            this.saveAsStripButton.Size = new System.Drawing.Size(40, 40);
            this.saveAsStripButton.Text = "&Save As";
            this.saveAsStripButton.ToolTipText = "Save As (Shift+Ctrl+S)";
            this.saveAsStripButton.Click += new System.EventHandler(this.saveAsStripButton_Click);
            // 
            // uploadToolStripButton
            // 
            this.uploadToolStripButton.AutoSize = false;
            this.uploadToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uploadToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Upload_image;
            this.uploadToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.uploadToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.uploadToolStripButton.Name = "uploadToolStripButton";
            this.uploadToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.uploadToolStripButton.Text = "Upload Scene";
            this.uploadToolStripButton.Click += new System.EventHandler(this.uploadToolStripButton_Click);
            this.uploadToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.uploadToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 43);
            // 
            // clearToolStripButton
            // 
            this.clearToolStripButton.AutoSize = false;
            this.clearToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearToolStripButton.Image = global::SceneStudioApp.Properties.Resources.No;
            this.clearToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.clearToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.clearToolStripButton.Name = "clearToolStripButton";
            this.clearToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.clearToolStripButton.Text = "Clear Selection (Esc)";
            this.clearToolStripButton.Click += new System.EventHandler(this.clearToolStripButton_Click);
            this.clearToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.clearToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // rotateCCWToolStripButton
            // 
            this.rotateCCWToolStripButton.AutoSize = false;
            this.rotateCCWToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateCCWToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Rotate_CCW;
            this.rotateCCWToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rotateCCWToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.rotateCCWToolStripButton.Name = "rotateCCWToolStripButton";
            this.rotateCCWToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.rotateCCWToolStripButton.Text = "Rotate CCW (Left)";
            this.rotateCCWToolStripButton.ToolTipText = "Rotate CCW (Left Arrow)";
            this.rotateCCWToolStripButton.Click += new System.EventHandler(this.rotateCCWToolStripButton_Click);
            this.rotateCCWToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.rotateCCWToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // rotateCWToolStripButton
            // 
            this.rotateCWToolStripButton.AutoSize = false;
            this.rotateCWToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateCWToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Rotate_CW;
            this.rotateCWToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rotateCWToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.rotateCWToolStripButton.Name = "rotateCWToolStripButton";
            this.rotateCWToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.rotateCWToolStripButton.Text = "Rotate CW (Right)";
            this.rotateCWToolStripButton.ToolTipText = "Rotate CW (Right Arrow)";
            this.rotateCWToolStripButton.Click += new System.EventHandler(this.rotateCWToolStripButton_Click);
            this.rotateCWToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.rotateCWToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // scaleUpToolStripButton
            // 
            this.scaleUpToolStripButton.AutoSize = false;
            this.scaleUpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.scaleUpToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Up;
            this.scaleUpToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.scaleUpToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.scaleUpToolStripButton.Name = "scaleUpToolStripButton";
            this.scaleUpToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.scaleUpToolStripButton.Text = "Increase Size (Up)";
            this.scaleUpToolStripButton.ToolTipText = "Increase Size (Up Arrow)";
            this.scaleUpToolStripButton.Click += new System.EventHandler(this.scaleUpToolStripButton_Click);
            this.scaleUpToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.scaleUpToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseEnter);
            // 
            // scaleDownToolStripButton
            // 
            this.scaleDownToolStripButton.AutoSize = false;
            this.scaleDownToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.scaleDownToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Down;
            this.scaleDownToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.scaleDownToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.scaleDownToolStripButton.Name = "scaleDownToolStripButton";
            this.scaleDownToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.scaleDownToolStripButton.Text = "Decrease Size (Down)";
            this.scaleDownToolStripButton.ToolTipText = "Decrease Size (Down Arrow)";
            this.scaleDownToolStripButton.Click += new System.EventHandler(this.scaleDownToolStripButton_Click);
            this.scaleDownToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.scaleDownToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // flipNToolStripButton
            // 
            this.flipNToolStripButton.AutoSize = false;
            this.flipNToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.flipNToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Rotate_left;
            this.flipNToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.flipNToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.flipNToolStripButton.Name = "flipNToolStripButton";
            this.flipNToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.flipNToolStripButton.Text = "Flip (N)";
            this.flipNToolStripButton.Click += new System.EventHandler(this.flipNToolStripButton_Click);
            this.flipNToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.flipNToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // flipMToolStripButton
            // 
            this.flipMToolStripButton.AutoSize = false;
            this.flipMToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.flipMToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Rotate_right;
            this.flipMToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.flipMToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.flipMToolStripButton.Name = "flipMToolStripButton";
            this.flipMToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.flipMToolStripButton.Text = "Flip (M)";
            this.flipMToolStripButton.Click += new System.EventHandler(this.flipMToolStripButton_Click);
            this.flipMToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.flipMToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // revertObjectToolStripButton
            // 
            this.revertObjectToolStripButton.AutoSize = false;
            this.revertObjectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.revertObjectToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Revert;
            this.revertObjectToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.revertObjectToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.revertObjectToolStripButton.Name = "revertObjectToolStripButton";
            this.revertObjectToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.revertObjectToolStripButton.Text = "Reset Rotation (B)";
            this.revertObjectToolStripButton.Click += new System.EventHandler(this.revertObjectToolStripButton_Click);
            this.revertObjectToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.revertObjectToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 43);
            // 
            // copyToolStripButton
            // 
            this.copyToolStripButton.AutoSize = false;
            this.copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Copy;
            this.copyToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.copyToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.copyToolStripButton.Name = "copyToolStripButton";
            this.copyToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.copyToolStripButton.Text = "Copy";
            this.copyToolStripButton.ToolTipText = "Copy (Ctrl+C)";
            this.copyToolStripButton.Click += new System.EventHandler(this.copyToolStripButton_Click);
            this.copyToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.copyToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // pasteToolStripButton
            // 
            this.pasteToolStripButton.AutoSize = false;
            this.pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Paste;
            this.pasteToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.pasteToolStripButton.Name = "pasteToolStripButton";
            this.pasteToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.pasteToolStripButton.Text = "Paste";
            this.pasteToolStripButton.ToolTipText = "Paste (Ctrl+V)";
            this.pasteToolStripButton.Click += new System.EventHandler(this.pasteToolStripButton_Click);
            this.pasteToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.pasteToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // deleteToolStripButton
            // 
            this.deleteToolStripButton.AutoSize = false;
            this.deleteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Erase;
            this.deleteToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.deleteToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.deleteToolStripButton.Name = "deleteToolStripButton";
            this.deleteToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.deleteToolStripButton.Text = "Delete";
            this.deleteToolStripButton.ToolTipText = "Delete (Del or Backspace)";
            this.deleteToolStripButton.Click += new System.EventHandler(this.deleteToolStripButton_Click);
            this.deleteToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.deleteToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // undoToolStripButton
            // 
            this.undoToolStripButton.AutoSize = false;
            this.undoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.undoToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Undo;
            this.undoToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.undoToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.undoToolStripButton.Name = "undoToolStripButton";
            this.undoToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.undoToolStripButton.Text = "Undo";
            this.undoToolStripButton.ToolTipText = "Undo (Ctrl+Z)";
            this.undoToolStripButton.Click += new System.EventHandler(this.undoToolStripButton_Click);
            this.undoToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.undoToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 43);
            // 
            // resetCameraToolStripButton
            // 
            this.resetCameraToolStripButton.AutoSize = false;
            this.resetCameraToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resetCameraToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Camera;
            this.resetCameraToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.resetCameraToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.resetCameraToolStripButton.Name = "resetCameraToolStripButton";
            this.resetCameraToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.resetCameraToolStripButton.Text = "Reset Camera (Ctrl+R)";
            this.resetCameraToolStripButton.Click += new System.EventHandler(this.resetCameraToolStripButton_Click);
            this.resetCameraToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.resetCameraToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // helpToolStripButton
            // 
            this.helpToolStripButton.AutoSize = false;
            this.helpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Help;
            this.helpToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.helpToolStripButton.Name = "helpToolStripButton";
            this.helpToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.helpToolStripButton.Text = "He&lp";
            this.helpToolStripButton.Click += new System.EventHandler(this.helpToolStripButton_Click);
            this.helpToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.helpToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // webPageToolStripButton
            // 
            this.webPageToolStripButton.AutoSize = false;
            this.webPageToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.webPageToolStripButton.Image = global::SceneStudioApp.Properties.Resources.Homepage;
            this.webPageToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.webPageToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.webPageToolStripButton.Name = "webPageToolStripButton";
            this.webPageToolStripButton.Size = new System.Drawing.Size(40, 40);
            this.webPageToolStripButton.Text = "Go to the SceneStudio home page";
            this.webPageToolStripButton.Click += new System.EventHandler(this.webPageToolStripButton_Click);
            this.webPageToolStripButton.MouseEnter += new System.EventHandler(this.toolStripButtons_MouseEnter);
            this.webPageToolStripButton.MouseLeave += new System.EventHandler(this.toolStripButtons_MouseLeave);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 43);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.statusBar);
            this.splitContainer1.Panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel1_MouseDown);
            this.splitContainer1.Panel1.MouseEnter += new System.EventHandler(this.splitContainer1_Panel1_MouseEnter);
            this.splitContainer1.Panel1.MouseLeave += new System.EventHandler(this.splitContainer1_Panel1_MouseLeave);
            this.splitContainer1.Panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel1_MouseMove);
            this.splitContainer1.Panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel1_MouseUp);
            this.splitContainer1.Panel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel1_MouseMove);
            this.splitContainer1.Panel1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.splitContainer1_Panel1_PreviewKeyDown);
            this.splitContainer1.Panel1.Resize += new System.EventHandler(this.splitContainer1_Panel1_Resize);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.resetExemplarsButton);
            this.splitContainer1.Panel2.Controls.Add(this.modelNameLabel);
            this.splitContainer1.Panel2.Controls.Add(this.exemplarSearchButton);
            this.splitContainer1.Panel2.Controls.Add(this.exemplarBrowser);
            this.splitContainer1.Panel2.Controls.Add(this.exemplarSearchBox);
            this.splitContainer1.Panel2.Controls.Add(this.modelBrowser);
            this.splitContainer1.Panel2.Controls.Add(this.modelSearchButton);
            this.splitContainer1.Panel2.Controls.Add(this.modelSearchBox);
            this.splitContainer1.Panel2.Resize += new System.EventHandler(this.splitContainer1_Panel2_Resize);
            this.splitContainer1.Panel2MinSize = 24;
            this.splitContainer1.Size = new System.Drawing.Size(1426, 905);
            this.splitContainer1.SplitterDistance = 842;
            this.splitContainer1.TabIndex = 1;
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.statusText});
            this.statusBar.Location = new System.Drawing.Point(0, 872);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(838, 29);
            this.statusBar.SizingGrip = false;
            this.statusBar.TabIndex = 0;
            this.statusBar.Text = "statusStrip";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(100, 23);
            this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // statusText
            // 
            this.statusText.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(51, 24);
            this.statusText.Text = "Ready";
            // 
            // resetExemplarsButton
            // 
            this.resetExemplarsButton.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resetExemplarsButton.Location = new System.Drawing.Point(7, 285);
            this.resetExemplarsButton.Name = "resetExemplarsButton";
            this.resetExemplarsButton.Size = new System.Drawing.Size(75, 24);
            this.resetExemplarsButton.TabIndex = 10;
            this.resetExemplarsButton.Text = "Reset";
            this.resetExemplarsButton.UseVisualStyleBackColor = true;
            this.resetExemplarsButton.Click += new System.EventHandler(this.resetExemplarsButton_Click);
            // 
            // modelNameLabel
            // 
            this.modelNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.modelNameLabel.AutoSize = true;
            this.modelNameLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.modelNameLabel.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modelNameLabel.Location = new System.Drawing.Point(3, 5);
            this.modelNameLabel.Name = "modelNameLabel";
            this.modelNameLabel.Size = new System.Drawing.Size(97, 19);
            this.modelNameLabel.TabIndex = 6;
            this.modelNameLabel.Text = "Model Name";
            this.modelNameLabel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.modelNameLabel_MouseDoubleClick);
            // 
            // exemplarSearchButton
            // 
            this.exemplarSearchButton.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exemplarSearchButton.Location = new System.Drawing.Point(498, 284);
            this.exemplarSearchButton.Name = "exemplarSearchButton";
            this.exemplarSearchButton.Size = new System.Drawing.Size(75, 24);
            this.exemplarSearchButton.TabIndex = 8;
            this.exemplarSearchButton.Text = "Search";
            this.exemplarSearchButton.UseVisualStyleBackColor = true;
            this.exemplarSearchButton.Click += new System.EventHandler(this.exemplarSearchButton_Click);
            // 
            // exemplarBrowser
            // 
            this.exemplarBrowser.AllowWebBrowserDrop = false;
            this.exemplarBrowser.IsWebBrowserContextMenuEnabled = false;
            this.exemplarBrowser.Location = new System.Drawing.Point(3, 314);
            this.exemplarBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.exemplarBrowser.Name = "exemplarBrowser";
            this.exemplarBrowser.Size = new System.Drawing.Size(570, 587);
            this.exemplarBrowser.TabIndex = 9;
            this.exemplarBrowser.WebBrowserShortcutsEnabled = false;
            this.exemplarBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.exemplarBrowser_Navigating);
            // 
            // exemplarSearchBox
            // 
            this.exemplarSearchBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exemplarSearchBox.Location = new System.Drawing.Point(192, 283);
            this.exemplarSearchBox.MinimumSize = new System.Drawing.Size(151, 26);
            this.exemplarSearchBox.Name = "exemplarSearchBox";
            this.exemplarSearchBox.Size = new System.Drawing.Size(300, 26);
            this.exemplarSearchBox.TabIndex = 7;
            this.exemplarSearchBox.Text = "Search exemplar scenes";
            this.exemplarSearchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.exemplarSearchBox_KeyDown);
            this.exemplarSearchBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.exemplarSearchBox_MouseDoubleClick);
            this.exemplarSearchBox.MouseEnter += new System.EventHandler(this.exemplarSearchBox_MouseEnter);
            this.exemplarSearchBox.MouseLeave += new System.EventHandler(this.exemplarSearchBox_MouseLeave);
            this.exemplarSearchBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.exemplarSearchBox_MouseUp);
            // 
            // modelBrowser
            // 
            this.modelBrowser.AllowWebBrowserDrop = false;
            this.modelBrowser.IsWebBrowserContextMenuEnabled = false;
            this.modelBrowser.Location = new System.Drawing.Point(3, 31);
            this.modelBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.modelBrowser.Name = "modelBrowser";
            this.modelBrowser.Size = new System.Drawing.Size(570, 244);
            this.modelBrowser.TabIndex = 4;
            this.modelBrowser.WebBrowserShortcutsEnabled = false;
            this.modelBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.modelBrowser_Navigating);
            // 
            // modelSearchButton
            // 
            this.modelSearchButton.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modelSearchButton.Location = new System.Drawing.Point(498, 2);
            this.modelSearchButton.Name = "modelSearchButton";
            this.modelSearchButton.Size = new System.Drawing.Size(75, 24);
            this.modelSearchButton.TabIndex = 3;
            this.modelSearchButton.Text = "Search";
            this.modelSearchButton.UseVisualStyleBackColor = true;
            this.modelSearchButton.Click += new System.EventHandler(this.modelSearchButton_Click);
            // 
            // modelSearchBox
            // 
            this.modelSearchBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modelSearchBox.Location = new System.Drawing.Point(192, 1);
            this.modelSearchBox.MinimumSize = new System.Drawing.Size(151, 26);
            this.modelSearchBox.Name = "modelSearchBox";
            this.modelSearchBox.Size = new System.Drawing.Size(300, 26);
            this.modelSearchBox.TabIndex = 2;
            this.modelSearchBox.Text = "Search objects";
            this.modelSearchBox.DoubleClick += new System.EventHandler(this.modelSearchBox_DoubleClick);
            this.modelSearchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.modelSearchTextBox_KeyDown);
            this.modelSearchBox.MouseEnter += new System.EventHandler(this.modelSearchBox_MouseEnter);
            this.modelSearchBox.MouseLeave += new System.EventHandler(this.modelSearchBox_MouseLeave);
            this.modelSearchBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.modelSearchBox_MouseUp);
            // 
            // timerRender
            // 
            this.timerRender.Interval = 300;
            this.timerRender.Tick += new System.EventHandler(this.timerRender_Tick);
            // 
            // menuBar
            // 
            this.menuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuBar.Location = new System.Drawing.Point(0, 0);
            this.menuBar.Name = "menuBar";
            this.menuBar.Size = new System.Drawing.Size(1426, 24);
            this.menuBar.TabIndex = 2;
            this.menuBar.Text = "menuStrip1";
            this.menuBar.Visible = false;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuNew,
            this.MenuOpen,
            this.toolStripSeparator3,
            this.MenuSave,
            this.MenuSaveAs,
            this.toolStripSeparator2,
            this.MenuExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // MenuNew
            // 
            this.MenuNew.Name = "MenuNew";
            this.MenuNew.ShortcutKeyDisplayString = "Ctrl+N";
            this.MenuNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.MenuNew.Size = new System.Drawing.Size(155, 22);
            this.MenuNew.Text = "New";
            this.MenuNew.Click += new System.EventHandler(this.MenuNew_Click);
            // 
            // MenuOpen
            // 
            this.MenuOpen.Name = "MenuOpen";
            this.MenuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MenuOpen.Size = new System.Drawing.Size(155, 22);
            this.MenuOpen.Text = "Open...";
            this.MenuOpen.Click += new System.EventHandler(this.MenuOpen_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(152, 6);
            // 
            // MenuSave
            // 
            this.MenuSave.Name = "MenuSave";
            this.MenuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.MenuSave.Size = new System.Drawing.Size(155, 22);
            this.MenuSave.Text = "Save";
            this.MenuSave.Click += new System.EventHandler(this.MenuSave_Click);
            // 
            // MenuSaveAs
            // 
            this.MenuSaveAs.Name = "MenuSaveAs";
            this.MenuSaveAs.Size = new System.Drawing.Size(155, 22);
            this.MenuSaveAs.Text = "Save As...";
            this.MenuSaveAs.Click += new System.EventHandler(this.MenuSaveAs_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(152, 6);
            // 
            // MenuExit
            // 
            this.MenuExit.Name = "MenuExit";
            this.MenuExit.Size = new System.Drawing.Size(155, 22);
            this.MenuExit.Text = "Exit";
            this.MenuExit.Click += new System.EventHandler(this.MenuExit_Click);
            // 
            // timerDownloadCheck
            // 
            this.timerDownloadCheck.Interval = 200;
            this.timerDownloadCheck.Tick += new System.EventHandler(this.timerDownloadCheck_Tick);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1426, 948);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.menuBar);
            this.Name = "MainWindow";
            this.Text = "SceneStudio";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.menuBar.ResumeLayout(false);
            this.menuBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolBar;
        private System.Windows.Forms.ToolStripButton newToolStripButton;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton copyToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton helpToolStripButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Timer timerRender;
        private System.Windows.Forms.Button modelSearchButton;
        private System.Windows.Forms.TextBox modelSearchBox;
        private System.Windows.Forms.WebBrowser modelBrowser;
        private System.Windows.Forms.Label modelNameLabel;
        private System.Windows.Forms.MenuStrip menuBar;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuNew;
        private System.Windows.Forms.ToolStripMenuItem MenuOpen;
        private System.Windows.Forms.ToolStripMenuItem MenuSave;
        private System.Windows.Forms.ToolStripMenuItem MenuSaveAs;
        private System.Windows.Forms.ToolStripMenuItem MenuExit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private System.Windows.Forms.Timer timerDownloadCheck;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton deleteToolStripButton;
        private System.Windows.Forms.ToolStripButton undoToolStripButton;
        private System.Windows.Forms.ToolStripButton resetCameraToolStripButton;
        private System.Windows.Forms.ToolStripButton pasteToolStripButton;
        private System.Windows.Forms.ToolStripButton rotateCCWToolStripButton;
        private System.Windows.Forms.ToolStripButton rotateCWToolStripButton;
        private System.Windows.Forms.ToolStripButton scaleUpToolStripButton;
        private System.Windows.Forms.ToolStripButton scaleDownToolStripButton;
        private System.Windows.Forms.ToolStripButton flipNToolStripButton;
        private System.Windows.Forms.ToolStripButton flipMToolStripButton;
        private System.Windows.Forms.ToolStripButton revertObjectToolStripButton;
        private System.Windows.Forms.ToolStripButton uploadToolStripButton;
        private System.Windows.Forms.ToolStripButton clearToolStripButton;
        private System.Windows.Forms.ToolStripButton webPageToolStripButton;
        private System.Windows.Forms.ToolStripButton saveAsStripButton;
        private System.Windows.Forms.WebBrowser exemplarBrowser;
        private System.Windows.Forms.TextBox exemplarSearchBox;
        private System.Windows.Forms.Button exemplarSearchButton;
        private System.Windows.Forms.Button resetExemplarsButton;
    }
}