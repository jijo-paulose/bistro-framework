using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Tests.Errors
{
    public class ErrorInconsResourceType :ErrorDescriptor 
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

            List<string> firstList = error.controllersList.Keys.OrderBy(x => x).ToList<string>();
            List<string> secondList = this.controllersList.Keys.OrderBy(x => x).ToList<string>();

            for (int i = 0; i < firstList.Count; i++)
            {
            }

        }

    }
}
