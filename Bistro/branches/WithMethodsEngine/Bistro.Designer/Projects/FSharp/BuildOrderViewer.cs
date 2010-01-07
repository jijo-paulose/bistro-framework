using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bistro.Designer.ProjectBase;
using Microsoft.Build.BuildEngine;

namespace Bistro.Designer.Projects.FSharp
{
    public partial class BuildOrderViewer : UserControl
    {
        ProjectManager project;
        class BuildElement
        {
            public BuildElement(BuildItemGroup BuildItemGroup, BuildItem BuildItem)
            {
                this.BuildItem = BuildItem;
                this.BuildItemGroup = BuildItemGroup;
            }
            public BuildItemGroup BuildItemGroup { get; private set; }
            public BuildItem BuildItem { get; private set; }
        }
        List<BuildElement> buildItems = new List<BuildElement>();

        public BuildOrderViewer(ProjectManager project)
        {
            this.project = project;
            InitializeComponent();

            int i = 0;
            foreach (BuildItemGroup group in project.BuildProject.ItemGroups)
            {
                foreach (BuildItem item in group)
                    if (item.Name == "Compile"
                        && project.IsCodeFile(item.FinalItemSpec)
                        && !item.IsImported)
                    {
                        buildItems.Add(new BuildElement(group, item));
                        Dependencies.Nodes.Add(item.FinalItemSpec);
                    }
            }           
        }

        private void MoveUp_Click(object sender, EventArgs e)
        {
            if (Dependencies.SelectedNode == null)
                return;
            if (Dependencies.SelectedNode.Index <= 0)
                return;
            TreeNode node = Dependencies.SelectedNode;
            TreeNodeCollection collection = Dependencies.Nodes;
            int index = node.Index;
            collection.Remove(node);
            collection.Insert(index - 1, node);
            Dependencies.SelectedNode = node;
        }

        private void MoveDown_Click(object sender, EventArgs e)
        {
            if (Dependencies.SelectedNode == null)
                return;
            if (Dependencies.SelectedNode.Index > Dependencies.Nodes.Count-1)
                return;
            TreeNode node = Dependencies.SelectedNode;
            TreeNodeCollection collection = Dependencies.Nodes;
            int index = node.Index;
            collection.Remove(node);
            collection.Insert(index + 1, node);
            Dependencies.SelectedNode = node;
        }
    }
}
