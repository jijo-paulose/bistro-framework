using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio;

namespace Bistro.Designer.Projects.FSharp
{
    public class ProjectTreeNode
    {
        SortedList<string, ProjectTreeNode> children = new SortedList<string, ProjectTreeNode>();
        Dictionary<uint, int> childrenMap;
        ProjectTreeNode parent;
        ProjectManager project;

        public uint ItemId { get; private set; }
        public ProjectTreeNode(ProjectManager project, uint itemId)
        {
            this.project = project;
            ItemId = itemId;
            uint child = project.GetNodeChild(itemId);
            while (child != VSConstants.VSITEMID_NIL)
            {
                ProjectTreeNode node = new ProjectTreeNode(project, child);
                node.parent = this;
                children.Add(project.GetNodeName(child), node);
                child = project.GetNodeSibling(child);
            }
            childrenMap = new Dictionary<uint, int>(children.Count);
            int i = 0;
            foreach (var item in children)
                childrenMap.Add(item.Value.ItemId, i++);
            project.MapProjectNode(itemId, this);
        }

        public uint NextSibling
        {
            get
            {
                if (parent == null)
                    return VSConstants.VSITEMID_NIL;
                int index = parent.childrenMap[ItemId];
                if (index+1 < parent.children.Count)
                    return parent.children.Values[index + 1].ItemId;
                return VSConstants.VSITEMID_NIL;
            }
        }

        public uint FirstChild
        {
            get
            {
                if (children.Count == 0)
                    return VSConstants.VSITEMID_NIL;
                return children.Values[0].ItemId;
            }
        }

        internal void Delete()
        {
            parent.children.RemoveAt(parent.childrenMap[ItemId]);
            parent.childrenMap.Remove(ItemId);
            project.UnmapProjectNode(ItemId);
        }

        internal void AddChild(uint itemidAdded)
        {
            ProjectTreeNode node = new ProjectTreeNode(project, itemidAdded);
            node.parent = this;
            children.Add(project.GetNodeName(itemidAdded), node);
            childrenMap.Clear();
            int i = 0;
            foreach (var item in children)
                childrenMap.Add(item.Value.ItemId, i++);
        }
    }
}
