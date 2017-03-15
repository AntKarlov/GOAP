namespace Anthill.AI
{
	public class AntAICondition
	{
		public bool[] values;
		public bool[] mask;

		public AntAICondition()
		{
			Clear();
		}

		public void Clear()
		{
			values = new bool[AntAIPlanner.MAX_ATOMS];
			mask = new bool[AntAIPlanner.MAX_ATOMS];
		}

		public bool Has(AntAIPlanner aPlanner, string aAtomName)
		{
			int index = aPlanner.GetAtomIndex(aAtomName);
			if (index >= 0 && index < values.Length)
			{
				return values[index];
			}
			return false;
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