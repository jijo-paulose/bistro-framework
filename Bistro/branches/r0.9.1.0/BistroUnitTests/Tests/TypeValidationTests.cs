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

namespace Bistro.UnitTests.Tests
{
    [TestFixture]
        public class TypeValidationTests : TestingBase
        {
            private bool containsValidation(string message, List<IValidationResult> messages)
            {
                foreach (IValidationResult res in messages)
                    if (message.Equals(res.Message))
                        return true;

                return false;
            }
            [Test]
            public void EmailTypeFailsValidation()
            {
                var resp = handler.RunForTest("GET/validationTest/asdasd@sdsd");
                var contexts = handler.AllContents;

                var messages = contexts["request"]["Messages"] as List<IValidationResult>;

                Assert.That(containsValidation("someField must be type of email", messages), "Type validation didn't fire");
            }

            [Test]
            public void EmailTypePassesValidation()
            {
                var resp = handler.RunForTest("GET/validationTest/mail@mail.com");
                var contexts = handler.AllContents;

                var messages = contexts["request"]["Messages"] as List<IValidationResult>;

                Assert.That(!containsValidation("someField must be type of email", messages), "Type shouldn't have fired");
            }
            [Test]
            public void DateTypeFailsValidation()
            {
                var resp = handler.RunForTest("GET/validationTest/wdfwfwerf");
                var contexts = handler.AllContents;

                var messages = contexts["request"]["Messages"] as List<IValidationResult>;

                Assert.That(containsValidation("someField must be type of date", messages), "Type validation didn't fire");
            }

            [Test]
            public void DateTypePassesValidation()
            {
                var resp = handler.RunForTest("GET/validationTest/22.07.2009");
                var contexts = handler.AllContents;

                var messages = contexts["request"]["Messages"] as List<IValidationResult>;

                Assert.That(!containsValidation("someField must be type of date", messages), "Type shouldn't have fired");
            }
            [Test]
            public void DateISOTypeFailsValidation()
            {
                var resp = handler.RunForTest("GET/validationTest/13123123");
                var contexts = handler.AllContents;

                var messages = contexts["request"]["Messages"] as List<IValidationResult>;

                Assert.That(containsValidation("someField must be type of dateISO", messages), "Type validation didn't fire");
            }

            [Test]
            public void DateISOTypePassesValidation()
            {
                var resp = handler.RunForTest("GET/validationTest/2008-01-01");
                var contexts = handler.AllContents;

                var messages = contexts["request"]["Messages"] as List<IValidationResult>;

                Assert.That(!containsValidation("someField must be type of dateISO", messages), "Type shouldn't have fired");
            }

            [Test]
            public void NumberTypeFailsValidation()
            {
                var resp = handler.RunForTest("GET/validationTest/1312.121.12");
                var contexts = handler.AllContents;

                var messages = contexts["request"]["Messages"] as List<IValidationResult>;

                Assert.That(containsValidation("someField must be type of number", messages), "Type validation didn't fire");
            }

            [Test]
            public void NumberTypePassesValidation()
            {
                var resp = handler.RunForTest("GET/validationTest/2341.234");
                var contexts = handler.AllContents;

                var messages = contexts["request"]["Messages"] as List<IValidationResult>;

                Assert.That(!containsValidation("someField must be type of number", messages), "Type shouldn't have fired");
            }

            [Test]
            public void DigitsTypeFailsValidation()
            {
                var resp = handler.RunForTest("GET/validationTest/1312wew");
                var contexts = handler.AllContents;

                var messages = contexts["request"]["Messages"] as List<IValidationResult>;

                Assert.That(containsValidation("someField must be type of digits", messages), "Type validation didn't fire");
            }

            [Test]
            public void DigitsTypePassesValidation()
            {
                var resp = handler.RunForTest("GET/validationTest/23412342");
                var contexts = handler.AllContents;

                var messages = contexts["request"]["Messages"] as List<IValidationResult>;

                Assert.That(!containsValidation("someField must be type of digits", messages), "Type shouldn't have fired");
            }

            [Test]
            public void UrlTypeFailsValidation()
            {
                var resp = handler.RunForTest("GET/validationTest/asda32");
                var contexts = handler.AllContents;

                var messages = contexts["request"]["Messages"] as List<IValidationResult>;

                Assert.That(containsValidation("someField must be type of url", messages), "Type validation didn't fire");
            }

            [Test]
            public void UrlTypePassesValidation()
            {
               // var resp = handler.RunForTest("GET/validationTest/http%3A%2F%2Fya.ru");
                var resp = handler.RunForTest("GET/validationTest/http//ya.ru");
                var contexts = handler.AllContents;

                var messages = contexts["request"]["Messages"] as List<IValidationResult>;

                Assert.That(!containsValidation("someField must be type of url", messages), "Type validation didn't fire");
            }
        }
}
