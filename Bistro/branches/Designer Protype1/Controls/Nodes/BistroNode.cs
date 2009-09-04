using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Designer.Explorer;
using Bistro.Methods;

namespace Controls.Nodes
{
    public class BistroNode : ExpandableNode
    {
        #region Constants
        const string BISTRO_NODE_NAME = "Bistro";
        #endregion

        #region Var definitions
        private MethodsNode methods;
        #endregion

        #region Construction
        public BistroNode(ExplorerNode root, Binding binding)
            : base(root, BISTRO_NODE_NAME, BINDERS_ICON, OPEN_FOLDER_ICON)
        {
            methods = new MethodsNode(this);
            CreateMethodsTreeFromBinding(methods, binding);
        }
        #endregion
        private void CreateMethodsTreeFromBinding(ExplorerNode parent, Binding binding)
        {
            foreach (Bistro.Methods.Binding item in binding.Bindings)
            {
                BindingNode newParent = new BindingNode(parent, item.Verb, item.BindingUrl);
                if (item.Bindings.Count > 0)
                {
                    CreateMethodsTreeFromBinding(new BindingNode(parent, item.Verb, item.BindingUrl), item);

                }
                
                if (item.Controllers.Count > 0)
                {
                    CreateControllersTreeFromBinding(newParent, item);
                }

            }
        }

        private void CreateControllersTreeFromBinding(ExplorerNode parent, Binding binding)
        {
            foreach (Bistro.Methods.Controller item in binding.Controllers)
            {
                ControllerNode parent1 = new ControllerNode(parent, item.Type, ControllerNode.ControllerNodeType.None);
                
                if (parent1.Name.CompareTo("DataAccessControl")== 0){
                    new ResourceNode(parent1, "Resource1", ControllerNode.ControllerNodeType.Consumer);
                    new ResourceNode(parent1, "Resource2", ControllerNode.ControllerNodeType.Provider);
                }
                if (parent1.Name.CompareTo("AdDisplay") == 0)
                {
                    new ResourceNode(parent1, "Resource3", ControllerNode.ControllerNodeType.Consumer);
                    new ResourceNode(parent1, "Resource1", ControllerNode.ControllerNodeType.Provider);
                }
                if (parent1.Name.CompareTo("AdUpdate") == 0)
                {
                    new ResourceNode(parent1, "Resource5", ControllerNode.ControllerNodeType.Consumer);
                    new ResourceNode(parent1, "Resource1", ControllerNode.ControllerNodeType.Provider);
                }
            }
        }

    }
}
