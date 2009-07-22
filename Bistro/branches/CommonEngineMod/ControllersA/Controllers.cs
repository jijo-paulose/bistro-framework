using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;
using BistroModel;

namespace ControllersA {
	public class MyBaseController : AbstractController {
		public override void DoProcessRequest(IExecutionContext context) {
			//throw new NotImplementedException();
		}
	}

	[Bind("GET/*/B/?/Z")]
	public class Z1 : MyBaseController {
		
	}
	[Bind("GET/*/B/?/X/Z")]
	public class Z2 : MyBaseController {
		
	}
	[Bind("GET/*/B/X/Z")]
	public class Z3 : MyBaseController {
		
	}
	[Bind("GET/*/B/Z")]
	public class Z4 : MyBaseController {
		
	}
}
