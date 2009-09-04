using System.Security.Permissions;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using TestDate;
using Controls.Nodes;
using System.Collections.Generic;
using System.Collections;

namespace Bistro.Designer.Explorer
{
    /// <summary>
    /// Summary description for MyControl.
    /// </summary>
    public partial class Control : UserControl
    {
        ExplorerNode root = null;
        IList<string> info = new List<string>();
    
 
        public Control()
        {
            InitializeComponent();
           
        }

        public ExplorerNode Root {
            get { return root; }
            set { root = value; }
        
        }

        public IList<string> Items
        {
            get { return info; }
            set { info = value; }

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
            //Controller provider
            info.Clear();
            if (e.Node.Name == "DataAccessControl") 
            {
                info.Add("Resources:");
                info.Add("Resource1");
                info.Add("Resource2");
            }
            if (e.Node.Name == "AdDisplay")
            {
                info.Add("Resources:");
                info.Add("Resource1");
                info.Add("Resource3");
            }
            if (e.Node.Name == "AdUpdate")
            {
                info.Add("Resources:");
                info.Add("Resource1");
                info.Add("Resource5");
            }
   
            //Resources provider
            if (e.Node.Name == "Resource1")
            {
                info.Add("Used Controllers:");
                info.Add("DataAccessControl");
                info.Add("AdDisplay");
                info.Add("AdUpdate");
            }
            if (e.Node.Name == "Resource2")
            {
                info.Add("Used Controller:");
                info.Add("DataAccessControl");
            }
            if (e.Node.Name == "Resource3")
            {
                info.Add("Used Controller:");
                info.Add("AdDisplay");
            }
            if (e.Node.Name == "Resource5")
            {
                info.Add("Used Controller:");
                info.Add("AdUpdate");
            }
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

        private void Control_DoubleClick(object sender, EventArgs e)
        {

        }
    }
}
