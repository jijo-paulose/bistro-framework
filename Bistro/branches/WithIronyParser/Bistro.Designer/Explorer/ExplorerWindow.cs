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
        private EnvDTE.DTE dte;
        private EnvDTE.Events _events;
        private EnvDTE.WindowEvents _windowsEvents;
        private EnvDTE.DocumentEvents _docEvents;
        private EnvDTE.SolutionEvents _slnEvents;
        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public ExplorerWindow() :
            base(null)
        {
            this.Caption = Resources.ToolWindowTitle;
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;
            control = new DesignerControl(this);
            projectMngrs = new Dictionary<string, IProjectManager>();
            
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
        public void Initialize(EnvDTE.DTE _dte)
        {
            // Store the dte so that it can be used later.
            dte = _dte;

        }
        public void AddEvents()
        {
            _events = (EnvDTE80.Events2)dte.Events;
            _events.SolutionItemsEvents.ItemAdded += new EnvDTE._dispProjectItemsEvents_ItemAddedEventHandler(_events_ItemAdded);
            _docEvents = (EnvDTE.DocumentEvents)_events.get_DocumentEvents(null);
            _docEvents.DocumentSaved += new EnvDTE._dispDocumentEvents_DocumentSavedEventHandler(_docEvents_DocumentSaved);
            _slnEvents = (EnvDTE.SolutionEvents)_events.SolutionEvents;
            _slnEvents.Opened += new EnvDTE._dispSolutionEvents_OpenedEventHandler(_slnEvents_SolutionOpened);
            _slnEvents.ProjectAdded += new EnvDTE._dispSolutionEvents_ProjectAddedEventHandler(_slnEvents_ProjectAdded);
            _slnEvents.ProjectRemoved += new EnvDTE._dispSolutionEvents_ProjectRemovedEventHandler(_slnEvents_ProjectRemoved);
            _slnEvents.BeforeClosing += new EnvDTE._dispSolutionEvents_BeforeClosingEventHandler(_slnEvents_BeforeClosing);
        }


   
        #region Private Members
        private DesignerControl control;
        private MetadataExtractor extractor;
        internal Dictionary<string,IProjectManager> projectMngrs;
        string activeProject;


        internal void ChangeActiveProject()
        {
            foreach (string key in projectMngrs.Keys)
            {
                if (key.EndsWith(control.ComboProjects.SelectedItem.ToString()))
                {
                    if (key == activeProject) return;
                    string path = projectMngrs[key].ProjectPath;
                    activeProject = path + control.ComboProjects.SelectedItem.ToString();
                    ReloadTreeView("forced");
                }
            }

            
        }
        private void GetStartupProject()
        {
            
            string msg = "";
            foreach (String item in (Array)dte.Solution.SolutionBuild.StartupProjects)
            {
                msg += item;
            }
            EnvDTE.Project startupProj = dte.Solution.Item(msg);
            activeProject = startupProj.FullName;
           
        }

        /// <summary>
        /// As there were changes in source code we need to reload bindingTreeView of the control
        /// </summary>
        /// <param name="param">either a fullname of updated file or flag for forced update("forced")</param> 
        private void ReloadTreeView(string param)
        {
            if (param != "forced") extractor.FileName = param;
            if (param == "forced" ||
                (projectMngrs[activeProject].GetSourceFiles().Contains(param) && extractor.FillControllerInfo()))
                if (UpdateTreeData())
                    LoadTree();
        }
        private void LoadTree()
        {
            Control.BindingTree.Nodes.Clear();

            Control.cashPatternsRes.Clear();
            Control.cashPatternsCtrl.Clear();
            Dictionary<string, List<ControllerDescription>> ctrlsStore = Control.cashPatternsCtrl;
            Dictionary<string, Dictionary<string, Resource>> resStore = Control.cashPatternsRes;
            foreach (BistroMethod bm in projectMngrs[activeProject].Engine.Processor.AllMethods)
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
            }
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
            if (!projectMngrs.ContainsKey(activeProject) || extractor.infobyFiles.Count == 0 ) return false;
            Control.BindingTree.Nodes.Clear();
            projectMngrs[activeProject].Engine.Clean();//not implemented yet
            List<string> files = projectMngrs[activeProject].GetSourceFiles();
            foreach (string file in files)
            {
                ControllersTable fileData = extractor.infobyFiles[file];
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> ctrlsData in fileData)
                {
                    ControllerDescription ctrldesc = new ControllerDescription(ctrlsData.Key, ctrlsData.Value);
                    projectMngrs[activeProject].Engine.RegisterController(ctrldesc);
                }
            }
            projectMngrs[activeProject].Engine.ForceUpdateBindPoints();
            return true;


        }
        private void _slnEvents_SolutionOpened()
        {
            try
            {
                GetStartupProject();
                string lang = (projectMngrs[activeProject].GetType() == typeof(Projects.CSharp.ProjectManager)) ? "c#" : "f#";
                extractor = new MetadataExtractor(lang, String.Empty);
                foreach (KeyValuePair<string,IProjectManager> kvp in projectMngrs)
                {
                    List<string> files = kvp.Value.GetSourceFiles();
                    string str = kvp.Key.Substring(kvp.Value.ProjectPath.Length);
                    control.ComboProjects.Items.Add(str);
                    if (String.Compare(activeProject, kvp.Key) == 0)
                        control.ComboProjects.SelectedItem = str;
                    foreach (string file in files)
                    {
                        extractor.FileName = file;
                        extractor.FillControllerInfo();
                    }
                }
                UpdateTreeData();
                LoadTree();

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        } 
        private void _slnEvents_BeforeClosing()
        {
            projectMngrs.Clear();
            extractor = null;
        }
        private void _events_ItemAdded(EnvDTE.ProjectItem item)
        {
            ReloadTreeView(item.Document.FullName);
        }
        private void _docEvents_DocumentSaved(EnvDTE.Document target)
        {
            ReloadTreeView(target.FullName);
        }
        /// <summary>
        ///In general,there can be different projects in one solution (C#,F#).As it is impossible to parse both languages simultaneously :
        ///1) we can store information only for the last added project
        ///2) add OnSave information if project's language is the same - see ReloadTreeView.
        ///If project's language is different from 

        /// </summary>
        /// <param name="project"></param>
        private void _slnEvents_ProjectAdded(EnvDTE.Project project)
        {
            string key = project.FullName;
            List<string> files = projectMngrs[key].GetSourceFiles();
            string str = key.Substring(projectMngrs[key].ProjectPath.Length);
            control.ComboProjects.Items.Add(str);
            control.ComboProjects.SelectedItem = str;
            foreach (string file in files)
            {
                extractor.FileName = file;
                extractor.FillControllerInfo();
            }
            activeProject = project.FullName;
            ReloadTreeView("forced");


        }
        private void _slnEvents_ProjectRemoved(EnvDTE.Project project)
        {
            if (!projectMngrs.ContainsKey(project.FullName)) return;
            List<string> toDelete = projectMngrs[project.FullName].GetSourceFiles();
            control.ComboProjects.Items.Remove(project.FullName.Substring(projectMngrs[project.FullName].ProjectPath.Length));
            foreach (string file in toDelete)
            {
                extractor.infobyFiles.Remove(file);
            }
            projectMngrs.Remove(project.FullName);

        }

        #endregion
    }
}
