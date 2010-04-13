// VsPkg.cs : Implementation of ProjectExtender
//

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using System.Xml;
using System.Collections.Generic;
using System.IO;

namespace FSharp.ProjectExtender
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the registration utility (regpkg.exe) that this class needs
    // to be registered as package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // A Visual Studio component can be registered under different regitry roots; for instance
    // when you debug your package you want to register it in the experimental hive. This
    // attribute specifies the registry root to use if no one is provided to regpkg.exe with
    // the /root switch.
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource(1000, 1)]
    // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
    // package needs to have a valid load key (it can be requested at 
    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
    // package has a load key embedded in its resources.
    //[ProvideLoadKey("Standard", "1.0", "F# Project System Extender", "Hill30 Inc", 1)]
    [ProvideProjectFactory(typeof(FSharp.ProjectExtender.Factory), "ProjectExtender", null, "fsproj", null,
        @".\NullPath", LanguageVsTemplate = "FSharp")]
    [ProvideObject(typeof(FSharp.ProjectExtender.Page),RegisterUsing=RegistrationMethod.CodeBase)]
    [Guid(Constants.guidProjectExtenderPkgString)]
    public sealed class ProjectExtenderPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public ProjectExtenderPackage()
        {
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();
            RegisterProjectFactory(new FSharp.ProjectExtender.Factory(this));

            ErrorHandler.ThrowOnFailure(((IVsMonitorSelection2)Package.GetGlobalService(typeof(IVsMonitorSelection))).GetEmptySelectionContext(out selectionTracker));
            solution = (IVsSolution)GetService(typeof(SVsSolution));

            // Create the command for the project extender
            CommandID extenderCommandID = new CommandID(Constants.guidProjectExtenderCmdSet, (int)Constants.cmdidProjectExtender);
            OleMenuCommand projectExtenderCommand = new OleMenuCommand(ProjectExtenderCommand, extenderCommandID);
            projectExtenderCommand.BeforeQueryStatus += new EventHandler(projectExtenderCommand_BeforeQueryStatus);
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            mcs.AddCommand(projectExtenderCommand);
        }


        void projectExtenderCommand_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Enabled = !(get_current_project() is IProjectManager);
        }

        private static IVsProject get_current_project()
        {
            IntPtr ppHier = IntPtr.Zero;
            uint pitemid;
            IVsMultiItemSelect ppMIS;
            IntPtr ppSC;
            try
            {
                ErrorHandler.ThrowOnFailure(selectionTracker.GetCurrentSelection(out ppHier, out pitemid, out ppMIS, out ppSC));
                return (IVsProject)Marshal.GetObjectForIUnknown(ppHier);
            }
            finally
            {
                Marshal.Release(ppHier);
            }
        }

        static IVsTrackSelectionEx selectionTracker;
        IVsSolution solution;

        #endregion

        private void ProjectExtenderCommand(object sender, EventArgs e)
        {
            ModifyProject(get_current_project(), enable_extender);
        }

        private void ModifyProject(IVsProject vsProject, Action<XmlDocument> effector)
        {
            var project = ProjectManager.getFSharpProjectNode(vsProject);
            var MSBuildProject = project.BuildProject;
            var minfo = typeof(Microsoft.Build.BuildEngine.Project).GetMethod("get_XmlDocument", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            var projFile = project.GetMkDocument();
            effector((XmlDocument)minfo.Invoke(MSBuildProject, new object[] { }));
            project.SetProjectFileDirty(true);
            
            ErrorHandler.ThrowOnFailure(solution.CloseSolutionElement(0/*(uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject*/, project, 0));

            var rguidProjectType = Guid.Empty;
            var iidProject = Guid.Empty;
            IntPtr ppProject;
            IVsProjectFactory ppProjectFactory;
            solution.GetProjectFactory(0, null, projFile, out ppProjectFactory);

            IVsUIShell shell = GetService(typeof(SVsUIShell)) as IVsUIShell;
            IVsWindowFrame frame = null;
            Guid persistenceSlot = new Guid(EnvDTE.Constants.vsWindowKindSolutionExplorer);
            ErrorHandler.ThrowOnFailure(shell.FindToolWindow(0, ref persistenceSlot, out frame));
            
            Guid guidProj = typeof(IVsProject).GUID;

            ErrorHandler.ThrowOnFailure(solution.CreateProject(ref rguidProjectType, projFile, null, null,
                (uint)(__VSCREATEPROJFLAGS.CPF_OPENFILE) + 2048 + 4096,
                ref iidProject, out ppProject));
        }

        private const string msBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
        private static XmlNamespaceManager namespace_manager = NamespaceManager();

        private static XmlNamespaceManager NamespaceManager()
        {
            var result = new XmlNamespaceManager(new NameTable());
            result.AddNamespace("default", msBuildNamespace);
            return result;
        }

        private static void enable_extender(XmlDocument project)
        {
            var projectTypeGuids = project.SelectSingleNode("//default:Project/default:PropertyGroup/default:ProjectTypeGuids", namespace_manager);
            if (projectTypeGuids == null)
            {
                projectTypeGuids = project.CreateElement("ProjectTypeGuids", msBuildNamespace);
                var projectGuid = project.SelectSingleNode("//default:Project/default:PropertyGroup/default:ProjectGuid", namespace_manager);
                projectGuid.ParentNode.InsertAfter(projectTypeGuids, projectGuid);
                projectTypeGuids.InnerText = "{" + Constants.guidProjectExtenderFactoryString + "};{" + Constants.guidFSharpProject + "}";
            }
            else
            {
                var types = new List<string>(projectTypeGuids.InnerText.Split(';'));
                if (types[types.Count - 1] != '{' + Constants.guidFSharpProject + '}')
                {
                    types.Insert(0, '{' + Constants.guidProjectExtenderFactoryString + '}');

                    var typestring = "";
                    types.ForEach(type => typestring += ';' + type);

                    projectTypeGuids.InnerText = typestring.Substring(1);
                }
            }
        }
    }
}