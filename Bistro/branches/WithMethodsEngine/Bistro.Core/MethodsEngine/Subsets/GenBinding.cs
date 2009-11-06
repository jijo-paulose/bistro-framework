using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bistro.Controllers.Descriptor;
using Bistro.Configuration.Logging;

namespace Bistro.MethodsEngine.Subsets
{
    internal class GenBindingTuple
    {
        internal GenBindingTuple(string _verbNormalizedUrl,Engine _engine)
        {
            PositiveBind = new GenBinding(_verbNormalizedUrl, true, _engine);
            NegativeBind = new GenBinding(_verbNormalizedUrl, false, _engine);
            Processed = false;

            engine = _engine;
        }

        internal GenBinding TryMatchUrl(string url)
        {
            return PositiveBind.TryMatchUrl(url) ? PositiveBind : NegativeBind;
        }


        internal void MarkProcessed () 
        {
            Processed = true;
        }

        private Engine engine;

        internal bool Processed { get; private set; }
        internal GenBinding PositiveBind { get; set; }
        internal GenBinding NegativeBind { get; set; }
    }


    internal class GenBinding
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_verbNormalizedUrl">binding in the form VERB/URL</param>
        /// <param name="_matchStatus"></param>
        internal GenBinding(string _verbNormalizedUrl, bool _matchStatus, Engine _engine)
        {
            engine = _engine;
            initialUrl = _verbNormalizedUrl;
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

        private int totalLength = 0;
        private int lengthWithoutEndParams = 0;

        private Engine engine;

//        private BindVerb verb;



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
                totalLength = totalLength + splitItems.Count();
                tempItems.Add(new List<string>(splitItems));
            }
            lengthWithoutEndParams = totalLength;
            bool brk = false;
            for (int i = (tempItems.Count - 1); i >= 0; i--)
            {
                for (int j = tempItems[i].Count - 1; j >= 0; j--)
                {
                    if (wildCardRegex.IsMatch(tempItems[i][j]))
                        lengthWithoutEndParams--;
                    else
                    {
                        brk = true;
                        break;
                    }
                }
                if (brk)
                    break;
            }
            return tempItems;
        }


        #endregion

        enum Errors
        {
            [DefaultMessage("Error splitting incoming url: {0}")]
            ErrorSplittingUrl
        }

        enum Messages
        {
            [DefaultMessage("Matching url: {0}")]
            MessageMatchingUrl
        }


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


        #region Participates
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

        #endregion


        internal bool TryMatchUrl(string requestUrl)
        {
            engine.Logger.Report(Messages.MessageMatchingUrl, requestUrl);

            var requestItems = GetSplitItems(initialUrl);

            //if (requestItems.Count != 1)
            //{
            //    engine.Logger.Report(Errors.ErrorSplittingUrl, requestUrl);
            //    return false;
            //}


            string[] splitQueryString = requestUrl.Split('?');
            string[] requestComponents = smartUrlSplit(splitQueryString[0]);

            // if there are more bind components than there are url components, we don't have a match.
            if (requestComponents.Length < lengthWithoutEndParams)
                return false;

            List<string> firstPart = items[0];

            bool firstItemMatchImpossible = false;
            for (int i = 0; i < firstPart.Count; i++)
            {
                if ((wildCardRegex.IsMatch(firstPart[i])) || (firstPart[i] == requestComponents[i]))
                    continue;

                firstItemMatchImpossible = true;
                break;
            }

            if (firstItemMatchImpossible)
                return false;

            int positionInMatchPart = firstPart.Count;

            var currentMatchPartEnum = items.GetEnumerator();
            currentMatchPartEnum.MoveNext();

            List<string> currentMatchPart = (currentMatchPartEnum.MoveNext()) ? currentMatchPartEnum.Current : null;

            while (currentMatchPart != null)
            {
                if ((positionInMatchPart + currentMatchPart.Count) <= requestComponents.Length)
                {
                    bool placed = true;
                    for (int i = 0; i < currentMatchPart.Count; i++)
                    {
                        if (wildCardRegex.IsMatch(currentMatchPart[i]))
                            continue;
                        if (currentMatchPart[i] == requestComponents[i + positionInMatchPart])
                            continue;
                        placed = false;
                        break;
                    }

                    if (placed)
                    {
                        positionInMatchPart = positionInMatchPart + currentMatchPart.Count;
                        currentMatchPartEnum.MoveNext();
                        currentMatchPart = currentMatchPartEnum.Current;
                    }
                    else 
                        positionInMatchPart++;
                }
                else
                {
                    return false;
                }
            }
            return true;
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


        #region from the old ControllerDispatcher
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
        #endregion


        #endregion



    }
}

