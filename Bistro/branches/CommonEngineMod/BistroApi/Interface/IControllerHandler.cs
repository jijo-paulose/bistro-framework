using System;
namespace BistroApi
{
    /// <summary>
    /// Manages the state of a controller pre and post processing.
    /// </summary>
    public interface IControllerHandler
    {
        /// <summary>
        /// Gets an instance of a controller prepared to execute the associated invocation info.
        /// </summary>
        /// <param name="info">The invocation info for the controller.</param>
        /// <param name="context">The http context for the current request.</param>
        /// <returns>an IController instance</returns>
			IController GetControllerInstance(IControllerInfo controllerInfo, IBinding binding, IContext bistroContext);

        /// <summary>
        /// Returns the controller back to a ready state.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="context">The http context for the current request.</param>
			void ReturnController(IController controller, IControllerInfo controllerInfo, IBinding binding, IContext bistroContext);
    }
}
