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
                new ControllerNode(parent, item.Type, ControllerNode.ControllerNodeType.None);
            }
        }

    }
}
