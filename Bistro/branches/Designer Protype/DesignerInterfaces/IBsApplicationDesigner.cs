using System;
using System.Collections.Generic;
using Bistro.Designer.Explorer;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using Bistro.Designer.Explorer;

namespace Bistro.Designer
{
    /// <summary>
    /// Public interface for the Application Proxy object defined in the ProjectService dll.
    /// </summary>
    /// <remarks>
    /// This is the 'External' interface for the Application Proxy. Methods of this interface are supposed to be called 
    /// from the main Visual Studio domain (i.e. property editors, designers, etc.)
    /// </remarks>
    public interface IBsApplicationDesigner : IServiceContainer, IDisposable
    {
        /// <summary>
        /// Gets the explorer node representing the application in the explorer.
        /// </summary>
        /// <value>The application node.</value>
        ExplorerNode ExplorerRoot { get;}

        IVsHierarchy Hierarchy { get;}

        /// <summary>
        /// Gets a value indicating whether the load process for the application has been completed.
        /// </summary>
        /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
        bool IsLoaded { get;}
        
        /// <summary>
        /// Gets a value indicating whether application is a file based project or a web site.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is a web site; otherwise, <c>false</c>.
        /// </value>
        bool IsWebSite { get; }
        
        /// <summary>
        /// Gets the application name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the application root directory.
        /// </summary>
        /// <value>The root directory.</value>
        string RootDirectory { get;}

        /// <summary>
        /// Notifies the applcation that certain files have been modified.
        /// </summary>
        /// <param name="filenames">List of modified files.</param>
        void ApplyChanges(string[] filenames);

        /// <summary>
        /// Creates an instance of Application Extender 
        /// </summary>
        /// <param name="typeName">the name of the type to be instantiated</param>
        /// <remarks>Application extender is created in the Application definition domain
        /// The extender type should be inherited from <see cref="WorkflowServer.Designer.Addins.ApplicationExtender"/>
        /// </remarks>
        void CreateExtender(string typeName);

        /// <summary>
        /// Generates the document classes for the application.
        /// </summary>
        /// <param name="force">if set to <c>true</c> forces generation for all document classes. Otherwise only 
        /// modified documents and their dependents are affected</param>
        void Generate(bool force);

        /// <summary>
        /// Gets the absolute activity path.
        /// </summary>
        /// <param name="context">The full name of the activity used as the starting point for the relative name calculation.</param>
        /// <param name="destination">The relative path to be converted to the absolute.</param>
        /// <returns></returns>
        string GetAbsoluteActivityPath(string context, string destination);

        /// <summary>
        /// Gets a list of child workflow explorer nodes for a given node.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        IEnumerable<ExplorerNode> GetChildrenForNode(ExplorerNode parent);
        
        /// <summary>
        /// Gets the content of the file. 
        /// </summary>
        /// <param name="path">The location of the file</param>
        /// <returns>the TextReader which can be used to access the content of the file</returns>
        /// <remarks>If the file is opened in Visual Studio, the content returned is taken from the buffer, not from the file</remarks>
        TextReader GetFileContent(string path);

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">additional arguments.</param>
        void LogMessage(string message, params string[] args);

        /// <summary>
        /// Reloads the application.
        /// </summary>
        void Reload();

        /// <summary>
        /// Opens a window with the source code generated for the document
        /// </summary>
        /// <param name="filename">path to the file with the document definition</param>
        void ShowGenerated(string filename);

        /// <summary>
        /// Opens a window with the source code for the object
        /// </summary>
        /// <param name="filename">path to the file with the source code</param>
        /// <param name="pos">offset from the beginning of the file to the first character of the object source code</param>
        /// <remarks>The cursor is positioned on the first character of the source code for the object</remarks>
        void ShowSource(string fileName, int pos);

        /// <summary>
        /// Shows the properties window.
        /// </summary>
        void ShowPropertiesWindow();

        /// <summary>
        /// Fires when a new document definition is added to the application
        /// </summary>
        event FactoryEventHandler OnFactoryAdded;

        /// <summary>
        /// Fires when a document definition is removed from the application
        /// </summary>
        event FactoryEventHandler OnFactoryRemoved;
    }

    /// <summary>
    /// Marker interface used to access services provided by the ApplicationProxy class. 
    /// </summary>
    public interface SBsApplicationDesigner
    {
    }

    public delegate void FactoryEventHandler(string factoryName);

}
