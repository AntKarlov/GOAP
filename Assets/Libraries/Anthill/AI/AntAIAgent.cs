using System;
using Anthill.Utils;

namespace Anthill.AI
{
	public class AntAIAgent
	{
		public ISense sense;                 // Органы чувств.
		public AntAIState[] states;          // Набор доступных состояний.
		public AntAIState currentState;      // Текущее состояние.
		public AntAIState defaultState;      // Состояние по умолчанию.
		public AntAICondition worldState;    // Текущее состояние.
		public AntAIPlanner planner;         // Планировщик.
		public AntAIPlan currentPlan;        // Текущий план.
		public AntAICondition currentGoal;   // Текущая цель.
		public AntAICondition defaultGoal;   // Цель по умолчанию.

		// Если установить flase, то план будет построен, но выполнятся не будет.
		public bool allowSetNewState;

		public AntAIAgent()
		{
			sense = null;
			currentState = null;
			defaultState = null;
			worldState = new AntAICondition();
			planner = new AntAIPlanner();
			currentPlan = new AntAIPlan();
			currentGoal = null;
			allowSetNewState = true;
		}

		#region Public Methods

		/// <summary>
		/// Обновление текущего состояния.
		/// </summary>
		public void UpdateState(float aDeltaTime)
		{
			currentState.Update(aDeltaTime);
		}

		/// <summary>
		/// Мозговой штурм.
		/// </summary>
		public void Think()
		{
			// Собираем информацию о текущем состоянии игрового мира.
			sense.GetConditions(this, worldState);

			if (currentState == null)
			{
				// Если текущее состояние не установлено, тогда устанавливаем дефолтное состояние.
				SetDefaultState();
			}
			else
			{
				if (currentState.IsFinished(this, worldState))
				{
					// Если текущее состояние завершено или было прервано, тогда
					// выбираем новое состояние и принудительно устанавливаем его.
					SetState(SelectNewState(worldState), true);
				}
				else if (currentState.AllowForceInterrupting)
				{
					// Если текущее состояние по прежнему активно (не было прервано или закончено), тогда
					// обновляем план на основе текущей обстановки мира и меняем состояние только 
					// в том случае если состояние из обновленного плана будет отличаться от текущего.
					SetState(SelectNewState(worldState));
				}
			}
		}

		/// <summary>
		/// Выбирает новое состояние на основе текущего состояния мира.
		/// </summary>
		public string SelectNewState(AntAICondition aWorldState)
		{
			string newState = defaultState.name;
			if (currentGoal != null)
			{
				planner.MakePlan(ref currentPlan, aWorldState, currentGoal);
				if (currentPlan.isSuccess || currentPlan.Count > 0)
				{
					string actionName = planner.GetAction(currentPlan[0]).name;
					if (allowSetNewState)
					{
						newState = planner.GetState(actionName);
					}
					
					/* Отладочный вывод плана в консоль.
					AntAICondition condition = aConditions.Clone();
					string p = string.Format("Conditions: {0}\n", _planner.NameIt(condition.Description()));
					for (int i = 0; i < _currentPlan.Count; i++)
					{
						AntAIAction action = _planner.GetAction(_currentPlan[i]);
						condition.Act(action.post);
						p += string.Format("<color=orange>{0}</color> => {1}\n", action.name, _planner.NameIt(condition.Description()));
					}
					AntLog.Trace(p);
					//*/
				}
			}
			else
			{
				AntLog.Report("AntAIAgent", "<b>Goal</b> is not defined!");
			}

			return newState;
		}

		/// <summary>
		/// Определяет какая цель будет целью по умолчанию.
		/// </summary>
		public void DefaultGoalIs(string aGoalName)
		{
			defaultGoal = FindGoal(aGoalName);
		}

		/// <summary>
		/// Устанавливает указанную цель как текущую.
		/// </summary>
		public void SetGoal(string aGoalName)
		{
			currentGoal = FindGoal(aGoalName);
			if (currentGoal == null)
			{
				AntLog.Report("AntAIAgent", "Can't find \"{0}\" goal.", aGoalName);
				SetDefaultGoal();
			}
		}

		/// <summary>
		/// Устанавливает цель по умолчанию как текущее.
		/// </summary>
		public void SetDefaultGoal()
		{
			if (defaultGoal != null)
			{
				currentGoal = defaultGoal;
			}
			else
			{
				AntLog.Report("AntAIAgent", "Default <b>Goal</b> is not defined!");
			}
		}

		/// <summary>
		/// Определяет какое из состояний будет состоянием по умолчанию.
		/// </summary>
		public void DefaultStateIs(string aStateName)
		{
			defaultState = FindState(aStateName);
			if (defaultState == null)
			{
				AntLog.Report("AntAIAgent", "Can't set \"{0}\" as <b>Default State</b> because it is not existing!", aStateName);
			}
		}

		/// <summary>
		/// Устанавливает состояение по умолчанию как текущее.
		/// </summary>
		public void SetDefaultState()
		{
			if (currentState != null)
			{
				currentState.Stop();
			}

			AntLog.Assert(defaultState == null, "Default <b>State</b> is not defined!", true);
			currentState = defaultState;
			currentState.Reset();
			currentState.Start();
		}
		
		/// <summary>
		/// Устанавливает указанное состояние как текущее.
		/// </summary>
		public void SetState(string aStateName, bool aForce = false)
		{
			if (aForce || !string.Equals(currentState.name, aStateName))
			{
				currentState.Stop();
				currentState = FindState(aStateName);
				if (currentState != null)
				{
					currentState.Reset();
					currentState.Start();
				}
				else
				{
					AntLog.Report("AntAIAgent", "Can't find \"{0}\" state.", aStateName);
					SetDefaultState();
				}
			}
		}

		#endregion
		#region Private Methods

		/// <summary>
		/// Ищет зарегистрированное состояние по имени.
		/// </summary>
		private AntAIState FindState(string aStateName)
		{
			int index = Array.FindIndex(states, x => string.Equals(x.name, aStateName));
			return (index >= 0 && index < states.Length) ? states[index] : null;
		}

		/// <summary>
		/// Ищет зарегистрированную цель по имени.
		/// </summary>
		private AntAICondition FindGoal(string aGoalName)
		{
			int index = planner.goals.FindIndex(x => x.name.Equals(aGoalName));
			return (index >= 0 && index < planner.goals.Count) ? planner.goals[index] : null;
		}

		#endregion
	}
}