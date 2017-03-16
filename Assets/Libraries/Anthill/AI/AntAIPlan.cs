using System.Collections.Generic;

namespace Anthill.AI
{
	public class AntAIPlan
	{	
		public bool isSuccess;
		private List<string> _actions;

		public AntAIPlan()
		{
			_actions = new List<string>();
		}

		public void Reset()
		{
			_actions.Clear();
		}

		public void Insert(string aValue)
		{
			_actions.Insert(0, aValue);
		}

		public string this[int aIndex]
		{
			get { return (aIndex >= 0 && aIndex < _actions.Count) ? _actions[aIndex] : null; }
		}

		public int Count
		{
			get { return _actions.Count; }
		}
	}
}