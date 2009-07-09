using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Validation;
using System.Linq.Expressions;

namespace Bistro.Extensions.Validation
{
    public class Validator<T>: IValidator
    {
        public Validator<T> As(string name)
        {
            throw new NotImplementedException();
        }

        public Validator<T> For<K>(Expression<Func<T, K>> expr)
        {
            throw new NotImplementedException();
        }

        public Validator<T> WithRulesFrom(IValidatable target)
        {
            throw new NotImplementedException();
        }

        public bool IsValid(object target, out List<string> messages)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }
    }
}
