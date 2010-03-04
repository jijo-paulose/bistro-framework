using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.MethodsEngine;


namespace Bistro.Designer.Projects
{
    internal interface IProjectData
    {
        List<string> GetSourceFiles();
        List<string> GetRefencedAssemblies();
        EngineControllerDispatcher Engine { get; } 
    }
}
