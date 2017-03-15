using System.Collections.Generic;
using Anthill.Utils;

namespace Anthill.AI
{
	public class AntAILogic : ILogic
	{
		private AntAIScenarioCondition _availConditions;
		private AntAIPlanner _planner;
		private AntAICondition _goals;
		private List<KeyValuePair<string, string>> _tasks;

		public AntAILogic(AntAIScenario aScenario)
		{
			// Копируем состояния из сценария.
			_availConditions = aScenario.condition.Clone();

			// Копируем действия из сценария.
			_planner = new AntAIPlanner();
			AntAIScenarioAction action;
			for (int i = 0, n = aScenario.actions.Length; i < n; i++)
			{
				action = aScenario.actions[i];
				//AntLog.Trace("Action: {0}", action.name);
				for (int j = 0, nj = action.preConditions.Length; j < nj; j++)
				{
					_planner.Pre(action.name, 
						_availConditions[action.preConditions[j].id], 
						action.preConditions[j].value);
					/*AntLog.Trace("Pre -> {0}:{1}", 
						_availConditions[action.preConditions[j].id],
						action.preConditions[j].value);*/
				}

				for (int j = 0, nj = action.postConditions.Length; j < nj; j++)
				{
					_planner.Post(action.name, 
						_availConditions[action.postConditions[j].id], 
						action.postConditions[j].value);
					/*AntLog.Trace("Post -> {0} : {1}", 
						_availConditions[action.postConditions[j].id],
						action.postConditions[j].value);*/
				}
				
				RegisterTask(action.name, action.task);
			}

			_goals = new AntAICondition();
			//AntLog.Trace("Goals:");
			for (int i = 0, n = aScenario.goals.Length; i < n; i++)
			{
				_goals.Set(_planner, _availConditions[aScenario.goals[i].id], aScenario.goals[i].value);
				/*AntLog.Trace("{0} : {1}", 
						_availConditions[aScenario.goals[i].id],
						aScenario.goals[i].value);*/
			}
		}

		// -----------------------------------------------------
		// Public Methods
		// -----------------------------------------------------

		public virtual string SelectNewTask(AntAICondition aConditions)
		{
			string result = "";
			//AntAICondition condition = aConditions.Clone();
			List<string> plan = _planner.GetPlan(aConditions, _goals);
			if (plan != null && plan.Count > 0)
			{
				// Берем первое действие из составленного плана.
				result = _planner.GetAction(plan[0]).name;

				// Отладочный вывод плана в консоль.
				//if (_outputPlan)
				/*{
					string p = string.Format("Conditions: {0}\n", _planner.NameIt(condition.Description()));
					for (int i = 0; i < plan.Count; i++)
					{
						AntAIAction action = _planner.GetAction(plan[i]);
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

			return GetTask(result);
		}

		// -----------------------------------------------------
		// Private Methods
		// -----------------------------------------------------

		private void RegisterTask(string aActionName, string aTaskName)
		{
			if (_tasks == null)
			{
				_tasks = new List<KeyValuePair<string, string>>();
			}

			int index = _tasks.FindIndex(x => string.Equals(aActionName, x.Key));
			if (index >= 0 && index < _tasks.Count)
			{
				AntLog.Warning("[AntAILogic] Task for the action \"{0}\" already registered!", aActionName);
			}
			else
			{
				_tasks.Add(new KeyValuePair<string, string>(aActionName, aTaskName));
			}
		}

		private string GetTask(string aActionName)
		{
			int index = _tasks.FindIndex(x => string.Equals(aActionName, x.Key));
			return (index >= 0 && index < _tasks.Count) ? _tasks[index].Value : null;
		}

		// -----------------------------------------------------
		// Getters/Setters
		// -----------------------------------------------------

		public AntAIPlanner Planner
		{
			get { return _planner; }
		}
	}
}