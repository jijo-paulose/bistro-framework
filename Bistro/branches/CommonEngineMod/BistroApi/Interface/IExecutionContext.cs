using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Security.Principal;

namespace BistroApi
{
	/// <summary>
	/// The execution context is used to allow controllers to pass execution instructions to the environment
	/// </summary>
	public interface IExecutionContext {
		/// <summary>
		/// Transfers execution to the given url. All currently queued controllers will finish execution.
		/// </summary>
		/// <param name="target"></param>
		void Transfer(string target);

		/// <summary>
		/// Gets the requested transfer target
		/// </summary>
		/// <returns></returns>
		string TransferTarget { get; }

		/// <summary>
		/// Determines whether the context has had a transfer request
		/// </summary>
		bool TransferRequested { get; }

		/// <summary>
		/// Clears the transfer request from the context
		/// </summary>
		void ClearTransferRequest();

		/// <summary>
		/// Gets the current user, or null if not authenticated
		/// </summary>
		/// <value>The current user.</value>
		IPrincipal CurrentUser { get; }

		/// <summary>
		/// Authenticates the specified user.
		/// </summary>
		/// <param name="token">The token identifiying a user.</param>
		/// <param name="password">The password.</param>
		/// <returns></returns>
		void Authenticate(IPrincipal user);

		/// <summary>
		/// Abandons the session.
		/// </summary>
		void Abandon();

		/// <summary>
		/// Gets the response object.
		/// </summary>
		/// <value>The response.</value>
		IResponse Response { get; }
	}
}
