using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Build.BuildEngine;
using System.Xml;
using System.Reflection;
using System.IO;

namespace Bistro.Designer.Projects.FSharp.Properties
{
    public partial class CompileOrderViewer : UserControl
    {
        IProjectManager project;
        class BuildElement
        {
            public BuildElement(BuildItemGroup BuildItemGroup, BuildItem BuildItem)
            {
                this.BuildItem = BuildItem;
                this.BuildItemGroup = BuildItemGroup;
            }
            public BuildItemGroup BuildItemGroup { get; private set; }
            public BuildItem BuildItem { get; private set; }

            public override string ToString()
            {
                return BuildItem.Include;
            }

            internal string GetDependencies()
            {
                return BuildItem.GetMetadata(FSharpPropertiesConstants.DependsOn);
            }

            internal void UpdateDependencies(List<BuildElement> dependencies)
            {
                if (dependencies.Count == 0)
                    BuildItem.RemoveMetadata(FSharpPropertiesConstants.DependsOn);
                else
                    BuildItem.SetMetadata(FSharpPropertiesConstants.DependsOn, dependencies.ConvertAll(elem => elem.ToString()).Aggregate("", (a, item) => a + ',' + item).Substring(1));
            }
        }

        public CompileOrderViewer(IProjectManager project)
        {
            this.project = project;
            InitializeComponent();
            refresh_file_list();
            var service = (ProjectManager)GetService(typeof(ProjectManager));
        }

        public event EventHandler OnPageUpdated;

        void project_OnProjectModified(object sender, EventArgs e)
        {
            refresh_file_list();
        }

        public void refresh_file_list()
        {
            CompileItems.Nodes.Clear();
            foreach (BuildItemGroup group in project.MSBuildProject.ItemGroups)
            {
                foreach (BuildItem item in group)
                    if (item.Name == "Compile" && Path.GetExtension(item.Include) == ".fs")
                    {
                        TreeNode compileItem = new TreeNode(item.Include);
                        compileItem.Tag = new BuildElement(group, item);
                        compileItem.ContextMenuStrip = compileItemMenu;
                        BuildDependencies(compileItem);
                        CompileItems.Nodes.Add(compileItem);
                    }
            }           
        }

        private void BuildDependencies(TreeNode node)
        {
            node.Nodes.Clear();
            string dependencies = ((BuildElement)node.Tag).GetDependencies();
            if (dependencies != null)
                foreach (var d in dependencies.Split(','))
                    if (d != "")
                        node.Nodes.Add(d);
        }

        private void CompileItems_AfterSelect(object sender, TreeViewEventArgs e)
        {
            MoveUp.Enabled = false;
            MoveDown.Enabled = false;
            if (e.Node.Level == 0 && CompileItems.SelectedNode != null)
            {
                MoveUp.Enabled = CompileItems.Nodes.IndexOf(e.Node) > 0;
                MoveDown.Enabled = CompileItems.Nodes.IndexOf(e.Node) < CompileItems.Nodes.Count - 1;
            }
        }

        private void MoveUp_Click(object sender, EventArgs e)
        {
            if (CompileItems.SelectedNode != null)
                Swap(CompileItems.SelectedNode, Direction.Up);
        }


        private void MoveDown_Click(object sender, EventArgs e)
        {
            if (CompileItems.SelectedNode != null)
                Swap(CompileItems.SelectedNode, Direction.Down);
        }

        enum Direction { Up, Down }

        private void Swap(TreeNode n, Direction dir)
        {
            if (!CompileItems.Nodes.Contains(n))
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
                    if (n.Index >= CompileItems.Nodes.Count - 1)
                        return;
                    new_index = n.Index + 1;
                    break;
            }
            if (OnPageUpdated != null)
                OnPageUpdated(this, EventArgs.Empty);

            BuildElement fst = (BuildElement)n.Tag;
            BuildElement snd = (BuildElement)CompileItems.Nodes[new_index].Tag;

            CompileItems.Nodes.Remove(n);
            CompileItems.Nodes.Insert(new_index, n);
            CompileItems.SelectedNode = n;

            int fst_loc = Locate(fst);
            int snd_loc = Locate(snd);
            fst.BuildItemGroup.RemoveItem(fst.BuildItem);
            AddItemAt(snd.BuildItemGroup, fst.BuildItem, snd_loc);
            snd.BuildItemGroup.RemoveItem(snd.BuildItem);
            AddItemAt(fst.BuildItemGroup, snd.BuildItem, fst_loc);
        }

        int Locate(BuildElement elem)
        {
            int result = -1;
            foreach (BuildItem b in elem.BuildItemGroup)
                if (elem.BuildItemGroup[++result] == elem.BuildItem)
                    return result;
            return result;
        }

        // The code below is ripped off from the FSharp project system

        /// <summary>
        /// Adds an existing BuildItem to a BuildItemGroup at given location
        /// </summary>
        /// <param name="big"></param>
        /// <param name="itemToAdd"></param>
        /// <param name="index"></param>
        internal static void AddItemAt(BuildItemGroup big, BuildItem itemToAdd, int index)
        {
            XmlNode node;
            XmlElement element = (XmlElement)typeof(BuildItemGroup)
                .InvokeMember("get_ItemGroupElement", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, big, new object [] { } );
            if (big.Count > 0)
            {
                XmlElement element2;
                if (index == big.Count)
                {
                    element2 = ItemElement(big[big.Count - 1]);
                    node = ((XmlElement)element2.ParentNode).InsertAfter(ItemElement(itemToAdd), element2);
                }
                else
                {
                    element2 = ItemElement(big[index]);
                    node = ((XmlElement)element2.ParentNode).InsertBefore(ItemElement(itemToAdd), element2);
                }
            }
            else
            {
                node = element.AppendChild(ItemElement(itemToAdd));
            }
            object[] args = new object[] { index, itemToAdd };
            object obj2 = typeof(BuildItemGroup)
                .InvokeMember("AddExistingItemAt", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, big, args);
        }

        internal static XmlElement ItemElement(BuildItem bi)
        {
            return (XmlElement)typeof(BuildItem).
                InvokeMember("get_ItemElement", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, bi, new object[] {});
        }


        private void compileItemMenu_Click(object sender, EventArgs e)
        {
            EditDependenciesDialog addForm = new EditDependenciesDialog();
            var origin = CompileItems.HitTest(((MouseEventArgs)e).Location);
            if (origin.Node == null)
                return;
            foreach (TreeNode n in CompileItems.Nodes)
            {
                if (origin.Node != n)
                    addForm.Dependencies.Items.Add(n.Tag);
                if (((BuildElement)origin.Node.Tag).GetDependencies().IndexOf(n.Tag.ToString()) >= 0)
                    addForm.Dependencies.SetItemChecked(addForm.Dependencies.Items.Count - 1, true);
            }
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                List<BuildElement> dependencies = new List<BuildElement>();
                foreach (BuildElement item in addForm.Dependencies.CheckedItems)
                    dependencies.Add(item);

                ((BuildElement)origin.Node.Tag).UpdateDependencies(dependencies);
                BuildDependencies(origin.Node);
            }
            addForm.Dispose();
        }
    }
}
