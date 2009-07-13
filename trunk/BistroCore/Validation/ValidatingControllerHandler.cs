using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Controllers;
using Bistro.Controllers.Descriptor;
using Bistro.Configuration.Logging;

namespace Bistro.Validation
{
    /// <summary>
    /// Controller handler that incorporates validation functionality
    /// </summary>
    public class ValidatingControllerHandler: ControllerHandler
    {
        enum Exceptions
        {
            [DefaultMessage("{0} does not implement IValidatable")]
            InvalidCast,
            [DefaultMessage("'{0}' occured while attempting to query {1} for validation rules. Validation rule discovery occurs outside of normal controller lifecycle, and cannot rely on any controller state.")]
            RuntimeError
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingControllerHandler"/> class.
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="logger"></param>
        protected internal ValidatingControllerHandler(ControllerDescriptor descriptor, ILogger logger)
            : base(descriptor, logger)
        {
            try
            {
                var instance = this.controllerConstructor.Invoke(EmptyParams) as IValidatable;
                ValidationRepository.Instance.RegisterValidator(instance.Validator);
            }
            catch (InvalidCastException)
            {
                logger.Report(Exceptions.InvalidCast, descriptor.ControllerTypeName);
            }
            catch (Exception ex)
            {
                logger.Report(Exceptions.RuntimeError, ex, ex.Message, descriptor.ControllerTypeName);
            }
        }

        /// <summary>
        /// Gets the controller instance.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <param name="requestContext">The request context.</param>
        /// <returns></returns>
        public override IController GetControllerInstance(ControllerInvocationInfo info, System.Web.HttpContextBase context, IContext requestContext)
        {
            var instance = base.GetControllerInstance(info, context, requestContext);

            var validatable = (IValidatable)instance;
            var validator = validatable.Validator;

            var messages = new List<string>();
            validatable.IsValid = validator.IsValid(instance, out messages);
            validatable.Messages = messages;

            return instance;
        }
    }
}
