﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using System.Web;
using System.IO;
using Bistro.Controllers;
using System.Collections.Specialized;

namespace Bistro.UnitTests.Support
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

    /// <summary>
    /// Implementation of http module capable of processing a bistro request without loading
    /// the ASP.NET runtime.
    /// </summary>
    public class TestingHandler: Bistro.Http.Module
    {
        private readonly HttpSessionMock sessionMock = new HttpSessionMock();
        private readonly MemoryStream stream = new MemoryStream();
        private IContext requestContext = null;
        private NameValueCollection formCollection = new NameValueCollection();

        protected readonly Mock<HttpContextBase> Context = new Mock<HttpContextBase>();
        //protected readonly Mock<HttpSessionStateBase> Session = new Mock<HttpSessionStateBase>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestingHandler"/> class.
        /// </summary>
        public TestingHandler()
            : base()
        {
            Context.Setup(ctx => ctx.Session).Returns(sessionMock);
            Context.Setup(ctx => ctx.Response.OutputStream).Returns(stream);
            Context.Setup(ctx => ctx.Request.Cookies).Returns(new HttpCookieCollection());
            Context.Setup(ctx => ctx.Request.Form).Returns(formCollection);
            Context.Setup(ctx => ctx.Request.Files.AllKeys).Returns(new string[] { });

            LoadFactories(null);
        }

        /// <summary>
        /// Retrieves the string response of executing the given url
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public virtual string RunForTest(string path)
        {
            return RunForTest(path, new NameValueCollection());
        }

        /// <summary>
        /// Retrieves the string response of executing the given url with the given form data
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="formData">The form data.</param>
        /// <returns></returns>
        public virtual string RunForTest(string path, NameValueCollection formData)
        {
            var httpContext = Context.Object;
            httpContext.Session.Clear();
            formCollection.Clear();
            formCollection.Add(formData);

            requestContext = CreateRequestContext(httpContext);

            ProcessRequestRecursive(httpContext, path, requestContext);

            return new StreamReader(stream).ReadToEnd();
        }

        /// <summary>
        /// Gets all contents as it would be at the end of chain execution. The session values are available
        /// under <c>"session"</c>, and the request contens is available under <c>"request"</c>
        /// </summary>
        /// <value>All contents.</value>
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
