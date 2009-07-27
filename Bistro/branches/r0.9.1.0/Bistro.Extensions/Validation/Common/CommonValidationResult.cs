using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Validation;

namespace Bistro.Extensions.Validation.Common
{
    public class CommonValidationResult: IValidationResult
    {
        public CommonValidationResult(IValidator rule, IValidatable target, string message, bool success)
        {
            Rule = rule;
            Target = target;
            Message = message;
            Success = success;
        }

        public IValidator Rule
        {
            get;
            set;
        }

        public IValidatable Target
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public bool Success
        {
            get;
            set;
        }
    }
}
