using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Validation;

namespace Bistro.Extensions.Validation.Common
{
    public class RequiredValidator<T> : DefaultValidator<T> where T : IValidatable
    {
        public RequiredValidator(string message) : base(message) { }

        public override bool DoValidate(object target, out List<string> messages)
        {
            messages = new List<string>();
            bool fail = false;

            if (target == null)
                fail = true;
            else
            {
                var stringTarget = target as string;
                if (stringTarget != null && String.IsNullOrEmpty(stringTarget))
                    fail = true;
            }

            if (fail)
                messages.Add(Message);
 
            return !fail;
        }
    }
}