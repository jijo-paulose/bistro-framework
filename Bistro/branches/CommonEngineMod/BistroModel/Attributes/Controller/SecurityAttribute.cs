using System;
using System.Security.Principal;
using BistroApi;

namespace BistroModel {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public abstract class SecurityAttribute : Attribute {
		/// <summary>
		/// The role given to all anonymous users
		/// </summary>
		public const string ANONYMOUS = "?";

		/// <summary>
		/// The role given to all authenticated users
		/// </summary>
		public const string AUTHENTICATED = "*";

		/// <summary>
		/// The name of the permission to enforce
		/// </summary>
		public string Role { get; set; }

		/// <summary>
		/// Action(s) to take if permssion is not present
		/// </summary>
		public FailAction OnFailure { get; set; }

		/// <summary>
		/// The redirect target
		/// </summary>
		public string Target { get; set; }

		/// <summary>
		/// Determines whether the specified user has access.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>
		/// 	<c>true</c> if the specified user has access; otherwise, <c>false</c>.
		/// </returns>
		public abstract bool HasAccess(IPrincipal user);

		/// <summary>
		/// Gets a value indicating whether this permission failing implies that access should be explicitly denied
		/// </summary>
		/// <value><c>true</c> if hard fail; otherwise, <c>false</c>.</value>
		public abstract bool HardFail { get; }

		/// <summary>
		/// Gets a value indicating whether this attribute succeeding should override a prior attribute failing
		/// </summary>
		/// <value><c>true</c> if override prior; otherwise, <c>false</c>.</value>
		public abstract bool OverridePrior { get; }
	}
}
