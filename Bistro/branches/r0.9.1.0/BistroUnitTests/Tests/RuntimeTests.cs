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
    public class RuntimeTests : TestingBase
    {
        [Test]
        public void ApplicationCreated()
        {
            Assert.IsNotNull(Application.Instance, "Application not created.");
            Assert.AreEqual(1, StartupController.hitcount, String.Format("Application startup event called {0} time(s), instead of 1.", StartupController.hitcount));
        }

        [Test]
        public void DispatcherCreated()
        {
            Assert.IsNotNull(dispatcher);
        }
    }
}
