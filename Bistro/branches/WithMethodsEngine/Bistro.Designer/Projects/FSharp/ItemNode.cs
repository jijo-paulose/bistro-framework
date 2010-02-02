using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.Build.BuildEngine;

namespace Bistro.Designer.Projects.FSharp
{
    /// <summary>
    /// Represents a node in the soltion explorer
    /// </summary>
    /// <remarks>
    /// Every ItemNode object shadows a hierarchy object as defined by the base project to facilitate the proper 
    /// order of the nodes in the solution explorer
    /// </remarks>
    internal class ItemNode
    {
        ItemList items;
        ItemNode parent;
        string node_key;
        string name;
        BuildItem buildItem;

        public BuildItem BuildItem { get { return buildItem; } }

        SortedList<string, ItemNode> children = new SortedList<string, ItemNode>();
        Dictionary<uint, int> childrenMap;

        public ItemNode(ItemList items, uint itemId)
        {
            this.items = items;
            ItemId = itemId;
            node_key = items.GetNodeKey(itemId);
            
            // we only care about physical files (node_key[0] == 'e') with the names ending in .fs
            if (node_key[0] == 'e' && node_key.EndsWith(".fs"))
            {
                name = items.GetInclude(itemId);
                buildItem = items.GetBuildItem(itemId);
            }

            // register the node. It has to be done before walking the child tree 
            // to preserve the registration order
            items.Register(this);
            
            // recursively walk the children list
            uint child = items.GetNodeFirstChild(itemId);
            while (child != VSConstants.VSITEMID_NIL)
            {
                ItemNode node = new ItemNode(items, child);
                node.parent = this;
                children.Add(node.node_key, node);
                child = items.GetNodeSibling(child);
            }

            // build the children map
            childrenMap = new Dictionary<uint, int>(children.Count);
            int i = 0;
            foreach (var item in children)
                childrenMap.Add(item.Value.ItemId, i++);
        }

        /// <summary>
        /// returns the item Id of the corresponding Hierarchy object
        /// </summary>
        public uint ItemId { get; private set; }

        /// <summary>
        /// returns the name of the itemNode. 
        /// </summary>
        /// <remarks>
        /// The name is only valid for the fsharp source files. It is the value of the Include attribute on the 
        /// appropriate MSBuild tag
        /// </remarks>
        public string Name { get { return name; } }

        public bool IsFSharpSource 
        { 
            get 
            {
                return node_key[0] == 'e' && node_key.EndsWith(".fs") && buildItem.Name == "Compile"; 
            } 
        }

        /// <summary>
        /// returns the item Id of the next sibling Hierarchy object or VSConstants.VSITEMID_NIL if there is none
        /// </summary>
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

        /// <summary>
        /// returns the item Id of the first child Hierarchy object or VSConstants.VSITEMID_NIL if there is none
        /// </summary>
        public uint FirstChild
        {
            get
            {
                if (children.Count == 0)
                    return VSConstants.VSITEMID_NIL;
                return children.Values[0].ItemId;
            }
        }

        /// <summary>
        /// Deletes the node from all lists and dictionaries
        /// </summary>
        internal void Delete()
        {
            parent.Unregiater(ItemId);
            items.Unregister(this);
            if (Deleting != null)
                Deleting(this, EventArgs.Empty);
        }

        /// <summary>
        /// Deletes the node from all lists and dictionaries
        /// </summary>
        private void Unregiater(uint itemId)
        {
            int i = childrenMap[itemId];
            children.RemoveAt(i);
            childrenMap.Remove(itemId);
            for (; i < children.Count; i++)
                childrenMap[children.Values[i].ItemId] = i;
        }

        public event EventHandler Deleting;

        /// <summary>
        /// Creates a new ItemNode as a child node in reponse to a new hierarchy child object created
        /// </summary>
        /// <param name="itemidAdded">item id of the new hierarchy object</param>
        internal void AddChild(uint itemidAdded)
        {
            ItemNode node = new ItemNode(items, itemidAdded);
            node.parent = this;
            children.Add(node.node_key, node);
            childrenMap.Clear();
            int i = 0;
            foreach (var item in children)
                childrenMap.Add(item.Value.ItemId, i++);
        }

        internal BuildItemGroup GetBuildGroup()
        {
            foreach (BuildItemGroup grp in items.GetBuildGroups())
                foreach (BuildItem item in grp)
                    if (item.Include == buildItem.Include)
                        return grp;

            throw new Exception("Cannot find the BuildItemGroup for a BuildItem");
        }

        public List<ItemNode> Dependencies 
        { 
            get { return dependencies; }
            set 
            { 
                dependencies = value;
                items.SetDependencies(ItemId, dependencies);
            }
        }
        List<ItemNode> dependencies = new List<ItemNode>();
        internal void BuildDependencies()
        {
            string dependenciesString = items.GetDependencies(ItemId);
            if (dependenciesString != null)
                foreach (string item in dependenciesString.Split(','))
                {
                    if (item == "")
                        continue;
                    dependencies.Add(items.LocateItem(item));
                }
        }

        /// <summary>
        /// returns the name of the node - used in the edit dependencies dialog
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
