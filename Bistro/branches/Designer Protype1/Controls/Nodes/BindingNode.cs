using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Designer.Explorer;
using Bistro.Methods;
using System.Windows.Forms;

namespace Controls.Nodes
{
    public class BindingNode : ExpandableNode
    {
        private string methodUrl;
        private string verb;
        private string fullMethodUrl;

        protected BindingNode(ExplorerNode parent, string name, string imageKey, string expandedImageKey)
            : base(parent, name, imageKey, expandedImageKey)
        {
            verb = "*";
        }

        private static string buildBindingName(ExplorerNode parent, string verb, string methodUrl)
        {
            if (parent is MethodsNode)
                if (verb == "*")
                    return "[ANY] " + methodUrl;
                else
                    return "[" + verb + "] " + methodUrl;
            else
                return methodUrl;

        }

        public BindingNode(ExplorerNode parent, string verb, string methodUrl)
            : base(parent, buildBindingName(parent, verb, methodUrl), BINDERS_ICON, OPEN_FOLDER_ICON)
        {
            this.methodUrl = methodUrl;
            fullMethodUrl = methodUrl;
            if (parent is BindingNode && !(parent is MethodsNode))
                fullMethodUrl = ((BindingNode)parent).fullMethodUrl + fullMethodUrl;
            this.verb = verb;
                       
        }

        protected override ContextMenuStrip GetContextMenu(ContextMenuStrip contextMenu)
        {
            if (null == contextMenu)
                return null;
            contextMenu.Items.Add(new ToolStripMenuItem("Bindings", null, new EventHandler(ShowProperties), "Bindings"));
            contextMenu.Items.Add(new ToolStripMenuItem("Show Source", null, new EventHandler(ShowSource), "ShowSource"));

            return contextMenu;
        }
        protected override void ShowProperties(object sender, EventArgs args)
        {
            MessageBox.Show("Binding Windows");
            //Explorer.SetSelected(this);
            //application.ShowPropertiesWindow();
        }
    }
}
