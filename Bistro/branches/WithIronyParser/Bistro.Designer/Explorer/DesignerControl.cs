using System.Windows.Forms;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Diagnostics;
using Bistro.MethodsEngine;
using Bistro.MethodsEngine.Reflection;

namespace Bistro.Designer.Explorer
{
    public partial class DesignerControl : UserControl
    {
        public Dictionary<string, List<ControllerDescription>> cashPatternsCtrl;
        public Dictionary<string, List<ControllerDescription>> dllPatternsCtrl;
        public Dictionary<string, Dictionary<string, Resource>> cashPatternsRes;
        public DesignerControl()
        {
            InitializeComponent();
            TreeNode application = new TreeNode("Controllers");
            cashPatternsCtrl = new Dictionary<string, List<ControllerDescription>>();
            cashPatternsRes = new Dictionary<string, Dictionary<string, Resource>>();
            propertiesTree.ImageList = imageList1;
        }
        public Bistro.MethodsEngine.Engine Engine
        {
            get { return engine; }
            set { engine = value; }
        }
        public TreeView BindingTree
        {
            get { return bindingTree; }
        }
        public TreeView PropertiesTree
        {
            get { return propertiesTree; }
        }
        /// <summary> 
        /// Let this control process the mnemonics.
        /// </summary>
        [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogChar(char charCode)
        {
            // If we're the top-level form or control, we need to do the mnemonic handling
            if (charCode != ' ' && ProcessMnemonic(charCode))
            {
                return true;
            }
            return base.ProcessDialogChar(charCode);
        }

        /// <summary>
        /// Enable the IME status handling for this control.
        /// </summary>
        protected override bool CanEnableIme
        {
            get
            {
                return true;
            }
        }
        #region Private members
        private Bistro.MethodsEngine.Engine engine;
        private ControllerDescription curCtrl;
        private Resource curResource;
        private NodeObject curObject;
        private enum NodeObject
        {
            UrlPattern,
            Controller,
            Resource
        }


        private void BindingTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertiesTree.Nodes.Clear();
            if (e.Node.Tag != null)
            {
                System.Type nodeType = e.Node.Tag.GetType();
                if (nodeType == typeof(ControllerDescription))
                {
                    //store controller's name and value to be able 
                    //to show additional info On_ShowBindingsResources

                    curObject = NodeObject.Controller;
                    curCtrl = (ControllerDescription)e.Node.Tag;
                }
                else if (nodeType == typeof(Resource))
                {
                    curObject = NodeObject.Resource;
                    curResource = (Resource)e.Node.Tag;
                }
            }
        }
        private void On_MethodShowBindings(object sender, System.EventArgs e)
        {
            Debug.WriteLine("click on url pattern");
        }
        private void On_ShowBindingsResources(object sender, System.EventArgs e)
        {
            Debug.WriteLine("show controller's bindings and resources");
            List<string> targs = new List<string>();
            List<string> resources = new List<string>();
            if (curCtrl != null)
            {
                foreach (IMethodsBindPointDesc bp in  curCtrl.Targets)
                {
                    if (!targs.Contains(bp.Target))
                    {
                        propertiesTree.Nodes.Add("Binding " + bp.Target);
                        propertiesTree.Nodes[propertiesTree.Nodes.Count - 1].ImageIndex = 0;
                        propertiesTree.Nodes[propertiesTree.Nodes.Count - 1].SelectedImageIndex = 0;
                        targs.Add(bp.Target);
                    }
                }
                foreach (string res in curCtrl.DependsOn)
                {
                    if (!resources.Contains(res))
                    {
                        propertiesTree.Nodes.Add("Resource " + res);
                        propertiesTree.Nodes[propertiesTree.Nodes.Count - 1].ImageIndex = 3;
                        propertiesTree.Nodes[propertiesTree.Nodes.Count - 1].SelectedImageIndex = 3;
                        resources.Add(res);
                    }
                }
                foreach (string res in curCtrl.Requires)
                {
                    if (!resources.Contains(res))
                    {
                        propertiesTree.Nodes.Add("Resource " + res);
                        propertiesTree.Nodes[propertiesTree.Nodes.Count - 1].ImageIndex = 2;
                        propertiesTree.Nodes[propertiesTree.Nodes.Count - 1].SelectedImageIndex = 2;
                        resources.Add(res);
                    }
                }
                foreach (string res in curCtrl.Provides)
                {
                    if (!resources.Contains(res))
                    {
                        propertiesTree.Nodes.Add("Resource " + res);
                        propertiesTree.Nodes[propertiesTree.Nodes.Count - 1].ImageIndex = 1;
                        propertiesTree.Nodes[propertiesTree.Nodes.Count - 1].SelectedImageIndex = 1;
                        resources.Add(res);
                    }
                }


            }
        }
        #endregion

    }
}
