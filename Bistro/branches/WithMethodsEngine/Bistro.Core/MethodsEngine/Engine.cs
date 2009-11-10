﻿/****************************************************************************
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
    internal class Engine
    {

        enum Errors
        {
            [DefaultMessage("Binding not found in the map: {0} ")]
            BindingNotFound
        }



        Dictionary<string, List<BindPointDescriptor>> map = new Dictionary<string, List<BindPointDescriptor>>();
        private SubSetsProcessor processor;

        internal Engine(ILogger logger)
        {
            Logger = logger;
            processor = new SubSetsProcessor(this);
        }

        internal ILogger Logger {get;set;}




        internal void RegisterController(IControllerDescriptor info)
        {
            List<string> newBindUrls = new List<string>();
            foreach (BindPointDescriptor bindPoint in info.Targets)
            {
                List<BindPointDescriptor> descriptors = null;

                if (!map.TryGetValue(bindPoint.Target, out descriptors))
                {
                    descriptors = new List<BindPointDescriptor>();
                    map.Add(bindPoint.Target, descriptors);
                    newBindUrls.Add(bindPoint.Target);
                }

                int i = 0;
                foreach (BindPointDescriptor comparedBindPoint in descriptors)
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


        }

        public bool HasExactBind(string requestUrl)
        {
            return map.ContainsKey(requestUrl);

        }

        /// <summary>
        /// This method should be used by SubSetsProcessor to retrieve IControllerTypeInfos associated with GenBindings
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        internal List<IMethodsBindPointDesc> GetTypesByBinding(GenBinding binding)
        {
            if (map.ContainsKey(binding.InitialUrl))
            {
                return map[binding.InitialUrl].OfType<IMethodsBindPointDesc>().ToList();
            }
            Logger.Report(Errors.BindingNotFound, binding.InitialUrl);
            throw new ApplicationException("Binding not found");
            
        }




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