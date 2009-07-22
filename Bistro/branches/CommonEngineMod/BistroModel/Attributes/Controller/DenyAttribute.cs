using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace BistroModel {
	/// <summary>
	/// Marks a controller as requiring a particular permission or role to enter.
	/// </summary>
	public class DenyAttribute : SecurityAttribute {
		/// <summary>
		/// Marks a security controller with permission data
		/// </summary>
		/// <param name="permissionName"></param>
		public DenyAttribute(string role) {
			Role = role;
		}

		/// <summary>
		/// Determines whether the specified user has access.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>
		/// 	<c>true</c> if the specified user has access; otherwise, <c>false</c>.
		/// </returns>
		public override bool HasAccess(IPrincipal user) {
			switch (Role) {
				case ANONYMOUS:
					// deny anonymous users - so have a profile, and marked authed
					return user != null && user.Identity.IsAuthenticated;
				case AUTHENTICATED:
					// deny authed users - greedy match - deny all
					return user == null || !user.Identity.IsAuthenticated;
				default:
					return !user.IsInRole(Role);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this permission failing implies that access should be explicitly denied
		/// </summary>
		/// <value><c>true</c> if hard fail; otherwise, <c>false</c>.</value>
		public override bool HardFail { get { return true; } }

		/// <summary>
		/// Gets a value indicating whether this attribute succeeding should override a prior attribute failing
		/// </summary>
		/// <value><c>true</c> if override prior; otherwise, <c>false</c>.</value>
		public override bool OverridePrior { get { return false; } }
	}
}
