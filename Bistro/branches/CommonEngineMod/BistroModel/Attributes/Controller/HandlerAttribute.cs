using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;

namespace BistroModel 
{
	/// <summary>
	/// Assigns an alternative handler to a controller class.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class HandlerAttribute : Attribute
	{
		Type _handlerType;
		public HandlerAttribute(Type handlerType) 
		{
			if (handlerType == null)
				throw new ArgumentNullException("handlerType");
			if (!typeof(IControllerHandler).IsAssignableFrom(handlerType))
				throw new ArgumentOutOfRangeException("handlerType", "Must be assignable to IControllerHandler.");
			_handlerType = handlerType;
		}
		public Type Type { get { return _handlerType; } }
	}
}
