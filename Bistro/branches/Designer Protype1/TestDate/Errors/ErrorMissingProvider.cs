using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Tests.Errors
{
    public class ErrorMissingProvider : ErrorDescriptor
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

            for (int i = 0; i < this.controllerTypesList.Count; i++)
            {
            }

        }



    }
}
