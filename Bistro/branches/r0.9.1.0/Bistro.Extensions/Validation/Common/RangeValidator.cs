using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Validation;
using System.Globalization;

namespace Bistro.Extensions.Validation.Common
{
    /// <summary>
    /// Validates that the target falls within the specified range. If the supplied value is not
    /// <see cref="System.IConvertible"/>, the validation fails. If the supplied value is IConvertible
    /// but does not convert to type <c>K</c>, the validation fails
    /// </summary>
    /// <typeparam name="T">The type of the target element</typeparam>
    /// <typeparam name="K">The type of the boundary values</typeparam>
    public class RangeValidator<T, K> : DefaultValidator<T>
        where T : IValidatable
        where K : IComparable
    {
        K min, max;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValidator&lt;T, K&gt;"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        public RangeValidator(string message, K min, K max)
            : base(message)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Evaluates this validator only.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="messages">The messages.</param>
        /// <returns></returns>
        public override bool DoValidate(object target, out List<string> messages)
        {
            messages = new List<string>();
            bool fail = false;

            IConvertible convertible = target as IConvertible;
            if (convertible == null)
                fail = true;
            else
                try
                {
                    K val = (K)Convert.ChangeType(convertible, typeof(K));
                    fail = (val.CompareTo(min) < 0) || (val.CompareTo(max) > 0);
                }
                catch (Exception)
                {
                    fail = true;
                }

            if (fail)
                messages.Add(Message);

            return !fail;
        }
    }
}