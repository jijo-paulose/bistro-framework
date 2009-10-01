using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Methods;

namespace Bistro.Engine.Methods.Generation
{
    internal class MethodUrlsSubset
    {
        internal MethodUrlsSubset()
        {
            bindingsList = new List<GenBinding>();


        }

        private MethodUrlsSubset(List<GenBinding> oldBindingsList, GenBinding newBinding)
        {
            bindingsList = new List<GenBinding>(oldBindingsList);
            bindingsList.Add(newBinding);
        }



        #region private members

        private List<GenBinding> bindingsList;
        #endregion

        #region internal members

        internal List<GenBinding> BindingsList
        {
            get { return bindingsList; }
        }


        /// <summary>
        /// This method is called for each binding and returns newly-constructed SubSet, consists of matching groups
        /// </summary>
        /// <param name="newBinding"></param>
        /// <returns></returns>
        internal MethodUrlsSubset ApplyBinding(GenBinding newBinding)
        {

            if (!newBinding.MatchWithSubSet(this))
            {
                return null;
            }

            return new MethodUrlsSubset(bindingsList, newBinding);
        }


        #endregion

    }
}
