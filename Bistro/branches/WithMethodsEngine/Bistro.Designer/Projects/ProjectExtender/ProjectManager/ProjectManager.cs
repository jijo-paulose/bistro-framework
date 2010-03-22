using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio;

using IOLEServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using System.ComponentModel.Design;

namespace FSharp.ProjectExtender
{
    [ComVisible(true)]
    public class ProjectManager : FlavoredProjectBase, IProjectManager, IOleCommandTarget
    {

        public ProjectManager()
            : base()
        { }

        /// <summary>
        /// Sets the service provider from which to access the services. 
        /// </summary>
        /// <param name="site">An instance to an Microsoft.VisualStudio.OLE.Interop object</param>
        /// <returns>A success or failure value.</returns>
        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider site)
        {
            serviceProvider = new ServiceProvider(site);
            return VSConstants.S_OK;
        }

        uint hierarchy_event_cookie = (uint)ShellConstants.VSCOOKIE_NIL;
        private string fileName;
        private ItemList itemList;

        protected override void InitializeForOuter(string fileName, string location, string name, uint flags, ref Guid guidProject, out bool cancel)
        {
            this.fileName = fileName;
            base.InitializeForOuter(fileName, location, name, flags, ref guidProject, out cancel);
        }

        protected override void OnAggregationComplete()
        {
            base.OnAggregationComplete();
            BuildManager = new MSBuildManager(fileName);
            itemList = new ItemList(this);
            hierarchy_event_cookie = AdviseHierarchyEvents(itemList);
        }
        
        protected override int GetProperty(uint itemId, int propId, out object property)
        {
            switch ((__VSHPROPID)propId)
            {
                case __VSHPROPID.VSHPROPID_FirstChild:
                case __VSHPROPID.VSHPROPID_FirstVisibleChild:
                    return itemList.GetFirstChild(itemId, out property);
                case __VSHPROPID.VSHPROPID_NextSibling:
                case __VSHPROPID.VSHPROPID_NextVisibleSibling:
                    return itemList.GetNextSibling(itemId, out property);
                default:
                    break;
            }

            int result = base.GetProperty(itemId, propId, out property);
            if (result != VSConstants.S_OK)
                return result;

            if (itemId == VSConstants.VSITEMID_ROOT)
            {
                switch ((__VSHPROPID2)propId)
                {
                    case __VSHPROPID2.VSHPROPID_PropertyPagesCLSIDList:
                        {
                            //Add the CompileOrder property page.
                            var properties = new List<string>(property.ToString().Split(';'));
                            properties.Add(typeof(Page).GUID.ToString("B"));
                            property = properties.Aggregate("", (a, next) => a + ';' + next).Substring(1);
                            return VSConstants.S_OK;
                        }
                    case __VSHPROPID2.VSHPROPID_PriorityPropertyPagesCLSIDList:
                        {
                            // set the order for the project property pages
                            var properties = new List<string>(property.ToString().Split(';'));
                            properties.Insert(1, typeof(Page).GUID.ToString("B"));
                            property = properties.Aggregate("", (a, next) => a + ';' + next).Substring(1);
                            return VSConstants.S_OK;
                        }
                    default:
                        break;
                }
            }
            return result;
        }

        internal uint GetNodeChild(uint itemId)
        {
            object result;
            ErrorHandler.ThrowOnFailure(base.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_FirstChild, out result));
            return (uint)(int)result;
        }

        internal uint GetNodeSibling(uint itemId)
        {
            object result;
            ErrorHandler.ThrowOnFailure(base.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_NextSibling, out result));
            return (uint)(int)result;
        }

        protected override void SetInnerProject(IntPtr innerIUnknown)
        {
            base.SetInnerProject(innerIUnknown);
            innerTarget = (IOleCommandTarget)Marshal.GetObjectForIUnknown(innerIUnknown);
        }
        IOleCommandTarget innerTarget;

        protected override void Close()
        {
            if (hierarchy_event_cookie != (uint)ShellConstants.VSCOOKIE_NIL)
                UnadviseHierarchyEvents(hierarchy_event_cookie);
            base.Close();
        }

        internal string GetMetadata(uint itemId, string property)
        {
            object browseObject;
            ErrorHandler.ThrowOnFailure(base.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_BrowseObject, out browseObject));
            return (string)browseObject.GetType().GetMethod("GetMetadata").Invoke(browseObject, new object[] { property });
        }

        internal string SetMetadata(uint itemId, string property, string value)
        {
            object browseObject;
            ErrorHandler.ThrowOnFailure(base.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_BrowseObject, out browseObject));
            return (string)browseObject.GetType().GetMethod("SetMetadata").Invoke(browseObject, new object[] { property, value });
        }

        #region IProjectManager Members

        public MSBuildManager BuildManager { get; private set; }

        #endregion

        #region IOleCommandTarget Members

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            return innerTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {

            int result = innerTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            if (result != VSConstants.S_OK)
                return result;

            // hide the FSharp project commands on the file nodes (moveup movedown, add above, add below)
            if (pguidCmdGroup.Equals(Constants.guidFSharpProjectCmdSet))
                prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_SUPPORTED | (uint)OLECMDF.OLECMDF_INVISIBLE;

            // show the Add new folder command on the project node
            if (pguidCmdGroup.Equals(Constants.guidStandardCommandSet97) && prgCmds[0].cmdID == 245)
                prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_SUPPORTED | (uint)OLECMDF.OLECMDF_ENABLED;

            return VSConstants.S_OK;
        }

        #endregion
    }
}
