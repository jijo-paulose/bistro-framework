using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi
{
	public interface ILogger
	{
		Exception Report(Exception e);
		void Report(Enum code, Exception e, params object[] args);
		void Report(Enum code, params object[] args);
	}
}
