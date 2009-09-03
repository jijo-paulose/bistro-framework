using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BistroDesignerMockUp
{
    public partial class DesignerControl : Form
    {
        public DesignerControl()
        {
            InitializeComponent();
            TreeNode application = new TreeNode("No Recruters");
            BindingTree.Nodes.Add(application);
            
            TreeNode url_a = new TreeNode("/a");
            url_a.ContextMenuStrip = MethodMenu;
            application.Nodes.Add(url_a);
            
            TreeNode url_a_c1 = new TreeNode("Controller c1");
            url_a_c1.Tag = "Controller";
            url_a_c1.ContextMenuStrip = ControllerMenu;
            url_a.Nodes.Add(url_a_c1);

            TreeNode url_a_r1 = new TreeNode("Resource r1");
            url_a_r1.Tag = "Resource";
            url_a.Nodes.Add(url_a_r1);

            TreeNode url_b = new TreeNode("/b");
            application.Nodes.Add(url_b);
        }

        private void BindingTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            PropertiesTree.Nodes.Clear();
            switch (Convert.ToString(e.Node.Tag))
            {
                case "Controller":
                    ShowController(e.Node);
                    break;
                case "Resource":
                    ShowResource(e.Node);
                    break;
                default:
                    break;
            }
        }

        private void ShowController(TreeNode treeNode)
        {
            TreeNode r1 = new TreeNode("Resource r1");
            PropertiesTree.Nodes.Add(r1);
            TreeNode b1 = new TreeNode("Binding /a");
            PropertiesTree.Nodes.Add(b1);
        }

        private void ShowResource(TreeNode treeNode)
        {
            TreeNode c1 = new TreeNode("Controller c1");
            PropertiesTree.Nodes.Add(c1);
        }

    }
}
