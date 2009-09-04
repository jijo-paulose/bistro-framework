using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Methods;

namespace Controls.Nodes
{
    public class MethodsNode : BindingNode
    {
        #region Constants
        const string CONTROLLERS_NODE_NAME = "Bindings";
        #endregion

        #region Construction

        public MethodsNode(BistroNode root)
            : base(root, CONTROLLERS_NODE_NAME, BINDERS_ICON, OPEN_FOLDER_ICON)
        {
        }

        #endregion

        #region Properties

        public override string Caption
        {
            get
            {
                return CONTROLLERS_NODE_NAME;
            }
        }

        #endregion

        internal void CreateControllerNodes(ControllerType type, string methodUrl)
        {
            //string verb;
            //switch (methodUrl[0])
            //{
            //    case '?':
            //    case '/':
            //        verb = "*";
            //        break;
            //    default:
            //        verb = methodUrl.Substring(0, methodUrl.IndexOfAny(new char[] { ' ', '/', '?' }));
            //        methodUrl = methodUrl.Substring(methodUrl.IndexOfAny(new char[] { '/', '?' }));
            //        break;

            //}
            //foreach (BindingNode parent in PlaceController(verb, methodUrl))
            //    new ControllerNode(parent, type, methodUrl);
        }

        protected override bool IsRemovable { get { return false; } }
    }
}
