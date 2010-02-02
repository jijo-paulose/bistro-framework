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

        public CompileOrderViewer(IProjectManager project)
        {
            this.project = project;
            InitializeComponent();
            CompileItems.BeginUpdate();
            remap_file_nodes();
            CompileItems.EndUpdate();
            project.ItemList.ItemAdded += new EventHandler(ItemList_ItemAdded);
        }

        void ItemList_ItemAdded(object sender, EventArgs e)
        {
            remap_file_nodes();
        }

        private void remap_file_nodes()
        {
            int i = 0;
            foreach (ItemNode item in project.ItemList.Items)
                if (i >= CompileItems.Nodes.Count)
                    insert_item(item, i++);
                else if (CompileItems.Nodes[i++].Tag != item)
                    insert_item(item, i - 1);
        }

        private void insert_item(ItemNode item, int index)
        {
            TreeNode compileItem = new TreeNode();
            compileItem.Name = item.ItemId.ToString();
            compileItem.Tag = item;
            compileItem.Text = item.Name;
            item.Deleting += (sender, args) => CompileItems.Nodes.Remove(compileItem);
            compileItem.ContextMenuStrip = compileItemMenu;
            build_dependencies(compileItem);
            CompileItems.Nodes.Insert(index, compileItem);
        }

        private void build_dependencies(TreeNode node)
        {
            node.Nodes.Clear();
            foreach (ItemNode dependency in ((ItemNode)node.Tag).Dependencies)
                node.Nodes.Add(dependency.Name);
        }

        public event EventHandler OnPageUpdated;

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

            ItemNode fst = (ItemNode)n.Tag;
            ItemNode snd = (ItemNode)CompileItems.Nodes[new_index].Tag;

            CompileItems.Nodes.Remove(n);
            CompileItems.Nodes.Insert(new_index, n);
            CompileItems.SelectedNode = n;

            BuildItemGroup fst_grp = fst.GetBuildGroup();
            BuildItemGroup snd_grp = snd.GetBuildGroup();

            int fst_loc = Locate(fst_grp, fst);
            int snd_loc = Locate(snd_grp, snd);
            fst_grp.RemoveItemAt(fst_loc);
            AddItemAt(snd_grp, fst.BuildItem, snd_loc);
        }

        private int Locate(BuildItemGroup group, ItemNode node)
        {
            int result = -1;
            foreach (BuildItem b in group)
                if (group[++result].Include == node.BuildItem.Include)
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
            Point p = ((Control)sender).PointToScreen(new Point(0,0));
            var origin = CompileItems.HitTest(CompileItems.PointToClient(p));
            if (origin.Node == null || origin.Node.Tag == null)
                return;
            foreach (TreeNode n in CompileItems.Nodes)
            {
                if (origin.Node != n)
                    addForm.Dependencies.Items.Add(n.Tag);
                if (((ItemNode)origin.Node.Tag).Dependencies.Contains((ItemNode)n.Tag))
                    addForm.Dependencies.SetItemChecked(addForm.Dependencies.Items.Count - 1, true);
            }
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                List<ItemNode> dependencies = new List<ItemNode>();
                foreach (ItemNode item in addForm.Dependencies.CheckedItems)
                    dependencies.Add(item);

                ((ItemNode)origin.Node.Tag).Dependencies = dependencies;
                build_dependencies(origin.Node);
            }
            addForm.Dispose();
        }
    }
}
