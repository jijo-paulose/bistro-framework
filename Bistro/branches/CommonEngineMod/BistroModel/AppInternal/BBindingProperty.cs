using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;

namespace BistroModel {
	/// <summary>
	/// Contains properties defined at the intersection of 
	/// a binding and a controller.
	/// </summary>
	internal class BBindingProperty {
		#region private
		int _priority;
		BindType _bindType;
		#endregion

		#region construction
		public BBindingProperty() 
		{
			_priority = DefaultPriority;
			_bindType = DefaultBindType;
		}
		public BBindingProperty(int priority) 
		{
			_priority = priority;
		}
		public BBindingProperty(BindType bindType) 
		{
			_bindType = bindType;
		}
		#endregion

		#region public
		public int Priority { get { return _priority; } set { _priority = value; } }
		public BindType BindType { get { return _bindType; } set { _bindType = value; } }

		public static int DefaultPriority { get { return -1; } }
		public static BindType DefaultBindType { get { return BindType.Before; } }
		#endregion
	}
}
