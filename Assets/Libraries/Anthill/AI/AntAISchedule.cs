using System.Collections.Generic;
using System;
using UnityEngine;

namespace Anthill.AI
{
	public class AntAISchedule
	{
		public string name;
		public bool isDefault;

		protected List<AntAITask> _tasks;
		protected AntAITask _currentTask;
		protected List<String> _interruptions;
		protected int _curIndex;
		protected int _prevIndex;
		protected bool _finished;

		public AntAISchedule(string aName, bool aDefault = false)
		{
			name = aName;
			isDefault = aDefault;
			_tasks = new List<AntAITask>();
			_interruptions = new List<String>();
		}

		public void Reset()
		{
			_currentTask = null;
			_curIndex = 0;
			_prevIndex = -1;
			_finished = false;

			for (int i = 0, n = _tasks.Count; i < n; i++)
			{
				_tasks[i].isFinished = false;
			}
		}

		public virtual void Start(GameObject aOwner)
		{
			// Override this method and prepare schedule to work.
		}

		public virtual void Stop(GameObject aOwner)
		{
			// Override this method and correctly finish the work.
		}

		public void AddTask(Func<bool> aFunction)
		{
			var task = new AntAITask();
			task.SetFunc(aFunction);
			_tasks.Add(task);
		}

		public void AddTask<T1>(Func<T1, bool> aFunction, T1 aArg1)
		{
			var task = new AntAITask<T1>();
			task.SetFunc(aFunction);
			task.SetArg(aArg1);
			_tasks.Add(task);
		}

		public void AddTask<T1, T2>(Func<T1, T2, bool> aFunction, T1 aArg1, T2 aArg2)
		{
			var task = new AntAITask<T1, T2>();
			task.SetFunc(aFunction);
			task.SetArg(aArg1, aArg2);
			_tasks.Add(task);
		}

		public void AddInterrupt(string aConditionName)
		{
			_interruptions.Add(aConditionName);
		}

		public void Update()
		{
			if (!_finished)
			{
				if (_curIndex != _prevIndex)
				{
					_currentTask = _tasks[_curIndex];
					_prevIndex = _curIndex;
				}

				if (_currentTask != null)
				{
					_currentTask.Update();
					if (_currentTask.isFinished)
					{
						_curIndex++;
						if (_curIndex >= _tasks.Count)
						{
							Finish();
						}
					}
				}
				else
				{
					Finish();
				}
			}
		}

		public bool IsFinished(ILogic aLogic, AntAICondition aConditions)
		{
			if (_finished || OverlapInterrupts(aLogic.Planner, aConditions))
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

		private void Finish()
		{
			_finished = true;
		}
	}
}