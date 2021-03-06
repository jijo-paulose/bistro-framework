﻿// VsPkg.cs : Implementation of Bistro_Designer
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
using Microsoft.VisualStudio.Shell.Flavor;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;


namespace Bistro.Designer
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
#if VS2008  
    [InstalledProductRegistration(false, "#110", "#112", "0.9", IconResourceID = 400)]
#endif
#if VS2010  
    [InstalledProductRegistration("#110", "#112", "0.9", IconResourceID = 400)]
#endif
    // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
    // package needs to have a valid load key (it can be requested at 
    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
    // package has a load key embedded in its resources.
    [ProvideLoadKey("Standard", "0.9", "Bistro Designer", "Hill30 Inc", 100)]

    [ProvideProjectFactory(
        typeof(Projects.FSharp.Factory), 
        null,
        null, 
        null,                                           // default project file extension
        null,                                           // possible project file extensions
        @".\NullPath",                                  // template directory .\\NullPath indicates that
                                                        // the template directory will not be used, instead
                                                        // the zipped templates will be scanned for vstemplate
                                                        // files with project type equal to template registartion key 
        LanguageVsTemplate = "Bistro")]                 // the value of the template registration key. This value 
                                                        // will also be used as the name of the node grouping
                                                        // projects in the AddNewProject dialog
    [WAProvideProjectFactory(typeof(Projects.FSharp.DummyWebFactory), "Web Bistro Factory", "Bistro", false, "Web", null)]
    [WAProvideProjectFactoryTemplateMapping("{" + "f2a71f9b-5d33-465a-a702-920d77279786" + "}", typeof(Projects.FSharp.DummyWebFactory))]

    [Guid(Guids.guidBistro_DesignerPkgString)]
    public sealed class DesignerPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public DesignerPackage()
        { }

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
            RegisterProjectFactory(new Projects.FSharp.Factory(this));
        }
        #endregion


    }
}