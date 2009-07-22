using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using BistroApi;

namespace BistroModel
{
	/// <summary>
	/// Loads IControllerInfos from assemblies.
	/// Dispatcher is passed to IControllerInfo constructor
	/// to allow registration of bindings and controllers.
	/// </summary>
	public class Loader : LoaderBase
	{
		#region private fields
		enum Messages {
			[DefaultMessage("Examining assembly {0} for IControllers.")]
			ExaminingAssembly,
			[DefaultMessage("\tFound IController {0}.")]
			FoundIController,
			[DefaultMessage("\t\t...is abstract.")]
			IsAbstract
		}
		IDispatcher _dispatcher;
		ILogger _logger;
		#endregion

		#region construction
		public Loader(IDispatcher dispatcher) 
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");
			_dispatcher = dispatcher;
			_logger = Global.Application.CreateLogger(null);
		}
		#endregion

		#region public
		/// <summary>
		/// Loads all currently available controllers, and subscribes to events of newly loaded assemblies to add new controllers.
		/// </summary>
		public override void Load() {
			foreach (Assembly assm in AppDomain.CurrentDomain.GetAssemblies())
				LoadAssembly(assm);

			AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
		}
		#endregion

		#region non-public
		/// <summary>
		/// Handles the AssemblyLoad event of the CurrentDomain.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The <see cref="System.AssemblyLoadEventArgs"/> instance containing the event data.</param>
		void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args) {
			LoadAssembly(args.LoadedAssembly);
		}
		/// <summary>
		/// Loads the assembly.
		/// </summary>
		/// <param name="assm">The assm.</param>
		protected virtual void LoadAssembly(Assembly assm) {
			_logger.Report(Messages.ExaminingAssembly, assm.FullName);
			foreach (Type t in assm.GetTypes())
				if (t.GetInterface(typeof(IController).Name) != null)
					LoadType(t);
		}
		/// <summary>
		/// Loads the type.
		/// </summary>
		/// <param name="t">The t.</param>
		protected virtual void LoadType(Type t) {
			_logger.Report(Messages.FoundIController, t.FullName);
			if (t.IsAbstract) {
				_logger.Report(Messages.IsAbstract);
				return;
			}

			//ControllerDescriptor descriptor = ControllerDescriptor.CreateDescriptor(t);
			IControllerInfo cinfo = new BControllerInfo(t, _dispatcher);
		}
		#endregion
	}
}
