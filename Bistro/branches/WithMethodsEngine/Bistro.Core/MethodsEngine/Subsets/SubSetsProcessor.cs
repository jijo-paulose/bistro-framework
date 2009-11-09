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

    internal class SubSetsProcessor
    {


        enum Errors
        {
            [DefaultMessage("Error processing link - method not found: {0}")]
            ErrorMethodNotFound
        }



        internal SubSetsProcessor(Engine _engine)
        {
            engine = _engine;
            allMethods = new List<MethodUrlsSubset>();
            allBindings = new List<GenBindingTuple>();
            // We need to have at least one empty method. Otherwise it's impossible to create new non-empty methods.
            allMethods.Add(new MethodUrlsSubset(_engine));


        }

        #region Groups

        #region Private fields



        private Regex bindingParser = new Regex(@"^\s*(?'binding'(\w*\s*)((\?|/|(/(\*|(\w|-)+|\{\w+}|\?/((\w|-)+|\{\w+})))*(/\?)?)(?:\?(?:(?:\w|-|=)+|\{\w+})(?:\&(?:(?:\w|-|=)+|\{\w+}))*|)))\s*$", RegexOptions.Compiled | RegexOptions.Singleline);


        private List<GenBindingTuple> allBindings;
        private List<MethodUrlsSubset> allMethods;

        private Engine engine;

        #endregion






        public void AddNewBinding(string verbNormalizedUrl)
        {
            allBindings.Add(new GenBindingTuple(verbNormalizedUrl,engine));
            allMethods = CreateNewMethodsLevel();

        }


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
//            allMethods = newBindingMethods;
            return newBindingMethods;
        }




        #endregion

        #endregion


    }
}
