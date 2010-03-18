using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Bistro.MethodsEngine;
using Microsoft.Build.BuildEngine;
using System.ComponentModel;
using Bistro.Designer;
namespace Bistro.Designer.Projects
{
    [ComVisible(true)]
    public interface IProjectManager
    {
        Project MSBuildProject { get; set; }
        List<string> GetSourceFiles();
        Explorer.ChangesTracker Tracker { get; set; } 
    }

}
