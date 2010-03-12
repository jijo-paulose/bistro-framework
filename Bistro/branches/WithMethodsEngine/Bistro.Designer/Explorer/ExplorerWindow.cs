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
        #region Private Members
        private DesignerControl control;
        /// <summary>
        /// As there were changes in source code we need to reload bindingTreeView of the control
        /// </summary>
        /// <param name="param">either a fullname of updated file or flag for forced update("forced")</param> 
        private void ReloadTreeView(string param)
        {
        }
        private void LoadTree()
        {
            Control.BindingTree.Nodes.Clear();

            Control.cashPatternsRes.Clear();
            Control.cashPatternsCtrl.Clear();
            Dictionary<string, List<ControllerDescription>> ctrlsStore = Control.cashPatternsCtrl;
            Dictionary<string, Dictionary<string, Resource>> resStore = Control.cashPatternsRes;
            /*foreach (BistroMethod bm in projectMngrs[activeProject].Engine.Processor.AllMethods)
            {
                foreach (IMethodsBindPointDesc bp in bm.BindPointsList)
                {
                    //add controller that process urls matching this target
                    if (ctrlsStore.ContainsKey(bp.Target))
                    {
                        if (!ctrlsStore[bp.Target].Contains((ControllerDescription)(bp.Controller)))
                            ctrlsStore[bp.Target].Add((ControllerDescription)bp.Controller);
                    }
                    else
                    {
                        ctrlsStore.Add(bp.Target, new List<ControllerDescription>());
                        ctrlsStore[bp.Target].Add((ControllerDescription)bp.Controller);
                        resStore.Add(bp.Target, new Dictionary<string, Resource>());
                    }
                    //add resource required by controllers to process urls matching the target
                    foreach (KeyValuePair<string, Resource> res in bm.Resources)
                    {
                        //check whether this resource is required by bindPoint
                        if (((List<IMethodsBindPointDesc>)res.Value.RequiredBy).Contains(bp)
                            && !resStore[bp.Target].ContainsValue(res.Value))
                        {
                            if (!resStore[bp.Target].ContainsKey(res.Key))
                                resStore[bp.Target].Add(res.Key, res.Value);
                        }
                        //check whether bindPoint depends on this resource 
                        if (((List<IMethodsBindPointDesc>)res.Value.Dependents).Contains(bp)
                            && !resStore[bp.Target].ContainsValue(res.Value))
                        {
                            if (!resStore[bp.Target].ContainsKey(res.Key))
                                resStore[bp.Target].Add(res.Key, res.Value);
                        }
                    }
                }
            }*/
            #region Fill TreeViewControl
            foreach (KeyValuePair<string, List<ControllerDescription>> kvp in ctrlsStore)
            {
                int nSubNodes = kvp.Value.Count;
                nSubNodes = (resStore.ContainsKey(kvp.Key)) ? nSubNodes + resStore[kvp.Key].Count : nSubNodes;
                TreeNode[] subNodes = new TreeNode[nSubNodes];
                int i = 0;
                for (; i < kvp.Value.Count; i++)
                {
                    subNodes[i] = new TreeNode();
                    subNodes[i].Tag = kvp.Value[i];
                    subNodes[i].Text = kvp.Value[i].ControllerTypeName;
                    subNodes[i].ImageIndex = 0;
                    subNodes[i].SelectedImageIndex = 0;
                    //subNodes[i].ContextMenuStrip = Control.ControllerMenu;
                }
                if (resStore.ContainsKey(kvp.Key))
                {
                    int j = 0;
                    foreach (KeyValuePair<string, Resource> res in resStore[kvp.Key])
                    {
                        subNodes[i + j] = new TreeNode();
                        subNodes[i + j].Tag = res.Value;
                        subNodes[i + j].Text = res.Key;
                        subNodes[i + j].ImageIndex = 5;
                        subNodes[i + j].SelectedImageIndex = 5;
                        j++;
                    }
                }
                TreeNode patternNode = new TreeNode(kvp.Key, subNodes);
                patternNode.ImageIndex = 4;
                patternNode.SelectedImageIndex = 4;
                //patternNode.ContextMenuStrip = Control.MethodMenu;
                Control.BindingTree.Nodes.Add(patternNode);

            }
            #endregion

        }
        /// <summary>
        /// Rebuilds all controller dependencies and bindings
        /// Note : engine.Clean is not implemented yet that's why it is impossible to update tree correctly now
        /// </summary>
        private bool UpdateTreeData()
        {
            return true;
        }
        private IVsSolution sln;
        /*private EnvDTE.DTE dte;
        private EnvDTE.Events _events;
        private EnvDTE.WindowEvents _windowsEvents;
        private EnvDTE.DocumentEvents _docEvents;
        private EnvDTE.SolutionEvents _slnEvents;*/
        #endregion

      
    }
}
