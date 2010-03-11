using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using Irony.Parsing;
using Irony.Samples.CSharp;
using Irony.Samples.FSharp;
using Bistro.MethodsEngine.Reflection;
using Bistro.Interfaces;
using Bistro.Controllers.Descriptor;

namespace Bistro.Designer.Explorer
{
    using ControllersTable = Dictionary<string, Dictionary<string, List<string>>>;
    public static class CtrlUtils
    {
        public static bool IsCtrlInCollection(IEnumerable<IMethodsControllerDesc> collection, string ctrlName)
        {
            foreach (IMethodsControllerDesc item in collection)
            {
                if (String.Compare(item.ControllerTypeName, ctrlName) == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public  class AttributesComparer : IEqualityComparer<ControllersTable>
    {

        #region IEqualityComparer<ControllersTable<string,Dictionary<string,List<string>>>> Members

        public bool Equals(Dictionary<string, Dictionary<string, List<string>>> x, Dictionary<string, Dictionary<string, List<string>>> y)
        {
            //x supposed to be previous data, y is new parsed data
            if (x.Count != y.Count) 
                return false;
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> ctrlRow in x)
            {
               Dictionary<string, List<string>> val;
               if (y.TryGetValue(ctrlRow.Key, out val))
               {
                   foreach (KeyValuePair<string, List<string>> kvp in val)
                   {
                       if (!ctrlRow.Value.ContainsKey(kvp.Key))
                           return false; //found new attribute on controller
                       else
                       {
                           if (kvp.Value.Count != ctrlRow.Value[kvp.Key].Count)
                               return false;//the amount of attribute params is different 
                           for (int i = 0; i < kvp.Value.Count; i++)
                           {
                               if (kvp.Value[i] != ctrlRow.Value[kvp.Key][i])
                                   return false;//the parameter's value is different in tables
                           }
                       }
                   }
               }
               else
                   return false;
            }
            return true;

        }

        public int GetHashCode(Dictionary<string, Dictionary<string, List<string>>> obj)
        {
            return obj.ToString().ToLower().GetHashCode();

        }

        #endregion
    }
}