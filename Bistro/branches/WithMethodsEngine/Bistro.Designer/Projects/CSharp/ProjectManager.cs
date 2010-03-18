using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

using IOLEServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using System.Reflection;
using Microsoft.Build.BuildEngine;
using System.ComponentModel;
using Bistro.Configuration;


namespace Bistro.Designer.Projects.CSharp
{

    [ComVisible(true)]
    public class ProjectManager : FlavoredProjectBase,IProjectManager
    {

        private DesignerPackage package;
        string fileName;
        string projectName;
        string projectDir;
        string projectExt;
        bool initialized;
        public ProjectManager(DesignerPackage package)
            : base()
        {
            this.package = package;

        }
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


        protected override void InitializeForOuter(string fileName, string location, string name, uint flags, ref Guid guidProject, out bool cancel)
        {
            this.fileName = fileName;
            base.InitializeForOuter(fileName, location, name, flags, ref guidProject, out cancel);
        }

        protected override void OnAggregationComplete()
        {
            base.OnAggregationComplete();
            projectExt = ".csproj";
            Tracker = new Explorer.ChangesTracker(projectExt);
            Tracker.RegisterObserver(package.explorer);
        }
        protected override int GetProperty(uint itemId, int propId, out object property)
        {
           
          int result = base.GetProperty(itemId, propId, out property);
          if (itemId == VSConstants.VSITEMID_ROOT)
           {
               switch ((__VSHPROPID)propId)
               {
                   
                   case __VSHPROPID.VSHPROPID_ProjectDir:
                       projectDir = property.ToString();
                       break;

                   case __VSHPROPID.VSHPROPID_Name:
                       if (projectName != property.ToString())
                       {
                           projectName = property.ToString();
                           Tracker.OnProjectRenamed(projectName);
                       }
                       if (!initialized)
                       {
                           
                           string path = projectDir + "\\" + projectName + projectExt;
                           MSBuildProject = Microsoft.Build.BuildEngine.Engine.GlobalEngine.GetLoadedProject(path);
                           Tracker.OnProjectOpened(GetSourceFiles());
                           initialized = true;
                       }

                       break;
                   case __VSHPROPID.VSHPROPID_SaveName:
                       //property is a new name of the project - notify TreeView
                       if (property != null)
                        Tracker.RaiseNodesChanged(null, property.ToString(), projectName,false);

                       break;
               }
           }
           else
           {
              switch ((__VSHPROPID)propId)
              {
                  case __VSHPROPID.VSHPROPID_Name:
                      if (property.ToString().EndsWith(".cs"))
                      {
                          Tracker.ActiveFile = projectDir + "\\Controllers\\" + property.ToString();
                      }
                      break;
                  case __VSHPROPID.VSHPROPID_SaveName:
                      //property is a new name of the project item -> need to rename corresponding key
                      break;
              }
           }
           return result;
        }
        protected override int SetProperty(uint itemId, int propId, object property)
        {
            return base.SetProperty(itemId, propId, property);
        }
        protected override void Close()
        {
            base.Close();
            Tracker = null;
        }

        #region IProjectManager Members

        public Project MSBuildProject
        {
            get;
            set;
        }
        public List<string> GetSourceFiles()
        {
            List<string> files = new List<string>();
            // Iterate through each ItemGroup in the Project to obtain the list of F# source files
            foreach (BuildItemGroup ig in this.MSBuildProject.ItemGroups)
            {
                foreach (BuildItem item in ig)
                {
                    if (String.Compare(item.Name, "Compile") == 0)
                    {
                        if (item.Include.EndsWith(".cs"))
                        {
                            files.Add(projectDir + "\\" + item.Include);
                        }

                    }
                    else
                        break;
                }
            }
            return files;

        }
        public Explorer.ChangesTracker Tracker { get; set; }


        #endregion
    }
}
