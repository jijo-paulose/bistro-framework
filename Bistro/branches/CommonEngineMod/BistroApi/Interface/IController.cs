using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Reflection;

namespace BistroApi
{
    /// <summary>
    /// Base interface for controller functionality
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        void ProcessRequest(HttpContextBase context, IContext requestContext);
        /// <summary>
        /// Gets a value indicating whether this <see cref="IController"/> is reusable. If reusable and Stateful, 
        /// the recycle method will be called once request processing is complete.
        /// </summary>
        /// <value><c>true</c> if reusable; otherwise, <c>false</c>.</value>
        bool Reusable { get;}
        /// <summary>
        /// Gets a value indicating whether this <see cref="IController"/> is stateful. If reusable and Stateful, 
        /// the recycle method will be called once request processing is complete.
        /// </summary>
        /// <value><c>true</c> if stateful; otherwise, <c>false</c>.</value>
        bool Stateful { get;}
        /// <summary>
        /// Initializes this instance. This method is called before system-manipulated fields have been populated.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Recycles this instance. Recycle is called after ProcessRequest completes for stateful reusable controllers.
        /// This method is intended to put the controller in a state ready for a new request. This method may not be
        /// called from the request execution thread.
        /// </summary>
        void Recycle();
				/// <summary>
				/// Gets a global type-system identifier for this class of controllers. In most cases this is simply the Type of 
				/// the controller class.
				/// </summary>
				/// <returns></returns>
				MemberInfo GlobalHandle { get; }
    }
}
