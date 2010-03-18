using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Bistro.Configuration;
using Bistro.MethodsEngine;
using Bistro.MethodsEngine.Reflection;
using Bistro.MethodsEngine.Subsets;
using Bistro.Controllers;
using Bistro.Interfaces;

namespace Bistro.Designer.Explorer
{
    using ControllersTable = Dictionary<string, Dictionary<string, List<string>>>;
    public delegate void UpdateTreeDelegate(object sender, UpdateEventArgs e);
    public struct Node
    {
        public string text;
        public object tag;
        public Node[] childNodes;
    }
    public class UpdateEventArgs : EventArgs
    {

        public UpdateEventArgs(string parent,Node newVal,string oldVal)
        {
            ParentName = parent;
            NewValue = newVal;
            OldValue = oldVal;
        }
        public string ParentName { get; set; }
        public string OldValue { get; set; }
        public Node NewValue { get; set; }

    }
   


    public class ChangesTracker : IVsTextManagerEvents,IVsTextLinesEvents
    {
        public ChangesTracker(string lang)
        {

            this.vsTextMgr =  Package.GetGlobalService(typeof(SVsTextManager)) as IVsTextManager2;
            SectionHandler sh = new SectionHandler();
            sh.Application = "Bistro.Application";
            sh.LoggerFactory = "Bistro.Logging.DefaultLoggerFactory";
            Bistro.Application.Initialize(sh);
            engine = new Bistro.MethodsEngine.EngineControllerDispatcher(Bistro.Application.Instance);
            parser = new Explorer.MetadataExtractor(lang, String.Empty);
            IConnectionPointContainer container = vsTextMgr as IConnectionPointContainer;
            IConnectionPoint textManagerEventsConnection = null;
            Guid tmEventGuid = typeof(IVsTextManagerEvents).GUID;
            container.FindConnectionPoint(ref tmEventGuid, out textManagerEventsConnection);
            uint cookie = 0;
            textManagerEventsConnection.Advise(this, out cookie);
        }
        public string ActiveFile { get; set; }
        //public event UpdateTreeDelegate NodesChanged;
        /// <summary>
        /// There will be always one observer for this subject - that's why no observer collection is needed
        /// </summary>
        /// <param name="observer"></param>
      
        public void RegisterObserver(ExplorerWindow observer)
        {
            explorer = observer;
        }
        public void UnRegisterObserver(ExplorerWindow observer)
        {
        }
        public void OnProjectRenamed(string projectName)
        {
            this.projectName = projectName;
            Node node;
            node.tag = null;
            node.text = projectName;
            node.childNodes = null;
            explorer.Notify(this, new UpdateEventArgs(null, node, null));

        }
        public void OnProjectOpened(List<string> srcfiles)
        {
            foreach (string file in srcfiles)
            {
                parser.FileName = file;
                parser.FillControllerInfo();
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp in parser.controllerInfo)
                {
                    engine.RegisterController(new Explorer.ControllerDescription(kvp.Key, kvp.Value));
                }

            }
            engine.ForceUpdateBindPoints();
            AnalyzeDependencies(true);

        }
        internal void RaiseNodesChanged(string parent,string newNode,string oldNode,bool bCreate)
        {
             // make sure the are some delegates in the invocation list 
            /*if (NodesChanged != null)
            {
                NodesChanged(this,new UpdateEventArgs(level,newNode,oldNode));
            }*/

        }
        /// <summary>
        /// TimerCallback
        /// </summary>
        /// <param name="state"></param>
        private void RebuildNodes(object state)
        {
            parser.FileName = ActiveFile;
            if (parser.FillControllerInfo())
            {
                engine.Clean();
                ControllersTable ctrlsData = parser.infobyFiles[ActiveFile];
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp in ctrlsData)
                {
                    engine.RegisterController(new Explorer.ControllerDescription(kvp.Key,kvp.Value));
                }
                engine.ForceUpdateBindPoints();
            }

            //RaiseNodesChanged();
        }
        private void AnalyzeDependencies(bool onOpen)
        {
            Dictionary<string, List<IControllerDescriptor>> ctrlsStore = new Dictionary<string, List<IControllerDescriptor>>();

            #region old version
            
            /*foreach (BistroMethod bm in engine.Processor.AllMethods)
            {
                foreach (IMethodsBindPointDesc bp in bm.BindPointsList)
                {

                    ctrlsStore.Add(bp.Target,new List<ControllerDescription>());
                    List<ControllerInvocationInfo> seq = engine.GetControllers(bp.Target);
                    foreach (ControllerInvocationInfo info in seq)
                    {
                        ctrlsStore[bp.Target].Add((ControllerDescription)info.BindPoint.Controller);
                    }
                    resStore.Add(bp.Target, new Dictionary<string, Resource>());
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
            #endregion
            if (onOpen)
            {
                foreach (string url in engine.Map.Keys)
                {
                    List<ControllerInvocationInfo> seq = engine.GetControllers(url);
                    foreach (ControllerInvocationInfo info in seq)
                    {
                        ctrlsStore[url].Add(info.BindPoint.Controller);
                    }
                    
                }
                
            }
            
            else
            {
                //necessary to find difference and update only nodes where smth has changed
            }

        }


        #region IVsTextManagerEvents Members


        void IVsTextManagerEvents.OnRegisterView(IVsTextView pView)
        {
            uint cookie = 0;
            pView.GetBuffer(out this.buffer);
            IConnectionPointContainer container = buffer as IConnectionPointContainer;
            container.FindConnectionPoint(ref tbEventGuid, out textBuferEventsConnection);
            textBuferEventsConnection.Advise(this, out cookie);
            //cookies.Add(pView, cookie);

        }

        void IVsTextManagerEvents.OnUnregisterView(IVsTextView pView)
        {
        }

        void IVsTextManagerEvents.OnRegisterMarkerType(int iMarkerType)
        {

        }
        void IVsTextManagerEvents.OnUserPreferencesChanged(VIEWPREFERENCES[] pViewPrefs, FRAMEPREFERENCES[] pFramePrefs, LANGPREFERENCES[] pLangPrefs, FONTCOLORPREFERENCES[] pColorPrefs)
        {
        }

        #endregion

        #region IVsTextLinesEvents Members

        void IVsTextLinesEvents.OnChangeLineAttributes(int iFirstLine, int iLastLine)
        {
        }

        void IVsTextLinesEvents.OnChangeLineText(TextLineChange[] pTextLineChange, int fLast)
        {
            if (parserTimer != null)
                parserTimer.Dispose();
            parserTimer = new Timer(RebuildNodes, null, PARSING_DELAY, Timeout.Infinite);
            //parserTimer = new Timer(RebuildNodes, null,0,PARSING_DELAY);

        }

        #endregion
        private ExplorerWindow explorer;
        private MetadataExtractor parser;
        private EngineControllerDispatcher engine;
        private IVsTextManager2 vsTextMgr;
        private IVsTextLines buffer;
        IConnectionPoint textBuferEventsConnection = null;
        Guid tbEventGuid = typeof(IVsTextLinesEvents).GUID;
        Guid monikerGuid = typeof(IVsUserData).GUID;
        string projectName;
        /// <summary>
        /// indicates the delay (in milliseconds) of parser invoking. 
        /// </summary>
        private const int PARSING_DELAY = 30000;
        /// <summary>
        /// The timer for optimization the parsing process. If there would be some changes with time 
        /// between sequential changes less then PARSING_DELAY, then rebuild process would be invoked only once.
        /// </summary>
        private Timer parserTimer;
        private Dictionary<IVsTextView, uint> cookies;

    }
}

