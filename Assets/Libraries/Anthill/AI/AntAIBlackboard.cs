using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anthill.AI
{
	public class AntAIBlackboard : MonoBehaviour
	{
		public delegate void PropertyUpdateDelegate(string aKey, AntAIBlackboardProp aProperty);
		public event PropertyUpdateDelegate EventPropertyAdded;
		public event PropertyUpdateDelegate EventPropertyRemoved;

		private Dictionary<string, AntAIBlackboardProp> _dict;

		public AntAIBlackboard()
		{
			_dict = new Dictionary<string, AntAIBlackboardProp>();
		}

		#region Public Methods

		public string GetKey(int aIndex)
		{
			return (aIndex >= 0 && aIndex < _dict.Count) ? _dict.ElementAt(aIndex).Key : null;
		}

		public bool Contains(string aKey)
		{
			return _dict.ContainsKey(aKey);
		}

		public AntAIBlackboardProp Remove(string aKey)
		{
			AntAIBlackboardProp result = null;
			if (_dict.ContainsKey(aKey))
			{
				result = _dict[aKey];
				_dict.Remove(aKey);
				if (EventPropertyRemoved != null)
				{
					EventPropertyRemoved(aKey, result);
				}
			}
			return result;
		}

		public void Clear()
		{
			_dict.Clear();
		}

		#endregion
		#region Getter/Setters

		public AntAIBlackboardProp this[int aIndex]
		{
			get
			{
				return _dict.ElementAt(aIndex).Value;
			}
			set
			{
				string key = _dict.ElementAt(aIndex).Key;
				_dict[key] = value;
			}
		}

		public AntAIBlackboardProp this[string aKey]
		{
			get
			{
				if (_dict.ContainsKey(aKey))
				{
					return _dict[aKey];
				}
				else
				{
					var prop = new AntAIBlackboardProp();
					_dict.Add(aKey, prop);
					if (EventPropertyAdded != null)
					{
						EventPropertyAdded(aKey, prop);
					}

					return prop;
				}
			}
			set
			{
				if (_dict.ContainsKey(aKey))
				{
					_dict[aKey] = value;
				}
				else
				{
					_dict.Add(aKey, value);
					if (EventPropertyAdded != null)
					{
						EventPropertyAdded(aKey, value);
					}
				}
			}
		}

		public int Count
		{
			get
			{
				int count = 0;
				foreach (var prop in _dict)
				{
					count++;
				}
				return count;
			}
		}

		#endregion
	}
}