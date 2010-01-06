using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace Bistro.Designer.Projects.FSharp
{
    public class ProjectConfig : ProjectBase.ProjectConfig
    {
        public ProjectConfig(ProjectManager project, string configuration)
            : base(project, configuration)
        { }

        public override int DebugLaunch(uint grfLaunch)
        {
            return base.DebugLaunch(grfLaunch);
        }

    }
}
