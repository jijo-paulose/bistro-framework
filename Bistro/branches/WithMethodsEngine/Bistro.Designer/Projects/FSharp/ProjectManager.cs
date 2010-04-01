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
    public class ProjectManager : FlavoredProject
    {
        
         private DesignerPackage package;
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

        // the fsharp debug project propety page - we need to suppress it
        const string debug_page_guid = "{9CFBEB2A-6824-43e2-BD3B-B112FEBC3772}";
        protected override void InitializeForOuter(string fileName, string location, string name, uint flags, ref Guid guidProject, out bool cancel)
        {
            this.fileName = fileName;
            base.InitializeForOuter(fileName, location, name, flags, ref guidProject, out cancel);
        }

        protected override void OnAggregationComplete()
        {
            base.OnAggregationComplete();
        }
        protected override int GetProperty(uint itemId, int propId, out object property)
        {
            return base.GetProperty(itemId, propId, out property);
        }
        protected override void Close()
        {
            base.Close();
        }

    }
}
