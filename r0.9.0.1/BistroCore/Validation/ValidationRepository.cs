using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Validation
{
    /// <summary>
    /// A repository of all known controller validation rules
    /// </summary>
    public class ValidationRepository
    {
        private static ValidationRepository instance = new ValidationRepository();

        /// <summary>
        /// Gets an instance of the repository.
        /// </summary>
        /// <value>The instance.</value>
        public static ValidationRepository Instance { get { return instance; } }

        /// <summary>
        /// Map of validators by namespace
        /// </summary>
        private Dictionary<string, IValidator> validators = new Dictionary<string, IValidator>();

        /// <summary>
        /// Registers the validator.
        /// </summary>
        /// <param name="validator">The validator.</param>
        public void RegisterValidator(IValidator validator)
        {
            IValidator current = null;
            if (validators.TryGetValue(validator.Name, out current))
                validators[validator.Name] = current.Merge(validator);
            else
                validators.Add(validator.Name, current);
        }

        /// <summary>
        /// Gets the validator for namespace.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IValidator GetValidatorForNamespace(string name)
        {
            IValidator validator = null;
            if (!validators.TryGetValue(name, out validator))
                return null;

            return validator;
        }
    }
}
