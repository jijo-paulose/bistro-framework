using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BistroModel {
	/// <summary>
	/// A reflection helper class.
	/// </summary>
	class BReflection {
		/// <summary>
		/// Iterates over a list of attributes taken from the targetType that are
		/// of type attributeType, with the supplied "inherit" flag.
		/// </summary>
		/// <typeparam name="T">The type of the parameter to iterate over. This type parameter will be passed a the parameter to the nonEmpty delegate</typeparam>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <param name="nonEmpty">The statement to invoke on the elements of a non-empty result set.</param>
		/// <param name="empty">The statement to invoke on an empty result set.</param>
		public static void IterateAttributes<T>(MemberInfo targetType, bool inherit, Action<T> nonEmpty, Action empty) {
			T[] attribs = targetType.GetCustomAttributes(typeof(T), inherit) as T[];

			if (attribs.Length > 0)
				foreach (T attribute in attribs)
					nonEmpty(attribute);
			else if (empty != null)
				empty();
		}
		/// <summary>
		/// Determines whether the specified target type is marked by the attribute.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="markerAttribute">The marker attribute.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns>
		/// 	<c>true</c> if the specified target type is marked; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsMarked(MemberInfo targetType, Type markerAttribute) {
			return IsMarked(targetType, markerAttribute, true);
		}
		public static bool IsMarked(MemberInfo targetType, Type markerAttribute, bool inherit) {
			return targetType.GetCustomAttributes(markerAttribute, inherit).Length > 0;
		}
		/// <summary>
		/// Iterates over a list of members taken off of the target parameter based on the binding flags supplied
		/// </summary>
		/// <param name="target">the target type</param>
		/// <param name="flags">the binding flags to filter the members by</param>
		/// <param name="nonEmpty">the statement to apply to each found member</param>
		public static void IterateMembers(Type target, BindingFlags flags, Action<MemberInfo> nonEmpty) {
			foreach (MemberInfo info in target.GetMembers(flags))
				nonEmpty(info);
		}
	}
}
