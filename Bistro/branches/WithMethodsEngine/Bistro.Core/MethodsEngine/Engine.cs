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
using System.Linq;
using System.Text;
using Bistro.MethodsEngine.Subsets;
using Bistro.Configuration.Logging;
using Bistro.MethodsEngine.Reflection;
using Bistro.Controllers.Descriptor;
using BindPointDescriptor = Bistro.Controllers.Descriptor.ControllerDescriptor.BindPointDescriptor;
using Bistro.Controllers;
using Bistro.Interfaces;

namespace Bistro.MethodsEngine
{
    /// <summary>
    /// Main methods engine class.
    /// </summary>
    internal class Engine
    {

        enum Errors
        {
            [DefaultMessage("Binding not found in the map: {0} ")]
            BindingNotFound,
            [DefaultMessage("Controller type {0} does not support IMethodsBindPointDesc")]
            InterfaceNotSupported
        }



        /// <summary>
        /// map of the binding urls to the list of the bind points.
        /// </summary>
        private Dictionary<string, List<IMethodsBindPointDesc>> map = new Dictionary<string, List<IMethodsBindPointDesc>>();


        /// <summary>
        /// url sebsets processor.
        /// </summary>
        private SubSetsProcessor processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Engine"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        internal Engine(ILogger logger)
        {
            Logger = logger;
            processor = new SubSetsProcessor(this);
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        internal ILogger Logger { get; private set; }



        internal void RegisterController(IControllerDescriptor info)
        {
            IMethodsControllerDesc infoNew = info as IMethodsControllerDesc;
            if (infoNew == null)
            {
                Logger.Report(Errors.InterfaceNotSupported, info.ControllerTypeName);
            }

            RegisterController(infoNew);
        }

        /// <summary>
        /// Registers the controller in the engine.
        /// </summary>
        /// <param name="info">The controller descriptor.</param>
        internal void RegisterController(IMethodsControllerDesc info)
        {
            List<string> newBindUrls = new List<string>();
            foreach (IMethodsBindPointDesc bindPoint in info.Targets)
            {
                List<IMethodsBindPointDesc> descriptors = null;

                if (!map.TryGetValue(bindPoint.Target, out descriptors))
                {
                    descriptors = new List<IMethodsBindPointDesc>();
                    map.Add(bindPoint.Target, descriptors);
                    newBindUrls.Add(bindPoint.Target);
                }

                int i = 0;
                foreach (IMethodsBindPointDesc comparedBindPoint in descriptors)
                {
                    if (comparedBindPoint.Priority > bindPoint.Priority)
                        break;

                    i++;
                }

                descriptors.Insert(i, bindPoint);
            }
            foreach (string bindUrl in newBindUrls)
            {
                processor.AddNewBinding(bindUrl);
            }
            processor.UpdateBindPoints();


        }

        /// <summary>
        /// Determines whether the request url has exact bind in the <c>map</c>
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <returns>
        /// 	<c>true</c> if the request url has exact bind in the map; otherwise, <c>false</c>.
        /// </returns>
        public bool HasExactBind(string requestUrl)
        {
            return map.ContainsKey(requestUrl);

        }

        /// <summary>
        /// This method should be used by SubSetsProcessor to retrieve Bind Points associated with GenBindings
        /// </summary>
        /// <param name="binding">binding to use for search</param>
        /// <returns>List of associated bind points found in the map</returns>
        internal List<IMethodsBindPointDesc> GetTypesByBinding(GenBinding binding)
        {
            if (map.ContainsKey(binding.InitialUrl))
            {
                return map[binding.InitialUrl];
            }
            Logger.Report(Errors.BindingNotFound, binding.InitialUrl);
            throw new ApplicationException("Binding not found");
            
        }




        /// <summary>
        /// Gets the array of ControllerInvocationInfo objects for the url.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <returns></returns>
        public ControllerInvocationInfo[] GetControllers(string requestUrl)
        {
            MethodUrlsSubset urlSubSet = processor.GetMethodByUrl(requestUrl);

            var retList = urlSubSet.BindPointsList.Select(bpd => new ControllerInvocationInfo((BindPointDescriptor)bpd, new Dictionary<string, string>()));

            return retList.ToArray();
        }




        //internal virtual void RaiseInvalidBinding(ControllerType controller, params string[] args)
        //{
        //}

        internal virtual void RaiseResourceLoop(string methodUrl, IEnumerable<IMethodsControllerDesc> controllers, params string[] args)
        {
        }

        internal virtual void RaiseMissingProvider(string resName, IEnumerable<IMethodsControllerDesc> controllers, params string[] args)
        {
        }

        internal virtual void RaiseInconsistentResourceType(string methodUrl, string resName, IEnumerable<IMethodsControllerDesc> controllers, params string[] args)
        {
        }



    }
}
