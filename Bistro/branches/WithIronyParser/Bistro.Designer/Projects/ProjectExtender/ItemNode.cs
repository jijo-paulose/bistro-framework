using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio;

namespace FSharp.ProjectExtender
{
    public class ItemNode
    {
        ItemList items;
        ItemNode parent;

        SortedList<string, ItemNode> children = new SortedList<string, ItemNode>();
        Dictionary<uint, int> childrenMap;

        public ItemNode(ItemList items, uint itemId)
        {
            this.items = items;
            ItemId = itemId;
            uint child = items.GetNodeFirstChild(itemId);
            while (child != VSConstants.VSITEMID_NIL)
            {
                ItemNode node = new ItemNode(items, child);
                node.parent = this;
                children.Add(items.GetNodeKey(child), node);
                child = items.GetNodeSibling(child);
            }
            childrenMap = new Dictionary<uint, int>(children.Count);
            int i = 0;
            foreach (var item in children)
                childrenMap.Add(item.Value.ItemId, i++);
            items.Register(this);
        }

        public uint ItemId { get; private set; }

        public uint NextSibling
        {
            get
            {
                if (parent == null)
                    return VSConstants.VSITEMID_NIL;
                int index = parent.childrenMap[ItemId];
                if (index + 1 < parent.children.Count)
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
            items.Unregister(ItemId);
        }

        internal void AddChild(uint itemidAdded)
        {
            ItemNode node = new ItemNode(items, itemidAdded);
            node.parent = this;
            children.Add(items.GetNodeKey(itemidAdded), node);
            childrenMap.Clear();
            int i = 0;
            foreach (var item in children)
                childrenMap.Add(item.Value.ItemId, i++);
        }
    }
}
