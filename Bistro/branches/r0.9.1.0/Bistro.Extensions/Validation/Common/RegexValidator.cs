using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Validation;
using System.Text.RegularExpressions;

namespace Bistro.Extensions.Validation.Common
{
    /// <summary>
    /// Regular expression validator. If the object supplied is not a string, or null, 
    /// the validation passes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RegexValidator<T>: DefaultValidator<T> where T : IValidatable
    {
        Regex re;

        public RegexValidator(string message, string regex, RegexOptions options)
            : base(message)
        {
            re = new Regex(regex, options);
        }

        public override bool DoValidate(object target, out List<string> messages)
        {
            messages = new List<string>();
            bool fail = false;

            string stringTarget = target as string;
            if (stringTarget == null)
                return true;

            fail = !re.IsMatch(stringTarget);

            if (fail)
                messages.Add(Message);

            return !fail;
        }
    }
}