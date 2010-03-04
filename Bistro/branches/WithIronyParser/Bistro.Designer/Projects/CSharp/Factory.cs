using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Bistro.Designer.Projects.CSharp
{
    [Guid(Guids.guidCSharpProjectFactoryString)]
    public class Factory : FlavoredProjectFactoryBase
    {
        private DesignerPackage package;
        internal ProjectManager projectMngr;
        public Factory(DesignerPackage package)
            : base()
        {
            this.package = package;
        }

        protected override object PreCreateForOuter(IntPtr outerProjectIUnknown)
        {
            //var project = new ProjectManager();
            projectMngr = new ProjectManager();
            projectMngr.SetSite((IOleServiceProvider)((IServiceProvider)package).GetService(typeof(IOleServiceProvider)));
            //return project;
            return projectMngr;
        }

    }
    [Guid("BA26ED4A-8279-40FD-8D03-3A36795578AD")]
    public class DummyWebFactory { }

}
