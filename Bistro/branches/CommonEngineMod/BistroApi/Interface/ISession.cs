using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi {
	/// <summary>
	/// Session stub. Minimal interface used for development.
	/// </summary>
	public interface ISession 
	{
		IUserProfile UserProfile { get; }
		bool Authenticate(string token, string password);
	}
}
