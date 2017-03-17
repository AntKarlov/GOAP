namespace Anthill.AI
{
	public class AntAILogic : ILogic
	{
		private AntAIPlanner _planner;
		private AntAIPlan _currentPlan;
		private AntAICondition _currentGoal;

		public AntAILogic(AntAIScenario aScenario)
		{
			_planner = new AntAIPlanner();
			_planner.LoadScenario(aScenario);
			//Debug.Log(_planner.Describe());

			_currentPlan = new AntAIPlan();
			_currentGoal = _planner.goals[0];
		}

		#region ILogic Implementation

		public virtual string SelectNewTask(AntAICondition aConditions)
		{
			string actionName = "";
			//AntAICondition condition = aConditions.Clone();
			_planner.MakePlan(ref _currentPlan, aConditions, _currentGoal);
			if (_currentPlan.isSuccess)
			{
				// Берем первое действие из составленного плана.
				actionName = _planner.GetAction(_currentPlan[0]).name;

				// Отладочный вывод плана в консоль.
				//if (_outputPlan)
				/*{
					string p = string.Format("Conditions: {0}\n", _planner.NameIt(condition.Description()));
					for (int i = 0; i < _currentPlan.Count; i++)
					{
						AntAIAction action = _planner.GetAction(_currentPlan[i]);
						condition.Act(action.post);
						p += string.Format("<color=orange>{0}</color> => {1}\n", action.name, _planner.NameIt(condition.Description()));
					}
					AntLog.Trace(p);
				}*/
			}
			/*else 
			{
				// План не удалось составить.
				string p = string.Format("<color=red>Plan not found!</color> Conditions: {0}\n", _planner.NameIt(condition.Description()));
				for (int i = 0; i < _planner.failedPlan.Count; i++)
				{
					AntAIAction action = _planner.GetAction(_planner.failedPlan[i]);
					condition.Act(action.post);
					p += string.Format("<color=orange>{0}</color> => {1}\n", action.name, _planner.NameIt(condition.Description()));
				}
				AntLog.Warning(p);
			}*/

			return _planner.GetTask(actionName);
		}

		public AntAIPlan CurrentPlan
		{
			get { return _currentPlan; }
		}

		public AntAIPlanner Planner
		{
			get { return _planner; }
		}
		
		#endregion
	}
}