using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Designer.Explorer;

namespace Controls.Nodes
{
    class ResourceNode : ExpandableNode
    {
        #region Constants

        public const string PROVIDERS_NODE_NAME = "Providers";
        public const string REQUIREDBY_NODE_NAME = "Required By";
        public const string DEPENDENTS_NODE_NAME = "Dependents";

        #endregion

        public ResourceNode(ExplorerNode parent, string name, Controls.Nodes.ControllerNode.ControllerNodeType nodeType)
            : base(parent, name, GetControllerNodeType(nodeType))
        {
        }

        /// <summary>
        /// required constructor for nodes displaying errors <see cref="M:ErrorsNode.ReportError"/>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="source"></param>
        public ResourceNode(ExplorerNode parent, ResourceNode source)
            : base(parent, source, RESOURCE_ICON, RESOURCE_ICON)
        {
            
        }

        private static string GetControllerNodeType(Controls.Nodes.ControllerNode.ControllerNodeType nodeType)
        {
            switch (nodeType)
            {
                case Controls.Nodes.ControllerNode.ControllerNodeType.Consumer: return RESOURCE_CONSUMER_ICON;
                default:
                case Controls.Nodes.ControllerNode.ControllerNodeType.Provider: return RESOURCE_PROVIDER_ICON;
            }
        }
    }
}
