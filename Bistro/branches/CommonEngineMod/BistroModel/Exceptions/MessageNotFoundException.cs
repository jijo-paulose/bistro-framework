using System;
using System.Collections.Generic;
using System.Text;

namespace BistroModel 
{
	public class MessageNotFoundException : ApplicationException 
	{
		public MessageNotFoundException() : base() { }
		public MessageNotFoundException(string message) : base(message) { }
	}
}
