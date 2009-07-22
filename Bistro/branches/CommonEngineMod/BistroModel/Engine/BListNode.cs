using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;

namespace BistroModel 
{
	/// <summary>
	/// This node is used to make up a set of nodes, sorted
	/// in the proper order which make up a Bisto method.
	/// It contains a controllerinfo + binding combination. 
	/// This is used to make a linked list when combined with
	/// parent and child nodes. A node inserted at the root of
	/// (first node of) the list will travel down the list to
	/// find the correct insertion point (if any). The InsertFull
	/// method looks at both Requires and DependsOn resources 
	/// when making insertion decisions. The InsertPartial method
	/// looks at Requires. Priority and BindType can also be
	/// included or excluded from the insertion decision. The
	/// BListNodeSet class builds this linked list by trying
	/// insertions from most to least strict.
	/// </summary>
	internal class BListNode : IComparable<BListNode>, IMethodPart {
		#region private
		BListNode _parent = null;
		BListNode _child = null;
		IControllerInfo _controllerInfo;
		IBinding _binding;
		bool _ignorePriority;
		bool _ignoreBindType;
		#endregion

		#region construction
		public BListNode(IControllerInfo controller, IBinding binding)
		{
			if (controller == null)
				throw new ArgumentNullException("controller");
			if (binding == null)
				throw new ArgumentNullException("binding");

			_controllerInfo = controller;
			_binding = binding;
		}
		#endregion

		#region public
		
		public BListNode Child { get { return _child; } set { _child = value; } }
		public IControllerInfo ControllerInfo { get { return _controllerInfo; } }
		public IBinding Binding { get { return _binding; } }
		public BListNode GetRoot() {
			if (_parent == null)
				return this;
			return _parent.GetRoot();
		}
		public List<BListNode> GetList() {
			List<BListNode> list = new List<BListNode>();
			AddToList(list);
			return list;
		}
		
		public bool Has(IResource br) {
			if (ControllerInfo.Has(br))
				return true;
			if (_parent != null)
				return _parent.Has(br);
			return false;
		}
		public bool Has(IResource[] brList) {
			if (brList == null)
				return true;
			foreach (IResource br in brList) {
				if (!Has(br))
					return false;
			}
			return true;
		}
		public int Count(IResource[] brList) {
			int count = 0;
			if (brList == null)
				return count;
			foreach (IResource br in brList) {
				if (Has(br))
					count++;
			}
			return count;
		}
		/// <summary>
		/// Attempt to insert the given node in correct order taking 
		/// into account the CompareTo method and with precedence to
		/// resource dependencies - Requires and also DependsOn.
		/// </summary>
		/// <param name="bn"></param>
		/// <returns></returns>
		public bool InsertFull(BListNode bn) {
			// Check if order is indeterminant...
			// If it is, we will pick an order based on controller name
			// to allow easy unit testing where we check the output order.
			//
			// We can also potentially assign some kind of "parallel ID"
			// to all indeterminately ordered items of the same group 
			// (meaning they follow one another) allowing those items to 
			// be executed simultaneously across multiple processors/cores/boxes...
			if (CanInsertBeforeFull(bn) && CanInsertAfterFull(bn)) {
				InsertIndeterminant(bn);
				return true;
			}
			// Insert before?
			if (CanInsertBeforeFull(bn)) {
				InsertBefore(bn);
				return true;
			}
			// Insert after?
			if (CanInsertAfterFull(bn)) {
				InsertAfter(bn);
				return true;
			}
			// Try child...
			if (Child != null)
				return Child.InsertFull(bn);
			return false;
		}
		/// <summary>
		/// Attempt to insert the given node in correct order taking 
		/// into account the CompareTo method and with precedence to
		/// only the resource dependency Requires. The DependsOn 
		/// resource is ignored.
		/// </summary>
		/// <param name="bn"></param>
		/// <returns></returns>
		public bool InsertPartial(BListNode bn) {
			return InsertPartial(bn, 0);
		}
		bool InsertPartial(BListNode bn, int parentCountDependsOnMet) {
			// Check if order is indeterminant...
			// If it is, we will pick an order based on controller name
			// to allow easy unit testing where we check the output order.
			//
			// We can also potentially assign some kind of "parallel ID"
			// to all indeterminately ordered items of the same group 
			// (meaning they follow one another) allowing those items to 
			// be executed simultaneously across multiple processors/cores/boxes...

			//Get count of DependsOn dependencies met to allow insertion at
			//maximum point.
			int countDependsOnMet = Count(bn.ControllerInfo.Resources.GetBy<DependsOnAttribute>());
			
			if (CanInsertBeforePartial(bn) && CanInsertAfterPartial(bn)) {
				if (Child != null) {
					if (Child.InsertPartial(bn, countDependsOnMet)) {
						return true;
					}
				}
				if (countDependsOnMet > parentCountDependsOnMet) {
					InsertAfter(bn);
					return true;
				}
				if (countDependsOnMet == parentCountDependsOnMet && countDependsOnMet > 0)
					return false;
				InsertIndeterminant(bn);
				return true;
			}
			// Insert before?
			if (CanInsertBeforePartial(bn)) {
				if (countDependsOnMet == parentCountDependsOnMet && countDependsOnMet > 0)
					return false;
				InsertBefore(bn);
				return true;
			}
			// Insert after?
			if (CanInsertAfterPartial(bn)) {
				if (Child != null) {
					if (Child.InsertPartial(bn, countDependsOnMet)) {
						return true;
					}
				}
				if (countDependsOnMet == parentCountDependsOnMet && countDependsOnMet > 0)
					return false;
				InsertAfter(bn);
				return true;
			}
			// Try child...
			if (Child != null)
				return Child.InsertPartial(bn, countDependsOnMet);
			return false;
		}
		public override string ToString() {
			return string.Format("[Controller:{0}, Binding:{1}, IsSecurity:{2}, Priority:{3}, BindType:{4}]", _controllerInfo.Name, _binding.Name, _controllerInfo.IsSecurity, _controllerInfo.GetPriority(_binding), _controllerInfo.GetBindType(_binding));
		}
		public string ExceptionString() {
			return string.Format("  {0}<->{1}<->{2}", _parent == null ? "null" : _parent._controllerInfo.Name, _controllerInfo.Name, _child == null ? "null" : _child._controllerInfo.Name);
		}
		#endregion

		#region private
		BListNode Parent { get { return _parent; } set { _parent = value; } }
		void AddToList(List<BListNode> list) {
			list.Add(this);
			if (_child != null)
				_child.AddToList(list);
		}
		void InsertBefore(BListNode bn) {
			//string za_bn = string.Format("{0}", bn.ExceptionString());
			//string zb_this = string.Format("{0}", this.ExceptionString());
			
			bn.Parent = this.Parent;
			if (bn.Parent != null)
				bn.Parent.Child = bn;
			this.Parent = bn;
			bn.Child = this;
			
			//string zc_bn = string.Format("{0}", bn.ExceptionString());
			//string zd_this = string.Format("{0}", this.ExceptionString());
		}
		void InsertAfter(BListNode bn) {
			//string za_this = string.Format("{0}", this.ExceptionString());
			//string zb_bn = string.Format("{0}", bn.ExceptionString());

			bn.Parent = this;
			bn.Child = this.Child;
			if (bn.Child != null)
				bn.Child.Parent = bn;
			this.Child = bn;

			//string zc_this = string.Format("{0}", this.ExceptionString());
			//string zd_bn = string.Format("{0}", bn.ExceptionString());
		}
		void InsertIndeterminant(BListNode bn) {
			if (bn.ControllerInfo.Name.CompareTo(this.ControllerInfo.Name) < 0)
				InsertBefore(bn);
			else
				InsertAfter(bn);
		}
		bool CanInsertBeforeFull(BListNode bn) {
			bool hasNoRequires = bn.ControllerInfo.Resources.HasNo<RequiresAttribute>();
			bool hasNoDependsOn = bn.ControllerInfo.Resources.HasNo<DependsOnAttribute>();
			bool meetsOtherInsertBeforeCriteria = MeetsOtherInsertBeforeCriteria(bn);
			return (hasNoRequires && hasNoDependsOn && meetsOtherInsertBeforeCriteria);
		}
		bool CanInsertAfterFull(BListNode bn) {
			bool thisHasRequired = Has(bn.ControllerInfo.Resources.GetBy<RequiresAttribute>());
			bool thisHasDepends = Has(bn.ControllerInfo.Resources.GetBy<DependsOnAttribute>());
			bool meetsOtherInsertAfterCriteria = MeetsOtherInsertAfterCriteria(bn);
			return (thisHasRequired && thisHasDepends && meetsOtherInsertAfterCriteria);
		}
		bool CanInsertBeforePartial(BListNode bn) {
			bool hasNoRequires = bn.ControllerInfo.Resources.HasNo<RequiresAttribute>();
			bool meetsOtherInsertBeforeCriteria = MeetsOtherInsertBeforeCriteria(bn);
			return (hasNoRequires && meetsOtherInsertBeforeCriteria);
		}
		bool CanInsertAfterPartial(BListNode bn) {
			bool thisHasRequired = Has(bn.ControllerInfo.Resources.GetBy<RequiresAttribute>());
			bool meetsOtherInsertAfterCriteria = MeetsOtherInsertAfterCriteria(bn);
			return (thisHasRequired && meetsOtherInsertAfterCriteria);
		}
		bool MeetsOtherInsertBeforeCriteria(BListNode bn) {
			return (this.CompareTo(bn) >= 0);
		}
		bool MeetsOtherInsertAfterCriteria(BListNode bn) {
			if (this._child == null)
				return (this.CompareTo(bn) <= 0);
			if(this._child.MeetsOtherInsertBeforeCriteria(bn)) //need to be able to insert before child...
				return (this.CompareTo(bn) <= 0);
			return false;
		}
		#endregion

		#region IComparable<BControllerNode> and related methods
		/// <summary>
		/// Allows sorting based on [security, bindtype, priority]
		/// Does not look at resource dependencies.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool IgnorePriority { get { return _ignorePriority; } set { _ignorePriority = value; } }
		public bool IgnoreBindType { get { return _ignoreBindType; } set { _ignoreBindType = value; } }
		public int CompareTo(BListNode other) {
			if (this.ControllerInfo.IsSecurity && other.ControllerInfo.IsSecurity)
				return ComparePriorities(other);
			if (this.ControllerInfo.IsSecurity)
				return -1;
			if (other.ControllerInfo.IsSecurity)
				return 1;
			return CompareBindTypes(other);
		}
		int ComparePriorities(BListNode other) {
			if (_ignorePriority)
				return 0;
			//Higher priorities come first...
			int ithis = this.ControllerInfo.GetPriority(this._binding) * -1;
			int iother = other.ControllerInfo.GetPriority(other._binding) * -1;
			return ithis.CompareTo(iother);
		}
		int CompareBindTypes(BListNode other) {
			if (_ignoreBindType)
				return 0;
			BindType bthis = this.ControllerInfo.GetBindType(_binding);
			BindType bother = other.ControllerInfo.GetBindType(other._binding);
			if (bthis == BindType.Before && bother == BindType.Before)
				return ComparePriorities(other);
			if (bthis == BindType.Before)
				return -1;
			if (bother == BindType.Before)
				return 1;
			if (bthis == BindType.Payload && bother == BindType.Payload)
				return ComparePriorities(other);
			if (bthis == BindType.Payload)
				return -1;
			if (bother == BindType.Payload)
				return 1;
			return ComparePriorities(other);
		}
		#endregion
	}
}
