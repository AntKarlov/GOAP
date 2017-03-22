using System.Collections.Generic;

namespace Anthill.AI
{
	public class AntAIState
	{
		public string name;

		protected List<string> _interruptions;
		protected bool _isFinished;

		public AntAIState(string aName)
		{
			name = aName;
			_interruptions = new List<string>();
			_isFinished = false;
		}

		public void AddInterrupt(string aConditionName)
		{
			_interruptions.Add(aConditionName);
		}

		public virtual void Start()
		{
			// Вызывается перед началом выполнения задачи.
		}

		public virtual void Update(float aDeltaTime)
		{
			// Вызывается каждый игровой цикл пока задача активна.
		}

		public virtual void Stop()
		{
			// Вызывается после завершения выполнения задачи (даже если задача была прервана).
		}

		public virtual void Reset()
		{
			_isFinished = false;
		}

		public bool IsFinished(AntAIAgent aAgent, AntAICondition aWorldState)
		{
			if (_isFinished || OverlapInterrupts(aAgent.planner, aWorldState))
			{
				Reset();
				return true;
			}
			return false;
		}

		public bool OverlapInterrupts(AntAIPlanner aPlanner, AntAICondition aConditions)
		{
			int index = -1;
			for (int i = 0, n = _interruptions.Count; i < n; i++)
			{
				index = aPlanner.GetAtomIndex(_interruptions[i]);
				if (aConditions.GetValue(index))
				{
					return true;
				}
			}
			return false;
		}
	}
}