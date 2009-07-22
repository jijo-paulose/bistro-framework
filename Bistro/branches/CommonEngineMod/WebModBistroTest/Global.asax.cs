using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Reflection;
using ControllersC;

namespace WebModBistroTest {
	public class Global : System.Web.HttpApplication {
		protected void Application_Start(object sender, EventArgs e) {
			//force loading of controller assemblies:
			//Assembly.LoadFrom("./bin/ControllersC.dll");
		}

		protected void Session_Start(object sender, EventArgs e) {

		}

		protected void Application_BeginRequest(object sender, EventArgs e) {

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e) {

		}

		protected void Application_Error(object sender, EventArgs e) {

		}

		protected void Session_End(object sender, EventArgs e) {

		}

		protected void Application_End(object sender, EventArgs e) {
		}
	}
}