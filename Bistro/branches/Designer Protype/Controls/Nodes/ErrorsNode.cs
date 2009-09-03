using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Designer.Explorer;

namespace Controls.Nodes
{
    class ErrorsNode : ExpandableNode
    {
        #region Constants
        public const string ERRORS_NODE_NAME = "Errors";
        #endregion

        public ErrorsNode(BistroNode parent)
            : base(parent, ERRORS_NODE_NAME, ERROR_ICON)
        {
        }

        public enum Errors
        {
            MalformedBinding,
            MissingProvider,
            ResourceLoop,
            TypeMismatch
        }

        private string getMessage(Errors error)
        {
            switch (error)
            {
                case Errors.MalformedBinding: return "Invalid format for binding URL";
                case Errors.MissingProvider: return "Resource has no providers";
                case Errors.ResourceLoop: return "Execution order for the controllers could not be determined because of a resource reference loop";
                case Errors.TypeMismatch: return "Resource type mismatch";
            }
            return "Unknown error";
        }

        protected override bool IsRemovable { get { return true; } }

        internal void ReportError(ExplorerNode node, Errors error)
        {
            ErrorNode parent = (ErrorNode)this[error.ToString()];
            if (parent == null)
                parent = new ErrorNode(this, error.ToString(), getMessage(error));
            Activator.CreateInstance(node.GetType(), (ExplorerNode)parent, node);
        }

        protected override System.Windows.Forms.ContextMenuStrip GetContextMenu(System.Windows.Forms.ContextMenuStrip contextMenu)
        {
            return null;
        }
    }
}
