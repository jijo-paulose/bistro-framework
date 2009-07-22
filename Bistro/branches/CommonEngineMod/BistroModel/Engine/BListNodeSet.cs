using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;

namespace BistroModel 
{
	/// <summary>
	/// This class holds a set of BListNodes, which together
	/// make up the internal structure of a Bistro Method. 
	/// This class builds the list of nodes as a linked list 
	/// by inserting into that list starting with strict 
	/// criteria and progressing to less strict criteria. 
	/// (Such as first trying to insert using both Requires 
	/// and DependsOn resource dependencies and then 
	/// dropping the DependsOn if needed, etc.)
	/// 
	/// See the BListNode class for more information.
	/// </summary>
	internal class BListNodeSet {
		#region private
		BListNode _root;
		List<BListNode> _remainder;
		bool _ignoringAdditionalSorts;
		#endregion

		#region contruction
		public BListNodeSet(Dictionary<string, BListNode> bnodeHash, string path)
		{
			if (bnodeHash == null)
				throw new ArgumentNullException("bnodeHash");

			_remainder = new List<BListNode>(bnodeHash.Values);
			_remainder.Sort(); //Sorts by [security, bindtype, priority] (all security controllers are Before bindtypes.)
			if (_remainder.Count > 0) {
				_root = FindSuitableRoot(_remainder, path);
				_remainder.Remove(_root);
				int rcount = 0;
				while (rcount != _remainder.Count && _remainder.Count > 0) {
					rcount = _remainder.Count;
					MakeSetFull();
				}
			}
			
			if (_remainder.Count > 0)
				throw new ApplicationException(string.Format("Controller dependencies cannot be resolved at method {0}.\n{1}", path, GetExceptionString()));
		}
		#endregion

		#region public
		public BListNode Root { get { return _root; } }
		public List<BListNode> Remainder { get { return _remainder; } }
		#endregion

		#region private methods
		BListNode FindSuitableRoot(List<BListNode> list, string path) {
			//First, look for a node without any Requires or DependsOn...
			foreach (BListNode bn in list) {
				bool hasNoRequires = bn.ControllerInfo.Resources.HasNo<RequiresAttribute>();
				bool hasNoDependsOn = bn.ControllerInfo.Resources.HasNo<DependsOnAttribute>();
				if (hasNoRequires && hasNoDependsOn)
					return bn;
			}
			//Failing the above, look for a node without any Requires...
			foreach (BListNode bn in list) {
				bool hasNoRequires = bn.ControllerInfo.Resources.HasNo<RequiresAttribute>();
				if (hasNoRequires)
					return bn;
			}
			//Could not find any non-dependent node:
			throw new ApplicationException(string.Format("Controller dependencies cannot be resolved at method {0}.\n{1}", path, GetExceptionString()));
		}
		string GetExceptionString() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Controllers chained:");
			BListNode child = _root;
			while (child != null) {
				sb.AppendLine(child.ExceptionString());
				sb.AppendLine(child.ControllerInfo.ToString());
				child = child.Child;
			}
			sb.AppendLine("Controllers remaining:");
			foreach(BListNode bn in _remainder)
				sb.AppendLine(bn.ControllerInfo.ToString());
			return sb.ToString();
		}
		/// <summary>
		///Full dependency match with both dependsOn and requires.
		/// </summary>
		void MakeSetFull() {
			BListNode[] nodes = _remainder.ToArray();
			foreach (BListNode bn in nodes) {
				if (_root.InsertFull(bn)) {
					_remainder.Remove(bn);
					_root = _root.GetRoot();
				}
			}
			if (nodes.Length > _remainder.Count && _remainder.Count > 0)
				MakeSetFull();
			else if (_remainder.Count > 0)
				MakeSetPartial();
		}
		/// <summary>
		/// Dependency match ignoring dependsOn, but respecting requires.
		/// </summary>
		void MakeSetPartial() {
			BListNode[] nodes = _remainder.ToArray();
			foreach (BListNode bn in nodes) {
				if (_root.InsertPartial(bn)) {
					_remainder.Remove(bn);
					_root = _root.GetRoot();
					break;
				}
			}
			if (nodes.Length == _remainder.Count) { //nothing was inserted.
				if (!_ignoringAdditionalSorts) { //don't recurse back into this section.
					_ignoringAdditionalSorts = true;
					_root.IgnorePriority = true;
					MakeSetFull();
					if (nodes.Length == _remainder.Count) { //still nothing was inserted
						_root.IgnoreBindType = true;
						MakeSetFull();
					}
					_root.IgnorePriority = false;
					_root.IgnoreBindType = false;
					_ignoringAdditionalSorts = false;
				}
			}
		}
		#endregion
	}
}
