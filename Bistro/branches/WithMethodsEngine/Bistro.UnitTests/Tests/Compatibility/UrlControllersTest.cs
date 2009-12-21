using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Controllers.Dispatch;
using NUnit.Framework;
using Bistro.Controllers;

namespace Bistro.UnitTests.Tests.Compatibility
{
	internal class UrlGroup
	{
		internal UrlGroup(params string[] controllers)
		{
			controllersList = controllers.ToList();
			if (controllersList.Count == 0)
				throw new Exception("UrlControllersTest has invalid definition");
		}

		private List<string> controllersList;

		internal bool CheckEqual(string ctrName)
		{
			return controllersList.Contains(ctrName);
		}

		internal int Count
		{
			get { return controllersList.Count; }
		}

	}


	internal class UrlControllersTest
	{
		internal UrlControllersTest(string name, string url, params object[] controllers)
		{
			testUrl = url;
			testName = name;
			testControllers = new List<UrlGroup>(controllers.Length);
			foreach (object obj in controllers)
			{
				if (obj is String)
				{
					testControllers.Add(new UrlGroup(obj as String));
				}
				else if (obj is UrlGroup)
				{
					testControllers.Add(obj as UrlGroup);
				}
				else
				{
					throw new Exception("UrlControllersTest has invalid definition");
				}
			}

		}

		private string testName;
		private string testUrl;
		private List<UrlGroup> testControllers;

		public void Validate(IControllerDispatcher dispatcher)
		{
			Func<String,ControllerInvocationInfo,String> sumStr = (oldStr, invInfo) => oldStr += "+" + invInfo.BindPoint.Controller.ControllerTypeName;
			var ctrlrs = dispatcher.GetControllers(testUrl);
			int count = testControllers.Sum(group => group.Count);
			Assert.AreEqual(count, ctrlrs.Length, "Controller queues lengths are different. Test name:{0}; Controllers returned:{1}", testName, 
				ctrlrs.Aggregate(string.Empty,sumStr ));
			int i = 0;
			int j = 0;

			foreach (var controllerInfo in ctrlrs)
			{
				if (j == testControllers[i].Count)
				{
					j = 0;
					i++;
				}
				Assert.IsTrue(testControllers[i].CheckEqual(controllerInfo.BindPoint.Controller.ControllerTypeName), "Controller names are different at position: {0},{1}; TestName:{2}; ControllersReturned: {3}", i, j, testName, ctrlrs.Aggregate(string.Empty, sumStr));
				j++;

			}


		}
	}

}
