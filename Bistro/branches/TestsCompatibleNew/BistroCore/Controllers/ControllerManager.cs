/****************************************************************************
 * 
 *  Bistro Framework Copyright � 2003-2009 Hill30 Inc
 *
 *  This file is part of Bistro Framework.
 *
 *  Bistro Framework is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Bistro Framework is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Bistro Framework.  If not, see <http://www.gnu.org/licenses/>.
 *  
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Web;
using Bistro.Controllers.Descriptor;
using System.Xml;
using Bistro.Configuration.Logging;
using Bistro.Controllers.Dispatch;
using Bistro.Special.Reflection;
using Bistro.Reflection.CLRTypeInfo;

namespace Bistro.Controllers
{
    /// <summary>
    /// Manages the lifecycle of a controller
    /// </summary>
    public class ControllerManager : IControllerManager
    {
        enum Messages
        {
            [DefaultMessage("Exception occured during applicaiton load")]
            ExceptionDuringLoad,
            [DefaultMessage("Exception occured attempting to load assembly '{0}'. This is likely caused by assembly version mismatches. Exception follows. \r\n{1}")]
            ExceptionLoadingAssembly
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerManager"/> class.
        /// </summary>
        /// <param name="handlerFactory">The handler factory.</param>
        /// <param name="dispatcherFactory">The dispatcher factory.</param>
        public ControllerManager(Application application)
        {
            Name = "Default controller manager";
            this.application = application;
            logger = application.LoggerFactory.GetLogger(GetType());
            this.handlerFactory = application.HandlerFactory;
            this.dispatcherFactory = application.DispatcherFactory;
        }
        private ILogger logger;

        private Application application;

        /// <summary>
        /// The handler factory to use
        /// </summary>
        IControllerHandlerFactory handlerFactory;

        /// <summary>
        /// The dispatcher factory to use
        /// </summary>
        IDispatcherFactory dispatcherFactory;

        /// <summary>
        /// A mapping of controller type to handler instance
        /// </summary>
        private Dictionary<ITypeInfo, IControllerHandler> handlers = new Dictionary<ITypeInfo, IControllerHandler>();

        ///// <summary>
        ///// The full listing of all controller descriptors known to the sysetm
        ///// </summary>
        //private List<ControllerDescriptor> controllers = new List<ControllerDescriptor>();

        /// <summary>
        /// Indicates whether loading has finished
        /// </summary>
        private bool loaded = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ControllerManager"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded
        {
            get { return loaded; }
            set { loaded = value; }
        }

        /// <summary>
        /// Loads all currently available controllers, and subscribes to events of newly loaded assemblies do add new controllers.
        /// </summary>
        public virtual void Load()
        {
            foreach (Assembly assm in AppDomain.CurrentDomain.GetAssemblies())
                LoadAssembly(assm);

            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
            loaded = true;
        }

        /// <summary>
        /// Handles the AssemblyLoad event of the CurrentDomain.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.AssemblyLoadEventArgs"/> instance containing the event data.</param>
        void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            LoadAssembly(args.LoadedAssembly);
        }

        /// <summary>
        /// Loads the assembly.
        /// </summary>
        /// <param name="assm">The assm.</param>
        protected virtual void LoadAssembly(Assembly assm)
        {
            try
            {
                foreach (Type t in assm.GetTypes())
                    if (t.GetInterface(typeof(IController).FullName) != null)
                        LoadType(new CLRTypeInfo(t));
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(ex.Message);
                for (int i = 0; i < ex.Types.Length; i++)
                    if (ex.Types[i] != null)
                        sb.AppendFormat("\t{0} loaded\r\n", ex.Types[i].Name);

                for (int i = 0; i < ex.LoaderExceptions.Length; i++)
                    if (ex.LoaderExceptions[i] != null)
                        sb.AppendFormat("\texception {0}\r\n", ex.LoaderExceptions[i].Message);

                logger.Report(Messages.ExceptionLoadingAssembly, assm.FullName, sb.ToString());

                throw ex;
            }
        }

        public void SpecialLoadForTest(IEnumerable<ITypeInfo> types)
        {
            foreach(ITypeInfo typeInfo in types)
            {
                LoadType(typeInfo);
            }
        }

        /// <summary>
        /// Clear manager contents to reload items
        /// </summary>
        public void ClearAll()
        {
            handlers.Clear();
        }

        /// <summary>
        /// Loads the type.
        /// </summary>
        /// <param name="t">The t.</param>
        protected virtual void LoadType(ITypeInfo t)
        {
            if (t.IsAbstract)
                return;

            ControllerDescriptor descriptor = ControllerDescriptor.CreateDescriptor(t, logger);
            RegisterController(descriptor);
        }

        /// <summary>
        /// Registers the controller.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        public virtual void RegisterController(ControllerDescriptor descriptor)
        {
            handlers.Add(descriptor.ControllerType, handlerFactory.CreateControllerHandler(descriptor));
            dispatcherFactory.GetDispatcherInstance().RegisterController(descriptor);
        }

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="requestPoint">The request point.</param>
        /// <returns></returns>
        public IController GetController(ControllerInvocationInfo invocation, HttpContextBase context, IContext requestContext)
        {
            return handlers[invocation.BindPoint.Controller.ControllerType].GetControllerInstance(invocation, context, requestContext);
        }

        /// <summary>
        /// Returns the controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public void ReturnController(IController controller, HttpContextBase context, IContext requestContext)
        {
            handlers[controller.GlobalHandle].ReturnController(controller, context, requestContext);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set;  }
    }
}
