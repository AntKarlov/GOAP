using UnityEngine;
using Anthill.AI;

namespace Scripts
{
	public class Test : MonoBehaviour
	{
		public AntAIPlanner planner;

		private void Start()
		{
			planner = new AntAIPlanner();

			// Если имеем пушку, то можем видеть врага.
			planner.Pre("Scout", "ArmedWithGun", true);
			planner.Post("Scout", "EnemyVisible", true);

			// Если пушки нет, то можем увидеть пушку на земле.
			planner.Pre("Search", "ArmedWithGun", false);
			//planner.Pre("Search", "ArmedWithBomb", false);
			planner.Post("Search", "GunInlineOfSight", true);
			//planner.Post("Search", "ArmedWithBomb", true);

			// Чтобы подобрать пушку, мы должны быть без пушки и видеть её на земле.
			planner.Pre("PickupGun", "ArmedWithGun", false);
			planner.Pre("PickupGun", "GunInlineOfSight", true);
			planner.Post("PickupGun", "ArmedWithGun", true); // Подбираем
			planner.Post("PickupGun", "GunInlineOfSight", false); // Больше не видем пушку.

			planner.Pre("PickupBomb", "ArmedWithBomb", false);
			planner.Pre("PickupBomb", "BombInlineOfSight", true);
			planner.Post("PickupBomb", "ArmedWithBomb", true);
			planner.Post("PickupBomb", "BombInlineOfSight", false);

			// Если мы видим врага, то можем приблизится к нему.
			planner.Pre("Approach", "EnemyVisible", true);
			planner.Post("Approach", "NearEnemy", true);

			planner.Pre("Aim", "EnemyVisible", true);
			planner.Pre("Aim", "WeaponLoaded", true);
			planner.Post("Aim", "EnemyLinedUp", true);

			planner.Pre("Shot", "EnemyLinedUp", true);
			planner.Post("Shot", "EnemyAlive", false);

			planner.Pre("Load", "ArmedWithGun", true);
			planner.Post("Load", "WeaponLoaded", true);
			
			planner.Pre("DetonateBomb", "ArmedWithBomb", true);
			planner.Pre("DetonateBomb", "NearEnemy", true);
			planner.Post("DetonateBomb", "EnemyAlive", false);
			planner.Post("DetonateBomb", "Alive", false);
			planner.Post("DetonateBomb", "ArmedWithBomb", false);
			
			planner.Pre("Flee", "EnemyVisible", true);
			planner.Post("Flee", "NearEnemy", false);

			Debug.Log(planner.Describe());

			AntAICondition current = new AntAICondition();
			current.Set(planner, "EnemyVisible", false);
			current.Set(planner, "ArmedWithGun", false);
			current.Set(planner, "GunInlineOfSight", false);
			current.Set(planner, "WeaponLoaded", false);
			current.Set(planner, "EnemyLinedUp", false);
			current.Set(planner, "EnemyAlive", true);
			current.Set(planner, "ArmedWithBomb", true);
			current.Set(planner, "NearEnemy", false);
			current.Set(planner, "Alive", true);
			//planner.SetCost("DetonateBomb", 1);
			Debug.Log("Current: " + planner.NameIt(current.Description()));

			AntAICondition goal = new AntAICondition();
			goal.Set(planner, "EnemyAlive", false);
			//goal.Set(planner.GetAtomIndex("Alive"), true);
			Debug.Log("Goal: " + planner.NameIt(goal.Description()));

			DoPlan(current, goal);
		}

		private void DoPlan(AntAICondition aCurrent, AntAICondition aGoal)
		{
			AntAIPlan plan = new AntAIPlan();
			planner.MakePlan(ref plan, aCurrent, aGoal);
			if (plan != null)
			{
				string p = string.Format("Plan: {0}\n", planner.NameIt(aCurrent.Description()));
				for (int i = 0; i < plan.Count; i++)
				{
					AntAIAction action = planner.GetAction(plan[i]);
					aCurrent.Act(action.post);
					p += string.Format("{0} => {1}\n", action.name, planner.NameIt(aCurrent.Description()));
				}
				Debug.Log(p);
			}
			else
			{
				Debug.Log("Plan not found!");
			}
		}
	}
}