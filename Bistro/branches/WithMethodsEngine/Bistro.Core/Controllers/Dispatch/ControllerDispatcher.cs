/****************************************************************************
 * 
 *  Bistro Framework Copyright © 2003-2009 Hill30 Inc
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
using System.Collections;

using Bistro.Controllers.Descriptor;
using System.Diagnostics;
using Bistro.Controllers.Security;
using Bistro.Controllers.Dispatch;
using System.Reflection;
using Bistro.Configuration.Logging;
using System.Text.RegularExpressions;
using Bistro.MethodsEngine;
using Bistro.Interfaces;

namespace Bistro.Controllers.Dispatch
{
    /// <summary>
    /// Manages controller application to urls
    /// </summary>
    public class ControllerDispatcher : IControllerDispatcher
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerDispatcher"/> class.
        /// </summary>
        public ControllerDispatcher(Application application) 
        {
            logger = application.LoggerFactory.GetLogger(GetType());
            engine = new Engine(logger);
        }

        /// <summary>
        /// Our logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Engine to process bindings
        /// </summary>
        private Engine engine;


        /// <summary>
        /// Registers the controller with the dispatcher.
        /// </summary>
        /// <param name="info">The controller info.</param>
        public virtual void RegisterController(IControllerDescriptor info)
        {
            engine.RegisterController(info);
        }



        public virtual ControllerInvocationInfo[] GetControllers(string requestUrl)
        {
            return engine.GetControllers(requestUrl);
        }


        /// <summary>
        /// Determines whether the specified url has a controller explicitly bound to it
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <returns>
        /// 	<c>true</c> if an exact binding exists; otherwise, <c>false</c>.
        /// </returns>
        public bool HasExactBind(string requestUrl)
        {
            return engine.HasExactBind(requestUrl);
            
        }
    }
}