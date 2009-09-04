using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Designer.Explorer;

namespace Controls.Nodes
{
    class ErrorNode : ExpandableNode
    {
        public ErrorNode(ErrorsNode parent, string name, string message)
            : base(parent, name, ERROR_ICON)
        {
            this.message = message;
        }

        string message;

        public override string Caption { get { return message; } }

        protected override bool IsRemovable { get { return true; } }

        protected override System.Windows.Forms.ContextMenuStrip GetContextMenu(System.Windows.Forms.ContextMenuStrip contextMenu)
        {
            return null;
        }
    }
}
