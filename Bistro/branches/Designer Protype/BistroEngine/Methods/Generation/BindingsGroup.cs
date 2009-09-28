using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Methods;

namespace Bistro.Engine.Methods.Generation
{
    internal class BindingsGroup
    {
//        internal BindingsGroup(int newId, List<GenBinding> bindingsList)
        internal BindingsGroup( List<GenBinding> bindingsList)
        {
            allBindings = bindingsList;

//            groupId = newId;

            bindingsStatus = new List<bool>(bindingsList.Count);


        }


        #region private members
        private List<GenBinding> allBindings;

        //private int groupId;
        private List<bool> bindingsStatus;
        #endregion

        #region internal members
        internal BindingsGroup ForkWithNewBinding()
        {
            if (bindingsStatus.Count <= allBindings.Count)
            {
                GenBinding nextBinding = allBindings[bindingsStatus.Count];
                switch (GroupMatcher.Instance.Match(this, nextBinding))
                {
                    case MatchResult.MatchOption: 
                    {
                        bindingsStatus.Add(true);
                        break;
                    }
                    case MatchResult.NoMatchOption:
                    {
                        bindingsStatus.Add(false);
                        break;
                    }
                    case MatchResult.BothOptions:
                    {
                        BindingsGroup retBinding = new BindingsGroup(allBindings);
                        retBinding.bindingsStatus = new List<bool>(bindingsStatus);
                        bindingsStatus.Add(true);
                        retBinding.bindingsStatus.Add(false);
                        return retBinding;
                        break;
                    }
                }

            }
            return null;


        }


        #endregion

    }
}
