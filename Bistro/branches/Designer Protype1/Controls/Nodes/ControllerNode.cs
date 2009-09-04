using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Designer.Explorer;
using Bistro.Methods;

namespace Controls.Nodes
{
    class ControllerNode : ExplorerNode
    {
        /// <summary>
        /// Creates a controller node for a resource dependency as a child of the resource node
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="type"></param>
        public ControllerNode(ExplorerNode parent, ControllerType type, ControllerNodeType nodeType)
            : base(parent, type.Name, ACTIVITY_ICON, ACTIVITY_ICON)
        {
            //this.type = type;
            //type.Register(this);
            //this.nodeType = nodeType;
        }

        private static string GetControllerNodeType(ControllerNodeType nodeType)
        {
            switch (nodeType)
            {
                case ControllerNodeType.Consumer: return RESOURCE_CONSUMER_ICON;
                default:
                case ControllerNodeType.Provider: return RESOURCE_PROVIDER_ICON;
            }
        }

        public enum ControllerNodeType
        {
            None = 0,
            Provider = 1,
            Consumer = 2
        }
    }
}
