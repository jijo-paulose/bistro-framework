using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;
using Bistro.Controllers;
using Bistro.Controllers.Dispatch;
using System.Collections;
using Bistro.Controllers.OutputHandling;
using Bistro.Configuration.Logging;
using Bistro.Configuration;
using Bistro.Validation;
using Bistro.UnitTests.Support;
using System.Collections.Specialized;

namespace Bistro.UnitTests.Tests
{
    [TestFixture]
    public class FormatTests : TestingBase
    {
        [Test]
        public void ValidOutput()
        {
            var expected = "{foo:\"bar\",baz:\"qux\"}";
            var resp = handler.RunForTest("GET/format");
            resp = resp
                .Replace(" ", "")
                .Replace("\r", "")
                .Replace("\n", "");

            Assert.That(
                expected.Equals(resp),
                String.Format("Expected '{0}', Received {1}. Issue with Json formatting.", expected, resp));
        }

        [Test]
        public void ValidInput()
        {
            var expected = "hello world";
            var formData = new NameValueCollection();
            formData.Add("input", "{ foo: \"hello\", bar: \"world\" }");

            var resp = handler.RunForTest("POST/format");

            Assert.That(
                expected.Equals(resp),
                String.Format("Expected '{0}', Received {1}. Issue with Json formatting.", expected, resp));
        }
    }
}
