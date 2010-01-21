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
using System.Xml;
using System.Reflection;

namespace Bistro.Designer.Projects.FSharp.Properties
{
    public partial class CompileOrderViewer : UserControl
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

        public CompileOrderViewer(ProjectManager project)
        {
            this.project = project;
            InitializeComponent();
            refresh_file_list();
            //project.OnProjectModified += new EventHandler(project_OnProjectModified);
        }

        void project_OnProjectModified(object sender, EventArgs e)
        {
            refresh_file_list();
        }

        void refresh_file_list()
        {
            Dependencies.Nodes.Clear();
            //foreach (BuildItemGroup group in project.BuildProject.ItemGroups)
            //{
            //    foreach (BuildItem item in group)
            //        if (item.Name == "Compile"
            //            && project.IsCodeFile(item.FinalItemSpec)
            //            && !item.IsImported)
            //        {
            //            Dependencies.Nodes.Add(item.FinalItemSpec)
            //                .Tag = new BuildElement(group, item);
            //        }
            //}           
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
                    if (n.Index >= Dependencies.Nodes.Count - 1)
                        return;
                    new_index = n.Index + 1;
                    break;
            }

            BuildElement fst = (BuildElement)n.Tag;
            BuildElement snd = (BuildElement)Dependencies.Nodes[new_index].Tag;

            Dependencies.Nodes.Remove(n);
            Dependencies.Nodes.Insert(new_index, n);
            Dependencies.SelectedNode = n;

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

    }
}
