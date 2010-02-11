namespace Bistro.Designer.Explorer
{
    partial class Explorer
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Controllers");
            this.treeView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Controllers";
            this.treeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeView.Size = new System.Drawing.Size(179, 180);
            this.treeView.TabIndex = 1;
            // 
            // Explorer
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.treeView);
            this.Name = "Explorer";
            this.Size = new System.Drawing.Size(179, 198);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TreeView treeView;
    }
}
