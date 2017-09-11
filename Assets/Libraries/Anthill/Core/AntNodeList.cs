using System.Collections.Generic;

namespace Anthill.Core
{
	public class AntNodeList<T>
	{
		internal enum PendingChange
		{
			Add,
			Remove
		}

		public delegate void NodeChangeDelegate(T aNode);
		public event NodeChangeDelegate EventNodeAdded;
		public event NodeChangeDelegate EventNodeRemoved;

		private List<T> _nodes;
		private int _count;
		private List<KeyValuePair<T, PendingChange>> _pending;
		private int _lockCount = 0;

		public AntNodeList()
		{
			_nodes = new List<T>();
			_pending = new List<KeyValuePair<T, PendingChange>>();
		}

		#region Public Methods

		public void Add(T aNode)
		{
			if (IsLocked)
			{
				_pending.Add(new KeyValuePair<T, PendingChange>(aNode, PendingChange.Add));
			}
			else
			{
				if (EventNodeAdded != null)
				{
					EventNodeAdded(aNode);
				}
				_nodes.Add(aNode);
				_count++;
			}
		}

		public void Remove(T aNode)
		{
			if (IsLocked)
			{
				_pending.Add(new KeyValuePair<T, PendingChange>(aNode, PendingChange.Remove));
			}
			else
			{
				if (EventNodeRemoved != null)
				{
					EventNodeRemoved(aNode);
				}
				_nodes.Remove(aNode);
				_count--;
			}
		}

		public void Lock()
		{
			_lockCount++;
		}

		public void Unlock()
		{
			_lockCount--;
			if (_lockCount <= 0)
			{
				ApplyPending();
				_lockCount = 0;
			}
		}

		#endregion
		#region Private Methods

		private void ApplyPending()
		{
			KeyValuePair<T, PendingChange> pair;
			for (int i = 0, n = _pending.Count; i < n; i++)
			{
				pair = _pending[i];
				if (pair.Value == PendingChange.Add)
				{
					Add(pair.Key);
				}
				else
				{
					Remove(pair.Key);
				}
			}
			_pending.Clear();
		}

		#endregion
		#region Getters / Setters

		public T this[int aIndex]
		{
			get { return (aIndex >= 0 && aIndex < _nodes.Count) ? _nodes[aIndex] : default(T); }
		}

		public int Count
		{
			get { return _count; }
		}

		public bool IsLocked
		{
			get { return (_lockCount > 0); }
		}

		#endregion
	}
}