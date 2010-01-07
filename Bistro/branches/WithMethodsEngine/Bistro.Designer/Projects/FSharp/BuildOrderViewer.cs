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

        public BuildOrderViewer(ProjectManager project)
        {
            this.project = project;
            InitializeComponent();
            foreach (BuildItemGroup group in project.BuildProject.ItemGroups)
            {
                foreach (BuildItem item in group)
                    if (item.Name == "Compile"
                        && project.IsCodeFile(item.FinalItemSpec)
                        && !item.IsImported)
                    {
                        Dependencies.Nodes.Add(item.FinalItemSpec)
                            .Tag = new BuildElement(group, item);
                    }
            }           
        }

        private void MoveUp_Click(object sender, EventArgs e)
        {
            if (Dependencies.SelectedNode != null)
                Swap(Dependencies.SelectedNode, Direction.Up);
        }


        private void MoveDown_Click(object sender, EventArgs e)
        {
            if (Dependencies.SelectedNode != null)
                Swap(Dependencies.SelectedNode, Direction.Down);
        }

        enum Direction { Up, Down }

        private void Swap(TreeNode n, Direction dir)
        {
            if (!Dependencies.Nodes.Contains(n))
                return;
            int new_index = 0;
            switch (dir)
            {
                case Direction.Up:
                    if (n.Index <= 0)
                        return;
                    new_index = n.Index - 1;
                    break;
                case Direction.Down:
                    if (n.Index >= Dependencies.Nodes.Count)
                        return;
                    new_index = n.Index + 1;
                    break;
            }

            BuildElement fst = (BuildElement)n.Tag;
            BuildElement snd = (BuildElement)Dependencies.Nodes[new_index].Tag;

            Dependencies.Nodes.Remove(n);
            Dependencies.Nodes.Insert(new_index, n);
            Dependencies.SelectedNode = n;

            //int fst_loc = Locate(fst);
            //int snd_loc = Locate(snd);
            //fst.BuildItemGroup[fst_loc] = snd.BuildItem;
            //new BuildItemGroup(

        }

        int Locate(BuildElement elem)
        {
            int result = -1;
            foreach (BuildItem b in elem.BuildItemGroup)
                if (elem.BuildItemGroup[++result] == elem.BuildItem)
                    return result;
            return result;
        }
    }
}
