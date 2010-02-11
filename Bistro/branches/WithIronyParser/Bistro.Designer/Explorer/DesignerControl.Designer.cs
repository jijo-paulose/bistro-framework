namespace Bistro.Designer.Explorer
{
    partial class DesignerControl
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.bindingTree = new System.Windows.Forms.TreeView();
            this.propertiesTree = new System.Windows.Forms.TreeView();
            this.MethodMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showBindingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ControllerMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showBindingsResourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.MethodMenu.SuspendLayout();
            this.ControllerMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.bindingTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertiesTree);
            this.splitContainer1.Size = new System.Drawing.Size(267, 612);
            this.splitContainer1.SplitterDistance = 389;
            this.splitContainer1.TabIndex = 0;
            // 
            // bindingTree
            // 
            this.bindingTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bindingTree.Location = new System.Drawing.Point(0, 0);
            this.bindingTree.Name = "bindingTree";
            this.bindingTree.Size = new System.Drawing.Size(267, 389);
            this.bindingTree.TabIndex = 0;
            this.bindingTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.BindingTree_AfterSelect);
            // 
            // propertiesTree
            // 
            this.propertiesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesTree.Location = new System.Drawing.Point(0, 0);
            this.propertiesTree.Name = "propertiesTree";
            this.propertiesTree.Size = new System.Drawing.Size(267, 219);
            this.propertiesTree.TabIndex = 0;
            // 
            // MethodMenu
            // 
            this.MethodMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showBindingsToolStripMenuItem});
            this.MethodMenu.Name = "MethodMenu";
            this.MethodMenu.Size = new System.Drawing.Size(154, 48);
            // 
            // showBindingsToolStripMenuItem
            // 
            this.showBindingsToolStripMenuItem.Name = "showBindingsToolStripMenuItem";
            this.showBindingsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.showBindingsToolStripMenuItem.Text = "Show Bindings";
            this.showBindingsToolStripMenuItem.Click += new System.EventHandler(this.On_MethodShowBindings);
            // 
            // ControllerMenu
            // 
            this.ControllerMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showSourceToolStripMenuItem,
            this.showBindingsResourcesToolStripMenuItem});
            this.ControllerMenu.Name = "contextMenuStrip1";
            this.ControllerMenu.Size = new System.Drawing.Size(208, 48);
            // 
            // showSourceToolStripMenuItem
            // 
            this.showSourceToolStripMenuItem.Name = "showSourceToolStripMenuItem";
            this.showSourceToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.showSourceToolStripMenuItem.Text = "Go To Source";
            // 
            // showBindingsResourcesToolStripMenuItem
            // 
            this.showBindingsResourcesToolStripMenuItem.Name = "showBindingsResourcesToolStripMenuItem";
            this.showBindingsResourcesToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.showBindingsResourcesToolStripMenuItem.Text = "Show Bindings/Resources";
            this.showBindingsResourcesToolStripMenuItem.Click += new System.EventHandler(this.On_ShowBindingsResources);
            // 
            // DesignerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "DesignerControl";
            this.Size = new System.Drawing.Size(267, 612);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.MethodMenu.ResumeLayout(false);
            this.ControllerMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView bindingTree;
        internal System.Windows.Forms.ContextMenuStrip MethodMenu;
        private System.Windows.Forms.TreeView propertiesTree;
        internal System.Windows.Forms.ToolStripMenuItem showBindingsToolStripMenuItem;
        internal System.Windows.Forms.ContextMenuStrip ControllerMenu;
        internal System.Windows.Forms.ToolStripMenuItem showSourceToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem showBindingsResourcesToolStripMenuItem;
    }
}