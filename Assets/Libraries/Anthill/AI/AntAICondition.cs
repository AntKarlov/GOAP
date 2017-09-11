namespace Anthill.AI
{
	public class AntAICondition
	{
		public string name;
		public bool[] values;
		public bool[] mask;

		private AntAIPlanner _currentPlanner;

		public AntAICondition()
		{
			values = new bool[AntAIPlanner.MAX_ATOMS];
			mask = new bool[AntAIPlanner.MAX_ATOMS];
		}

		public void Clear()
		{
			for (int i = 0, n = AntAIPlanner.MAX_ATOMS; i < n; i++)
			{
				values[i] = false;
				mask[i] = false;
			}
		}

		public void BeginUpdate(AntAIPlanner aPlanner)
		{
			_currentPlanner = aPlanner;
		}

		public void EndUpdate()
		{
			_currentPlanner = null;
		}

		public bool Has(string aAtomName)
		{
			return Has(_currentPlanner, aAtomName);
		}

		public bool Has(AntAIPlanner aPlanner, string aAtomName)
		{
			int index = aPlanner.GetAtomIndex(aAtomName);
			return (index >= 0 && index < values.Length) 
				? values[index] 
				: false;
		}

		public bool Set(string aAtomName, bool aValue)
		{
			return Set(_currentPlanner.GetAtomIndex(aAtomName), aValue);
		}

		public bool Set(AntAIPlanner aPlanner, string aAtomName, bool aValue)
		{
			return Set(aPlanner.GetAtomIndex(aAtomName), aValue);
		}

		public bool Set(int aIndex, bool aValue)
		{
			if (aIndex >= 0 && aIndex < AntAIPlanner.MAX_ATOMS)
			{
				values[aIndex] = aValue;
				mask[aIndex] = true;
				return true;
			}
			return false;
		}

		public int Heuristic(AntAICondition aOther)
		{
			int dist = 0;
			for (int i = 0; i < AntAIPlanner.MAX_ATOMS; i++)
			{
				if (aOther.mask[i] && values[i] != aOther.values[i])
				{
					dist++;
				}
			}
			return dist;
		}

		public bool Match(AntAICondition aOther)
		{
			for (int i = 0; i < AntAIPlanner.MAX_ATOMS; i++)
			{
				if ((mask[i] && aOther.mask[i]) && (values[i] != aOther.values[i])) 
				{
					return false;
				}
			}
			return true;
		}

		public bool GetMask(int aIndex)
		{
			return (aIndex >= 0 && aIndex < AntAIPlanner.MAX_ATOMS) ? mask[aIndex] : false;
		}

		public bool GetValue(int aIndex)
		{
			return (aIndex >= 0 && aIndex < AntAIPlanner.MAX_ATOMS) ? values[aIndex] : false;
		}

		public AntAICondition Clone()
		{
			AntAICondition clone = new AntAICondition();
			for (int i = 0; i < AntAIPlanner.MAX_ATOMS; i++)
			{
				clone.values[i] = values[i];
				clone.mask[i] = mask[i];
			}
			return clone;
		}

		public void Act(AntAICondition aPost)
		{
			for (int i = 0; i < AntAIPlanner.MAX_ATOMS; i++)
			{
				mask[i] = mask[i] || aPost.mask[i];
				if (aPost.mask[i])
				{
					values[i] = aPost.values[i];
				}
			}
		}

		public bool Equals(AntAICondition aCondition)
		{
			for (int i = 0; i < AntAIPlanner.MAX_ATOMS; i++)
			{
				if (values[i] != aCondition.values[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool[] Description()
		{
			bool[] result = new bool[AntAIPlanner.MAX_ATOMS];
			for (int i = 0; i < AntAIPlanner.MAX_ATOMS; i++)
			{
				result[i] = mask[i] && values[i];
			}
			return result;
		}
	}
}