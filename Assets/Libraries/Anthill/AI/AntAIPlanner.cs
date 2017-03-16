using System.Collections.Generic;
using System.Text;

namespace Anthill.AI
{
	public class AntAIPlanner
	{
		public const int MAX_ATOMS = 32;
		public const int MAX_ACTIONS = 32;

		public List<string> atoms = new List<string>();
		public List<AntAIAction> actions = new List<AntAIAction>();
		public AntAICondition goal;

		public void LoadScenario(AntAIScenario aScenario)
		{
			AntAIScenarioAction scenarioAction;
			AntAIAction action;
			int atomIndex;
			for (int i = 0, n = aScenario.actions.Length; i < n; i++)
			{
				scenarioAction = aScenario.actions[i];
				action = GetAction(scenarioAction.name);
				action.task = scenarioAction.task;
				action.cost = scenarioAction.cost;

				for (int j = 0, nj = scenarioAction.preConditions.Length; j < nj; j++)
				{
					atomIndex = GetAtomIndex(aScenario.condition[scenarioAction.preConditions[j].id]);
					action.pre.Set(atomIndex, scenarioAction.preConditions[j].value);
					/*Pre(action.name, 
						aScenario.condition[action.preConditions[j].id], 
						action.preConditions[j].value);*/
					/*AntLog.Trace("Pre -> {0}:{1}", 
						_availConditions[action.preConditions[j].id],
						action.preConditions[j].value);*/
				}

				for (int j = 0, nj = scenarioAction.postConditions.Length; j < nj; j++)
				{
					atomIndex = GetAtomIndex(aScenario.condition[scenarioAction.postConditions[j].id]);
					action.post.Set(atomIndex, scenarioAction.postConditions[j].value);
					/*Post(action.name, 
						aScenario.condition[action.postConditions[j].id], 
						action.postConditions[j].value);*/
					/*AntLog.Trace("Post -> {0} : {1}", 
						_availConditions[action.postConditions[j].id],
						action.postConditions[j].value);*/
				}
				
				//_planner.SetTask(action.name, action.task);
				//RegisterTask(action.name, action.task);
			}

			goal = new AntAICondition();
			//AntLog.Trace("Goals:");
			for (int i = 0, n = aScenario.goals.Length; i < n; i++)
			{
				goal.Set(this, aScenario.condition[aScenario.goals[i].id], aScenario.goals[i].value);
				/*AntLog.Trace("{0} : {1}", 
						_availConditions[aScenario.goals[i].id],
						aScenario.goals[i].value);*/
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

		public void SetTask(string aActionName, string aTaskName)
		{
			FindAction(aActionName).task = aTaskName;
		}

		public string GetTask(string aActionName)
		{
			return FindAction(aActionName).task;
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

		public string Describe()
		{
			StringBuilder result = new StringBuilder("Result:\n");
			AntAIAction action;
			for (int i = 0, n = actions.Count; i < n; i++)
			{
				action = actions[i];
				result.Append(string.Format("Action: {0} Task: {1} Cost: {2}\n", action.name, action.task, action.cost));
				result.Append("  Preconditions:\n");
				for (int j = 0; j < MAX_ATOMS; j++)
				{
					if (action.pre.GetMask(j))
					{
						result.Append(string.Format("    {0} == {1}\n", atoms[j], action.pre.GetValue(j)));
					}
				}
				result.Append("  Postconditions:\n");
				for (int j = 0; j < MAX_ATOMS; j++)
				{
					if (action.post.GetMask(j))
					{
						result.Append(string.Format("    {0} == {1}\n", atoms[j], action.post.GetValue(j)));
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

		public AntAIAction GetAction(string aActionName)
		{
			AntAIAction action = FindAction(aActionName);
			if (action == null && actions.Count < MAX_ATOMS)
			{
				action = new AntAIAction(aActionName);
				actions.Add(action);
			}
			return action;
		}

		public AntAIAction FindAction(string aActionName)
		{
			return actions.Find(x => x.name.Equals(aActionName));
		}

		public void MakePlan(ref AntAIPlan aPlan, AntAICondition aCurrent, AntAICondition aGoal)
		{
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

			ReconstructPlan(ref aPlan, closed, current);
			aPlan.isSuccess = true;
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
		#endregion Private Methods
	}
}