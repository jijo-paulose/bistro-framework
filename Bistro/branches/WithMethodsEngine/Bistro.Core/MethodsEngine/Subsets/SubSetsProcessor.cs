using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Controllers.Descriptor;
using Bistro.MethodsEngine.Reflection;
using System.Text.RegularExpressions;

namespace Bistro.MethodsEngine.Subsets
{
    internal class SubSetsProcessor
    {
        internal SubSetsProcessor(Engine _engine)
        {
            engine = _engine;
        }

        #region Groups

        #region Private fields

        private Regex bindingParser = new Regex(@"^\s*(?'binding'(\w*\s*)((\?|/|(/(\*|(\w|-)+|\{\w+}|\?/((\w|-)+|\{\w+})))*(/\?)?)(?:\?(?:(?:\w|-|=)+|\{\w+})(?:\&(?:(?:\w|-|=)+|\{\w+}))*|)))\s*$", RegexOptions.Compiled | RegexOptions.Singleline);


        private Dictionary<BindVerb, List<IEnumerable<GenBinding>>> allBindings;
        private Dictionary<BindVerb, List<MethodUrlsSubset>> allMethods;

        private Engine engine;

        #endregion



        //public MethodUrlsSubset GetMethodByUrl(string url)
        //{
        //    var splittedUrl = smartUrlSplit(url);

        //    if (splittedUrl.Length == 0)
        //        throw new ApplicationException("Invalid (empty) url");








        //}







        //private void SortSubsets()
        //{
        //    foreach (var methodsList in allMethods)
        //    {
        //        foreach (var method in methodsList.Value)
        //        {
        //            method.AddParticipates();
        //        }
        //        allBindings[methodsList.Key].Sort((x, y) => x.First(bind => bind.MatchStatus).ParticipatesCount.CompareTo(y.First(bind => bind.MatchStatus).ParticipatesCount));
        //    }
        //}





        public void ProcessControllers(List<ITypeInfo> controllers)
        {

            allMethods = new Dictionary<BindVerb, List<MethodUrlsSubset>>();

            allBindings = new Dictionary<BindVerb, List<IEnumerable<GenBinding>>>();
            foreach (BindVerb verb in Enum.GetValues(typeof(BindVerb)))
            {
                allBindings.Add(verb, new List<IEnumerable<GenBinding>>());
            }

            controllers.ForEach(controller => CreateGenBindings(controller));

            StringBuilder sb = new StringBuilder();



            foreach (BindVerb verb in Enum.GetValues(typeof(BindVerb)))
            {
                List<MethodUrlsSubset> allGroups = CreateGroups(verb);
                allMethods.Add(verb, allGroups);

                //sb.AppendFormat("NEXT VERB!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!:{0}\r\n", verb.ToString());

                //foreach (MethodUrlsSubset subset in allGroups)
                //{
                //    sb.AppendLine("New subset!!!!!!!!!!!!!!!");
                //    foreach (var binding in subset.BindingsList)
                //    {
                //        sb.AppendFormat("MatchStatus:{0}   Url:{1}\r\n", binding.MatchStatus, binding.InitialUrl);
                //    }
                //}

            }
            //SortSubsets();
            //string result = sb.ToString();


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




        private List<MethodUrlsSubset> CreateGroups(BindVerb verb)
        {
            MethodUrlsSubset firstGroup = new MethodUrlsSubset();
            List<MethodUrlsSubset> allGroups = new List<MethodUrlsSubset>();

            List<MethodUrlsSubset> newBindingGroups;
            allGroups.Add(firstGroup);
            List<IEnumerable<GenBinding>> bindingsForVerb = allBindings[verb];
            foreach (IEnumerable<GenBinding> bindingsPair in bindingsForVerb)
            {

                newBindingGroups = new List<MethodUrlsSubset>();

                foreach (MethodUrlsSubset group in allGroups)
                {
                    foreach (GenBinding binding in bindingsPair)
                    {
                        MethodUrlsSubset newGroup = group.ApplyBinding(binding);
                        if (newGroup != null)
                            newBindingGroups.Add(newGroup);
                    }
                }

                allGroups = newBindingGroups;

            }

            return allGroups;

        }



        private void CreateGenBindings(ITypeInfo classInfo)
        {

            List<string> bindings = new List<string>();
            foreach (IAttributeInfo attribute in classInfo.Attributes)
            {
                if (attribute.Type == typeof(BindAttribute).FullName && attribute.Parameters.Count > 0)
                {
                    bindings.Add(attribute.Parameters[0].AsString());
                }
            }

            foreach (string item in bindings)
            {
                Match match = bindingParser.Match(item);
                if (match.Success)
                {
                    string methodUrl = match.Groups["binding"].Captures[0].Value;
                    string verb;
                    switch (methodUrl[0])
                    {
                        case '?':
                        case '/':
                            verb = "*";
                            break;
                        default:
                            verb = methodUrl.Substring(0, methodUrl.IndexOfAny(new char[] { ' ', '/', '?' }));
                            methodUrl = methodUrl.Substring(methodUrl.IndexOfAny(new char[] { '/', '?' }));
                            break;

                    }

                    if (verb == "*")
                    {
                        foreach (BindVerb bindVerb in Enum.GetValues(typeof(BindVerb)))
                        {
                            AddNewBinding(bindVerb, methodUrl, classInfo);
                        }
                    }
                    else
                    {
                        foreach (BindVerb bindVerb in Enum.GetValues(typeof(BindVerb)))
                        {
                            if (verb.ToUpper() == bindVerb.ToString().ToUpper())
                            {
                                AddNewBinding(bindVerb, methodUrl, classInfo);
                            }
                        }
                    }

                }
                else
                {
                    engine.RaiseInvalidBinding(null, item);
                }
            }



        }


        private void AddNewBinding(BindVerb verb, string bindUrl, ITypeInfo classInfo)
        {
            allBindings[verb].Add(new GenBinding[2] { new GenBinding(bindUrl, classInfo.FullName, true, verb), new GenBinding(bindUrl, classInfo.FullName, false, verb) });
        }




        #endregion

        #endregion





    }
}
