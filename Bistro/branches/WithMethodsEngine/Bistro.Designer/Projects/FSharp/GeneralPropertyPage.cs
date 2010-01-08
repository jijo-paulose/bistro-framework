using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Bistro.Designer.ProjectBase;
using System.Diagnostics;
using Microsoft.VisualStudio;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Bistro.Designer.Projects.FSharp
{
    internal enum GeneralPropertyPageTag
    {
        AssemblyName,
        OutputType,
        RootNamespace,
        StartupObject,
        ApplicationIcon,
        TargetPlatform,
        TargetPlatformLocation
    }

    [ComVisible(true), Guid("853F920C-9CB1-45af-95D9-F8E8864A41CA")]
    public class GeneralPropertyPage : SettingsPage, EnvDTE80.IInternalExtenderProvider
    {
        #region fields
        private string assemblyName;
        private OutputType outputType;
        private string defaultNamespace;
        private string startupObject;
        private string applicationIcon;
        private PlatformType targetPlatform = PlatformType.v2;
        private string targetPlatformLocation;
        #endregion

        public GeneralPropertyPage()
        {
            this.Name = "GeneralCaption"; // TODO: SR.GetString(SR.GeneralCaption);
        }

        #region overriden methods
        public override string GetClassName()
        {
            return this.GetType().FullName;
        }

        protected override void BindProperties()
        {
            if (this.ProjectMgr == null)
            {
                Debug.Assert(false);
                return;
            }

            this.assemblyName = this.ProjectMgr.GetProjectProperty(GeneralPropertyPageTag.AssemblyName.ToString(), true);

            string outputType = this.ProjectMgr.GetProjectProperty(GeneralPropertyPageTag.OutputType.ToString(), false);

            if (outputType != null && outputType.Length > 0)
            {
                try
                {
                    this.outputType = (OutputType)Enum.Parse(typeof(OutputType), outputType);
                }
                catch
                { } //Should only fail if project file is corrupt
            }

            this.defaultNamespace = this.ProjectMgr.GetProjectProperty(GeneralPropertyPageTag.RootNamespace.ToString(), false);
            this.startupObject = this.ProjectMgr.GetProjectProperty(GeneralPropertyPageTag.StartupObject.ToString(), false);
            this.applicationIcon = this.ProjectMgr.GetProjectProperty(GeneralPropertyPageTag.ApplicationIcon.ToString(), false);

            string targetPlatform = this.ProjectMgr.GetProjectProperty(GeneralPropertyPageTag.TargetPlatform.ToString(), false);

            if (targetPlatform != null && targetPlatform.Length > 0)
            {
                try
                {
                    this.targetPlatform = (PlatformType)Enum.Parse(typeof(PlatformType), targetPlatform);
                }
                catch
                { }
            }

            this.targetPlatformLocation = this.ProjectMgr.GetProjectProperty(GeneralPropertyPageTag.TargetPlatformLocation.ToString(), false);
        }

        protected override int ApplyChanges()
        {
            if (this.ProjectMgr == null)
            {
                Debug.Assert(false);
                return VSConstants.E_INVALIDARG;
            }

            ValidateProperties();

            this.ProjectMgr.SetProjectProperty(GeneralPropertyPageTag.AssemblyName.ToString(), this.assemblyName);
            this.ProjectMgr.SetProjectProperty(GeneralPropertyPageTag.OutputType.ToString(), this.outputType.ToString());
            this.ProjectMgr.SetProjectProperty(GeneralPropertyPageTag.RootNamespace.ToString(), this.defaultNamespace);
            this.ProjectMgr.SetProjectProperty(GeneralPropertyPageTag.StartupObject.ToString(), this.startupObject);
            this.ProjectMgr.SetProjectProperty(GeneralPropertyPageTag.ApplicationIcon.ToString(), this.applicationIcon);
            this.ProjectMgr.SetProjectProperty(GeneralPropertyPageTag.TargetPlatform.ToString(), this.targetPlatform.ToString());
            this.ProjectMgr.SetProjectProperty(GeneralPropertyPageTag.TargetPlatformLocation.ToString(), this.targetPlatformLocation);
            this.IsDirty = false;

            return VSConstants.S_OK;
        }
        #endregion

        #region exposed properties
        [SRCategoryAttribute(InternalSR.Application)]
        [LocDisplayName(InternalSR.AssemblyName)]
        [SRDescriptionAttribute(InternalSR.AssemblyNameDescription)]
        public string AssemblyName
        {
            get { return this.assemblyName; }
            set { this.assemblyName = value; this.IsDirty = true; }
        }

        [SRCategoryAttribute(InternalSR.Application)]
        [LocDisplayName(InternalSR.OutputType)]
        [SRDescriptionAttribute(InternalSR.OutputTypeDescription)]
        public OutputType OutputType
        {
            get { return this.outputType; }
            set { this.outputType = value; this.IsDirty = true; }
        }

        [SRCategoryAttribute(InternalSR.Application)]
        [LocDisplayName(InternalSR.DefaultNamespace)]
        [SRDescriptionAttribute(InternalSR.DefaultNamespaceDescription)]
        public string DefaultNamespace
        {
            get { return this.defaultNamespace; }
            set { this.defaultNamespace = value; this.IsDirty = true; }
        }

        [SRCategoryAttribute(InternalSR.Application)]
        [LocDisplayName(InternalSR.StartupObject)]
        [SRDescriptionAttribute(InternalSR.StartupObjectDescription)]
        public string StartupObject
        {
            get { return this.startupObject; }
            set { this.startupObject = value; this.IsDirty = true; }
        }

        [SRCategoryAttribute(InternalSR.Application)]
        [LocDisplayName(InternalSR.ApplicationIcon)]
        [SRDescriptionAttribute(InternalSR.ApplicationIconDescription)]
        public string ApplicationIcon
        {
            get { return this.applicationIcon; }
            set { this.applicationIcon = value; this.IsDirty = true; }
        }

        [SRCategoryAttribute(InternalSR.Project)]
        [LocDisplayName(InternalSR.ProjectFile)]
        [SRDescriptionAttribute(InternalSR.ProjectFileDescription)]
        [AutomationBrowsable(false)]
        public string ProjectFile
        {
            get { return Path.GetFileName(this.ProjectMgr.ProjectFile); }
        }

        [SRCategoryAttribute(InternalSR.Project)]
        [LocDisplayName(InternalSR.ProjectFolder)]
        [SRDescriptionAttribute(InternalSR.ProjectFolderDescription)]
        [AutomationBrowsable(false)]
        public string ProjectFolder
        {
            get { return Path.GetDirectoryName(this.ProjectMgr.ProjectFolder); }
        }

        [SRCategoryAttribute(InternalSR.Project)]
        [LocDisplayName(InternalSR.OutputFile)]
        [SRDescriptionAttribute(InternalSR.OutputFileDescription)]
        [AutomationBrowsable(false)]
        public string OutputFile
        {
            get
            {
                return this.assemblyName + ".dll";
            }
        }

        [SRCategoryAttribute(InternalSR.Project)]
        [LocDisplayName(InternalSR.TargetPlatform)]
        [SRDescriptionAttribute(InternalSR.TargetPlatformDescription)]
        [AutomationBrowsable(false)]
        public PlatformType TargetPlatform
        {
            get { return this.targetPlatform; }
            set { this.targetPlatform = value; IsDirty = true; }
        }

        [SRCategoryAttribute(InternalSR.Project)]
        [LocDisplayName(InternalSR.TargetPlatformLocation)]
        [SRDescriptionAttribute(InternalSR.TargetPlatformLocationDescription)]
        [AutomationBrowsable(false)]
        public string TargetPlatformLocation
        {
            get { return this.targetPlatformLocation; }
            set { this.targetPlatformLocation = value; IsDirty = true; }
        }
        #endregion

        #region IInternalExtenderProvider Members

        bool EnvDTE80.IInternalExtenderProvider.CanExtend(string extenderCATID, string extenderName, object extendeeObject)
        {
            IVsHierarchy outerHierarchy = HierarchyNode.GetOuterHierarchy(this.ProjectMgr);
            if (outerHierarchy is EnvDTE80.IInternalExtenderProvider)
                return ((EnvDTE80.IInternalExtenderProvider)outerHierarchy).CanExtend(extenderCATID, extenderName, extendeeObject);
            return false;
        }

        object EnvDTE80.IInternalExtenderProvider.GetExtender(string extenderCATID, string extenderName, object extendeeObject, EnvDTE.IExtenderSite extenderSite, int cookie)
        {
            IVsHierarchy outerHierarchy = HierarchyNode.GetOuterHierarchy(this.ProjectMgr);
            if (outerHierarchy is EnvDTE80.IInternalExtenderProvider)
                return ((EnvDTE80.IInternalExtenderProvider)outerHierarchy).GetExtender(extenderCATID, extenderName, extendeeObject, extenderSite, cookie);
            return null;
        }

        object EnvDTE80.IInternalExtenderProvider.GetExtenderNames(string extenderCATID, object extendeeObject)
        {
            IVsHierarchy outerHierarchy = HierarchyNode.GetOuterHierarchy(this.ProjectMgr);
            if (outerHierarchy is EnvDTE80.IInternalExtenderProvider)
                return ((EnvDTE80.IInternalExtenderProvider)outerHierarchy).GetExtenderNames(extenderCATID, extendeeObject);
            return null;
        }

        #endregion

        #region ExtenderSupport
        [Browsable(false)]
        [AutomationBrowsable(false)]
        public virtual string ExtenderCATID
        {
            get
            {
                Guid catid = this.ProjectMgr.ProjectMgr.GetCATIDForType(this.GetType());
                if (Guid.Empty.CompareTo(catid) == 0)
                    throw new NotImplementedException();
                return catid.ToString("B");
            }
        }
        [Browsable(false)]
        [AutomationBrowsable(false)]
        public object ExtenderNames
        {
            get
            {
                EnvDTE.ObjectExtenders extenderService = (EnvDTE.ObjectExtenders)this.ProjectMgr.GetService(typeof(EnvDTE.ObjectExtenders));
                return extenderService.GetExtenderNames(this.ExtenderCATID, this);
            }
        }
        public object get_Extender(string extenderName)
        {
            EnvDTE.ObjectExtenders extenderService = (EnvDTE.ObjectExtenders)this.ProjectMgr.GetService(typeof(EnvDTE.ObjectExtenders));
            return extenderService.GetExtender(this.ExtenderCATID, extenderName, this);
        }
        #endregion

        #region helper methods
        private void ValidateProperties()
        {
            ValidateRootnamespace();
        }

        private void ValidateRootnamespace()
        {
            String invalidChars = @"([/?:&\\*<>|#%!" + '\"' + "])";
            Regex invalidCharactersRegex = new Regex(invalidChars, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            if (invalidCharactersRegex.IsMatch(this.defaultNamespace))
            {
                throw new ArgumentException("Default Namespace:\nThe string for the default namespace must be a valid identifier");
            }

        }
        #endregion

        static class InternalSR
        {
            internal const string ProjectReferenceError = "ProjectReferenceError";
            internal const string ProjectReferenceError2 = "ProjectReferenceError2";
            internal const string Application = "Application";
            internal const string ApplicationIcon = "ApplicationIcon";
            internal const string ApplicationIconDescription = "ApplicationIconDescription";
            internal const string AssemblyName = "AssemblyName";
            internal const string AssemblyNameDescription = "AssemblyNameDescription";
            internal const string DefaultNamespace = "DefaultNamespace";
            internal const string DefaultNamespaceDescription = "DefaultNamespaceDescription";
            internal const string GeneralCaption = "GeneralCaption";
            internal const string OutputFile = "OutputFile";
            internal const string OutputFileDescription = "OutputFileDescription";
            internal const string OutputType = "OutputType";
            internal const string OutputTypeDescription = "OutputTypeDescription";
            internal const string Project = "Project";
            internal const string ProjectFile = "ProjectFile";
            internal const string ProjectFileDescription = "ProjectFileDescription";
            internal const string ProjectFileExtensionFilter = "ProjectFileExtensionFilter";
            internal const string ProjectFolder = "ProjectFolder";
            internal const string ProjectFolderDescription = "ProjectFolderDescription";
            internal const string StartupObject = "StartupObject";
            internal const string StartupObjectDescription = "StartupObjectDescription";
            internal const string TargetPlatform = "TargetPlatform";
            internal const string TargetPlatformDescription = "TargetPlatformDescription";
            internal const string TargetPlatformLocation = "TargetPlatformLocation";
            internal const string TargetPlatformLocationDescription = "TargetPlatformLocationDescription";
        }
   }
}