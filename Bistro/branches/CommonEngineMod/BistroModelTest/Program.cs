using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;
using BistroModel;
using System.Reflection;

namespace BistroModelTest {
	class Program {
		IDispatcher _dispatcher;
		ILoader _loader;
		static void Main(string[] args) {
			Program p = new Program();
			//p.TestEH();
			p.Test();
		}
		void TestEH() {
			try {
				Test();
			}
			catch (Exception ex) {
				Console.WriteLine(ex.ToString());
				Console.WriteLine("paused...");
				Console.ReadLine();
			}
		}
		void Test() {
			Assembly.LoadFrom("NDjango.BistroIntegrationMod.dll");
			Assembly.LoadFrom("ControllersA.dll");
			_dispatcher = Global.Application.CreateDispatcher();
			_loader = Global.Application.CreateLoader(_dispatcher);
			_loader.Load();
			_dispatcher.BuildMethods();
		}
	}
}
