using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi {
	/// <summary>
	/// Semantics to describe a controller that defines security policy for a set of bind points
	/// </summary>
	public interface ISecurityController {
		/// <summary>
		/// Determines whether the currently logged in user (or the anonymous user) has access
		/// to the bind point or points that this controller binds to.
		/// 
		/// Given two controllers A and B, with the security rules of A being invoked prior to B, 
		/// A has the responsibility of populating the failedPermissions list with the full list 
		/// of permissions that have failed as a result of this check. Once B is invoked, it can 
		/// overrule the decision of A by removing an entry from the failedPermissions list. A 
		/// full permissions evaluation will succeed if and only if the last security controller
		/// invoked returned a <code>true</code> <i>and</i> the list of failed permissions is empty
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="failedPermissions">A set of role requirements that were not met by previous security controllers.</param>
		/// <returns>
		/// 	<c>true</c> if access should be granted; otherwise, <c>false</c>.
		/// </returns>
		bool HasAccess(IContext context, IDictionary<string, KeyValuePair<FailAction, string>> failedPermissions);
	}
}
