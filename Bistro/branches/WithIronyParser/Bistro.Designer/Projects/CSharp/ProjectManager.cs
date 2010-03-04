using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Bistro.Designer.Projects.FSharp.Properties;
using Microsoft.VisualStudio.OLE.Interop;

using IOLEServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using System.Reflection;
using Microsoft.Build.BuildEngine;
using System.ComponentModel;

namespace Bistro.Designer.Projects.CSharp
{
    [ComVisible(true)]
    public interface IProjectManager
    {
        Project MSBuildProject { get; }
    }

    [ComVisible(true)]
    public class ProjectManager : FlavoredProjectBase, IProjectManager,IProjectData
    {

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

        // the fsharp debug project propety page - we need to suppress it
        const string debug_page_guid = "{9CFBEB2A-6824-43e2-BD3B-B112FEBC3772}";
        uint hierarchy_event_cookie = (uint)ShellConstants.VSCOOKIE_NIL;
        string fileName;

        protected override void InitializeForOuter(string fileName, string location, string name, uint flags, ref Guid guidProject, out bool cancel)
        {
            this.fileName = fileName;
            base.InitializeForOuter(fileName, location, name, flags, ref guidProject, out cancel);
        }

        protected override void OnAggregationComplete()
        {
            base.OnAggregationComplete();
            MSBuildProject = Microsoft.Build.BuildEngine.Engine.GlobalEngine.GetLoadedProject(fileName);
            //itemList = new ItemList(this, MSBuildProject);
            //hierarchy_event_cookie = AdviseHierarchyEvents(itemList);
        }

        protected override int GetProperty(uint itemId, int propId, out object property)
        {
            switch ((__VSHPROPID)propId)
            {
                case __VSHPROPID.VSHPROPID_FirstChild:
                case __VSHPROPID.VSHPROPID_FirstVisibleChild:
                    //return itemList.GetFirstChild(itemId, out property);
                case __VSHPROPID.VSHPROPID_NextSibling:
                case __VSHPROPID.VSHPROPID_NextVisibleSibling:
                   // return itemList.GetNextSibling(itemId, out property);
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
                    case __VSHPROPID2.VSHPROPID_CfgPropertyPagesCLSIDList:
                        //Remove the Debug page
                        property = property.ToString().Split(';')
                            .Aggregate("", (a, next) => next.Equals(debug_page_guid, StringComparison.OrdinalIgnoreCase) ? a : a + ';' + next).Substring(1);
                        return VSConstants.S_OK;
                    case __VSHPROPID2.VSHPROPID_PropertyPagesCLSIDList:
                        {
                            //Add the CompileOrder property page.
                            var properties = new List<string>(property.ToString().Split(';'));
                            properties.Add(typeof(CompileOrderPage).GUID.ToString("B"));
                            property = properties.Aggregate("", (a, next) => a + ';' + next).Substring(1);
                            return VSConstants.S_OK;
                        }
                    case __VSHPROPID2.VSHPROPID_PriorityPropertyPagesCLSIDList:
                        {
                            // set the order for the project property pages
                            var properties = new List<string>(property.ToString().Split(';'));
                            properties.Insert(1, typeof(CompileOrderPage).GUID.ToString("B"));
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

        public Project MSBuildProject
        {
            get;
            private set;
        }

        #endregion

        #region IProjectData Members

        public List<string> GetSourceFiles()
        {
            List<string> files = new List<string>();
            string path = this.MSBuildProject.FullFileName;
            int len = path.LastIndexOf("\\");
            path = path.Substring(0, len + 1);
            // Iterate through each ItemGroup in the Project to obtain the list of F# source files
            foreach (BuildItemGroup ig in this.MSBuildProject.ItemGroups)
            {
                foreach (BuildItem item in ig)
                {
                    if (String.Compare(item.Name, "Compile") == 0)
                    {
                        if (item.Include.EndsWith(".cs"))
                        {
                            files.Add(path + item.Include);
                        }

                    }
                    else
                        break;
                }
            }
            return files;

        }

        public List<string> GetRefencedAssemblies()
        {
            throw new NotImplementedException();
        }

        public Bistro.MethodsEngine.EngineControllerDispatcher Engine
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
