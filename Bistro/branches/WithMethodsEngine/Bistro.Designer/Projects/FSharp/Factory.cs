using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Bistro.Designer.ProjectBase;
using Microsoft.VisualStudio.Shell;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Bistro.Designer.Projects.FSharp
{
    [Guid(Guids.guidFSharpProjectFactoryString)]
    public class Factory : ProjectFactory
    {
        public Factory(Package package)
            : base(package) { }

        protected override ProjectNode CreateProject()
        {
            ProjectManager project = new ProjectManager(this.Package as DesignerPackage);
            project.SetSite((IOleServiceProvider)((IServiceProvider)this.Package).GetService(typeof(IOleServiceProvider)));
            return project;
        }
    }

    [Guid("C39A00F4-B2A4-478e-A0B2-C3E69B3BD899")]
    public class DummyWebFactory { }
}
