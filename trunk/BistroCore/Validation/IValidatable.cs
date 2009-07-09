using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Validation
{
    /// <summary>
    /// Marks a class as supporting validation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValidatable
    {
        /// <summary>
        /// Gets the validator that encompases the validation rules for T.
        /// </summary>
        /// <value>The validator.</value>
        IValidator Validator { get; }
    }
}
