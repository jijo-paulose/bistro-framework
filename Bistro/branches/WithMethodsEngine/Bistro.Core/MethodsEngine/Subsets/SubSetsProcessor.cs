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
using Bistro.Controllers.Descriptor;
using Bistro.MethodsEngine.Reflection;
using System.Text.RegularExpressions;
using Bistro.Configuration.Logging;

namespace Bistro.MethodsEngine.Subsets
{

    /// <summary>
    /// This class is responsible for handling of all the url subsets and bindings registered in the engine.
    /// </summary>
    internal class SubSetsProcessor
    {


        enum Errors
        {
            [DefaultMessage("Error processing link - method not found: {0}")]
            ErrorMethodNotFound
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="SubSetsProcessor"/> class.
        /// </summary>
        /// <param name="_engine">The engine.</param>
        internal SubSetsProcessor(Engine _engine)
        {
            engine = _engine;
            allMethods = new List<MethodUrlsSubset>();
            allBindings = new List<GenBindingTuple>();
            // We need to have at least one empty method. Otherwise it will be impossible to create new non-empty methods.
            allMethods.Add(new MethodUrlsSubset(_engine));


        }

        #region Private fields


        /// <summary>
        /// list to store all the binding tuples(positive/negative).
        /// </summary>
        private List<GenBindingTuple> allBindings;

        /// <summary>
        /// List to store all the methods.
        /// </summary>
        private List<MethodUrlsSubset> allMethods;

        /// <summary>
        /// Link to the engine.
        /// </summary>
        private Engine engine;

        #endregion




        /// <summary>
        /// Creates new binding tuple from the binding URL and executes new level of methods creation to replace old methods list.
        /// </summary>
        /// <param name="verbNormalizedUrl">The verb normalized URL.</param>
        public void AddNewBinding(string verbNormalizedUrl)
        {
            allBindings.Add(new GenBindingTuple(verbNormalizedUrl,engine));
            allMethods = CreateNewMethodsLevel();

        }


        /// <summary>
        /// Searches and gets the method by an URL.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <returns></returns>
        internal MethodUrlsSubset GetMethodByUrl(string requestUrl)
        {
            // Compare with each Binding
            Dictionary<GenBinding,GenBinding> bindingsToSearch = new Dictionary<GenBinding,GenBinding>();
            foreach (GenBindingTuple tuple in allBindings)
            {
                GenBinding tempBind = tuple.TryMatchUrl(requestUrl);
                bindingsToSearch.Add(tempBind,tempBind);
            }


            // Compare result with each method
            foreach (MethodUrlsSubset subset in allMethods)
            {
                bool notFound = false;
                foreach (GenBinding bind in subset.BindingsList)
                {
                    if (bindingsToSearch.ContainsKey(bind))
                        continue;
                    notFound = true;
                }
                if (notFound)
                    continue;
                return subset;
            }
            engine.Logger.Report(Errors.ErrorMethodNotFound, requestUrl);
            //throw new ApplicationException("Method not found - see log for details");
            return null;
        }


        #region Private methods

        /// <summary>
        /// Normalizes the url and splits it by slashes, not presenting a blank element if the 
        /// url begins with a slash
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private string[] smartUrlSplit(string url)
        {
            // trim any excess whitespace, and also the leading /
            string workingCopy = url.Trim().TrimStart('/');

            return BindPointUtilities.GetBindComponents(url);
        }



        /// <summary>
        /// Creates a new methods level for each unprocessed GenBindingTuple iteratively.
        /// </summary>
        /// <returns>Methods list after the last iteration.</returns>
        private List<MethodUrlsSubset> CreateNewMethodsLevel()
        {
            List<MethodUrlsSubset> newBindingMethods = allMethods;
            List<MethodUrlsSubset> oldBindingMethods;
            foreach (GenBindingTuple tuple in allBindings.Where(tpl => !tpl.Processed))
            {
                oldBindingMethods = newBindingMethods;
                newBindingMethods = new List<MethodUrlsSubset>();
                
                foreach (MethodUrlsSubset group in allMethods)
                {
                    MethodUrlsSubset newGroupTrue = group.ApplyBinding(tuple.PositiveBind);
                    MethodUrlsSubset newGroupFalse = group.ApplyBinding(tuple.NegativeBind);
                    tuple.MarkProcessed();

                    if (newGroupTrue != null)
                        newBindingMethods.Add(newGroupTrue);
                    if (newGroupFalse != null)
                        newBindingMethods.Add(newGroupFalse);
                }
            }

            return newBindingMethods;
        }


        #endregion

    }
}
