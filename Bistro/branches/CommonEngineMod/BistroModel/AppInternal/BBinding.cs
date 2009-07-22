using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BistroApi;

namespace BistroModel 
{
	/// <summary>
	/// Represents a Bistro binding of a URL (which may contain
	/// allowed wildcard characters) to a set of controller infos.
	/// </summary>
	internal class BBinding : IBinding {
		#region private
		BUrl _burl;
		List<IControllerInfo> _controllerInfos;
		List<string> _parameters;
		List<int> _parameterIndexes;
		BBinding _baseBinding;
		#endregion

		#region construction
		public BBinding() {
			_burl = new BUrl();
			_controllerInfos = new List<IControllerInfo>();
			GetParameters();
		}
		public BBinding(BUrl burl) {
			if (burl == null)
				throw new ArgumentNullException("burl");
			_burl = burl;
			ValidateUrl(_burl);
			_controllerInfos = new List<IControllerInfo>();
			GetParameters();
		}
		public BBinding(BUrl burl, BBinding baseBinding) {
			if (burl == null)
				throw new ArgumentNullException("burl");
			if (baseBinding == null)
				throw new ArgumentNullException("baseBinding");
			_burl = burl;
			ValidateUrl(_burl);
			_baseBinding = baseBinding;
		}
		public static BBinding[] Create(BindAttribute bindAttribute) {
			if(bindAttribute == null)
				throw new ArgumentNullException("bindAttribute");
			BUrl burl = new BUrl(bindAttribute.Target);
			return BBinding.Create(burl);
		}
		static BBinding[] Create(BUrl burl) {
			if (burl == null)
				throw new ArgumentNullException("burl");
			BBinding[] bindings;
			if (burl.Head == "ALL/") {
				bindings = new BBinding[4];
				bindings[0] = new BBinding(new BUrl("GET/" + burl.Tail));
				bindings[1] = new BBinding(new BUrl("POST/" + burl.Tail));
				bindings[2] = new BBinding(new BUrl("PUT/" + burl.Tail));
				bindings[3] = new BBinding(new BUrl("DELETE/" + burl.Tail));
				return bindings;
			}
			bindings = new BBinding[1];
			bindings[0] = new BBinding(burl);
			return bindings;
		}
		#endregion

		#region public
		public IBinding BaseBinding { 
			get { 
				if(_baseBinding != null)
					return _baseBinding;
				return this;
			} 
		}
		public string Name { get { return _burl.Name; } }
		public string Head { get { return _burl.Head; } }
		public HttpAction HttpAction { get { return _burl.HttpAction; } }
		public string[] Parts {	get {	return _burl.Parts;	}	}
		public string[] SubParts { get { return _burl.SubParts; } }
		public IBinding SubBinding { 
			get { 
				return _burl.Length > 1 ? new BBinding(new BUrl(SubParts), (_baseBinding ?? this)) : null; 
			} 
		}
		public string this[int index] { get { return _burl[index]; } }
		public int Length { get { return _burl.Length; } }
		public void AddControllerInfo(IControllerInfo controllerInfo) {
			if (controllerInfo == null)
				throw new ArgumentNullException("controllerInfo");
			if (_controllerInfos.Contains(controllerInfo))
				throw new ApplicationException(string.Format("Binding {0} already contains Controller {1}.", Name, controllerInfo.Name));
			_controllerInfos.Add(controllerInfo);
			
		}
		public IEnumerable<IControllerInfo> ControllerInfos { 
			get {
				if (_controllerInfos == null)
					yield return null;
				else
					foreach(IControllerInfo bc in _controllerInfos)
						yield return bc; 
			} 
		}
		public string[] ParameterNames { get { return _parameters.ToArray(); } }
		public string[] ParameterValuesIn(IUrl url) {
			List<string> valueList = new List<string>();
			Dictionary<string, IQueryStringItem> qItemHash = new Dictionary<string, IQueryStringItem>();
			foreach (IQueryStringItem qitem in url.QueryStringItems)
				qItemHash.Add(qitem.Name, qitem);
			foreach (int i in _parameterIndexes) {
				if (url.Length > i)
					valueList.Add(url[i]);
				else { //try to add value from query string... This leaves open the possibility of holes in the list so see a few lines below...
					IQueryStringItem qitem = null;
					if (qItemHash.TryGetValue(_parameters[i], out qitem))
						valueList.Add(qitem.Value);
					else
						valueList.Add(null); //can't leave holes in the list since it is positional to ParameterNames.
				}
			}
			return valueList.ToArray();
		}
		public override string ToString() {
			return string.Format("Name={0}", Name);
		}
		#endregion

		#region private
		void ValidateUrl(BUrl burl) {
			if (burl.Head == "ALL/")
				throw new ArgumentException("A binding cannot be created from an url starting with \"ALL/\".", "burl");
		}
		void GetParameters() {
			_parameters = new List<string>();
			_parameterIndexes = new List<int>();
			int i = 0;
			foreach (string p in Parts) {
				if (p.StartsWith("{") && p.EndsWith("}") && p.Length > 2) {
					_parameters.Add(p.Substring(1, p.Length - 2));
					_parameterIndexes.Add(i);
				}
				i++;
			}
		}
		#endregion
	}
}
