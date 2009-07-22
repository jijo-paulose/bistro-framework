using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NUnit.Framework;
using BistroApi;
using BistroModel;
using BistroModel.Test;

namespace ModBistroUnitTests {
	[TestFixture]
	public class DispatcherTests {
		ILoader _loader;
		IDispatcher _dispatcher;
		string _dispatchMessage = "\t\t{0} dispatched to {1}";

		[TestFixtureSetUp]
		public void Init() {
			_dispatcher = Global.Application.CreateDispatcher();
			_loader = Global.Application.CreateLoader(_dispatcher);
			_loader.Load();
			_dispatcher.BuildMethods();
		}
		[TestFixtureTearDown]
		public void Cleanup() { }

		[Test]
		public void _DispatcherCreated() {
			Assert.IsNotNull(_dispatcher);
		}

		[Test]
		public void _LoaderCreated() {
			Assert.IsNotNull(_loader, "Loader not created.");
		}

		[Test]
		public void HomeURL() {
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl("GET/"));
			Assert.IsNotNull(method.LinkedMethodParts);
			Assert.AreEqual(3, method.LinkedMethodParts.Count);
			if (method.UnlinkedMethodParts != null)
				Assert.AreEqual(0, method.UnlinkedMethodParts.Count);
			Assert.AreEqual(typeof(HomeUrlController1).FullName, method.LinkedMethodParts[0].ControllerInfo.Name);
			Assert.AreEqual(typeof(HomeUrlController2).FullName, method.LinkedMethodParts[1].ControllerInfo.Name);
		}
		[Test]
		public void HelloYou1() {
			string request = "GET/hello/world";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.IsNotNull(method.LinkedMethodParts);
			//Note: Original checks for only 1 part. Need to discuss this difference.
			Assert.AreEqual(3, method.LinkedMethodParts.Count);
			int cRT = GetIndexOf(typeof(ReturnTypesController), method);
			Assert.That(cRT == method.LinkedMethodParts.Count - 1, "ReturnTypesController s/be last.");
			Console.WriteLine(_dispatchMessage, request, method.Url.Name);
		}
		[Test]
		public void HelloYou2() {
			string request = "GET/hello/how/are/you";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.IsNotNull(method.LinkedMethodParts);
			//Note: Original checks for only 2 parts. Need to discuss this difference.
			Assert.AreEqual(4, method.LinkedMethodParts.Count);
			int cRT = GetIndexOf(typeof(ReturnTypesController), method);
			Assert.That(cRT == method.LinkedMethodParts.Count - 1, "ReturnTypesController s/be last.");
			Console.WriteLine(_dispatchMessage, request, method.Url.Name);
		}
		[Test]
		public void HelloYou3() {
			string request = "GET/hello/howAre/you";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.IsNotNull(method.LinkedMethodParts);
			//Note: Original checks for only 3 parts. Need to discuss this difference.
			Assert.AreEqual(5, method.LinkedMethodParts.Count);
			int cRT = GetIndexOf(typeof(ReturnTypesController), method);
			Assert.That(cRT == method.LinkedMethodParts.Count - 1, "ReturnTypesController s/be last.");
			Console.WriteLine(_dispatchMessage, request, method.Url.Name);
		}

		//This test checks the correct binding and order of 2 related controllers
		[Test]
		public void URLs2() {
			string request = "GET/one_little_url";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(5, method.LinkedMethodParts.Count);
			int c1 = GetIndexOf(typeof(littleController1).FullName, method);
			int c2 = GetIndexOf(typeof(littleController2).FullName, method);
			int cRT = GetIndexOf(typeof(ReturnTypesController), method);
			Assert.That(c2 < c1, "Controllers in wrong order littleController2 should be before littleController1.");
			Assert.That(cRT == method.LinkedMethodParts.Count - 1, "ReturnTypesController s/be last.");
			Console.WriteLine(_dispatchMessage, request, method.Url.Name);
		}

		//This test checks the correct binding and order of 3 related controllers
		[Test]
		public void URLs3() {
			string request = "GET/little_url/more";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(6, method.LinkedMethodParts.Count);
			int c3 = GetIndexOf(typeof(littleController3).FullName, method);
			int c5 = GetIndexOf(typeof(littleController5).FullName, method);
			int c4 = GetIndexOf(typeof(littleController4).FullName, method);
			int cRT = GetIndexOf(typeof(ReturnTypesController), method);
			string msg = "Controllers in wrong order. littleController{0} s/be before littleController{1}.";
			Assert.That(c3 < c5, msg, 3, 5);
			Assert.That(c5 < c4, msg, 5, 4);
			Assert.That(cRT == method.LinkedMethodParts.Count - 1, "ReturnTypesController s/be last.");
			Console.WriteLine(_dispatchMessage, request, method.Url.Name);
		}

		//This test checks that rather complicated graph of controllers will be invoked in correct order
		//Controllers 1 to 6 should be in the following order: 5,2,1,4,3,6. controller 7 should 
		//be invoked at any position earlier than controller 6. For detailed dependencies see Controllers.cs
		//file, controllers OrderController1....OrderController7
		[Test]
		public void Order() {
			string request = "GET/order/world/new";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(10, method.LinkedMethodParts.Count);
			int c5 = GetIndexOf(typeof(OrderController5), method);
			int c2 = GetIndexOf(typeof(OrderController2), method);
			int c1 = GetIndexOf(typeof(OrderController1), method);
			int c4 = GetIndexOf(typeof(OrderController4), method);
			int c3 = GetIndexOf(typeof(OrderController3), method);
			int c7 = GetIndexOf(typeof(OrderController7), method);
			int c6 = GetIndexOf(typeof(OrderController6), method);
			int cRT = GetIndexOf(typeof(ReturnTypesController), method);
			string msg = "Controllers in wrong order. OrderController{0} s/be before OrderControler{1}.";
			Assert.That(c5 < c2, msg, 5, 2);
			Assert.That(c2 < c1, msg, 2, 1);
			Assert.That(c4 < c3, msg, 4, 3);
			Assert.That(c3 < c6, msg, 3, 6);
			Assert.That(c7 < c6, msg, 7, 6);
			Assert.That(cRT == method.LinkedMethodParts.Count - 1, "ReturnTypesController s/be last.");
			Console.WriteLine(_dispatchMessage, request, method.Url.Name);
		}

		[Test]
		public void Dispatch1() {
			string request = "GET/hi/new/world/a/now";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(5, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/hi/new/world/a", url);
		}

		[Test]
		public void Dispatch2() {
			string request = "GET/hi/new/world/now";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(3, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/hi/new/world", url);
		}

		[Test]
		public void Dispatch3() {
			string request = "GET/hi/some/world/now";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(3, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/hi/*/world/*", url);
		}

		[Test]
		public void Dispatch4() {
			string request = "GET/hi/some/world/right/now";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(5, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/hi/*/world/*/now", url);
		}

		[Test]
		public void Dispatch5() {
			string request = "GET/hi/some/world/right/right/now";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(4, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/hi/*/world/?/now", url);
		}

		[Test]
		public void Dispatch6() {
			string request = "GET/hi/new/world/a";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(5, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/hi/new/world/a", url);
		}

		[Test]
		public void Dispatch7() {
			string request = "GET/hi/new/world/a/now";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(5, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/hi/new/world/a", url);
		}

		[Test]
		public void Dispatch8() {
			string request = "GET/hi/old/world/a/now";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(8, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/hi/*/world/a/now", url);
		}
		
		[Test]
		public void Secure() {
			string request = "GET/secure/one/two";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(5, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/secure/one", url);
			Assert.AreEqual(typeof(SecurityController1).FullName, method.LinkedMethodParts[0].ControllerInfo.Name);
		}

		[Test]
		public void SecureA() {
			string request = "GET/secureA/one/two";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(6, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/secureA", url);
			Assert.AreEqual(typeof(SecurityControllerA2).FullName, method.LinkedMethodParts[0].ControllerInfo.Name);
		}

		[Test]
		public void SecureARun() {
			string url = "~/secureA/one/two";
			string request = "GET/secureA/one/two";
			HttpContextBase httpContext = MoqHelper.FakeHttpContext(url);
			BistroContext bc = new BistroContext(httpContext, TestHarness.NewUrl(request));
			_dispatcher.Dispatch(bc);
		}

		[Test]
		public void DataWithoutPaging() {
			string request = "GET/data/client/id/5/providers/id/10";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(6, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/data/client/id/{clientId}/providers/id/{dataId}", url);
			
			int dr = GetIndexOf(typeof(DataRoot), method);
			int h2 = GetIndexOf(typeof(HomeUrlController2), method);
			int h1 = GetIndexOf(typeof(HomeUrlController1), method);
			int pd = GetIndexOf(typeof(ProvidersData), method);
			int pr = GetIndexOf(typeof(ProvidersRender), method);
			int rt = GetIndexOf(typeof(ReturnTypesController), method);
			
			string msg = "Controllers in wrong order. Controller {0} s/be before Controller {1}.";
			
			Assert.That(dr < pd, msg, "DataRoot", "ProvidersData");
			Assert.That(pd < pr, msg, "ProvidersData", "ProvidersRender");
			Assert.That(pr < rt, msg, "ProvidersRender", "ReturnTypesController");
			Assert.That(h1 < rt, msg, "HomeUrlController1", "ReturnTypesController");
			Assert.That(h2 < rt, msg, "HomeUrlController2", "ReturnTypesController");
			Assert.That(rt == method.LinkedMethodParts.Count - 1, "ReturnTypesController s/be last.");
		}

		[Test]
		public void DataWithPaging() {
			string request = "GET/data/client/id/5/providers/id/10/withpaging/30/1";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(7, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/data/client/id/{clientId}/providers/id/{dataId}/withpaging/{linesPerPage}/{pageNumber}", url);
			
			int dr = GetIndexOf(typeof(DataRoot), method);
			int h2 = GetIndexOf(typeof(HomeUrlController2), method);
			int h1 = GetIndexOf(typeof(HomeUrlController1), method);
			int pd = GetIndexOf(typeof(ProvidersData), method);
			int wp = GetIndexOf(typeof(WithPaging), method);
			int pr = GetIndexOf(typeof(ProvidersRender), method);
			int rt = GetIndexOf(typeof(ReturnTypesController), method);
			
			string msg = "Controllers in wrong order. Controller {0} s/be before Controller {1}.";
			
			Assert.That(dr < pd, msg, "DataRoot", "ProvidersData");
			Assert.That(pd < wp, msg, "ProvidersData", "WithPaging");
			Assert.That(wp < pr, msg, "WithPaging", "ProvidersRender");
			Assert.That(pr < rt, msg, "ProvidersRender", "ReturnTypesController");
			Assert.That(h1 < rt, msg, "HomeUrlController1", "ReturnTypesController");
			Assert.That(h2 < rt, msg, "HomeUrlController2", "ReturnTypesController");
			Assert.That(rt == method.LinkedMethodParts.Count - 1, "ReturnTypesController s/be last.");
		}

		[Test]
		public void DataWithoutPagingBlueCross() {
			string request = "GET/data/client/id/11/providers/id/10";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(7, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/data/client/id/11/providers/id/{dataId}", url);
			
			int dr = GetIndexOf(typeof(DataRoot), method);
			int h2 = GetIndexOf(typeof(HomeUrlController2), method);
			int h1 = GetIndexOf(typeof(HomeUrlController1), method);
			int pd = GetIndexOf(typeof(ProvidersData), method);
			int bd = GetIndexOf(typeof(BlueCrossProvidersData), method);
			int pr = GetIndexOf(typeof(ProvidersRender), method);
			int rt = GetIndexOf(typeof(ReturnTypesController), method);
			
			string msg = "Controllers in wrong order. Controller {0} s/be before Controller {1}.";
			
			Assert.That(dr < pd, msg, "DataRoot", "ProvidersData");
			Assert.That(pd < bd, msg, "ProvidersData", "BlueCrossProvidersData");
			Assert.That(bd < pr, msg, "BlueCrossProvidersData", "ProvidersRender");
			Assert.That(pr < rt, msg, "ProvidersRender", "ReturnTypesController");
			Assert.That(h1 < rt, msg, "HomeUrlController1", "ReturnTypesController");
			Assert.That(h2 < rt, msg, "HomeUrlController2", "ReturnTypesController");
			Assert.That(rt == method.LinkedMethodParts.Count - 1, "ReturnTypesController s/be last.");
		}

		[Test]
		public void DataWithPagingBlueCross() {
			string request = "GET/data/client/id/11/providers/id/10/withpaging/30/1";
			IMethod method = _dispatcher.GetMethodAt(TestHarness.NewUrl(request));
			Assert.AreEqual(8, method.LinkedMethodParts.Count);
			string url = method.Url.Name;
			Console.WriteLine(_dispatchMessage, request, url);
			Assert.AreEqual("GET/data/client/id/11/providers/id/{dataId}/withpaging/{linesPerPage}/{pageNumber}", url);

			int dr = GetIndexOf(typeof(DataRoot), method);
			int pd = GetIndexOf(typeof(ProvidersData), method);
			int bd = GetIndexOf(typeof(BlueCrossProvidersData), method);
			int wp = GetIndexOf(typeof(WithPaging), method);
			int pr = GetIndexOf(typeof(ProvidersRender), method);
			int h1 = GetIndexOf(typeof(HomeUrlController1), method);
			int h2 = GetIndexOf(typeof(HomeUrlController2), method);
			int rt = GetIndexOf(typeof(ReturnTypesController), method);

			string msg = "Controllers in wrong order. Controller {0} s/be before Controller {1}.";

			Assert.That(dr < pd, msg, "DataRoot", "ProvidersData");
			Assert.That(pd < bd, msg, "ProvidersData", "BlueCrossProvidersData");
			Assert.That(bd < wp, msg, "BlueCrossProvidersData", "WithPaging");
			Assert.That(wp < pr, msg, "WithPaging", "ProvidersRender");
			Assert.That(pr < rt, msg, "ProvidersRender", "ReturnTypesController");
			Assert.That(h1 < rt, msg, "HomeUrlController1", "ReturnTypesController");
			Assert.That(h2 < rt, msg, "HomeUrlController2", "ReturnTypesController");
			Assert.That(rt == method.LinkedMethodParts.Count - 1, "ReturnTypesController s/be last.");
		}

		#region helper methods
		int GetIndexOf(Type controllerType, IMethod method) {
			return GetIndexOf(controllerType.FullName, method);
		}
		int GetIndexOf(string fullName, IMethod method) {
			for (int i = 0; i < method.LinkedMethodParts.Count; i++) {
				if (fullName == method.LinkedMethodParts[i].ControllerInfo.Name)
					return i;
			}
			return -1;
		}
		#endregion
	}
}
