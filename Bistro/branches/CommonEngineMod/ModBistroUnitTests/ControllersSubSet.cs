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
