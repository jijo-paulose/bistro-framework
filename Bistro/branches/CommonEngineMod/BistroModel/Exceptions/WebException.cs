using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;

namespace BistroModel {
	/// <summary>
	/// An application exception that should be reported to the end user with an http status code
	/// </summary>
	public class WebException : ApplicationException {
		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The code.</value>
		public StatusCode Code { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="WebException"/> class.
		/// </summary>
		/// <param name="statusCode">The status code.</param>
		/// <param name="message">The message.</param>
		public WebException(StatusCode statusCode, string message)
			: base(message) {
			Code = statusCode;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebException"/> class.
		/// </summary>
		/// <param name="statusCode">The status code.</param>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		public WebException(StatusCode statusCode, string message, Exception exception)
			: base(message, exception) {
			Code = statusCode;
		}
	}
}