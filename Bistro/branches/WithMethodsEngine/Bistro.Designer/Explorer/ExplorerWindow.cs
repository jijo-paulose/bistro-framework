using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.Build.BuildEngine;

using Bistro;
using Bistro.Configuration;
using Bistro.Configuration.Logging;
using Bistro.Controllers;
using Bistro.MethodsEngine;
using Bistro.MethodsEngine.Reflection;
using Bistro.MethodsEngine.Subsets;
using Bistro.Designer.Projects;

namespace Bistro.Designer.Explorer
{
    //key=ctrlName,value={key=attribute,value=parameters' values}
    using ControllersTable = Dictionary<string, Dictionary<string, List<string>>>;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsWindowPane interface.
    /// </summary>
    [Guid("f9395d80-5ace-41c2-8c02-4be3582a6750")]
    public class ExplorerWindow : ToolWindowPane
    {
        // This is the user control hosted by the tool window; it is exposed to the base class 
        // using the Window property. Note that, even if this class implements IDispose, we are
        // not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
        // the object returned by the Window property.
        /*private EnvDTE.DTE dte;
        private EnvDTE.Events _events;
        private EnvDTE.WindowEvents _windowsEvents;
        private EnvDTE.DocumentEvents _docEvents;
        private EnvDTE.SolutionEvents _slnEvents;*/
        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public ExplorerWindow() :
            base(null)
        {
            this.Caption = Resources.ToolWindowTitle;
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;
            control = new DesignerControl();
            
        }
        public void Notify(object sender, UpdateEventArgs e)
        {

            TreeNode[] nodesToUpdate;
            if (e.ParentName != null)
            {
                //update subnode
                if (e.OldValue != null)
                {
                    nodesToUpdate = control.BindingTree.Nodes.Find(e.OldValue, true);
                    nodesToUpdate[0].Tag = e.NewValue.tag;
                    nodesToUpdate[0].Text = e.NewValue.text;
                }
                //add new subnode
                else
                {
                    nodesToUpdate = control.BindingTree.Nodes.Find(e.ParentName, true);
                    TreeNode newNode = new TreeNode();
                    newNode.Text = e.NewValue.text;
                    newNode.Tag = e.NewValue.tag;
                    nodesToUpdate[0].Nodes.Add(newNode);
                }

            }
            else
            {
                if (null == e.OldValue)
                {
                    //add node to the highest level of hierarchy (Project)
                    control.BindingTree.Nodes.Add(new TreeNode(e.NewValue.text));
                }
                else
                {
                    //rename node of the highest level of hierarchy (Project)
                    nodesToUpdate = control.BindingTree.Nodes.Find(e.ParentName, false);
                    nodesToUpdate[0].Text = e.NewValue.text;
                }

            }
        }
        public void InsertBranch(string project,TreeNode projectBranch)
        {
            control.BindingTree.Nodes.Find(project, false)[0].Nodes.Add(projectBranch);
        }
        public DesignerControl Control
        {
            get { return control; }
        }
        /// <summary>
        /// This property returns the handle to the user control that should
        /// be hosted in the Tool Window.
        /// </summary>
        override public IWin32Window Window
        {
            get { return (IWin32Window)control; }
        }
        private DesignerControl control;

      
    }
}
