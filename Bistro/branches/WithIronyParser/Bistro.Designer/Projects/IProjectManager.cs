using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Bistro.MethodsEngine;
using Microsoft.Build.BuildEngine;
using System.ComponentModel;

namespace Bistro.Designer.Projects
{

    public interface IProjectManager
    {
        Project MSBuildProject { get; set; }
        List<string> GetSourceFiles();
        List<string> GetRefencedAssemblies();
        EngineControllerDispatcher Engine { get; set; }
        string ProjectPath { get; set; }
    }

}
