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

namespace Bistro.MethodsEngine
{
    internal class Engine
    {

        Dictionary<string, List<BindPointDescriptor>> map = new Dictionary<string, List<BindPointDescriptor>>();
        private SubSetsProcessor processor;

        internal Engine(ILogger logger)
        {
            Logger = logger;
//            root = new Binding(this);
            processor = new SubSetsProcessor(this);
        }

        internal ILogger Logger {get;set;}




        internal void RegisterController(ControllerDescriptor info)
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


        public List<ControllerInvocationInfo> GetControllers(string requestUrl)
        {

            List<ControllerInvocationInfo> retList = new List<ControllerInvocationInfo>();

            MethodUrlsSubset urlSubSet = processor.GetMethodByUrl(requestUrl);

            var genBindings = urlSubSet.BindingsList.Where(ctr => ctr.MatchStatus);

            foreach (var binding in genBindings)
            {
                foreach(var bindPoint in map[binding.InitialUrl])
                {
                    retList.Add(new ControllerInvocationInfo(bindPoint,new Dictionary<string,string>(),1));
                }
            }

            return retList;
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
