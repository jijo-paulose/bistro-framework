using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Build.BuildEngine;

using Bistro.Configuration;

using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;

namespace Bistro.Designer.Projects.FSharp
{
    [ComVisible(true)]
    public class ProjectManager : FlavoredProjectBase, IProjectManager
    {
        
         private DesignerPackage package;

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

        // the fsharp debug project propety page - we need to suppress it
        const string debug_page_guid = "{9CFBEB2A-6824-43e2-BD3B-B112FEBC3772}";
        internal string fileName;

        protected override void InitializeForOuter(string fileName, string location, string name, uint flags, ref Guid guidProject, out bool cancel)
        {
            this.fileName = fileName;
            base.InitializeForOuter(fileName, location, name, flags, ref guidProject, out cancel);
        }

        protected override void OnAggregationComplete()
        {
            base.OnAggregationComplete();
            MSBuildProject = Microsoft.Build.BuildEngine.Engine.GlobalEngine.GetLoadedProject(fileName);
            //package.explorer.projectMngrs.Add(fileName, this);
            SectionHandler sh = new SectionHandler();
            sh.Application = "Bistro.Application";
            sh.LoggerFactory = "Bistro.Logging.DefaultLoggerFactory";
            Bistro.Application.Initialize(sh);
            Engine = new Bistro.MethodsEngine.EngineControllerDispatcher(Bistro.Application.Instance);
        }

        protected override int GetProperty(uint itemId, int propId, out object property)
        {

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
                    default:
                        break;
                }
            }
            return result;
        }


        #region IProjectManager Members

        public Project MSBuildProject
        {
            get;
            set;
        }
        public string ProjectPath
        {
            get;
            set;
        }
        public List<string> GetSourceFiles()
        {
            List<string> files = new List<string>();
            string path = this.MSBuildProject.FullFileName;
            int len = path.LastIndexOf("\\");
            ProjectPath = path.Substring(0, len + 1);
            // Iterate through each ItemGroup in the Project to obtain the list of F# source files
            foreach (BuildItemGroup ig in this.MSBuildProject.ItemGroups)
            {
                foreach (BuildItem item in ig)
                {
                    if (String.Compare(item.Name, "Compile") == 0)
                    {
                        if (item.Include.EndsWith(".fs"))
                        {
                            files.Add(ProjectPath + item.Include);
                        }

                    }
                    else
                        break;
                }
            }
            return files;
        }
        public Bistro.MethodsEngine.EngineControllerDispatcher Engine
        {
            get;
            set;
        }
        #endregion
    }
}
