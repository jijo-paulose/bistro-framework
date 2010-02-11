using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using Irony.Parsing;
using Irony.Samples.CSharp;
using Irony.Samples.FSharp;
using Bistro.MethodsEngine.Reflection;
using Bistro.Interfaces;
using Bistro.Controllers.Descriptor;

namespace Bistro.Designer.Explorer
{
    
    using ControllersTable = Dictionary<string, Dictionary<string, List<string>>>;
    public class MetadataExtractor
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
        public MetadataExtractor(string lang, string fname)
        {
            m_parseTree = null;
            if (String.Compare(lang, "c#") == 0)
                m_grammar = new CSharpGrammar();
            else if (String.Compare(lang, "f#") == 0)
                m_grammar = new FSharpGrammar();
            else
            {
                throw new Exception("The grammar name you've typed is not supported");

            }
            m_parser = new Parser(new LanguageData(m_grammar));
            m_srcFilename = fname;
            controllerInfo = new ControllersTable();
            infobyFiles = new Dictionary<string, ControllersTable>();
        }
        public string FileName
        {
            set { m_srcFilename = value; }
            get { return m_srcFilename; }
        }
        /// <summary>
        /// Method is used to extract controllers' data from specified source file into ControllersTable 
        /// </summary>
        /// <returns>true if essential changes in controllers' declarations were detected,
        /// else - otherwise</returns>
        public bool FillControllerInfo()
        {
            TextReader rd = new StreamReader(m_srcFilename);
            controllerInfo.Clear();
            Extract(rd.ReadToEnd(), "<source>");
            if (m_parseTree == null) return false;
            AddParseNodeRec(m_parseTree.Root);
            //if source file has been recently added to the project,add new row into ControllersTable
            //else if there were any changes in source file,update corresponding row in ControllersTable
            if (!infobyFiles.ContainsKey(m_srcFilename))
            {
                ControllersTable curtbl = new ControllersTable(controllerInfo);
                infobyFiles.Add(m_srcFilename, curtbl);
                return true;
            }
            bool equal = new AttributesComparer().Equals(infobyFiles[m_srcFilename],controllerInfo);
            if (!equal)
            {
                if (infobyFiles.Remove(m_srcFilename))
                {
                    ControllersTable curtbl = new ControllersTable(controllerInfo);
                    infobyFiles.Add(m_srcFilename, curtbl);
                }
                return true;
            }
            return false;
        }

        #region Private members
        private ParseTree m_parseTree;
        private Parser m_parser;
        private Grammar m_grammar;
        private string m_srcFilename;
        private const string tail = " (identifier)";
        private string curCtrl;//name of the controller that is being processed
        private string curAttr;//name of the attribute that is being processed
        private int nBindTargs;//as Dictionary key must be unique,so we'll save it like Bind0..BindN for each controller 
        private bool bBindAttribute;//this flag shows whether curAttr is BindAttribute 
        private void Extract(string srcText, string fname)
        {
            try
            {
                m_parser.Parse(srcText, fname);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                m_parseTree = m_parser.Context.CurrentParseTree;
            }
        }

        private void AddParseNodeRec(ParseTreeNode nodeInfo)
        {
            if (nodeInfo == null) return;
            Token token = nodeInfo.AstNode as Token;
            if (token != null)
            {
                if (token.Terminal.Category != TokenCategory.Content) return;
            }
            string txt = nodeInfo.ToString();
            if (m_grammar.GetType() == typeof(FSharpGrammar)&& (String.Compare(txt, "FunctionDef") == 0) )
            {
                Trace.WriteLine(txt + " of " + nodeInfo.ChildNodes[2] + " :");
                curCtrl = nodeInfo.ChildNodes[2].ToString();
                curCtrl = curCtrl.Substring(0,curCtrl.Length - tail.Length);
                controllerInfo.Add(curCtrl, new Dictionary<string, List<string>>());
                nBindTargs = 0;
            }
            else if (m_grammar.GetType() == typeof(CSharpGrammar) && (String.Compare(txt, "class_declaration") == 0))
            {
                //Not implemented yet
                Trace.WriteLine(txt + " of " + nodeInfo.ChildNodes[3] + " :");
                curCtrl = nodeInfo.ChildNodes[3].ToString();
                curCtrl = curCtrl.Substring(0, curCtrl.Length - tail.Length);
                controllerInfo.Add(curCtrl, new Dictionary<string, List<string>>());
                nBindTargs = 0;

            }
            foreach (var child in nodeInfo.ChildNodes)
            {

                ParseTreeNode curChild = child;
                bool bIsAttribute  = String.Compare(child.ToString(), "attribute") == 0;
                bool bIsAttrArgs = String.Compare(child.ToString(), "attr_arg") == 0;
                if (bIsAttribute)
                {
                    while (curChild.ChildNodes.Count > 0)
                    {
                        curChild = curChild.ChildNodes[0];
                    }
                    Trace.WriteLine("<attr name>:" + curChild.ToString());
                    curAttr = curChild.ToString();
                    curAttr = curAttr.Substring(0, curAttr.Length - tail.Length);
                    bBindAttribute = String.Compare(curAttr, "Bind") == 0;
                    if (bBindAttribute)
                    {
                        curAttr += (nBindTargs++).ToString() ;
                        
                    }
                    controllerInfo[curCtrl].Add(curAttr, new List<string>());
                }
                else if (bIsAttrArgs)
                {
                    string curVal="";
                    bool bFullName = false;
                    //go down until we reach the leaf
                    if (bBindAttribute)
                    {
                        //Bind("target") - arg is Literal NT is processed as other common attr_args
                        //Bind("target",Priority=1,ControllerBindType = BindType.After)- args are Literal,BinExpr,Binexpr
                        while (curChild.ChildNodes.Count > 0)
                        {
                            curChild = curChild.ChildNodes[0];
                            //Processing of named parameters
                            if (String.Compare(curChild.ToString(), "BinExpr") == 0)
                            {
                                curVal += curChild.ChildNodes[0].ToString() + "= ";
                                //[0] is identifier like "Priority",
                                //[1] is assignment operation "="
                                //[2] is the value of named parameter
                                curChild = curChild.ChildNodes[2];
                                bFullName = true;//the value of named parameter (it maybe NT - that's why always select the last(right) child)
                                break;

                            }
                            else if (String.Compare(curChild.ToString(), "Literal") == 0)
                            {
                                break;
                            }
                        }
                    }
                    while (curChild.ChildNodes.Count > 0)
                    {
                        curChild = (bFullName)? curChild.ChildNodes[curChild.ChildNodes.Count-1]: curChild.ChildNodes[0];
                    }
                    Trace.WriteLine("<val>:" + curChild.ToString());
                    curVal += curChild.ToString();
                    (controllerInfo[curCtrl][curAttr]).Add(curVal);
                }

                AddParseNodeRec(child);
            }

        }
        #endregion


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
            if (ctrlDesc.attrs.ContainsKey(strBind))
            {
                //TODO: usually target is the first parameter,but it can be set via named parameter Target
                string[] splitTarget = ctrlDesc.attrs[strBind][0].Split(' ');
                target =  splitTarget[0];
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
        public Dictionary<string, List<string>> attrs;
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
            Initialize();

        }
        private string name;
        private int nTargs;
        private List<IMethodsBindPointDesc> targs;
        private List<string> dependsOn;
        private List<string> provides;
        private List<string> requires;
        private bool bisSecurity;

        private void Initialize()
        {
            dependsOn = (attrs.ContainsKey("DependsOn")) ? attrs["DependsOn"] : new List<string>();
            bisSecurity = attrs.ContainsKey("IsSecurity");
            provides = (attrs.ContainsKey("Provides")) ? attrs["Provides"] : new List<string>();
            requires = (attrs.ContainsKey("Requires")) ? attrs["Requires"] : new List<string>();

        }
        #region IMethodsControllerDesc Members

        public string ControllerTypeName
        {
            get { return name; }
        }

        public List<string> DependsOn
        {
            get 
            {
                return dependsOn;
            }
        }

        public string GetResourceType(string resourceName)
        {
            throw new NotImplementedException();
        }

        public bool IsSecurity
        {
            get { return bisSecurity; }
        }

        public List<string> Provides
        {
            get { return provides; }  
        }

        public List<string> Requires
        {
            get { return requires; }
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
   
}
