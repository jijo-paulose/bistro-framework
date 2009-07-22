using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Controllers;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Security;

namespace Bistro.UnitTests
{
	public abstract class MyAbstractBase : AbstractController {
		public override void DoProcessRequest(IExecutionContext context) { }
	}
    [Bind("/")]
    public class HomeUrlController1 : AbstractController
    {
        public override void DoProcessRequest(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    [Bind("/")]
    public class HomeUrlController2 : AbstractController
    {
        public override void DoProcessRequest(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    [Bind("/hello/?/you")]
    public class HelloYouController1 : AbstractController
    {
        public override void DoProcessRequest(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    [Bind("/hello/*/you")]
    public class HelloYouController2 : AbstractController
    {
        public override void DoProcessRequest(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
    [Bind("/order/world/new")]
    public class OrderController1 : AbstractController
    {   [Session]
        public string c1;
        [Session, Requires]
        public string c2;
        
        public override void DoProcessRequest(IExecutionContext context)
        {
            c1 = c2 + "1";
        }
    }
    [Bind("/order/world/new")]
    public class OrderController2 : AbstractController
    {
        [Session]
        public string c2;
        [Session, Requires]
        public string c5;

        public override void DoProcessRequest(IExecutionContext context)
        {
            c2 = c5 + "2";
        }
    }
    [Bind("/order/world/new")]
    public class OrderController3 : AbstractController
    {
        [Session]
        public string c3;

        [Session, Requires]
        public string c2;
        [Session, Requires]
        public string c4;
        [Session, Requires]
        public string c5;

        public override void DoProcessRequest(IExecutionContext context)
        {
            c3 = c2 + c4 + c5 + "3";
        }
    }
    [Bind("/order/world/new")]
    public class OrderController4 : AbstractController
    {
        [Session]
        public string c4;

        [Session, Requires]
        public string c1;
        [Session, Requires]
        public string c2;
        [Session, Requires]
        public string c5;

        public override void DoProcessRequest(IExecutionContext context)
        {
            c4 = c1 + c2 + c5 + "4";
        }
    }
    [Bind("/order/world/new")]
    public class OrderController5 : AbstractController
    {
        [Session]
        public string c5;

        public override void DoProcessRequest(IExecutionContext context)
        {
            c5 = "5";
        }
    }
    [Bind("/order/world/new")]
    public class OrderController6 : AbstractController
    {
        [Session]
        public string c6;
        [Session, Requires]
        public string c3;
        [Session, Requires]
        public string c7;


        public override void DoProcessRequest(IExecutionContext context)
        {
            c6 = c3 + c7 + "6";
        }
    }
    [Bind("/order/world/new")]
    public class OrderController7 : AbstractController
    {
        [Session]
        public string c7;

        public override void DoProcessRequest(IExecutionContext context)
        {
            c7 = "7";
        }
    }
    
    [Bind("/one_little_url")]
    public class littleController1 : AbstractController
    {
        [Request]
        public string l1;
        [Request, Requires]
        public string l2;

        public override void DoProcessRequest(IExecutionContext context)
        {
            l1 = "1" + l2;
        }
    }
    [Bind("/one_little_url")]
    public class littleController2 : AbstractController
    {
        [Request]
        public string l2;

        public override void DoProcessRequest(IExecutionContext context)
        {
            l2 = "2";
        }
    }
    [Bind("/little_url/more")]
    public class littleController3 : AbstractController
    {
        [Request]
        public string l3;

        public override void DoProcessRequest(IExecutionContext context)
        {
            l3 = "3";
        }
    }
    [Bind("/little_url/more")]
    public class littleController4 : AbstractController
    {
        [Request]
        public string l4;
        [Request, Requires]
        public string l3;
        [Request, Requires]
        public string l5;

        public override void DoProcessRequest(IExecutionContext context)
        {
            l4 = "4" + l3 + l5;
        }
    }
    [Bind("/little_url/more")]
    public class littleController5 : AbstractController
    {
        [Request]
        public string l5;
        [Request, Requires]
        public string l3;

        public override void DoProcessRequest(IExecutionContext context)
        {
            l5 = "5" + l3;
        }
    }

		#region GET/hi/...
		[Bind("GET/hi/new/world/a")]
		public class hiController1 : MyAbstractBase { }
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
				//IContext context, IDictionary<string, KeyValuePair<FailAction, string>> failedPermissions
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
		[Bind("GET/secureA")]
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
	}
