using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;
using BistroModel;

namespace ModBistroUnitTests {
	public abstract class MyAbstractBase : AbstractController 
	{
		public override void DoProcessRequest(IExecutionContext context) { }
	}
	
	#region home/root
	[Bind("/?")]
	public class HomeUrlController1 : MyAbstractBase { }
	[Bind("/?")]
	public class HomeUrlController2 : MyAbstractBase { }
	#endregion

	#region /hello/...
	[Bind("/hello/?/you")]
	public class HelloYouController1 : MyAbstractBase { }
	[Bind("/hello/*/you")]
	public class HelloYouController2 : MyAbstractBase { }
	#endregion

	#region /order/world/new
	[Bind("/order/world/new")]
	public class OrderController1 : MyAbstractBase {
		[Session]
		public string c1;
		[Session, Requires]
		public string c2;

		public override void DoProcessRequest(IExecutionContext context) {
			c1 = c2 + "1";
		}
	}
	[Bind("/order/world/new")]
	public class OrderController2 : MyAbstractBase {
		[Session]
		public string c2;
		[Session, Requires]
		public string c5;

		public override void DoProcessRequest(IExecutionContext context) {
			c2 = c5 + "2";
		}
	}
	[Bind("/order/world/new")]
	public class OrderController3 : MyAbstractBase {
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
	public class OrderController4 : MyAbstractBase {
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
	public class OrderController5 : MyAbstractBase {
		[Session]
		public string c5;

		public override void DoProcessRequest(IExecutionContext context) {
			c5 = "5";
		}
	}
	[Bind("/order/world/new")]
	public class OrderController6 : MyAbstractBase {
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
	public class OrderController7 : MyAbstractBase {
		[Session]
		public string c7;

		public override void DoProcessRequest(IExecutionContext context) {
			c7 = "7";
		}
	}
	#endregion

	#region /one_little_url
	[Bind("/one_little_url")]
	public class littleController1 : MyAbstractBase {
		[Request]
		public string l1;
		[Request, Requires]
		public string l2;

		public override void DoProcessRequest(IExecutionContext context) {
			l1 = "1" + l2;
		}
	}
	[Bind("/one_little_url")]
	public class littleController2 : MyAbstractBase {
		[Request]
		public string l2;

		public override void DoProcessRequest(IExecutionContext context) {
			l2 = "2";
		}
	}
	#endregion

	#region /little_url/more
	[Bind("/little_url/more")]
	public class littleController3 : MyAbstractBase {
		[Request]
		public string l3;

		public override void DoProcessRequest(IExecutionContext context) {
			l3 = "3";
		}
	}
	[Bind("/little_url/more")]
	public class littleController4 : MyAbstractBase {
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
	public class littleController5 : MyAbstractBase {
		[Request]
		public string l5;
		[Request, Requires]
		public string l3;

		public override void DoProcessRequest(IExecutionContext context) {
			l5 = "5" + l3;
		}
	}
	#endregion
	
	#region GET/hi/...
	[Bind("GET/hi/new/world/a")]
	public class hiController1 : MyAbstractBase {	}
	[Bind("GET/hi/new/*/*/now")]
	public class hiController2 : MyAbstractBase { }
	[Bind("GET/hi/*/world/?/now")]
	public class hiController3 : MyAbstractBase { }
	[Bind("GET/hi/*/world/*/now")]
	public class hiController4 : MyAbstractBase { }
	[Bind("GET/hi/*/world/a/now")]
	public class hiController5 : MyAbstractBase { }
	[Bind("GET/hi/*/world/a")]
	public class hiController6 : MyAbstractBase { }
	[Bind("GET/hi/*/world/a/*")]
	public class hiController7 : MyAbstractBase { }
	#endregion

	#region GET/secure/...
	[Bind("GET/secure")]
	public class SecurityController1 : SecurityController {
		public override bool HasAccess(IContext context, IDictionary<string, KeyValuePair<FailAction, string>> failedPermissions) {
			return true;
		}
	}
	[Bind("Get/secure/one")]
	public class SecuredController1 : MyAbstractBase { }
	#endregion

	#region GET/secureA/...
	[Bind("GET/secureA")]
	public class SecurityControllerA1 : SecurityController {
		[Request, DependsOn]
		int z = 5;
		
		public override bool HasAccess(IContext context, IDictionary<string, KeyValuePair<FailAction, string>> failedPermissions) {
			int a = z;
			return true;
			//return false;
		}
	}
	[Bind("GET/secureA")]
	public class SecurityControllerA2 : SecurityController {
		[Request]
		int z = 1;

		public override bool HasAccess(IContext context, IDictionary<string, KeyValuePair<FailAction, string>> failedPermissions) {
			int a = z;
			return false;
		}
	}
	[Bind("Get/secureA")]
	public class SecuredControllerA : MyAbstractBase {
		[Request]
		int z = 6;
		void UseVar() { int a = z; }
	}
	#endregion

	#region DependsOn/Requires
	[Bind("GET/dependson/requires")]
	public class DRController2 : MyAbstractBase {
		[Request, Requires]
		int z = 1;
		public override void DoProcessRequest(IExecutionContext context) { z = z + 1; }
	}
	[Bind("GET/dependson/requires")]
	public class DRController1 : MyAbstractBase {
		//[Request, DependsOn]
		[Request]
		int z = 1;
		public override void DoProcessRequest(IExecutionContext context) { z = z + 1; }
	}
	#endregion

	#region DataQPaging

	[Bind("GET/data/?")]
	public class DataRoot : MyAbstractBase {
		[Request]
		bool dataRoot = true;

		public override void DoProcessRequest(IExecutionContext context) {
			bool b = dataRoot;
		}
	}

	[Bind("GET/data/client/id/{clientId}/providers/id/{dataId}")]
	[Bind("GET/data/client/id/{clientId}/providers/id/{dataId}/withpaging/{linesPerPage}/{pageNumber}")]
	public class ProvidersData : MyAbstractBase {
		[Request, Requires]
		bool dataRoot = false;

		[Request]
		bool dataSource = true;

		int dataId = 0;

		public override void DoProcessRequest(IExecutionContext context) {
			bool b = dataSource;
			int id = dataId;
			bool d = dataRoot;
		}
	}

	//todo: look at [11] idea:
	//[Bind("GET/data/client/id/[11]/providers/id/{dataId}")]
	//[Bind("GET/data/client/id/[11]/providers/id/{dataId}/withpaging/{linesPerPage}/{pageNumber}")]
	[Bind("GET/data/client/id/11/providers/id/{dataId}")]
	[Bind("GET/data/client/id/11/providers/id/{dataId}/withpaging/{linesPerPage}/{pageNumber}")]
	public class BlueCrossProvidersData : MyAbstractBase {
		[Request, Requires]
		bool dataSource = false;

		[Request]
		bool dataSourceCustom = true;

		int dataId = 0;

		public override void DoProcessRequest(IExecutionContext context) {
			bool b = dataSourceCustom;
			int id = dataId;
			bool d = dataSource;
		}
	}

	[Bind("GET/data/?/withpaging/{linesPerPage}/{pageNumber}")]
	public class WithPaging : MyAbstractBase {
		[Request, DependsOn]
		bool dataSource = false;
		[Request, DependsOn]
		bool dataSourceCustom = false;

		[Request]
		bool withPaging = true;

		public override void DoProcessRequest(IExecutionContext context) {
			bool b = dataSourceCustom;
			bool a = dataSource;
			bool p = withPaging;
		}
	}

	[Bind("GET/data/client/id/*/providers/id/*")]
	public class ProvidersRender : MyAbstractBase {
		[Request, Requires]
		bool dataSource = false;

		[Request, DependsOn]
		bool dataSourceCustom = false;

		[Request, DependsOn]
		bool withPaging = false;

		public override void DoProcessRequest(IExecutionContext context) {
			bool b = dataSource;
			bool a = dataSourceCustom;
			bool p = withPaging;
		}
	}

	#endregion
}
