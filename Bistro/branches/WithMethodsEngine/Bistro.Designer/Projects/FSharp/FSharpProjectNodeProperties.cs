using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Bistro.Designer.ProjectBase;
using System.ComponentModel;

namespace Bistro.Designer.Projects.FSharp
{
    [Serializable, ComVisible(true), CLSCompliant(false), ClassInterface(ClassInterfaceType.AutoDual), Guid("FC487B15-535A-43ad-A425-09BC71957B93")]
    public class FSharpProjectNodeProperties : ProjectNodeProperties
    {
        // Methods
        internal FSharpProjectNodeProperties(ProjectManager node)
            : base(node)
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

        //[Browsable(false)]
        //public string OutputFileName
        //{
        //    get
        //    {
        //        return ((ProjectManager)base.Node.ProjectMgr).OutputFileName;
        //    }
        //}

        [Browsable(false)]
        public OutputType OutputType
        {
            get
            {
                return OutputType.Library;
            }
            set
            {
            }
        }

        //[Browsable(false)]
        //public string PostBuildEvent
        //{
        //    get
        //    {
        //        return base.Node.ProjectMgr.GetUnevaluatedProjectProperty("PostBuildEvent", true);
        //    }
        //    set
        //    {
        //        base.Node.ProjectMgr.SetOrCreateBuildEventProperty("PostBuildEvent", value);
        //    }
        //}

        //[Browsable(false)]
        //public string PreBuildEvent
        //{
        //    get
        //    {
        //        return base.Node.ProjectMgr.GetUnevaluatedProjectProperty("PreBuildEvent", true);
        //    }
        //    set
        //    {
        //        base.Node.ProjectMgr.SetOrCreateBuildEventProperty("PreBuildEvent", value);
        //    }
        //}

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

        //[Browsable(false)]
        //public int RunPostBuildEvent
        //{
        //    get
        //    {
        //        string projectProperty = base.Node.ProjectMgr.GetProjectProperty("RunPostBuildEvent");
        //        if (string.Equals(projectProperty, "OnBuildSuccess"))
        //        {
        //            return 1;
        //        }
        //        if (string.Equals(projectProperty, "OnOutputUpdated"))
        //        {
        //            return 2;
        //        }
        //        return 0;
        //    }
        //    set
        //    {
        //        // This item is obfuscated and can not be translated.
        //        switch (value)
        //        {
        //            case 0:
        //            case 1:
        //                goto Label_004F;

        //            case 2:
        //            {
        //                string propertyValue = "OnOutputUpdated";
        //                base.Node.ProjectMgr.SetProjectProperty("RunPostBuildEvent", propertyValue);
        //                return;
        //            }
        //        }
        //        ArgumentException exception = new ArgumentException(FSharpSR.resources.Value.GetString("InvalidRunPostBuildEvent", CultureInfo.CurrentUICulture), "value");
        //        throw exception;
        //    }
        //}

        //[Browsable(false)]
        //public uint TargetFramework
        //{
        //    get
        //    {
        //        return ProjectReferenceNode.ParseTargetFrameworkVersion(base.Node.ProjectMgr.GetProjectProperty("TargetFrameworkVersion"));
        //    }
        //    set
        //    {
        //        // This item is obfuscated and can not be translated.
        //        switch (value)
        //        {
        //            case 0x20000:
        //            {
        //                string propertyValue = "v2.0";
        //                base.Node.ProjectMgr.SetProjectProperty("TargetFrameworkVersion", propertyValue);
        //                return;
        //            }
        //            case 0x30000:
        //            case 0x30005:
        //            case 0x40000:
        //                goto Label_0085;
        //        }
        //        ArgumentException exception = new ArgumentException(FSharpSR.resources.Value.GetString("InvalidTargetFrameworkVersion", CultureInfo.CurrentUICulture), "value");
        //        throw exception;
        //    }
        //}

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

 
