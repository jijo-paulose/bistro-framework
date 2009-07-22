using System;
using System.Collections.Generic;
using System.Text;
namespace BistroModel
{
	/// <summary>
	/// Used to mark enums for use as messages.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class DefaultMessageAttribute : Attribute 
	{
		string _message;
		public DefaultMessageAttribute(string message)
		{
			_message = message;
		}
		public string Message { get { return _message; } set { _message = value; } }
	}
}
