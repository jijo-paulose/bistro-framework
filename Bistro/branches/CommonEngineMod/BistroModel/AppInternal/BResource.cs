using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BistroApi;

namespace BistroModel 
{
	/// <summary>
	/// Represents a controller resource.
	/// </summary>
	internal class BResource : IResource {
		#region private fields
		string _name; //namespaces?
		string _dataType; //this s/be the fullname.
		bool _isParameter;
		MemberInfo _memberInfo;
		Dictionary<Type, IResourceAttribute> _attributes = new Dictionary<Type, IResourceAttribute>();
		#endregion

		#region construction
		public BResource() 
		{
		}
		public BResource(string name) 
		{
			_name = name;
		}
		public BResource(string name, string dataType) 
		{
			_name = name;
			_dataType = dataType;
		}
		public BResource(MemberInfo memberInfo) 
		{
			if (memberInfo == null)
				throw new ArgumentNullException("memberInfo");
			_name = memberInfo.Name;
			_dataType = memberInfo.DeclaringType.ToString();
			_memberInfo = memberInfo;
		}
		#endregion

		#region public
		public string Name { get { return _name; } set { _name = value; } }
		public string DataType { get { return _dataType; } set { _dataType = value; } }
		public bool IsParameter { get { return _isParameter; } set { _isParameter = value; } }
		public string GetAlias<TResourceAttribute>() where TResourceAttribute : IResourceAttribute
		{
			IResourceAttribute ra = null;
			if (_attributes.TryGetValue(typeof(TResourceAttribute), out ra))
				return ra.Name ?? this.Name;
			return this.Name;
		}
		public bool HasAttribute<TResourceAttribute>() where TResourceAttribute : IResourceAttribute
		{
			return _attributes.ContainsKey(typeof(TResourceAttribute));
		}
		public MemberInfo MemberInfo { get { return _memberInfo; } set { _memberInfo = value; } }
		public override bool Equals(object obj) {
			if(!(obj is BResource))
				return false;
			BResource br = (BResource)obj;
			//return (br.Name == Name && br.DataType == DataType);
			return (br.Name == Name);
		}
		public override int GetHashCode() {
			return Name.GetHashCode();
			//return string.Format("{0}:{1}:{2}", Name, DataType).GetHashCode();
		}
		public override string ToString() {
			return Name;
		}
		#endregion

		#region non-public
		internal void AddAttribute<TResourceAttribute>(IResourceAttribute attribute) where TResourceAttribute : IResourceAttribute {
			if (_attributes.ContainsKey(typeof(TResourceAttribute)))
				throw new ArgumentException(string.Format("Resource {0} already contains an attribute of type {1}. Another cannot be added.", this.Name, typeof(TResourceAttribute)), "attribute");
			_attributes.Add(typeof(TResourceAttribute), attribute);
		}
		#endregion
	}
}
