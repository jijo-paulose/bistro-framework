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
using Bistro.Configuration.Logging;

namespace Bistro.MethodsEngine.Subsets
{
    /// <summary>
    /// Class, representing a part of the url field. It contains one of two GenBindings for every binding in the engine.
    /// Hence, there's information on how this urlsubset relates to the bindings.
    /// </summary>
    public class MethodUrlsSubset
    {

        enum Messages
        {
            [DefaultMessage("ControllersList sorted \r\n{0}\r\n---------------------\r\n")]
            ListSorted
        }


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
            
            bindingsList = new List<GenBinding>(oldBindingsList);
            bindingsList.Add(newBinding);



//            UpdateBindPoints();
        }


        /// <summary>
        /// Updates bind point information
        /// </summary>
        internal void UpdateBindPoints()
        {
            bindPointsList = new List<IMethodsBindPointDesc>();
            foreach (GenBinding binding in bindingsList.Where(bind => bind.MatchStatus))
            {
                foreach (IMethodsBindPointDesc bindPointInfo in engine.GetTypesByBinding(binding))
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
                foreach (string resName in bindPoint.Controller.Provides)
                {
                    Resource res = AddResource(resName);
                    res.AddProvider(bindPoint);
                }
                foreach (string resName in bindPoint.Controller.DependsOn)
                {
                    Resource res = AddResource(resName);
                    res.AddDependents(bindPoint);
                }
                foreach (string resName in bindPoint.Controller.Requires)
                {
                    Resource res = AddResource(resName);
                    res.AddRequiredBy(bindPoint);
                }
            }
        }

        /// <summary>
        /// Creates resource if it does not exist in the collection.
        /// </summary>
        /// <param name="resName">Resource name</param>
        /// <returns>Resource from the resources collection</returns>
        private Resource AddResource(string resName)
        {
            if (!resources.ContainsKey(resName))
            {
                resources.Add(resName, new Resource(engine, resName));
            }
            return resources[resName];
        }



        /// <summary>
        /// Method to set bindpoints order depending on the parameters, priority, etc...
        /// </summary>
        private void SortBindPoints()
        {


            DependencyGraph graph = new DependencyGraph(engine, bindPointsList);

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
            if (!graph.TopologicalSort(out bindPointsList))
            {
                engine.RaiseResourceLoop(string.Empty, bindPointsList.Select(bpd => bpd.Controller));
            }

            StringBuilder tempsb = bindPointsList.Aggregate(new StringBuilder(),(oldStr,bpd) => oldStr.Append(bpd.Controller.ControllerTypeName).Append(":").Append(bpd.Target).Append("\r\n"));
            

            engine.Logger.Report(Messages.ListSorted,tempsb.ToString());

            var securityControllers = new List<IMethodsBindPointDesc>();

            int i = 0;
            while (i < bindPointsList.Count)
                if (bindPointsList[i].Controller.IsSecurity)
            {
                securityControllers.Add(bindPointsList[i]);
                bindPointsList.RemoveAt(i);
            }
            else
                i++;

            // we can't just sort, because the standard sort may re-arrange the existing order.
            // we just want to move all security controllers to the top of the chain
            bindPointsList.InsertRange(0, securityControllers);


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
