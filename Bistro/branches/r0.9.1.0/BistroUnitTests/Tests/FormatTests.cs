using System;
using NUnit.Framework;
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
            var expected = "{foo:bar,baz:qux}";
            var resp = handler.RunForTest("GET/format");
            resp = resp
                .Replace(" ", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\"", "");

            Assert.That(
                expected.Equals(resp),
                String.Format("Expected '{0}', Received {1}. Issue with Json formatting.", expected, resp));
        }

        [Test]
        public void ValidInput()
        {
            var expected = "\"hello world\"";
            var formData = new NameValueCollection();
            formData.Add("input", "{ foo: \"hello\", baz: \"world\" }");

            var resp = handler.RunForTest("POST/format", formData);

            Assert.That(
                expected.Equals(resp),
                String.Format("Expected '{0}', Received {1}. Issue with Json formatting.", expected, resp));
        }
    }
}
