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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesignerControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.bindingTree = new System.Windows.Forms.TreeView();
            this.propertiesTree = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.comboBox1);
            this.splitContainer1.Panel1.Controls.Add(this.bindingTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertiesTree);
            this.splitContainer1.Size = new System.Drawing.Size(267, 612);
            this.splitContainer1.SplitterDistance = 389;
            this.splitContainer1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Choose project:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(93, 0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(174, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // bindingTree
            // 
            this.bindingTree.Location = new System.Drawing.Point(0, 21);
            this.bindingTree.Name = "bindingTree";
            this.bindingTree.Size = new System.Drawing.Size(267, 391);
            this.bindingTree.TabIndex = 0;
            this.bindingTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.BindingTree_AfterSelect);
            this.bindingTree.Click += new System.EventHandler(this.On_Click);
            // 
            // propertiesTree
            // 
            this.propertiesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesTree.Location = new System.Drawing.Point(0, 0);
            this.propertiesTree.Name = "propertiesTree";
            this.propertiesTree.Size = new System.Drawing.Size(267, 219);
            this.propertiesTree.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "mechanic.gif");
            this.imageList1.Images.SetKeyName(1, "green_star.png");
            this.imageList1.Images.SetKeyName(2, "lock.gif");
            this.imageList1.Images.SetKeyName(3, "alert.png");
            this.imageList1.Images.SetKeyName(4, "bullet.png");
            this.imageList1.Images.SetKeyName(5, "white.PNG");
            // 
            // DesignerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "DesignerControl";
            this.Size = new System.Drawing.Size(267, 612);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView bindingTree;
        private System.Windows.Forms.TreeView propertiesTree;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        //internal System.Windows.Forms.ToolStripMenuItem showBindingsToolStripMenuItem;
        //internal System.Windows.Forms.ToolStripMenuItem showSourceToolStripMenuItem;
        //internal System.Windows.Forms.ToolStripMenuItem showBindingsResourcesToolStripMenuItem;
        //internal System.Windows.Forms.ContextMenuStrip MethodMenu;
        //internal System.Windows.Forms.ContextMenuStrip ControllerMenu;
    }
}