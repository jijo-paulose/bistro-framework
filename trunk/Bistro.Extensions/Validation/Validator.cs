using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Validation;
using System.Linq.Expressions;

namespace Bistro.Extensions.Validation
{
    /// <summary>
    /// Build class for validation rules
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Validator<T>: IValidator
    {
        List<IValidator> children = new List<IValidator>();

        /// <summary>
        /// Sets a validation namespace for this, and all child validations.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Validator<T> As(string name)
        {
            Name = name;

            return this;
        }

        /// <summary>
        /// Generates a <see cref="ValidationSite"/> validation site for the specified member
        /// </summary>
        /// <typeparam name="K">the type of the member</typeparam>
        /// <param name="expr">The expression tree for the member.</param>
        /// <returns></returns>
        public ValidationSite<T,K> For<K>(Expression<Func<T, K>> expr)
        {
            return new ValidationSite<T, K>(expr);
        }

        /// <summary>
        /// Incorporates rules from within the target into this validator. This operation will
        /// place the validation rules of the target as children within the namespace of the
        /// instance. Given a rule on the target with a name of "a.b", and a validator with a name of
        /// "c", a.b will become "c.a.b".
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>a validator which is the composition of the instance with the target</returns>
        public Validator<T> WithRulesFrom(IValidatable target)
        {
            children.Add(target.Validator);
            return this;
        }

        /// <summary>
        /// Determines whether the specified target is valid.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>
        /// 	<c>true</c> if the specified target is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid(object target, out List<string> messages)
        {
            var valid = true;
            messages = new List<string>();

            foreach (IValidator child in children)
            {
                var newMessages = new List<string>();
                valid = valid && child.IsValid(target, out newMessages);

                messages.AddRange(newMessages);
            }

            return valid;
        }

        /// <summary>
        /// Gets the name of the validation rule defined by this validator.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the child validators of this instance
        /// </summary>
        /// <value>The children.</value>
        public IEnumerable<IValidator> Children
        {
            get { return children; }
        }
    }
}
