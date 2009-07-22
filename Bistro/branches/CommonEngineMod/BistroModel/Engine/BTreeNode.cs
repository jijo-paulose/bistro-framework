using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;

namespace BistroModel 
{
	/// <summary>
	/// Represents the node of a tree structure based
	/// on URLs/Bindings where each part of the URL is
	/// a separate branch of the tree.
	/// 
	/// Each node holds information on its binding and
	/// controllers and (if built) its methods. And also
	/// its parent node and child nodes.
	/// 
	/// The tree can be built by calling the internal
	/// AddBinding method from the root node.
	/// </summary>
	internal class BTreeNode : IMethodTreeNode {
		#region private
		BTreeNode _parent;
		List<BTreeNode> _children;
		IBinding _binding = null;
		BListNodeSet _listNodeSet = null;
		IMethod _method;
		string _name;
		#endregion

		#region construction
		internal BTreeNode() 
		{
			//implicit root
			_binding = null; //new BBinding();
			_name = ""; //_binding.Head;
			_parent = null;
			_children = new List<BTreeNode>();
		}
		internal BTreeNode(BTreeNode parent, IBinding binding)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			if (binding == null)
				throw new ArgumentNullException("binding");
			
			_name = binding.Head;
			if(binding.Length == 1)
				_binding = binding.BaseBinding;
			_parent = parent;
			_children = new List<BTreeNode>();
			AddBinding(binding.SubBinding);
		}
		#endregion

		#region public
		public string Name { get { return _name; } }
		public IMethodTreeNode Parent { get { return _parent; } }
		public List<IMethodTreeNode> Children { 
			get {
				List<IMethodTreeNode> list = new List<IMethodTreeNode>();
				foreach (IMethodTreeNode tn in _children)
					list.Add(tn);
				return list;
			} 
		}
		public IMethod Method {
			get {
				if (_method == null && _parent != null)
					return _parent.Method;
				return _method;
			}
		}
		public string Path {
			get {
				if (_parent == null)
					return Name;
				string p = _parent.Path;
				if (p.Length == 0)
					return Name;
				if (p.EndsWith("/"))
					return p + Name;
				return p + "/" + Name;
			}
		}
		public override string ToString() {
			return string.Format("Name={0}", Name);
		}
		#endregion

		#region internal
		internal void AddBinding(IBinding binding) {
			if (binding != null) {
				bool found = false;
				foreach (BTreeNode btreeNode in _children)
					if (btreeNode.Name == binding.Head) {
						found = true;
						if (binding.Length == 1 && btreeNode._binding == null)
							btreeNode._binding = binding.BaseBinding;
						else if (binding.Length == 1)
							throw new ApplicationException("BTree.Binding already contains a reference so it cannot be set to the subBinding - if this has happened it is a bug!");
						btreeNode.AddBinding(binding.SubBinding);
					}
				if (!found) {
					BTreeNode btreeNode = new BTreeNode(this, binding);
					_children.Add(btreeNode);
				}
			}
		}
		internal void BuildMethods() {
			BuildMethod();
			foreach (BTreeNode treenode in _children)
				treenode.BuildMethods();
		}
		/// <summary>
		/// Return the method that corresponds to
		/// the given URL.
		/// </summary>
		/// <param name="burl"></param>
		/// <returns>IMethod</returns>
		internal IMethod GetMethod(IUrl burl) {
			return WalkUrl(burl);
		}
		#endregion

		#region private methods
		bool IsWildCard(string item) {
			return (IsGlobalWildCard(item) || IsLocalWildCard(item));
		}
		bool IsLocalWildCard(string item) {
			return (item == "*" || (item.StartsWith("{") && item.EndsWith("}")));
		}
		bool IsGlobalWildCard(string item) {
			return (item == "?");
		}

		/// <summary>
		/// Walk the node tree and find matches.
		/// Used for building methods.
		/// </summary>
		/// <param name="burl"></param>
		/// <returns>List of matching nodes.</returns>
		List<BTreeNode> Walk(IUrl burl) {
			List<BTreeNode> list = new List<BTreeNode>();
			if (burl.IsWild)
				WalkWild(burl, list);
			else
				WalkNormal(burl, list);
			return list;
		}

		/// <summary>
		/// For IUrls that contain one or more
		/// wildcards.
		/// Walk the node tree and find matches.
		/// Add matching nodes to list.
		/// Used for building methods.
		/// </summary>
		/// <param name="burl"></param>
		/// <param name="list"></param>
		void WalkWild(IUrl burl, List<BTreeNode> list) {
			BTreeNode target = this;
			if (target._parent == null) { //root
				foreach (BTreeNode child in target._children)
					child.WalkWild(burl, list);
				return;
			}
			#region both global wildcards...
			//at end of burl...
			if (IsGlobalWildCard(target.Name) && IsGlobalWildCard(burl.Head) && burl.Length == 1) {
				target.AddUp(list);
				return;
			}

			//more burl and target has no children. No match.
			if (IsGlobalWildCard(target.Name) && IsGlobalWildCard(burl.Head) && target._children.Count == 0 && burl.Length > 1)
				return;

			//more burl and more target children, keep looking...
			if (IsGlobalWildCard(target.Name) && IsGlobalWildCard(burl.Head) && target._children.Count > 0 && burl.Length > 1) {
				IUrl bb = burl.SubUrl; //skip past the url's global wildcard...
				while (bb != null && bb.Length > 0) {//keep eating url's children until a match (if any) is found with the target.child...
					foreach (BTreeNode child in target._children) {
						if (child.Name == bb.Head)
							child.WalkWild(bb, list);
					}
					bb = bb.SubUrl;
				}
				return;
			}
			#endregion

			#region only burl is global wildcard
			//burl is a global wildcard and target is anything else. No match!
			if (IsGlobalWildCard(burl.Head))
				return;
			#endregion

			#region only target is global wildcard
			//at end of burl...
			if (IsGlobalWildCard(target.Name) && burl.Length == 1) {
				target.AddUp(list);
				return;
			}

			//more burl, keep looking...
			if (IsGlobalWildCard(target.Name)) {
				IUrl bb = burl;
				while (bb != null && bb.Length > 0) {//keep eating url's children until a match (if any) is found with the target.child...
					foreach (BTreeNode child in target._children) {
						if (child.Name == bb.Head)
							child.WalkWild(bb, list);
					}
					bb = bb.SubUrl;
				}
				return;
			}
			#endregion
			
			#region target is local wildcard or exact match
			//at end of burl...
			if ((target.Name == burl.Head || IsLocalWildCard(target.Name)) && burl.Length == 1) {
				target.AddUp(list);
				return;
			}

			//more burl, keep looking...
			if ((target.Name == burl.Head || IsLocalWildCard(target.Name)) && burl.Length > 1) {
				foreach (BTreeNode child in target._children)
					child.WalkWild(burl.SubUrl, list);
				return;
			}
			#endregion

			return;
		}

		/// <summary>
		/// For IUrls that do not contain any
		/// wildcards.
		/// Walk the node tree and find matches.
		/// Add matching nodes to list.
		/// Used for building methods.
		/// </summary>
		/// <param name="burl"></param>
		/// <param name="list"></param>
		bool WalkNormal(IUrl burl, List<BTreeNode> list) {
			BTreeNode target = this;
			if (target._parent == null) { //root!
				bool foundMatch = false;
				foreach (BTreeNode child in target._children)
					if (child.WalkNormal(burl, list))
						foundMatch = true;
				return foundMatch;
			}

			if ((target.Name == burl.Head || IsWildCard(target.Name)) && burl.Length == 1) { // Match upto here. (URL is shorter)
				target.AddUp(list);
				return true;
			}
			if ((target.Name == burl.Head || IsWildCard(target.Name)) && target._children.Count == 0) { // Complete target matches leading part of url. (URL may be longer)
				target.AddUp(list);
				return true;
			}
			if (IsGlobalWildCard(target.Name)) { // We have a global wildcard + children, keep walking... (URL is longer)
				bool foundMatch = false;
				IUrl bb = burl; //no need to worry about our url having a global wildcard here - remember which method we are in!
				while (bb != null && bb.Length > 0) {
					foreach (BTreeNode child in target._children) {
						if (child.Name == bb.Head)
							if (child.WalkNormal(bb, list))
								foundMatch = true;
					}
					bb = bb.SubUrl;
				}
				if (!foundMatch) //Nothing found below so match here. (target has implied trailing global wildcard.)
					target.AddUp(list);
				return true;
			}
			if (target.Name == burl.Head || IsLocalWildCard(target.Name)) { // We have a match + children, keep walking... (URL is longer)
				bool foundMatch = false;
				foreach (BTreeNode child in _children)
					if (child.WalkNormal(burl.SubUrl, list))
						foundMatch = true;
				if (!foundMatch) //Nothing found below so match here. (target has implied trailing global wildcard.)
					target.AddUp(list);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Add myself and parent chain to the list.
		/// </summary>
		/// <param name="list"></param>
		void AddUp(List<BTreeNode> list) {
			if (_parent != null)
				_parent.AddUp(list);
			list.Add(this);
		}
		
		/// <summary>
		/// Walk the node tree to match the
		/// given URL to a node and return
		/// the corresponding method.
		/// </summary>
		/// <param name="burl"></param>
		/// <returns>IMethod.</returns>
		IMethod WalkUrl(IUrl burl) {
			List<IMethod> methodList = new List<IMethod>();
			WalkUrl(burl, methodList);
			if(methodList.Count == 0)
				return null;

			if (methodList.Count == 1)
				return methodList[0];

			//There are multiple matches...
			//We may have more than 1 match (from wildcards... or partials) So pick the
			//most specific match and return that.
			//Look at each method's URL parts for the most exact match.
			for (int i = 0; i < burl.Length; i++) {
				int maxMatchValue = -1;
				for(int j=0;j < methodList.Count;j++){
					IMethod method = methodList[j];
					if(method != null){
						if (i < method.Url.Length) {
							int mpartMatchValue = GetMatchValue(method, i);
							if (mpartMatchValue > maxMatchValue)
								maxMatchValue = mpartMatchValue; //the highest matching level becomes the filter...
						}
					}
				}
				for(int j=0;j < methodList.Count;j++){ //now filter by the highest matching level determined above...
					IMethod method = methodList[j];
					if(method != null){
						if (maxMatchValue > -1) { //found some... (if maxMatchType == -1 then there is nothing to check against (binding urls are shorter).
							if (i < method.Url.Length) {
								int mpartMatchValue = GetMatchValue(method, i);
								if (mpartMatchValue != maxMatchValue)//if not most exact match
									methodList[j] = null; //logically remove from list by setting reference to null.
							}
							else {//this one is shorter than the others that remain so remove it...
								methodList[j] = null;
							}
						}
					}
				}
				if (maxMatchValue == -1)
					break;
			}
			foreach(IMethod method in methodList)
				if(method != null)
					return method;
			throw new ApplicationException("Programmer error!"); //should never reach here!
		}
		/// <summary>
		/// Assign a "match value" to a binding's url part.
		/// The higher the value, the more exact a match.
		/// 
		/// An exact match trumps all wildcards, but embedded
		/// wildcards trump trailing wildcards.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		int GetMatchValue(IMethod method, int index) {
			string mpart = method.Url.Parts[index];
			bool atEnd = method.Url.Length == (index + 1);
			int mpartMatchType = 0;
			if (mpart == "?" && atEnd) // this is the least exact match.
				mpartMatchType = 0;
			else if (mpart == "*" && atEnd) // a little better.
				mpartMatchType = 1;
			else if (mpart.StartsWith("{") && mpart.EndsWith("}") && atEnd) // slightly more specific than previous.
				mpartMatchType = 2; 
			else if (mpart == "?") // better than above.
				mpartMatchType = 3;
			else if (mpart == "*") // better than above.
				mpartMatchType = 4;
			else if (mpart.StartsWith("{") && mpart.EndsWith("}")) // slightly more specific than previous.
				mpartMatchType = 5; 
			else // this is an exact match.
				mpartMatchType = 6;
			return mpartMatchType;
		}
		bool WalkUrl(IUrl burl, List<IMethod> methodList) {
			BTreeNode target = this;
			if (target._parent == null) { //root!
				bool foundMatch = false;
				foreach (BTreeNode child in target._children)
					if (child.WalkUrl(burl, methodList))
						foundMatch = true;
				return foundMatch;
			}
			if ((target.Name == burl.Head || IsWildCard(target.Name)) && burl.Length == 1) { // Match up to here. (URL is shorter)
				methodList.Add(Method);
				return true;
			}
			if ((target.Name == burl.Head || IsWildCard(target.Name)) && target._children.Count == 0) { // Complete target matches leading part of url. (URL is longer)
				methodList.Add(Method);
				return true;
			}
			if (IsGlobalWildCard(target.Name)) { // We have a global wildcard + children, keep walking... (URL is longer)
				bool foundMatch = false;
				IUrl bb = burl;
				while (bb != null && bb.Length > 0) {
					foreach (BTreeNode child in target._children) {
						if (child.Name == bb.Head)
							if (child.WalkUrl(bb, methodList))
								foundMatch = true;
					}
					bb = bb.SubUrl;
				}
				if (!foundMatch) //If nothing was found below, match here as a trailing global wildcard is implied - so the remaining portion of the URL is implicitly matched.
					methodList.Add(Method);
				return true;
			}
			if ((target.Name == burl.Head) || IsLocalWildCard(target.Name)) { // We have a local match (or match on *) + children, keep walking... (URL is longer)
				bool foundMatch = false;
				foreach (BTreeNode child in _children)
					if (child.WalkUrl(burl.SubUrl, methodList))
						foundMatch = true;
				if (!foundMatch) //If nothing was found below, match here as a trailing global wildcard is implied - so the remaining portion of the URL is implicitly matched.
					methodList.Add(Method);
				return true;
			}
			return false;
		}
		
		void BuildMethod() {
			if (Path.Length == 0)
				return;
			if (_listNodeSet == null) {
				List<BTreeNode> treeNodes = Root.Walk(new BUrl(Path));
				
				//Get controllers...
				Dictionary<string, BListNode> bnodeHash = new Dictionary<string, BListNode>();
				foreach (BTreeNode tn in treeNodes) {
					if (tn._binding != null) {
						foreach (IControllerInfo ci in tn._binding.ControllerInfos) {
							if (ci != null) {
								BListNode currentListNode = null;
								if (bnodeHash.TryGetValue(ci.Name, out currentListNode)) {
									if (currentListNode.Binding.Length < tn._binding.Length)
										bnodeHash[ci.Name] = new BListNode(ci, tn._binding); //replace, we want most specific binding (the longer url) for a given controller...
								}
								else
									bnodeHash.Add(ci.Name, new BListNode(ci, tn._binding));
							}
						}
					}
				}
				//Hold list of nodes sorted by resource dependencies, priorities, bindtypes, security...
				_listNodeSet = new BListNodeSet(bnodeHash, Path);
				
				//Create the method with the sorted set of controller infos...
				if (_listNodeSet.Root == null)
					_method = null;
				else {
					List<BListNode> listNodes = _listNodeSet.Root.GetList();
					if (listNodes != null)
						_method = new BMethod(listNodes, _listNodeSet.Remainder, new BUrl(Path));
				}
			}
		}
		BTreeNode Root {
			get {
				if (_parent != null)
					return _parent.Root;
				return this;
			}
		}
		#endregion

		#region zzz
		/// <summary>
		/// For IUrls that contain one or more
		/// wildcards.
		/// Walk the node tree and find matches.
		/// Add matching nodes to list.
		/// Used for building methods.
		/// </summary>
		/// <param name="burl"></param>
		/// <param name="list"></param>
		void zzzWalkWild(IUrl burl, List<BTreeNode> list) {
			BTreeNode target = this;
			if (target._parent == null) { //root
				foreach (BTreeNode child in target._children)
					child.WalkWild(burl, list);
				return;
			}
			//target is a global wild card and has no children
			if (IsGlobalWildCard(target.Name) && target._children.Count == 0) {
				list.Add(target);
				return;
			}
			//target name matches and target has a trailing global wildcard (one child that is a global wild card) and we are at the end of the URL...
			if (target.Name == burl.Name && burl.Length == 1 && target._children.Count == 1 && IsGlobalWildCard(target._children[0].Name)) {
				list.Add(target);
				return;
			}
			//both have more children and are global wildcards...
			if (IsGlobalWildCard(target.Name) && IsGlobalWildCard(burl.Head) && target._children.Count > 0 && burl.Length > 1) {
				IUrl bb = burl.SubUrl; //skip past the url's global wildcard...
				while (bb != null && bb.Length > 0) {//keep eating url's children until a match (if any) is found with the target.child...
					foreach (BTreeNode child in target._children) {
						if (child.Name == bb.Head)
							child.WalkWild(bb, list);
					}
					bb = bb.SubUrl;
				}
				return;
			}
			//target has more children and is a global wildcard...
			if (IsGlobalWildCard(target.Name) && target._children.Count > 0) {
				IUrl bb = burl;
				while (bb != null && bb.Length > 0) {//keep eating url's children until a match (if any) is found with the target.child...
					foreach (BTreeNode child in target._children) {
						if (child.Name == bb.Head)
							child.WalkWild(bb, list);
					}
					bb = bb.SubUrl;
				}
				return;
			}
			//target is either an exact match or a local wildcard and we are at the end of both...
			if ((target.Name == burl.Head || IsLocalWildCard(target.Name)) && target._children.Count == 0 && burl.Length == 1) {
				list.Add(target);
				return;
			}
			//target is either an exact match or a local wildcard and we have more of both...
			if ((target.Name == burl.Head || IsLocalWildCard(target.Name)) && target._children.Count > 0 && burl.Length > 1) {
				foreach (BTreeNode child in target._children)
					child.WalkWild(burl.SubUrl, list);
				return;
			}

			return;
		}
		#endregion
	}
}
