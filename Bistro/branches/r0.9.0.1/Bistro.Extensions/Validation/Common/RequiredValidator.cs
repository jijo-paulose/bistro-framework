using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Extensions.Validation.Common
{
    public class RequiredValidator<T> : Validator<T>
    {
        string message;

        public RequiredValidator(string message)
        {
            this.message = message;
        }

        public override bool DoValidate(object target, out List<string> messages)
        {
            messages = new List<string>();

            if (target == null)
                messages.Add(message);

            return messages.Count == 0;
        }
    }
}
