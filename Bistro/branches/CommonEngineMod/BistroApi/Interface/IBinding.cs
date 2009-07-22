using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi
{
	/// <summary>
	/// Represents a Bistro binding of a URL (which may contain
	/// allowed wildcard characters) to a set of controller infos.
	/// </summary>
	public interface IBinding 
	{
		/// <summary>
		/// The URL string representation of this binding.
		/// Can contain wildcard characters.
		/// </summary>
		string Name { get; }
		/// <summary>
		/// The first part of the URL string representation
		/// of this binding. Can contain wildcard characters.
		/// </summary>
		string Head { get; }
		/// <summary>
		/// The HttpAction.
		/// </summary>
		HttpAction HttpAction { get; }
		/// <summary>
		/// Array elements containing each part of the URL of thie
		/// binding.
		/// </summary>
		string[] Parts { get; }
		/// <summary>
		/// Array elements containing each part of the URL of this
		/// binding except the first part.
		/// </summary>
		string[] SubParts { get; }
		/// <summary>
		/// Returns a binding based on the SubParts of this binding.
		/// </summary>
		IBinding SubBinding { get; }
		/// <summary>
		/// Returns the full Binding object from which this SubBinding
		/// was directly or indirectly derived. If this is not a
		/// SubBinding then returns itself.
		/// </summary>
		IBinding BaseBinding { get; }
		/// <summary>
		/// Returns the part of the URL of this binding based on postion.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		string this[int index] { get; }
		/// <summary>
		/// Returns the count of Parts (URL parts) of this binding.
		/// </summary>
		int Length { get; }
		/// <summary>
		/// Associates a ControllerInfo with this binding.
		/// </summary>
		/// <param name="controller"></param>
		void AddControllerInfo(IControllerInfo controllerInfo);
		/// <summary>
		/// Returns all ControllerInfos associated with this binding.
		/// </summary>
		IEnumerable<IControllerInfo> ControllerInfos { get; }
		/// <summary>
		/// Returns an array of Parameter Names defined in the URL of
		/// this binding.
		/// </summary>
		string[] ParameterNames { get; }
		/// <summary>
		/// Returns an array of Parameter Values that are contained in
		/// the given URL based on this binding's parameters.
		/// </summary>
		/// <param name="url">The URL from which to extract parameter values.</param>
		/// <returns></returns>
		string[] ParameterValuesIn(IUrl url);
	}
}
