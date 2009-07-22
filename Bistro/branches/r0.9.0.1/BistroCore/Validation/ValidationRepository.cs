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
        private Dictionary<string, IValidator> validatorsByNamespace = new Dictionary<string, IValidator>();

        private Dictionary<Type, IValidator> validatorsByType = new Dictionary<Type, IValidator>();

        /// <summary>
        /// Registers the validator.
        /// </summary>
        /// <param name="validator">The validator.</param>
        public void RegisterValidator(Type target, IValidator validator)
        {
            IValidator current = null;
            if (validatorsByNamespace.TryGetValue(validator.Name, out current))
                validatorsByNamespace[validator.Name] = validator.Merge(current);
            else
                validatorsByNamespace.Add(validator.Name, validator);

            if (validatorsByType.TryGetValue(target, out current))
                validatorsByType[target] = validator.Merge(current);
            else
                validatorsByType.Add(target, validator);
        }

        /// <summary>
        /// Gets the validator for namespace.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IValidator GetValidatorForNamespace(string name)
        {
            IValidator validator = null;
            if (!validatorsByNamespace.TryGetValue(name, out validator))
                return null;

            return validator;
        }

        public IValidator GetValidatorForType(Type type)
        {
            IValidator validator = null;
            if (!validatorsByType.TryGetValue(type, out validator))
                return null;

            return validator;
        }
    }
}
