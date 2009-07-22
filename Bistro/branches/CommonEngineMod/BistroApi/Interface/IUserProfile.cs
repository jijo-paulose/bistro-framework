using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi
{
	/// <summary>
	/// UserProfile stub. Minimal interface used for development.
	/// </summary>
	public interface IUserProfile 
	{
		bool IsAuthenticated { get; }
		bool IsInRole(string role);
	}
}
