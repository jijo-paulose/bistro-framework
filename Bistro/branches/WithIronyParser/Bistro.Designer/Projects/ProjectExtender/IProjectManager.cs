using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.BuildEngine;

namespace FSharp.ProjectExtender
{
    public interface IProjectManager
    {
        Project MSBuildProject { get; }
    }
}
