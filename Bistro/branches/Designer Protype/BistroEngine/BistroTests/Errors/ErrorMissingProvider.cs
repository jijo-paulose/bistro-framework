using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Bistro.Tests.Errors
{
    internal class ErrorMissingProvider : ErrorDescriptor
    {
        private string fullBindingUrl;

        private string resourceName;

        private List<string> controllerTypesList;


        internal ErrorMissingProvider(string fullBindUrl, string resName, params string[] ctrlrs) : base()
        {
            fullBindingUrl = fullBindUrl;
            resourceName = resName;
            controllerTypesList = new List<string>(ctrlrs);
        }

        public override void Validate(IErrorDescriptor errorDesc)
        {
            base.Validate(errorDesc);
            ErrorMissingProvider error = (errorDesc as ErrorMissingProvider);
            Assert.IsNotNull(error, "Compared Missing Provider error has incompatible type.");

            Assert.AreEqual(error.fullBindingUrl, this.fullBindingUrl, "Binding urls are different: '{0}' and '{1}'", error.fullBindingUrl, this.fullBindingUrl);
            Assert.AreEqual(error.resourceName, this.resourceName, "Resource names are different: '{0}' and '{1}'", error.resourceName, this.resourceName);
            Assert.AreEqual(error.controllerTypesList.Count, this.controllerTypesList.Count, "Controllers lists have different length: '{0}' and '{1}'", error.controllerTypesList.Count, this.controllerTypesList.Count);

            for (int i = 0; i < this.controllerTypesList.Count; i++)
            {
                Assert.AreEqual(this.controllerTypesList[i], error.controllerTypesList[i], "Controller names are different in lists: '{0}' and '{1}'", this.controllerTypesList[i], error.controllerTypesList[i]);
            }

        }



    }
}
