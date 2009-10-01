using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;


using Bistro.Methods;
using Bistro.Tests;
using Bistro.Methods.Reflection;
using Bistro.Tests.Errors;
using BistroTests;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Descriptor;

namespace Bistro.Tests
{
    [TestFixture]
    public class MethodEngine
    {
        #region Stubs
        private class UserProfileStub
        {
            private UserProfileStub()
            { }
        }

        private class AIMUserStub
        {
            private AIMUserStub()
            { }
        }

        private class PostingStub
        {
            private PostingStub()
            { }
        }

        private class DataHolderStub
        {
            private DataHolderStub()
            { }
        }

        private class CaseServiceStub
        {
            private CaseServiceStub()
            { }
        }

        private class ClinicalQuestionStub
        {
            private ClinicalQuestionStub()
            { }
        }

        private class ClinicalActionStub
        {
            private ClinicalActionStub()
            { }
        }

        private class DiagnosisStub
        {
            private DiagnosisStub()
            { }
        }

        private class GenderStub
        {
            private GenderStub()
            { }
        }

        private class TransactionTypeStub
        {
            private TransactionTypeStub()
            { }
        }

        private class RevisionTypeStub
        {
            private RevisionTypeStub()
            { }
        }

        private class CommentsServiceStub
        {
            private CommentsServiceStub()
            { }
        }

        private class DrugServiceStub
        {
        }

        private class CaseStub
        {
        }

        private class ExamStub
        {
        }

        private class MemberServiceStub
        {
        }

        private class UIStateStub
        {
        }

        private class ClientServiceStub
        {
        }


        private class ProviderServiceStub
        {
        }

        private class UserServiceStub
        {
        }
        #endregion 

        internal class TestDescriptor
        {
            public string Name { get; set; }
            public TestTypeInfo[] Controllers { get; set; }
            public IErrorDescriptor[] Errors { get; set; }
            public UrlControllersTest[] UrlTests { get; set; }
            public BindingTest[] BindingTree { get; set; }

            public override string ToString()
            {
                return Name;
            }

            public TestDescriptor(string name, TestTypeInfo[] controllers, IErrorDescriptor[] errors, UrlControllersTest[] urlTests, params BindingTest[] bindingTree)
            {
                Name = name;
                Controllers = controllers;
                Errors = errors ?? new IErrorDescriptor[0];
                UrlTests = urlTests ?? new UrlControllersTest[0];
                BindingTree = bindingTree;
            }

            public void ValidateErrors(List<IErrorDescriptor> baseErrorsList)
            {

                Assert.IsNotNull(baseErrorsList,"Base errors list is null, something is wrong with tests initialization." );

                IErrorDescriptor[] baseErrorsArr = baseErrorsList.ToArray();

                Assert.AreEqual(baseErrorsArr.Length, Errors.Length,"Error lists have different lengths: actual-'{0}' expected-'{1}'",baseErrorsArr.Length, Errors.Length);
                    

                for (int i = 0; i < baseErrorsArr.Length; i++)
                {
                    baseErrorsArr[i].Validate(Errors[i]);
                }
                
            }

        }
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

        #region Node creation methods

        private BindingTest Node(string url, params ControllerTest[] controllers)
        {
            return Node(url, controllers, new BindingTest[] { });
        }

        private BindingTest Node(string url, params string[] controllers)
        {
            return Node(url, Controllers(controllers), new BindingTest[] { });
        }

        private BindingTest Node(string url, ControllerTest[] controllers, params BindingTest[] children)
        {
            return new BindingTest(url, controllers, children);
        }

        #endregion

        #region Controller tests creation methods
        private ControllerTest[] Controllers() { return new ControllerTest[0]; }

        private ControllerTest[] Controllers(params ControllerTest[] controllers) { return controllers; }

        private ControllerTest[] Controllers(params string[] controllers) 
        {
            List<ControllerTest> result = new List<ControllerTest>();
            foreach (string name in controllers)
                result.Add(new ControllerTest(name, 0));
            return result.ToArray(); 
        }

        private ControllerTest Controller(string name, int seq) { return new ControllerTest(name, seq); }

        #endregion

        List<TestDescriptor> tests = new List<TestDescriptor>();

        #region Test Creation Methods

        private void NewTest(string name, TestTypeInfo[] types, params BindingTest[] nodes)
        {
            tests.Add(new TestDescriptor(name, types,null,null, nodes));
        }

        private void NewTestWithUrl(string name, TestTypeInfo[] types, UrlControllersTest[] urlTests, IErrorDescriptor[] errors, params BindingTest[] nodes)
        {
            tests.Add(new TestDescriptor(name, types, errors, urlTests, nodes));
        }





        private void NewErrorTest(string name, TestTypeInfo[] types,IErrorDescriptor[] errors , params BindingTest[] nodes)
        {
            tests.Add(new TestDescriptor(name, types, errors,null, nodes));
        }

        #endregion

        #region Errors

        private IErrorDescriptor[] Errors(params IErrorDescriptor[] errors)
        {
            return errors;
        }


        private ErrorInvalidBinding InvalidBindingError(string ctrlrType, params string[] bindings)
        {
            return new ErrorInvalidBinding(ctrlrType, bindings);
        }

        private ErrorResourceLoop ResourceLoopError(string fullBindUrl, params string[] controllers)
        {
            return new ErrorResourceLoop(fullBindUrl, controllers);
        }

        private ErrorMissingProvider MissingProviderErrod(string fullBindUrl, string resName, params string[] controllers)
        {
            return new ErrorMissingProvider(fullBindUrl, resName, controllers);
        }

        private ErrorInconsResourceType InconsistentResourceTypeError(string fullBindUrl, string resName, params string[] values)
        {
            return new ErrorInconsResourceType(fullBindUrl, resName, values);
        }



        #endregion

        TestEngine e;

        [TestFixtureSetUp]
        public void setup()
        {
            errorList = new List<IErrorDescriptor>();
            tests.Clear();
            e = new TestEngine();
            e.OnInvalidBinding += new TestEngine.ControllerEvent(e_OnInvalidBinding);
            e.OnResourceLoop += new TestEngine.MethodEvent(e_OnResourceLoop);
            e.OnMissingProvider += new TestEngine.MethodEvent(e_OnMissingProvider);
            e.OnInconsistentResourceType += new TestEngine.MethodResourceEvent(e_OnInconsistentResourceType);
        }


        #region Error handlers

        void e_OnMissingProvider(object sender, TestEngine.MethodEventArgs e)
        {
            List<string> loopControllers = new List<string>();
            foreach (ITypeInfo ctrType in e.Controllers)
            {
                loopControllers.Add(ctrType.FullName);
            }

            ErrorMissingProvider errMissProv = new ErrorMissingProvider(e.MethodUrl, e.ResourceName, loopControllers.ToArray());
            errorList.Add(errMissProv);

        }

        void e_OnInconsistentResourceType(object sender, TestEngine.MethodResourceEventArgs e)
        {
            List<string> loopValues = new List<string>();
            foreach (ControllerType ctrType in e.Controllers)
            {
                loopValues.Add(ctrType.Type.FullName);
                loopValues.Add(ctrType.GetResourceType(e.ResourceName));
            }

            ErrorInconsResourceType errInconsResType = new ErrorInconsResourceType(e.MethodUrl, e.ResourceName, loopValues.ToArray());
            errorList.Add(errInconsResType);
        }

        void e_OnResourceLoop(object sender, TestEngine.MethodEventArgs e)
        {
            List<string> loopControllers = new List<string>();
            foreach(ITypeInfo ctrType in e.Controllers)
            {
                loopControllers.Add(ctrType.FullName);
            }

            ErrorResourceLoop errResLoop = new ErrorResourceLoop(e.MethodUrl, loopControllers.ToArray());
            errorList.Add(errResLoop);
        }

        void e_OnInvalidBinding(object sender, TestEngine.ControllerEventArgs e)
        {
            ErrorInvalidBinding errBind = new ErrorInvalidBinding(e.Controller.FullName, e.Args);
            errorList.Add(errBind);
        }




        #endregion

        [Test, TestCaseSource("TestSource")]
        public void run(object test)
        {
            realTest(test);
        }


        private List<IErrorDescriptor> errorList; 


        // The NUnit will not run a test for a method which is not public 
        // and I do not want to make TestDescriptor public
        void realTest(object test)
        {
            errorList.Clear();
            



            TestDescriptor descriptor = (TestDescriptor)test;
            e.ProcessControllers(new List<string>(), new List<ITypeInfo>(descriptor.Controllers));

            descriptor.ValidateErrors(errorList);


//            string builtTree = GetTreeResultText(string.Empty, e.Root);
            

            new BindingTest("", descriptor.BindingTree).Validate(e.Root);

            List<string> remove = new List<string>();
            foreach (TestTypeInfo c in descriptor.Controllers)
                remove.Add(c.FullName);
            e.ProcessControllers(remove, new List<ITypeInfo>());
            new BindingTest("").Validate(e.Root);
        }

        #region Some generating stuff

        private string GetTreeRecursive(string spacer, Binding leaf)
        {
            string result = String.Format("{1}{2} {0} \r\n",leaf.BindingUrl,spacer,leaf.Verb);
            foreach (Controller ctrlr in leaf.Controllers)
            {
                result += String.Format("{1}    Controller: {0} \r\n", ctrlr.Type.Name,spacer);
            }
            foreach (Binding bnd in leaf.Bindings)
            {
                result += GetTreeRecursive(spacer + "   ",bnd);
            }
            return result;
        }

        private string GetTreeResultText(string spacer, Binding leaf)
        {
            string result = String.Format("{1}Node(\"{2}{0}\",Controllers( \r\n", leaf.BindingUrl, spacer, spacer == "   " ?  leaf.Verb+" ":"");
            bool first = false;
            foreach (Controller ctrlr in (leaf.Controllers.OrderByDescending(ctrlr => ctrlr.SeqNumber)))
            {
                result += String.Format("{1}    {2}\"{0}\" \r\n", ctrlr.Type.Name, spacer, (first ? "," : ""));
                first = true;
            }

            result += String.Format("{0}){1}\r\n", spacer,(leaf.Bindings.Count>0? ",":"" ));
            bool first1 = false;
            foreach (Binding bnd in leaf.Bindings)
            {
                result += (first1 ? String.Format("{0},\r\n",spacer) : "") + GetTreeResultText(spacer + "   ", bnd);
                first1 = true;
            }
            result += String.Format("{0})\r\n", spacer);


            return result;
        }

        #endregion


        internal IList<TestDescriptor> TestSource()
        {
            NewTest("ValidUrls",
                Types(
                    Type(
                        "TestController",
                        BindAttribute("/auth/signin?{originalRequest}"),
                        BindAttribute("/postings/{contentType}?{firstTime}")
                        )
                    ),
                Node("* /auth/signin?{originalRequest}", "TestController"),
                Node("* /postings/{contentType}?{firstTime}", "TestController")
                );

            #region SRxPortal

            NewTest("SRxPortal - Big Test",
                Types(
                    Type("Pageable",
                        Attributes(
                            BindAttribute("GET /?/Pageable?{PageNumber}&{PageSize}")
                        ),
                        Field("QueryStringCollection","NameValueCollection",DependsOnAttribute,RequestAttribute),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("PageNumber","int",ProvidesAttribute,RequestAttribute),
                        Field("PageSize","int",ProvidesAttribute,RequestAttribute),
                        Field("TotalItemCount","int",ProvidesAttribute,RequestAttribute),
                        Field("HasPreviousPage","bool",ProvidesAttribute,RequestAttribute),
                        Field("HasNextPage","bool",ProvidesAttribute,RequestAttribute),
                        Field("PageCount","int",ProvidesAttribute,RequestAttribute),
                        Field("IsPaged","bool",ProvidesAttribute,RequestAttribute),
                        Field("PageNumbers","ArrayList",ProvidesAttribute,RequestAttribute),
                        Field("PagingInfo","string",DependsOnAttribute,RequestAttribute),
                        Field("Data","DataHolderStub",DependsOnAttribute,RequestAttribute)
                        ),

                    Type("Sortable",
                        Attributes(
                            BindAttribute("GET /?/Sortable?{OrderBy}&{Direction}")
                        ),
                        Field("QueryStringCollection","NameValueCollection",DependsOnAttribute,RequestAttribute),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("OrderBy","string",ProvidesAttribute,RequestAttribute),
                        Field("Direction","string",ProvidesAttribute,RequestAttribute),
                        Field("IsSorted","bool",ProvidesAttribute,RequestAttribute),
                        Field("PagingInfo","string",ProvidesAttribute,RequestAttribute),
                        Field("Data","DataHolderStub",DependsOnAttribute,RequestAttribute)
                        ),
                    Type("Authenticator",
                        Attributes(
                            BindAttribute("/?")
                        ),
                        Field("User","AIMUserStub",ProvidesAttribute,SessionAttribute),
                        Field("PermissionManager","object",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("Case",
                        Attributes(
                            BindAttribute("GET /Case/{CaseID}")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("QueryStringCollection","NameValueCollection",DependsOnAttribute,RequestAttribute),
                        Field("CaseID","string",ProvidesAttribute,RequestAttribute),
                        Field("User","AIMUserStub",DependsOnAttribute,SessionAttribute),
                        Field("CaseService","CaseServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("Data","DataHolderStub",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("CaseDelete",
                        Attributes(
                            BindAttribute("DELETE /Case/{CaseID}")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("QueryStringCollection","NameValueCollection",DependsOnAttribute,RequestAttribute),
                        Field("CaseID","string",ProvidesAttribute,RequestAttribute),
                        Field("User","AIMUserStub",DependsOnAttribute,SessionAttribute),
                        Field("CaseService","CaseServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("Data","DataHolderStub",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("Cases",
                        Attributes(
                            BindAttribute("GET /Cases")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("QueryStringCollection","NameValueCollection",DependsOnAttribute,RequestAttribute),
                        Field("CaseService","CaseServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("User","AIMUserStub",DependsOnAttribute,SessionAttribute),
                        Field("ClientId","short",DependsOnAttribute,RequestAttribute),
                        Field("Data","DataHolderStub",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("GetNextStep",
                        Attributes(
                            BindAttribute("GET /Clinical/GetNextStep?{TransactionId}&{DiagnosisId}")
                        ),
                        Field("TransactionId","string",RequestAttribute),
                        Field("DiagnosisId","string",RequestAttribute),
                        Field("Questions","List<ClinicalQuestionStub>",RequestAttribute),
                        Field("Actions","List<ClinicalAction>",RequestAttribute)
                        ),
                    Type("InitializeClinical",
                        Attributes(
                            BindAttribute("GET /Clinical/Initialize")
                        ),
                        Field("TransactionId","string",ProvidesAttribute,RequestAttribute),
                        Field("Diagnoses","List<Diagnosis>",ProvidesAttribute,RequestAttribute),
                        Field("ApplicationId","int",RequestAttribute),
                        Field("CaseId","int",RequestAttribute),
                        Field("ExamId","int",RequestAttribute),
                        Field("CptGroup","short",RequestAttribute),
                        Field("Date","DateTime",RequestAttribute),
                        Field("ClientId","short",RequestAttribute),
                        Field("MemberDob","DateTime",RequestAttribute),
                        Field("MemberGender","GenderStub",RequestAttribute),
                        Field("TransactionType","TransactionTypeStub",RequestAttribute),
                        Field("UserId","int",RequestAttribute),
                        Field("IsOverwrite","bool",RequestAttribute),
                        Field("CaseCptGroups","string",RequestAttribute),
                        Field("RevisionType","RevisionTypeStub",RequestAttribute),
                        Field("ClinicalProductId","byte",RequestAttribute),
                        Field("DateOfService","DateTime",RequestAttribute),
                        Field("Data","DataHolderStub",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("Comments",
                        Attributes(
                            BindAttribute("GET /Comments")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("CommentsService","CommentsServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("Categories","object",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("CommentsSend",
                        Attributes(
                            BindAttribute("GET /Comments/Get")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("CommentsService","CommentsServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("UserComments","object",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("DrugCategories",
                        Attributes(
                            BindAttribute("GET /DrugCategories")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("DrugService","DrugServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("vtCase","CaseStub",DependsOnAttribute,SessionAttribute),
                        Field("Categories","object",ProvidesAttribute,SessionAttribute)
                        ),
                    Type("Drugs",
                        Attributes(
                            BindAttribute("GET /Drugs")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("DrugService","DrugServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("vtCase","CaseStub",DependsOnAttribute,SessionAttribute)
                        ),
                    Type("Enrollments",
                        Attributes(
                            BindAttribute("GET /Enrollments")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("MemberService","MemberServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("Data","DataHolderStub",ProvidesAttribute,RequestAttribute),
                        Field("RenderMemberEligibilityList","bool",DependsOnAttribute,RequestAttribute)
                        ),
                    Type("HomeController",
                        Attributes(
                            BindAttribute("GET /default")
                        ),
                        Field("HelpUrl","string",ProvidesAttribute,RequestAttribute),
                        Field("User","AIMUserStub",DependsOnAttribute,SessionAttribute),
                        Field("Data","DataHolderStub",ProvidesAttribute,RequestAttribute),
                        Field("CaseService","CaseServiceStub",DependsOnAttribute,RequestAttribute)
                        ),
                    Type("Member",
                        Attributes(
                            BindAttribute("GET /EligibilityRouter")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("MemberService","MemberServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("MemberRequest","object",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("Members",
                        Attributes(
                            BindAttribute("GET /Members")
                        ),
                        Field("MemberService","MemberServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("QueryStringCollection","NameValueCollection",DependsOnAttribute,RequestAttribute),
                        Field("User","AIMUserStub",DependsOnAttribute,SessionAttribute),
                        Field("Data","DataHolderStub",ProvidesAttribute,RequestAttribute),
                        Field("HelpUrl","string",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("OrderInquiry",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderInquiry")
                        ),
                        Field("User","AIMUserStub",DependsOnAttribute,SessionAttribute),
                        Field("UIState","UIStateStub",SessionAttribute),
                        Field("ClientService","ClientServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("HealthPlanListHolder","object",ProvidesAttribute,RequestAttribute),
                        Field("HelpUrl","string",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("OrderInquiryListBySite",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderInquiry/OrderListBySite")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("ClientId","short",DependsOnAttribute,RequestAttribute),
                        Field("CaseService","CaseServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("OrderInquiryList","object",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("DeleteCase",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/DeleteCase")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("CaseService","CaseServiceStub",DependsOnAttribute,RequestAttribute)
                        ),
                    Type("ProcessCaseDeletion",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/ProcessCaseDeletion")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("CaseService","CaseServiceStub",DependsOnAttribute,RequestAttribute)
                        ),
                    Type("DrugSelector",
                        Attributes(
                            BindAttribute("GET /DrugSelector?{CategoryId}&{DrugId}")/////////////////////////////////////////////////////////////
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("vtCase","CaseStub",DependsOnAttribute,SessionAttribute),
                        Field("vtExam","ExamStub",DependsOnAttribute,SessionAttribute),
                        Field("User","AIMUserStub",DependsOnAttribute,SessionAttribute),
                        Field("CaseService","CaseServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("DrugService","DrugServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("CategoryId","string",ProvidesAttribute,RequestAttribute),
                        Field("DrugId","string",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("EligibilityRouter",
                        Attributes(
                            BindAttribute("GET /EligibilityRouter")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("MemberService","MemberServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("Data","DataHolderStub",ProvidesAttribute,RequestAttribute),
                        Field("vtCase","CaseStub",DependsOnAttribute,SessionAttribute),
                        Field("RenderMemberEligibilityList","bool",ProvidesAttribute,RequestAttribute),
                        Field("RenderMemberInformation","bool",ProvidesAttribute,RequestAttribute),
                        Field("RenderMemberHistoryList","bool",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("MemberHistoryList",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/MemberHistoryList")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("MemberService","MemberServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("CaseService","CaseServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("PagedDataHolder","DataHolderStub",ProvidesAttribute,RequestAttribute),
                        Field("RenderMemberHistoryList","bool",DependsOnAttribute,RequestAttribute),
                        Field("RenderPhysicianListForRequest","bool",ProvidesAttribute,RequestAttribute),
                        Field("MemberPlanID","byte?",ProvidesAttribute,RequestAttribute),
                        Field("vtCase","CaseStub",DependsOnAttribute,SessionAttribute),
                        Field("User","AIMUserStub",DependsOnAttribute,SessionAttribute),
                        Field("StepNumber","int",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("MemberInformation",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/MemberInformation")
                        ),
                        Field("RenderMemberInformation","bool",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("OrderRequest",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest")
                        ),
                        Field("User","AIMUserStub",DependsOnAttribute,SessionAttribute),
                        Field("CaseService","CaseServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("ClientService","ClientServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("UIState","UIStateStub",SessionAttribute),
                        Field("HealthPlanListHolder","object",ProvidesAttribute,RequestAttribute),
                        Field("Data","DataHolderStub",ProvidesAttribute,RequestAttribute),
                        Field("CurrentDate","string",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("OrderRequestQueue",
                        Attributes(
                            BindAttribute("GET /OrderRequestQueue")
                        ),
                        Field("User","AIMUserStub",DependsOnAttribute,SessionAttribute),
                        Field("Data","DataHolderStub",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("PhysicianSelector",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest/PhysicianSelector?{PhysicianId}")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("CaseService","CaseServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("ProviderService","ProviderServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("vtCase","CaseStub",DependsOnAttribute,SessionAttribute),
                        Field("Physician","object",ProvidesAttribute,RequestAttribute),
                        Field("PhysicianId","int",ProvidesAttribute,RequestAttribute),
                        Field("User","AIMUserStub",DependsOnAttribute,SessionAttribute),
                        Field("StepNumber","int",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("StepWizard",
                        Attributes(
                            BindAttribute("GET /OrderManager/OrderRequest")/////////////////////////////////////////////////////
                        ),
                        Field("StepNumber","int",DependsOnAttribute,RequestAttribute)
                        ),
                    Type("Physicians",
                        Attributes(
                            BindAttribute("GET /Physicians")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("ProviderService","ProviderServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("User","AIMUserStub",DependsOnAttribute,SessionAttribute),
                        Field("vtCase","CaseStub",DependsOnAttribute,SessionAttribute),
                        Field("Data","DataHolderStub",ProvidesAttribute,RequestAttribute),
                        Field("MemberPlanID","byte?",DependsOnAttribute,RequestAttribute),
                        Field("RenderPhysicianListForRequest","bool",DependsOnAttribute,RequestAttribute)
                        ),
                    Type("Members",
                        Attributes(
                            BindAttribute("GET /Providers")
                        ),
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("QueryStringCollection","NameValueCollection",DependsOnAttribute,RequestAttribute),
                        Field("User","AIMUserStub",DependsOnAttribute,SessionAttribute),
                        Field("ClientId","short",DependsOnAttribute,RequestAttribute),
                        Field("Data","DataHolderStub",ProvidesAttribute,RequestAttribute),
                        Field("HelpUrl","string",ProvidesAttribute,RequestAttribute),
                        Field("ProviderService","ProviderServiceStub",DependsOnAttribute,RequestAttribute)
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
                        Field("QueryString","string",DependsOnAttribute,RequestAttribute),
                        Field("DrugService","DrugServiceStub",DependsOnAttribute,RequestAttribute),
                        Field("vtCase","CaseStub",DependsOnAttribute,SessionAttribute),
                        Field("Data","DataHolderStub",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("AjaxDeterminer",
                        Attributes(
                            BindAttribute("/?")
                        ),
                        Field("IsAjaxRequest","bool",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("AntiForgeryRequestHandler",
                        Attributes(
                            BindAttribute("POST /?")
                        ),
                        Field("AntiForgeryRequestTokenCookie","string",DependsOnAttribute,RequestAttribute),
                        Field("AntiForgeryRequestToken","string",DependsOnAttribute,RequestAttribute,FormFieldAttribute)
                        ),
                    Type("AntiForgeryRequestSetter",
                        Attributes(
                            BindAttribute("GET /?")
                        ),
                        Field("AntiForgeryRequestTokenCookie","string",DependsOnAttribute,RequestAttribute),
                        Field("AntiForgeryRequestToken","string",DependsOnAttribute,RequestAttribute,FormFieldAttribute)
                        ),
                    Type("Fake",
                        Attributes(
                            BindAttribute("/?")
                        ),
                        Field("UserID","int",ProvidesAttribute,RequestAttribute),
                        Field("ClientId","short",ProvidesAttribute,RequestAttribute)
                        ),
                    Type("Messenger",
                        Attributes(
                            BindAttribute("/?")
                        ),
                        Field("MessageList","object",ProvidesAttribute,RequestAttribute),
                        Field("MessageID","string",DependsOnAttribute,RequestAttribute)
                        ),
                    Type("QueryStringDeterminer",
                        Attributes(
                            BindAttribute("/?")
                        ),
                        Field("QueryStringCollection","NameValueCollection",ProvidesAttribute,RequestAttribute),
                        Field("QueryString","string",ProvidesAttribute,RequestAttribute)
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
                        Field("CaseService","CaseServiceStub",ProvidesAttribute,RequestAttribute),
                        Field("MemberService","MemberServiceStub",ProvidesAttribute,RequestAttribute),
                        Field("ClientService","ClientServiceStub",ProvidesAttribute,RequestAttribute),
                        Field("UserService","UserServiceStub",ProvidesAttribute,RequestAttribute),
                        Field("ProviderService","ProviderServiceStub",ProvidesAttribute,RequestAttribute),
                        Field("UIState","UIStateStub",ProvidesAttribute,SessionAttribute),
                        Field("CommentsService","CommentsServiceStub",ProvidesAttribute,RequestAttribute),
                        Field("Root","string",ProvidesAttribute,RequestAttribute)
                        )
                    ),
   Node("GET /?", Controllers(
       "DefaultController"
       , "RootRedirect"
       , "QueryStringDeterminer"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "Authenticator"
   ),
      Node("/Pageable?{PageNumber}&{PageSize}", Controllers(
          "DefaultController"
          , "RootRedirect"
          , "Messenger"
          , "Fake"
          , "AntiForgeryRequestSetter"
          , "AjaxDeterminer"
          , "Authenticator"
          , "QueryStringDeterminer"
          , "Pageable"
      )
      )
   ,
      Node("/Sortable?{OrderBy}&{Direction}", Controllers(
          "DefaultController"
          , "RootRedirect"
          , "Messenger"
          , "Fake"
          , "AntiForgeryRequestSetter"
          , "AjaxDeterminer"
          , "Authenticator"
          , "QueryStringDeterminer"
          , "Sortable"
      )
      )
   )
,
   Node("* /?", Controllers(
       "DefaultController"
       , "RootRedirect"
       , "QueryStringDeterminer"
       , "Messenger"
       , "Fake"
       , "AjaxDeterminer"
       , "Authenticator"
   )
   )
,
   Node("GET /Case/{CaseID}", Controllers(
       "RootRedirect"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "DefaultController"
       , "QueryStringDeterminer"
       , "Authenticator"
       , "Case"
   )
   )
,
   Node("DELETE /Case/{CaseID}", Controllers(
       "RootRedirect"
       , "Messenger"
       , "Fake"
       , "AjaxDeterminer"
       , "DefaultController"
       , "QueryStringDeterminer"
       , "Authenticator"
       , "CaseDelete"
   )
   )
,
   Node("GET /Cases", Controllers(
       "RootRedirect"
       , "Messenger"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "Fake"
       , "DefaultController"
       , "QueryStringDeterminer"
       , "Authenticator"
       , "Cases"
   )
   )
,
   Node("GET /Clinical", Controllers(
   ),
      Node("/GetNextStep?{TransactionId}&{DiagnosisId}", Controllers(
          "DefaultController"
          , "RootRedirect"
          , "QueryStringDeterminer"
          , "Messenger"
          , "Fake"
          , "AntiForgeryRequestSetter"
          , "AjaxDeterminer"
          , "GetNextStep"
          , "Authenticator"
      )
      )
   ,
      Node("/Initialize", Controllers(
          "DefaultController"
          , "RootRedirect"
          , "QueryStringDeterminer"
          , "Messenger"
          , "Fake"
          , "AntiForgeryRequestSetter"
          , "AjaxDeterminer"
          , "InitializeClinical"
          , "Authenticator"
      )
      )
   )
,
   Node("GET /Comments", Controllers(
       "RootRedirect"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "DefaultController"
       , "QueryStringDeterminer"
       , "Comments"
       , "Authenticator"
   ),
      Node("/Get", Controllers(
          "RootRedirect"
          , "Messenger"
          , "Fake"
          , "AntiForgeryRequestSetter"
          , "AjaxDeterminer"
          , "DefaultController"
          , "QueryStringDeterminer"
          , "CommentsSend"
          , "Comments"
          , "Authenticator"
      )
      )
   )
,
   Node("GET /DrugCategories", Controllers(
       "DefaultController"
       , "RootRedirect"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "QueryStringDeterminer"
       , "DrugCategories"
       , "Authenticator"
   )
   )
,
   Node("GET /Drugs", Controllers(
       "DefaultController"
       , "RootRedirect"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "QueryStringDeterminer"
       , "Drugs"
       , "Authenticator"
   )
   )
,
   Node("GET /Enrollments", Controllers(
       "RootRedirect"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "DefaultController"
       , "QueryStringDeterminer"
       , "Enrollments"
       , "Authenticator"
   )
   )
,
   Node("GET /default", Controllers(
       "RootRedirect"
       , "QueryStringDeterminer"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "DefaultController"
       , "Authenticator"
       , "HomeController"
   )
   )
,
   Node("GET /EligibilityRouter", Controllers(
       "RootRedirect"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "DefaultController"
       , "QueryStringDeterminer"
       , "EligibilityRouter"
       , "Member"
       , "Authenticator"
   )
   )
,
   Node("GET /Members", Controllers(
       "DefaultController"
       , "RootRedirect"
       , "QueryStringDeterminer"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "Authenticator"
   )
   )
,
   Node("GET /OrderManager", Controllers(
   ),
      Node("/OrderInquiry", Controllers(
          "RootRedirect"
          , "QueryStringDeterminer"
          , "Messenger"
          , "Fake"
          , "AntiForgeryRequestSetter"
          , "AjaxDeterminer"
          , "DefaultController"
          , "Authenticator"
          , "OrderInquiry"
      ),
         Node("/OrderListBySite", Controllers(
             "RootRedirect"
             , "Messenger"
             , "AntiForgeryRequestSetter"
             , "AjaxDeterminer"
             , "DefaultController"
             , "Fake"
             , "QueryStringDeterminer"
             , "OrderInquiryListBySite"
             , "Authenticator"
             , "OrderInquiry"
         )
         )
      )
   ,
      Node("/OrderRequest", Controllers(
          "RootRedirect"
          , "QueryStringDeterminer"
          , "Messenger"
          , "Fake"
          , "AntiForgeryRequestSetter"
          , "AjaxDeterminer"
          , "StepWizard"
          , "DefaultController"
          , "Authenticator"
          , "OrderRequest"
      ),
         Node("/DeleteCase", Controllers(
             "RootRedirect"
             , "Messenger"
             , "Fake"
             , "AntiForgeryRequestSetter"
             , "AjaxDeterminer"
             , "StepWizard"
             , "DefaultController"
             , "Authenticator"
             , "OrderRequest"
             , "QueryStringDeterminer"
             , "DeleteCase"
         )
         )
      ,
         Node("/ProcessCaseDeletion", Controllers(
             "RootRedirect"
             , "Messenger"
             , "Fake"
             , "AntiForgeryRequestSetter"
             , "AjaxDeterminer"
             , "StepWizard"
             , "DefaultController"
             , "Authenticator"
             , "OrderRequest"
             , "QueryStringDeterminer"
             , "ProcessCaseDeletion"
         )
         )
      ,
         Node("/MemberHistoryList", Controllers(
             "RootRedirect"
             , "Messenger"
             , "Fake"
             , "AntiForgeryRequestSetter"
             , "AjaxDeterminer"
             , "DefaultController"
             , "QueryStringDeterminer"
             , "Authenticator"
             , "MemberHistoryList"
             , "StepWizard"
             , "OrderRequest"
         )
         )
      ,
         Node("/MemberInformation", Controllers(
             "RootRedirect"
             , "QueryStringDeterminer"
             , "Messenger"
             , "Fake"
             , "AntiForgeryRequestSetter"
             , "AjaxDeterminer"
             , "StepWizard"
             , "DefaultController"
             , "Authenticator"
             , "OrderRequest"
             , "MemberInformation"
         )
         )
      ,
         Node("/PhysicianSelector?{PhysicianId}", Controllers(
             "RootRedirect"
             , "Messenger"
             , "Fake"
             , "AntiForgeryRequestSetter"
             , "AjaxDeterminer"
             , "QueryStringDeterminer"
             , "DefaultController"
             , "Authenticator"
             , "PhysicianSelector"
             , "StepWizard"
             , "OrderRequest"
         )
         )
      )
   )
,
   Node("GET /DrugSelector?{CategoryId}&{DrugId}", Controllers(
       "RootRedirect"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "DefaultController"
       , "QueryStringDeterminer"
       , "Authenticator"
       , "DrugSelector"
   )
   )
,
   Node("GET /OrderRequestQueue", Controllers(
       "DefaultController"
       , "RootRedirect"
       , "QueryStringDeterminer"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "Authenticator"
       , "OrderRequestQueue"
   )
   )
,
   Node("GET /Physicians", Controllers(
       "RootRedirect"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "DefaultController"
       , "QueryStringDeterminer"
       , "Authenticator"
       , "Physicians"
   )
   )
,
   Node("GET /Providers", Controllers(
       "RootRedirect"
       , "Messenger"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "DefaultController"
       , "Fake"
       , "QueryStringDeterminer"
       , "Authenticator"
       , "Members"
   )
   )
,
   Node("GET /QUnit", Controllers(
       "DefaultController"
       , "RootRedirect"
       , "QueryStringDeterminer"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "QUnit"
       , "Authenticator"
   )
   )
,
   Node("GET /RequestItems", Controllers(
       "DefaultController"
       , "RootRedirect"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestSetter"
       , "AjaxDeterminer"
       , "QueryStringDeterminer"
       , "RequestItems"
       , "Authenticator"
   )
   )
,
   Node("POST /?", Controllers(
       "DefaultController"
       , "RootRedirect"
       , "QueryStringDeterminer"
       , "Messenger"
       , "Fake"
       , "AntiForgeryRequestHandler"
       , "AjaxDeterminer"
       , "Authenticator"
   )
   )
,
   Node("* /", Controllers(
       "DefaultController"
       , "RootRedirect"
       , "QueryStringDeterminer"
       , "Messenger"
       , "Fake"
       , "AjaxDeterminer"
       , "Authenticator"
   )
   )
                    );










            #endregion

            #region NoRecruiters-WFS
            NewTest("zzz-NoRecruiters-WFS",
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
                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),
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
                        Field("shortName", "string"),
                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute)
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
                        Field("comment", "string", RequestAttribute, FormFieldAttribute),
                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute)
                        ),









                    Type("ResumeDisplay",
                        Attributes(
                            BindAttribute("GET /posting/resume/byname/{shortName}")
                        ),
                        Field("shortName", "string"),
                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),
                        Field("posting", "PostingStub", RequestAttribute, RequiresAttribute)
                        ),
                    Type("ResumeUpdate",
                        Attributes(
                            BindAttribute("POST /posting/resume/byname/{shortName}")
                        ),
                        Field("shortName", "string"),
                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),
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
                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),
                        Field("posting", "PostingStub", RequestAttribute, RequiresAttribute)
                        ),
                    Type("Manage",
                        Attributes(
                            BindAttribute("GET /posting/manage")
                        ),
                        Field("unpublished", "List<string>", RequestAttribute),
                        Field("published", "List<string>", RequestAttribute),

                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute)
                        ),
                    Type("ViewAllApplicatints",
                        Attributes(
                            BindAttribute("GET /posting/ad/applicants/byId/{adId}")
                        ),
                        Field("adId", "string", RequestAttribute),
                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),
                        Field("applicants", "List<string>", RequestAttribute)
                        ),
                    Type("AdDisplay",
                        Attributes(
                            BindAttribute("GET /posting/ad/byname/{shortName}")
                        ),
                        Field("shortName", "string"),
                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),
                        Field("posting", "PostingStub", RequestAttribute, RequiresAttribute)
                        ),
                    Type("AdUpdate",
                        Attributes(
                            BindAttribute("POST /posting/ad/byname/{shortName}")
                        ),
                        Field("shortName", "string"),
                        Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute),
                        Field("posting", "PostingStub", RequestAttribute, RequiresAttribute),

                        Field("heading", "string", FormFieldAttribute, RequestAttribute),
                        Field("tags", "string", FormFieldAttribute, RequestAttribute),
                        Field("detail", "string", FormFieldAttribute, RequestAttribute),
                        Field("published", "string", FormFieldAttribute, RequestAttribute)
                        )

                    ),

                    Node("/?", Controllers())
                    );

            #endregion


            #region Branching
            NewTest("Branching",
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
                Node("* /a",Controllers(),
                    Node("/b1",Controllers(),
                        Node("/c1","C1"),
                        Node("/c2","C2")
                        ),
                    Node("/b2",Controllers(),
                        Node("/c1","C3"),
                        Node("/c2","C4")
                        )
                    )
                );
            #endregion

            #region tree - one controller - 2 bindings (flat)
            NewTest(
                "tree - one controller - 2 bindings (flat)",
                Types(Type(
                    "Controller1",
                    Attributes(
                        BindAttribute("/"),
                        BindAttribute("/path1"))
                    )),
                Node("* /", "Controller1"),
                Node("* /path1", "Controller1")
            );
            #endregion

            #region Imported - home/root
            NewTest(
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
                Node("* /?", "HomeUrlController2", "HomeUrlController1") // Actually that's not so good - controllers may come in any order here.
                );
            #endregion

            #region Imported - /hello/...
            NewTest("Imported - /hello/...",
                Types(
                    Type("HelloYouController1", BindAttribute("/hello/?/you")),
                    Type("HelloYouController2", BindAttribute("/hello/*/you"))
                ),
                Node("* /hello", Controllers(),
                    Node("/*/you", "HelloYouController2", "HelloYouController1"),
                    Node("/?/you", "HelloYouController1")
                    )
                );
            #endregion

            #region Imported - /order/world/new

            NewTest("Imported - /order/world/new",
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
                Node("* /order/world/new", "OrderController7", "OrderController5", "OrderController2", "OrderController1", "OrderController4", "OrderController3", "OrderController6")
            );
            #endregion


            #region Imported - /one_little_url
            NewTest(
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
                Node("* /one_little_url", "littleController2", "littleController1")
            );
            #endregion

            #region Imported - /little_url/more
            NewTest(
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
                Node("* /little_url/more", "littleController3", "littleController5", "littleController4")
            );
            #endregion

            #region Imported - GET/hi/...
            NewTest(
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
                Node("GET /hi",Controllers(),
                    Node("/*/world",Controllers(),
                        Node("/*/now", "hiController4", "hiController3"),
                        Node("/?/now","hiController3"),
                        Node("/a",Controllers("hiController7","hiController6"),
                            Node("/*","hiController7","hiController6"),
                            Node("/now", "hiController7", "hiController6", "hiController5", "hiController4", "hiController3")
                            )
                        ),
                    Node("/new",Controllers(),
                        Node("/*/*/now","hiController2"),
                        Node("/world/a", "hiController7", "hiController6", "hiController1")
                        )
                    )
                );
            #endregion

            #region Imported - GET/hi/... - 0
            NewTest(
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
                Node("GET /hi/*/world", Controllers(),
                    Node("/*/now", "hiController4"),
                    Node("/a/*", "hiController7")
                    )
                );
            #endregion


            #region Imported - GET/hi/... - 1
            NewTest(
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
                Node("GET /hi", Controllers(),
                    Node("/*/world", Controllers(),
                        Node("/*/now", "hiController4", "hiController3"),
                        Node("/?/now", "hiController3"),
                        Node("/a", Controllers("hiController7", "hiController6"),
                            Node("/*", "hiController7", "hiController6")
                            )
                        ),
                    Node("/new/*/*/now",  "hiController2"
                        )
                    )
                );
            #endregion

            #region Imported - GET/hi/... - 2
            NewTest(
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
                Node("GET /hi/*/world", Controllers(),
                        Node("/*/now", "hiController4", "hiController3"),
                        Node("/?/now", "hiController3"),
                        Node("/a", Controllers("hiController7", "hiController6"),
                            Node("/*", "hiController7","hiController6")//,
                            )
                    )
                );
            #endregion

            #region Imported - GET/hi/... - 2 - 1
            NewTest(
                "Imported - GET/hi/... - 2 - 1",
                Types(
                    Type(
                        "hiController3",
                        BindAttribute("GET /hi/*/world/?/notnow")
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
                Node("GET /hi/*/world", Controllers(),
                        Node("/*/now", "hiController4", "hiController3"),
                        Node("/?/now", "hiController3"),
                        Node("/a", Controllers("hiController7", "hiController6"),
                            Node("/*", "hiController7", "hiController6")//,
                            )
                    )
                );
            #endregion


            #region Imported - GET/hi/... - 3
            NewTest(
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
                Node("GET /*/now", "hiController4"),
                Node("GET /a", Controllers("hiController7", "hiController6"),
                    Node("/*", "hiController7", "hiController6")
                    )
                );
            #endregion


            #region Imported - GET/hi/... - 4
            NewTest(
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
                Node("GET /hi/*/world", Controllers(),
                        Node("/*/now", "hiController4"),
                        Node("/a", Controllers("hiController7", "hiController6"),
                            Node("/*", "hiController7", "hiController6")
                            )
                    )
                );
            #endregion


            #region Imported - GET/hi/... - 5
            NewTest(
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
                Node("GET /hi", Controllers(),
                    Node("/*/world", Controllers(),
                        Node("/*/now",  "hiController4", "hiController3"),
                        Node("/?/now", "hiController3"),
                        Node("/a/*", "hiController7")
                        ),
                    Node("/new/*/*/now",  "hiController2"
                        )
                    )
                );
            #endregion


            // Security Controllers

            #region Imported - DependsOn/Requires
            NewTest(
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
                Node("GET /dependson/requires", "DRController1", "DRController2") // Check for Verbs???
                );
            #endregion

            #region Imported - DataQPaging
            NewTest(
                "Imported - DataQPaging",
                Types(
                    Type("DataRoot",
                        Attributes(BindAttribute("GET /data/?")),
                        Field("dataRoot","Boolean",RequestAttribute)
                        ),
                    Type("ProvidersData",
                        Attributes(
                            BindAttribute("GET /data/client/id/{clientId}/providers/id/{dataId}"),
                            BindAttribute("GET /data/client/id/{clientId}/providers/id/{dataId}/withpaging/{linesPerPage}/{pageNumber}")
                        ),
                        Field("dataRoot","Boolean",RequestAttribute,RequiresAttribute),
                        Field("dataSource","Boolean",RequestAttribute)
                        ),
                    Type("BlueCrossProvidersData",
                        Attributes(
                            BindAttribute("GET /data/client/id/11/providers/id/{dataId}"),
                            BindAttribute("GET /data/client/id/11/providers/id/{dataId}/withpaging/{linesPerPage}/{pageNumber}")
                        ),
                        Field("dataSource","Boolean",RequestAttribute,RequiresAttribute),
                        Field("dataSourceCustom","Boolean",RequestAttribute),
                        Field("dataId","int")
                        ),
                    Type("WithPaging",
                        Attributes(
                            BindAttribute("GET /data/?/withpaging/{linesPerPage}/{pageNumber}")
                        ),
                        Field("dataSource","Boolean",RequestAttribute,DependsOnAttribute),
                        Field("dataSourceCustom","Boolean",RequestAttribute,DependsOnAttribute),
                        Field("withPaging","Boolean",RequestAttribute)
                        ),
                    Type("ProvidersRender",
                        Attributes(
                            BindAttribute("GET /data/client/id/*/providers/id/*")
                        ),
                        Field("dataSource","Boolean",RequestAttribute,RequiresAttribute),
                        Field("dataSourceCustom","Boolean",RequestAttribute,DependsOnAttribute),
                        Field("withPaging","Boolean",RequestAttribute,DependsOnAttribute)
                        )
                    ),
                    Node("GET /data",Controllers(),
                        Node("/?",Controllers("DataRoot"),
                            Node("/withpaging/{linesPerPage}/{pageNumber}", Controllers("WithPaging","DataRoot" ))),
                        Node("/client/id",Controllers(),
                            Node("/*/providers/id/*", Controllers("DataRoot", "ProvidersData", "ProvidersRender")),
                            Node("/{clientId}/providers/id/{dataId}", Controllers("DataRoot", "ProvidersData", "ProvidersRender"),
                                Node("/withpaging/{linesPerPage}/{pageNumber}", Controllers("DataRoot", "ProvidersData", "WithPaging", "ProvidersRender"))),
                            Node("/11/providers/id/{dataId}", Controllers("DataRoot", "ProvidersData", "BlueCrossProvidersData", "ProvidersRender"),
                                Node("/withpaging/{linesPerPage}/{pageNumber}", Controllers("DataRoot", "ProvidersData", "BlueCrossProvidersData", "WithPaging", "ProvidersRender")))
                            )
                        )
                    );
            #endregion

            #region Imported - DataQPaging - 2
            NewTest(
                "Imported - DataQPaging - 2",
                Types(
                    //Type("DataRoot",
                    //    Attributes(BindAttribute("GET /data/?")),
                    //    Field("dataRoot", "Boolean", RequestAttribute)
                    //    ),
                    Type("ProvidersData",
                        Attributes(
//                            BindAttribute("GET /data/client/id/{clientId}/providers/id/{dataId}"),
                            BindAttribute("GET /data/client/id/{clientId}/providers/id/{dataId}/withpaging/{linesPerPage}/{pageNumber}")
                        ),
                        Field("dataRoot", "Boolean", RequestAttribute, RequiresAttribute),
                        Field("dataSource", "Boolean", RequestAttribute)
                        ),
                    Type("BlueCrossProvidersData",
                        Attributes(
//                            BindAttribute("GET /data/client/id/11/providers/id/{dataId}"),
                            BindAttribute("GET /data/client/id/11/providers/id/{dataId}/withpaging/{linesPerPage}/{pageNumber}")
                        ),
                        Field("dataSource", "Boolean", RequestAttribute, RequiresAttribute),
                        Field("dataSourceCustom", "Boolean", RequestAttribute),
                        Field("dataId", "int")
                        )//,
                    //Type("WithPaging",
                    //    Attributes(
                    //        BindAttribute("GET /data/?/withpaging/{linesPerPage}/{pageNumber}")
                    //    ),
                    //    Field("dataSource", "Boolean", RequestAttribute, DependsOnAttribute),
                    //    Field("dataSourceCustom", "Boolean", RequestAttribute, DependsOnAttribute),
                    //    Field("withPaging", "Boolean", RequestAttribute)
                    //    ),
                    //Type("ProvidersRender",
                    //    Attributes(
                    //        BindAttribute("GET /data/client/id/*/providers/id/*")
                    //    ),
                    //    Field("dataSource", "Boolean", RequestAttribute, RequiresAttribute),
                    //    Field("dataSourceCustom", "Boolean", RequestAttribute, DependsOnAttribute),
                    //    Field("withPaging", "Boolean", RequestAttribute, DependsOnAttribute)
                    //    )
                    ),
                    Node("GET /data", Controllers(),
                        Node("/?", Controllers("DataRoot"),
                            Node("/withpaging/{linesPerPage}/{pageNumber}", Controllers("WithPaging", "DataRoot"))),
                        Node("/client/id", Controllers(),
                            Node("/*/providers/id/*", Controllers("DataRoot", "ProvidersData", "ProvidersRender")),
                            Node("/{clientId}/providers/id/{dataId}", Controllers("DataRoot", "ProvidersData", "ProvidersRender"),
                                Node("/withpaging/{linesPerPage}/{pageNumber}", Controllers("DataRoot", "ProvidersData", "WithPaging", "ProvidersRender"))),
                            Node("/11/providers/id/{dataId}", Controllers("DataRoot", "ProvidersData", "BlueCrossProvidersData", "ProvidersRender"),
                                Node("/withpaging/{linesPerPage}/{pageNumber}", Controllers("DataRoot", "ProvidersData", "BlueCrossProvidersData", "WithPaging", "ProvidersRender")))
                            )
                        )
                    );
            #endregion


            #region Imported - DataQPaging-simple
            NewTest(
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
                    Type("ProvidersRender",
                        Attributes(
                            BindAttribute("GET /a/b/c2")
                        )
                        ),
                    Type("ProvidersRenderBis",
                        Attributes(
                            BindAttribute("GET /a/b/c1")
                        )
                        )
                    ),

                    Node("GET /a", Controllers(),// is empty because it's not a method node
                        Node("/?", Controllers("DataRoot"),
                            Node("/c3", Controllers("WithPaging", "DataRoot"))),
                        Node("/b", Controllers(),// is empty because it's not a method node
                            Node("/c2", Controllers( "ProvidersRender", "DataRoot")),
                            Node("/c1", Controllers("ProvidersRenderBis", "DataRoot"))
                            )
                        )
                    );
            #endregion

            #region tree - single controller
            NewTest(
                "tree - single controller",
                Types(Type("Controller1", BindAttribute("/?"))),
                Node("* /?", Controller("Controller1", 1))
                );
            #endregion

            #region tree - one controller - 3 bindings (flat)
            NewTest(
                "tree - one controller - 3 bindings (flat)",
                Types(Type(
                    "Controller1",
                    Attributes(
                        BindAttribute("/default"),
                        BindAttribute("/path1"),
                        BindAttribute("/path2/more"))
                    )),
                Node("* /default", "Controller1"),
                Node("* /path1", "Controller1"),
                Node("* /path2/more", "Controller1")
                );
            #endregion

            #region tree - one controller - 3 bindings (tree)
            NewTest(
                "tree - one controller - 3 bindings (tree)",
                Types(Type(
                    "Controller1",
                    Attributes(
                        BindAttribute("/default"),
                        BindAttribute("/path2"),
                        BindAttribute("/path2/more"))
                    )),
                Node("* /default", "Controller1"),
                Node("* /path2", Controllers("Controller1"),
                    Node("/more", "Controller1")
                    )
                );
            #endregion

            #region tree - one generic one specific
            NewTest(
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
                Node("* /default", "Controller2", "Controller1"),
                Node("* /path2", Controllers("Controller2", "Controller1"),
                    Node("/more", "Controller2", "Controller1")
                    ),
                Node("* /?", "Controller2")
                );
            #endregion

            #region tree - one generic one specific - reversed
            NewTest(
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
                Node("* ?", "Controller2"),
                Node("* /default", "Controller1", "Controller2"),
                Node("* /path2", Controllers("Controller1", "Controller2"),
                    Node("/more", "Controller1", "Controller2")
                    )
                );
            #endregion

            #region tree - long path
            NewTest(
                "tree - long path",
                Types(
                    Type("Controller2", BindAttribute("/?")),
                    Type(
                    "Controller1",
                    Attributes(
                        BindAttribute("/default"),
                        BindAttribute("/path2/more/more2"))
                        )
                    ),
                Node("* /?", "Controller2"),
                Node("* /default", "Controller1", "Controller2"),
                Node("* /path2/more/more2", "Controller1", "Controller2")
                );
            #endregion


            #region tree - long path break in
            NewTest(
                "tree - long path break in",
                Types(
                    Type("Controller2", BindAttribute("/?")),
                    Type(
                    "Controller1",
                    Attributes(
                        BindAttribute("/default"),
                        BindAttribute("/path2/more/more2"))
                        ),
                    Type("Controller3", BindAttribute("/path2/more"))
                    ),
                Node("* /?", "Controller2"),
                Node("* /default", "Controller1", "Controller2"),
                Node("* /path2/more", Controllers("Controller3", "Controller2"),
                    Node("/more2", "Controller3", "Controller1", "Controller2")
                    )
                );
            #endregion

            #region tree - long path build up
            NewTest(
                "tree - long path build up",
                Types(
                    Type("Controller2", BindAttribute("/?")),
                    Type("Controller3", BindAttribute("/path2/more")),
                    Type("Controller1",
                        Attributes(
                            BindAttribute("/default"),
                            BindAttribute("/path2/more/more2"))
                        )
                    ),
                Node("* /?", "Controller2"),
                Node("* /path2/more", Controllers("Controller3", "Controller2"),
                    Node("/more2", "Controller1", "Controller3","Controller2")
                    ),
                Node("* /default", "Controller1", "Controller2")
                );
            #endregion

            #region controller ordering - 1
            NewTest(
                "controller ordering - 1",
                Types(
                    Type("Controller1", BindAttribute("/default")),
                    Type("Controller2", BindAttribute("/default"))
                    ),
                Node("* /default", "Controller2", "Controller1")
                );
            #endregion

            #region controller ordering - 2
            NewTest(
                "controller ordering - 2",
                Types(
                    Type(
                        "Controller1",
                        Attributes(BindAttribute("/default")),
                        Field("f1", "string", RequestAttribute)),
                    Type("Controller2", Attributes(BindAttribute("/default")),
                        Field("f1", "string", RequiresAttribute))
                    ),
                Node("* /default", "Controller1", "Controller2")
                );
            #endregion


            #region controller ordering - 3
            NewTest(
                "controller ordering - 3",
                Types(
                    Type(
                        "Controller1",
                        Attributes(BindAttribute("/default")),
                        Field("f1", "string", RequiresAttribute)),
                    Type("Controller2", Attributes(BindAttribute("/default")),
                        Field("f1", "string", RequestAttribute))
                    ),
                Node("* /default","Controller2", "Controller1")
                );
            #endregion

            #region controller ordering - 4
            NewTest(
                "controller ordering - 4", // c1 -(f2)-> c3 ; c3 -(f1)-> c2 ; c4 -(f1)-> c2
                Types(
                    Type("Controller1", Attributes(BindAttribute("/default")),
                        Field("f2", "string", RequiresAttribute)),
                    Type("Controller2", Attributes(BindAttribute("/default")),
                        Field("f1", "string", RequestAttribute)),
                    Type("Controller3", Attributes(BindAttribute("/default")),
                        Field("f2", "string", RequestAttribute),
                        Field("f1", "string", RequiresAttribute)
                        ),
                    Type("Controller4", Attributes(BindAttribute("/default")),
                        Field("f1", "string", RequiresAttribute))
                    ),
                Node("* /default", "Controller2", "Controller4", "Controller3", "Controller1")
                );
            #endregion

            #region controller invalid binding - 1
            NewErrorTest(
                "controller invalid binding - 1",
                Types(
                    Type("Controller1", Attributes(BindAttribute("##################")))
                ),
                Errors(
                    InvalidBindingError("Controller1","##################")
                )
                );
            #endregion

            #region controller resource loop - 1
            NewErrorTest(
                "controller resource loop - 1", // c1 -(f2)-> c3 ; c3 -(f1)-> c2 ; c4 -(f1)-> c2
                Types(
                    Type("Controller1", Attributes(BindAttribute("/default")),
                        Field("f2", "string", RequiresAttribute)),
                    Type("Controller2", Attributes(BindAttribute("/default")),
                        Field("f1", "string", RequestAttribute),
                        Field("f2", "string", RequiresAttribute)
                        ),
                    Type("Controller3", Attributes(BindAttribute("/default")),
                        Field("f2", "string", RequestAttribute),
                        Field("f1", "string", RequiresAttribute)
                        ),
                    Type("Controller4", Attributes(BindAttribute("/default")),
                        Field("f1", "string", RequiresAttribute))
                    ),
                Errors(
                    // Obviosly, for resource loop check sorting should be implemented...
                    ResourceLoopError("* /default","Controller1", "Controller2", "Controller3", "Controller4")
                ),
                Node("* /default", "Controller2", "Controller3", "Controller1", "Controller4")
                );
            #endregion



            return tests;
        }
    }
}
