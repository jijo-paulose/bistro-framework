﻿using System;
using System.Collections.Specialized;
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
using Bistro.UnitTests.Tests.Data;

namespace Bistro.UnitTests.Tests
{
    [TestFixture]
    public class EntityTests: TestingBase
    {
        [Test]
        public void MapsFromInputToEntity()
        {
            var resp = handler.RunForTest("GET/entityTest?foo=hello&bar=world&thirdField=stuff&unwrap=false&extra=something");
            var contexts = handler.AllContents;
            var entity = contexts["request"]["entity"] as SimpleEntity;

            Assert.NotNull(entity);

            if (entity != null)
            {
                Assert.AreEqual("hello", entity.foo);
                Assert.AreEqual("world", entity.bar);
                Assert.AreEqual("stuff", entity.baz, String.Format("Expected 'stuff', received '{0}'. If the other tests passed, and this failed, the explicit mapping call is suspect", entity.baz));
                Assert.IsNull(entity.extra, "Field 'extra' should remain null. Issue with Except() method.");
            }
        }

        [Test]
        public void MapsFromEntityToInput()
        {
            NameValueCollection formFields = new NameValueCollection();
            formFields.Add("entity", "{foo: \"hello\", bar: \"world\", baz: \"stuff\"}");
            var resp = handler.RunForTest("GET/entityTest?unwrap=true", formFields);
            var contexts = handler.AllContents;

            Assert.AreEqual("hello", contexts["request"]["foo"]);
            Assert.AreEqual("world", contexts["request"]["bar"]);
            Assert.AreEqual("stuff", contexts["request"]["thirdField"], String.Format("Expected 'stuff', received '{0}'. If the other tests passed, and this failed, the explicit mapping call is suspect", contexts["request"]["thirdField"]));
        }

        [Test]
        public void ValidatesByMapping()
        {
            var resp = handler.RunForTest("GET/entityTest?bar=world&thirdField=stuff&unwrap=false");
            var contexts = handler.AllContents;
            var entity = contexts["request"]["entity"] as SimpleEntity;

            Assert.Null(entity, "Entity should have been null - not enough data was supplied.");

            var messages = contexts["request"]["Messages"] as List<IValidationResult>;
            Assert.AreEqual(1, messages.Count);
        }
    }
}
