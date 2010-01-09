using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSLangProj;
using System.Runtime.InteropServices;
using Bistro.Designer.ProjectBase;

namespace Bistro.Designer.Projects.FSharp
{
    [ComVisible(true), CLSCompliant(false)]
    public class OAProjectProperties : ProjectNodeProperties, ProjectProperties 
    {
        ProjectManager project;
        public OAProjectProperties(ProjectManager project)
            : base(project)
        { this.project = project; }

        //new PropertyControlData(0x271d, "AssemblyName", this.AssemblyName, new Control[] { this.AssemblyNameLabel }), 
        //new PropertyControlData(0x2754, "DefaultNamespace", this.RootNameSpace, new Control[] { this.RootNamespaceLabel }), 
        //new PropertyControlData(0x271b, "ApplicationIcon", this.ApplicationIcon, new PropertyControlData.SetDelegate(this.ApplicationIconSet), new PropertyControlData.GetDelegate(this.ApplicationIconGet), ControlDataFlags.UserHandledEvents, new Control[] { this.AppIconImage, this.AppIconBrowse, this.IconRadioButton, this.ApplicationIconLabel }), 
        //new PropertyControlData(0x2720, "OutputType", this.OutputType, new PropertyControlData.SetDelegate(this.OutputTypeSet), new PropertyControlData.GetDelegate(this.OutputTypeGet), ControlDataFlags.UserHandledEvents, new Control[] { this.OutputTypeLabel }), 
        //new PropertyControlData(0x271f, "StartupObject", this.StartupObject, new PropertyControlData.SetDelegate(this.StartupObjectSet), new PropertyControlData.GetDelegate(this.StartupObjectGet), ControlDataFlags.UserHandledEvents, new Control[] { this.StartupObjectLabel }), 
        //new PropertyControlData(0x2f50, "Win32ResourceFile", this.Win32ResourceFile, new PropertyControlData.SetDelegate(this.Win32ResourceSet), new PropertyControlData.GetDelegate(this.Win32ResourceGet), ControlDataFlags.None, new Control[] { this.Win32ResourceFileBrowse, this.Win32ResourceRadioButton }), 
        //new PropertyControlData(0x2fa9, "ApplicationManifest", this.ApplicationManifest, new PropertyControlData.SetDelegate(this.ApplicationManifestSet), new PropertyControlData.GetDelegate(this.ApplicationManifestGet), ControlDataFlags.UserHandledEvents, new Control[] { this.ApplicationManifest, this.ApplicationManifestLabel }), 
        //new PropertyControlData(0x300c, "TargetFrameworkSubset", this.ClientSubsetCheckbox, new PropertyControlData.SetDelegate(this.SetClientSubset), new PropertyControlData.GetDelegate(this.GetClientSubset), ControlDataFlags.None, new Control[] { this.ClientSubsetCheckbox }), 
        //new PropertyControlData(0x2fa8, "TargetFramework", this.TargetFramework, new PropertyControlData.SetDelegate(this.SetTargetFramework), new PropertyControlData.GetDelegate(this.GetTargetFramework), ControlDataFlags.NoOptimisticFileCheckout | ControlDataFlags.ProjectMayBeReloadedDuringPropertySet, new Control[] { this.TargetFrameworkLabel }) };
        //new PropertyControlData(0x2715, "DefineConstants", this.txtConditionalCompilationSymbols, new PropertyControlData.MultiValueSetDelegate(this.ConditionalCompilationSet), new PropertyControlData.MultiValueGetDelegate(this.ConditionalCompilationGet), ControlDataFlags.None, new Control[] { this.txtConditionalCompilationSymbols, this.chkDefineDebug, this.chkDefineTrace, this.lblConditionalCompilationSymbols }), 
        //new PropertyControlData(0x277d, "PlatformTarget", this.cboPlatformTarget, new PropertyControlData.SetDelegate(this.PlatformTargetSet), new PropertyControlData.GetDelegate(this.PlatformTargetGet), ControlDataFlags.None, new Control[] { this.lblPlatformTarget }), 
        //new PropertyControlData(0x274d, "AllowUnsafeBlocks", this.chkAllowUnsafeCode), new SingleConfigPropertyControlData(SingleConfigPropertyControlData.Configs.Release, 0x2750, "Optimize", this.chkOptimizeCode), 
        //new PropertyControlData(0x2744, "WarningLevel", this.cboWarningLevel, new PropertyControlData.SetDelegate(this.WarningLevelSet), new PropertyControlData.GetDelegate(this.WarningLevelGet), ControlDataFlags.None, new Control[] { this.lblWarningLevel }), new PropertyControlData(0x275f, "NoWarn", this.txtSupressWarnings, new Control[] { this.lblSupressWarnings }), new PropertyControlData(0x2745, "TreatWarningsAsErrors", this.rbWarningAll, new PropertyControlData.SetDelegate(this.TreatWarningsInit), new PropertyControlData.GetDelegate(this.TreatWarningsGet)), new PropertyControlData(0x2f59, "TreatSpecificWarningsAsErrors", this.txtSpecificWarnings, new PropertyControlData.SetDelegate(this.TreatSpecificWarningsInit), new PropertyControlData.GetDelegate(this.TreatSpecificWarningsGet)), new SingleConfigPropertyControlData(SingleConfigPropertyControlData.Configs.Release, 0x2714, "OutputPath", this.txtOutputPath, new Control[] { this.lblOutputPath }), new PropertyControlData(0x274f, "DocumentationFile", this.txtXMLDocumentationFile, new PropertyControlData.MultiValueSetDelegate(this.XMLDocumentationFileInit), new PropertyControlData.MultiValueGetDelegate(this.XMLDocumentationFileGet), ControlDataFlags.None, new Control[] { this.txtXMLDocumentationFile, this.chkXMLDocumentationFile }), new PropertyControlData(0x2758, "RegisterForComInterop", this.chkRegisterForCOM, new PropertyControlData.SetDelegate(this.RegisterForCOMInteropSet), new PropertyControlData.GetDelegate(this.RegisterForCOMInteropGet)), new PropertyControlData(0x2720, "OutputType", null, new PropertyControlData.SetDelegate(this.OutputTypeSet), null), new SingleConfigPropertyControlData(SingleConfigPropertyControlData.Configs.Release, 0x277f, "GenerateSerializationAssemblies", this.cboSGenOption, new Control[] { this.lblSGenOption }) };

        public uint TargetFramework
        {
            get { return 0x20000; }
            set { }
        }

        string defaultNamespace = "";
        public string DefaultNamespace
        {
            get { return defaultNamespace; }
            set { defaultNamespace = value; }
        }

        string assemblyName = "";
        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; }
        }

        string preBuildEvent = "";
        public string PreBuildEvent
        {
            get { return preBuildEvent; }
            set { preBuildEvent = value; }
        }

        string postBuildEvent = "";
        public string PostBuildEvent
        {
            get { return postBuildEvent; }
            set { postBuildEvent = value; }
        }

        string runPostBuildEvent = "Always";
        public string RunPostBuildEvent
        {
            get { return runPostBuildEvent; }
            set { runPostBuildEvent = value; }
        }

        #region ProjectProperties Members

        string ProjectProperties.AbsoluteProjectDirectory
        {
            get { throw new NotImplementedException(); }
        }

        ProjectConfigurationProperties ProjectProperties.ActiveConfigurationSettings
        {
            get { throw new NotImplementedException(); }
        }

        string ProjectProperties.ActiveFileSharePath
        {
            get { throw new NotImplementedException(); }
        }

        prjWebAccessMethod ProjectProperties.ActiveWebAccessMethod
        {
            get { throw new NotImplementedException(); }
        }

        string ProjectProperties.ApplicationIcon
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ProjectProperties.AssemblyKeyContainerName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ProjectProperties.AssemblyName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ProjectProperties.AssemblyOriginatorKeyFile
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        prjOriginatorKeyMode ProjectProperties.AssemblyOriginatorKeyMode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        prjScriptLanguage ProjectProperties.DefaultClientScript
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        prjHTMLPageLayout ProjectProperties.DefaultHTMLPageLayout
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ProjectProperties.DefaultNamespace
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        prjTargetSchema ProjectProperties.DefaultTargetSchema
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        bool ProjectProperties.DelaySign
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ProjectProperties.ExtenderCATID
        {
            get { throw new NotImplementedException(); }
        }

        object ProjectProperties.ExtenderNames
        {
            get { throw new NotImplementedException(); }
        }

        string ProjectProperties.FileName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ProjectProperties.FileSharePath
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ProjectProperties.FullPath
        {
            get { throw new NotImplementedException(); }
        }

        bool ProjectProperties.LinkRepair
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ProjectProperties.LocalPath
        {
            get { throw new NotImplementedException(); }
        }

        string ProjectProperties.OfflineURL
        {
            get { throw new NotImplementedException(); }
        }

        prjCompare ProjectProperties.OptionCompare
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        prjOptionExplicit ProjectProperties.OptionExplicit
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        prjOptionStrict ProjectProperties.OptionStrict
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ProjectProperties.OutputFileName
        {
            get { throw new NotImplementedException(); }
        }

        prjOutputType ProjectProperties.OutputType
        {
            get
            {
                return prjOutputType.prjOutputTypeLibrary;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        prjProjectType ProjectProperties.ProjectType
        {
            get { throw new NotImplementedException(); }
        }

        string ProjectProperties.ReferencePath
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ProjectProperties.RootNamespace
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ProjectProperties.ServerExtensionsVersion
        {
            get { throw new NotImplementedException(); }
        }

        string ProjectProperties.StartupObject
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ProjectProperties.URL
        {
            get { throw new NotImplementedException(); }
        }

        prjWebAccessMethod ProjectProperties.WebAccessMethod
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ProjectProperties.WebServer
        {
            get { throw new NotImplementedException(); }
        }

        string ProjectProperties.WebServerVersion
        {
            get { throw new NotImplementedException(); }
        }

        string ProjectProperties.__id
        {
            get { throw new NotImplementedException(); }
        }

        object ProjectProperties.__project
        {
            get { throw new NotImplementedException(); }
        }

        object ProjectProperties.get_Extender(string ExtenderName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
