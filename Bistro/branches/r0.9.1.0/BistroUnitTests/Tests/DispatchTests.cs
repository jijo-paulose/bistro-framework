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
    public class DispatchTests: TestingBase
    {
        [Test]
        public void HomeURL()
        {
            ControllerInvocationInfo[] controllers = dispatcher.GetControllers("GET/");
            Assert.AreEqual(3, controllers.Length);
            Assert.AreEqual(typeof(HomeUrlController1), controllers[1].BindPoint.Controller.ControllerType);
            Assert.AreEqual(typeof(HomeUrlController2), controllers[0].BindPoint.Controller.ControllerType);
        }

        [Test]
        public void HelloYou1()
        {
            ControllerInvocationInfo[] controllers = dispatcher.GetControllers("GET/hello/world");
            Assert.AreEqual(1, controllers.Length);
        }

        [Test]
        public void HelloYou2()
        {
            ControllerInvocationInfo[] controllers = dispatcher.GetControllers("GET/hello/how/are/you");
            Assert.AreEqual(2, controllers.Length);
            Assert.AreEqual(typeof(HelloYouController1), controllers[0].BindPoint.Controller.ControllerType);
        }

        [Test]
        public void HelloYou3()
        {
            ControllerInvocationInfo[] controllers = dispatcher.GetControllers("GET/hello/howAre/you");
            Assert.AreEqual(3, controllers.Length);
            Assert.AreEqual(typeof(HelloYouController1), controllers[0].BindPoint.Controller.ControllerType);
            Assert.AreEqual(typeof(HelloYouController2), controllers[1].BindPoint.Controller.ControllerType);
        }

        //This test checks the correct binding and order of 2 related controllers
        [Test]
        public void URLs2()
        {
            ControllerInvocationInfo[] controllers = dispatcher.GetControllers("GET/one_little_url");
            Assert.That(controllers.Length == 3, "We have " + controllers.Length + "controllers bound to the URL \"/one_little_url\" instead of 3");
            Assert.That(controllers[0].BindPoint.Controller.ControllerType == typeof(littleController2), "Wrong order, problems with littleController1");
            Assert.That(controllers[1].BindPoint.Controller.ControllerType == typeof(littleController1), "Wrong order, problems with littleController2");
            Assert.That(controllers[2].BindPoint.Controller.ControllerType == typeof(ReturnTypesController), "Wrong order, problems with ReturnTypesController");
        }

        //This test checks the correct binding and order of 3 related controllers
        [Test]
        public void URLs3()
        {
            ControllerInvocationInfo[] controllers = dispatcher.GetControllers("GET/little_url/more");
            Assert.That(controllers.Length == 4, "We have " + controllers.Length + "controllers bound to the URL \"/little_url/more\" instead of 4");
            Assert.That(controllers[0].BindPoint.Controller.ControllerType == typeof(littleController3), "Wrong order, problems with littleController3");
            Assert.That(controllers[1].BindPoint.Controller.ControllerType == typeof(littleController5), "Wrong order, problems with littleController5");
            Assert.That(controllers[2].BindPoint.Controller.ControllerType == typeof(littleController4), "Wrong order, problems with littleController4");
            Assert.That(controllers[3].BindPoint.Controller.ControllerType == typeof(ReturnTypesController), "Wrong order, problems with ReturnTypesController");
        }

        //This test checks that rather complicated graph of controllers will be invoked in correct order
        //Controllers 1 to 6 should be in the following order: 5,2,1,4,3,6. controller 7 should 
        //be invoked at any position earlier than controller 6. For detailed dependencies see Controllers.cs
        //file, controllers OrderController1....OrderController7
        [Test]
        public void Order()
        {
            ControllerInvocationInfo[] controllers = dispatcher.GetControllers("GET/order/world/new");
            Assert.That(controllers.Length == 8, "We have " + controllers.Length + "controllers bound to the URL \"/order/world/new\" instead of 8");
            List<string> ctrs = new List<string>();
            foreach (ControllerInvocationInfo ctr in controllers)
            {
                ctrs.Add(ctr.BindPoint.Controller.ControllerType.ToString().Substring(32));
            }
            int i = 0;
            string controllerSequence = "";
            foreach (string ctrNum in ctrs)
            {
                if (ctrNum == ctrs[ctrs.Count - 1] && ctrNum.Substring(ctrNum.LastIndexOf(".") + 1) == "ReturnTypesController")
                    continue;

                if (ctrNum == "7")
                    Assert.That(i < 6, "Seventh controller will be invoked too late, causing chaos, panic and destruction");
                else
                    controllerSequence += ctrNum;

                i++;
            }
            Assert.That(controllerSequence == "521436", "Controllers will be invoked in wrong order (" + controllerSequence + " instead of \"521436\"), causing chaos, panic and destruction");
        }
    }
}
