using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Validation;

namespace Bistro.Extensions.Validation.Common
{
    public class LengthValidator<T> : DefaultValidator<T> where T : IValidatable
    {
        int min, max;

        public LengthValidator(string message, int min, int max)
            : base(message)
        {
            this.min = min;
            this.max = max;
        }

        public override bool DoValidate(object target, out List<string> messages)
        {
            messages = new List<string>();
            bool fail = false;

            string stringTarget = target as string;
            if (stringTarget == null)
                return true;

            if (min > -1 && stringTarget.Length < min)
                fail = true;
            else if (max > -1 && stringTarget.Length > max)
                fail = true;

            if (fail)
                messages.Add(Message);

            return !fail;
        }
    }
}
