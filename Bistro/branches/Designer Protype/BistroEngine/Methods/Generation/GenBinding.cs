using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bistro.Engine.Methods.Generation
{
    internal class GenBinding
    {
        internal GenBinding(string _url, string _controller)
        {
            initialUrl = _url;
            initialController = _controller;

            items = GetSplitItems(initialUrl);

        }


        #region Private members
        private static Regex splitRegex = new Regex(@"/\?/|\?/|/\?", RegexOptions.Compiled);
        private static Regex subSplitRegex = new Regex(@"/", RegexOptions.Compiled);
        private static Regex wildCardRegex = new Regex(@"\A(?:\*|{[^}]+})\z", RegexOptions.Compiled);


        private List<List<string>> items;
        private string initialUrl;

        private string initialController;

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



        #region Public properties
        internal string InitialUrl
        {
            get { return initialUrl; }
            set { initialUrl = value; }
        }
        #endregion

    }
}
