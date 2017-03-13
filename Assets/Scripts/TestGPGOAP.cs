using System.Collections.Generic;
using UnityEngine;

namespace Gpgoap
{
	public class TestGPGOAP : MonoBehaviour {

		[SerializeField]
		ActionPlanner planner;


		public void Start() {
			planner.Clear();
			planner.Pre("scout", "armedwithgun", true);
			planner.Post("scout", "enemyvisible", true);

			planner.Pre("search", "armedwithgun", false);
			planner.Post("search", "guninlineofsight", true);

			planner.Pre("pickup", "armedwithgun", false);
			planner.Pre("pickup", "guninlineofsight", true);
			planner.Post("pickup", "armedwithgun", true);
			planner.Post("pickup", "guninlineofsight", false);

			planner.Pre("approach", "enemyvisible", true);
			planner.Post("approach", "nearenemy", true);
			planner.Pre("aim", "enemyvisible", true);
			planner.Pre("aim", "weaponloaded", true);
			planner.Post("aim", "enemylinedup", true);
			planner.Pre("shoot", "enemylinedup", true);
			planner.Post("shoot", "enemyalive", false);
			planner.Pre("load", "armedwithgun", true);
			planner.Post("load", "weaponloaded", true);
			planner.Pre("detonatebomb", "armedwithbomb", true);
			planner.Pre("detonatebomb", "nearenemy", true);
			planner.Post("detonatebomb", "alive", false);
			planner.Post("detonatebomb", "enemyalive", false);
			planner.Post("detonatebomb", "armedwithbomb", false);
			planner.Pre("flee", "enemyvisible", true);
			planner.Post("flee", "nearenemy", false);


			string desc = planner.Describe();
			Debug.Log(desc);

			WorldState from = new WorldState();
			from.Clear();
			from.Set(planner.AtomIndex("enemyvisible"), false);
			from.Set(planner.AtomIndex("armedwithgun"), false);
			from.Set(planner.AtomIndex("guninlineofsight"), false);
			from.Set(planner.AtomIndex("weaponloaded"), false);
			from.Set(planner.AtomIndex("enemylinedup"), false);
			from.Set(planner.AtomIndex("enemyalive"), true);
			from.Set(planner.AtomIndex("armedwithbomb"), true);
			from.Set(planner.AtomIndex("nearenemy"), false);
			from.Set(planner.AtomIndex("alive"), true);
			planner.Cost("detonatebomb", 5);  // make suicide more expensive than shooting.

			WorldState goal = new WorldState();
			goal.Set(planner.AtomIndex("enemyalive"), false);
			goal.Set(planner.AtomIndex("alive"), true);
			float time = Time.realtimeSinceStartup;
			Debug.Log("Goal: "+ planner.NameIt(goal.Description()));
			List<string> plan = planner.Plan(from, goal);
			if (plan != null) {
				string p = "Plan:"+ planner.NameIt(from.Description())+"\n";
				for (int i = 0, j = plan.Count; i < j; i++) {
					Action action = planner.GetAction(plan[i]);
					from.Act(action.post);
					p+=(action.name+"\t"+ planner.NameIt(from.Description())+"\n");
				}
				Debug.Log(p);
			}else {
				Debug.Log("Plan is stupid");
			}
			Debug.Log("Speed: " + (Time.realtimeSinceStartup - time));
		}

	}
}