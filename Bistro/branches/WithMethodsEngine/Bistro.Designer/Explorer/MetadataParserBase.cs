using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Reflection;
using Irony.Parsing;
using Irony.Samples.CSharp;
using Irony.Samples.FSharp;
using Bistro.MethodsEngine;
using Bistro.MethodsEngine.Reflection;
using Bistro.Interfaces;
using Bistro.Controllers;
using Bistro.Controllers.Descriptor;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Bistro.Designer.Explorer
{
    
    using ControllersTable = Dictionary<string, Dictionary<string, List<string>>>;
    
    public class MetadataParserBase
    {

        /// <summary>
        ///info about controllers is stored here after parser finished parsing current file
        ///format key=ctrlName,value={key=attribute,value=parameters' values}
        /// </summary>
        public ControllersTable controllerInfo;
        /// <summary>
        /// format key = absolute src filename,value={controllerInfo(see above)}
        /// </summary>
        public Dictionary<string, ControllersTable> infobyFiles;
        public MetadataParserBase()
        {
            parseTree = null;
            controllerInfo = new ControllersTable();
            infobyFiles = new Dictionary<string, ControllersTable>();
        }
        public string FileName
        {
            set { srcFilename = value; }
            get { return srcFilename; }
        }
        /// <summary>
        /// Method is used to extract controllers' data from specified source file into ControllersTable 
        /// </summary>
        /// <returns>true if essential changes in controllers' declarations were detected,
        /// else - otherwise</returns>
        public bool FillControllerInfo()
        {
            TextReader rd = new StreamReader(srcFilename);
            controllerInfo.Clear();
            Extract(rd.ReadToEnd(), "<source>");
            if (parseTree == null) return false;
            AddParseNodeRec(parseTree.Root,false);
            //if source file has been recently added to the project,add new row into ControllersTable
            //else if there were any changes in source file,update corresponding row in ControllersTable
            if (!infobyFiles.ContainsKey(srcFilename))
            {
                ControllersTable curtbl = new ControllersTable(controllerInfo);
                infobyFiles.Add(srcFilename, curtbl);
                return true;
            }
            else
            {
                bool equal = new AttributesComparer().Equals(infobyFiles[srcFilename], controllerInfo);
                if (!equal)
                {
                    if (infobyFiles.Remove(srcFilename))
                    {
                        ControllersTable curtbl = new ControllersTable(controllerInfo);
                        infobyFiles.Add(srcFilename, curtbl);
                        return true;
                    }
                }
                return false;
            }
        }

        protected ParseTree parseTree;
        protected Parser parser;
        protected Grammar grammar;
        private string srcFilename;
        protected const string tail = " (identifier)";
        protected const string toCut = " (StringLiteral)";

        protected string curCtrl;//name of the controller that is being processed
        protected string curAttr;//name of the attribute that is being processed
        protected int bindTargs;//as Dictionary key must be unique,so we'll save it like Bind0..BindN for each controller 
        protected bool isBindAttr;//this flag shows whether curAttr is Bind 
        protected bool isRenderAttr;//this flag shows whether curAttr is RenderWith 
        private void Extract(string srcText, string fname)
        {
            try
            {
                parser.Parse(srcText, fname);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                parseTree = parser.Context.CurrentParseTree;
            }
        }
        protected virtual void AddParseNodeRec(ParseTreeNode nodeInfo, bool skip)
        {
        }
        protected virtual void AnalyzeTree(ParseTreeNode nodeInfo)
        {
        }

    }
    public class BindPointDescription : IMethodsBindPointDesc
    {
        /// <summary>
        /// BindAttribute Syntax Example [<Bind("target",Priority = 1,ControllerBindType = BindType.After,Target="target")>]
        /// </summary>
        public BindPointDescription(ControllerDescription _ctrlDesc, int num)
        {
            ctrlDesc = _ctrlDesc;
            strBind = "Bind" + num.ToString();
            Initialize();
        }
        private ControllerDescription ctrlDesc;
        private string strBind;
        private BindType bindType;
        private int bindLen;
        private int priority;
        private string target;
        private void Initialize()
        {
            try
            {
                if (ctrlDesc.attrs.ContainsKey(strBind))
                {
                    //TODO: usually target is the first parameter,but it can be set via named parameter Target
                    string[] splitTarget = ctrlDesc.attrs[strBind][0].Split(' ');
                    target = splitTarget[0];
                    if ((String.Compare(target, "get") == 0 || String.Compare(target, "post") == 0) && splitTarget.Length > 1)
                        target = splitTarget[1];

                    foreach (string param in ctrlDesc.attrs[strBind])
                    {
                        if (param.Contains("ControllerBindType"))
                        {
                            string[] type = param.Split(' ');
                            switch (type[2])
                            {
                                case "After":
                                    bindType = BindType.After;
                                    break;
                                case "Before":
                                    bindType = BindType.Before;
                                    break;
                                case "TearDown":
                                    bindType = BindType.Teardown;
                                    break;
                                case "PayLoad":
                                    bindType = BindType.Payload;
                                    break;
                            }
                        }
                        else if (param.Contains("Priority"))
                        {
                            string[] splitPrior = param.Split(' ');
                            priority = Convert.ToInt32(splitPrior[2]);
                        }

                    }
                }
                else
                {
                    //set default values
                    bindType = BindType.After;
                    priority = -1;
                    target = "";
                }
                bindLen = BindPointUtilities.GetBindComponents(Target).Length;
            }
            catch (Exception ex)
            {
            }
            
        }
        #region IMethodsBindPointDesc Members

        public int BindLength
        {
            get { return bindLen; }
        }

        public IMethodsControllerDesc Controller
        {
            get { return ctrlDesc; }
        }

        public Bistro.Controllers.Descriptor.BindType ControllerBindType
        {
            ///"controllerBindType(identifier)= After (identifier)
            get { return bindType; }
        }

        public int Priority
        {
            ///"Priority (identifier)= 1 (number)
            get { return priority; }
              
        }

        public string Target
        {
            get { return target; }
        }

        #endregion
    }
    public class ControllerDescription : IMethodsControllerDesc
    {
        public ControllerDescription(string _name, Dictionary<string, List<string>> _attrs)
        {
            name = _name;
            attrs = _attrs;
            nTargs = 0;
            foreach (KeyValuePair<string, List<string>> kvp in attrs)
            {
                if (kvp.Key.Contains("Bind"))
                    nTargs++;
            }
            targs = new List<IMethodsBindPointDesc>(nTargs);

        }
        public Dictionary<string, List<string>> attrs;
        private string name;
        private int nTargs;
        private List<IMethodsBindPointDesc> targs;

        #region IMethodsControllerDesc Members

        public string ControllerTypeName
        {
            get { return name; }
        }

        public List<string> DependsOn
        {
            get 
            {
                return attrs["DependsOn"];
            }
        }

        public string GetResourceType(string resourceName)
        {
            return resourceName + "resourceType";
        }

        public bool IsSecurity
        {
            get { return attrs.ContainsKey("Security"); }
        }

        public List<string> Provides
        {
            get { return attrs["Provides"]; }  
        }

        public List<string> Requires
        {
            get { return attrs["Requires"]; }
        }

        public IEnumerable<IMethodsBindPointDesc> Targets
        {
            get 
            { 
                for (int i = 0;i < nTargs; i++)
                {
                    targs.Add(new BindPointDescription(this, i));
                }
                return targs;
            }
        }

        #endregion
    }
    public static class MetadataFromDll
    {
        
        /// <summary>
        /// Loads the assembly.
        /// </summary>
        /// <param name="assm">The assm.</param>
        public static bool LoadAssemblies(List<Assembly> assmlist,EngineControllerDispatcher engine)
        {
            bool controllerFound = false;
            int k = 0;
            try
            {
                foreach (Assembly assm in assmlist)
                {
                    var aaa = assm.GetTypes();
                    int i = 0;
                    int j = aaa.Length;
                    foreach (Type t in aaa)
                    {
                        if (t.GetInterface(typeof(IController).Name) != null)
                        {
                            controllerFound = true;
                            if (!t.IsAbstract)
                            {
                                IControllerDescriptor descriptor = ControllerDescriptor.CreateDescriptor(t, null);
                                engine.RegisterController(descriptor);
                            }
                        }
                        i++;
                    }
                    k++;
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(ex.Message);
                for (int i = 0; i < ex.Types.Length; i++)
                    if (ex.Types[i] != null)
                        sb.AppendFormat("\t{0} loaded\r\n", ex.Types[i].Name);

                for (int i = 0; i < ex.LoaderExceptions.Length; i++)
                    if (ex.LoaderExceptions[i] != null)
                        sb.AppendFormat("\texception {0}\r\n", ex.LoaderExceptions[i].Message);

                Debug.WriteLine("ExceptionLoadingAssembly " + assmlist[k].FullName + " " + sb.ToString());
            }
            return controllerFound;
        }
    }
   
}
