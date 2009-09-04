using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            for (int i = 0; i < this.controllersList.Count; i++)
            {
            }

        }
    }
}
