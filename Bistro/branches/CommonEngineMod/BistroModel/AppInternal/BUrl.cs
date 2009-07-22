using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;

namespace BistroModel 
{
	/// <summary>
	/// Represents a URL.
	/// Allows wildcard characters for use in Bindings.
	/// </summary>
	internal class BUrl : IUrl {
		#region private
		HttpAction _httpAction = HttpAction.ALL;
		string _name;
		string[] _parts;
		bool _isWild;
		BQueryStringItem[] _queryStringItems;
		#endregion

		#region construction
		public BUrl() {
			_name = ""; //root.
			_parts = new string[0];
		}
		public BUrl(string url) : this(url, HttpAction.ALL){}
		public BUrl(string url, HttpAction defaultAction) {
			string tmpUrl;
			if (url == null)
				tmpUrl = "";
			else
				tmpUrl = url.Trim();

			string testUrl = tmpUrl.ToUpper();

			// the url can be specified as "VERB url", "VERB/url", or "url" (defaults to ALL)
			if (testUrl.StartsWith("ALL")) {
				_httpAction = HttpAction.ALL;
				tmpUrl = "ALL/" + tmpUrl.Substring(3).Trim(' ', '/', '\t');
			}
			else if (testUrl.StartsWith("GET")) {
				_httpAction = HttpAction.GET;
				tmpUrl = "GET/" + tmpUrl.Substring(3).Trim(' ', '/', '\t');
			}
			else if (testUrl.StartsWith("POST")) {
				_httpAction = HttpAction.POST;
				tmpUrl = "POST/" + tmpUrl.Substring(4).Trim(' ', '/', '\t');
			}
			else if (testUrl.StartsWith("PUT")) {
				_httpAction = HttpAction.PUT;
				tmpUrl = "PUT/" + tmpUrl.Substring(3).Trim(' ', '/', '\t');
			}
			else if (testUrl.StartsWith("DELETE")) {
				_httpAction = HttpAction.DELETE;
				tmpUrl = "DELETE/" + tmpUrl.Substring(6).Trim(' ', '/', '\t');
			}
			else {
				_httpAction = defaultAction;
				tmpUrl = _httpAction.ToString() + "/" + tmpUrl.Trim('/');
			}

			_parts = tmpUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			_parts[0] = _parts[0] + "/";
			Validate(DeQueue(_parts));
			_parts = RemoveTrailingGlobalWildcard(ExtractQueryString(_parts));
			_name = Join(_parts);
		}
		internal BUrl(string[] url) {
			if (url == null)
				throw new ArgumentNullException("url");
			if (url.Length == 0)
				throw new ArgumentOutOfRangeException("url", "Must not be empty.");
			
			// This could be a "suburl" so don't force
			// an HTTP Verb at the beginning...
			string[] curl = Copy(url);
			string[] tmpurl;
			if (curl[0] == "ALL/") {
				_httpAction = HttpAction.ALL;
				tmpurl = DeQueue(curl);
			}
			else if (curl[0] == "GET/") {
				_httpAction = HttpAction.GET;
				tmpurl = DeQueue(curl);
			}
			else if (curl[0] == "POST/") {
				_httpAction = HttpAction.POST;
				tmpurl = DeQueue(curl);
			}
			else if (curl[0] == "PUT/") {
				_httpAction = HttpAction.PUT;
				tmpurl = DeQueue(curl);
			}
			else if (curl[0] == "DELETE/") {
				_httpAction = HttpAction.DELETE;
				tmpurl = DeQueue(curl);
			}
			else {
				tmpurl = curl;
			}
			
			Validate(tmpurl); // using tmpurl to validate w/out http verb + "/".
			
			_parts = RemoveTrailingGlobalWildcard(ExtractQueryString(curl));
			_name = Join(_parts);
		}
		#endregion

		#region public
		public string Name { get { return _name; } }
		public string Head { get { return _parts.Length > 0 ? _parts[0] : _name; } }
		public string Tail { get { return Join(SubParts); } }
		public string[] Parts {
			get {
				string[] ret = new string[_parts.Length];
				_parts.CopyTo(ret, 0);
				return ret;
			}
		}
		public string[] SubParts { get { return DeQueue(_parts); } }
		public IUrl SubUrl { get { return _parts.Length > 1 ? new BUrl(SubParts) : null; } }
		public string this[int index] { get { return _parts[index]; } }
		public int Length { get { return _parts.Length; } }
		public HttpAction HttpAction { get { return _httpAction; } }
		public override string ToString() {
			return string.Format("Name={0}", Name);
		}
		public bool IsWild { get { return _isWild; } }
		public IQueryStringItem[] QueryStringItems { get { return _queryStringItems; } }
		#endregion

		#region private methods
		void Validate(string[] parts) {
			string prior = null;
			foreach (string s in parts) {
				if (s == null)
					throw new ArgumentOutOfRangeException("url", "Element was null.");
				if (s.Trim().Length == 0)
					throw new ArgumentOutOfRangeException("url", "Element was empty or contained only whitespace.");
				if (s.Contains('/'))
					throw new ArgumentOutOfRangeException("url", "Element contains '/'.");
				if (s.Contains(' '))
					throw new ArgumentOutOfRangeException("url", "Element contains ' ' (space character).");
				if (s.Contains('\t'))
					throw new ArgumentOutOfRangeException("url", "Element contains '\t' (tab character).");
				if (IsGlobalWildcard(prior) && IsLocalWildcard(s))
					throw new ArgumentOutOfRangeException("url", "Consecutive URL elements of ? followed by * are not allowed.");
				if (PartIsWild(s)) //*Explicit* trailing wildcards will be removed, but we still must mark these burls as "wild".
					_isWild = true;  //This is true even though *implied* trailing wildcards do not result in burls being marked as "wild".
				                   //This is by design. This is critical so that the BTreeNode.Walk() method directs burls based on 
				                   //tree locations with global wild cards to BTreeNode.WalkWild() instead of BTreeNode.WalkNormal().
			}
		}
		bool PartIsWild(string part){return (IsGlobalWildcard(part) || IsLocalWildcard(part));}
		bool IsGlobalWildcard(string part) { return part == "?"; }
		bool IsLocalWildcard(string part) { return (part == "*" || (part.StartsWith("{}") && part.EndsWith("}"))); }
		string Join(string[] array) {
			StringBuilder sb = new StringBuilder();
			string sep = "";
			foreach (string s in array)
				if (s.EndsWith("/")) {
					sb.Append(s);
				}
				else {
					sb.Append(sep + s);
					if (sep.Length == 0)
						sep = "/";
				}
			return sb.ToString();
		}
		/// <summary>
		/// Returns a new array containing all of the elements
		/// of the input array except the first element.
		/// </summary>
		/// <param name="array"></param>
		/// <returns>A new array</returns>
		string[] DeQueue(string[] array) {
			if (array == null)
				return null;
			if (array.Length == 0)
				return new string[0];
			string[] newArray = new string[array.Length - 1];
			if (newArray.Length == 0)
				return newArray;
			Array.Copy(array, 1, newArray, 0, newArray.Length);
			return newArray;
		}
		/// <summary>
		/// Returns a new array containing all of the elements
		/// of the input array except the last element.
		/// </summary>
		/// <param name="array"></param>
		/// <returns>A new array</returns>
		string[] Pop(string[] array) {
			if (array == null)
				return null;
			if (array.Length == 0)
				return new string[0];
			string[] newArray = new string[array.Length - 1];
			if (newArray.Length == 0)
				return newArray;
			Array.Copy(array, 0, newArray, 0, newArray.Length);
			return newArray;
		}
		/// <summary>
		/// Returns a new array containing all of the elements
		/// of the input array.
		/// </summary>
		/// <param name="array"></param>
		/// <returns>A new array</returns>
		string[] Copy(string[] array) {
			if (array == null)
				return null;
			if (array.Length == 0)
				return new string[0];
			string[] newArray = new string[array.Length];
			array.CopyTo(newArray, 0);
			return newArray;
		}
		string[] ExtractQueryString(string[] parts) {
			string last = parts[parts.Length - 1];
			if (last.StartsWith("?") && last.Length > 1) {
				string[] qparts = last.Split('&');
				_queryStringItems = new BQueryStringItem[qparts.Length];
				int i = 0;
				foreach (string qpart in qparts)
					_queryStringItems[i++] = new BQueryStringItem(qpart);
				return Pop(parts);
			}
			else
				_queryStringItems = new BQueryStringItem[0];
			return parts;
		}
		string[] RemoveTrailingGlobalWildcard(string[] parts) {
			//Trailing global wildcards are implied for all bindings (and that's
			//where this burl is going if it contains wildcards...) so just
			//remove them.
			if (parts.Length > 1 && IsGlobalWildcard(parts[parts.Length - 1]))
				return Pop(parts);
			return parts;
		}
		#endregion
	}
}
