namespace Bistro.Designer.Explorer
{
    partial class Control
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }


        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Control));
            this.ControllerView = new System.Windows.Forms.TreeView();
            this.NodeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TreeIcons = new System.Windows.Forms.ImageList(this.components);
            this.NodeMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ControllerView
            // 
            this.ControllerView.ContextMenuStrip = this.NodeMenu;
            this.ControllerView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ControllerView.ImageIndex = 0;
            this.ControllerView.ImageList = this.TreeIcons;
            this.ControllerView.Location = new System.Drawing.Point(0, 0);
            this.ControllerView.Name = "ControllerView";
            this.ControllerView.SelectedImageIndex = 0;
            this.ControllerView.Size = new System.Drawing.Size(250, 369);
            this.ControllerView.TabIndex = 0;
            // 
            // NodeMenu
            // 
            this.NodeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showSourceToolStripMenuItem});
            this.NodeMenu.Name = "NodeMenu";
            this.NodeMenu.Size = new System.Drawing.Size(148, 26);
            // 
            // showSourceToolStripMenuItem
            // 
            this.showSourceToolStripMenuItem.Name = "showSourceToolStripMenuItem";
            this.showSourceToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.showSourceToolStripMenuItem.Text = "Show Source";
            // 
            // TreeIcons
            // 
            this.TreeIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TreeIcons.ImageStream")));
            this.TreeIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.TreeIcons.Images.SetKeyName(0, "Action.bmp");
            this.TreeIcons.Images.SetKeyName(1, "Activities.bmp");
            this.TreeIcons.Images.SetKeyName(2, "Activity.bmp");
            this.TreeIcons.Images.SetKeyName(3, "Application.bmp");
            this.TreeIcons.Images.SetKeyName(4, "Binder.bmp");
            this.TreeIcons.Images.SetKeyName(5, "Binders.bmp");
            this.TreeIcons.Images.SetKeyName(6, "Document.bmp");
            this.TreeIcons.Images.SetKeyName(7, "Documents.bmp");
            this.TreeIcons.Images.SetKeyName(8, "Error.bmp");
            this.TreeIcons.Images.SetKeyName(9, "Field.bmp");
            this.TreeIcons.Images.SetKeyName(10, "FieldSelected.bmp");
            this.TreeIcons.Images.SetKeyName(11, "Folder.bmp");
            this.TreeIcons.Images.SetKeyName(12, "Link.bmp");
            this.TreeIcons.Images.SetKeyName(13, "ModelExplorerToolWindowBitmaps.bmp");
            this.TreeIcons.Images.SetKeyName(14, "OpenFolder.bmp");
            this.TreeIcons.Images.SetKeyName(15, "OpenFolderYellow.bmp");
            this.TreeIcons.Images.SetKeyName(16, "Resource.bmp");
            this.TreeIcons.Images.SetKeyName(17, "ResourceConsumer.bmp");
            this.TreeIcons.Images.SetKeyName(18, "ResourceProvider.bmp");
            // 
            // Control
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.ControllerView);
            this.Name = "Control";
            this.Size = new System.Drawing.Size(250, 369);
            this.NodeMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.ControllerView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NodeTree_MouseDoubleClick);
            this.ControllerView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.NodeTree_AfterCollapse);
            this.ControllerView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.NodeTree_AfterLabelEdit);
            this.ControllerView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.NodeTree_BeforeExpand);
            this.ControllerView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.NodeTree_AfterSelect);
            this.ControllerView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.NodeTree_BeforeLabelEdit);
            this.ControllerView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.NodeTree_AfterExpand);
        }
        #endregion

        public System.Windows.Forms.TreeView ControllerView;
        private System.Windows.Forms.ContextMenuStrip NodeMenu;
        private System.Windows.Forms.ToolStripMenuItem showSourceToolStripMenuItem;
        private System.Windows.Forms.ImageList TreeIcons;

    }
}
