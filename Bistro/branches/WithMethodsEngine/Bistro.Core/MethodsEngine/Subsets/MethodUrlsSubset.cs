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
using Bistro.MethodsEngine.Reflection;
using Bistro.Controllers.Descriptor;

namespace Bistro.MethodsEngine.Subsets
{
    /// <summary>
    /// Class, representing a part of the url field. It contains one of two GenBindings for every binding in the engine.
    /// Hence, there's information on how this urlsubset relates to the bindings.
    /// </summary>
    public class MethodUrlsSubset
    {

        /// <summary>
        /// Initializes a new empty instance of the <see cref="MethodUrlsSubset"/> class.
        /// </summary>
        /// <param name="_engine">The engine.</param>
        internal MethodUrlsSubset(Engine _engine)
        {
            bindPointsList = new List<IMethodsBindPointDesc>();
            bindingsList = new List<GenBinding>();
            engine = _engine;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodUrlsSubset"/> class. 
        /// Copies contents of the old bindings list and adds new to the new list GenBinding.
        /// </summary>
        /// <param name="_engine">The engine.</param>
        /// <param name="oldBindingsList">Old bindings list.</param>
        /// <param name="newBinding">The new binding.</param>
        private MethodUrlsSubset(Engine _engine, List<GenBinding> oldBindingsList, GenBinding newBinding)
        {
            engine = _engine;
            bindPointsList = new List<IMethodsBindPointDesc>();
            bindingsList = new List<GenBinding>(oldBindingsList);
            bindingsList.Add(newBinding);

            foreach (GenBinding binding in bindingsList.Where(bind => bind.MatchStatus))
            {
                foreach (IMethodsBindPointDesc bindPointInfo in binding.GetBindPoints())
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

        /// <summary>
        /// Scans resources and creates a new dictionary
        /// </summary>
        private void ScanResources()
        {
            resources = new Dictionary<string,Resource>();
            foreach (IMethodsBindPointDesc bindPoint in bindPointsList)
            {
                foreach (string resName in bindPoint.ControllerMethodDesc.Provides
                                        .Concat(bindPoint.ControllerMethodDesc.Requires)
                                        .Concat(bindPoint.ControllerMethodDesc.DependsOn))
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
            Func<List<IMethodsBindPointDesc>> init = () => new List<IMethodsBindPointDesc>();
            var before = init();
            var payload = init();
            var after = init();
            var teardown = init();

            foreach (IMethodsBindPointDesc descriptor in bindPointsList)
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

            Comparison<IMethodsBindPointDesc> priorityComparison = 
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

                foreach (IMethodsBindPointDesc providingBindPoint in resource.Providers)
                {
                    foreach (IMethodsBindPointDesc consumer in resource.Dependents)
                        graph.AddEdge(consumer, providingBindPoint);
                    foreach (IMethodsBindPointDesc consumer in resource.RequiredBy)
                        graph.AddEdge(consumer, providingBindPoint);
                }
            }
            if (!graph.TopologicalSort())
            {
                engine.RaiseResourceLoop(string.Empty, before.Select(bpd => bpd.ControllerMethodDesc));
            }


            var securityControllers = new List<IMethodsBindPointDesc>();

            int i = 0;
            while (i < before.Count)
                if (before[i].ControllerMethodDesc.IsSecurity)
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
        /// <summary>
        /// Engine stored here.
        /// </summary>
        private Engine engine;

        /// <summary>
        /// Dictionary of resources
        /// </summary>
        private Dictionary<string, Resource> resources;


        /// <summary>
        /// List of bindings involved in this method subset.
        /// </summary>
        private List<GenBinding> bindingsList;

        /// <summary>
        /// BindPoints list associated with bindings.
        /// </summary>
        private List<IMethodsBindPointDesc> bindPointsList;

        #endregion

        #region internal members

        /// <summary>
        /// Gets the bindings list.
        /// </summary>
        /// <value>The bindings list.</value>
        internal List<GenBinding> BindingsList
        {
            get { return bindingsList; }
        }

        /// <summary>
        /// Gets the bind points list.
        /// </summary>
        /// <value>The bind points list.</value>
        internal List<IMethodsBindPointDesc> BindPointsList
        {
            get { return bindPointsList; }
        }


        /// <summary>
        /// This method is called for each binding and returns newly-constructed SubSet, consisting of matching/not matching GenBindings.
        /// </summary>
        /// <param name="newBinding">new binding</param>
        /// <returns>newly-created method.</returns>
        internal MethodUrlsSubset ApplyBinding(GenBinding newBinding)
        {

            if (newBinding.MatchWithSubset(this))
            {
                return new MethodUrlsSubset(engine, bindingsList, newBinding);
            }

            return null;
        }


        #endregion
    }
}
