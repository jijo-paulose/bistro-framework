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
            this.TreeIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.TreeIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.TreeIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // Control
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.ControllerView);
            this.Name = "Control";
            this.Size = new System.Drawing.Size(250, 369);
            this.NodeMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TreeView ControllerView;
        private System.Windows.Forms.ContextMenuStrip NodeMenu;
        private System.Windows.Forms.ToolStripMenuItem showSourceToolStripMenuItem;
        private System.Windows.Forms.ImageList TreeIcons;

    }
}
