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
using System.Text.RegularExpressions;

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


        #region Generate stuff. We'll move it to some other place

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

        private Regex rgx = new Regex(@"(/(?:\*|\?|\{\w+}))", RegexOptions.Compiled | RegexOptions.Singleline);



        string[] testQuestionMark = new string[] { "", "/abcde/edcba/aaaa123/bbb124", "/bbb124", "/aaaa123/bbb124" };

        string[] testAsterisk = new string[] { "/aaaaa", "/abcde", "/testvalue" };
        string[] testVariable = new string[] { "/", "/variablevalue1", "/123412423", "/testvalue" };


        private void ProcessUrlRec(List<UrlTuple> urlsList, string vrb, string preProcessedUrl)
        {
            Match mtch = rgx.Match(preProcessedUrl);
            if (!mtch.Success)
            {
                if (preProcessedUrl.Trim() != String.Empty)
                    urlsList.Add(new UrlTuple(vrb, preProcessedUrl));
            }
            else
            {

                switch (mtch.Groups[1].Value)
                {
                    case "/?":
                        foreach (string repItem in testQuestionMark)
                        {
                            ProcessUrlRec(urlsList, vrb, preProcessedUrl.Remove(mtch.Groups[1].Index, mtch.Groups[1].Length).Insert(mtch.Groups[1].Index, repItem));
                        }
                        break;
                    case "/*":
                        foreach (string repItem in testAsterisk)
                        {
                            ProcessUrlRec(urlsList, vrb, preProcessedUrl.Remove(mtch.Groups[1].Index, mtch.Groups[1].Length).Insert(mtch.Groups[1].Index, repItem));
                        }

                        break;
                    default:
                        foreach (string repItem in testVariable)
                        {
                            ProcessUrlRec(urlsList, vrb, preProcessedUrl.Remove(mtch.Groups[1].Index, mtch.Groups[1].Length).Insert(mtch.Groups[1].Index, repItem));
                        }

                        break;
                }
            }

        }

        #endregion


        void realTest(object test)
        {
            #region Load part
            TestDescriptor descriptor = (TestDescriptor)test;


            Application.Initialize(initSh);

            application = Application.Instance;
            manager = application.ManagerFactory.GetManagerInstance();
            dispatcher = application.DispatcherFactory.GetDispatcherInstance();

            TestControllerManager testMgr = manager as TestControllerManager;
            Assert.IsNotNull(testMgr,"Invalid TestControllerManager");

            testMgr.LoadSpecial(descriptor.Controllers);
            #endregion



            #region Test part



            foreach (UrlControllersTest urlTest in descriptor.UrlTests)
            {
                urlTest.Validate(dispatcher);
            }

            #endregion
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

            #region ValidUrls2 test
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
                    UrlTest("test1", "GET /A/B", "CommonController", "TestController"),
                    UrlTest("GET /A/B", "GET /A/B", "CommonController", "TestController"),
                    UrlTest("POST /A/B", "POST /A/B", "CommonController", "TestController"),
                    UrlTest("PUT /A/B", "PUT /A/B", "CommonController", "TestController"),
                    UrlTest("DELETE /A/B", "DELETE /A/B", "CommonController", "TestController"),
                    UrlTest("GET /B/C", "GET /B/C", "TestController"),
                    UrlTest("POST /B/C", "POST /B/C", "TestController"),
                    UrlTest("PUT /B/C", "PUT /B/C", "TestController"),
                    UrlTest("DELETE /B/C", "DELETE /B/C", "TestController"),
                    UrlTest("GET /A/aaaaa", "GET /A/aaaaa", "CommonController"),
                    UrlTest("GET /A/abcde", "GET /A/abcde", "CommonController"),
                    UrlTest("GET /A/testvalue", "GET /A/testvalue", "CommonController"),
                    UrlTest("POST /A/aaaaa", "POST /A/aaaaa", "CommonController"),
                    UrlTest("POST /A/abcde", "POST /A/abcde", "CommonController"),
                    UrlTest("POST /A/testvalue", "POST /A/testvalue", "CommonController"),
                    UrlTest("PUT /A/aaaaa", "PUT /A/aaaaa", "CommonController"),
                    UrlTest("PUT /A/abcde", "PUT /A/abcde", "CommonController"),
                    UrlTest("PUT /A/testvalue", "PUT /A/testvalue", "CommonController"),
                    UrlTest("DELETE /A/aaaaa", "DELETE /A/aaaaa", "CommonController"),
                    UrlTest("DELETE /A/abcde", "DELETE /A/abcde", "CommonController"),
                    UrlTest("DELETE /A/testvalue", "DELETE /A/testvalue", "CommonController")
                    );
            #endregion

            #region Big Test
            NewTestWithUrl("Big Test",
                Types(
                    Type("Pageable",
                        Attributes(
                            BindAttribute("GET /?/Pageable?{PageNumber}&{PageSize}")
                        ),
                        Field("QueryStringCollection", "NameValueCollection", DependsOnAttribute, RequestAttribute),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("PageNumber", "int", ProvidesAttribute, RequestAttribute),
                        Field("PageSize", "int", ProvidesAttribute, RequestAttribute),
                        Field("TotalItemCount", "int", ProvidesAttribute, RequestAttribute),
                        Field("HasPreviousPage", "bool", ProvidesAttribute, RequestAttribute),
                        Field("HasNextPage", "bool", ProvidesAttribute, RequestAttribute),
                        Field("PageCount", "int", ProvidesAttribute, RequestAttribute),
                        Field("IsPaged", "bool", ProvidesAttribute, RequestAttribute),
                        Field("PageNumbers", "ArrayList", ProvidesAttribute, RequestAttribute),
                        Field("PagingInfo", "string", DependsOnAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", DependsOnAttribute, RequestAttribute)
                        ),

                    Type("Sortable",
                        Attributes(
                            BindAttribute("GET /?/Sortable?{OrderBy}&{Direction}")
                        ),
                        Field("QueryStringCollection", "NameValueCollection", DependsOnAttribute, RequestAttribute),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("OrderBy", "string", ProvidesAttribute, RequestAttribute),
                        Field("Direction", "string", ProvidesAttribute, RequestAttribute),
                        Field("IsSorted", "bool", ProvidesAttribute, RequestAttribute),
                        Field("PagingInfo", "string", ProvidesAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("Authenticator",
                        Attributes(
                            BindAttribute("/?")
                        ),
                        Field("User", "AIMUserStub", ProvidesAttribute, SessionAttribute),
                        Field("PermissionManager", "object", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("Bar",
                        Attributes(
                            BindAttribute("GET /Bar/{BarID}")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("QueryStringCollection", "NameValueCollection", DependsOnAttribute, RequestAttribute),
                        Field("BarID", "string", ProvidesAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("BarData16", "BarData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("BarDelete",
                        Attributes(
                            BindAttribute("DELETE /Bar/{BarID}")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("QueryStringCollection", "NameValueCollection", DependsOnAttribute, RequestAttribute),
                        Field("BarID", "string", ProvidesAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("BarData16", "BarData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("Bars",
                        Attributes(
                            BindAttribute("GET /Bars")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("QueryStringCollection", "NameValueCollection", DependsOnAttribute, RequestAttribute),
                        Field("BarData16", "BarData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("Data12Id", "short", DependsOnAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("GetNextStep",
                        Attributes(
                            BindAttribute("GET /Clinical/GetNextStep?{TransactionId}&{Data17isId}")
                        ),
                        Field("TransactionId", "string", RequestAttribute),
                        Field("Data17isId", "string", RequestAttribute),
                        Field("Questions", "List<ClinicalQuestionStub>", RequestAttribute),
                        Field("Actions", "List<ClinicalAction>", RequestAttribute)
                        ),
                    Type("InitializeClinical",
                        Attributes(
                            BindAttribute("GET /Clinical/Initialize")
                        ),
                        Field("TransactionId", "string", ProvidesAttribute, RequestAttribute),
                        Field("Data17es", "List<Data17is>", ProvidesAttribute, RequestAttribute),
                        Field("ApplicationId", "int", RequestAttribute),
                        Field("BarId", "int", RequestAttribute),
                        Field("ExamId", "int", RequestAttribute),
                        Field("Data15Group", "short", RequestAttribute),
                        Field("Date", "DateTime", RequestAttribute),
                        Field("Data12Id", "short", RequestAttribute),
                        Field("FooDob", "DateTime", RequestAttribute),
                        Field("FooGender", "GenderStub", RequestAttribute),
                        Field("TransactionType", "TransactionTypeStub", RequestAttribute),
                        Field("UserId", "int", RequestAttribute),
                        Field("IsOverwrite", "bool", RequestAttribute),
                        Field("BarData15Groups", "string", RequestAttribute),
                        Field("RevisionType", "RevisionTypeStub", RequestAttribute),
                        Field("ClinicalProductId", "byte", RequestAttribute),
                        Field("DateOfData16", "DateTime", RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("Comments",
                        Attributes(
                            BindAttribute("GET /Comments")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("CommentsData16", "CommentsData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("Categories", "object", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("CommentsSend",
                        Attributes(
                            BindAttribute("GET /Comments/Get")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("CommentsData16", "CommentsData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("UserComments", "object", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("DrugCategories",
                        Attributes(
                            BindAttribute("GET /DrugCategories")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("DrugData16", "DrugData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("vtBar", "BarStub", DependsOnAttribute, SessionAttribute),
                        Field("Categories", "object", ProvidesAttribute, SessionAttribute)
                        ),
                    Type("Drugs",
                        Attributes(
                            BindAttribute("GET /Drugs")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("DrugData16", "DrugData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("vtBar", "BarStub", DependsOnAttribute, SessionAttribute)
                        ),
                    Type("Enrollments",
                        Attributes(
                            BindAttribute("GET /Enrollments")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("FooData16", "FooData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("RenderFooEligibilityList", "bool", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("HomeController",
                        Attributes(
                            BindAttribute("GET /default")
                        ),
                        Field("HelpUrl", "string", ProvidesAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("BarData16", "BarData16Stub", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("Foo",
                        Attributes(
                            BindAttribute("GET /EligibilityRouter")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("FooData16", "FooData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("FooRequest", "object", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("Foos",
                        Attributes(
                            BindAttribute("GET /Foos")
                        ),
                        Field("FooData16", "FooData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("QueryStringCollection", "NameValueCollection", DependsOnAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("HelpUrl", "string", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("OrderInquiry",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderInquiry")
                        ),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("UIState", "UIStateStub", SessionAttribute),
                        Field("Data12Data16", "Data12Data16Stub", DependsOnAttribute, RequestAttribute),
                        Field("HealthPlanListHolder", "object", ProvidesAttribute, RequestAttribute),
                        Field("HelpUrl", "string", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("OrderInquiryListBySite",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderInquiry/OrderListBySite")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("Data12Id", "short", DependsOnAttribute, RequestAttribute),
                        Field("BarData16", "BarData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("OrderInquiryList", "object", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("DeleteBar",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/DeleteBar")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("BarData16", "BarData16Stub", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("ProcessBarDeletion",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/ProcessBarDeletion")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("BarData16", "BarData16Stub", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("DrugSelector",
                        Attributes(
                            BindAttribute("GET /DrugSelector?{CategoryId}&{DrugId}")/////////////////////////////////////////////////////////////
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("vtBar", "BarStub", DependsOnAttribute, SessionAttribute),
                        Field("vtExam", "ExamStub", DependsOnAttribute, SessionAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("BarData16", "BarData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("DrugData16", "DrugData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("CategoryId", "string", ProvidesAttribute, RequestAttribute),
                        Field("DrugId", "string", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("EligibilityRouter",
                        Attributes(
                            BindAttribute("GET /EligibilityRouter")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("FooData16", "FooData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("vtBar", "BarStub", DependsOnAttribute, SessionAttribute),
                        Field("RenderFooEligibilityList", "bool", ProvidesAttribute, RequestAttribute),
                        Field("RenderFooInformation", "bool", ProvidesAttribute, RequestAttribute),
                        Field("RenderFooHistoryList", "bool", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("FooHistoryList",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/FooHistoryList")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("FooData16", "FooData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("BarData16", "BarData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("PagedDataHolder", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("RenderFooHistoryList", "bool", DependsOnAttribute, RequestAttribute),
                        Field("RenderPhysicianListForRequest", "bool", ProvidesAttribute, RequestAttribute),
                        Field("FooPlanID", "byte?", ProvidesAttribute, RequestAttribute),
                        Field("vtBar", "BarStub", DependsOnAttribute, SessionAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("StepNumber", "int", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("FooInformation",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/FooInformation")
                        ),
                        Field("RenderFooInformation", "bool", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("OrderRequest",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest")
                        ),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("BarData16", "BarData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("Data12Data16", "Data12Data16Stub", DependsOnAttribute, RequestAttribute),
                        Field("UIState", "UIStateStub", SessionAttribute),
                        Field("HealthPlanListHolder", "object", ProvidesAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("CurrentDate", "string", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("OrderRequestQueue",
                        Attributes(
                            BindAttribute("GET /OrderRequestQueue")
                        ),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("PhysicianSelector",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/PhysicianSelector?{PhysicianId}")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("BarData16", "BarData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("Data14Data16", "Data14Data16Stub", DependsOnAttribute, RequestAttribute),
                        Field("vtBar", "BarStub", DependsOnAttribute, SessionAttribute),
                        Field("Physician", "object", ProvidesAttribute, RequestAttribute),
                        Field("PhysicianId", "int", ProvidesAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("StepNumber", "int", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("StepWizard",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest")/////////////////////////////////////////////////////
                        ),
                        Field("StepNumber", "int", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("Physicians",
                        Attributes(
                            BindAttribute("GET /Physicians")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("Data14Data16", "Data14Data16Stub", DependsOnAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("vtBar", "BarStub", DependsOnAttribute, SessionAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("FooPlanID", "byte?", DependsOnAttribute, RequestAttribute),
                        Field("RenderPhysicianListForRequest", "bool", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("Foos",
                        Attributes(
                            BindAttribute("GET /Data14s")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("QueryStringCollection", "NameValueCollection", DependsOnAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("Data12Id", "short", DependsOnAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("HelpUrl", "string", ProvidesAttribute, RequestAttribute),
                        Field("Data14Data16", "Data14Data16Stub", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("QUnit",
                        Attributes(
                            BindAttribute("GET /QUnit")
                        )
                        ),
                    Type("RequestItems",
                        Attributes(
                            BindAttribute("GET /RequestItems")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("DrugData16", "DrugData16Stub", DependsOnAttribute, RequestAttribute),
                        Field("vtBar", "BarStub", DependsOnAttribute, SessionAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("AjaxDeterminer",
                        Attributes(
                            BindAttribute("/?")
                        ),
                        Field("IsAjaxRequest", "bool", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("AntiForgeryRequestHandler",
                        Attributes(
                            BindAttribute("POST /?")
                        ),
                        Field("AntiForgeryRequestTokenCookie", "string", DependsOnAttribute, RequestAttribute),
                        Field("AntiForgeryRequestToken", "string", DependsOnAttribute, RequestAttribute, FormFieldAttribute)
                        ),
                    Type("AntiForgeryRequestSetter",
                        Attributes(
                            BindAttribute("GET /?")
                        ),
                        Field("AntiForgeryRequestTokenCookie", "string", DependsOnAttribute, RequestAttribute),
                        Field("AntiForgeryRequestToken", "string", DependsOnAttribute, RequestAttribute, FormFieldAttribute)
                        ),
                    Type("Fake",
                        Attributes(
                            BindAttribute("/?")
                        ),
                        Field("UserID", "int", ProvidesAttribute, RequestAttribute),
                        Field("Data12Id", "short", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("Messenger",
                        Attributes(
                            BindAttribute("/?")
                        ),
                        Field("MessageList", "object", ProvidesAttribute, RequestAttribute),
                        Field("MessageID", "string", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("QueryStringDeterminer",
                        Attributes(
                            BindAttribute("/?")
                        ),
                        Field("QueryStringCollection", "NameValueCollection", ProvidesAttribute, RequestAttribute),
                        Field("QueryString", "string", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("RootRedirect",
                        Attributes(
                            BindAttribute("/")
                        )
                        ),
                    Type("DefaultController",
                        Attributes(
                            BindAttribute("/?")
                        ),
                        Field("BarData16", "BarData16Stub", ProvidesAttribute, RequestAttribute),
                        Field("FooData16", "FooData16Stub", ProvidesAttribute, RequestAttribute),
                        Field("Data12Data16", "Data12Data16Stub", ProvidesAttribute, RequestAttribute),
                        Field("UserData16", "UserData16Stub", ProvidesAttribute, RequestAttribute),
                        Field("Data14Data16", "Data14Data16Stub", ProvidesAttribute, RequestAttribute),
                        Field("UIState", "UIStateStub", ProvidesAttribute, SessionAttribute),
                        Field("CommentsData16", "CommentsData16Stub", ProvidesAttribute, RequestAttribute),
                        Field("Root", "string", ProvidesAttribute, RequestAttribute)
                        )
                    ),
                    UrlTest("GET /Pageable?{PageNumber}&{PageSize}", "GET /Pageable?{PageNumber}&{PageSize}", "Messenger", "Fake", "DefaultController", "QueryStringDeterminer", "Authenticator", "Pageable", "AntiForgeryRequestSetter", "AjaxDeterminer"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/Pageable?{PageNumber}&{PageSize}", "GET /abcde/edcba/aaaa123/bbb124/Pageable?{PageNumber}&{PageSize}", "Messenger", "Fake", "DefaultController", "QueryStringDeterminer", "Authenticator", "Pageable", "AntiForgeryRequestSetter", "AjaxDeterminer"),
                    UrlTest("GET /bbb124/Pageable?{PageNumber}&{PageSize}", "GET /bbb124/Pageable?{PageNumber}&{PageSize}", "Messenger", "Fake", "DefaultController", "QueryStringDeterminer", "Authenticator", "Pageable", "AntiForgeryRequestSetter", "AjaxDeterminer"),
                    UrlTest("GET /aaaa123/bbb124/Pageable?{PageNumber}&{PageSize}", "GET /aaaa123/bbb124/Pageable?{PageNumber}&{PageSize}", "Messenger", "Fake", "DefaultController", "QueryStringDeterminer", "Authenticator", "Pageable", "AntiForgeryRequestSetter", "AjaxDeterminer"),
                    UrlTest("GET /Sortable?{OrderBy}&{Direction}", "GET /Sortable?{OrderBy}&{Direction}", "Messenger", "Fake", "DefaultController", "QueryStringDeterminer", "Authenticator", "Sortable", "AntiForgeryRequestSetter", "AjaxDeterminer"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/Sortable?{OrderBy}&{Direction}", "GET /abcde/edcba/aaaa123/bbb124/Sortable?{OrderBy}&{Direction}", "Messenger", "Fake", "DefaultController", "QueryStringDeterminer", "Authenticator", "Sortable", "AntiForgeryRequestSetter", "AjaxDeterminer"),
                    UrlTest("GET /bbb124/Sortable?{OrderBy}&{Direction}", "GET /bbb124/Sortable?{OrderBy}&{Direction}", "Messenger", "Fake", "DefaultController", "QueryStringDeterminer", "Authenticator", "Sortable", "AntiForgeryRequestSetter", "AjaxDeterminer"),
                    UrlTest("GET /aaaa123/bbb124/Sortable?{OrderBy}&{Direction}", "GET /aaaa123/bbb124/Sortable?{OrderBy}&{Direction}", "Messenger", "Fake", "DefaultController", "QueryStringDeterminer", "Authenticator", "Sortable", "AntiForgeryRequestSetter", "AjaxDeterminer"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124", "GET /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /bbb124", "GET /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /aaaa123/bbb124", "GET /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124", "POST /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /bbb124", "POST /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /aaaa123/bbb124", "POST /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124", "PUT /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("PUT /bbb124", "PUT /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("PUT /aaaa123/bbb124", "PUT /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124", "DELETE /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /bbb124", "DELETE /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /aaaa123/bbb124", "DELETE /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("GET /Bar/", "GET /Bar/", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "Bar"),
                    UrlTest("GET /Bar/variablevalue1", "GET /Bar/variablevalue1", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "Bar"),
                    UrlTest("GET /Bar/123412423", "GET /Bar/123412423", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "Bar"),
                    UrlTest("GET /Bar/testvalue", "GET /Bar/testvalue", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "Bar"),
                    UrlTest("DELETE /Bar/", "DELETE /Bar/", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake", "BarDelete"),
                    UrlTest("DELETE /Bar/variablevalue1", "DELETE /Bar/variablevalue1", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake", "BarDelete"),
                    UrlTest("DELETE /Bar/123412423", "DELETE /Bar/123412423", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake", "BarDelete"),
                    UrlTest("DELETE /Bar/testvalue", "DELETE /Bar/testvalue", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake", "BarDelete"),
                    UrlTest("GET /Bars", "GET /Bars", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "Fake", "Bars", "AntiForgeryRequestSetter"),
                    UrlTest("GET /Clinical/GetNextStep?{TransactionId}&{Data17isId}", "GET /Clinical/GetNextStep?{TransactionId}&{Data17isId}", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "GetNextStep"),
                    UrlTest("GET /Clinical/Initialize", "GET /Clinical/Initialize", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "InitializeClinical"),
                    UrlTest("GET /Comments", "GET /Comments", "QueryStringDeterminer", "Messenger", "DefaultController", "Comments", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /Comments/Get", "GET /Comments/Get", "QueryStringDeterminer", "Messenger", "DefaultController", "Comments", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter", "CommentsSend"),
                    UrlTest("GET /DrugCategories", "GET /DrugCategories", "QueryStringDeterminer", "Messenger", "DrugCategories", "DefaultController", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /Drugs", "GET /Drugs", "QueryStringDeterminer", "Messenger", "Drugs", "DefaultController", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /Enrollments", "GET /Enrollments", "QueryStringDeterminer", "Messenger", "DefaultController", "Enrollments", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /default", "GET /default", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "HomeController", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /EligibilityRouter", "GET /EligibilityRouter", "DefaultController", "QueryStringDeterminer", "EligibilityRouter", "Foo", "Messenger", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /Foos", "GET /Foos", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "Foos", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /OrderManager/OrderInquiry", "GET /OrderManager/OrderInquiry", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderInquiry"),
                    UrlTest("GET /OrderManager/OrderInquiry/OrderListBySite", "GET /OrderManager/OrderInquiry/OrderListBySite", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderInquiry", "OrderInquiryListBySite"),
                    UrlTest("GET /OrderManager/OrderRequest/DeleteBar", "GET /OrderManager/OrderRequest/DeleteBar", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderRequest", "StepWizard", "DeleteBar"),
                    UrlTest("GET /OrderManager/OrderRequest/ProcessBarDeletion", "GET /OrderManager/OrderRequest/ProcessBarDeletion", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderRequest", "StepWizard", "ProcessBarDeletion"),
                    UrlTest("GET /DrugSelector?{CategoryId}&{DrugId}", "GET /DrugSelector?{CategoryId}&{DrugId}", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "DrugSelector", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /EligibilityRouter", "GET /EligibilityRouter", "DefaultController", "QueryStringDeterminer", "EligibilityRouter", "Foo", "Messenger", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /OrderManager/OrderRequest/FooHistoryList", "GET /OrderManager/OrderRequest/FooHistoryList", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderRequest", "FooHistoryList", "StepWizard"),
                    UrlTest("GET /OrderManager/OrderRequest/FooInformation", "GET /OrderManager/OrderRequest/FooInformation", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderRequest", "StepWizard", "FooInformation"),
                    UrlTest("GET /OrderManager/OrderRequest", "GET /OrderManager/OrderRequest", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "StepWizard", "OrderRequest"),
                    UrlTest("GET /OrderRequestQueue", "GET /OrderRequestQueue", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "OrderRequestQueue", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /OrderManager/OrderRequest/PhysicianSelector?{PhysicianId}", "GET /OrderManager/OrderRequest/PhysicianSelector?{PhysicianId}", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderRequest", "PhysicianSelector", "StepWizard"),
                    UrlTest("GET /OrderManager/OrderRequest", "GET /OrderManager/OrderRequest", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "StepWizard", "OrderRequest"),
                    UrlTest("GET /Physicians", "GET /Physicians", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "Physicians", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /Data14s", "GET /Data14s", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "Fake", "Foos", "AntiForgeryRequestSetter"),
                    UrlTest("GET /QUnit", "GET /QUnit", "QueryStringDeterminer", "Messenger", "QUnit", "DefaultController", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /RequestItems", "GET /RequestItems", "QueryStringDeterminer", "Messenger", "RequestItems", "DefaultController", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124", "GET /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /bbb124", "GET /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /aaaa123/bbb124", "GET /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124", "POST /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /bbb124", "POST /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /aaaa123/bbb124", "POST /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124", "PUT /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("PUT /bbb124", "PUT /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("PUT /aaaa123/bbb124", "PUT /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124", "DELETE /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /bbb124", "DELETE /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /aaaa123/bbb124", "DELETE /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124", "POST /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /bbb124", "POST /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /aaaa123/bbb124", "POST /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124", "GET /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /bbb124", "GET /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /aaaa123/bbb124", "GET /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124", "GET /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /bbb124", "GET /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /aaaa123/bbb124", "GET /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124", "POST /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /bbb124", "POST /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /aaaa123/bbb124", "POST /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124", "PUT /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("PUT /bbb124", "PUT /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("PUT /aaaa123/bbb124", "PUT /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124", "DELETE /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /bbb124", "DELETE /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /aaaa123/bbb124", "DELETE /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124", "GET /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /bbb124", "GET /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /aaaa123/bbb124", "GET /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124", "POST /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /bbb124", "POST /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /aaaa123/bbb124", "POST /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124", "PUT /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("PUT /bbb124", "PUT /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("PUT /aaaa123/bbb124", "PUT /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124", "DELETE /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /bbb124", "DELETE /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /aaaa123/bbb124", "DELETE /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124", "GET /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /bbb124", "GET /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /aaaa123/bbb124", "GET /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124", "POST /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /bbb124", "POST /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /aaaa123/bbb124", "POST /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124", "PUT /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("PUT /bbb124", "PUT /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("PUT /aaaa123/bbb124", "PUT /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124", "DELETE /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /bbb124", "DELETE /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /aaaa123/bbb124", "DELETE /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("GET /", "GET /", "QueryStringDeterminer", "Messenger", "RootRedirect", "DefaultController", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("POST /", "POST /", "QueryStringDeterminer", "Messenger", "RootRedirect", "DefaultController", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestHandler"),
                    UrlTest("PUT /", "PUT /", "QueryStringDeterminer", "DefaultController", "RootRedirect", "Messenger", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /", "DELETE /", "QueryStringDeterminer", "DefaultController", "RootRedirect", "Messenger", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124", "GET /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /bbb124", "GET /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("GET /aaaa123/bbb124", "GET /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124", "POST /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /bbb124", "POST /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("POST /aaaa123/bbb124", "POST /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestHandler"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124", "PUT /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("PUT /bbb124", "PUT /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("PUT /aaaa123/bbb124", "PUT /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124", "DELETE /abcde/edcba/aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /bbb124", "DELETE /bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake"),
                    UrlTest("DELETE /aaaa123/bbb124", "DELETE /aaaa123/bbb124", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake")

                    );
            #endregion

            #region NoRecruiters-WFS - Big Test
            NewTestWithUrl("NoRecruiters-WFS - lacks currentUser",
                Types(
                    Type(
                        "RegisterDisplay",
                        BindAttribute("GET /auth/register")
                        ),
                    Type(
                        "Register",
                        Attributes(
                            BindAttribute("POST /auth/register")
                        ),
                        Field("username", "string", FormFieldAttribute, RequestAttribute),
                        Field("email", "string", FormFieldAttribute, RequestAttribute),
                        Field("firstname", "string", FormFieldAttribute, RequestAttribute),
                        Field("lastname", "string", FormFieldAttribute, RequestAttribute),
                        Field("password", "string", FormFieldAttribute, RequestAttribute),

                        Field("defaultContentType", "string", RequestAttribute, RequiresAttribute),

                        Field("errors", "Dictionary<string,string>", RequestAttribute)//Inherited

                        ),
                    Type(
                        "SignInDisplay",
                        BindAttribute("GET /auth/signin?{originalRequest}")
                        ),
                    Type(
                        "SignIn",
                        Attributes(
                            BindAttribute("POST /auth/signin")
                        ),
                        Field("username", "string", FormFieldAttribute, RequestAttribute),
                        Field("originalRequest", "string", FormFieldAttribute, RequestAttribute),

                        Field("password", "string", FormFieldAttribute),
                        Field("defaultContentType", "string", RequestAttribute),

                        Field("errors", "Dictionary<string,string>", RequestAttribute)//Inherited
                        ),
                    Type(
                        "SignOut",
                        BindAttribute("GET /auth/signout")
                        ),
                    Type(
                        "DataAccessControl",
                        Attributes(
                            BindAttribute("/?/byname/{shortName}"),
                            BindAttribute("/?/byId/{postingId}")
                        ),
                //                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),

                            Field("posting", "PostingStub", RequestAttribute)
                        ),
                    Type(
                        "CompanyFunctionAccessControl",
                        BindAttribute("GET /posting/manage"),
                        BindAttribute("GET /posting/ad/applicants/byId/{adId}")
                        ),
                    Type("GeneralFunctionAccessControl",
                        Attributes(
                            BindAttribute("/posting")
                        ),
                        Field("currentUser", "UserProfileStub", RequestAttribute)
                        ),

                    Type("DefaultController",
                        Attributes(
                            BindAttribute("/?")
                        ),
                        Field("userType", "string", RequestAttribute),
                        Field("root", "string", RequestAttribute),
                        Field("defaultContentType", "string", RequestAttribute)
                        ),

                    Type("ClearPreferences",
                        Attributes(
                            BindAttribute("/default/{preferenceReset}")
                        ),
                        Field("preferenceReset", "bool", RequestAttribute),
                        Field("defaultContentType", "string", RequestAttribute, RequiresAttribute)
                        ),

                    Type("Home",
                        Attributes(
                            BindAttribute("/default")
                        ),
                        Field("preferenceReset", "bool", RequestAttribute, DependsOnAttribute),
                        Field("defaultContentType", "string", RequestAttribute, RequiresAttribute)
                        ),

                    Type("Static",
                        Attributes(
                            BindAttribute("/static/{contentId}")
                        ),
                        Field("contentId", "string")
                        ),

                    Type("View",
                        Attributes(
                            BindAttribute("GET /ad/{shortName}"),
                            BindAttribute("GET /resume/{shortName}")
                        ),
                        Field("contentType", "string", RequestAttribute, DependsOnAttribute),
                        Field("posting", "PostingStub", RequestAttribute),
                        Field("defaultContentType", "string", RequestAttribute, RequiresAttribute),
                        Field("shortName", "string")
                        ),

                    Type("Untag",
                        Attributes(
                            BindAttribute("GET /?/without-tag/{tag}")
                        ),
                        Field("tag", "string"),
                        Field("currentTags", "List<string>", SessionAttribute)
                        ),

                    Type("Tag",
                        Attributes(
                            BindAttribute("GET /?/with-tag/{tagList}")
                        ),
                        Field("currentTags", "List<string>", SessionAttribute),
                        Field("tagList", "string")
                        ),
                    Type("FirstTimeSearch",
                        Attributes(
                            BindAttribute("GET /postings/{contentType}?{firstTime}")
                        ),
                        Field("contentType", "string"),
                        Field("firstTime", "bool"),
                        Field("defaultContentType", "string", RequestAttribute, RequiresAttribute)
                        ),
                    Type("Search",
                        Attributes(
                            BindAttribute("/postings/{contentType}")
                        ),
                        Field("txtQuery", "string", FormFieldAttribute),
                        Field("currentTags", "List<string>", SessionAttribute, DependsOnAttribute),
                        Field("popularTags", "List<string>", RequestAttribute),
                        Field("searchResults", "List<string>", RequestAttribute),
                        Field("contentType", "string", RequestAttribute)
                        ),

                    Type("Flag",
                        Attributes(
                            BindAttribute("/posting/flag/{contentType}/{flagType}/{shortName}")
                        ),
                        Field("flagType", "string"),
                        Field("contentType", "string"),
                        Field("shortName", "string")//,
                //                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute)
                        ),

                    Type("ApplyDisplay",
                        Attributes(
                            BindAttribute("GET /posting/apply/{appContentType}/{shortName}")
                        ),
                        Field("appContentType", "string", RequestAttribute),
                        Field("shortName", "string", RequestAttribute)
                        ),

                    Type("ApplyController",
                        Attributes(
                            BindAttribute("POST /posting/apply/{appContentType}/{shortName}")
                        ),
                        Field("appContentType", "string", RequestAttribute),
                        Field("shortName", "string", RequestAttribute),
                        Field("comment", "string", RequestAttribute, FormFieldAttribute)//,
                //                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute)
                        ),

                    Type("ResumeDisplay",
                        Attributes(
                            BindAttribute("GET /posting/resume/byname/{shortName}")
                        ),
                        Field("shortName", "string"),
                //                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),
                        Field("posting", "PostingStub", RequestAttribute, RequiresAttribute)
                        ),
                    Type("ResumeUpdate",
                        Attributes(
                            BindAttribute("POST /posting/resume/byname/{shortName}")
                        ),
                        Field("shortName", "string"),
                //                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),
                        Field("posting", "PostingStub", RequestAttribute, RequiresAttribute),

                        Field("heading", "string", FormFieldAttribute, RequestAttribute),
                        Field("tags", "string", FormFieldAttribute, RequestAttribute),
                        Field("detail", "string", FormFieldAttribute, RequestAttribute),
                        Field("published", "string", FormFieldAttribute, RequestAttribute)
                        ),
                    Type("PreviewDisplay",
                        Attributes(
                            BindAttribute("GET /posting/resume/preview/byname/{shortName}")
                        ),
                        Field("shortName", "string"),
                //                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),
                        Field("posting", "PostingStub", RequestAttribute, RequiresAttribute)
                        ),
                    Type("Manage",
                        Attributes(
                            BindAttribute("GET /posting/manage")
                        ),
                        Field("unpublished", "List<string>", RequestAttribute),
                        Field("published", "List<string>", RequestAttribute)//,

//                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute)
                        ),
                    Type("ViewAllApplicatints",
                        Attributes(
                            BindAttribute("GET /posting/ad/applicants/byId/{adId}")
                        ),
                        Field("adId", "string", RequestAttribute),
                //                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),
                        Field("applicants", "List<string>", RequestAttribute)
                        ),
                    Type("AdDisplay",
                        Attributes(
                            BindAttribute("GET /posting/ad/byname/{shortName}")
                        ),
                        Field("shortName", "string"),
                //                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),
                        Field("posting", "PostingStub", RequestAttribute, RequiresAttribute)
                        ),
                    Type("AdUpdate",
                        Attributes(
                            BindAttribute("POST /posting/ad/byname/{shortName}")
                        ),
                        Field("shortName", "string"),
                //                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),
                        Field("posting", "PostingStub", RequestAttribute, RequiresAttribute),

                        Field("heading", "string", FormFieldAttribute, RequestAttribute),
                        Field("tags", "string", FormFieldAttribute, RequestAttribute),
                        Field("detail", "string", FormFieldAttribute, RequestAttribute),
                        Field("published", "string", FormFieldAttribute, RequestAttribute)
                        )

                    ),
                    UrlTest("GET /auth/register", "GET /auth/register", "DefaultController", "RegisterDisplay"),
                    UrlTest("POST /auth/register", "POST /auth/register", "DefaultController", "Register"),
                    UrlTest("GET /auth/signin?{originalRequest}", "GET /auth/signin?{originalRequest}", "DefaultController", "SignInDisplay"),
                    UrlTest("POST /auth/signin", "POST /auth/signin", "DefaultController", "SignIn"),
                    UrlTest("GET /auth/signout", "GET /auth/signout", "DefaultController", "SignOut"),
                    UrlTest("GET /byname/", "GET /byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /byname/variablevalue1", "GET /byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /byname/123412423", "GET /byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /byname/testvalue", "GET /byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/byname/", "GET /abcde/edcba/aaaa123/bbb124/byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/byname/variablevalue1", "GET /abcde/edcba/aaaa123/bbb124/byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/byname/123412423", "GET /abcde/edcba/aaaa123/bbb124/byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/byname/testvalue", "GET /abcde/edcba/aaaa123/bbb124/byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /bbb124/byname/", "GET /bbb124/byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /bbb124/byname/variablevalue1", "GET /bbb124/byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /bbb124/byname/123412423", "GET /bbb124/byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /bbb124/byname/testvalue", "GET /bbb124/byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /aaaa123/bbb124/byname/", "GET /aaaa123/bbb124/byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /aaaa123/bbb124/byname/variablevalue1", "GET /aaaa123/bbb124/byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /aaaa123/bbb124/byname/123412423", "GET /aaaa123/bbb124/byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /aaaa123/bbb124/byname/testvalue", "GET /aaaa123/bbb124/byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /byname/", "POST /byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /byname/variablevalue1", "POST /byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /byname/123412423", "POST /byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /byname/testvalue", "POST /byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124/byname/", "POST /abcde/edcba/aaaa123/bbb124/byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124/byname/variablevalue1", "POST /abcde/edcba/aaaa123/bbb124/byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124/byname/123412423", "POST /abcde/edcba/aaaa123/bbb124/byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124/byname/testvalue", "POST /abcde/edcba/aaaa123/bbb124/byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /bbb124/byname/", "POST /bbb124/byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /bbb124/byname/variablevalue1", "POST /bbb124/byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /bbb124/byname/123412423", "POST /bbb124/byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /bbb124/byname/testvalue", "POST /bbb124/byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /aaaa123/bbb124/byname/", "POST /aaaa123/bbb124/byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /aaaa123/bbb124/byname/variablevalue1", "POST /aaaa123/bbb124/byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /aaaa123/bbb124/byname/123412423", "POST /aaaa123/bbb124/byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /aaaa123/bbb124/byname/testvalue", "POST /aaaa123/bbb124/byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /byname/", "PUT /byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /byname/variablevalue1", "PUT /byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /byname/123412423", "PUT /byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /byname/testvalue", "PUT /byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124/byname/", "PUT /abcde/edcba/aaaa123/bbb124/byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124/byname/variablevalue1", "PUT /abcde/edcba/aaaa123/bbb124/byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124/byname/123412423", "PUT /abcde/edcba/aaaa123/bbb124/byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124/byname/testvalue", "PUT /abcde/edcba/aaaa123/bbb124/byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /bbb124/byname/", "PUT /bbb124/byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /bbb124/byname/variablevalue1", "PUT /bbb124/byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /bbb124/byname/123412423", "PUT /bbb124/byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /bbb124/byname/testvalue", "PUT /bbb124/byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /aaaa123/bbb124/byname/", "PUT /aaaa123/bbb124/byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /aaaa123/bbb124/byname/variablevalue1", "PUT /aaaa123/bbb124/byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /aaaa123/bbb124/byname/123412423", "PUT /aaaa123/bbb124/byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /aaaa123/bbb124/byname/testvalue", "PUT /aaaa123/bbb124/byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /byname/", "DELETE /byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /byname/variablevalue1", "DELETE /byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /byname/123412423", "DELETE /byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /byname/testvalue", "DELETE /byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124/byname/", "DELETE /abcde/edcba/aaaa123/bbb124/byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124/byname/variablevalue1", "DELETE /abcde/edcba/aaaa123/bbb124/byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124/byname/123412423", "DELETE /abcde/edcba/aaaa123/bbb124/byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124/byname/testvalue", "DELETE /abcde/edcba/aaaa123/bbb124/byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /bbb124/byname/", "DELETE /bbb124/byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /bbb124/byname/variablevalue1", "DELETE /bbb124/byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /bbb124/byname/123412423", "DELETE /bbb124/byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /bbb124/byname/testvalue", "DELETE /bbb124/byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /aaaa123/bbb124/byname/", "DELETE /aaaa123/bbb124/byname/", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /aaaa123/bbb124/byname/variablevalue1", "DELETE /aaaa123/bbb124/byname/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /aaaa123/bbb124/byname/123412423", "DELETE /aaaa123/bbb124/byname/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /aaaa123/bbb124/byname/testvalue", "DELETE /aaaa123/bbb124/byname/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /byId/", "GET /byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /byId/variablevalue1", "GET /byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /byId/123412423", "GET /byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /byId/testvalue", "GET /byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/byId/", "GET /abcde/edcba/aaaa123/bbb124/byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/byId/variablevalue1", "GET /abcde/edcba/aaaa123/bbb124/byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/byId/123412423", "GET /abcde/edcba/aaaa123/bbb124/byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/byId/testvalue", "GET /abcde/edcba/aaaa123/bbb124/byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /bbb124/byId/", "GET /bbb124/byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /bbb124/byId/variablevalue1", "GET /bbb124/byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /bbb124/byId/123412423", "GET /bbb124/byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /bbb124/byId/testvalue", "GET /bbb124/byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /aaaa123/bbb124/byId/", "GET /aaaa123/bbb124/byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /aaaa123/bbb124/byId/variablevalue1", "GET /aaaa123/bbb124/byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /aaaa123/bbb124/byId/123412423", "GET /aaaa123/bbb124/byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /aaaa123/bbb124/byId/testvalue", "GET /aaaa123/bbb124/byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /byId/", "POST /byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /byId/variablevalue1", "POST /byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /byId/123412423", "POST /byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /byId/testvalue", "POST /byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124/byId/", "POST /abcde/edcba/aaaa123/bbb124/byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124/byId/variablevalue1", "POST /abcde/edcba/aaaa123/bbb124/byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124/byId/123412423", "POST /abcde/edcba/aaaa123/bbb124/byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124/byId/testvalue", "POST /abcde/edcba/aaaa123/bbb124/byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /bbb124/byId/", "POST /bbb124/byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /bbb124/byId/variablevalue1", "POST /bbb124/byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /bbb124/byId/123412423", "POST /bbb124/byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /bbb124/byId/testvalue", "POST /bbb124/byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /aaaa123/bbb124/byId/", "POST /aaaa123/bbb124/byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /aaaa123/bbb124/byId/variablevalue1", "POST /aaaa123/bbb124/byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /aaaa123/bbb124/byId/123412423", "POST /aaaa123/bbb124/byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("POST /aaaa123/bbb124/byId/testvalue", "POST /aaaa123/bbb124/byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /byId/", "PUT /byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /byId/variablevalue1", "PUT /byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /byId/123412423", "PUT /byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /byId/testvalue", "PUT /byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124/byId/", "PUT /abcde/edcba/aaaa123/bbb124/byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124/byId/variablevalue1", "PUT /abcde/edcba/aaaa123/bbb124/byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124/byId/123412423", "PUT /abcde/edcba/aaaa123/bbb124/byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124/byId/testvalue", "PUT /abcde/edcba/aaaa123/bbb124/byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /bbb124/byId/", "PUT /bbb124/byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /bbb124/byId/variablevalue1", "PUT /bbb124/byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /bbb124/byId/123412423", "PUT /bbb124/byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /bbb124/byId/testvalue", "PUT /bbb124/byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /aaaa123/bbb124/byId/", "PUT /aaaa123/bbb124/byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /aaaa123/bbb124/byId/variablevalue1", "PUT /aaaa123/bbb124/byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /aaaa123/bbb124/byId/123412423", "PUT /aaaa123/bbb124/byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("PUT /aaaa123/bbb124/byId/testvalue", "PUT /aaaa123/bbb124/byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /byId/", "DELETE /byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /byId/variablevalue1", "DELETE /byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /byId/123412423", "DELETE /byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /byId/testvalue", "DELETE /byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124/byId/", "DELETE /abcde/edcba/aaaa123/bbb124/byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124/byId/variablevalue1", "DELETE /abcde/edcba/aaaa123/bbb124/byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124/byId/123412423", "DELETE /abcde/edcba/aaaa123/bbb124/byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124/byId/testvalue", "DELETE /abcde/edcba/aaaa123/bbb124/byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /bbb124/byId/", "DELETE /bbb124/byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /bbb124/byId/variablevalue1", "DELETE /bbb124/byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /bbb124/byId/123412423", "DELETE /bbb124/byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /bbb124/byId/testvalue", "DELETE /bbb124/byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /aaaa123/bbb124/byId/", "DELETE /aaaa123/bbb124/byId/", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /aaaa123/bbb124/byId/variablevalue1", "DELETE /aaaa123/bbb124/byId/variablevalue1", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /aaaa123/bbb124/byId/123412423", "DELETE /aaaa123/bbb124/byId/123412423", "DefaultController", "DataAccessControl"),
                    UrlTest("DELETE /aaaa123/bbb124/byId/testvalue", "DELETE /aaaa123/bbb124/byId/testvalue", "DefaultController", "DataAccessControl"),
                    UrlTest("GET /posting/manage", "GET /posting/manage", "GeneralFunctionAccessControl", "DefaultController", "CompanyFunctionAccessControl", "Manage"),
                    UrlTest("GET /posting/ad/applicants/byId/", "GET /posting/ad/applicants/byId/", "GeneralFunctionAccessControl", "DefaultController", "DataAccessControl", "CompanyFunctionAccessControl", "ViewAllApplicatints"),
                    UrlTest("GET /posting/ad/applicants/byId/variablevalue1", "GET /posting/ad/applicants/byId/variablevalue1", "GeneralFunctionAccessControl", "DefaultController", "DataAccessControl", "CompanyFunctionAccessControl", "ViewAllApplicatints"),
                    UrlTest("GET /posting/ad/applicants/byId/123412423", "GET /posting/ad/applicants/byId/123412423", "GeneralFunctionAccessControl", "DefaultController", "DataAccessControl", "CompanyFunctionAccessControl", "ViewAllApplicatints"),
                    UrlTest("GET /posting/ad/applicants/byId/testvalue", "GET /posting/ad/applicants/byId/testvalue", "GeneralFunctionAccessControl", "DefaultController", "DataAccessControl", "CompanyFunctionAccessControl", "ViewAllApplicatints"),
                    UrlTest("GET /posting", "GET /posting", "DefaultController", "GeneralFunctionAccessControl"),
                    UrlTest("POST /posting", "POST /posting", "DefaultController", "GeneralFunctionAccessControl"),
                    UrlTest("PUT /posting", "PUT /posting", "DefaultController", "GeneralFunctionAccessControl"),
                    UrlTest("DELETE /posting", "DELETE /posting", "DefaultController", "GeneralFunctionAccessControl"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124", "GET /abcde/edcba/aaaa123/bbb124", "DefaultController"),
                    UrlTest("GET /bbb124", "GET /bbb124", "DefaultController"),
                    UrlTest("GET /aaaa123/bbb124", "GET /aaaa123/bbb124", "DefaultController"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124", "POST /abcde/edcba/aaaa123/bbb124", "DefaultController"),
                    UrlTest("POST /bbb124", "POST /bbb124", "DefaultController"),
                    UrlTest("POST /aaaa123/bbb124", "POST /aaaa123/bbb124", "DefaultController"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124", "PUT /abcde/edcba/aaaa123/bbb124", "DefaultController"),
                    UrlTest("PUT /bbb124", "PUT /bbb124", "DefaultController"),
                    UrlTest("PUT /aaaa123/bbb124", "PUT /aaaa123/bbb124", "DefaultController"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124", "DELETE /abcde/edcba/aaaa123/bbb124", "DefaultController"),
                    UrlTest("DELETE /bbb124", "DELETE /bbb124", "DefaultController"),
                    UrlTest("DELETE /aaaa123/bbb124", "DELETE /aaaa123/bbb124", "DefaultController"),
                    UrlTest("GET /default/", "GET /default/", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("GET /default/variablevalue1", "GET /default/variablevalue1", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("GET /default/123412423", "GET /default/123412423", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("GET /default/testvalue", "GET /default/testvalue", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("POST /default/", "POST /default/", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("POST /default/variablevalue1", "POST /default/variablevalue1", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("POST /default/123412423", "POST /default/123412423", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("POST /default/testvalue", "POST /default/testvalue", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("PUT /default/", "PUT /default/", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("PUT /default/variablevalue1", "PUT /default/variablevalue1", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("PUT /default/123412423", "PUT /default/123412423", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("PUT /default/testvalue", "PUT /default/testvalue", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("DELETE /default/", "DELETE /default/", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("DELETE /default/variablevalue1", "DELETE /default/variablevalue1", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("DELETE /default/123412423", "DELETE /default/123412423", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("DELETE /default/testvalue", "DELETE /default/testvalue", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("GET /default", "GET /default", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("POST /default", "POST /default", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("PUT /default", "PUT /default", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("DELETE /default", "DELETE /default", "DefaultController", "ClearPreferences", "Home"),
                    UrlTest("GET /static/", "GET /static/", "DefaultController", "Static"),
                    UrlTest("GET /static/variablevalue1", "GET /static/variablevalue1", "DefaultController", "Static"),
                    UrlTest("GET /static/123412423", "GET /static/123412423", "DefaultController", "Static"),
                    UrlTest("GET /static/testvalue", "GET /static/testvalue", "DefaultController", "Static"),
                    UrlTest("POST /static/", "POST /static/", "DefaultController", "Static"),
                    UrlTest("POST /static/variablevalue1", "POST /static/variablevalue1", "DefaultController", "Static"),
                    UrlTest("POST /static/123412423", "POST /static/123412423", "DefaultController", "Static"),
                    UrlTest("POST /static/testvalue", "POST /static/testvalue", "DefaultController", "Static"),
                    UrlTest("PUT /static/", "PUT /static/", "DefaultController", "Static"),
                    UrlTest("PUT /static/variablevalue1", "PUT /static/variablevalue1", "DefaultController", "Static"),
                    UrlTest("PUT /static/123412423", "PUT /static/123412423", "DefaultController", "Static"),
                    UrlTest("PUT /static/testvalue", "PUT /static/testvalue", "DefaultController", "Static"),
                    UrlTest("DELETE /static/", "DELETE /static/", "DefaultController", "Static"),
                    UrlTest("DELETE /static/variablevalue1", "DELETE /static/variablevalue1", "DefaultController", "Static"),
                    UrlTest("DELETE /static/123412423", "DELETE /static/123412423", "DefaultController", "Static"),
                    UrlTest("DELETE /static/testvalue", "DELETE /static/testvalue", "DefaultController", "Static"),
                    UrlTest("GET /ad/", "GET /ad/", "DefaultController", "View"),
                    UrlTest("GET /ad/variablevalue1", "GET /ad/variablevalue1", "DefaultController", "View"),
                    UrlTest("GET /ad/123412423", "GET /ad/123412423", "DefaultController", "View"),
                    UrlTest("GET /ad/testvalue", "GET /ad/testvalue", "DefaultController", "View"),
                    UrlTest("GET /resume/", "GET /resume/", "DefaultController", "View"),
                    UrlTest("GET /resume/variablevalue1", "GET /resume/variablevalue1", "DefaultController", "View"),
                    UrlTest("GET /resume/123412423", "GET /resume/123412423", "DefaultController", "View"),
                    UrlTest("GET /resume/testvalue", "GET /resume/testvalue", "DefaultController", "View"),
                    UrlTest("GET /without-tag/", "GET /without-tag/", "DefaultController", "Untag"),
                    UrlTest("GET /without-tag/variablevalue1", "GET /without-tag/variablevalue1", "DefaultController", "Untag"),
                    UrlTest("GET /without-tag/123412423", "GET /without-tag/123412423", "DefaultController", "Untag"),
                    UrlTest("GET /without-tag/testvalue", "GET /without-tag/testvalue", "DefaultController", "Untag"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/without-tag/", "GET /abcde/edcba/aaaa123/bbb124/without-tag/", "DefaultController", "Untag"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/without-tag/variablevalue1", "GET /abcde/edcba/aaaa123/bbb124/without-tag/variablevalue1", "DefaultController", "Untag"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/without-tag/123412423", "GET /abcde/edcba/aaaa123/bbb124/without-tag/123412423", "DefaultController", "Untag"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/without-tag/testvalue", "GET /abcde/edcba/aaaa123/bbb124/without-tag/testvalue", "DefaultController", "Untag"),
                    UrlTest("GET /bbb124/without-tag/", "GET /bbb124/without-tag/", "DefaultController", "Untag"),
                    UrlTest("GET /bbb124/without-tag/variablevalue1", "GET /bbb124/without-tag/variablevalue1", "DefaultController", "Untag"),
                    UrlTest("GET /bbb124/without-tag/123412423", "GET /bbb124/without-tag/123412423", "DefaultController", "Untag"),
                    UrlTest("GET /bbb124/without-tag/testvalue", "GET /bbb124/without-tag/testvalue", "DefaultController", "Untag"),
                    UrlTest("GET /aaaa123/bbb124/without-tag/", "GET /aaaa123/bbb124/without-tag/", "DefaultController", "Untag"),
                    UrlTest("GET /aaaa123/bbb124/without-tag/variablevalue1", "GET /aaaa123/bbb124/without-tag/variablevalue1", "DefaultController", "Untag"),
                    UrlTest("GET /aaaa123/bbb124/without-tag/123412423", "GET /aaaa123/bbb124/without-tag/123412423", "DefaultController", "Untag"),
                    UrlTest("GET /aaaa123/bbb124/without-tag/testvalue", "GET /aaaa123/bbb124/without-tag/testvalue", "DefaultController", "Untag"),
                    UrlTest("GET /with-tag/", "GET /with-tag/", "DefaultController", "Tag"),
                    UrlTest("GET /with-tag/variablevalue1", "GET /with-tag/variablevalue1", "DefaultController", "Tag"),
                    UrlTest("GET /with-tag/123412423", "GET /with-tag/123412423", "DefaultController", "Tag"),
                    UrlTest("GET /with-tag/testvalue", "GET /with-tag/testvalue", "DefaultController", "Tag"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/with-tag/", "GET /abcde/edcba/aaaa123/bbb124/with-tag/", "DefaultController", "Tag"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/with-tag/variablevalue1", "GET /abcde/edcba/aaaa123/bbb124/with-tag/variablevalue1", "DefaultController", "Tag"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/with-tag/123412423", "GET /abcde/edcba/aaaa123/bbb124/with-tag/123412423", "DefaultController", "Tag"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124/with-tag/testvalue", "GET /abcde/edcba/aaaa123/bbb124/with-tag/testvalue", "DefaultController", "Tag"),
                    UrlTest("GET /bbb124/with-tag/", "GET /bbb124/with-tag/", "DefaultController", "Tag"),
                    UrlTest("GET /bbb124/with-tag/variablevalue1", "GET /bbb124/with-tag/variablevalue1", "DefaultController", "Tag"),
                    UrlTest("GET /bbb124/with-tag/123412423", "GET /bbb124/with-tag/123412423", "DefaultController", "Tag"),
                    UrlTest("GET /bbb124/with-tag/testvalue", "GET /bbb124/with-tag/testvalue", "DefaultController", "Tag"),
                    UrlTest("GET /aaaa123/bbb124/with-tag/", "GET /aaaa123/bbb124/with-tag/", "DefaultController", "Tag"),
                    UrlTest("GET /aaaa123/bbb124/with-tag/variablevalue1", "GET /aaaa123/bbb124/with-tag/variablevalue1", "DefaultController", "Tag"),
                    UrlTest("GET /aaaa123/bbb124/with-tag/123412423", "GET /aaaa123/bbb124/with-tag/123412423", "DefaultController", "Tag"),
                    UrlTest("GET /aaaa123/bbb124/with-tag/testvalue", "GET /aaaa123/bbb124/with-tag/testvalue", "DefaultController", "Tag"),
                    UrlTest("GET /postings{firstTime}", "GET /postings{firstTime}", "DefaultController"),
                    UrlTest("GET /postings/abcde/edcba/aaaa123/bbb124{firstTime}", "GET /postings/abcde/edcba/aaaa123/bbb124{firstTime}", "DefaultController", "Search", "FirstTimeSearch"),
                    UrlTest("GET /postings/bbb124{firstTime}", "GET /postings/bbb124{firstTime}", "DefaultController", "Search", "FirstTimeSearch"),
                    UrlTest("GET /postings/aaaa123/bbb124{firstTime}", "GET /postings/aaaa123/bbb124{firstTime}", "DefaultController", "Search", "FirstTimeSearch"),
                    UrlTest("GET /postings/variablevalue1?{firstTime}", "GET /postings/variablevalue1?{firstTime}", "DefaultController", "Search", "FirstTimeSearch"),
                    UrlTest("GET /postings/123412423?{firstTime}", "GET /postings/123412423?{firstTime}", "DefaultController", "Search", "FirstTimeSearch"),
                    UrlTest("GET /postings/testvalue?{firstTime}", "GET /postings/testvalue?{firstTime}", "DefaultController", "Search", "FirstTimeSearch"),
                    UrlTest("GET /postings/", "GET /postings/", "DefaultController", "Search", "FirstTimeSearch"),
                    UrlTest("GET /postings/variablevalue1", "GET /postings/variablevalue1", "DefaultController", "Search", "FirstTimeSearch"),
                    UrlTest("GET /postings/123412423", "GET /postings/123412423", "DefaultController", "Search", "FirstTimeSearch"),
                    UrlTest("GET /postings/testvalue", "GET /postings/testvalue", "DefaultController", "Search", "FirstTimeSearch"),
                    UrlTest("POST /postings/", "POST /postings/", "DefaultController", "Search"),
                    UrlTest("POST /postings/variablevalue1", "POST /postings/variablevalue1", "DefaultController", "Search"),
                    UrlTest("POST /postings/123412423", "POST /postings/123412423", "DefaultController", "Search"),
                    UrlTest("POST /postings/testvalue", "POST /postings/testvalue", "DefaultController", "Search"),
                    UrlTest("PUT /postings/", "PUT /postings/", "DefaultController", "Search"),
                    UrlTest("PUT /postings/variablevalue1", "PUT /postings/variablevalue1", "DefaultController", "Search"),
                    UrlTest("PUT /postings/123412423", "PUT /postings/123412423", "DefaultController", "Search"),
                    UrlTest("PUT /postings/testvalue", "PUT /postings/testvalue", "DefaultController", "Search"),
                    UrlTest("DELETE /postings/", "DELETE /postings/", "DefaultController", "Search"),
                    UrlTest("DELETE /postings/variablevalue1", "DELETE /postings/variablevalue1", "DefaultController", "Search"),
                    UrlTest("DELETE /postings/123412423", "DELETE /postings/123412423", "DefaultController", "Search"),
                    UrlTest("DELETE /postings/testvalue", "DELETE /postings/testvalue", "DefaultController", "Search"),
                    UrlTest("GET /posting/flag///", "GET /posting/flag///", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag///variablevalue1", "GET /posting/flag///variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag///123412423", "GET /posting/flag///123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag///testvalue", "GET /posting/flag///testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag//variablevalue1/", "GET /posting/flag//variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag//variablevalue1/variablevalue1", "GET /posting/flag//variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag//variablevalue1/123412423", "GET /posting/flag//variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag//variablevalue1/testvalue", "GET /posting/flag//variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag//123412423/", "GET /posting/flag//123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag//123412423/variablevalue1", "GET /posting/flag//123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag//123412423/123412423", "GET /posting/flag//123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag//123412423/testvalue", "GET /posting/flag//123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag//testvalue/", "GET /posting/flag//testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag//testvalue/variablevalue1", "GET /posting/flag//testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag//testvalue/123412423", "GET /posting/flag//testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag//testvalue/testvalue", "GET /posting/flag//testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1//", "GET /posting/flag/variablevalue1//", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1//variablevalue1", "GET /posting/flag/variablevalue1//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1//123412423", "GET /posting/flag/variablevalue1//123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1//testvalue", "GET /posting/flag/variablevalue1//testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1/variablevalue1/", "GET /posting/flag/variablevalue1/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1/variablevalue1/variablevalue1", "GET /posting/flag/variablevalue1/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1/variablevalue1/123412423", "GET /posting/flag/variablevalue1/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1/variablevalue1/testvalue", "GET /posting/flag/variablevalue1/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1/123412423/", "GET /posting/flag/variablevalue1/123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1/123412423/variablevalue1", "GET /posting/flag/variablevalue1/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1/123412423/123412423", "GET /posting/flag/variablevalue1/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1/123412423/testvalue", "GET /posting/flag/variablevalue1/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1/testvalue/", "GET /posting/flag/variablevalue1/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1/testvalue/variablevalue1", "GET /posting/flag/variablevalue1/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1/testvalue/123412423", "GET /posting/flag/variablevalue1/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/variablevalue1/testvalue/testvalue", "GET /posting/flag/variablevalue1/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423//", "GET /posting/flag/123412423//", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423//variablevalue1", "GET /posting/flag/123412423//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423//123412423", "GET /posting/flag/123412423//123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423//testvalue", "GET /posting/flag/123412423//testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423/variablevalue1/", "GET /posting/flag/123412423/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423/variablevalue1/variablevalue1", "GET /posting/flag/123412423/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423/variablevalue1/123412423", "GET /posting/flag/123412423/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423/variablevalue1/testvalue", "GET /posting/flag/123412423/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423/123412423/", "GET /posting/flag/123412423/123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423/123412423/variablevalue1", "GET /posting/flag/123412423/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423/123412423/123412423", "GET /posting/flag/123412423/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423/123412423/testvalue", "GET /posting/flag/123412423/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423/testvalue/", "GET /posting/flag/123412423/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423/testvalue/variablevalue1", "GET /posting/flag/123412423/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423/testvalue/123412423", "GET /posting/flag/123412423/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/123412423/testvalue/testvalue", "GET /posting/flag/123412423/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue//", "GET /posting/flag/testvalue//", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue//variablevalue1", "GET /posting/flag/testvalue//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue//123412423", "GET /posting/flag/testvalue//123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue//testvalue", "GET /posting/flag/testvalue//testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue/variablevalue1/", "GET /posting/flag/testvalue/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue/variablevalue1/variablevalue1", "GET /posting/flag/testvalue/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue/variablevalue1/123412423", "GET /posting/flag/testvalue/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue/variablevalue1/testvalue", "GET /posting/flag/testvalue/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue/123412423/", "GET /posting/flag/testvalue/123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue/123412423/variablevalue1", "GET /posting/flag/testvalue/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue/123412423/123412423", "GET /posting/flag/testvalue/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue/123412423/testvalue", "GET /posting/flag/testvalue/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue/testvalue/", "GET /posting/flag/testvalue/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue/testvalue/variablevalue1", "GET /posting/flag/testvalue/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue/testvalue/123412423", "GET /posting/flag/testvalue/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/flag/testvalue/testvalue/testvalue", "GET /posting/flag/testvalue/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag///", "POST /posting/flag///", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag///variablevalue1", "POST /posting/flag///variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag///123412423", "POST /posting/flag///123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag///testvalue", "POST /posting/flag///testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag//variablevalue1/", "POST /posting/flag//variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag//variablevalue1/variablevalue1", "POST /posting/flag//variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag//variablevalue1/123412423", "POST /posting/flag//variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag//variablevalue1/testvalue", "POST /posting/flag//variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag//123412423/", "POST /posting/flag//123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag//123412423/variablevalue1", "POST /posting/flag//123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag//123412423/123412423", "POST /posting/flag//123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag//123412423/testvalue", "POST /posting/flag//123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag//testvalue/", "POST /posting/flag//testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag//testvalue/variablevalue1", "POST /posting/flag//testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag//testvalue/123412423", "POST /posting/flag//testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag//testvalue/testvalue", "POST /posting/flag//testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1//", "POST /posting/flag/variablevalue1//", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1//variablevalue1", "POST /posting/flag/variablevalue1//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1//123412423", "POST /posting/flag/variablevalue1//123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1//testvalue", "POST /posting/flag/variablevalue1//testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1/variablevalue1/", "POST /posting/flag/variablevalue1/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1/variablevalue1/variablevalue1", "POST /posting/flag/variablevalue1/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1/variablevalue1/123412423", "POST /posting/flag/variablevalue1/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1/variablevalue1/testvalue", "POST /posting/flag/variablevalue1/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1/123412423/", "POST /posting/flag/variablevalue1/123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1/123412423/variablevalue1", "POST /posting/flag/variablevalue1/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1/123412423/123412423", "POST /posting/flag/variablevalue1/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1/123412423/testvalue", "POST /posting/flag/variablevalue1/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1/testvalue/", "POST /posting/flag/variablevalue1/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1/testvalue/variablevalue1", "POST /posting/flag/variablevalue1/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1/testvalue/123412423", "POST /posting/flag/variablevalue1/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/variablevalue1/testvalue/testvalue", "POST /posting/flag/variablevalue1/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423//", "POST /posting/flag/123412423//", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423//variablevalue1", "POST /posting/flag/123412423//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423//123412423", "POST /posting/flag/123412423//123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423//testvalue", "POST /posting/flag/123412423//testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423/variablevalue1/", "POST /posting/flag/123412423/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423/variablevalue1/variablevalue1", "POST /posting/flag/123412423/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423/variablevalue1/123412423", "POST /posting/flag/123412423/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423/variablevalue1/testvalue", "POST /posting/flag/123412423/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423/123412423/", "POST /posting/flag/123412423/123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423/123412423/variablevalue1", "POST /posting/flag/123412423/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423/123412423/123412423", "POST /posting/flag/123412423/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423/123412423/testvalue", "POST /posting/flag/123412423/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423/testvalue/", "POST /posting/flag/123412423/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423/testvalue/variablevalue1", "POST /posting/flag/123412423/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423/testvalue/123412423", "POST /posting/flag/123412423/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/123412423/testvalue/testvalue", "POST /posting/flag/123412423/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue//", "POST /posting/flag/testvalue//", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue//variablevalue1", "POST /posting/flag/testvalue//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue//123412423", "POST /posting/flag/testvalue//123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue//testvalue", "POST /posting/flag/testvalue//testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue/variablevalue1/", "POST /posting/flag/testvalue/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue/variablevalue1/variablevalue1", "POST /posting/flag/testvalue/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue/variablevalue1/123412423", "POST /posting/flag/testvalue/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue/variablevalue1/testvalue", "POST /posting/flag/testvalue/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue/123412423/", "POST /posting/flag/testvalue/123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue/123412423/variablevalue1", "POST /posting/flag/testvalue/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue/123412423/123412423", "POST /posting/flag/testvalue/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue/123412423/testvalue", "POST /posting/flag/testvalue/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue/testvalue/", "POST /posting/flag/testvalue/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue/testvalue/variablevalue1", "POST /posting/flag/testvalue/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue/testvalue/123412423", "POST /posting/flag/testvalue/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("POST /posting/flag/testvalue/testvalue/testvalue", "POST /posting/flag/testvalue/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag///", "PUT /posting/flag///", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag///variablevalue1", "PUT /posting/flag///variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag///123412423", "PUT /posting/flag///123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag///testvalue", "PUT /posting/flag///testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag//variablevalue1/", "PUT /posting/flag//variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag//variablevalue1/variablevalue1", "PUT /posting/flag//variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag//variablevalue1/123412423", "PUT /posting/flag//variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag//variablevalue1/testvalue", "PUT /posting/flag//variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag//123412423/", "PUT /posting/flag//123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag//123412423/variablevalue1", "PUT /posting/flag//123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag//123412423/123412423", "PUT /posting/flag//123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag//123412423/testvalue", "PUT /posting/flag//123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag//testvalue/", "PUT /posting/flag//testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag//testvalue/variablevalue1", "PUT /posting/flag//testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag//testvalue/123412423", "PUT /posting/flag//testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag//testvalue/testvalue", "PUT /posting/flag//testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1//", "PUT /posting/flag/variablevalue1//", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1//variablevalue1", "PUT /posting/flag/variablevalue1//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1//123412423", "PUT /posting/flag/variablevalue1//123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1//testvalue", "PUT /posting/flag/variablevalue1//testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1/variablevalue1/", "PUT /posting/flag/variablevalue1/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1/variablevalue1/variablevalue1", "PUT /posting/flag/variablevalue1/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1/variablevalue1/123412423", "PUT /posting/flag/variablevalue1/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1/variablevalue1/testvalue", "PUT /posting/flag/variablevalue1/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1/123412423/", "PUT /posting/flag/variablevalue1/123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1/123412423/variablevalue1", "PUT /posting/flag/variablevalue1/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1/123412423/123412423", "PUT /posting/flag/variablevalue1/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1/123412423/testvalue", "PUT /posting/flag/variablevalue1/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1/testvalue/", "PUT /posting/flag/variablevalue1/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1/testvalue/variablevalue1", "PUT /posting/flag/variablevalue1/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1/testvalue/123412423", "PUT /posting/flag/variablevalue1/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/variablevalue1/testvalue/testvalue", "PUT /posting/flag/variablevalue1/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423//", "PUT /posting/flag/123412423//", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423//variablevalue1", "PUT /posting/flag/123412423//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423//123412423", "PUT /posting/flag/123412423//123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423//testvalue", "PUT /posting/flag/123412423//testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423/variablevalue1/", "PUT /posting/flag/123412423/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423/variablevalue1/variablevalue1", "PUT /posting/flag/123412423/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423/variablevalue1/123412423", "PUT /posting/flag/123412423/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423/variablevalue1/testvalue", "PUT /posting/flag/123412423/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423/123412423/", "PUT /posting/flag/123412423/123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423/123412423/variablevalue1", "PUT /posting/flag/123412423/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423/123412423/123412423", "PUT /posting/flag/123412423/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423/123412423/testvalue", "PUT /posting/flag/123412423/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423/testvalue/", "PUT /posting/flag/123412423/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423/testvalue/variablevalue1", "PUT /posting/flag/123412423/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423/testvalue/123412423", "PUT /posting/flag/123412423/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/123412423/testvalue/testvalue", "PUT /posting/flag/123412423/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue//", "PUT /posting/flag/testvalue//", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue//variablevalue1", "PUT /posting/flag/testvalue//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue//123412423", "PUT /posting/flag/testvalue//123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue//testvalue", "PUT /posting/flag/testvalue//testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue/variablevalue1/", "PUT /posting/flag/testvalue/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue/variablevalue1/variablevalue1", "PUT /posting/flag/testvalue/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue/variablevalue1/123412423", "PUT /posting/flag/testvalue/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue/variablevalue1/testvalue", "PUT /posting/flag/testvalue/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue/123412423/", "PUT /posting/flag/testvalue/123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue/123412423/variablevalue1", "PUT /posting/flag/testvalue/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue/123412423/123412423", "PUT /posting/flag/testvalue/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue/123412423/testvalue", "PUT /posting/flag/testvalue/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue/testvalue/", "PUT /posting/flag/testvalue/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue/testvalue/variablevalue1", "PUT /posting/flag/testvalue/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue/testvalue/123412423", "PUT /posting/flag/testvalue/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("PUT /posting/flag/testvalue/testvalue/testvalue", "PUT /posting/flag/testvalue/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag///", "DELETE /posting/flag///", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag///variablevalue1", "DELETE /posting/flag///variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag///123412423", "DELETE /posting/flag///123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag///testvalue", "DELETE /posting/flag///testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag//variablevalue1/", "DELETE /posting/flag//variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag//variablevalue1/variablevalue1", "DELETE /posting/flag//variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag//variablevalue1/123412423", "DELETE /posting/flag//variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag//variablevalue1/testvalue", "DELETE /posting/flag//variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag//123412423/", "DELETE /posting/flag//123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag//123412423/variablevalue1", "DELETE /posting/flag//123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag//123412423/123412423", "DELETE /posting/flag//123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag//123412423/testvalue", "DELETE /posting/flag//123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag//testvalue/", "DELETE /posting/flag//testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag//testvalue/variablevalue1", "DELETE /posting/flag//testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag//testvalue/123412423", "DELETE /posting/flag//testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag//testvalue/testvalue", "DELETE /posting/flag//testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1//", "DELETE /posting/flag/variablevalue1//", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1//variablevalue1", "DELETE /posting/flag/variablevalue1//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1//123412423", "DELETE /posting/flag/variablevalue1//123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1//testvalue", "DELETE /posting/flag/variablevalue1//testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1/variablevalue1/", "DELETE /posting/flag/variablevalue1/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1/variablevalue1/variablevalue1", "DELETE /posting/flag/variablevalue1/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1/variablevalue1/123412423", "DELETE /posting/flag/variablevalue1/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1/variablevalue1/testvalue", "DELETE /posting/flag/variablevalue1/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1/123412423/", "DELETE /posting/flag/variablevalue1/123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1/123412423/variablevalue1", "DELETE /posting/flag/variablevalue1/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1/123412423/123412423", "DELETE /posting/flag/variablevalue1/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1/123412423/testvalue", "DELETE /posting/flag/variablevalue1/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1/testvalue/", "DELETE /posting/flag/variablevalue1/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1/testvalue/variablevalue1", "DELETE /posting/flag/variablevalue1/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1/testvalue/123412423", "DELETE /posting/flag/variablevalue1/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/variablevalue1/testvalue/testvalue", "DELETE /posting/flag/variablevalue1/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423//", "DELETE /posting/flag/123412423//", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423//variablevalue1", "DELETE /posting/flag/123412423//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423//123412423", "DELETE /posting/flag/123412423//123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423//testvalue", "DELETE /posting/flag/123412423//testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423/variablevalue1/", "DELETE /posting/flag/123412423/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423/variablevalue1/variablevalue1", "DELETE /posting/flag/123412423/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423/variablevalue1/123412423", "DELETE /posting/flag/123412423/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423/variablevalue1/testvalue", "DELETE /posting/flag/123412423/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423/123412423/", "DELETE /posting/flag/123412423/123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423/123412423/variablevalue1", "DELETE /posting/flag/123412423/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423/123412423/123412423", "DELETE /posting/flag/123412423/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423/123412423/testvalue", "DELETE /posting/flag/123412423/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423/testvalue/", "DELETE /posting/flag/123412423/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423/testvalue/variablevalue1", "DELETE /posting/flag/123412423/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423/testvalue/123412423", "DELETE /posting/flag/123412423/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/123412423/testvalue/testvalue", "DELETE /posting/flag/123412423/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue//", "DELETE /posting/flag/testvalue//", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue//variablevalue1", "DELETE /posting/flag/testvalue//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue//123412423", "DELETE /posting/flag/testvalue//123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue//testvalue", "DELETE /posting/flag/testvalue//testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue/variablevalue1/", "DELETE /posting/flag/testvalue/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue/variablevalue1/variablevalue1", "DELETE /posting/flag/testvalue/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue/variablevalue1/123412423", "DELETE /posting/flag/testvalue/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue/variablevalue1/testvalue", "DELETE /posting/flag/testvalue/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue/123412423/", "DELETE /posting/flag/testvalue/123412423/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue/123412423/variablevalue1", "DELETE /posting/flag/testvalue/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue/123412423/123412423", "DELETE /posting/flag/testvalue/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue/123412423/testvalue", "DELETE /posting/flag/testvalue/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue/testvalue/", "DELETE /posting/flag/testvalue/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue/testvalue/variablevalue1", "DELETE /posting/flag/testvalue/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue/testvalue/123412423", "DELETE /posting/flag/testvalue/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("DELETE /posting/flag/testvalue/testvalue/testvalue", "DELETE /posting/flag/testvalue/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "Flag"),
                    UrlTest("GET /posting/apply//", "GET /posting/apply//", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply//variablevalue1", "GET /posting/apply//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply//123412423", "GET /posting/apply//123412423", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply//testvalue", "GET /posting/apply//testvalue", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply/variablevalue1/", "GET /posting/apply/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply/variablevalue1/variablevalue1", "GET /posting/apply/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply/variablevalue1/123412423", "GET /posting/apply/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply/variablevalue1/testvalue", "GET /posting/apply/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply/123412423/", "GET /posting/apply/123412423/", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply/123412423/variablevalue1", "GET /posting/apply/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply/123412423/123412423", "GET /posting/apply/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply/123412423/testvalue", "GET /posting/apply/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply/testvalue/", "GET /posting/apply/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply/testvalue/variablevalue1", "GET /posting/apply/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply/testvalue/123412423", "GET /posting/apply/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("GET /posting/apply/testvalue/testvalue", "GET /posting/apply/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "ApplyDisplay"),
                    UrlTest("POST /posting/apply//", "POST /posting/apply//", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply//variablevalue1", "POST /posting/apply//variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply//123412423", "POST /posting/apply//123412423", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply//testvalue", "POST /posting/apply//testvalue", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply/variablevalue1/", "POST /posting/apply/variablevalue1/", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply/variablevalue1/variablevalue1", "POST /posting/apply/variablevalue1/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply/variablevalue1/123412423", "POST /posting/apply/variablevalue1/123412423", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply/variablevalue1/testvalue", "POST /posting/apply/variablevalue1/testvalue", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply/123412423/", "POST /posting/apply/123412423/", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply/123412423/variablevalue1", "POST /posting/apply/123412423/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply/123412423/123412423", "POST /posting/apply/123412423/123412423", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply/123412423/testvalue", "POST /posting/apply/123412423/testvalue", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply/testvalue/", "POST /posting/apply/testvalue/", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply/testvalue/variablevalue1", "POST /posting/apply/testvalue/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply/testvalue/123412423", "POST /posting/apply/testvalue/123412423", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("POST /posting/apply/testvalue/testvalue", "POST /posting/apply/testvalue/testvalue", "DefaultController", "GeneralFunctionAccessControl", "ApplyController"),
                    UrlTest("GET /posting/resume/byname/", "GET /posting/resume/byname/", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "ResumeDisplay"),
                    UrlTest("GET /posting/resume/byname/variablevalue1", "GET /posting/resume/byname/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "ResumeDisplay"),
                    UrlTest("GET /posting/resume/byname/123412423", "GET /posting/resume/byname/123412423", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "ResumeDisplay"),
                    UrlTest("GET /posting/resume/byname/testvalue", "GET /posting/resume/byname/testvalue", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "ResumeDisplay"),
                    UrlTest("POST /posting/resume/byname/", "POST /posting/resume/byname/", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "ResumeUpdate"),
                    UrlTest("POST /posting/resume/byname/variablevalue1", "POST /posting/resume/byname/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "ResumeUpdate"),
                    UrlTest("POST /posting/resume/byname/123412423", "POST /posting/resume/byname/123412423", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "ResumeUpdate"),
                    UrlTest("POST /posting/resume/byname/testvalue", "POST /posting/resume/byname/testvalue", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "ResumeUpdate"),
                    UrlTest("GET /posting/resume/preview/byname/", "GET /posting/resume/preview/byname/", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "PreviewDisplay"),
                    UrlTest("GET /posting/resume/preview/byname/variablevalue1", "GET /posting/resume/preview/byname/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "PreviewDisplay"),
                    UrlTest("GET /posting/resume/preview/byname/123412423", "GET /posting/resume/preview/byname/123412423", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "PreviewDisplay"),
                    UrlTest("GET /posting/resume/preview/byname/testvalue", "GET /posting/resume/preview/byname/testvalue", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "PreviewDisplay"),
                    UrlTest("GET /posting/manage", "GET /posting/manage", "GeneralFunctionAccessControl", "DefaultController", "CompanyFunctionAccessControl", "Manage"),
                    UrlTest("GET /posting/ad/applicants/byId/", "GET /posting/ad/applicants/byId/", "GeneralFunctionAccessControl", "DefaultController", "DataAccessControl", "CompanyFunctionAccessControl", "ViewAllApplicatints"),
                    UrlTest("GET /posting/ad/applicants/byId/variablevalue1", "GET /posting/ad/applicants/byId/variablevalue1", "GeneralFunctionAccessControl", "DefaultController", "DataAccessControl", "CompanyFunctionAccessControl", "ViewAllApplicatints"),
                    UrlTest("GET /posting/ad/applicants/byId/123412423", "GET /posting/ad/applicants/byId/123412423", "GeneralFunctionAccessControl", "DefaultController", "DataAccessControl", "CompanyFunctionAccessControl", "ViewAllApplicatints"),
                    UrlTest("GET /posting/ad/applicants/byId/testvalue", "GET /posting/ad/applicants/byId/testvalue", "GeneralFunctionAccessControl", "DefaultController", "DataAccessControl", "CompanyFunctionAccessControl", "ViewAllApplicatints"),
                    UrlTest("GET /posting/ad/byname/", "GET /posting/ad/byname/", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "AdDisplay"),
                    UrlTest("GET /posting/ad/byname/variablevalue1", "GET /posting/ad/byname/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "AdDisplay"),
                    UrlTest("GET /posting/ad/byname/123412423", "GET /posting/ad/byname/123412423", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "AdDisplay"),
                    UrlTest("GET /posting/ad/byname/testvalue", "GET /posting/ad/byname/testvalue", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "AdDisplay"),
                    UrlTest("POST /posting/ad/byname/", "POST /posting/ad/byname/", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "AdUpdate"),
                    UrlTest("POST /posting/ad/byname/variablevalue1", "POST /posting/ad/byname/variablevalue1", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "AdUpdate"),
                    UrlTest("POST /posting/ad/byname/123412423", "POST /posting/ad/byname/123412423", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "AdUpdate"),
                    UrlTest("POST /posting/ad/byname/testvalue", "POST /posting/ad/byname/testvalue", "DefaultController", "GeneralFunctionAccessControl", "DataAccessControl", "AdUpdate")
                    );


            #endregion

            #region Branching
            NewTestWithUrl("Branching",
               Types(
                   Type(
                       "C1",
                       BindAttribute("/a/b1/c1")
                       ),
                   Type(
                       "C2",
                       BindAttribute("/a/b1/c2")
                       ),
                   Type(
                       "C3",
                       BindAttribute("/a/b2/c1")
                       ),
                   Type(
                       "C4",
                       BindAttribute("/a/b2/c2")
                       )
                   ),
                   UrlTest("test1","GET /a"),
                   UrlTest("GET /a/b1/c1","GET /a/b1/c1" ,"C1"),
                   UrlTest("POST /a/b1/c1","POST /a/b1/c1" ,"C1"),
                   UrlTest("PUT /a/b1/c1","PUT /a/b1/c1" ,"C1"),
                   UrlTest("DELETE /a/b1/c1","DELETE /a/b1/c1" ,"C1"),
                   UrlTest("HEAD /a/b1/c1","HEAD /a/b1/c1" ,"C1"),
                   UrlTest("GET /a/b1/c2","GET /a/b1/c2" ,"C2"),
                   UrlTest("POST /a/b1/c2","POST /a/b1/c2" ,"C2"),
                   UrlTest("PUT /a/b1/c2","PUT /a/b1/c2" ,"C2"),
                   UrlTest("DELETE /a/b1/c2","DELETE /a/b1/c2" ,"C2"),
                   UrlTest("HEAD /a/b1/c2","HEAD /a/b1/c2" ,"C2"),
                   UrlTest("GET /a/b2/c1","GET /a/b2/c1" ,"C3"),
                   UrlTest("POST /a/b2/c1","POST /a/b2/c1" ,"C3"),
                   UrlTest("PUT /a/b2/c1","PUT /a/b2/c1" ,"C3"),
                   UrlTest("DELETE /a/b2/c1","DELETE /a/b2/c1" ,"C3"),
                   UrlTest("HEAD /a/b2/c1","HEAD /a/b2/c1" ,"C3"),
                   UrlTest("GET /a/b2/c2","GET /a/b2/c2" ,"C4"),
                   UrlTest("POST /a/b2/c2","POST /a/b2/c2" ,"C4"),
                   UrlTest("PUT /a/b2/c2","PUT /a/b2/c2" ,"C4"),
                   UrlTest("DELETE /a/b2/c2","DELETE /a/b2/c2" ,"C4"),
                   UrlTest("HEAD /a/b2/c2","HEAD /a/b2/c2" ,"C4")
               //Node("* /a", Controllers(),
               //    Node("/b1", Controllers(),
               //        Node("/c1", "C1"),
               //        Node("/c2", "C2")
               //        ),
               //    Node("/b2", Controllers(),
               //        Node("/c1", "C3"),
               //        Node("/c2", "C4")
               //        )
               //    )

               );
            #endregion

            #region tree - one controller - 2 bindings (flat)
            NewTestWithUrl(
                "tree - one controller - 2 bindings (flat)",
                Types(Type(
                    "Controller1",
                    Attributes(
                        BindAttribute("/"),
                        BindAttribute("/path1"))
                    )),
                //Node("* /", "Controller1"),
                //Node("* /path1", "Controller1")
                UrlTest("GET /","GET /" ,"Controller1"),
                UrlTest("POST /","POST /" ,"Controller1"),
                UrlTest("PUT /","PUT /" ,"Controller1"),
                UrlTest("DELETE /","DELETE /" ,"Controller1"),
                UrlTest("HEAD /","HEAD /" ,"Controller1"),
                UrlTest("GET /path1","GET /path1" ,"Controller1"),
                UrlTest("POST /path1","POST /path1" ,"Controller1"),
                UrlTest("PUT /path1","PUT /path1" ,"Controller1"),
                UrlTest("DELETE /path1","DELETE /path1" ,"Controller1"),
                UrlTest("HEAD /path1","HEAD /path1" ,"Controller1")

            );
            #endregion

            #region Imported - home/root
            NewTestWithUrl(
                "Imported - home/root",
                Types(
                    Type(
                        "HomeUrlController1",
                        BindAttribute("/?")
                    ),
                    Type(
                        "HomeUrlController2",
                        BindAttribute("/?")
                    )
                ),
                UrlTest("GET /abcde/edcba/aaaa123/bbb124","GET /abcde/edcba/aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("GET /bbb124","GET /bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("GET /aaaa123/bbb124","GET /aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("POST /abcde/edcba/aaaa123/bbb124","POST /abcde/edcba/aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("POST /bbb124","POST /bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("POST /aaaa123/bbb124","POST /aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("PUT /abcde/edcba/aaaa123/bbb124","PUT /abcde/edcba/aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("PUT /bbb124","PUT /bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("PUT /aaaa123/bbb124","PUT /aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("DELETE /abcde/edcba/aaaa123/bbb124","DELETE /abcde/edcba/aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("DELETE /bbb124","DELETE /bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("DELETE /aaaa123/bbb124","DELETE /aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("HEAD /abcde/edcba/aaaa123/bbb124","HEAD /abcde/edcba/aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("HEAD /bbb124","HEAD /bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("HEAD /aaaa123/bbb124","HEAD /aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("GET /abcde/edcba/aaaa123/bbb124","GET /abcde/edcba/aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("GET /bbb124","GET /bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("GET /aaaa123/bbb124","GET /aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("POST /abcde/edcba/aaaa123/bbb124","POST /abcde/edcba/aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("POST /bbb124","POST /bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("POST /aaaa123/bbb124","POST /aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("PUT /abcde/edcba/aaaa123/bbb124","PUT /abcde/edcba/aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("PUT /bbb124","PUT /bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("PUT /aaaa123/bbb124","PUT /aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("DELETE /abcde/edcba/aaaa123/bbb124","DELETE /abcde/edcba/aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("DELETE /bbb124","DELETE /bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("DELETE /aaaa123/bbb124","DELETE /aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("HEAD /abcde/edcba/aaaa123/bbb124","HEAD /abcde/edcba/aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("HEAD /bbb124","HEAD /bbb124" ,"HomeUrlController2","HomeUrlController1"),
                UrlTest("HEAD /aaaa123/bbb124","HEAD /aaaa123/bbb124" ,"HomeUrlController2","HomeUrlController1")

//                Node("* /?", "HomeUrlController2", "HomeUrlController1") // Actually that's not so good - controllers may come in any order here.
                );
            #endregion

            #region Imported - /hello/...
            NewTestWithUrl("Imported - /hello/...",
                Types(
                    Type("HelloYouController1", BindAttribute("/hello/?/you")),
                    Type("HelloYouController2", BindAttribute("/hello/*/you"))
                ),
                UrlTest("GET /hello/you","GET /hello/you" ,"HelloYouController1"),
                UrlTest("GET /hello/abcde/edcba/aaaa123/bbb124/you","GET /hello/abcde/edcba/aaaa123/bbb124/you" ,"HelloYouController1"),
                UrlTest("GET /hello/bbb124/you","GET /hello/bbb124/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("GET /hello/aaaa123/bbb124/you","GET /hello/aaaa123/bbb124/you" ,"HelloYouController1"),
                UrlTest("POST /hello/you","POST /hello/you" ,"HelloYouController1"),
                UrlTest("POST /hello/abcde/edcba/aaaa123/bbb124/you","POST /hello/abcde/edcba/aaaa123/bbb124/you" ,"HelloYouController1"),
                UrlTest("POST /hello/bbb124/you","POST /hello/bbb124/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("POST /hello/aaaa123/bbb124/you","POST /hello/aaaa123/bbb124/you" ,"HelloYouController1"),
                UrlTest("PUT /hello/you","PUT /hello/you" ,"HelloYouController1"),
                UrlTest("PUT /hello/abcde/edcba/aaaa123/bbb124/you","PUT /hello/abcde/edcba/aaaa123/bbb124/you" ,"HelloYouController1"),
                UrlTest("PUT /hello/bbb124/you","PUT /hello/bbb124/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("PUT /hello/aaaa123/bbb124/you","PUT /hello/aaaa123/bbb124/you" ,"HelloYouController1"),
                UrlTest("DELETE /hello/you","DELETE /hello/you" ,"HelloYouController1"),
                UrlTest("DELETE /hello/abcde/edcba/aaaa123/bbb124/you","DELETE /hello/abcde/edcba/aaaa123/bbb124/you" ,"HelloYouController1"),
                UrlTest("DELETE /hello/bbb124/you","DELETE /hello/bbb124/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("DELETE /hello/aaaa123/bbb124/you","DELETE /hello/aaaa123/bbb124/you" ,"HelloYouController1"),
                UrlTest("HEAD /hello/you","HEAD /hello/you" ,"HelloYouController1"),
                UrlTest("HEAD /hello/abcde/edcba/aaaa123/bbb124/you","HEAD /hello/abcde/edcba/aaaa123/bbb124/you" ,"HelloYouController1"),
                UrlTest("HEAD /hello/bbb124/you","HEAD /hello/bbb124/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("HEAD /hello/aaaa123/bbb124/you","HEAD /hello/aaaa123/bbb124/you" ,"HelloYouController1"),
                UrlTest("GET /hello/aaaaa/you","GET /hello/aaaaa/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("GET /hello/abcde/you","GET /hello/abcde/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("GET /hello/testvalue/you","GET /hello/testvalue/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("POST /hello/aaaaa/you","POST /hello/aaaaa/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("POST /hello/abcde/you","POST /hello/abcde/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("POST /hello/testvalue/you","POST /hello/testvalue/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("PUT /hello/aaaaa/you","PUT /hello/aaaaa/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("PUT /hello/abcde/you","PUT /hello/abcde/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("PUT /hello/testvalue/you","PUT /hello/testvalue/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("DELETE /hello/aaaaa/you","DELETE /hello/aaaaa/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("DELETE /hello/abcde/you","DELETE /hello/abcde/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("DELETE /hello/testvalue/you","DELETE /hello/testvalue/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("HEAD /hello/aaaaa/you","HEAD /hello/aaaaa/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("HEAD /hello/abcde/you","HEAD /hello/abcde/you" ,"HelloYouController1","HelloYouController2"),
                UrlTest("HEAD /hello/testvalue/you","HEAD /hello/testvalue/you" ,"HelloYouController1","HelloYouController2")

                //Node("* /hello", Controllers(),
                //    Node("/*/you", "HelloYouController2", "HelloYouController1"),
                //    Node("/?/you", "HelloYouController1")
                //    )
                );
            #endregion

            #region Imported - /order/world/new
            NewTestWithUrl("Imported - /order/world/new",
                Types(
                    Type(
                        "OrderController1",
                        Attributes(
                            BindAttribute("/order/world/new")
                            ),
                        Field("c1", "string", SessionAttribute),
                        Field("c2", "string", SessionAttribute, RequiresAttribute)
                    ),
                    Type(
                        "OrderController2",
                        Attributes(
                            BindAttribute("/order/world/new")
                            ),
                        Field("c2", "string", SessionAttribute),
                        Field("c5", "string", SessionAttribute, RequiresAttribute)
                    ),
                    Type(
                        "OrderController3",
                        Attributes(
                            BindAttribute("/order/world/new")
                            ),
                        Field("c3", "string", SessionAttribute),
                        Field("c2", "string", SessionAttribute, RequiresAttribute),
                        Field("c4", "string", SessionAttribute, RequiresAttribute),
                        Field("c5", "string", SessionAttribute, RequiresAttribute)
                    ),
                    Type(
                        "OrderController4",
                        Attributes(
                            BindAttribute("/order/world/new")
                            ),
                        Field("c4", "string", SessionAttribute),
                        Field("c1", "string", SessionAttribute, RequiresAttribute),
                        Field("c2", "string", SessionAttribute, RequiresAttribute),
                        Field("c5", "string", SessionAttribute, RequiresAttribute)
                    ),
                    Type(
                        "OrderController5",
                        Attributes(
                            BindAttribute("/order/world/new")
                            ),
                        Field("c5", "string", SessionAttribute)
                    ),
                    Type(
                        "OrderController6",
                        Attributes(
                            BindAttribute("/order/world/new")
                            ),
                        Field("c6", "string", SessionAttribute),
                        Field("c3", "string", SessionAttribute, RequiresAttribute),
                        Field("c7", "string", SessionAttribute, RequiresAttribute)
                    ),
                    Type(
                        "OrderController7",
                        Attributes(
                            BindAttribute("/order/world/new")
                            ),
                        Field("c7", "string", SessionAttribute)
                    )
                ),
                UrlTest("GET /order/world/new","GET /order/world/new" ,"OrderController5","OrderController7","OrderController2","OrderController1","OrderController4","OrderController3","OrderController6"),
                UrlTest("POST /order/world/new","POST /order/world/new" ,"OrderController5","OrderController7","OrderController2","OrderController1","OrderController4","OrderController3","OrderController6"),
                UrlTest("PUT /order/world/new","PUT /order/world/new" ,"OrderController5","OrderController7","OrderController2","OrderController1","OrderController4","OrderController3","OrderController6"),
                UrlTest("DELETE /order/world/new","DELETE /order/world/new" ,"OrderController5","OrderController7","OrderController2","OrderController1","OrderController4","OrderController3","OrderController6"),
                UrlTest("HEAD /order/world/new","HEAD /order/world/new" ,"OrderController5","OrderController7","OrderController2","OrderController1","OrderController4","OrderController3","OrderController6")
                //Node("* /order/world/new", "OrderController7", "OrderController5", "OrderController2", "OrderController1", "OrderController4", "OrderController3", "OrderController6")
            );
            #endregion

            #region Imported - /one_little_url
            NewTestWithUrl(
                "Imported - /one_little_url",
                Types(
                    Type("littleController1",
                        Attributes(
                            BindAttribute("/one_little_url")
                        ),
                        Field("l1", "string", RequestAttribute),
                        Field("l2", "string", RequestAttribute, RequiresAttribute)
                    ),
                    Type("littleController2",
                        Attributes(
                            BindAttribute("/one_little_url")
                        ),
                        Field("l2", "string", RequestAttribute)
                    )
                ),
                UrlTest("GET /one_little_url","GET /one_little_url" ,"littleController2","littleController1"),
                UrlTest("POST /one_little_url","POST /one_little_url" ,"littleController2","littleController1"),
                UrlTest("PUT /one_little_url","PUT /one_little_url" ,"littleController2","littleController1"),
                UrlTest("DELETE /one_little_url","DELETE /one_little_url" ,"littleController2","littleController1"),
                UrlTest("HEAD /one_little_url","HEAD /one_little_url" ,"littleController2","littleController1")
                //Node("* /one_little_url", "littleController2", "littleController1")
            );
            #endregion

            #region Imported - /little_url/more
            NewTestWithUrl(
                "Imported - /little_url/more",
                Types(
                    Type(
                        "littleController3",
                        Attributes(
                            BindAttribute("/little_url/more")
                        ),
                        Field("l3", "string", RequestAttribute)
                    ),
                    Type(
                        "littleController4",
                        Attributes(
                            BindAttribute("/little_url/more")
                        ),
                        Field("l4", "string", RequestAttribute),
                        Field("l3", "string", RequestAttribute, RequiresAttribute),
                        Field("l5", "string", RequestAttribute, RequiresAttribute)
                    ),
                    Type(
                        "littleController5",
                        Attributes(
                            BindAttribute("/little_url/more")
                        ),
                        Field("l5", "string", RequestAttribute),
                        Field("l3", "string", RequestAttribute, RequiresAttribute)
                    )
                ),
                UrlTest("GET /little_url/more","GET /little_url/more" ,"littleController3","littleController5","littleController4"),
                UrlTest("POST /little_url/more","POST /little_url/more" ,"littleController3","littleController5","littleController4"),
                UrlTest("PUT /little_url/more","PUT /little_url/more" ,"littleController3","littleController5","littleController4"),
                UrlTest("DELETE /little_url/more","DELETE /little_url/more" ,"littleController3","littleController5","littleController4"),
                UrlTest("HEAD /little_url/more","HEAD /little_url/more" ,"littleController3","littleController5","littleController4")
                //Node("* /little_url/more", "littleController3", "littleController5", "littleController4")
            );
            #endregion

            #region Imported - GET/hi/...
            NewTestWithUrl(
                "Imported - GET/hi/...",
                Types(
                    Type(
                        "hiController1",
                        BindAttribute("GET /hi/new/world/a")
                    ),
                    Type(
                        "hiController2",
                        BindAttribute("GET /hi/new/*/*/now")
                    ),
                    Type(
                        "hiController3",
                        BindAttribute("GET /hi/*/world/?/now")
                    ),
                    Type(
                        "hiController4",
                        BindAttribute("GET /hi/*/world/*/now")
                    ),
                    Type(
                        "hiController5",
                        BindAttribute("GET /hi/*/world/a/now")
                    ),
                    Type(
                        "hiController6",
                        BindAttribute("GET /hi/*/world/a")
                    ),
                    Type(
                        "hiController7",
                        BindAttribute("GET /hi/*/world/a/*")
                    )
                ),
                UrlTest("GET /hi/new/world/a","GET /hi/new/world/a" ,"hiController6","hiController1"),
                UrlTest("GET /hi/new/aaaaa/aaaaa/now","GET /hi/new/aaaaa/aaaaa/now" ,"hiController2"),
                UrlTest("GET /hi/new/aaaaa/abcde/now","GET /hi/new/aaaaa/abcde/now" ,"hiController2"),
                UrlTest("GET /hi/new/aaaaa/testvalue/now","GET /hi/new/aaaaa/testvalue/now" ,"hiController2"),
                UrlTest("GET /hi/new/abcde/aaaaa/now","GET /hi/new/abcde/aaaaa/now" ,"hiController2"),
                UrlTest("GET /hi/new/abcde/abcde/now","GET /hi/new/abcde/abcde/now" ,"hiController2"),
                UrlTest("GET /hi/new/abcde/testvalue/now","GET /hi/new/abcde/testvalue/now" ,"hiController2"),
                UrlTest("GET /hi/new/testvalue/aaaaa/now","GET /hi/new/testvalue/aaaaa/now" ,"hiController2"),
                UrlTest("GET /hi/new/testvalue/abcde/now","GET /hi/new/testvalue/abcde/now" ,"hiController2"),
                UrlTest("GET /hi/new/testvalue/testvalue/now","GET /hi/new/testvalue/testvalue/now" ,"hiController2"),
                UrlTest("GET /hi/aaaaa/world/now","GET /hi/aaaaa/world/now" ,"hiController3"),
                UrlTest("GET /hi/aaaaa/world/abcde/edcba/aaaa123/bbb124/now","GET /hi/aaaaa/world/abcde/edcba/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/aaaaa/world/bbb124/now","GET /hi/aaaaa/world/bbb124/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/aaaa123/bbb124/now","GET /hi/aaaaa/world/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/abcde/world/now","GET /hi/abcde/world/now" ,"hiController3"),
                UrlTest("GET /hi/abcde/world/abcde/edcba/aaaa123/bbb124/now","GET /hi/abcde/world/abcde/edcba/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/abcde/world/bbb124/now","GET /hi/abcde/world/bbb124/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/aaaa123/bbb124/now","GET /hi/abcde/world/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/testvalue/world/now","GET /hi/testvalue/world/now" ,"hiController3"),
                UrlTest("GET /hi/testvalue/world/abcde/edcba/aaaa123/bbb124/now","GET /hi/testvalue/world/abcde/edcba/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/testvalue/world/bbb124/now","GET /hi/testvalue/world/bbb124/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/aaaa123/bbb124/now","GET /hi/testvalue/world/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/aaaaa/world/aaaaa/now","GET /hi/aaaaa/world/aaaaa/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/abcde/now","GET /hi/aaaaa/world/abcde/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/testvalue/now","GET /hi/aaaaa/world/testvalue/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/aaaaa/now","GET /hi/abcde/world/aaaaa/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/abcde/now","GET /hi/abcde/world/abcde/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/testvalue/now","GET /hi/abcde/world/testvalue/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/aaaaa/now","GET /hi/testvalue/world/aaaaa/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/abcde/now","GET /hi/testvalue/world/abcde/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/testvalue/now","GET /hi/testvalue/world/testvalue/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/a/now","GET /hi/aaaaa/world/a/now" ,"hiController6","hiController3","hiController7","hiController4","hiController5"),
                UrlTest("GET /hi/abcde/world/a/now","GET /hi/abcde/world/a/now" ,"hiController6","hiController3","hiController7","hiController4","hiController5"),
                UrlTest("GET /hi/testvalue/world/a/now","GET /hi/testvalue/world/a/now" ,"hiController6","hiController3","hiController7","hiController4","hiController5"),
                UrlTest("GET /hi/aaaaa/world/a","GET /hi/aaaaa/world/a" ,"hiController6"),
                UrlTest("GET /hi/abcde/world/a","GET /hi/abcde/world/a" ,"hiController6"),
                UrlTest("GET /hi/testvalue/world/a","GET /hi/testvalue/world/a" ,"hiController6"),
                UrlTest("GET /hi/aaaaa/world/a/aaaaa","GET /hi/aaaaa/world/a/aaaaa" ,"hiController6","hiController7"),
                UrlTest("GET /hi/aaaaa/world/a/abcde","GET /hi/aaaaa/world/a/abcde" ,"hiController6","hiController7"),
                UrlTest("GET /hi/aaaaa/world/a/testvalue","GET /hi/aaaaa/world/a/testvalue" ,"hiController6","hiController7"),
                UrlTest("GET /hi/abcde/world/a/aaaaa","GET /hi/abcde/world/a/aaaaa" ,"hiController6","hiController7"),
                UrlTest("GET /hi/abcde/world/a/abcde","GET /hi/abcde/world/a/abcde" ,"hiController6","hiController7"),
                UrlTest("GET /hi/abcde/world/a/testvalue","GET /hi/abcde/world/a/testvalue" ,"hiController6","hiController7"),
                UrlTest("GET /hi/testvalue/world/a/aaaaa","GET /hi/testvalue/world/a/aaaaa" ,"hiController6","hiController7"),
                UrlTest("GET /hi/testvalue/world/a/abcde","GET /hi/testvalue/world/a/abcde" ,"hiController6","hiController7"),
                UrlTest("GET /hi/testvalue/world/a/testvalue","GET /hi/testvalue/world/a/testvalue" ,"hiController6","hiController7")

                //Node("GET /hi", Controllers(),
                //    Node("/*/world", Controllers(),
                //        Node("/*/now", "hiController4", "hiController3"),
                //        Node("/?/now", "hiController3"),
                //        Node("/a", Controllers("hiController7", "hiController6"),
                //            Node("/*", "hiController7", "hiController6"),
                //            Node("/now", "hiController7", "hiController6", "hiController5", "hiController4", "hiController3")
                //            )
                //        ),
                //    Node("/new", Controllers(),
                //        Node("/*/*/now", "hiController2"),
                //        Node("/world/a", "hiController7", "hiController6", "hiController1")
                //        )
                //    )
                );
            #endregion

            //We need more complicated tests - with complex url AND parameters to sort by.

            #region Imported - GET/hi/... - 0
            NewTestWithUrl(
                "Imported - GET/hi/... - 0",
                Types(
                    Type(
                        "hiController4",
                        BindAttribute("GET /hi/*/world/*/now")
                    ),
                    Type(
                        "hiController7",
                        BindAttribute("GET /hi/*/world/a/*")
                    )
                ),
                UrlTest("GET /hi/aaaaa/world/aaaaa/now","GET /hi/aaaaa/world/aaaaa/now" ,"hiController4"),
                UrlTest("GET /hi/aaaaa/world/abcde/now","GET /hi/aaaaa/world/abcde/now" ,"hiController4"),
                UrlTest("GET /hi/aaaaa/world/testvalue/now","GET /hi/aaaaa/world/testvalue/now" ,"hiController4"),
                UrlTest("GET /hi/abcde/world/aaaaa/now","GET /hi/abcde/world/aaaaa/now" ,"hiController4"),
                UrlTest("GET /hi/abcde/world/abcde/now","GET /hi/abcde/world/abcde/now" ,"hiController4"),
                UrlTest("GET /hi/abcde/world/testvalue/now","GET /hi/abcde/world/testvalue/now" ,"hiController4"),
                UrlTest("GET /hi/testvalue/world/aaaaa/now","GET /hi/testvalue/world/aaaaa/now" ,"hiController4"),
                UrlTest("GET /hi/testvalue/world/abcde/now","GET /hi/testvalue/world/abcde/now" ,"hiController4"),
                UrlTest("GET /hi/testvalue/world/testvalue/now","GET /hi/testvalue/world/testvalue/now" ,"hiController4"),
                UrlTest("GET /hi/aaaaa/world/a/aaaaa","GET /hi/aaaaa/world/a/aaaaa" ,"hiController7"),
                UrlTest("GET /hi/aaaaa/world/a/abcde","GET /hi/aaaaa/world/a/abcde" ,"hiController7"),
                UrlTest("GET /hi/aaaaa/world/a/testvalue","GET /hi/aaaaa/world/a/testvalue" ,"hiController7"),
                UrlTest("GET /hi/abcde/world/a/aaaaa","GET /hi/abcde/world/a/aaaaa" ,"hiController7"),
                UrlTest("GET /hi/abcde/world/a/abcde","GET /hi/abcde/world/a/abcde" ,"hiController7"),
                UrlTest("GET /hi/abcde/world/a/testvalue","GET /hi/abcde/world/a/testvalue" ,"hiController7"),
                UrlTest("GET /hi/testvalue/world/a/aaaaa","GET /hi/testvalue/world/a/aaaaa" ,"hiController7"),
                UrlTest("GET /hi/testvalue/world/a/abcde","GET /hi/testvalue/world/a/abcde" ,"hiController7"),
                UrlTest("GET /hi/testvalue/world/a/testvalue","GET /hi/testvalue/world/a/testvalue" ,"hiController7")

                //Node("GET /hi/*/world", Controllers(),
                //    Node("/*/now", "hiController4"),
                //    Node("/a/*", "hiController7")
                //    )
                );
            #endregion

            #region Imported - GET/hi/... - 1
            NewTestWithUrl(
                "Imported - GET/hi/... - 1",
                Types(
                    Type(
                        "hiController2",
                        BindAttribute("GET /hi/new/*/*/now")
                    ),
                    Type(
                        "hiController3",
                        BindAttribute("GET /hi/*/world/?/now")
                    ),
                    Type(
                        "hiController4",
                        BindAttribute("GET /hi/*/world/*/now")
                    ),
                    Type(
                        "hiController6",
                        BindAttribute("GET /hi/*/world/a")
                    ),
                    Type(
                        "hiController7",
                        BindAttribute("GET /hi/*/world/a/*")
                    )
                ),
                UrlTest("GET /hi/new/aaaaa/aaaaa/now","GET /hi/new/aaaaa/aaaaa/now" ,"hiController2"),
                UrlTest("GET /hi/new/aaaaa/abcde/now","GET /hi/new/aaaaa/abcde/now" ,"hiController2"),
                UrlTest("GET /hi/new/aaaaa/testvalue/now","GET /hi/new/aaaaa/testvalue/now" ,"hiController2"),
                UrlTest("GET /hi/new/abcde/aaaaa/now","GET /hi/new/abcde/aaaaa/now" ,"hiController2"),
                UrlTest("GET /hi/new/abcde/abcde/now","GET /hi/new/abcde/abcde/now" ,"hiController2"),
                UrlTest("GET /hi/new/abcde/testvalue/now","GET /hi/new/abcde/testvalue/now" ,"hiController2"),
                UrlTest("GET /hi/new/testvalue/aaaaa/now","GET /hi/new/testvalue/aaaaa/now" ,"hiController2"),
                UrlTest("GET /hi/new/testvalue/abcde/now","GET /hi/new/testvalue/abcde/now" ,"hiController2"),
                UrlTest("GET /hi/new/testvalue/testvalue/now","GET /hi/new/testvalue/testvalue/now" ,"hiController2"),
                UrlTest("GET /hi/aaaaa/world/now","GET /hi/aaaaa/world/now" ,"hiController3"),
                UrlTest("GET /hi/aaaaa/world/abcde/edcba/aaaa123/bbb124/now","GET /hi/aaaaa/world/abcde/edcba/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/aaaaa/world/bbb124/now","GET /hi/aaaaa/world/bbb124/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/aaaa123/bbb124/now","GET /hi/aaaaa/world/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/abcde/world/now","GET /hi/abcde/world/now" ,"hiController3"),
                UrlTest("GET /hi/abcde/world/abcde/edcba/aaaa123/bbb124/now","GET /hi/abcde/world/abcde/edcba/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/abcde/world/bbb124/now","GET /hi/abcde/world/bbb124/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/aaaa123/bbb124/now","GET /hi/abcde/world/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/testvalue/world/now","GET /hi/testvalue/world/now" ,"hiController3"),
                UrlTest("GET /hi/testvalue/world/abcde/edcba/aaaa123/bbb124/now","GET /hi/testvalue/world/abcde/edcba/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/testvalue/world/bbb124/now","GET /hi/testvalue/world/bbb124/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/aaaa123/bbb124/now","GET /hi/testvalue/world/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/aaaaa/world/aaaaa/now","GET /hi/aaaaa/world/aaaaa/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/abcde/now","GET /hi/aaaaa/world/abcde/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/testvalue/now","GET /hi/aaaaa/world/testvalue/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/aaaaa/now","GET /hi/abcde/world/aaaaa/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/abcde/now","GET /hi/abcde/world/abcde/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/testvalue/now","GET /hi/abcde/world/testvalue/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/aaaaa/now","GET /hi/testvalue/world/aaaaa/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/abcde/now","GET /hi/testvalue/world/abcde/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/testvalue/now","GET /hi/testvalue/world/testvalue/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/a","GET /hi/aaaaa/world/a" ,"hiController6"),
                UrlTest("GET /hi/abcde/world/a","GET /hi/abcde/world/a" ,"hiController6"),
                UrlTest("GET /hi/testvalue/world/a","GET /hi/testvalue/world/a" ,"hiController6"),
                UrlTest("GET /hi/aaaaa/world/a/aaaaa","GET /hi/aaaaa/world/a/aaaaa" ,"hiController6","hiController7"),
                UrlTest("GET /hi/aaaaa/world/a/abcde","GET /hi/aaaaa/world/a/abcde" ,"hiController6","hiController7"),
                UrlTest("GET /hi/aaaaa/world/a/testvalue","GET /hi/aaaaa/world/a/testvalue" ,"hiController6","hiController7"),
                UrlTest("GET /hi/abcde/world/a/aaaaa","GET /hi/abcde/world/a/aaaaa" ,"hiController6","hiController7"),
                UrlTest("GET /hi/abcde/world/a/abcde","GET /hi/abcde/world/a/abcde" ,"hiController6","hiController7"),
                UrlTest("GET /hi/abcde/world/a/testvalue","GET /hi/abcde/world/a/testvalue" ,"hiController6","hiController7"),
                UrlTest("GET /hi/testvalue/world/a/aaaaa","GET /hi/testvalue/world/a/aaaaa" ,"hiController6","hiController7"),
                UrlTest("GET /hi/testvalue/world/a/abcde","GET /hi/testvalue/world/a/abcde" ,"hiController6","hiController7"),
                UrlTest("GET /hi/testvalue/world/a/testvalue","GET /hi/testvalue/world/a/testvalue" ,"hiController6","hiController7")

                //Node("GET /hi", Controllers(),
                //    Node("/*/world", Controllers(),
                //        Node("/*/now", "hiController4", "hiController3"),
                //        Node("/?/now", "hiController3"),
                //        Node("/a", Controllers("hiController7", "hiController6"),
                //            Node("/*", "hiController7", "hiController6")
                //            )
                //        ),
                //    Node("/new/*/*/now", "hiController2"
                //        )
                //    )
                );
            #endregion

            #region Imported - GET/hi/... - 2
            NewTestWithUrl(
                "Imported - GET/hi/... - 2",
                Types(
                    Type(
                        "hiController3",
                        BindAttribute("GET /hi/*/world/?/now")
                    ),
                    Type(
                        "hiController4",
                        BindAttribute("GET /hi/*/world/*/now")
                    ),
                    Type(
                        "hiController6",
                        BindAttribute("GET /hi/*/world/a")
                    ),
                    Type(
                        "hiController7",
                        BindAttribute("GET /hi/*/world/a/*")
                    )
                ),
                UrlTest("GET /hi/aaaaa/world/now","GET /hi/aaaaa/world/now" ,"hiController3"),
                UrlTest("GET /hi/aaaaa/world/abcde/edcba/aaaa123/bbb124/now","GET /hi/aaaaa/world/abcde/edcba/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/aaaaa/world/bbb124/now","GET /hi/aaaaa/world/bbb124/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/aaaa123/bbb124/now","GET /hi/aaaaa/world/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/abcde/world/now","GET /hi/abcde/world/now" ,"hiController3"),
                UrlTest("GET /hi/abcde/world/abcde/edcba/aaaa123/bbb124/now","GET /hi/abcde/world/abcde/edcba/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/abcde/world/bbb124/now","GET /hi/abcde/world/bbb124/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/aaaa123/bbb124/now","GET /hi/abcde/world/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/testvalue/world/now","GET /hi/testvalue/world/now" ,"hiController3"),
                UrlTest("GET /hi/testvalue/world/abcde/edcba/aaaa123/bbb124/now","GET /hi/testvalue/world/abcde/edcba/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/testvalue/world/bbb124/now","GET /hi/testvalue/world/bbb124/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/aaaa123/bbb124/now","GET /hi/testvalue/world/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/aaaaa/world/aaaaa/now","GET /hi/aaaaa/world/aaaaa/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/abcde/now","GET /hi/aaaaa/world/abcde/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/testvalue/now","GET /hi/aaaaa/world/testvalue/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/aaaaa/now","GET /hi/abcde/world/aaaaa/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/abcde/now","GET /hi/abcde/world/abcde/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/testvalue/now","GET /hi/abcde/world/testvalue/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/aaaaa/now","GET /hi/testvalue/world/aaaaa/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/abcde/now","GET /hi/testvalue/world/abcde/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/testvalue/now","GET /hi/testvalue/world/testvalue/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/a","GET /hi/aaaaa/world/a" ,"hiController6"),
                UrlTest("GET /hi/abcde/world/a","GET /hi/abcde/world/a" ,"hiController6"),
                UrlTest("GET /hi/testvalue/world/a","GET /hi/testvalue/world/a" ,"hiController6"),
                UrlTest("GET /hi/aaaaa/world/a/aaaaa","GET /hi/aaaaa/world/a/aaaaa" ,"hiController6","hiController7"),
                UrlTest("GET /hi/aaaaa/world/a/abcde","GET /hi/aaaaa/world/a/abcde" ,"hiController6","hiController7"),
                UrlTest("GET /hi/aaaaa/world/a/testvalue","GET /hi/aaaaa/world/a/testvalue" ,"hiController6","hiController7"),
                UrlTest("GET /hi/abcde/world/a/aaaaa","GET /hi/abcde/world/a/aaaaa" ,"hiController6","hiController7"),
                UrlTest("GET /hi/abcde/world/a/abcde","GET /hi/abcde/world/a/abcde" ,"hiController6","hiController7"),
                UrlTest("GET /hi/abcde/world/a/testvalue","GET /hi/abcde/world/a/testvalue" ,"hiController6","hiController7"),
                UrlTest("GET /hi/testvalue/world/a/aaaaa","GET /hi/testvalue/world/a/aaaaa" ,"hiController6","hiController7"),
                UrlTest("GET /hi/testvalue/world/a/abcde","GET /hi/testvalue/world/a/abcde" ,"hiController6","hiController7"),
                UrlTest("GET /hi/testvalue/world/a/testvalue","GET /hi/testvalue/world/a/testvalue" ,"hiController6","hiController7")

                //Node("GET /hi/*/world", Controllers(),
                //        Node("/*/now", "hiController4", "hiController3"),
                //        Node("/?/now", "hiController3"),
                //        Node("/a", Controllers("hiController7", "hiController6"),
                //            Node("/*", "hiController7", "hiController6")//,
                //            )
                //    )
                );
            #endregion

            #region Imported - GET/hi/... - 3
            NewTestWithUrl(
                "Imported - GET/hi/... - 3",
                Types(
                    Type(
                        "hiController4",
                        BindAttribute("GET /*/now")
                    ),
                    Type(
                        "hiController6",
                        BindAttribute("GET /a")
                    ),
                    Type(
                        "hiController7",
                        BindAttribute("GET /a/*")
                    )
                ),
                UrlTest("GET /aaaaa/now","GET /aaaaa/now" ,"hiController4"),
                UrlTest("GET /abcde/now","GET /abcde/now" ,"hiController4"),
                UrlTest("GET /testvalue/now","GET /testvalue/now" ,"hiController4"),
                UrlTest("GET /a","GET /a" ,"hiController6"),
                UrlTest("GET /a/aaaaa","GET /a/aaaaa" ,"hiController6","hiController7"),
                UrlTest("GET /a/abcde","GET /a/abcde" ,"hiController6","hiController7"),
                UrlTest("GET /a/testvalue","GET /a/testvalue" ,"hiController6","hiController7")

                //Node("GET /*/now", "hiController4"),
                //Node("GET /a", Controllers("hiController7", "hiController6"),
                //    Node("/*", "hiController7", "hiController6")
                //    )
                );
            #endregion

            #region Imported - GET/hi/... - 4
            NewTestWithUrl(
                "Imported - GET/hi/... - 4",
                Types(
                    Type(
                        "hiController4",
                        BindAttribute("GET /hi/*/world/*/now")
                    ),
                    Type(
                        "hiController6",
                        BindAttribute("GET /hi/*/world/a")
                    ),
                    Type(
                        "hiController7",
                        BindAttribute("GET /hi/*/world/a/*")
                    )
                ),
                UrlTest("GET /hi/aaaaa/world/aaaaa/now", "GET /hi/aaaaa/world/aaaaa/now", "hiController4"),
                UrlTest("GET /hi/aaaaa/world/abcde/now", "GET /hi/aaaaa/world/abcde/now", "hiController4"),
                UrlTest("GET /hi/aaaaa/world/testvalue/now", "GET /hi/aaaaa/world/testvalue/now", "hiController4"),
                UrlTest("GET /hi/abcde/world/aaaaa/now", "GET /hi/abcde/world/aaaaa/now", "hiController4"),
                UrlTest("GET /hi/abcde/world/abcde/now", "GET /hi/abcde/world/abcde/now", "hiController4"),
                UrlTest("GET /hi/abcde/world/testvalue/now", "GET /hi/abcde/world/testvalue/now", "hiController4"),
                UrlTest("GET /hi/testvalue/world/aaaaa/now", "GET /hi/testvalue/world/aaaaa/now", "hiController4"),
                UrlTest("GET /hi/testvalue/world/abcde/now", "GET /hi/testvalue/world/abcde/now", "hiController4"),
                UrlTest("GET /hi/testvalue/world/testvalue/now", "GET /hi/testvalue/world/testvalue/now", "hiController4"),
                UrlTest("GET /hi/aaaaa/world/a", "GET /hi/aaaaa/world/a", "hiController6"),
                UrlTest("GET /hi/abcde/world/a", "GET /hi/abcde/world/a", "hiController6"),
                UrlTest("GET /hi/testvalue/world/a", "GET /hi/testvalue/world/a", "hiController6"),
                UrlTest("GET /hi/aaaaa/world/a/aaaaa", "GET /hi/aaaaa/world/a/aaaaa", "hiController6", "hiController7"),
                UrlTest("GET /hi/aaaaa/world/a/abcde", "GET /hi/aaaaa/world/a/abcde", "hiController6", "hiController7"),
                UrlTest("GET /hi/aaaaa/world/a/testvalue", "GET /hi/aaaaa/world/a/testvalue", "hiController6", "hiController7"),
                UrlTest("GET /hi/abcde/world/a/aaaaa", "GET /hi/abcde/world/a/aaaaa", "hiController6", "hiController7"),
                UrlTest("GET /hi/abcde/world/a/abcde", "GET /hi/abcde/world/a/abcde", "hiController6", "hiController7"),
                UrlTest("GET /hi/abcde/world/a/testvalue", "GET /hi/abcde/world/a/testvalue", "hiController6", "hiController7"),
                UrlTest("GET /hi/testvalue/world/a/aaaaa", "GET /hi/testvalue/world/a/aaaaa", "hiController6", "hiController7"),
                UrlTest("GET /hi/testvalue/world/a/abcde", "GET /hi/testvalue/world/a/abcde", "hiController6", "hiController7"),
                UrlTest("GET /hi/testvalue/world/a/testvalue", "GET /hi/testvalue/world/a/testvalue", "hiController6", "hiController7")
                //Node("GET /hi/*/world", Controllers(),
                //        Node("/*/now", "hiController4"),
                //        Node("/a", Controllers("hiController7", "hiController6"),
                //            Node("/*", "hiController7", "hiController6")
                //            )
                //    )
                );
            #endregion

            #region Imported - GET/hi/... - 5
            NewTestWithUrl(
                "Imported - GET/hi/... - 5",
                Types(
                    Type(
                        "hiController2",
                        BindAttribute("GET /hi/new/*/*/now")
                    ),
                    Type(
                        "hiController3",
                        BindAttribute("GET /hi/*/world/?/now")
                    ),
                    Type(
                        "hiController4",
                        BindAttribute("GET /hi/*/world/*/now")
                    ),
                    Type(
                        "hiController7",
                        BindAttribute("GET /hi/*/world/a/*")
                    )
                ),
                UrlTest("GET /hi/new/aaaaa/aaaaa/now","GET /hi/new/aaaaa/aaaaa/now" ,"hiController2"),
                UrlTest("GET /hi/new/aaaaa/abcde/now","GET /hi/new/aaaaa/abcde/now" ,"hiController2"),
                UrlTest("GET /hi/new/aaaaa/testvalue/now","GET /hi/new/aaaaa/testvalue/now" ,"hiController2"),
                UrlTest("GET /hi/new/abcde/aaaaa/now","GET /hi/new/abcde/aaaaa/now" ,"hiController2"),
                UrlTest("GET /hi/new/abcde/abcde/now","GET /hi/new/abcde/abcde/now" ,"hiController2"),
                UrlTest("GET /hi/new/abcde/testvalue/now","GET /hi/new/abcde/testvalue/now" ,"hiController2"),
                UrlTest("GET /hi/new/testvalue/aaaaa/now","GET /hi/new/testvalue/aaaaa/now" ,"hiController2"),
                UrlTest("GET /hi/new/testvalue/abcde/now","GET /hi/new/testvalue/abcde/now" ,"hiController2"),
                UrlTest("GET /hi/new/testvalue/testvalue/now","GET /hi/new/testvalue/testvalue/now" ,"hiController2"),
                UrlTest("GET /hi/aaaaa/world/now","GET /hi/aaaaa/world/now" ,"hiController3"),
                UrlTest("GET /hi/aaaaa/world/abcde/edcba/aaaa123/bbb124/now","GET /hi/aaaaa/world/abcde/edcba/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/aaaaa/world/bbb124/now","GET /hi/aaaaa/world/bbb124/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/aaaa123/bbb124/now","GET /hi/aaaaa/world/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/abcde/world/now","GET /hi/abcde/world/now" ,"hiController3"),
                UrlTest("GET /hi/abcde/world/abcde/edcba/aaaa123/bbb124/now","GET /hi/abcde/world/abcde/edcba/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/abcde/world/bbb124/now","GET /hi/abcde/world/bbb124/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/aaaa123/bbb124/now","GET /hi/abcde/world/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/testvalue/world/now","GET /hi/testvalue/world/now" ,"hiController3"),
                UrlTest("GET /hi/testvalue/world/abcde/edcba/aaaa123/bbb124/now","GET /hi/testvalue/world/abcde/edcba/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/testvalue/world/bbb124/now","GET /hi/testvalue/world/bbb124/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/aaaa123/bbb124/now","GET /hi/testvalue/world/aaaa123/bbb124/now" ,"hiController3"),
                UrlTest("GET /hi/aaaaa/world/aaaaa/now","GET /hi/aaaaa/world/aaaaa/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/abcde/now","GET /hi/aaaaa/world/abcde/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/testvalue/now","GET /hi/aaaaa/world/testvalue/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/aaaaa/now","GET /hi/abcde/world/aaaaa/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/abcde/now","GET /hi/abcde/world/abcde/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/abcde/world/testvalue/now","GET /hi/abcde/world/testvalue/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/aaaaa/now","GET /hi/testvalue/world/aaaaa/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/abcde/now","GET /hi/testvalue/world/abcde/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/testvalue/world/testvalue/now","GET /hi/testvalue/world/testvalue/now" ,"hiController3","hiController4"),
                UrlTest("GET /hi/aaaaa/world/a/aaaaa","GET /hi/aaaaa/world/a/aaaaa" ,"hiController7"),
                UrlTest("GET /hi/aaaaa/world/a/abcde","GET /hi/aaaaa/world/a/abcde" ,"hiController7"),
                UrlTest("GET /hi/aaaaa/world/a/testvalue","GET /hi/aaaaa/world/a/testvalue" ,"hiController7"),
                UrlTest("GET /hi/abcde/world/a/aaaaa","GET /hi/abcde/world/a/aaaaa" ,"hiController7"),
                UrlTest("GET /hi/abcde/world/a/abcde","GET /hi/abcde/world/a/abcde" ,"hiController7"),
                UrlTest("GET /hi/abcde/world/a/testvalue","GET /hi/abcde/world/a/testvalue" ,"hiController7"),
                UrlTest("GET /hi/testvalue/world/a/aaaaa","GET /hi/testvalue/world/a/aaaaa" ,"hiController7"),
                UrlTest("GET /hi/testvalue/world/a/abcde","GET /hi/testvalue/world/a/abcde" ,"hiController7"),
                UrlTest("GET /hi/testvalue/world/a/testvalue","GET /hi/testvalue/world/a/testvalue" ,"hiController7")
                //Node("GET /hi", Controllers(),
                //    Node("/*/world", Controllers(),
                //        Node("/*/now", "hiController4", "hiController3"),
                //        Node("/?/now", "hiController3"),
                //        Node("/a/*", "hiController7")
                //        ),
                //    Node("/new/*/*/now", "hiController2"
                //        )
                //    )
                );
            #endregion

            #region Imported - DependsOn/Requires
            NewTestWithUrl(
                "Imported - DependsOn/Requires",
                Types(
                    Type("DRController2",
                        Attributes(BindAttribute("GET /dependson/requires")),
                        Field("z", "int", RequestAttribute, RequiresAttribute)
                        ),
                    Type("DRController1",
                        Attributes(BindAttribute("GET /dependson/requires")),
                        Field("z", "int", RequestAttribute)
                        )
                    ),
                    UrlTest("GET /dependson/requires", "GET /dependson/requires", "DRController1", "DRController2")
                //Node("GET /dependson/requires", "DRController1", "DRController2") // Check for Verbs???
                );
            #endregion

            #region Imported - DataQPaging
            NewTestWithUrl(
                "Imported - DataQPaging",
                Types(
                    Type("DataRoot",
                        Attributes(BindAttribute("GET /data/?")),
                        Field("dataRoot", "Boolean", RequestAttribute)
                        ),
                    Type("Data14sData",
                        Attributes(
                            BindAttribute("GET /data/Data12/id/{Data12Id}/Data14s/id/{dataId}"),
                            BindAttribute("GET /data/Data12/id/{Data12Id}/Data14s/id/{dataId}/withpaging/{linesPerPage}/{pageNumber}")
                        ),
                        Field("dataRoot", "Boolean", RequestAttribute, RequiresAttribute),
                        Field("dataSource", "Boolean", RequestAttribute)
                        ),
                    Type("BlueCrossData14sData",
                        Attributes(
                            BindAttribute("GET /data/Data12/id/11/Data14s/id/{dataId}"),
                            BindAttribute("GET /data/Data12/id/11/Data14s/id/{dataId}/withpaging/{linesPerPage}/{pageNumber}")
                        ),
                        Field("dataSource", "Boolean", RequestAttribute, RequiresAttribute),
                        Field("dataSourceCustom", "Boolean", RequestAttribute),
                        Field("dataId", "int")
                        ),
                    Type("WithPaging",
                        Attributes(
                            BindAttribute("GET /data/?/withpaging/{linesPerPage}/{pageNumber}")
                        ),
                        Field("dataSource", "Boolean", RequestAttribute, DependsOnAttribute),
                        Field("dataSourceCustom", "Boolean", RequestAttribute, DependsOnAttribute),
                        Field("withPaging", "Boolean", RequestAttribute)
                        ),
                    Type("Data14sRender",
                        Attributes(
                            BindAttribute("GET /data/Data12/id/*/Data14s/id/*")
                        ),
                        Field("dataSource", "Boolean", RequestAttribute, RequiresAttribute),
                        Field("dataSourceCustom", "Boolean", RequestAttribute, DependsOnAttribute),
                        Field("withPaging", "Boolean", RequestAttribute, DependsOnAttribute)
                        )
                    ),
                    UrlTest("GET /data", "GET /data"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124", "GET /data/abcde/edcba/aaaa123/bbb124", "DataRoot"),
                    UrlTest("GET /data/bbb124", "GET /data/bbb124", "DataRoot"),
                    UrlTest("GET /data/aaaa123/bbb124", "GET /data/aaaa123/bbb124", "DataRoot"),
                    UrlTest("GET /data/Data12/id//Data14s/id/", "GET /data/Data12/id//Data14s/id/", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1", "GET /data/Data12/id//Data14s/id/variablevalue1", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423", "GET /data/Data12/id//Data14s/id/123412423", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue", "GET /data/Data12/id//Data14s/id/testvalue", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/", "GET /data/Data12/id/variablevalue1/Data14s/id/", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/", "GET /data/Data12/id/123412423/Data14s/id/", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423", "GET /data/Data12/id/123412423/Data14s/id/123412423", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue", "GET /data/Data12/id/123412423/Data14s/id/testvalue", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/", "GET /data/Data12/id/testvalue/Data14s/id/", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423", "GET /data/Data12/id/testvalue/Data14s/id/123412423", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue", "GET /data/Data12/id/testvalue/Data14s/id/testvalue", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging//", "GET /data/Data12/id//Data14s/id//withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging//variablevalue1", "GET /data/Data12/id//Data14s/id//withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging//123412423", "GET /data/Data12/id//Data14s/id//withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging//testvalue", "GET /data/Data12/id//Data14s/id//withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging/variablevalue1/", "GET /data/Data12/id//Data14s/id//withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id//Data14s/id//withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging/variablevalue1/123412423", "GET /data/Data12/id//Data14s/id//withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging/variablevalue1/testvalue", "GET /data/Data12/id//Data14s/id//withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging/123412423/", "GET /data/Data12/id//Data14s/id//withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging/123412423/variablevalue1", "GET /data/Data12/id//Data14s/id//withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging/123412423/123412423", "GET /data/Data12/id//Data14s/id//withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging/123412423/testvalue", "GET /data/Data12/id//Data14s/id//withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging/testvalue/", "GET /data/Data12/id//Data14s/id//withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging/testvalue/variablevalue1", "GET /data/Data12/id//Data14s/id//withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging/testvalue/123412423", "GET /data/Data12/id//Data14s/id//withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id//withpaging/testvalue/testvalue", "GET /data/Data12/id//Data14s/id//withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging//", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging//variablevalue1", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging//123412423", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging//testvalue", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/variablevalue1/", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/variablevalue1/123412423", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/variablevalue1/testvalue", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/123412423/", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/123412423/variablevalue1", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/123412423/123412423", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/123412423/testvalue", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/testvalue/", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/testvalue/variablevalue1", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/testvalue/123412423", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/testvalue/testvalue", "GET /data/Data12/id//Data14s/id/variablevalue1/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging//", "GET /data/Data12/id//Data14s/id/123412423/withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging//variablevalue1", "GET /data/Data12/id//Data14s/id/123412423/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging//123412423", "GET /data/Data12/id//Data14s/id/123412423/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging//testvalue", "GET /data/Data12/id//Data14s/id/123412423/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging/variablevalue1/", "GET /data/Data12/id//Data14s/id/123412423/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id//Data14s/id/123412423/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging/variablevalue1/123412423", "GET /data/Data12/id//Data14s/id/123412423/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging/variablevalue1/testvalue", "GET /data/Data12/id//Data14s/id/123412423/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging/123412423/", "GET /data/Data12/id//Data14s/id/123412423/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging/123412423/variablevalue1", "GET /data/Data12/id//Data14s/id/123412423/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging/123412423/123412423", "GET /data/Data12/id//Data14s/id/123412423/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging/123412423/testvalue", "GET /data/Data12/id//Data14s/id/123412423/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging/testvalue/", "GET /data/Data12/id//Data14s/id/123412423/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging/testvalue/variablevalue1", "GET /data/Data12/id//Data14s/id/123412423/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging/testvalue/123412423", "GET /data/Data12/id//Data14s/id/123412423/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/123412423/withpaging/testvalue/testvalue", "GET /data/Data12/id//Data14s/id/123412423/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging//", "GET /data/Data12/id//Data14s/id/testvalue/withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging//variablevalue1", "GET /data/Data12/id//Data14s/id/testvalue/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging//123412423", "GET /data/Data12/id//Data14s/id/testvalue/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging//testvalue", "GET /data/Data12/id//Data14s/id/testvalue/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging/variablevalue1/", "GET /data/Data12/id//Data14s/id/testvalue/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id//Data14s/id/testvalue/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging/variablevalue1/123412423", "GET /data/Data12/id//Data14s/id/testvalue/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging/variablevalue1/testvalue", "GET /data/Data12/id//Data14s/id/testvalue/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging/123412423/", "GET /data/Data12/id//Data14s/id/testvalue/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging/123412423/variablevalue1", "GET /data/Data12/id//Data14s/id/testvalue/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging/123412423/123412423", "GET /data/Data12/id//Data14s/id/testvalue/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging/123412423/testvalue", "GET /data/Data12/id//Data14s/id/testvalue/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging/testvalue/", "GET /data/Data12/id//Data14s/id/testvalue/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging/testvalue/variablevalue1", "GET /data/Data12/id//Data14s/id/testvalue/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging/testvalue/123412423", "GET /data/Data12/id//Data14s/id/testvalue/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id//Data14s/id/testvalue/withpaging/testvalue/testvalue", "GET /data/Data12/id//Data14s/id/testvalue/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging//", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging//variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging//123412423", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging//testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/variablevalue1/", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/variablevalue1/123412423", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/variablevalue1/testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/123412423/", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/123412423/variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/123412423/123412423", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/123412423/testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/testvalue/", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/testvalue/variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/testvalue/123412423", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/testvalue/testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id//withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging//", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging//variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging//123412423", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging//testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/variablevalue1/", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/variablevalue1/123412423", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/variablevalue1/testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/123412423/", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/123412423/variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/123412423/123412423", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/123412423/testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/testvalue/", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/testvalue/variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/testvalue/123412423", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/testvalue/testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id/variablevalue1/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging//", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging//variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging//123412423", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging//testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/variablevalue1/", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/variablevalue1/123412423", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/variablevalue1/testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/123412423/", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/123412423/variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/123412423/123412423", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/123412423/testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/testvalue/", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/testvalue/variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/testvalue/123412423", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/testvalue/testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id/123412423/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging//", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging//variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging//123412423", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging//testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/variablevalue1/", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/variablevalue1/123412423", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/variablevalue1/testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/123412423/", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/123412423/variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/123412423/123412423", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/123412423/testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/testvalue/", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/testvalue/variablevalue1", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/testvalue/123412423", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/testvalue/testvalue", "GET /data/Data12/id/variablevalue1/Data14s/id/testvalue/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging//", "GET /data/Data12/id/123412423/Data14s/id//withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging//variablevalue1", "GET /data/Data12/id/123412423/Data14s/id//withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging//123412423", "GET /data/Data12/id/123412423/Data14s/id//withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging//testvalue", "GET /data/Data12/id/123412423/Data14s/id//withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging/variablevalue1/", "GET /data/Data12/id/123412423/Data14s/id//withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/123412423/Data14s/id//withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging/variablevalue1/123412423", "GET /data/Data12/id/123412423/Data14s/id//withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging/variablevalue1/testvalue", "GET /data/Data12/id/123412423/Data14s/id//withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging/123412423/", "GET /data/Data12/id/123412423/Data14s/id//withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging/123412423/variablevalue1", "GET /data/Data12/id/123412423/Data14s/id//withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging/123412423/123412423", "GET /data/Data12/id/123412423/Data14s/id//withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging/123412423/testvalue", "GET /data/Data12/id/123412423/Data14s/id//withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging/testvalue/", "GET /data/Data12/id/123412423/Data14s/id//withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging/testvalue/variablevalue1", "GET /data/Data12/id/123412423/Data14s/id//withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging/testvalue/123412423", "GET /data/Data12/id/123412423/Data14s/id//withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id//withpaging/testvalue/testvalue", "GET /data/Data12/id/123412423/Data14s/id//withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging//", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging//variablevalue1", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging//123412423", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging//testvalue", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/variablevalue1/", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/variablevalue1/123412423", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/variablevalue1/testvalue", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/123412423/", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/123412423/variablevalue1", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/123412423/123412423", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/123412423/testvalue", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/testvalue/", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/testvalue/variablevalue1", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/testvalue/123412423", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/testvalue/testvalue", "GET /data/Data12/id/123412423/Data14s/id/variablevalue1/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging//", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging//variablevalue1", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging//123412423", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging//testvalue", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/variablevalue1/", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/variablevalue1/123412423", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/variablevalue1/testvalue", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/123412423/", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/123412423/variablevalue1", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/123412423/123412423", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/123412423/testvalue", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/testvalue/", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/testvalue/variablevalue1", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/testvalue/123412423", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/testvalue/testvalue", "GET /data/Data12/id/123412423/Data14s/id/123412423/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging//", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging//variablevalue1", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging//123412423", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging//testvalue", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/variablevalue1/", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/variablevalue1/123412423", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/variablevalue1/testvalue", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/123412423/", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/123412423/variablevalue1", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/123412423/123412423", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/123412423/testvalue", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/testvalue/", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/testvalue/variablevalue1", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/testvalue/123412423", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/testvalue/testvalue", "GET /data/Data12/id/123412423/Data14s/id/testvalue/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging//", "GET /data/Data12/id/testvalue/Data14s/id//withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging//variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id//withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging//123412423", "GET /data/Data12/id/testvalue/Data14s/id//withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging//testvalue", "GET /data/Data12/id/testvalue/Data14s/id//withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging/variablevalue1/", "GET /data/Data12/id/testvalue/Data14s/id//withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id//withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging/variablevalue1/123412423", "GET /data/Data12/id/testvalue/Data14s/id//withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging/variablevalue1/testvalue", "GET /data/Data12/id/testvalue/Data14s/id//withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging/123412423/", "GET /data/Data12/id/testvalue/Data14s/id//withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging/123412423/variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id//withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging/123412423/123412423", "GET /data/Data12/id/testvalue/Data14s/id//withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging/123412423/testvalue", "GET /data/Data12/id/testvalue/Data14s/id//withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging/testvalue/", "GET /data/Data12/id/testvalue/Data14s/id//withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging/testvalue/variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id//withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging/testvalue/123412423", "GET /data/Data12/id/testvalue/Data14s/id//withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id//withpaging/testvalue/testvalue", "GET /data/Data12/id/testvalue/Data14s/id//withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging//", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging//variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging//123412423", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging//testvalue", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/variablevalue1/", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/variablevalue1/123412423", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/variablevalue1/testvalue", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/123412423/", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/123412423/variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/123412423/123412423", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/123412423/testvalue", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/testvalue/", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/testvalue/variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/testvalue/123412423", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/testvalue/testvalue", "GET /data/Data12/id/testvalue/Data14s/id/variablevalue1/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging//", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging//variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging//123412423", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging//testvalue", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/variablevalue1/", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/variablevalue1/123412423", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/variablevalue1/testvalue", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/123412423/", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/123412423/variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/123412423/123412423", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/123412423/testvalue", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/testvalue/", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/testvalue/variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/testvalue/123412423", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/testvalue/testvalue", "GET /data/Data12/id/testvalue/Data14s/id/123412423/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging//", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging//", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging//variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging//123412423", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging//testvalue", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/variablevalue1/", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/variablevalue1/123412423", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/variablevalue1/testvalue", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/123412423/", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/123412423/variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/123412423/123412423", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/123412423/testvalue", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/testvalue/", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/testvalue/variablevalue1", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/testvalue/123412423", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/testvalue/testvalue", "GET /data/Data12/id/testvalue/Data14s/id/testvalue/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/", "GET /data/Data12/id/11/Data14s/id/", "DataRoot", "Data14sData", "BlueCrossData14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1", "GET /data/Data12/id/11/Data14s/id/variablevalue1", "DataRoot", "Data14sData", "BlueCrossData14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423", "GET /data/Data12/id/11/Data14s/id/123412423", "DataRoot", "Data14sData", "BlueCrossData14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue", "GET /data/Data12/id/11/Data14s/id/testvalue", "DataRoot", "Data14sData", "BlueCrossData14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging//", "GET /data/Data12/id/11/Data14s/id//withpaging//", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging//variablevalue1", "GET /data/Data12/id/11/Data14s/id//withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging//123412423", "GET /data/Data12/id/11/Data14s/id//withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging//testvalue", "GET /data/Data12/id/11/Data14s/id//withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging/variablevalue1/", "GET /data/Data12/id/11/Data14s/id//withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/11/Data14s/id//withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging/variablevalue1/123412423", "GET /data/Data12/id/11/Data14s/id//withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging/variablevalue1/testvalue", "GET /data/Data12/id/11/Data14s/id//withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging/123412423/", "GET /data/Data12/id/11/Data14s/id//withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging/123412423/variablevalue1", "GET /data/Data12/id/11/Data14s/id//withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging/123412423/123412423", "GET /data/Data12/id/11/Data14s/id//withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging/123412423/testvalue", "GET /data/Data12/id/11/Data14s/id//withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging/testvalue/", "GET /data/Data12/id/11/Data14s/id//withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging/testvalue/variablevalue1", "GET /data/Data12/id/11/Data14s/id//withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging/testvalue/123412423", "GET /data/Data12/id/11/Data14s/id//withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id//withpaging/testvalue/testvalue", "GET /data/Data12/id/11/Data14s/id//withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging//", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging//", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging//variablevalue1", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging//123412423", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging//testvalue", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/variablevalue1/", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/variablevalue1/123412423", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/variablevalue1/testvalue", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/123412423/", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/123412423/variablevalue1", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/123412423/123412423", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/123412423/testvalue", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/testvalue/", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/testvalue/variablevalue1", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/testvalue/123412423", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/testvalue/testvalue", "GET /data/Data12/id/11/Data14s/id/variablevalue1/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging//", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging//", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging//variablevalue1", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging//123412423", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging//testvalue", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging/variablevalue1/", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging/variablevalue1/123412423", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging/variablevalue1/testvalue", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging/123412423/", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging/123412423/variablevalue1", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging/123412423/123412423", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging/123412423/testvalue", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging/testvalue/", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging/testvalue/variablevalue1", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging/testvalue/123412423", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/123412423/withpaging/testvalue/testvalue", "GET /data/Data12/id/11/Data14s/id/123412423/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging//", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging//", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging//variablevalue1", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging//variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging//123412423", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging//123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging//testvalue", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging//testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/variablevalue1/", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/variablevalue1/", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/variablevalue1/variablevalue1", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/variablevalue1/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/variablevalue1/123412423", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/variablevalue1/123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/variablevalue1/testvalue", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/variablevalue1/testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/123412423/", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/123412423/", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/123412423/variablevalue1", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/123412423/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/123412423/123412423", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/123412423/123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/123412423/testvalue", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/123412423/testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/testvalue/", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/testvalue/", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/testvalue/variablevalue1", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/testvalue/variablevalue1", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/testvalue/123412423", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/testvalue/123412423", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/testvalue/testvalue", "GET /data/Data12/id/11/Data14s/id/testvalue/withpaging/testvalue/testvalue", "DataRoot", "Data14sData", "Data14sData", "BlueCrossData14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender"),
                    UrlTest("GET /data/withpaging//", "GET /data/withpaging//", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging//variablevalue1", "GET /data/withpaging//variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging//123412423", "GET /data/withpaging//123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging//testvalue", "GET /data/withpaging//testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging/variablevalue1/", "GET /data/withpaging/variablevalue1/", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging/variablevalue1/variablevalue1", "GET /data/withpaging/variablevalue1/variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging/variablevalue1/123412423", "GET /data/withpaging/variablevalue1/123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging/variablevalue1/testvalue", "GET /data/withpaging/variablevalue1/testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging/123412423/", "GET /data/withpaging/123412423/", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging/123412423/variablevalue1", "GET /data/withpaging/123412423/variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging/123412423/123412423", "GET /data/withpaging/123412423/123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging/123412423/testvalue", "GET /data/withpaging/123412423/testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging/testvalue/", "GET /data/withpaging/testvalue/", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging/testvalue/variablevalue1", "GET /data/withpaging/testvalue/variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging/testvalue/123412423", "GET /data/withpaging/testvalue/123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/withpaging/testvalue/testvalue", "GET /data/withpaging/testvalue/testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging//", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging//", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging//variablevalue1", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging//variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging//123412423", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging//123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging//testvalue", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging//testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging/variablevalue1/", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging/variablevalue1/", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging/variablevalue1/variablevalue1", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging/variablevalue1/variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging/variablevalue1/123412423", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging/variablevalue1/123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging/variablevalue1/testvalue", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging/variablevalue1/testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging/123412423/", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging/123412423/", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging/123412423/variablevalue1", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging/123412423/variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging/123412423/123412423", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging/123412423/123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging/123412423/testvalue", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging/123412423/testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging/testvalue/", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging/testvalue/", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging/testvalue/variablevalue1", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging/testvalue/variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging/testvalue/123412423", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging/testvalue/123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/abcde/edcba/aaaa123/bbb124/withpaging/testvalue/testvalue", "GET /data/abcde/edcba/aaaa123/bbb124/withpaging/testvalue/testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging//", "GET /data/bbb124/withpaging//", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging//variablevalue1", "GET /data/bbb124/withpaging//variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging//123412423", "GET /data/bbb124/withpaging//123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging//testvalue", "GET /data/bbb124/withpaging//testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging/variablevalue1/", "GET /data/bbb124/withpaging/variablevalue1/", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging/variablevalue1/variablevalue1", "GET /data/bbb124/withpaging/variablevalue1/variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging/variablevalue1/123412423", "GET /data/bbb124/withpaging/variablevalue1/123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging/variablevalue1/testvalue", "GET /data/bbb124/withpaging/variablevalue1/testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging/123412423/", "GET /data/bbb124/withpaging/123412423/", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging/123412423/variablevalue1", "GET /data/bbb124/withpaging/123412423/variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging/123412423/123412423", "GET /data/bbb124/withpaging/123412423/123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging/123412423/testvalue", "GET /data/bbb124/withpaging/123412423/testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging/testvalue/", "GET /data/bbb124/withpaging/testvalue/", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging/testvalue/variablevalue1", "GET /data/bbb124/withpaging/testvalue/variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging/testvalue/123412423", "GET /data/bbb124/withpaging/testvalue/123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/bbb124/withpaging/testvalue/testvalue", "GET /data/bbb124/withpaging/testvalue/testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging//", "GET /data/aaaa123/bbb124/withpaging//", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging//variablevalue1", "GET /data/aaaa123/bbb124/withpaging//variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging//123412423", "GET /data/aaaa123/bbb124/withpaging//123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging//testvalue", "GET /data/aaaa123/bbb124/withpaging//testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging/variablevalue1/", "GET /data/aaaa123/bbb124/withpaging/variablevalue1/", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging/variablevalue1/variablevalue1", "GET /data/aaaa123/bbb124/withpaging/variablevalue1/variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging/variablevalue1/123412423", "GET /data/aaaa123/bbb124/withpaging/variablevalue1/123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging/variablevalue1/testvalue", "GET /data/aaaa123/bbb124/withpaging/variablevalue1/testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging/123412423/", "GET /data/aaaa123/bbb124/withpaging/123412423/", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging/123412423/variablevalue1", "GET /data/aaaa123/bbb124/withpaging/123412423/variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging/123412423/123412423", "GET /data/aaaa123/bbb124/withpaging/123412423/123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging/123412423/testvalue", "GET /data/aaaa123/bbb124/withpaging/123412423/testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging/testvalue/", "GET /data/aaaa123/bbb124/withpaging/testvalue/", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging/testvalue/variablevalue1", "GET /data/aaaa123/bbb124/withpaging/testvalue/variablevalue1", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging/testvalue/123412423", "GET /data/aaaa123/bbb124/withpaging/testvalue/123412423", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/aaaa123/bbb124/withpaging/testvalue/testvalue", "GET /data/aaaa123/bbb124/withpaging/testvalue/testvalue", "DataRoot", "WithPaging"),
                    UrlTest("GET /data/Data12/id/aaaaa/Data14s/id/aaaaa", "GET /data/Data12/id/aaaaa/Data14s/id/aaaaa", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/aaaaa/Data14s/id/abcde", "GET /data/Data12/id/aaaaa/Data14s/id/abcde", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/aaaaa/Data14s/id/testvalue", "GET /data/Data12/id/aaaaa/Data14s/id/testvalue", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/abcde/Data14s/id/aaaaa", "GET /data/Data12/id/abcde/Data14s/id/aaaaa", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/abcde/Data14s/id/abcde", "GET /data/Data12/id/abcde/Data14s/id/abcde", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/abcde/Data14s/id/testvalue", "GET /data/Data12/id/abcde/Data14s/id/testvalue", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/aaaaa", "GET /data/Data12/id/testvalue/Data14s/id/aaaaa", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/abcde", "GET /data/Data12/id/testvalue/Data14s/id/abcde", "DataRoot", "Data14sData", "Data14sRender"),
                    UrlTest("GET /data/Data12/id/testvalue/Data14s/id/testvalue", "GET /data/Data12/id/testvalue/Data14s/id/testvalue", "DataRoot", "Data14sData", "Data14sRender")
                    //Node("GET /data", Controllers(),
                    //    Node("/?", Controllers("DataRoot"),
                    //        Node("/withpaging/{linesPerPage}/{pageNumber}", Controllers("WithPaging", "DataRoot"))),
                    //    Node("/Data12/id", Controllers(),
                    //        Node("/*/Data14s/id/*", Controllers("DataRoot", "Data14sData", "Data14sRender")),
                    //        Node("/{Data12Id}/Data14s/id/{dataId}", Controllers("DataRoot", "Data14sData", "Data14sRender"),
                    //            Node("/withpaging/{linesPerPage}/{pageNumber}", Controllers("DataRoot", "Data14sData", "WithPaging", "Data14sRender"))),
                    //        Node("/11/Data14s/id/{dataId}", Controllers("DataRoot", "Data14sData", "BlueCrossData14sData", "Data14sRender"),
                    //            Node("/withpaging/{linesPerPage}/{pageNumber}", Controllers("DataRoot", "Data14sData", "BlueCrossData14sData", "WithPaging", "Data14sRender")))
                    //        )
                    //    )
                    );
            #endregion

            #region Imported - DataQPaging-simple
            NewTestWithUrl(
                "Imported - DataQPaging-simple",
                Types(
                    Type("DataRoot",
                        Attributes(BindAttribute("GET /a/?"))
                        ),
                    Type("WithPaging",
                        Attributes(
                            BindAttribute("GET /a/?/c3")
                        )
                        ),
                    Type("Data14sRender",
                        Attributes(
                            BindAttribute("GET /a/b/c2")
                        )
                        ),
                    Type("Data14sRenderBis",
                        Attributes(
                            BindAttribute("GET /a/b/c1")
                        )
                        )
                    ),
                    UrlTest("GET /a", "GET /a"),
                    UrlTest("GET /a/abcde/edcba/aaaa123/bbb124", "GET /a/abcde/edcba/aaaa123/bbb124", "DataRoot"),
                    UrlTest("GET /a/bbb124", "GET /a/bbb124", "DataRoot"),
                    UrlTest("GET /a/aaaa123/bbb124", "GET /a/aaaa123/bbb124", "DataRoot"),
                    UrlTest("GET /a/c3", "GET /a/c3", "WithPaging", "DataRoot"),
                    UrlTest("GET /a/abcde/edcba/aaaa123/bbb124/c3", "GET /a/abcde/edcba/aaaa123/bbb124/c3", "WithPaging", "DataRoot"),
                    UrlTest("GET /a/bbb124/c3", "GET /a/bbb124/c3", "WithPaging", "DataRoot"),
                    UrlTest("GET /a/aaaa123/bbb124/c3", "GET /a/aaaa123/bbb124/c3", "WithPaging", "DataRoot"),
                    UrlTest("GET /a/b/c2", "GET /a/b/c2", "DataRoot", "Data14sRender"),
                    UrlTest("GET /a/b/c1", "GET /a/b/c1", "DataRoot", "Data14sRenderBis")

                    //Node("GET /a", Controllers(),// is empty because it's not a method node
                    //    Node("/?", Controllers("DataRoot"),
                    //        Node("/c3", Controllers("WithPaging", "DataRoot"))),
                    //    Node("/b", Controllers(),// is empty because it's not a method node
                    //        Node("/c2", Controllers("Data14sRender", "DataRoot")),
                    //        Node("/c1", Controllers("Data14sRenderBis", "DataRoot"))
                    //        )
                    //    )
                );
            #endregion

            #region tree - single controller
            NewTestWithUrl(
                "tree - single controller",
                Types(Type("Controller1", BindAttribute("/?"))),
                UrlTest("GET /abcde/edcba/aaaa123/bbb124","GET /abcde/edcba/aaaa123/bbb124" ,"Controller1"),
                UrlTest("GET /bbb124","GET /bbb124" ,"Controller1"),
                UrlTest("GET /aaaa123/bbb124","GET /aaaa123/bbb124" ,"Controller1"),
                UrlTest("POST /abcde/edcba/aaaa123/bbb124","POST /abcde/edcba/aaaa123/bbb124" ,"Controller1"),
                UrlTest("POST /bbb124","POST /bbb124" ,"Controller1"),
                UrlTest("POST /aaaa123/bbb124","POST /aaaa123/bbb124" ,"Controller1"),
                UrlTest("PUT /abcde/edcba/aaaa123/bbb124","PUT /abcde/edcba/aaaa123/bbb124" ,"Controller1"),
                UrlTest("PUT /bbb124","PUT /bbb124" ,"Controller1"),
                UrlTest("PUT /aaaa123/bbb124","PUT /aaaa123/bbb124" ,"Controller1"),
                UrlTest("DELETE /abcde/edcba/aaaa123/bbb124","DELETE /abcde/edcba/aaaa123/bbb124" ,"Controller1"),
                UrlTest("DELETE /bbb124","DELETE /bbb124" ,"Controller1"),
                UrlTest("DELETE /aaaa123/bbb124","DELETE /aaaa123/bbb124" ,"Controller1"),
                UrlTest("HEAD /abcde/edcba/aaaa123/bbb124","HEAD /abcde/edcba/aaaa123/bbb124" ,"Controller1"),
                UrlTest("HEAD /bbb124","HEAD /bbb124" ,"Controller1"),
                UrlTest("HEAD /aaaa123/bbb124","HEAD /aaaa123/bbb124" ,"Controller1")

//                Node("* /?", Controller("Controller1", 1))
            );
            #endregion

            #region tree - one controller - 3 bindings (flat)
            NewTestWithUrl(
                "tree - one controller - 3 bindings (flat)",
                Types(Type(
                    "Controller1",
                    Attributes(
                        BindAttribute("/default"),
                        BindAttribute("/path1"),
                        BindAttribute("/path2/more"))
                    )),
                    UrlTest("GET /default","GET /default" ,"Controller1"),
                    UrlTest("POST /default","POST /default" ,"Controller1"),
                    UrlTest("PUT /default","PUT /default" ,"Controller1"),
                    UrlTest("DELETE /default","DELETE /default" ,"Controller1"),
                    UrlTest("HEAD /default","HEAD /default" ,"Controller1"),
                    UrlTest("GET /path1","GET /path1" ,"Controller1"),
                    UrlTest("POST /path1","POST /path1" ,"Controller1"),
                    UrlTest("PUT /path1","PUT /path1" ,"Controller1"),
                    UrlTest("DELETE /path1","DELETE /path1" ,"Controller1"),
                    UrlTest("HEAD /path1","HEAD /path1" ,"Controller1"),
                    UrlTest("GET /path2/more","GET /path2/more" ,"Controller1"),
                    UrlTest("POST /path2/more","POST /path2/more" ,"Controller1"),
                    UrlTest("PUT /path2/more","PUT /path2/more" ,"Controller1"),
                    UrlTest("DELETE /path2/more","DELETE /path2/more" ,"Controller1"),
                    UrlTest("HEAD /path2/more","HEAD /path2/more" ,"Controller1")

                //Node("* /default", "Controller1"),
                //Node("* /path1", "Controller1"),
                //Node("* /path2/more", "Controller1")
                );
            #endregion

            #region tree - one controller - 3 bindings (tree)
            NewTestWithUrl(
                "tree - one controller - 3 bindings (tree)",
                Types(Type(
                    "Controller1",
                    Attributes(
                        BindAttribute("/default"),
                        BindAttribute("/path2"),
                        BindAttribute("/path2/more"))
                    )),
                    UrlTest("GET /default","GET /default" ,"Controller1"),
                    UrlTest("POST /default","POST /default" ,"Controller1"),
                    UrlTest("PUT /default","PUT /default" ,"Controller1"),
                    UrlTest("DELETE /default","DELETE /default" ,"Controller1"),
                    UrlTest("HEAD /default","HEAD /default" ,"Controller1"),
                    UrlTest("GET /path2","GET /path2" ,"Controller1"),
                    UrlTest("POST /path2","POST /path2" ,"Controller1"),
                    UrlTest("PUT /path2","PUT /path2" ,"Controller1"),
                    UrlTest("DELETE /path2","DELETE /path2" ,"Controller1"),
                    UrlTest("HEAD /path2","HEAD /path2" ,"Controller1"),
                    UrlTest("GET /path2/more","GET /path2/more" ,"Controller1","Controller1"),
                    UrlTest("POST /path2/more","POST /path2/more" ,"Controller1","Controller1"),
                    UrlTest("PUT /path2/more","PUT /path2/more" ,"Controller1","Controller1"),
                    UrlTest("DELETE /path2/more","DELETE /path2/more" ,"Controller1","Controller1"),
                    UrlTest("HEAD /path2/more","HEAD /path2/more" ,"Controller1","Controller1")

                //Node("* /default", "Controller1"),
                //Node("* /path2", Controllers("Controller1"),
                //    Node("/more", "Controller1")
                //    )
                );
            #endregion

            #region tree - one generic one specific
            NewTestWithUrl(
                "tree - one generic one specific",
                Types(Type(
                    "Controller1",
                    Attributes(
                        BindAttribute("/default"),
                        BindAttribute("/path2"),
                        BindAttribute("/path2/more"))
                        ),
                    Type("Controller2", BindAttribute("/?"))
                    ),
                    UrlTest("GET /default", "GET /default", "Controller2", "Controller1"),
                    UrlTest("POST /default", "POST /default", "Controller2", "Controller1"),
                    UrlTest("PUT /default", "PUT /default", "Controller2", "Controller1"),
                    UrlTest("DELETE /default", "DELETE /default", "Controller2", "Controller1"),
                    UrlTest("HEAD /default", "HEAD /default", "Controller2", "Controller1"),
                    UrlTest("GET /path2", "GET /path2", "Controller2", "Controller1"),
                    UrlTest("POST /path2", "POST /path2", "Controller2", "Controller1"),
                    UrlTest("PUT /path2", "PUT /path2", "Controller2", "Controller1"),
                    UrlTest("DELETE /path2", "DELETE /path2", "Controller2", "Controller1"),
                    UrlTest("HEAD /path2", "HEAD /path2", "Controller2", "Controller1"),
                    UrlTest("GET /path2/more", "GET /path2/more", "Controller2", "Controller1", "Controller1"),
                    UrlTest("POST /path2/more", "POST /path2/more", "Controller2", "Controller1", "Controller1"),
                    UrlTest("PUT /path2/more", "PUT /path2/more", "Controller2", "Controller1", "Controller1"),
                    UrlTest("DELETE /path2/more", "DELETE /path2/more", "Controller2", "Controller1", "Controller1"),
                    UrlTest("HEAD /path2/more", "HEAD /path2/more", "Controller2", "Controller1", "Controller1"),
                    UrlTest("GET /abcde/edcba/aaaa123/bbb124", "GET /abcde/edcba/aaaa123/bbb124", "Controller2"),
                    UrlTest("GET /bbb124", "GET /bbb124", "Controller2"),
                    UrlTest("GET /aaaa123/bbb124", "GET /aaaa123/bbb124", "Controller2"),
                    UrlTest("POST /abcde/edcba/aaaa123/bbb124", "POST /abcde/edcba/aaaa123/bbb124", "Controller2"),
                    UrlTest("POST /bbb124", "POST /bbb124", "Controller2"),
                    UrlTest("POST /aaaa123/bbb124", "POST /aaaa123/bbb124", "Controller2"),
                    UrlTest("PUT /abcde/edcba/aaaa123/bbb124", "PUT /abcde/edcba/aaaa123/bbb124", "Controller2"),
                    UrlTest("PUT /bbb124", "PUT /bbb124", "Controller2"),
                    UrlTest("PUT /aaaa123/bbb124", "PUT /aaaa123/bbb124", "Controller2"),
                    UrlTest("DELETE /abcde/edcba/aaaa123/bbb124", "DELETE /abcde/edcba/aaaa123/bbb124", "Controller2"),
                    UrlTest("DELETE /bbb124", "DELETE /bbb124", "Controller2"),
                    UrlTest("DELETE /aaaa123/bbb124", "DELETE /aaaa123/bbb124", "Controller2"),
                    UrlTest("HEAD /abcde/edcba/aaaa123/bbb124", "HEAD /abcde/edcba/aaaa123/bbb124", "Controller2"),
                    UrlTest("HEAD /bbb124", "HEAD /bbb124", "Controller2"),
                    UrlTest("HEAD /aaaa123/bbb124", "HEAD /aaaa123/bbb124", "Controller2")
                //Node("* /default", "Controller2", "Controller1"),
                //Node("* /path2", Controllers("Controller2", "Controller1"),
                //    Node("/more", "Controller2", "Controller1")
                //    ),
                //Node("* /?", "Controller2")
                );
            #endregion


            // Note that there's questionmark here, without leading slash
            #region tree - one generic one specific - reversed
            NewTestWithUrl(
                "tree - one generic one specific - reversed",
                Types(
                    Type("Controller2", BindAttribute("?")),
                    Type(
                    "Controller1",
                    Attributes(
                        BindAttribute("/default"),
                        BindAttribute("/path2"),
                        BindAttribute("/path2/more"))
                        )
                    ),
                    UrlTest("test special","GET /anytestpath","Controller2"),
                    UrlTest("GET /default","GET /default" ,"Controller1","Controller2"),
                    UrlTest("POST /default","POST /default" ,"Controller1","Controller2"),
                    UrlTest("PUT /default","PUT /default" ,"Controller1","Controller2"),
                    UrlTest("DELETE /default","DELETE /default" ,"Controller1","Controller2"),
                    UrlTest("HEAD /default","HEAD /default" ,"Controller1","Controller2"),
                    UrlTest("GET /path2","GET /path2" ,"Controller1","Controller2"),
                    UrlTest("POST /path2","POST /path2" ,"Controller1","Controller2"),
                    UrlTest("PUT /path2","PUT /path2" ,"Controller1","Controller2"),
                    UrlTest("DELETE /path2","DELETE /path2" ,"Controller1","Controller2"),
                    UrlTest("HEAD /path2","HEAD /path2" ,"Controller1","Controller2"),
                    UrlTest("GET /path2/more","GET /path2/more" ,"Controller1","Controller2","Controller1"),
                    UrlTest("POST /path2/more","POST /path2/more" ,"Controller1","Controller2","Controller1"),
                    UrlTest("PUT /path2/more","PUT /path2/more" ,"Controller1","Controller2","Controller1"),
                    UrlTest("DELETE /path2/more","DELETE /path2/more" ,"Controller1","Controller2","Controller1"),
                    UrlTest("HEAD /path2/more","HEAD /path2/more" ,"Controller1","Controller2","Controller1")
                //Node("* ?", "Controller2"),
                //Node("* /default", "Controller1", "Controller2"),
                //Node("* /path2", Controllers("Controller1", "Controller2"),
                //    Node("/more", "Controller1", "Controller2")
                //    )
                );
            #endregion


            return tests;
        }




    }
}
