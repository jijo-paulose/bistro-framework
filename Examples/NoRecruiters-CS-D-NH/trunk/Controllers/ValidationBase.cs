using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers;

namespace NoRecruiters.Controllers.Auth
{
    /// <summary>
    /// Base class for authentication controllers. This class provides basic validation functionality
    /// and exposes default validation components used in authentication and user management
    /// screens.
    /// </summary>
    public abstract class ValidationBase : AbstractController
    {
        /// <summary>
        /// A form field-specific errors dictionary. This value is exposed to the request 
        /// context.
        /// </summary>
        [Request]
        protected Dictionary<string, string> errors;

        /// <summary>
        /// Gets the error count.
        /// </summary>
        /// <value>The error count.</value>
        protected int ErrorCount { get { return errors == null ? 0 : errors.Count; } }

        /// <summary>
        /// Places an error onto the errors dictionary
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="error">The error.</param>
        protected void ReportError(string element, string error)
        {
            if (errors == null)
                errors = new Dictionary<string, string>();

            string errorKey = element ?? errors.Count.ToString();

            if (errors.ContainsKey(errorKey))
                errors[errorKey] += error;
            else
                errors.Add(errorKey, error);
        }
    }
}
