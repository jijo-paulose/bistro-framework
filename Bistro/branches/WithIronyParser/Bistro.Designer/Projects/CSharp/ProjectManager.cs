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

using Bistro;
using Bistro.Configuration;
using Bistro.Configuration.Logging;
using Bistro.Controllers;
using Bistro.MethodsEngine;

namespace Bistro.Designer.Projects.CSharp
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

        internal string fileName;

        protected override void InitializeForOuter(string fileName, string location, string name, uint flags, ref Guid guidProject, out bool cancel)
        {
            this.fileName = fileName;
            base.InitializeForOuter(fileName, location, name, flags, ref guidProject, out cancel);
        }

        public FSharp.ItemList ItemList { get { return null; } }

        protected override void OnAggregationComplete()
        {
            base.OnAggregationComplete();
            MSBuildProject = Microsoft.Build.BuildEngine.Engine.GlobalEngine.GetLoadedProject(fileName);
            package.explorer.projectMngrs.Add(fileName, this);
            SectionHandler sh = new SectionHandler();
            sh.Application = "Bistro.Application";
            sh.LoggerFactory = "Bistro.Logging.DefaultLoggerFactory";
            Bistro.Application.Initialize(sh);
            Engine = new Bistro.MethodsEngine.EngineControllerDispatcher(Bistro.Application.Instance);

        }

        protected override int GetProperty(uint itemId, int propId, out object property)
        {
 
            return  base.GetProperty(itemId, propId, out property);

        }

        protected override void Close()
        {
            base.Close();
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
                        if (item.Include.EndsWith(".cs"))
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

        public List<string> GetRefencedAssemblies()
        {
            throw new NotImplementedException();
        }

        public Bistro.MethodsEngine.EngineControllerDispatcher Engine
        {
            get;
            set;
        }

        #endregion
    }
}
