using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace Bistro.Engine.Methods.Generation
{
    internal class PatternPartInfo
    {
        internal PatternPartInfo(int _thread, int _itemNum, List<string> _partList)
        {
            thread = _thread;
            itemNum = _itemNum;
            partList = _partList;
        }
        private int thread;
        private int itemNum;
        private List<string> partList;

        internal int Thread { get { return thread; } }
        internal int ItemNum { get { return itemNum; } }
        internal List<string> PartList { get { return partList; } }
    }


    internal class Pattern
    {
        #region Constructor
        internal Pattern(string incomingUrl, string initialController)
        {

            initialUrl = incomingUrl;
            controllers = new List<string>();
            controllers.Add(initialController);
            items1 = new List<List<string>>();
            items2 = new List<List<string>>();

        }

        private Pattern(string url1, string url2,string initialController)
        {
            controllers = new List<string>();

            items1 = GetSplitItems(url1);
            items2 = GetSplitItems(url2);

            IEnumerable<List<PatternPartInfo>> allPermutations = Process(-1, -1, new List<PatternPartInfo>());
            foreach (var patternsList in allPermutations)
            {
                //List<string> patterns = GenerateAllUrls(allPermutations);
            }
            
        }

        private List<string> GenerateAllUrls(List<PatternPartInfo> patternsList)
        {
            return null;
        }


        private List<string> GenerateStepInto(List<PatternPartInfo> patternsList,int oldPrevThreadItem,int nextItem, int startPoint, List<string> mergedItems)
        {
//            List<string> newMergedItems = PutNextItem(patternsList, nextItem, startPoint, mergedItems);
            List<string> newMergedItems = new List<string>(mergedItems);
            List<string> nextItemList = patternsList[nextItem].PartList;

            bool swapOnNextTurn = (startPoint + nextItemList.Count > newMergedItems.Count);


            for (int i = startPoint; i < startPoint + nextItemList.Count; i++)
            {
                if (i > newMergedItems.Count)
                {
                    newMergedItems.Add(nextItemList[i - startPoint]);
                }
                else
                {
                    if (wildCardRegex.IsMatch(newMergedItems[i]))
                    {
                        newMergedItems[i] = nextItemList[i - startPoint];
                    }
                }
            }

            if (swapOnNextTurn)
            {

            }
            else
            {

            }
            return null;











        }


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

        #region private methods

        private IEnumerable<List<PatternPartInfo>> Process(int lItem1Level0, int lItem2Level0, List<PatternPartInfo> mergedItems)
        {
            List<List<PatternPartInfo>> retList = new List<List<PatternPartInfo>>();


            bool finish = true;
            if (lItem1Level0 < (items1.Count - 1))
            {
                int tmpLItem1Level0 = lItem1Level0+1;
                List<string> nextItem = items1[tmpLItem1Level0];
                PatternPartInfo patternInfo = new PatternPartInfo(0, tmpLItem1Level0, nextItem);

                List<PatternPartInfo> newMergedItems = new List<PatternPartInfo>(mergedItems);
                newMergedItems.Add(patternInfo);
                retList.AddRange(Process(tmpLItem1Level0, lItem2Level0, newMergedItems));

                //                var generatedItems = ShiftGenerate(lItem1Level0,lItem1Level1,nextItem, mergedItems);
                finish = false;
            }

            if (lItem2Level0 < (items2.Count - 1))
            {
                int tmpLItem2Level0 = lItem2Level0 + 1;
                List<string> nextItem = items2[tmpLItem2Level0];
                PatternPartInfo patternInfo = new PatternPartInfo(1, tmpLItem2Level0, nextItem);

                List<PatternPartInfo> newMergedItems = new List<PatternPartInfo>(mergedItems);
                newMergedItems.Add(patternInfo);
                retList.AddRange(Process(lItem1Level0, tmpLItem2Level0, newMergedItems));


                finish = false;
            }
            if (finish)
            {
                retList.Add(mergedItems);
            }


            return retList;
        }




        private IEnumerable<Pattern> ShiftGenerate(int lItem1Level0, int lItem1Level1,List<string> nextItem, List<List<string>> mergedItems)
        {
            bool completed = false;
            while (!completed)
            {
                List<List<string>> newMergedItems = new List<List<string>>();

                for (int i = 0; i < (lItem1Level0); i++)
                {
                    newMergedItems.Add(mergedItems[i]);
                }
                
                ///////////
                // for (outer)
                for (int j = lItem1Level1 + 1; j < mergedItems[lItem1Level0].Count; j++)
                {
                    if (IsCasePossible(lItem1Level0,j,nextItem,mergedItems))
                    {


                        TryCaseRecursive(lItem1Level0, j, nextItem, mergedItems, newMergedItems);


                    }




                }




            }
            return null;
        }
        /// <summary>
        /// This method tries to substitute all possible parts from the pattern to specifically placed new pattern.
        /// </summary>
        /// <param name="lItem1Level0"></param>
        /// <param name="j"></param>
        /// <param name="nextItem"></param>
        /// <param name="mergedItems"></param>
        private void TryCaseRecursive(int lItem1Level0, int startLevel1Fixed, List<string> nextItem, List<List<string>> mergedItems, List<List<string>> newMergedItems)
        {
            List<string> newItemsList = new List<string>();
            for (int k = 0; k < startLevel1Fixed; k++)
            {
                newItemsList.Add(mergedItems[lItem1Level0][k]);
            }
            for (int k = 0; k < nextItem.Count; k++)
            {
                newItemsList.Add(nextItem[k]);
            }

            newMergedItems.Add(newItemsList);// probably we'll have to change newItemsList in the code later, to get all possible combinations.




        }





        private bool IsCasePossible(int lItem1Level0, int startLevel1, List<string> nextItem, List<List<string>> mergedItems)
        {//
            for (int k = 0; k < nextItem.Count; k++)
            {
                if (lItem1Level0>=mergedItems.Count)
                    return true;
                if (mergedItems[lItem1Level0][startLevel1]!= nextItem[k])
                {
                    return false;
                }
                
                if (startLevel1>= mergedItems[lItem1Level0].Count)
                {
                    return true;
                    //startLevel1 = 0;
                    //lItem1Level0++;
                } else 
                {
                    startLevel1++;
                }

            }
            return true;
        }
        

        #endregion

        #region static methods

        internal static IEnumerable<Pattern> GetNewPatterns(Pattern pattern1, Pattern pattern2)
        {
            Pattern newPattern = new Pattern(pattern1.InitialUrl, pattern2.InitialUrl, string.Empty);

//            IEnumerable<Pattern> patternsProduced = newPattern.Process();

            return null;
        }


        #endregion

        #region Private members
        private static Regex splitRegex = new Regex(@"/\?/|\?/|/\?", RegexOptions.Compiled);
        private static Regex subSplitRegex = new Regex(@"/", RegexOptions.Compiled);
        private static Regex wildCardRegex = new Regex(@"\A(?:\*|{[^}]+})\z", RegexOptions.Compiled);


        private string initialUrl;
        private bool processed = false;

        
        private bool terminal = false;



        private List<string> controllers;
        private List<List<string>> items1;
        private List<List<string>> items2;

        private int lastPlace1 = 0;
        private int lastPlace2 = 0;

        private int lastItem1 = -1;
        private int lastItem2 = -1;

        #endregion


        #region Public properties
        public string InitialUrl 
        { 
            get { return initialUrl; } 
            set { initialUrl = value; } 
        }


        #endregion

    }

}
