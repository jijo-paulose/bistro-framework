using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Tests.Errors
{
    public class ErrorInvalidBinding : ErrorDescriptor
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

            //TODO: make a sort here would be better
            for (int i = 0; i < this.bindingsList.Count; i++)
            {
            }
        }
    }
}
