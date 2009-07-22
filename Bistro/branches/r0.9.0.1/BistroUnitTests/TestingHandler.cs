using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using System.Web;
using System.IO;
using Bistro.Controllers;
using System.Collections.Specialized;

namespace Bistro.UnitTests
{
    /// <summary>
    /// HTTP session mockup.
    /// </summary>
    public class HttpSessionMock : HttpSessionStateBase
    {
        /// <summary>
        /// Little wrapper class for getting at a populated instance of KeysCollection. 
        /// The class doesn't have (enough) virtual methods, and doesn't have a public
        /// constructor, so we're stuck with these shenanigans.
        /// </summary>
        public abstract class NaiveNOCBImpl : NameObjectCollectionBase
        {
            public NameObjectCollectionBase.KeysCollection Populate(Dictionary<string, object> values)
            {
                BaseClear();
                foreach (string key in values.Keys)
                    BaseAdd(key, values[key]);

                return base.Keys;
            }
        }

        private Dictionary<string, object> objects = new Dictionary<string, object>();
        private Mock<NaiveNOCBImpl> translator = new Mock<NaiveNOCBImpl>();
        
        public override object this[string name]
        {
            get { return (objects.ContainsKey(name)) ? objects[name] : null; }
            set { objects[name] = value; }
        }

        public override NameObjectCollectionBase.KeysCollection Keys
        {
            get
            {
                var translatorInstance = translator.Object;

                return translatorInstance.Populate(objects);
            }
        }

        public override int Count { get { return objects.Count; } }

        internal Dictionary<string, object> InternalContents { get { return objects;}}

        public override void Clear() { objects.Clear(); }
    }

    public class TestingHandler: Bistro.Http.Module
    {
        private readonly HttpSessionMock sessionMock = new HttpSessionMock();
        private readonly MemoryStream stream = new MemoryStream();
        private IContext requestContext = null;

        protected readonly Mock<HttpContextBase> Context = new Mock<HttpContextBase>();
        //protected readonly Mock<HttpSessionStateBase> Session = new Mock<HttpSessionStateBase>();

        public TestingHandler()
            : base()
        {
            Context.Setup(ctx => ctx.Session).Returns(sessionMock);
            Context.Setup(ctx => ctx.Response.OutputStream).Returns(stream);
            Context.Setup(ctx => ctx.Request.Cookies).Returns(new HttpCookieCollection());
            Context.Setup(ctx => ctx.Request.Form).Returns(new NameValueCollection());
            Context.Setup(ctx => ctx.Request.Files.AllKeys).Returns(new string[] { });

            LoadFactories(null);
        }

        public virtual string RunForTest(string path)
        {
            var httpContext = Context.Object;
            httpContext.Session.Clear();

            requestContext = CreateRequestContext(httpContext);

            ProcessRequestRecursive(httpContext, path, requestContext);

            return new StreamReader(stream).ReadToEnd();
        }

        public virtual Dictionary<string, Dictionary<string, object>> AllContents
        {
            get
            {
                var ret = new Dictionary<string, Dictionary<string, object>>();
                ret.Add("session", sessionMock.InternalContents);
                ret.Add("request", (Dictionary<string, object>)requestContext);
                
                return ret;
            }
        }
    }
}
