using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BistroApi
{
	/// <summary>
	/// Contains meta data about a controller in a
	/// convenient form.
	/// </summary>
	public interface IControllerInfo 
	{
		string Name { get; }
		IResources Resources { get; }
		bool IsSecurity { get; }
		int GetPriority(IBinding binding);
		void SetPriority(IBinding binding, int priority);
		BindType GetBindType(IBinding binding);
		void SetBindType(IBinding binding, BindType bindType);
		bool Has(IResource br);
		IControllerHandler Handler { get; }
		MemberInfo MemberInfo { get; }
		IEnumerable<IBinding> Bindings { get; }
		string DefaultTemplate { get; }
	}
}
