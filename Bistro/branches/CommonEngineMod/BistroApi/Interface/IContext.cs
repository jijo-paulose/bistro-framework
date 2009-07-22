using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Web;

namespace BistroApi
{
    /// <summary>
    /// A context used to pass information between controllers.
    /// </summary>
    public interface IContext : IDictionary, IExecutionContext
    {
			/// <summary>
			/// Gets the session.
			/// </summary>
			/// <value>The session.</value>
			HttpSessionStateBase Session { get; }
			IUrl Url { get; set; }
			HttpContextBase HttpContext { get; }
    }
}