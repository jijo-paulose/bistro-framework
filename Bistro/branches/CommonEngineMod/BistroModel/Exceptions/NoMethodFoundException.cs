using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroModel {
	public class NoMethodFoundException : ApplicationException {
		public NoMethodFoundException() : base() { }
		public NoMethodFoundException(string message) : base(message) { }
	}
}
