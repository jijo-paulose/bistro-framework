using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Configuration;
using Bistro.Controllers;
using Bistro.Controllers.Dispatch;
using Bistro.Special.Reflection;
using System.Text.RegularExpressions;

namespace Bistro.UnitTestsNew
{
    [TestFixture]
    public class ControllerDispatcherTest
    {
        #region TestDescriptor
        internal class TestDescriptor
        {
            public string Name { get; set; }
            public TestTypeInfo[] Controllers { get; set; }
//            public IErrorDescriptor[] Errors { get; set; }
            public UrlControllersTest[] UrlTests { get; set; }
//            public BindingTest[] BindingTree { get; set; }

            public override string ToString()
            {
                return Name;
            }

            public TestDescriptor(string name, TestTypeInfo[] controllers, UrlControllersTest[] urlTests)
            {
                Name = name;
                Controllers = controllers;
                //Errors = errors ?? new IErrorDescriptor[0];
                UrlTests = urlTests ?? new UrlControllersTest[0];
                //BindingTree = bindingTree;
            }

            //public void ValidateErrors(List<IErrorDescriptor> baseErrorsList)
            //{

            //    Assert.IsNotNull(baseErrorsList, "Base errors list is null, something is wrong with tests initialization.");

            //    IErrorDescriptor[] baseErrorsArr = baseErrorsList.ToArray();

            //    Assert.AreEqual(baseErrorsArr.Length, Errors.Length, "Error lists have different lengths: actual-'{0}' expected-'{1}'", baseErrorsArr.Length, Errors.Length);


            //    for (int i = 0; i < baseErrorsArr.Length; i++)
            //    {
            //        baseErrorsArr[i].Validate(Errors[i]);
            //    }

            //}

        }
        #endregion

        #region Types and Type creation methods
        private TestTypeInfo[] Types(params TestTypeInfo[] types) { return types; }

        private TestTypeInfo Type(string type, params TestTypeInfo.TestAttributeInfo[] attributes)
        {
            
            return Type(type, attributes, new TestTypeInfo.TestFieldInfo[] { });
        }

        private TestTypeInfo Type(string type, TestTypeInfo.TestAttributeInfo[] attributes, params TestTypeInfo.TestFieldInfo[] fields)
        {
            return Type(type, attributes, fields, new TestTypeInfo.TestPropertyInfo[] { });
        }

        private TestTypeInfo Type(string type, TestTypeInfo.TestAttributeInfo[] attributes, params TestTypeInfo.TestPropertyInfo[] properties)
        {
            return Type(type, attributes, new TestTypeInfo.TestFieldInfo[] { }, properties);
        }

        private TestTypeInfo Type(string type, TestTypeInfo.TestAttributeInfo[] attributes, TestTypeInfo.TestFieldInfo[] fields, params TestTypeInfo.TestPropertyInfo[] properties)
        {
            return new TestTypeInfo(type, attributes, fields, properties);
        }
        #endregion

        #region Bind attributes creation methods

        private TestTypeInfo.TestAttributeInfo[] Attributes(params TestTypeInfo.TestAttributeInfo[] attributes) { return attributes; }

        private TestTypeInfo.TestFieldInfo Field(string name, string type, params TestTypeInfo.TestAttributeInfo[] attributes)
        {
            return new TestTypeInfo.TestFieldInfo(name, type, attributes);
        }

        private TestTypeInfo.TestAttributeInfo BindAttribute(string binding)
        {
            return new TestTypeInfo.TestAttributeInfo(typeof(BindAttribute), new TestTypeInfo.TestAttributeInfo.Parameter(binding));
        }

        private TestTypeInfo.TestAttributeInfo RequestAttribute { get { return new TestTypeInfo.TestAttributeInfo(typeof(RequestAttribute)); } }

        private TestTypeInfo.TestAttributeInfo FormFieldAttribute { get { return new TestTypeInfo.TestAttributeInfo(typeof(FormFieldAttribute)); } }

        private TestTypeInfo.TestAttributeInfo RequiresAttribute { get { return new TestTypeInfo.TestAttributeInfo(typeof(RequiresAttribute)); } }

        private TestTypeInfo.TestAttributeInfo ProvidesAttribute { get { return new TestTypeInfo.TestAttributeInfo(typeof(ProvidesAttribute)); } }

        private TestTypeInfo.TestAttributeInfo DependsOnAttribute { get { return new TestTypeInfo.TestAttributeInfo(typeof(DependsOnAttribute)); } }

        private TestTypeInfo.TestAttributeInfo SessionAttribute { get { return new TestTypeInfo.TestAttributeInfo(typeof(SessionAttribute)); } }
        #endregion

        #region Url tests creation methods

        private UrlControllersTest UrlTest(string name,string url, params string[] controllers)
        {
            url = url.Replace(" ", "");
            return new UrlControllersTest(name, url, controllers);
        }




        #endregion

        #region Node creation methods

        //private BindingTest Node(string url, params ControllerTest[] controllers)
        //{
        //    return Node(url, controllers, new BindingTest[] { });
        //}

        //private BindingTest Node(string url, params string[] controllers)
        //{
        //    return Node(url, Controllers(controllers), new BindingTest[] { });
        //}

        //private BindingTest Node(string url, ControllerTest[] controllers, params BindingTest[] children)
        //{
        //    return new BindingTest(url, controllers, children);
        //}

        #endregion`

        #region Controller tests creation methods
        //private ControllerTest[] Controllers() { return new ControllerTest[0]; }

        //private ControllerTest[] Controllers(params ControllerTest[] controllers) { return controllers; }

        //private ControllerTest[] Controllers(params string[] controllers)
        //{
        //    List<ControllerTest> result = new List<ControllerTest>();
        //    foreach (string name in controllers)
        //        result.Add(new ControllerTest(name, 0));
        //    return result.ToArray();
        //}

        //private ControllerTest Controller(string name, int seq) { return new ControllerTest(name, seq); }

        #endregion

        List<TestDescriptor> tests = new List<TestDescriptor>();

        #region Test Creation Methods

        //private void NewTest(string name, TestTypeInfo[] types, params BindingTest[] nodes)
        //{
        //    tests.Add(new TestDescriptor(name, types, null, null, nodes));
        //}

        private void NewTestWithUrl(string name, TestTypeInfo[] types, params UrlControllersTest[] urlTests)//, IErrorDescriptor[] errors, params BindingTest[] nodes)
        {
            tests.Add(new TestDescriptor(name, types, urlTests));
        }





        //private void NewErrorTest(string name, TestTypeInfo[] types, IErrorDescriptor[] errors, params BindingTest[] nodes)
        //{
        //    tests.Add(new TestDescriptor(name, types, errors, null, nodes));
        //}

        #endregion

        [TestFixtureSetUp]
        public void setup()
        {
            if (Application.Instance == null)
            {
                Application.Initialize(new SectionHandler());
            }

            application = Application.Instance;
            manager = application.ManagerFactory.GetManagerInstance();
            dispatcher = application.DispatcherFactory.GetDispatcherInstance();

        }

        private IControllerDispatcher dispatcher;
        private Application application;
        private IControllerManager manager;

        [Test, TestCaseSource("TestSource")]
        public void run(object test)
        {
            realTest(test);
        }

        private class UrlTuple
        {

            internal UrlTuple(string verb, string url)
            {
                Url = url;
                Verb = verb;
            }

            internal string Verb { get; set; }
            internal string Url { get; set; }
        
        }


        
        // The NUnit will not run a test for a method which is not public 
        // and I do not want to make TestDescriptor public
        void realTest(object test)
        {


            TestDescriptor descriptor = (TestDescriptor)test;

            List<IAttributeInfo> list = new List<IAttributeInfo>();

            ///
            foreach (TestTypeInfo testType in descriptor.Controllers)
            {
                IEnumerable<IAttributeInfo> attrList = testType.Attributes.Where(attr => { return attr.Type == typeof(BindAttribute).FullName; });
                list.AddRange(attrList);
            }

            List<UrlTuple> urlsList = new List<UrlTuple>();


            foreach (IAttributeInfo item in list)
            {
                string url = item.Parameters[0].AsString();
                string verb = "";
                foreach (string verbItem in BindPointUtilities.HttpVerbs)
                {
                    if (!url.StartsWith(verbItem, StringComparison.OrdinalIgnoreCase))
                        continue;

                    string remainder = url.Substring(verbItem.Length);
                    url = remainder;
                    verb = verbItem;
                    break;
                }
                if (verb == "")
                {
                    foreach (var vrb in BindPointUtilities.HttpVerbs)
                    {
                        AddItems(urlsList, vrb, url);
                    }
                }
                else
                {
                    AddItems(urlsList, verb, url);
                }

            }
            


            ///

            
            
            
            
            var mgr = manager as ControllerManager;
            var dsp = dispatcher as ControllerDispatcher;

            mgr.ClearAll();
            dsp.ClearAll();
            mgr.SpecialLoadForTest(descriptor.Controllers);
            foreach (UrlControllersTest urlTest in descriptor.UrlTests)
            {
                urlTest.Validate(dispatcher);
            }
        }

        #region Generate stuff. We'll move it to some other place
        private Regex rgx = new Regex(@"/(\*|\?|\{\w+})", RegexOptions.Compiled | RegexOptions.Singleline);

        private void AddItems(List<UrlTuple> urlsList, string vrb, string url)
        {
            MatchCollection mCol = rgx.Matches(url);
//            foreach( 
//            urlsList.Add(new UrlTuple(vrb, url));
        }
        #endregion

        internal IList<TestDescriptor> TestSource()
        {
            NewTestWithUrl("ValidUrls",
                Types(
                    Type(
                        "TestController",
                        BindAttribute("/auth/signin"),
                        BindAttribute("/postings/{contentType}")
                        )
                    ),
                    UrlTest("test1","GET /auth/signin","TestController")
                    );

            NewTestWithUrl("ValidUrls2",
                Types(
                    Type(
                        "TestController",
                        BindAttribute("/A/B"),
                        BindAttribute("/B/C")
                        ),
                    Type(
                        "CommonController",
                        BindAttribute("/A/*")
                        )
                    ),
                    UrlTest("test1", "GET /A/B", "CommonController","TestController")
                    );
            return tests;
        }
    }
}
