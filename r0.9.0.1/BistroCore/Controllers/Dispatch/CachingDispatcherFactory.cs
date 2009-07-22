using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Controller.Dispatch
{
    public class CachingDispatcherFactory : IDispatcherFactory
    {
        private ControllerDispatcher instance = new CachingDispatcher();

        /// <summary>
        /// Creates the dispatcher.
        /// </summary>
        /// <returns></returns>
        public IControllerDispatcher CreateDispatcher()
        {
            return instance;
        }

    }
}
