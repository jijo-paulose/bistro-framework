using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Bistro.Controllers.Dispatch;
using Bistro.Controllers;
using Bistro.Configuration;
using Bistro.CompatibilityTests.Reflection;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Descriptor.Data;

namespace Bistro.CompatibilityTests
{
    [TestFixture]
    public class TestsDispatcher
    {
        #region Test creation stuff
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
            return new TestTypeInfo.TestAttributeInfo(typeof(BindAttribute), new Parameter("Target", binding), new Parameter("ControllerBindType", BindType.Before), new Parameter("Priority", -1));
        }

        private TestTypeInfo.TestAttributeInfo RequestAttribute { get { return new TestTypeInfo.TestAttributeInfo(typeof(RequestAttribute)); } }

        private TestTypeInfo.TestAttributeInfo FormFieldAttribute { get { return new TestTypeInfo.TestAttributeInfo(typeof(FormFieldAttribute)); } }

        private TestTypeInfo.TestAttributeInfo RequiresAttribute { get { return new TestTypeInfo.TestAttributeInfo(typeof(RequiresAttribute)); } }

        private TestTypeInfo.TestAttributeInfo ProvidesAttribute { get { return new TestTypeInfo.TestAttributeInfo(typeof(ProvidesAttribute)); } }

        private TestTypeInfo.TestAttributeInfo DependsOnAttribute { get { return new TestTypeInfo.TestAttributeInfo(typeof(DependsOnAttribute)); } }

        private TestTypeInfo.TestAttributeInfo SessionAttribute { get { return new TestTypeInfo.TestAttributeInfo(typeof(SessionAttribute)); } }
        #endregion

        #region Url tests creation methods

        private UrlControllersTest UrlTest(string name, string url, params string[] controllers)
        {
            url = url.Replace(" ", "");
            return new UrlControllersTest(name, url, controllers);
        }




        #endregion


        List<TestDescriptor> tests = new List<TestDescriptor>();

        #region Test Creation Methods

        private void NewTestWithUrl(string name, TestTypeInfo[] types, params UrlControllersTest[] urlTests)//, IErrorDescriptor[] errors, params BindingTest[] nodes)
        {
            tests.Add(new TestDescriptor(name, types, urlTests));
        }


        #endregion



        #endregion


        [TestFixtureSetUp]
        public void setup()
        {

            initSh = new SectionHandler();

            initSh.ControllerManagerFactory = "Bistro.CompatibilityTests.TestControllerManagerFactory, Bistro.CompatibilityTests";





        }


        #region private fields
        private Application application;
        private IControllerManager manager;
        private IControllerDispatcher dispatcher;
        private SectionHandler initSh;
        #endregion



        void realTest(object test)
        {
            TestDescriptor descriptor = (TestDescriptor)test;


            Application.Initialize(initSh);

            application = Application.Instance;
            manager = application.ManagerFactory.GetManagerInstance();
            dispatcher = application.DispatcherFactory.GetDispatcherInstance();

            TestControllerManager testMgr = manager as TestControllerManager;
            Assert.IsNotNull(testMgr,"Invalid TestControllerManager");

            testMgr.LoadSpecial(descriptor.Controllers);


        }


        [Test, TestCaseSource("TestSource")]
        public void run(object test)
        {
            realTest(test);
        }


        internal IList<TestDescriptor> TestSource()
        {
            #region ValidUrls1 test
            NewTestWithUrl("ValidUrls",
                Types(
                    Type(
                        "TestController",
                        BindAttribute("/auth/signin"),
                        BindAttribute("/postings/{contentType}")
                        )
                    ),
                    UrlTest("test1", "GET /auth/signin", "TestController"),
                    UrlTest("GET /auth/signin", "GET /auth/signin", "TestController"),
                    UrlTest("POST /auth/signin", "POST /auth/signin", "TestController"),
                    UrlTest("PUT /auth/signin", "PUT /auth/signin", "TestController"),
                    UrlTest("DELETE /auth/signin", "DELETE /auth/signin", "TestController"),
                    UrlTest("GET /postings/", "GET /postings/", "TestController"),
                    UrlTest("GET /postings/variablevalue1", "GET /postings/variablevalue1", "TestController"),
                    UrlTest("GET /postings/123412423", "GET /postings/123412423", "TestController"),
                    UrlTest("GET /postings/testvalue", "GET /postings/testvalue", "TestController"),
                    UrlTest("POST /postings/", "POST /postings/", "TestController"),
                    UrlTest("POST /postings/variablevalue1", "POST /postings/variablevalue1", "TestController"),
                    UrlTest("POST /postings/123412423", "POST /postings/123412423", "TestController"),
                    UrlTest("POST /postings/testvalue", "POST /postings/testvalue", "TestController"),
                    UrlTest("PUT /postings/", "PUT /postings/", "TestController"),
                    UrlTest("PUT /postings/variablevalue1", "PUT /postings/variablevalue1", "TestController"),
                    UrlTest("PUT /postings/123412423", "PUT /postings/123412423", "TestController"),
                    UrlTest("PUT /postings/testvalue", "PUT /postings/testvalue", "TestController"),
                    UrlTest("DELETE /postings/", "DELETE /postings/", "TestController"),
                    UrlTest("DELETE /postings/variablevalue1", "DELETE /postings/variablevalue1", "TestController"),
                    UrlTest("DELETE /postings/123412423", "DELETE /postings/123412423", "TestController"),
                    UrlTest("DELETE /postings/testvalue", "DELETE /postings/testvalue", "TestController")
                    );
            #endregion






            return tests;
        }




    }
}
