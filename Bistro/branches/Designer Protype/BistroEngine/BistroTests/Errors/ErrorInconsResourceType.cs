using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Bistro.Tests.Errors
{
    internal class ErrorInconsResourceType :ErrorDescriptor 
    {
        private string fullBindingUrl;

        private string resourceName;

        private Dictionary<string,string> controllersList;

        internal ErrorInconsResourceType(string fullBindUrl, string resName, params string[] ctrlrsNTypes)
            : base()
        {
            fullBindingUrl = fullBindUrl;
            resourceName = resName;

            if (ctrlrsNTypes.Length % 2 != 0)
                throw new Exception("Error occured while creating Inconsistent Resource Type error");
            
            for(int i=0; i<ctrlrsNTypes.Length; i=i+2)
            {
                controllersList.Add(ctrlrsNTypes[i], ctrlrsNTypes[i + 1]);
            }

        }

        public override void Validate(IErrorDescriptor errorDesc)
        {
            base.Validate(errorDesc);
            ErrorInconsResourceType error = (errorDesc as ErrorInconsResourceType);
            Assert.IsNotNull(error, "Compared Inconsistent error has incompatible type.");

            Assert.AreEqual(error.fullBindingUrl, this.fullBindingUrl, "Binding urls are different: '{0}' and '{1}'", error.fullBindingUrl, this.fullBindingUrl);
            Assert.AreEqual(error.resourceName, this.resourceName, "Resource names are different: '{0}' and '{1}'", error.resourceName, this.resourceName);
            Assert.AreEqual(error.controllersList.Count, this.controllersList.Count, "Controllers lists have different length: '{0}' and '{1}'", error.controllersList.Count, this.controllersList.Count);

            List<string> firstList = error.controllersList.Keys.OrderBy(x => x).ToList<string>();
            List<string> secondList = this.controllersList.Keys.OrderBy(x => x).ToList<string>();

            for (int i = 0; i < firstList.Count; i++)
            {
                Assert.AreEqual(firstList[i], secondList[i], "Different controller names in Inconsistent Resource Error");
                Assert.AreEqual(error.controllersList[firstList[i]], this.controllersList[secondList[i]], "Different types for controller name '{0}'", firstList[i]);
            }

        }

    }
}
