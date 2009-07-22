using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;

namespace BistroModel {
	class BQueryStringItem : IQueryStringItem
	{
		string _name;
		string _value;
		public BQueryStringItem(string nameEqualsValue) {
			if (nameEqualsValue == null)
				throw new ArgumentNullException("nameEqualsValue");
			if (nameEqualsValue.Trim().Length == 0)
				throw new ArgumentOutOfRangeException("nameEqualsValue", "name cannot be empty or whitespace characters.");
			string[] parts = nameEqualsValue.Split('=');
			if(parts.Length > 2)
				throw new ArgumentOutOfRangeException("nameEqualsValue", "contains too many '=' characters. (should only be one.)");
			_name = parts[0];
			if (parts.Length == 2)
				_value = parts[1];
		}
		public string Name { get { return _name; } }
		public string Value { get { return _value; } }
	}
}
