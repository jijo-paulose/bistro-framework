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

            foreach (UrlControllersTest urlTest in descriptor.UrlTests)
            {
                urlTest.Validate(dispatcher);
            }
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


            #region SRxPortal - Big Test
            NewTestWithUrl("SRxPortal - Big Test",
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
                    Type("Case",
                        Attributes(
                            BindAttribute("GET /Case/{CaseID}")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("QueryStringCollection", "NameValueCollection", DependsOnAttribute, RequestAttribute),
                        Field("CaseID", "string", ProvidesAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("CaseService", "CaseServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("CaseDelete",
                        Attributes(
                            BindAttribute("DELETE /Case/{CaseID}")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("QueryStringCollection", "NameValueCollection", DependsOnAttribute, RequestAttribute),
                        Field("CaseID", "string", ProvidesAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("CaseService", "CaseServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("Cases",
                        Attributes(
                            BindAttribute("GET /Cases")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("QueryStringCollection", "NameValueCollection", DependsOnAttribute, RequestAttribute),
                        Field("CaseService", "CaseServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("ClientId", "short", DependsOnAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("GetNextStep",
                        Attributes(
                            BindAttribute("GET /Clinical/GetNextStep?{TransactionId}&{DiagnosisId}")
                        ),
                        Field("TransactionId", "string", RequestAttribute),
                        Field("DiagnosisId", "string", RequestAttribute),
                        Field("Questions", "List<ClinicalQuestionStub>", RequestAttribute),
                        Field("Actions", "List<ClinicalAction>", RequestAttribute)
                        ),
                    Type("InitializeClinical",
                        Attributes(
                            BindAttribute("GET /Clinical/Initialize")
                        ),
                        Field("TransactionId", "string", ProvidesAttribute, RequestAttribute),
                        Field("Diagnoses", "List<Diagnosis>", ProvidesAttribute, RequestAttribute),
                        Field("ApplicationId", "int", RequestAttribute),
                        Field("CaseId", "int", RequestAttribute),
                        Field("ExamId", "int", RequestAttribute),
                        Field("CptGroup", "short", RequestAttribute),
                        Field("Date", "DateTime", RequestAttribute),
                        Field("ClientId", "short", RequestAttribute),
                        Field("MemberDob", "DateTime", RequestAttribute),
                        Field("MemberGender", "GenderStub", RequestAttribute),
                        Field("TransactionType", "TransactionTypeStub", RequestAttribute),
                        Field("UserId", "int", RequestAttribute),
                        Field("IsOverwrite", "bool", RequestAttribute),
                        Field("CaseCptGroups", "string", RequestAttribute),
                        Field("RevisionType", "RevisionTypeStub", RequestAttribute),
                        Field("ClinicalProductId", "byte", RequestAttribute),
                        Field("DateOfService", "DateTime", RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("Comments",
                        Attributes(
                            BindAttribute("GET /Comments")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("CommentsService", "CommentsServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("Categories", "object", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("CommentsSend",
                        Attributes(
                            BindAttribute("GET /Comments/Get")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("CommentsService", "CommentsServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("UserComments", "object", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("DrugCategories",
                        Attributes(
                            BindAttribute("GET /DrugCategories")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("DrugService", "DrugServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("vtCase", "CaseStub", DependsOnAttribute, SessionAttribute),
                        Field("Categories", "object", ProvidesAttribute, SessionAttribute)
                        ),
                    Type("Drugs",
                        Attributes(
                            BindAttribute("GET /Drugs")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("DrugService", "DrugServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("vtCase", "CaseStub", DependsOnAttribute, SessionAttribute)
                        ),
                    Type("Enrollments",
                        Attributes(
                            BindAttribute("GET /Enrollments")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("MemberService", "MemberServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("RenderMemberEligibilityList", "bool", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("HomeController",
                        Attributes(
                            BindAttribute("GET /default")
                        ),
                        Field("HelpUrl", "string", ProvidesAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("CaseService", "CaseServiceStub", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("Member",
                        Attributes(
                            BindAttribute("GET /EligibilityRouter")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("MemberService", "MemberServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("MemberRequest", "object", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("Members",
                        Attributes(
                            BindAttribute("GET /Members")
                        ),
                        Field("MemberService", "MemberServiceStub", DependsOnAttribute, RequestAttribute),
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
                        Field("ClientService", "ClientServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("HealthPlanListHolder", "object", ProvidesAttribute, RequestAttribute),
                        Field("HelpUrl", "string", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("OrderInquiryListBySite",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderInquiry/OrderListBySite")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("ClientId", "short", DependsOnAttribute, RequestAttribute),
                        Field("CaseService", "CaseServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("OrderInquiryList", "object", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("DeleteCase",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/DeleteCase")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("CaseService", "CaseServiceStub", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("ProcessCaseDeletion",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/ProcessCaseDeletion")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("CaseService", "CaseServiceStub", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("DrugSelector",
                        Attributes(
                            BindAttribute("GET /DrugSelector?{CategoryId}&{DrugId}")/////////////////////////////////////////////////////////////
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("vtCase", "CaseStub", DependsOnAttribute, SessionAttribute),
                        Field("vtExam", "ExamStub", DependsOnAttribute, SessionAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("CaseService", "CaseServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("DrugService", "DrugServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("CategoryId", "string", ProvidesAttribute, RequestAttribute),
                        Field("DrugId", "string", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("EligibilityRouter",
                        Attributes(
                            BindAttribute("GET /EligibilityRouter")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("MemberService", "MemberServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("vtCase", "CaseStub", DependsOnAttribute, SessionAttribute),
                        Field("RenderMemberEligibilityList", "bool", ProvidesAttribute, RequestAttribute),
                        Field("RenderMemberInformation", "bool", ProvidesAttribute, RequestAttribute),
                        Field("RenderMemberHistoryList", "bool", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("MemberHistoryList",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/MemberHistoryList")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("MemberService", "MemberServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("CaseService", "CaseServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("PagedDataHolder", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("RenderMemberHistoryList", "bool", DependsOnAttribute, RequestAttribute),
                        Field("RenderPhysicianListForRequest", "bool", ProvidesAttribute, RequestAttribute),
                        Field("MemberPlanID", "byte?", ProvidesAttribute, RequestAttribute),
                        Field("vtCase", "CaseStub", DependsOnAttribute, SessionAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("StepNumber", "int", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("MemberInformation",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/MemberInformation")
                        ),
                        Field("RenderMemberInformation", "bool", ProvidesAttribute, RequestAttribute)
                        ),
                    Type("OrderRequest",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest")
                        ),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("CaseService", "CaseServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("ClientService", "ClientServiceStub", DependsOnAttribute, RequestAttribute),
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
                        Field("CaseService", "CaseServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("ProviderService", "ProviderServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("vtCase", "CaseStub", DependsOnAttribute, SessionAttribute),
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
                        Field("ProviderService", "ProviderServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("vtCase", "CaseStub", DependsOnAttribute, SessionAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("MemberPlanID", "byte?", DependsOnAttribute, RequestAttribute),
                        Field("RenderPhysicianListForRequest", "bool", DependsOnAttribute, RequestAttribute)
                        ),
                    Type("Members",
                        Attributes(
                            BindAttribute("GET /Providers")
                        ),
                        Field("QueryString", "string", DependsOnAttribute, RequestAttribute),
                        Field("QueryStringCollection", "NameValueCollection", DependsOnAttribute, RequestAttribute),
                        Field("User", "AIMUserStub", DependsOnAttribute, SessionAttribute),
                        Field("ClientId", "short", DependsOnAttribute, RequestAttribute),
                        Field("Data", "DataHolderStub", ProvidesAttribute, RequestAttribute),
                        Field("HelpUrl", "string", ProvidesAttribute, RequestAttribute),
                        Field("ProviderService", "ProviderServiceStub", DependsOnAttribute, RequestAttribute)
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
                        Field("DrugService", "DrugServiceStub", DependsOnAttribute, RequestAttribute),
                        Field("vtCase", "CaseStub", DependsOnAttribute, SessionAttribute),
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
                        Field("ClientId", "short", ProvidesAttribute, RequestAttribute)
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
                        Field("CaseService", "CaseServiceStub", ProvidesAttribute, RequestAttribute),
                        Field("MemberService", "MemberServiceStub", ProvidesAttribute, RequestAttribute),
                        Field("ClientService", "ClientServiceStub", ProvidesAttribute, RequestAttribute),
                        Field("UserService", "UserServiceStub", ProvidesAttribute, RequestAttribute),
                        Field("ProviderService", "ProviderServiceStub", ProvidesAttribute, RequestAttribute),
                        Field("UIState", "UIStateStub", ProvidesAttribute, SessionAttribute),
                        Field("CommentsService", "CommentsServiceStub", ProvidesAttribute, RequestAttribute),
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
                    UrlTest("GET /Case/", "GET /Case/", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "Case"),
                    UrlTest("GET /Case/variablevalue1", "GET /Case/variablevalue1", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "Case"),
                    UrlTest("GET /Case/123412423", "GET /Case/123412423", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "Case"),
                    UrlTest("GET /Case/testvalue", "GET /Case/testvalue", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "Case"),
                    UrlTest("DELETE /Case/", "DELETE /Case/", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake", "CaseDelete"),
                    UrlTest("DELETE /Case/variablevalue1", "DELETE /Case/variablevalue1", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake", "CaseDelete"),
                    UrlTest("DELETE /Case/123412423", "DELETE /Case/123412423", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake", "CaseDelete"),
                    UrlTest("DELETE /Case/testvalue", "DELETE /Case/testvalue", "Messenger", "QueryStringDeterminer", "DefaultController", "Authenticator", "AjaxDeterminer", "Fake", "CaseDelete"),
                    UrlTest("GET /Cases", "GET /Cases", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "Fake", "Cases", "AntiForgeryRequestSetter"),
                    UrlTest("GET /Clinical/GetNextStep?{TransactionId}&{DiagnosisId}", "GET /Clinical/GetNextStep?{TransactionId}&{DiagnosisId}", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "GetNextStep"),
                    UrlTest("GET /Clinical/Initialize", "GET /Clinical/Initialize", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "InitializeClinical"),
                    UrlTest("GET /Comments", "GET /Comments", "QueryStringDeterminer", "Messenger", "DefaultController", "Comments", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /Comments/Get", "GET /Comments/Get", "QueryStringDeterminer", "Messenger", "DefaultController", "Comments", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter", "CommentsSend"),
                    UrlTest("GET /DrugCategories", "GET /DrugCategories", "QueryStringDeterminer", "Messenger", "DrugCategories", "DefaultController", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /Drugs", "GET /Drugs", "QueryStringDeterminer", "Messenger", "Drugs", "DefaultController", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /Enrollments", "GET /Enrollments", "QueryStringDeterminer", "Messenger", "DefaultController", "Enrollments", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /default", "GET /default", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "HomeController", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /EligibilityRouter", "GET /EligibilityRouter", "DefaultController", "QueryStringDeterminer", "EligibilityRouter", "Member", "Messenger", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /Members", "GET /Members", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "Members", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /OrderManager/OrderInquiry", "GET /OrderManager/OrderInquiry", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderInquiry"),
                    UrlTest("GET /OrderManager/OrderInquiry/OrderListBySite", "GET /OrderManager/OrderInquiry/OrderListBySite", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderInquiry", "OrderInquiryListBySite"),
                    UrlTest("GET /OrderManager/OrderRequest/DeleteCase", "GET /OrderManager/OrderRequest/DeleteCase", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderRequest", "StepWizard", "DeleteCase"),
                    UrlTest("GET /OrderManager/OrderRequest/ProcessCaseDeletion", "GET /OrderManager/OrderRequest/ProcessCaseDeletion", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderRequest", "StepWizard", "ProcessCaseDeletion"),
                    UrlTest("GET /DrugSelector?{CategoryId}&{DrugId}", "GET /DrugSelector?{CategoryId}&{DrugId}", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "DrugSelector", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /EligibilityRouter", "GET /EligibilityRouter", "DefaultController", "QueryStringDeterminer", "EligibilityRouter", "Member", "Messenger", "AjaxDeterminer", "Authenticator", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /OrderManager/OrderRequest/MemberHistoryList", "GET /OrderManager/OrderRequest/MemberHistoryList", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderRequest", "MemberHistoryList", "StepWizard"),
                    UrlTest("GET /OrderManager/OrderRequest/MemberInformation", "GET /OrderManager/OrderRequest/MemberInformation", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderRequest", "StepWizard", "MemberInformation"),
                    UrlTest("GET /OrderManager/OrderRequest", "GET /OrderManager/OrderRequest", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "StepWizard", "OrderRequest"),
                    UrlTest("GET /OrderRequestQueue", "GET /OrderRequestQueue", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "OrderRequestQueue", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /OrderManager/OrderRequest/PhysicianSelector?{PhysicianId}", "GET /OrderManager/OrderRequest/PhysicianSelector?{PhysicianId}", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "OrderRequest", "PhysicianSelector", "StepWizard"),
                    UrlTest("GET /OrderManager/OrderRequest", "GET /OrderManager/OrderRequest", "Messenger", "QueryStringDeterminer", "DefaultController", "Fake", "Authenticator", "AjaxDeterminer", "AntiForgeryRequestSetter", "StepWizard", "OrderRequest"),
                    UrlTest("GET /Physicians", "GET /Physicians", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "Physicians", "Fake", "AntiForgeryRequestSetter"),
                    UrlTest("GET /Providers", "GET /Providers", "QueryStringDeterminer", "Messenger", "DefaultController", "AjaxDeterminer", "Authenticator", "Fake", "Members", "AntiForgeryRequestSetter"),
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





            return tests;
        }




    }
}
