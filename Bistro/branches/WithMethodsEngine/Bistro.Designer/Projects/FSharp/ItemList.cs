using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.Build.BuildEngine;
using Microsoft.VisualStudio.Shell.Interop;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Bistro.Designer.Projects.FSharp
{
    /// <summary>
    /// Maintains a list of project items and provides access to them to other classes
    /// </summary>
    public class ItemList: IVsHierarchyEvents
    {
        ProjectManager project;
        Project buildProject;
        // maps itemId's to instances of the ItemNode class
        Dictionary<uint, ItemNode> itemMap = new Dictionary<uint, ItemNode>();

        List<ItemNode> items = new List<ItemNode>();

        /// <summary>
        /// Lists ItemNodes in the order of the appropriate elements in the build file. 
        /// Only FSharp source files are included
        /// </summary>
        internal List<ItemNode> Items { get { return items; } }

        /// <summary>
        /// Creates an instance of the ItemList and populates it from the 
        /// current state of the project hierarchy
        /// </summary>
        /// <param name="project"></param>
        /// <param name="buildProject"></param>
        /// <remarks>
        /// Creates the root node, which in turn recursively descends through the hierarchy tree
        /// every node, including the root, when created registers itself in itemList dictionaries through a call to 
        /// the <see cref="M:Register"/> method
        /// </remarks>
        public  ItemList(ProjectManager project, Project buildProject)
        {
            this.project = project;
            this.buildProject = buildProject;

            new ItemNode(this, VSConstants.VSITEMID_ROOT);

            foreach (ItemNode item in itemMap.Values)
            {
                if (item.IsFSharpSource)
                    item.BuildDependencies();
            }
        }

        /// <summary>
        /// Gets the itemId for the next sibling of the hierarchy with a given itemId
        /// </summary>
        /// <param name="itemId">Hierarchy item Id</param>
        /// <param name="value">the item id for the next sibling (boxed as an object</param>
        /// <returns>S_OK if successful, otherwise the error code</returns>
        /// <remarks>inteded for use in implementation of the IVsHierarchy.GetProperty method for __VSHPROPID.VSHPROPID_NextSibling</remarks>
        public int GetNextSibling(uint itemId, out object value)
        {
            ItemNode n;
            value = VSConstants.VSITEMID_NIL;
            if (itemMap.TryGetValue(itemId, out n))
            {
                value = n.NextSibling;
                return VSConstants.S_OK;
            }
            else
                return VSConstants.E_INVALIDARG;
        }

        /// <summary>
        /// Gets the itemId for the first child of the hierarchy with a given itemId
        /// </summary>
        /// <param name="itemId">Hierarchy item Id</param>
        /// <param name="value">the item id for the first child (boxed as an object</param>
        /// <returns>S_OK if successful, otherwise the error code</returns>
        /// <remarks>inteded for use in implementation of the IVsHierarchy.GetProperty method for __VSHPROPID.VSHPROPID_FirstChild</remarks>
        public int GetFirstChild(uint itemId, out object value)
        {
            ItemNode n;
            value = null;
            if (itemMap.TryGetValue(itemId, out n))
            {
                value = n.FirstChild;
                return VSConstants.S_OK;
            }
            else
                return VSConstants.E_INVALIDARG;
        }

        /// <summary>
        /// Gets the itemId of the first child Hierarchy as defined by the base project
        /// </summary>
        /// <param name="itemId">Hierarchy item Id</param>
        /// <returns>Item id of the first child</returns>
        /// <remarks> intended for use in the ItemNode building process to walk the base</remarks>
        internal uint GetNodeFirstChild(uint itemId)
        {
            return project.GetNodeChild(itemId);
        }

        /// <summary>
        /// Gets the key which detrmines the sort order of the hierarchy node
        /// </summary>
        /// <param name="itemId">Hierarchy item Id</param>
        /// <returns></returns>
        internal string GetNodeKey(uint itemId)
        {
            IVsHierarchy hier = (IVsHierarchy)project;
            object name;
            ErrorHandler.ThrowOnFailure(hier.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_Caption, out name));
            Guid type;
            try
            {
                ErrorHandler.ThrowOnFailure(hier.GetGuidProperty(itemId, (int)__VSHPROPID.VSHPROPID_TypeGuid, out type));
            }
            catch (COMException e)
            {
                // FSharp project returns Guid.Empty as the type guid for reference nodes, which causes the WAP to throw an exception
                var pinfo = e.GetType().GetProperty("HResult", BindingFlags.Instance | BindingFlags.NonPublic);
                if ((int)pinfo.GetValue(e, new object[] { }) == VSConstants.DISP_E_MEMBERNOTFOUND)
                    type = Guid.Empty;
                else
                    throw;
            }

            // set the sort order based on the item type
            string sort_order = "";
            if (type == Guid.Empty)
                sort_order = "a";   // Reference nodes go first. This does not really matter, because reference nodes
                                    // only happen as children of the References node container. They do not mix with other node types
            else if (type == VSConstants.GUID_ItemType_SubProject)
                sort_order = "b";   // SubProject (nested projects) go next
            else if (type == VSConstants.GUID_ItemType_VirtualFolder)
                sort_order = "c";   // Virtual folder nodes - i.e references node, properties node, etc
            else if (type == VSConstants.GUID_ItemType_PhysicalFolder)
                sort_order = "d";   // Physical folder
            else if (type == VSConstants.GUID_ItemType_PhysicalFile)
                sort_order = "e";   // Physical file

            return sort_order + ';' + (string)name;
        }

        /// <summary>
        /// Gets the itemId of the next sibling Hierarchy as defined by the base project
        /// </summary>
        /// <param name="itemId">Hierarchy item Id</param>
        /// <returns>Item id of the next sibling</returns>
        /// <remarks> intended for use in the ItemNode building process to walk the base</remarks>
        internal uint GetNodeSibling(uint itemId)
        {
            return project.GetNodeSibling(itemId);
        }

        internal void Register(ItemNode itemNode)
        {
            itemMap.Add(itemNode.ItemId, itemNode);
            if (itemNode.IsFSharpSource)
                items.Add(itemNode);
        }

        internal void Unregister(ItemNode itemNode)
        {
            itemMap.Remove(itemNode.ItemId);
            if (itemNode.IsFSharpSource)
                items.Remove(itemNode);
        }

        #region IVsHierarchyEvents Members

        int IVsHierarchyEvents.OnInvalidateIcon(IntPtr hicon)
        {
            return VSConstants.S_OK;
        }

        int IVsHierarchyEvents.OnInvalidateItems(uint itemidParent)
        {
            return VSConstants.S_OK;
        }

        int IVsHierarchyEvents.OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded)
        {
            // for some reason during rename OnItemAdded is called twice - let us ignore the second one
            if (itemMap.ContainsKey(itemidAdded))
                return VSConstants.S_OK;

            ItemNode n;
            if (!itemMap.TryGetValue(itemidParent, out n))
                return VSConstants.E_INVALIDARG;
            n.AddChild(itemidAdded);
            if (ItemAdded != null)
                ItemAdded(this, EventArgs.Empty);
            return VSConstants.S_OK;
        }

        int IVsHierarchyEvents.OnItemDeleted(uint itemid)
        {
            ItemNode n;
            if (itemMap.TryGetValue(itemid, out n))
                n.Delete();
            return VSConstants.S_OK;
        }

        int IVsHierarchyEvents.OnItemsAppended(uint itemidParent)
        {
            return VSConstants.S_OK;
        }

        int IVsHierarchyEvents.OnPropertyChanged(uint itemid, int propid, uint flags)
        {
            return VSConstants.S_OK;
        }

        #endregion

        internal string GetDependencies(uint itemId)
        {
            return project.GetMetadata(itemId, Properties.FSharpPropertiesConstants.DependsOn);
        }

        internal void SetDependencies(uint itemId, List<ItemNode> dependencies)
        {
            string dependenciesString = null;
            if (dependencies.Count > 0)
                dependenciesString = 
                    dependencies.ConvertAll(item => GetCanonicalName(item.ItemId))
                    .Aggregate("", (result, item) => result + ',' + item)
                    .Substring(1);
            project.SetMetadata(itemId, Properties.FSharpPropertiesConstants.DependsOn, dependenciesString);
        }

        internal string GetCanonicalName(uint itemId)
        {
            string name;
            ErrorHandler.ThrowOnFailure(((IVsHierarchy)project).GetCanonicalName(itemId, out name));
            return name;
        }

        internal ItemNode LocateItem(string item)
        {
            uint itemId;
            ErrorHandler.ThrowOnFailure(((IVsHierarchy)project).ParseCanonicalName(item, out itemId));
            return itemMap[itemId];
        }

        internal string GetInclude(uint itemId)
        {
            return project.GetMetadata(itemId, "Include");
        }

        internal BuildItem GetBuildItem(uint itemId)
        {
            return project.GetBuildItem(itemId);
        }

        public event EventHandler ItemAdded;

        internal BuildItemGroupCollection GetBuildGroups()
        {
            return project.MSBuildProject.ItemGroups;
        }
    }
}
