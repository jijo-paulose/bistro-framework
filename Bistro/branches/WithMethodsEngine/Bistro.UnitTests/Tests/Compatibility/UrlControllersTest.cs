using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Controllers.Dispatch;
using NUnit.Framework;

namespace Bistro.UnitTests.Tests.Compatibility
{

    internal class UrlControllersTest
    {
        internal UrlControllersTest(string name, string url, params string[] controllers)
        {
            testUrl = url;
            testControllers = controllers;
        }

        string testUrl;
        string[] testControllers;

        public void Validate(IControllerDispatcher dispatcher)
        {
            var ctrlrs = dispatcher.GetControllers(testUrl);
            Assert.AreEqual(testControllers.Length, ctrlrs.Length, "Controller queues lengths are different. \r\n Request url: {0}", testUrl);
            int i = 0;
            foreach (var controllerInfo in ctrlrs)
            {
                Assert.AreEqual(controllerInfo.BindPoint.Controller.ControllerTypeName, testControllers[i], "Controller names are different at position: {0}, \r\n Request url: {1}", i, testUrl);
                i++;
            }

            //TODO: Check whether right controllers has been returned.
        }
    }

}
