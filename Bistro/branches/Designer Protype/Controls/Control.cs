using System.Security.Permissions;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using TestDate;
using Controls.Nodes;

namespace Bistro.Designer.Explorer
{
    /// <summary>
    /// Summary description for MyControl.
    /// </summary>
    public partial class Control : UserControl
    {
        ExplorerNode root = null;
        public Control()
        {
            InitializeComponent();
        }

        public ExplorerNode Root {
            get { return root; }
            set { root = value; }
        
        }

        public void CreateRootNode(TestDescriptor td)
        {
            this.Root = new Controls.Nodes.ApplicationNode(td);
            this.PaintTreeNode(root);
        }

        public void CreateTreeNodes(Bistro.Methods.Binding binding)
        {
            root = new BistroNode(root, binding);
            
            this.PaintTreeNode(root);

        }

        public void PaintTreeNode(ExplorerNode node)
        {
            node.Paint(ControllerView);
        }
               
        internal void SetSelected(ExplorerNode node)
        {
            foreach (TreeNode treeNode in ControllerView.Nodes.Find(node.Name, true))
                if (treeNode == node.TreeNode)
                {
                    treeNode.Expand();
                    ControllerView.SelectedNode = treeNode;
                }
        }

        private void NodeTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //this.SetSelected(e.Node.Tag);
        }

        private void NodeTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            ((ExplorerNode)e.Node.Tag).BeforeLabelEdit(sender, e);
        }

        private void NodeTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            ((ExplorerNode)e.Node.Tag).AfterLabelEdit(sender, e);
        }

        private void NodeTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            ExplorerNode node = e.Node.Tag as ExplorerNode;
            if (node == null)
                return;
            node.BeforeExpand(sender, e);
        }

        private void NodeTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            ExplorerNode node = e.Node.Tag as ExplorerNode;
            if (node == null)
                return;
            node.AfterExpand(sender, e);
        }

        private void NodeTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            ExplorerNode node = e.Node.Tag as ExplorerNode;
            if (node == null)
                return;
            node.AfterCollapse(sender, e);
        }

        private void NodeTree_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TreeNode treeNode = ControllerView.GetNodeAt(e.Location);
            if (treeNode != null)
            {
                ExplorerNode node = treeNode.Tag as ExplorerNode;
                if (node != null)
                    node.DoubleClick(sender, e);
            }
        }
    }
}
