using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;

namespace BistroModel
{
	/// <summary>
	/// Session stub. Minimal interface used for development.
	/// </summary>
	internal class BSession : ISession
	{
		public IUserProfile UserProfile { get { return null; } }
		public bool Authenticate(string token, string password) { return true; }
	}
}
