using System.Collections.Generic;
using System.Text;

namespace Anthill.AI
{
	public class AntAIPlanner
	{
		public const int MAX_ATOMS = 32;
		public const int MAX_ACTIONS = 32;

		public delegate void PlanUpdatedDelegate(AntAIPlan aNewPlan);
		public event PlanUpdatedDelegate EventPlanUpdated;

		public List<string> atoms = new List<string>();
		public List<AntAIAction> actions = new List<AntAIAction>();
		public List<AntAICondition> goals = new List<AntAICondition>();

		#if UNITY_EDITOR
		public AntAICondition debugConditions; // Used for AI Debugger only.
		#endif

		public void LoadScenario(AntAIScenario aScenario)
		{
			int atomIndex;

			// AntLog.Trace("<b>Action List</b>");

			AntAIAction action;
			AntAIScenarioAction scenarioAction;
			for (int i = 0, n = aScenario.actions.Length; i < n; i++)
			{
				scenarioAction = aScenario.actions[i];
				action = GetAction(scenarioAction.name);
				action.state = scenarioAction.state;
				action.cost = scenarioAction.cost;

				// AntLog.Trace("Action: {0}", action.name);

				for (int j = 0, nj = scenarioAction.pre.Length; j < nj; j++)
				{
					atomIndex = GetAtomIndex(aScenario.conditions.GetName(scenarioAction.pre[j].id));
					action.pre.Set(atomIndex, scenarioAction.pre[j].value);
					/*AntLog.Trace("Pre -> {0}:{1}", 
						aScenario.conditions.GetName(scenarioAction.pre[j].id),
						scenarioAction.pre[j].value);*/
				}

				for (int j = 0, nj = scenarioAction.post.Length; j < nj; j++)
				{
					atomIndex = GetAtomIndex(aScenario.conditions.GetName(scenarioAction.post[j].id));
					action.post.Set(atomIndex, scenarioAction.post[j].value);
					/*AntLog.Trace("Post -> {0} : {1}", 
						aScenario.conditions.GetName(scenarioAction.post[j].id),
						scenarioAction.post[j].value);*/
				}
			}

			// AntLog.Trace("<b>Goal List</b>");

			AntAICondition goal;
			AntAIScenarioGoal scenarioGoal;
			for (int i = 0, n = aScenario.goals.Length; i < n; i++)
			{
				scenarioGoal = aScenario.goals[i];
				goal = GetGoal(scenarioGoal.name);

				//AntLog.Trace("Goal: {0}", goal.name);

				for (int j = 0, nj = scenarioGoal.conditions.Length; j < nj; j++)
				{
					goal.Set(this, aScenario.conditions.GetName(scenarioGoal.conditions[j].id),
						scenarioGoal.conditions[j].value);
					/*AntLog.Trace("Cond -> {0} : {1}", 
						aScenario.conditions.GetName(scenarioGoal.conditions[j].id),
						scenarioGoal.conditions[j].value);*/
				}
			}
		}

		public bool Pre(string aActionName, string aAtomName, bool aValue)
		{
			AntAIAction action = GetAction(aActionName);
			int atomId = GetAtomIndex(aAtomName);
			if (action == null || atomId == -1)
			{
				return false;
			}
			return action.pre.Set(atomId, aValue);
		}

		public bool Post(string aActionName, string aAtomName, bool aValue)
		{
			AntAIAction action = GetAction(aActionName);
			int atomId = GetAtomIndex(aAtomName);
			if (action == null || atomId == -1)
			{
				return false;
			}
			return action.post.Set(atomId, aValue);
		}

		public bool SetCost(string aActionName, int aCost)
		{
			AntAIAction action = GetAction(aActionName);
			if (action != null)
			{
				action.cost = aCost;
				return true;
			}
			return false;
		}

		public void SetState(string aActionName, string aStateName)
		{
			FindAction(aActionName).state = aStateName;
		}

		public string GetState(string aActionName)
		{
			var action = FindAction(aActionName);
			AntLog.Assert(action == null, string.Format("Action \"{0}\" not registered!", aActionName), true);
			return action.state;
		}

		public string NameIt(bool[] aBits)
		{
			string result = "";
			for (int i = 0, n = atoms.Count; i < n; i++)
			{
				//result += (aBits[i] ? atoms[i].ToUpper() : atoms[i].ToLower()) + " ";
				if (aBits[i])
				{
					result = string.Format("{0} <color=green>{1}</color>", result, atoms[i]);
				}
				else
				{
					result = string.Concat(result, " ", atoms[i]);
				}
			}
			return result;
		}

		public void Clear()
		{
			atoms.Clear();
			actions.Clear();
		}

		public string Describe(string aTitle = "Result:")
		{
			bool value = false;
			AntAIAction action;
			StringBuilder result = new StringBuilder(string.Concat(aTitle, "\n"));
			for (int i = 0, n = actions.Count; i < n; i++)
			{
				action = actions[i];
				result.Append(string.Format("Action: '{0}' State: '{1}' Cost: {2}\n", 
					action.name, action.state, action.cost));
				result.Append("  Preconditions:\n");
				for (int j = 0; j < MAX_ATOMS; j++)
				{
					if (action.pre.GetMask(j))
					{
						value = action.pre.GetValue(j);
						result.Append(string.Format("    '<color={2}>{0}</color>' = <color={2}>{1}</color>\n", 
							atoms[j], value, (value) ? "green" : "red"));
					}
				}
				result.Append("  Postconditions:\n");
				for (int j = 0; j < MAX_ATOMS; j++)
				{
					if (action.post.GetMask(j))
					{
						value = action.post.GetValue(j);
						result.Append(string.Format("    '<color={2}>{0}</color>' = <color={2}>{1}</color>\n", 
							atoms[j], value, (value) ? "green" : "red"));
					}
				}
			}
			return result.ToString();
		}

		public int GetAtomIndex(string aAtomName)
		{
			int index = atoms.IndexOf(aAtomName);
			if (index == -1 && atoms.Count < MAX_ATOMS)
			{
				atoms.Add(aAtomName);
				index = atoms.Count - 1;
			}
			return index;
		}

		public AntAICondition GetGoal(string aGoalName)
		{
			AntAICondition goal = FindGoal(aGoalName);
			if (goal == null)
			{
				goal = new AntAICondition() { name = aGoalName };
				goals.Add(goal);
			}
			return goal;
		}

		public AntAICondition FindGoal(string aGoalName)
		{
			return goals.Find(x => x.name.Equals(aGoalName));
		}

		public AntAIAction GetAction(string aActionName)
		{
			AntAIAction action = FindAction(aActionName);
			if (action == null && actions.Count < MAX_ACTIONS)
			{
				action = new AntAIAction(aActionName);
				actions.Add(action);
			}
			return action;
		}

		public AntAIAction FindAction(string aActionName)
		{
			return actions.Find(x => (x.name != null && x.name.Equals(aActionName)));
		}

		public void MakePlan(ref AntAIPlan aPlan, AntAICondition aCurrent, AntAICondition aGoal)
		{
			#if UNITY_EDITOR
			debugConditions = aCurrent.Clone();
			#endif

			List<AntAINode> opened = new List<AntAINode>();
			List<AntAINode> closed = new List<AntAINode>();

			opened.Add(new AntAINode()
			{
				world = aCurrent,
				parent = null,
				cost = 0,
				heuristic = aCurrent.Heuristic(aGoal),
				sum = aCurrent.Heuristic(aGoal),
				action = ""
			});

			AntAINode current = opened[0];
			while (opened.Count > 0)
			{
				// Find lowest rank
				current = opened[0];
				for (int i = 1, n = opened.Count; i < n; i++)
				{
					if (opened[i].sum < current.sum)
					{
						current = opened[i];
					}
				}

				opened.Remove(current);

				if (current.world.Match(aGoal))
				{
					// Plan is found!
					ReconstructPlan(ref aPlan, closed, current);
					aPlan.isSuccess = true;
					if (EventPlanUpdated != null)
					{
						EventPlanUpdated(aPlan);
					}

					return;
				}

				closed.Add(current);

				// Get neighbors
				List<AntAIAction> neighbors = GetPossibleTransitions(current.world);
				AntAICondition neighbor;
				int openedIndex = -1;
				int closedIndex = -1;
				int cost = -1;
				for (int i = 0, n = neighbors.Count; i < n; i++)
				{
					cost = current.cost + neighbors[i].cost;
					
					neighbor = current.world.Clone();
					neighbor.Act(neighbors[i].post);

					openedIndex = FindEqual(opened, neighbor);
					closedIndex = FindEqual(closed, neighbor);

					if (openedIndex > -1 && cost < opened[openedIndex].cost)
					{
						opened.RemoveAt(openedIndex);
						openedIndex = -1;
					}

					if (closedIndex > -1 && cost < closed[closedIndex].cost)
					{
						closed.RemoveAt(closedIndex);
						closedIndex = -1;
					}

					if (openedIndex == -1 && closedIndex == -1)
					{
						opened.Add(new AntAINode()
						{
							world = neighbor,
							cost = cost,
							heuristic = neighbor.Heuristic(aGoal),
							sum = cost + neighbor.Heuristic(aGoal),
							action = neighbors[i].name,
							parent = current.world
						});
					}
				}
			}

			// Failed plan.
			ReconstructPlan(ref aPlan, closed, current);
			aPlan.isSuccess = false;

			if (EventPlanUpdated != null)
			{
				EventPlanUpdated(aPlan);
			}
		}

		#region Private Methods

		private List<AntAIAction> GetPossibleTransitions(AntAICondition aCurrent)
		{
			List<AntAIAction> possible = new List<AntAIAction>();
			for (int i = 0, n = actions.Count; i < n; i++)
			{
				if (actions[i].pre.Match(aCurrent))
				{
					possible.Add(actions[i]);
				}
			}
			return possible;
		}

		private int FindEqual(List<AntAINode> aList, AntAICondition aCondition)
		{
			for (int i = 0, n = aList.Count; i < n; i++)
			{
				if (aList[i].world.Equals(aCondition))
				{
					return i;
				}
			}
			return -1;
		}

		private void ReconstructPlan(ref AntAIPlan aPlan, List<AntAINode> aClosed, AntAINode aGoal)
		{
			aPlan.Reset();
			int index = -1;
			AntAINode current = aGoal;
			while (current != null && current.parent != null)
			{
				aPlan.Insert(current.action);
				index = FindEqual(aClosed, current.parent);
				current = (index == -1) ? aClosed[0] : aClosed[index];
			}
		}

		#endregion
	}
}