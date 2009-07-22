using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;
using System.Configuration;

namespace BistroModel
{
	/// <summary>
	/// Loads an IApplication from somewhere...
	/// </summary>
	public class Global 
	{
		static IApplication _application;
		static Global() 
		{
			//Load an IApplication based on web.config app setting: BistroCustomIApplicationTypeName
			LoadCustomIApplication();
			//If not loaded, use default...
			if(_application == null)
				_application = new Application();
		}
		public static IApplication Application { get { return _application; } }
		static void LoadCustomIApplication() {
			string customApplicationTypeName = ConfigurationManager.AppSettings["BistroCustomIApplicationTypeName"];
			if (customApplicationTypeName == null)
				return;
			Type type = Type.GetType(customApplicationTypeName);
			if (type == null)
				return;
			_application = (IApplication)Activator.CreateInstance(type, null);
		}
	}
}
