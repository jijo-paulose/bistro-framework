using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Bistro.Tests.Errors
{
    internal class ErrorResourceLoop : ErrorDescriptor
    {
        private string fullBindingUrl;

        private List<string> controllersList;

        internal ErrorResourceLoop(string fullBindUrl, params string[] ctrlrs)
            : base()
        {
            fullBindingUrl = fullBindUrl;
            controllersList = new List<string>(ctrlrs);
        }

        public override void Validate(IErrorDescriptor errorDesc)
        {
            base.Validate(errorDesc);
            ErrorResourceLoop error = (errorDesc as ErrorResourceLoop);
            Assert.IsNotNull(error, "Compared Resource Loop error has incompatible type.");

            Assert.AreEqual(error.fullBindingUrl, this.fullBindingUrl, "Binding urls are different: '{0}' and '{1}'", error.fullBindingUrl, this.fullBindingUrl);
            Assert.AreEqual(error.controllersList.Count, this.controllersList.Count, "Controllers lists have different length: '{0}' and '{1}'", error.controllersList.Count, this.controllersList.Count);

            for (int i = 0; i < this.controllersList.Count; i++)
            {
                Assert.AreEqual(this.controllersList[i], error.controllersList[i], "Controller names are different in lists: '{0}' and '{1}'", this.controllersList[i], error.controllersList[i]);
            }

        }
    }
}
