using System.Collections.Generic;
using UnityEngine;
using Anthill.AI;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// В данном классе описывается логика поведения бота.
	/// В планировщик задач добавляются условия и последствия исходя
	/// из которых бот строит свой план и старается следовать ему.
	/// </summary>
	public class BotLogic : ILogic
	{
		private bool _outputPlan;
		private AntAIPlanner _planner;
		private AntAICondition _goal;
		private List<KeyValuePair<string, string>> _actualActions;
		
		public BotLogic(GameObject aObject)
		{
			_outputPlan = aObject.GetComponent<AIControl>().outputPlan;
			_planner = new AntAIPlanner();

			// Это сценарий поведения бота.
			// Особенность данного сценриая в том, что он
			// не отражает реальную игровую обстановку,
			// данный сценарий используется только для построения
			// причинно-следственных связий исходя из которых
			// бот может построить свою стратегию действий.

			_planner.Pre("ScoutWithGun", "ArmedWithGun", true);
			_planner.Pre("ScoutWithGun", "HasAmmo", true);
			_planner.Post("ScoutWithGun", "EnemyVisible", true);
			RegisterAction("ScoutWithGun", "Scout");

			_planner.Pre("ScoutWithBomb", "ArmedWithBomb", true);
			_planner.Post("ScoutWithBomb", "EnemyVisible", true);
			RegisterAction("ScoutWithBomb", "Scout");

			_planner.Pre("PickupBomb", "BombInlineOfSight", true);
			_planner.Pre("PickupBomb", "ArmedWithGun", false);
			_planner.Pre("PickupBomb", "ArmedWithBomb", false);
			_planner.Post("PickupBomb", "ArmedWithBomb", true);
			_planner.Post("PickupBomb", "BombInlineOfSight", false);
			RegisterAction("PickupBomb", "PickupBomb");

			_planner.Pre("PickupBombWithGun", "BombInlineOfSight", true);
			_planner.Pre("PickupBombWithGun", "AmmoInlineOfSight", false);
			_planner.Pre("PickupBombWithGun", "ArmedWithGun", true);
			_planner.Pre("PickupBombWithGun", "HasAmmo", false);
			_planner.Post("PickupBombWithGun", "ArmedWithGun", false);
			_planner.Post("PickupBombWithGun", "BombInlineOfSight", false);
			_planner.Post("PickupBombWithGun", "ArmedWithBomb", true);
			RegisterAction("PickupBombWithGun", "PickupBomb");

			_planner.Pre("SearchGun", "ArmedWithGun", false);
			_planner.Pre("SearchGun", "ArmedWithBomb", false);
			_planner.Pre("SearchGun", "BombInlineOfSight", false);
			_planner.Post("SearchGun", "GunInlineOfSight", true);
			_planner.SetCost("SearchGun", 1);
			RegisterAction("SearchGun", "SearchGun");

			_planner.Pre("PickupGun", "ArmedWithGun", false);
			_planner.Pre("PickupGun", "GunInlineOfSight", true);
			_planner.Post("PickupGun", "ArmedWithGun", true);
			_planner.Post("PickupGun", "GunInlineOfSight", false);
			RegisterAction("PickupGun", "PickupGun");

			_planner.Pre("SearchAmmo", "ArmedWithGun", true);
			_planner.Pre("SearchAmmo", "HasAmmo", false);
			_planner.Post("SearchAmmo", "AmmoInlineOfSight", true);
			_planner.SetCost("SearchAmmo", 2);
			RegisterAction("SearchAmmo", "SearchAmmo");

			_planner.Pre("PickupAmmo", "ArmedWithGun", true);
			_planner.Pre("PickupAmmo", "HasAmmo", false);
			_planner.Pre("PickupAmmo", "AmmoInlineOfSight", true);
			_planner.Post("PickupAmmo", "HasAmmo", true);
			_planner.Post("PickupAmmo", "AmmoInlineOfSight", false);
			RegisterAction("PickupAmmo", "PickupAmmo");

			_planner.Pre("Approach", "EnemyVisible", true);
			_planner.Pre("Approach", "ArmedWithBomb", true);
			_planner.Post("Approach", "NearEnemy", true);
			RegisterAction("Approach", "Approach");

			_planner.Pre("Aim", "EnemyVisible", true);
			_planner.Pre("Aim", "ArmedWithGun", true);
			_planner.Pre("Aim", "HasAmmo", true);
			_planner.Post("Aim", "EnemyLinedUp", true);
			_planner.SetCost("Aim", 1);
			RegisterAction("Aim", "Aim");

			_planner.Pre("Shot", "HasAmmo", true);
			_planner.Pre("Shot", "EnemyLinedUp", true);
			_planner.Post("Shot", "EnemyAlive", false);
			_planner.Post("Shot", "EnemyVisible", false);
			_planner.Post("Shot", "EnemyLinedUp", false);
			_planner.SetCost("Shot", 1);
			RegisterAction("Shot", "Shot");

			_planner.Pre("DetonateBomb", "ArmedWithBomb", true);
			_planner.Pre("DetonateBomb", "NearEnemy", true);
			_planner.Post("DetonateBomb", "EnemyAlive", false);
			_planner.Post("DetonateBomb", "Alive", false);
			_planner.Post("DetonateBomb", "ArmedWithBomb", false);
			_planner.Post("DetonateBomb", "Injured", false);
			_planner.SetCost("DetonateBomb", 3);
			RegisterAction("DetonateBomb", "DetonateBomb");
			
			_planner.Pre("SearchHeal", "Injured", true);
			_planner.Pre("SearchHeal", "EnemyVisible", false);
			_planner.Pre("SearchHeal", "HealInlineOfSight", false);
			_planner.Post("SearchHeal", "HealInlineOfSight", true);
			_planner.SetCost("SearchHeal", 4);
			RegisterAction("SearchHeal", "SearchHeal");

			_planner.Pre("PickupHeal", "HealInlineOfSight", true);
			_planner.Pre("PickupHeal", "Injured", true);
			_planner.Post("PickupHeal", "HealInlineOfSight", false);
			_planner.Post("PickupHeal", "Injured", false);
			RegisterAction("PickupHeal", "PickupHeal");

			//_planner.Pre("Flee", "EnemyVisible", true);
			//_planner.Post("Flee", "NearEnemy", false);

			//Debug.Log(_planner.Describe());

			// Задачи которых должен достигнуть бот.
			_goal = new AntAICondition();
			_goal.Set(_planner, "EnemyAlive", false);
			_goal.Set(_planner, "Injured", false);
		}

		public string SelectNewSchedule(AntAICondition aCondition)
		{
			// (!) Здесь должна быть стейт-машина которая бы в зависимости от текущих 
			//     условий возвращала бы подходящее состояние. Но её здесь нет...

			string newAction = "";
			AntAICondition condition = aCondition.Clone();
			List<string> plan = _planner.GetPlan(aCondition, _goal);
			if (plan != null && plan.Count > 0)
			{
				// Берем первое действие из составленного плана.
				newAction = _planner.GetAction(plan[0]).name;

				// Отладочный вывод плана в консоль.
				if (_outputPlan)
				{
					string p = string.Format("Conditions: {0}\n", _planner.NameIt(condition.Description()));
					for (int i = 0; i < plan.Count; i++)
					{
						AntAIAction action = _planner.GetAction(plan[i]);
						condition.Act(action.post);
						p += string.Format("<color=orange>{0}</color> => {1}\n", action.name, _planner.NameIt(condition.Description()));
					}
					Debug.Log(p);
				}
			}
			else 
			{
				// План не удалось составить.
				string p = string.Format("<color=red>Plan not found!</color> Conditions: {0}\n", _planner.NameIt(condition.Description()));
				for (int i = 0; i < _planner.failedPlan.Count; i++)
				{
					AntAIAction action = _planner.GetAction(_planner.failedPlan[i]);
					condition.Act(action.post);
					p += string.Format("<color=orange>{0}</color> => {1}\n", action.name, _planner.NameIt(condition.Description()));
				}
				Debug.LogWarning(p);
			}

			return GetAction(newAction);
		}

		private void RegisterAction(string aAbstractActionName, string aRealActionName)
		{
			if (_actualActions == null)
			{
				_actualActions = new List<KeyValuePair<string, string>>();
			}

			int index = _actualActions.FindIndex(x => string.Equals(aAbstractActionName, x.Key));
			if (index >= 0 && index < _actualActions.Count)
			{
				Debug.LogWarning(string.Format("Abstract action \"{0}\" already registered!", aAbstractActionName));
			}
			else
			{
				_actualActions.Add(new KeyValuePair<string, string>(aAbstractActionName, aRealActionName));
			}
		}

		private string GetAction(string aAbstractActionName)
		{
			int index = _actualActions.FindIndex(x => string.Equals(aAbstractActionName, x.Key));
			return (index >= 0 && index < _actualActions.Count) ? _actualActions[index].Value : null;
		}

		public AntAIPlanner Planner
		{
			get { return _planner; }
		}
	}
}