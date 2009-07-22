using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi 
{
	public interface IQueryStringItem 
	{
		string Name { get; }
		string Value { get; }
	}
}
