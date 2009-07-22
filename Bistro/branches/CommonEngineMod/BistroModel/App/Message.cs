using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
namespace BistroModel
{
	/// <summary>
	/// Provides messages based on attributed enumerations.
	/// </summary>
	public class Message
	{
		#region private
		string _key;
		string _nameSpace;
		string _enumType;
		string _name;
		string _message;
		Enum _enum;
		bool _buildKey = true;
		#endregion

		#region construction
		public Message() { }
		public Message(Enum e) : this(e, null, true) { }
		public Message(Enum e, bool grabDefault) : this(e, null, grabDefault) { }
		public Message(Enum e, string message) : this(e, message, false) { }
		public Message(Enum e, string message, bool grabDefault) {
			string tmp = e.GetType().FullName;
			int i = tmp.LastIndexOf('.');
			_enum = e;
			if(i == -1) // There is no Namespace.
				Init("!no_namespace", tmp, e.ToString(), message, grabDefault);
			else
				Init(tmp.Substring(0, i), tmp.Substring(i + 1), e.ToString(), message, grabDefault);
		}
		private void Init(string nameSpace, string enumType, string name, string message, bool grabDefault) {
			_nameSpace = nameSpace;
			_enumType = enumType;
			_name = name;
			_message = message;
			if (grabDefault)
				GrabDefaultMessage();
		}
		private void GrabDefaultMessage() {
			Enum e = this.Enum;
			FieldInfo fi = e.GetType().GetField(e.ToString());
			DefaultMessageAttribute[] defaultMessages = (DefaultMessageAttribute[])fi.GetCustomAttributes(typeof(DefaultMessageAttribute), false);
			foreach (DefaultMessageAttribute dm in defaultMessages) {
				Message msg = new Message(e, dm.Message);
				if (msg.Key == this.Key)
					_message = msg._message;
			}
		}
		#endregion

		#region public interface
		public string Key {
			get {
				if (_key == null)
					BuildKey();
				return _key;
			}
		}
		public string NameSpace { get { return _nameSpace; } set { _nameSpace = value; BuildKey(); } }
		public string EnumType { get { return _enumType; } set { _enumType = value; BuildKey(); } }
		public string Name { get { return _name; } set { _name = value; BuildKey(); } }
		public string MessageText { get { return _message; } set { _message = value;} }
		public Enum Enum { get { return _enum; } }
		public string Create(params object[] list) {
			return string.Format(MessageText, list);
		}
		public static string GetDefault(Enum e, params object[] list) 
		{
			Message msg = new Message(e);
			return msg.Create(list);
		}
		public override string ToString() {
			if (Key != null)
				return Key;
			return base.ToString();
		}
		#endregion

		#region private methods
		private void BuildKey() {
			if (_buildKey) {
				if (_nameSpace != null && _enumType != null && _name != null)
					_key = _nameSpace + "." + _enumType + "." + _name;
			}
		}
		#endregion
	}
}
