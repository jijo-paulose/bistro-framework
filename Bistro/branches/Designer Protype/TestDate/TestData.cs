using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Tests;
using Bistro.Tests.Errors;
using Bistro.Controllers.Descriptor;

namespace TestDate
{
    
    public class TestData
    {
        private IList<TestDescriptor> testData = null;
        /// <summary>
        /// Gets or sets current token symbol.
        /// </summary>
        public  IList<TestDescriptor> GetTestData
        {
            get { return testData; }
            set { testData = value; }
        }

        /// <summary>
        /// Class .ctor
        /// </summary>
        public TestData() {
            this.testData = TestSource();
        }

        List<TestDescriptor> tests = new List<TestDescriptor>();

        internal IList<TestDescriptor> TestSource()
        {
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
                            Field("currentUser", "UserProfileStub", RequestAttribute, RequiresAttribute)
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
            return tests;
        }
        #region Test Creation Methods

        private void NewTest(string name, TestTypeInfo[] types, params BindingTest[] nodes)
        {
            tests.Add(new TestDescriptor(name, types, null, nodes));
        }

        private void NewErrorTest(string name, TestTypeInfo[] types, IErrorDescriptor[] errors, params BindingTest[] nodes)
        {
            tests.Add(new TestDescriptor(name, types, errors, nodes));
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

    }
}
