using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Engine.Methods.Generation
{
    internal enum MatchResult 
    {
        MatchOption,
        NoMatchOption,
        BothOptions
    }
    
    internal class GroupMatcher
    {
        private GroupMatcher()
        {
        }

        #region Singleton
        private static GroupMatcher instance;

        internal static GroupMatcher Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GroupMatcher();
                }
                return instance;
            }
        }
        #endregion

        internal MatchResult Match(MethodUrlsSubset group, GenBinding newBinding)
        {
            #region MatchPart



            #endregion


            #region NoMatchPart



            #endregion


            return MatchResult.BothOptions;

            
        }


    }
}
