using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bistro.Engine.Methods.Generation
{
    internal class GenBinding
    {
        internal GenBinding(string _url, string _controller, bool _matchStatus)
        {
            initialUrl = _url;
            initialController = _controller;
            matchStatus = _matchStatus;

            items = GetSplitItems(initialUrl);

        }


        #region Private members
        private static Regex splitRegex = new Regex(@"/\?/|\?/|/\?", RegexOptions.Compiled);
        private static Regex subSplitRegex = new Regex(@"/", RegexOptions.Compiled);
        private static Regex wildCardRegex = new Regex(@"\A(?:\*|{[^}]+})\z", RegexOptions.Compiled);


        private List<List<string>> items;
        private string initialUrl;

        private string initialController;

        private bool matchStatus;

        #endregion

        #region private methods

        private List<List<string>> GetSplitItems(string splitUrl)
        {
            var preSplitItems = splitRegex.Split(splitUrl);
            var tempItems = new List<List<string>>(preSplitItems.Length);
            foreach (string preSplitItem in preSplitItems)
            {
                var splitItems = subSplitRegex.Split(preSplitItem).Where(inputStr => inputStr != string.Empty);
                tempItems.Add(new List<string>(splitItems));
            }
            return tempItems;
        }


        #endregion



        #region Internal properties
        internal string InitialUrl
        {
            get { return initialUrl; }
            set { initialUrl = value; }
        }

        internal bool MatchStatus
        {
            get { return matchStatus; }
        }

        #endregion

        #region Internal methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodUrlsSubset"></param>
        /// <returns></returns>
        internal bool MatchWithSubSet(MethodUrlsSubset methodUrlsSubset)
        {
            #region create common url with all matched bindings.

            List<string> commonBinding = new List<string>();

            List<GenBinding> newList = new List<GenBinding>(methodUrlsSubset.BindingsList);
            newList.Add(this);

            IEnumerable<GenBinding> matchBindings = newList.Where(binding => binding.MatchStatus);
            IEnumerable<GenBinding> noMatchBindings = newList.Where(binding => !binding.MatchStatus);
            foreach (GenBinding binding in matchBindings)
            {
                List<string> firstPart = binding.items.Count == 0 ? new List<string>() : new List<string>(binding.items[0]);
                #region equalization
                int difference = Math.Abs(commonBinding.Count - firstPart.Count);

                if (commonBinding.Count > firstPart.Count)
                {
                    for (int i = 0; i < difference; i++)
                        firstPart.Add("*");
                }
                else
                {
                    for (int i = 0; i < difference; i++)
                        commonBinding.Add("*");
                }

                #endregion

                for (int i = 0; i < commonBinding.Count; i++)
                {
                    if ((wildCardRegex.IsMatch(firstPart[i])) || (commonBinding[i] == firstPart[i]))
                        continue;

                    if (wildCardRegex.IsMatch(commonBinding[i]))
                    {
                        commonBinding[i] = firstPart[i];
                        continue;
                    }

                    return false;
                }


            }
            #endregion



            #region create boolean vector 

            List<bool> excludeList = new List<bool>(commonBinding.Count);
            for (int i = 0; i < commonBinding.Count; i++)
                excludeList.Add(false);

            #endregion


            #region exclude items based on nomatch bindings
            foreach (GenBinding binding in noMatchBindings)
            {
                List<string> firstPart = binding.items.Count == 0 ? new List<string>() : new List<string>(binding.items[0]);
                #region equalization
                int difference = Math.Abs(commonBinding.Count - firstPart.Count);

                if (commonBinding.Count > firstPart.Count)
                    for (int i = 0; i < difference; i++)
                        firstPart.Add("*");
                else
                {
                    for (int i = 0; i < difference; i++)
                    {
                        commonBinding.Add("*");
                        excludeList.Add(false);
                    }
                }

                #endregion

                bool processExclusion = true;

                for (int i = 0; i < commonBinding.Count; i++)
                {
                    if ((wildCardRegex.IsMatch(firstPart[i])) || wildCardRegex.IsMatch(commonBinding[i]) || (commonBinding[i] == firstPart[i]))
                        continue;

                    processExclusion = false;
                    break;
                }

                if (processExclusion)
                {
                    for (int i = 0; i < excludeList.Count; i++)
                    {
                        if (wildCardRegex.IsMatch(firstPart[i]) || (firstPart[i] == commonBinding[i]))
                            excludeList[i] = true;

                    }

                }
            }
            #endregion

            #region check for exclusion of all matches
            if (excludeList.TrueForAll(exclude => exclude))
                return false;

            #endregion

            return true;
        }



        #endregion



    }
}
