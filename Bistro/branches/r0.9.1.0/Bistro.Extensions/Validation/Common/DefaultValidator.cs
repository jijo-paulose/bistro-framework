using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Validation;

namespace Bistro.Extensions.Validation.Common
{
    public class DefaultValidator<T> : Validator<T> where T : IValidatable
    {
        public virtual string Message { get; protected set; }

        public DefaultValidator(string message)
        {
            Message = message;

            DefiningParams.Add("message", message);
        }
    }
}
