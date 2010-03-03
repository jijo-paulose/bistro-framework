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
using Microsoft.Build.BuildEngine;
using Bistro.Designer.Projects.FSharp;
using Bistro;
using Bistro.Configuration;
using Bistro.Configuration.Logging;
using Bistro.Controllers;
using Bistro.MethodsEngine;
using Bistro.MethodsEngine.Reflection;
using Bistro.MethodsEngine.Subsets;

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
            // Set the window title reading it from the resources.
            this.Caption = Resources.ToolWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
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
        public void Initialize(EnvDTE.DTE _dte, Factory _factory)
        {
            // Store the dte so that it can be used later.
            dte = _dte;
            factory = _factory;

        }
        public void AddEvents()
        {
            _events = (EnvDTE80.Events2)dte.Events;
            //_windowsEvents = (EnvDTE.WindowEvents)_events.get_WindowEvents(null);
            //_windowsEvents.WindowActivated += new EnvDTE._dispWindowEvents_WindowActivatedEventHandler(windowsEvents_WindowActivated);
            _docEvents = (EnvDTE.DocumentEvents)_events.get_DocumentEvents(null);
            //_docEvents.DocumentOpened += new EnvDTE._dispDocumentEvents_DocumentOpenedEventHandler(InitParser);
            _docEvents.DocumentSaved += new EnvDTE._dispDocumentEvents_DocumentSavedEventHandler(ReloadTreeView);
            _slnEvents = (EnvDTE.SolutionEvents)_events.SolutionEvents;
            _slnEvents.Opened += new EnvDTE._dispSolutionEvents_OpenedEventHandler(InitParser);
            _slnEvents.AfterClosing += new EnvDTE._dispSolutionEvents_AfterClosingEventHandler(_slnEvents_AfterClosing);
        }
   
        #region Private Members
        private Factory factory;
        private DesignerControl control;
        private MetadataExtractor extractor;
        private bool initialized;
 
        /// <summary>
        /// as there were changes in one or more files we need to reload bindingTreeView of control
        /// </summary>
        private void ReloadTreeView(EnvDTE.Document target)
        {
            extractor.FileName = target.FullName;
            if (extractor.FillControllerInfo())
            {
                ///TODO:
                ///how can we keep controllers' info got from dlls(if there were any)?
                
                ///keep this line commented until engine.Clean will be implemented
                UpdateTreeData();//reevaluate dependencies
                
                LoadTree();
            }
        }
        private void LoadTree()
        {
            Control.BindingTree.Nodes.Clear();

            Control.cashPatternsRes.Clear();
            Control.cashPatternsCtrl.Clear();
            Dictionary<string, List<ControllerDescription>> ctrlsStore = Control.cashPatternsCtrl;//temp obj at stack to decrease property's calls
            Dictionary<string, Dictionary<string, Resource>> resStore = Control.cashPatternsRes;//temp obj at stack to decrease property's calls
            foreach (BistroMethod bm in control.Engine.Processor.AllMethods)
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
        private void UpdateTreeData()
        {

            Control.BindingTree.Nodes.Clear();
            if (extractor.infobyFiles.Count == 0)
            {
                Control.BindingTree.Nodes.Add("Failed to parse source code");
                return;
            }
            ///TODO : clear controllers been added from source code 
            control.Engine.Clean();//not implemented yet
            foreach (KeyValuePair<string, ControllersTable> fileData in extractor.infobyFiles)
            {
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> ctrlsData in fileData.Value)
                {
                    ControllerDescription ctrldesc = new ControllerDescription(ctrlsData.Key, ctrlsData.Value);
                    control.Engine.RegisterController(ctrldesc);
                }
            }
            control.Engine.ForceUpdateBindPoints();


        }
         /// <summary>
        /// fills dictionary with info about controllers from all F# files of the project
        /// must be called after ProjectManager was instantiated by Factory(OnSolutionOpened)
        /// it may be called only once because when you add new item to the project,info will be added OnSave 
        /// </summary>
        private void InitParser()
        {
            if (initialized) return;
            SectionHandler sh = new SectionHandler();
            sh.Application = "Bistro.Application";
            sh.LoggerFactory = "Bistro.Logging.DefaultLoggerFactory";
            Bistro.Application.Initialize(sh);
            control.Engine = new Bistro.MethodsEngine.Engine(Bistro.Application.Instance);
            List<Assembly> refDlls = new List<Assembly>();
            string ext = ".fs";//default while CSharp Bistro project is not developed
            string lang = "f#";
            extractor = new MetadataExtractor(lang, String.Empty);
            try
            {
                string path = factory.projectMngr.MSBuildProject.FullFileName;
                int len = path.LastIndexOf("\\");
                path = path.Substring(0, len + 1);
                // Iterate through each ItemGroup in the Project to obtain the list of F# source files
                foreach (BuildItemGroup ig in factory.projectMngr.MSBuildProject.ItemGroups)
                {
                    foreach (BuildItem item in ig)
                    {
                        if (String.Compare(item.Name, "Compile") == 0)
                        {
                            if (item.Include.EndsWith(ext))
                            {
                                extractor.FileName = path + item.Include;
                                extractor.FillControllerInfo();
                            }

                        }
                        /*else if (String.Compare(item.Name, "Reference") == 0)
                        {
                            foreach (string meta in item.MetadataNames)
                            {
                                Debug.WriteLine(item.Include + " " + meta + " " + item.GetMetadata(meta));
                                if (String.Compare(meta,"HintPath") == 0)
                                {
                                    string refPath = item.GetMetadata(meta);
                                    if (!refPath.Contains("System") && !refPath.Contains("Microsoft"))
                                    {
                                        try
                                        {
                                            //Note:absolute path is required
                                            string[] info = refPath.Split(',');//<fileName>,<version>
                                            refDlls.Add(Assembly.LoadFile(info[0]));
                                        }
                                        catch (Exception ex)
                                        {
                                            //if reference is broken,just skip 
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }*/
                        else
                            break;
                    }
                }
                //MetadataFromDll.LoadAssemblies(refDlls, Control.Engine);
                UpdateTreeData();
                LoadTree();


            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        } 
        private void _slnEvents_AfterClosing()
        {
            control.Engine = null;
        }

        /*private void windowsEvents_WindowActivated(EnvDTE.Window gotFocus, EnvDTE.Window lostfocus)
        {
            try
            {
                Debug.Assert(gotFocus != null);
                if (gotFocus == null)
                {
                    return;
                }

                if (gotFocus.Type.Equals(EnvDTE.vsWindowType.vsWindowTypeDocument) && gotFocus.Caption.EndsWith(".fs"))
                {
                    Trace.WriteLine("document wnd with F# file is activated");
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }*/
        #endregion
    }
}
