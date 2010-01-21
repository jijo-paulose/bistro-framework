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

namespace Bistro.Designer.Projects.FSharp
{
    [Guid(Guids.guidFSharpProjectFactoryString)]
    public class Factory : FlavoredProjectFactoryBase
    {
        private DesignerPackage package;
        public Factory(DesignerPackage package)
            : base() 
        {
            this.package = package;
        }

        //protected override ProjectNode CreateProject()
        //{
        //    ProjectManager project = new ProjectManager(this.Package as DesignerPackage);
        //    project.SetSite((IOleServiceProvider)((IServiceProvider)this.Package).GetService(typeof(IOleServiceProvider)));
        //    return project;
        //}

        protected override object PreCreateForOuter(IntPtr outerProjectIUnknown)
        {
            var project = new ProjectManager();
            project.SetSite((IOleServiceProvider)((IServiceProvider)package).GetService(typeof(IOleServiceProvider)));
            //ILocalRegistryCorrected globalService = (ILocalRegistryCorrected)Package.GetGlobalService(typeof(SLocalRegistry));
            //Guid clsid = new Guid("{C402364C-5474-47e7-AE72-BF5418780221}");
            //Guid riid = VSConstants.IID_IUnknown;
            //uint dwFlags = 1;
            //IntPtr ppvObj = IntPtr.Zero;
            //IVsProjectAggregator2 objectForIUnknown = null;
            //int hr = globalService.CreateInstance(clsid, outerProjectIUnknown, ref riid, dwFlags, out ppvObj);
            //var o = Marshal.CreateAggregatedObject(ppvObj, project);
            return project;
        }
    }

    [Guid("C39A00F4-B2A4-478e-A0B2-C3E69B3BD899")]
    public class DummyWebFactory { }
}
