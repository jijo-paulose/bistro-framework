using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using BistroApi;
using BistroModel;

namespace ControllersC {
	public abstract class MyBase : AbstractController {
		public override void DoProcessRequest(IExecutionContext context) {}
	}

	[Bind("/")]
	//[Deny("?", OnFailure = FailAction.Redirect, Target = "/logon")]
	public class MySecure : SecurityController {
		[Request]
		int securityMain = 0;
		public override bool HasAccess(IContext context, IDictionary<string, KeyValuePair<FailAction, string>> failedPermissions) {
			//context.tr
			if (context.Url.SubUrl == null) //allow root
				return true;
			if (context.Url.SubUrl.Name == "logon")
				return true;
			if (context.CurrentUser != null && !(context.CurrentUser is AnonymousUser)  )
				return true;
			context.Transfer(context.Url.Head);
			return false;
		}
	}

	/*[Bind("/")]
	public class MySecure2 : SecurityController {
		[Request, DependsOn]
		int securityMain;
		public override bool HasAccess(IContext context, IDictionary<string, KeyValuePair<FailAction, string>> failedPermissions) {
			return true;
		}
	}*/

	public class Default : AbstractController {
		public override void DoProcessRequest(IExecutionContext context) {
			//throw new NotImplementedException();
		}
	}

	[Bind("/")]
	public class HomeUrlController1 : AbstractController {
		public override void DoProcessRequest(IExecutionContext context) {
			//throw new NotImplementedException();
		}
	}

	[Bind("/logon")]
	[RenderWith("templates/logon.django")]
	public class DoLogon : MyBase
	{
		[Request, DependsOn]
		int _defaultHomeRender = 0;

		[FormField, Request]
		protected string
		username,
		password;
	}

	[Bind("/")]
	[Bind("/A")]
	public class HomeUrlController2 : AbstractController //, ISecurityController
	{
		[Request]
		int _defaultHomeRender = 0;
		[Request]
		int _a;
		[FormField, Request]
		string myTextA;
		[FormField]
		string myTextB;
		public override void DoProcessRequest(IExecutionContext context) {
			_a = 5;
			_a = _defaultHomeRender;
			myTextA = "zz top";
			myTextB = "foo fighters";
			context.Response.RenderWith("templates\\default.django");
			//throw new NotImplementedException();
		}

		#region ISecurityController Members

		public bool HasAccess(IContext context, IDictionary<string, KeyValuePair<FailAction, string>> failedPermissions) {
			return true;
		}

		#endregion
	}

	[Bind("/A/B/C")]
	public class ABC : AbstractController {
		public override void DoProcessRequest(IExecutionContext context) {
			//
		}
	}

	[Bind("/A/PutFile")]
	public class PutFileController : AbstractController {
		[Request, DependsOn]
		int _a;

		[FormField]
		public HttpPostedFileBase myFileA;

		[FormField]
		public string myFileB;

		[FormField]
		public byte[] myFileC;

		[FormField]
		public Stream myFileD;

		[Request]
		string myTextA;

		[FormField]
		string myTextB;

		public override void DoProcessRequest(IExecutionContext context) {
			int x = _a;
			string a = myTextA;
			string b = myTextB;
			context.Response.RenderWith("templates\\default.django");
		}
	}

	[Bind("/A/{junk}/B/{stuff}")]
	public class TestParam : AbstractController {
		[Request]
		string junk;
		string stuff;
		public override void DoProcessRequest(IExecutionContext context) {
			string test1 = junk;
			string test2 = stuff;
		}
	}

	[Bind("/hello/?/you")]
	public class HelloYouController1 : AbstractController {
		public override void DoProcessRequest(IExecutionContext context) {
			throw new NotImplementedException();
		}
	}

	[Bind("/hello/*/you")]
	public class HelloYouController2 : AbstractController {
		public override void DoProcessRequest(IExecutionContext context) {
			throw new NotImplementedException();
		}
	}

	[Bind("/order/world/new")]
	public class OrderController1 : AbstractController {
		[Session]
		public string c1;
		[Session, Requires]
		public string c2;

		public override void DoProcessRequest(IExecutionContext context) {
			c1 = c2 + "1";
		}
	}

	[Bind("/order/world/new")]
	public class OrderController2 : AbstractController {
		[Session]
		public string c2;
		[Session, Requires]
		public string c5;

		public override void DoProcessRequest(IExecutionContext context) {
			c2 = c5 + "2";
		}
	}

	[Bind("/order/world/new")]
	public class OrderController3 : AbstractController {
		[Session]
		public string c3;

		[Session, Requires]
		public string c2;
		[Session, Requires]
		public string c4;
		[Session, Requires]
		public string c5;

		public override void DoProcessRequest(IExecutionContext context) {
			c3 = c2 + c4 + c5 + "3";
		}
	}

	[Bind("/order/world/new")]
	public class OrderController4 : AbstractController {
		[Session]
		public string c4;

		[Session, Requires]
		public string c1;
		[Session, Requires]
		public string c2;
		[Session, Requires]
		public string c5;

		public override void DoProcessRequest(IExecutionContext context) {
			c4 = c1 + c2 + c5 + "4";
		}
	}

	[Bind("/order/world/new")]
	public class OrderController5 : AbstractController {
		[Session]
		public string c5;

		public override void DoProcessRequest(IExecutionContext context) {
			c5 = "5";
		}
	}

	[Bind("/order/world/new")]
	public class OrderController6 : AbstractController {
		[Session]
		public string c6;
		[Session, Requires]
		public string c3;
		[Session, Requires]
		public string c7;


		public override void DoProcessRequest(IExecutionContext context) {
			c6 = c3 + c7 + "6";
		}
	}

	[Bind("/order/world/new")]
	public class OrderController7 : AbstractController {
		[Session]
		public string c7;

		public override void DoProcessRequest(IExecutionContext context) {
			c7 = "7";
		}
	}

	[Bind("/one_little_url")]
	public class littleController1 : AbstractController {
		[Request]
		public string l1;
		[Request, Requires]
		public string l2;

		public override void DoProcessRequest(IExecutionContext context) {
			l1 = "1" + l2;
			System.Web.HttpContext.Current.Response.Write("<p>So did this!</p></body></html>");
		}
	}

	[Bind("/one_little_url")]
	public class littleController2 : AbstractController {
		[Request]
		public string l2;

		public override void DoProcessRequest(IExecutionContext context) {
			l2 = "2";
			System.Web.HttpContext.Current.Response.Write("<html><body><p>It worked!</p>");
		}
	}

	//[Bind("/little_url/more", ControllerBindType=BindType.After)]
	[Bind("/little_url/more")]
	public class littleController3 : AbstractController {
		[Request]
		public string l3;

		public override void DoProcessRequest(IExecutionContext context) {
			l3 = "3";
		}
	}

	[Bind("/little_url/more")]
	public class littleController4 : AbstractController {
		[Request]
		public string l4;
		[Request, Requires]
		public string l3;
		[Request, Requires]
		public string l5;

		public override void DoProcessRequest(IExecutionContext context) {
			l4 = "4" + l3 + l5;
		}
	}

	[Bind("/little_url/more")]
	public class littleController5 : AbstractController {
		[Request]
		public string l5;
		[Request, Requires]
		public string l3;

		public override void DoProcessRequest(IExecutionContext context) {
			l5 = "5" + l3;
		}
	}
}
