using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Bistro.Designer.ProjectBase;
using System.ComponentModel;

namespace Bistro.Designer.Projects.FSharp
{
    [Serializable, 
    ComVisible(true), 
    CLSCompliant(false), 
    ClassInterface(ClassInterfaceType.AutoDual), 
    Guid("65B214E0-6F03-43db-BD1A-20366D71D3F4")//, CompilationMapping(SourceConstructFlags.ObjectType)
    ]
    public class ProjectProperties : ProjectNodeProperties
    {
        // Methods
        public ProjectProperties(ProjectManager node) : base(node)
        {
        }

        // Properties
        [Browsable(false)]
        public string AssemblyName
        {
            get
            {
                return base.Node.ProjectMgr.GetProjectProperty("AssemblyName");
            }
            set
            {
                base.Node.ProjectMgr.SetProjectProperty("AssemblyName", value);
            }
        }

        [Browsable(false)]
        public string OutputFileName
        {
            get
            {
                return "(base.Node.ProjectMgr).OutputFileName";
            }
        }

        [Browsable(false)]
        public int OutputType
        {
            get
            {
                string projectProperty = base.Node.ProjectMgr.GetProjectProperty("OutputType");
                if (string.Equals(projectProperty, "WinExe"))
                {
                    return 0;
                }
                if (string.Equals(projectProperty, "Library"))
                {
                    return 2;
                }
                return 1;
            }
            set
            {
                // This item is obfuscated and can not be translated.
                switch (value)
                {
                    case 0:
                    case 1:
                        return;

                    case 2:
                    {
                        string propertyValue = "Library";
                        base.Node.ProjectMgr.SetProjectProperty("OutputType", propertyValue);
                        return;
                    }
                }
                ArgumentException exception = new ArgumentException("Invalid OutputType value", "value");
                throw exception;
            }
        }

        [Browsable(false)]
        public string PostBuildEvent
        {
            get
            {
                return "base.Node.ProjectMgr.GetUnevaluatedProjectProperty(\"PostBuildEvent\", true)";
            }
            set
            {
                //base.Node.ProjectMgr.SetOrCreateBuildEventProperty("PostBuildEvent", value);
            }
        }

        [Browsable(false)]
        public string PreBuildEvent
        {
            get
            {
                return "base.Node.ProjectMgr.GetUnevaluatedProjectProperty(\"PreBuildEvent\", true)";
            }
            set
            {
                //base.Node.ProjectMgr.SetOrCreateBuildEventProperty("PreBuildEvent", value);
            }
        }

        [Browsable(false)]
        public string ReferencePath
        {
            get
            {
                return base.Node.ProjectMgr.GetProjectProperty("ReferencePath");
            }
            set
            {
                base.Node.ProjectMgr.SetProjectProperty("ReferencePath", value);
            }
        }

        [Browsable(false)]
        public string RootNamespace
        {
            get
            {
                return base.Node.ProjectMgr.GetProjectProperty("RootNamespace");
            }
            set
            {
                base.Node.ProjectMgr.SetProjectProperty("RootNamespace", value);
            }
        }

        [Browsable(false)]
        public int RunPostBuildEvent
        {
            get
            {
                string projectProperty = base.Node.ProjectMgr.GetProjectProperty("RunPostBuildEvent");
                if (string.Equals(projectProperty, "OnBuildSuccess"))
                {
                    return 1;
                }
                if (string.Equals(projectProperty, "OnOutputUpdated"))
                {
                    return 2;
                }
                return 0;
            }
            set
            {
                // This item is obfuscated and can not be translated.
                switch (value)
                {
                    case 0:
                    case 1:
                        return;

                    case 2:
                    {
                        string propertyValue = "OnOutputUpdated";
                        base.Node.ProjectMgr.SetProjectProperty("RunPostBuildEvent", propertyValue);
                        return;
                    }
                }
                ArgumentException exception = new ArgumentException("Invalid RunPostBuildEvent value", "value");
                throw exception;
            }
        }

        [Browsable(false)]
        public uint TargetFramework
        {
            get
            {
                return 0x30005;//ProjectReferenceNode.ParseTargetFrameworkVersion(base.Node.ProjectMgr.GetProjectProperty("TargetFrameworkVersion"));
            }
            set
            {
                // This item is obfuscated and can not be translated.
                switch (value)
                {
                    case 0x20000:
                    {
                        string propertyValue = "v2.0";
                        base.Node.ProjectMgr.SetProjectProperty("TargetFrameworkVersion", propertyValue);
                        return;
                    }
                    case 0x30000:
                    case 0x30005:
                    case 0x40000:
                        return;
                }
                ArgumentException exception = new ArgumentException("Invalid TargetFrameworkVersion number", "value");
                throw exception;
            }
        }

        [Browsable(false)]
        public string Win32ResourceFile
        {
            get
            {
                return base.Node.ProjectMgr.GetProjectProperty("Win32ResourceFile");
            }
            set
            {
                base.Node.ProjectMgr.SetProjectProperty("Win32ResourceFile", value);
            }
        }
    }
}