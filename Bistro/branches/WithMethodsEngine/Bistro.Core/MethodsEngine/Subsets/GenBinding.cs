using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bistro.Controllers.Descriptor;

namespace Bistro.MethodsEngine.Subsets
{

    internal class GenBinding
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_url"></param> -> bindingString
        /// <param name="_controller"></param>
        /// <param name="_matchStatus"></param>
        /// <param name="_verb"></param>
        internal GenBinding(string _url, string _controller, bool _matchStatus, BindVerb _verb)
        {
            initialUrl = _url;
            initialController = _controller;
            matchStatus = _matchStatus;

            items = GetSplitItems(initialUrl);

            participatesIn = new List<MethodUrlsSubset>();
        }


        #region Private members
        private static Regex splitRegex = new Regex(@"/\?/|\?/|/\?", RegexOptions.Compiled);
        private static Regex subSplitRegex = new Regex(@"/", RegexOptions.Compiled);
        private static Regex wildCardRegex = new Regex(@"\A(?:\*|{[^}]+})\z", RegexOptions.Compiled);

        private List<MethodUrlsSubset> participatesIn;


        private List<List<string>> items;
        private string initialUrl;

        private BindVerb verb;



        private string initialController;

        private bool matchStatus;

        #endregion

        #region private methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="splitUrl"></param>
        /// <returns></returns>
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
        internal BindVerb Verb
        {
            get { return verb; }
            set { verb = value; }
        }
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

        internal void AddParticipant(MethodUrlsSubset newParticipant)
        {
            participatesIn.Add(newParticipant);
        }

        internal int ParticipatesCount
        {
            get
            {
                return participatesIn.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodUrlsSubset"></param>
        /// <returns></returns>
        /// 
        internal bool MatchWithSubset(MethodUrlsSubset methodUrlsSubset)
        {
            List<GenBinding> newList = new List<GenBinding>(methodUrlsSubset.BindingsList);
            IEnumerable<GenBinding> matchBindings = newList.Where(binding => binding.MatchStatus);
            IEnumerable<GenBinding> noMatchBindings = newList.Where(binding => !binding.MatchStatus);

            if (this.MatchStatus)
            {
                foreach (GenBinding binding in matchBindings)
                {
                    if (!CompareWithMatch(binding))
                        return false;
                }

                foreach (GenBinding binding in noMatchBindings)
                {
                    if (!CompareMatchAndNoMatch(this, binding))
                        return false;
                }

            }
            else
            {
                foreach (GenBinding binding in matchBindings)
                {
                    if (!CompareMatchAndNoMatch(binding, this))
                        return false;
                }

            }

            return true;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matchBind"></param>
        /// <returns></returns>
        private bool CompareWithMatch(GenBinding matchBind)
        {
            if (!matchBind.MatchStatus || !this.MatchStatus)
                throw new ApplicationException("improper usage of CompareWithMatch");

            List<string> firstPartOfThis = this.items.Count == 0 ? new List<string>() : new List<string>(this.items[0]);

            List<string> firstPart = matchBind.items.Count == 0 ? new List<string>() : new List<string>(matchBind.items[0]);
            #region Check for different beginning

            int smallestSize = firstPartOfThis.Count < firstPart.Count ? firstPartOfThis.Count : firstPart.Count;


            for (int i = 0; i < smallestSize; i++)
            {
                if ((wildCardRegex.IsMatch(firstPart[i])) || (wildCardRegex.IsMatch(firstPartOfThis[i])) || (firstPartOfThis[i] == firstPart[i]))
                    continue;
                return false;
            }

            #endregion
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matchBind"></param>
        /// <param name="noMatchBind"></param>
        /// <returns></returns>
        private static bool CompareMatchAndNoMatch(GenBinding matchBind, GenBinding noMatchBind)
        {
            if (!matchBind.MatchStatus && noMatchBind.MatchStatus)
                throw new ApplicationException("improper usage of CompareMatchAndNoMatch");

            List<string> firstPart = matchBind.items.Count == 0 ? new List<string>() : new List<string>(matchBind.items[0]);

            List<string> firstPartOfThis = noMatchBind.items.Count == 0 ? new List<string>() : new List<string>(noMatchBind.items[0]);

            #region Get the start point firstPartOfThis - B; firstPart - A
            if (firstPartOfThis.Count > firstPart.Count)
                return true;

            bool firstItemMatchImpossible = false;
            for (int i = 0; i < firstPartOfThis.Count; i++)
            {
                if ((wildCardRegex.IsMatch(firstPartOfThis[i])) || (firstPart[i] == firstPartOfThis[i]))
                    continue;

                firstItemMatchImpossible = true;
                break;
            }

            if (firstItemMatchImpossible)
                return true;

            var currentNoMatchPartEnum = noMatchBind.items.GetEnumerator();
            currentNoMatchPartEnum.MoveNext();
            var currentMatchPartEnum = matchBind.items.GetEnumerator();

            int positionInMatchPart = (currentNoMatchPartEnum.Current == null) ? 0 : currentNoMatchPartEnum.Current.Count;

            List<string> currentNoMatchPart = (currentNoMatchPartEnum.MoveNext()) ? currentNoMatchPartEnum.Current : null;
            List<string> currentMatchPart = (currentMatchPartEnum.MoveNext()) ? currentMatchPartEnum.Current : null;



            #endregion

            while ((currentNoMatchPart != null) && (currentMatchPart != null))
            {
                //try to place it.
                if (positionInMatchPart + currentNoMatchPart.Count <= currentMatchPart.Count)
                {
                    bool placed = true;
                    for (int i = 0; i < currentNoMatchPart.Count; i++)
                    {
                        if (wildCardRegex.IsMatch(currentNoMatchPart[i]))
                            continue;
                        if (currentNoMatchPart[i] == currentMatchPart[i + positionInMatchPart])
                            continue;

                        placed = false;
                        break;
                    }

                    if (placed)
                    {
                        positionInMatchPart = positionInMatchPart + currentNoMatchPart.Count;
                        currentNoMatchPartEnum.MoveNext();
                        currentNoMatchPart = currentNoMatchPartEnum.Current;
                    }
                    else
                        positionInMatchPart++;

                }
                else
                {
                    currentMatchPartEnum.MoveNext();
                    currentMatchPart = currentMatchPartEnum.Current;
                    positionInMatchPart = 0;
                }

            }

            if ((currentNoMatchPart == null) && (currentMatchPart != null))
            {
                // noMatch binding completely matches with one of the match bindings.
                return false;
            }



            return true;

        }


        #endregion



    }
}

