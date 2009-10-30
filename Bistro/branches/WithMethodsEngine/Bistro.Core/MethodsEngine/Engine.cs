using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.MethodsEngine.Subsets;
using Bistro.Configuration.Logging;
using Bistro.MethodsEngine.Reflection;

namespace Bistro.MethodsEngine
{
    internal class Engine
    {
        private SubSetsProcessor processor;

        internal Engine(ILogger logger)
        {
//            root = new Binding(this);
            processor = new SubSetsProcessor(this);
        }

        internal void ProcessControllers(List<ITypeInfo> controllers)
        {
            processor.ProcessControllers(controllers);
        }


        internal virtual void RaiseInvalidBinding(ControllerType controller, params string[] args)
        {
        }

        internal virtual void RaiseResourceLoop(string methodUrl, IEnumerable<Controller> controllers, params string[] args)
        {
        }

        internal virtual void RaiseMissingProvider(string methodUrl, string resName, IEnumerable<ControllerType> controllers, params string[] args)
        {
        }

        internal virtual void RaiseInconsistentResourceType(string methodUrl, string resName, IEnumerable<ControllerType> controllers, params string[] args)
        {
        }

    }
}
