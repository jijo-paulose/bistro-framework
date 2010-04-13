﻿// VsPkg.cs : Implementation of ProjectExtender
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
    // Provide the F# project extender project project factory. This is a flavored project and it does not
    // introduce any new templates
    [ProvideProjectFactory(typeof(FSharp.ProjectExtender.Factory), "ProjectExtender", null, null, null, null)]
    // Provide object so it can be created through the ILocalRegistry interface - in this case the new property page
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
        public ProjectExtenderPackage() { }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
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

        static IVsTrackSelectionEx selectionTracker;
        IVsSolution solution;

        #endregion

        string enable_extender_text = "Enable F# project extender";
        string disable_extender_text = "Disable F# project extender";
        
        /// <summary>
        /// Modifies caption on the project extender command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void projectExtenderCommand_BeforeQueryStatus(object sender, EventArgs e)
        {
            if (get_current_project() is IProjectManager)
                ((OleMenuCommand)sender).Text = disable_extender_text;
            else
                ((OleMenuCommand)sender).Text = enable_extender_text;
        }

        /// <summary>
        /// retrieves the IVsProject interface for currentll selected project
        /// </summary>
        /// <returns></returns>
        private static IVsProject get_current_project()
        {
            IntPtr ppHier = IntPtr.Zero;
            uint pitemid;
            IVsMultiItemSelect ppMIS;
            IntPtr ppSC;

            ErrorHandler.ThrowOnFailure(selectionTracker.GetCurrentSelection(out ppHier, out pitemid, out ppMIS, out ppSC));
            var result = (IVsProject)Marshal.GetObjectForIUnknown(ppHier);
            Marshal.Release(ppHier);
            if (!IntPtr.Zero.Equals(ppSC))
                Marshal.Release(ppSC);
            return result;
        }

        /// <summary>
        /// Project Extender command handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectExtenderCommand(object sender, EventArgs e)
        {
            var project = get_current_project();
            if (project is IProjectManager)
                ModifyProject(project, disable_extender);
            else
                ModifyProject(project, enable_extender);
        }

        /// <summary>
        /// Modifies the loaded project by changing the project's proj file
        /// </summary>
        /// <param name="vsProject">project to be modified</param>
        /// <param name="effector"></param>
        private void ModifyProject(IVsProject vsProject, Action<XmlDocument> effector)
        {
            var project = ProjectManager.getFSharpProjectNode(vsProject);
            var MSBuildProject = project.BuildProject;

            // method get_XmlDocument on the MSBuild project is internal
            // We will have to use reflection to call it
            var minfo = typeof(Microsoft.Build.BuildEngine.Project)
                .GetMethod("get_XmlDocument", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            // apply modifications to XML
            effector((XmlDocument)minfo.Invoke(MSBuildProject, new object[] { }));
            
            // Set dirty flag to true to force project save
            project.SetProjectFileDirty(true);
            
            // Unload the project - also saves the modifications
            ErrorHandler.ThrowOnFailure(solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, project, 0));

            // Reload the project
            ((EnvDTE.DTE)GetService(typeof(SDTE))).ExecuteCommand("Project.ReloadProject", "");
        }

        private const string msBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
        private static XmlNamespaceManager namespace_manager = NamespaceManager();

        private static XmlNamespaceManager NamespaceManager()
        {
            var result = new XmlNamespaceManager(new NameTable());
            result.AddNamespace("default", msBuildNamespace);
            return result;
        }

        /// <summary>
        /// Modifies the XML to enable the extender
        /// </summary>
        /// <param name="project"></param>
        private static void enable_extender(XmlDocument project)
        {
            // Locate the ProjectTypeGuids node
            var projectTypeGuids = project.SelectSingleNode("//default:Project/default:PropertyGroup/default:ProjectTypeGuids", namespace_manager);
            if (projectTypeGuids == null)
            {
                // Not found - create a new one
                projectTypeGuids = project.CreateElement("ProjectTypeGuids", msBuildNamespace);
                var projectGuid = project.SelectSingleNode("//default:Project/default:PropertyGroup/default:ProjectGuid", namespace_manager);
                // insert it after the ProjectGuid node
                projectGuid.ParentNode.InsertAfter(projectTypeGuids, projectGuid);
                // initialize the project type guid list
                projectTypeGuids.InnerText = "{" + Constants.guidProjectExtenderFactoryString + "};{" + Constants.guidFSharpProject + "}";
            }
            else
            {
                // parse the existing guid list
                var types = new List<string>(projectTypeGuids.InnerText.Split(';'));
                
                // prepend the guid list with the extender project type 
                types.Insert(0, '{' + Constants.guidProjectExtenderFactoryString + '}');

                // format the guid list
                var typestring = "";
                types.ForEach(type => typestring += ';' + type);
                // replace the guid list
                projectTypeGuids.InnerText = typestring.Substring(1);
            }
        }

        /// <summary>
        /// Modifies XML to disable extender
        /// </summary>
        /// <param name="project"></param>
        private static void disable_extender(XmlDocument project)
        {
            // locate the ProjectTypeGuids node
            var projectTypeGuids = project.SelectSingleNode("//default:Project/default:PropertyGroup/default:ProjectTypeGuids", namespace_manager);
            // remove the extender guid from the list
            if (projectTypeGuids != null)
                projectTypeGuids.InnerText =
                    projectTypeGuids.InnerText.Replace('{' + Constants.guidProjectExtenderFactoryString + "};", "");
        }
    }
}