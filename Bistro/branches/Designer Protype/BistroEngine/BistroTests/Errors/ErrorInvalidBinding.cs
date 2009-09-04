using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Bistro.Tests.Errors
{
    internal class ErrorInvalidBinding : ErrorDescriptor
    {
        private string controllerType;

        private List<string> bindingsList;

        internal ErrorInvalidBinding(string ctrlrType, params string[] bindings )
            : base()
        {
            controllerType = ctrlrType;
            bindingsList = new List<string>(bindings);

        }

        public override void Validate(IErrorDescriptor errorDesc)
        {
            base.Validate(errorDesc);
            ErrorInvalidBinding errorIB = (errorDesc as ErrorInvalidBinding);
            Assert.IsNotNull(errorIB, "Compared Invalid Binding error has incompatible type.");

            Assert.AreEqual(errorIB.controllerType, this.controllerType, "Controller types in Invalid Binding error are different: '{0}' and '{1}'", errorIB.controllerType, this.controllerType);
            Assert.AreEqual(errorIB.bindingsList.Count, this.bindingsList.Count, "Binding lists in compared Invalid Binding errors have different length: '{0}' and '{1}'", errorIB.bindingsList.Count, this.bindingsList.Count);

            //TODO: make a sort here would be better
            for (int i = 0; i < this.bindingsList.Count; i++)
            {
                Assert.AreEqual(this.bindingsList[i], errorIB.bindingsList[i], "Bindings are different in Invalid Binding error: '{0}' and '{1}'", this.bindingsList[i], errorIB.bindingsList[i]);
            }
        }
    }
}
