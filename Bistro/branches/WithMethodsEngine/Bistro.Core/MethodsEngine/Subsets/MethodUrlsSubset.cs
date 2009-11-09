using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.MethodsEngine.Reflection;
using Bistro.Controllers.Descriptor;

namespace Bistro.MethodsEngine.Subsets
{
    public class MethodUrlsSubset
    {
        internal MethodUrlsSubset(Engine _engine)
        {
            bindPointsList = new List<IBindPointDescriptor>();
            bindingsList = new List<GenBinding>();
            engine = _engine;
        }

        private MethodUrlsSubset(Engine _engine, List<GenBinding> oldBindingsList, GenBinding newBinding)
        {
            engine = _engine;
            bindPointsList = new List<IBindPointDescriptor>();
            bindingsList = new List<GenBinding>(oldBindingsList);
            bindingsList.Add(newBinding);

            foreach (GenBinding binding in bindingsList.Where(bind => bind.MatchStatus))
            {
                foreach (IBindPointDescriptor bindPointInfo in binding.GetBindPoints())
                {
                    if (!bindPointsList.Contains(bindPointInfo))
                    {
                        bindPointsList.Add(bindPointInfo);
                    }
                }
            }

            ScanResources();

            SortBindPoints();

        }

        private void ScanResources()
        {
            resources = new Dictionary<string,Resource>();
            foreach (IBindPointDescriptor bindPoint in bindPointsList)
            {
                foreach (string resName in bindPoint.ControllerInfo.Provides
                                        .Concat(bindPoint.ControllerInfo.Requires)
                                        .Concat(bindPoint.ControllerInfo.DependsOn))
                {
                    if (!resources.ContainsKey(resName))
                    {
                        resources.Add(resName,new Resource(engine,resName));
                    }
                }
            }
        }



        /// <summary>
        /// Method to set bindpoints order depending on the parameters, priority, etc...
        /// </summary>
        private void SortBindPoints()
        {
            Func<List<IBindPointDescriptor>> init = () => new List<IBindPointDescriptor>();
            var before = init();
            var payload = init();
            var after = init();
            var teardown = init();

            foreach (IBindPointDescriptor descriptor in bindPointsList)
            {
                switch (descriptor.ControllerBindType)
                {
                    case BindType.Before:
                        before.Add(descriptor);
                        break;
                    case BindType.Payload:
                        payload.Add(descriptor);
                        break;
                    case BindType.After:
                        after.Add(descriptor);
                        break;
                    case BindType.Teardown:
                        teardown.Add(descriptor);
                        break;
                }

            }

            Comparison<IBindPointDescriptor> priorityComparison = 
                (x,y) => x.Priority.CompareTo(y.Priority);

            before.Sort(priorityComparison);
            after.Sort(priorityComparison);
            payload.Sort(priorityComparison);
            teardown.Sort(priorityComparison);

            before.AddRange(payload);
            before.AddRange(after);
            before.AddRange(teardown);

            DependencyGraph graph = new DependencyGraph(before);

            foreach (Resource resource in resources.Values)
            {
                resource.Validate();

                foreach (IBindPointDescriptor providingBindPoint in resource.Providers)
                {
                    foreach (IBindPointDescriptor consumer in resource.Dependents)
                        graph.AddEdge(consumer, providingBindPoint);
                    foreach (IBindPointDescriptor consumer in resource.RequiredBy)
                        graph.AddEdge(consumer, providingBindPoint);
                }
            }
            if (!graph.TopologicalSort())
            {
                engine.RaiseResourceLoop(string.Empty, before.Select(bpd => bpd.ControllerInfo));
            }


            var securityControllers = new List<IBindPointDescriptor>();

            int i = 0;
            while (i < before.Count)
                if (before[i].ControllerInfo.IsSecurity)
            {
                securityControllers.Add(before[i]);
                before.RemoveAt(i);
            }
            else
                i++;

            // we can't just sort, because the standard sort may re-arrange the existing order.
            // we just want to move all security controllers to the top of the chain
            before.InsertRange(0, securityControllers);


            bindPointsList = before;
        }


        #region private members
        private Engine engine;
        private Dictionary<string, Resource> resources;


        /// <summary>
        /// List of bindings involved in this Method
        /// </summary>
        private List<GenBinding> bindingsList;

        /// <summary>
        /// BindPoints list associated with bindings.
        /// </summary>
        private List<IBindPointDescriptor> bindPointsList;

        #endregion

        #region internal members

        internal List<GenBinding> BindingsList
        {
            get { return bindingsList; }
        }

        internal List<IBindPointDescriptor> BindPointsList
        {
            get { return bindPointsList; }
        }

        /// <summary>
        /// This method is called for each binding and returns newly-constructed SubSet, consists of matching groups
        /// </summary>
        /// <param name="newBinding"></param>
        /// <returns></returns>
        internal MethodUrlsSubset ApplyBinding(GenBinding newBinding)
        {

            if (newBinding.MatchWithSubset(this))
            {
                return new MethodUrlsSubset(engine,bindingsList, newBinding);
            }

            return null;
        }




        #endregion

    }
}
