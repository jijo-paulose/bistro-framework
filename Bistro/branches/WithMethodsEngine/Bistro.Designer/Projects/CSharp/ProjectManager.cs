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
    public class ProjectManager : FlavoredProject
    {

        private DesignerPackage package;
        private Explorer.ProjectFileManager observer;
        string fileName;
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
            observer = new Explorer.ProjectFileManager(this as IVsProject,"csproj",package.explorer);

        }

        protected override int GetProperty(uint itemId, int propId, out object property)
        {
           
          return base.GetProperty(itemId, propId, out property);
        }
        protected override int SetProperty(uint itemId, int propId, object property)
        {
            return base.SetProperty(itemId, propId, property);
        }
        protected override void Close()
        {
            base.Close();
            //delete object - dispose
            observer.Dispose();
        }
        

        #region IProjectManager Members

        internal Project MSBuildProject
        {
            get;
            set;
        }
        internal Explorer.ProjectFileManager Observer { get; set; }
        #endregion
    }
}
