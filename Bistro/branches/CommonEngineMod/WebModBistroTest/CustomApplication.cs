using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BistroApi;
using BistroModel;
using ControllersC;
namespace WebModBistroTest {
	public class CustomApplication : Application 
	{
		int _a;
		public CustomApplication() 
		{
			ControllersC.Default tmp = new Default();
			_a = 1;
		}
		public int A { get { return _a; } }
	}
}
