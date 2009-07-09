using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Validation
{
    /// <summary>
    /// Interface for validator objects that can be used to validate an instance of type T
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Determines whether the specified target is valid.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>
        /// 	<c>true</c> if the specified target is valid; otherwise, <c>false</c>.
        /// </returns>
        bool IsValid(object target, out List<string> messages);

        /// <summary>
        /// Gets the name of the validation rule defined by this validator.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
    }
}
