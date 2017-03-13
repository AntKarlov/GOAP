using System;
using System.Collections.Generic;
using System.Text;

namespace Gpgoap
{
	[Serializable]
	public class ActionPlanner {

		public const int ATOMS = 32; // имена битов
		public const int ACTIONS = 32;

		public List<string> atoms = new List<string>();
		public List<Action> actions = new List<Action>();

		public bool Pre(string name, string atom, bool value) {
			Action action = GetAction(name);
			int atomId = AtomIndex(atom);
			if (action == null || atomId == -1) return false;
			return action.pre.Set(atomId, value);
		}

		public bool Post(string name, string atom, bool value) {
			Action action = GetAction(name);
			int atomId = AtomIndex(atom);
			if (action == null || atomId == -1) return false;
			return action.post.Set(atomId, value);
		}

		public bool Cost(string name, int cost) {
			Action action = GetAction(name);
			if (action == null) return false;
			action.cost = cost;
			return true;
		}

		public string Describe() {
			StringBuilder result = new StringBuilder("Result:\n");
			for (int i = 0, j = actions.Count; i < j; i++) {
				Action action = actions[i];
				result.Append(action.name).Append("\n");
				result.Append("\tPreconditions:\n");
				for (int k = 0; k < atoms.Count; k++) {
					if (action.pre.Masked(k))
						result.Append("\t\t").Append(atoms[k]).Append(" == ").Append(action.pre.Value(k)).Append("\n");
				}
				result.Append("\tPostconditions:\n");
				for (int k = 0;k < ATOMS; k++) {
					if (action.post.Masked(k))
						result.Append("\t\t").Append(atoms[k]).Append(" == ").Append(action.post.Value(k)).Append("\n");
				}
			}
			return result.ToString();
		}

		public int AtomIndex(string name) {
			int index = atoms.IndexOf(name);
			if (index == -1 && atoms.Count < ATOMS)
			{
				atoms.Add(name);
				return atoms.Count - 1;
			}
			return index;
		}

		public Action GetAction(string name) {
			Action action = actions.Find((x)=>x.name.Equals(name));
			if (action == null && actions.Count < ACTIONS) {
				action = new Action(name);
				actions.Add(action);
			}
			return action;
		}

		public List<string> Plan(WorldState start, WorldState goal) {
			List<Node> opened = new List<Node>();
			List<Node> closed = new List<Node>();

			opened.Add(new Node() {
				world = start,
				parent = null,
				cost = 0,
				heuristic = start.Heuristic(goal),
				sum = 0 + start.Heuristic(goal),
				action = ""
			});
			while (opened.Count > 0) {

				// Find lowest rank
				Node current = opened[0];
				for (int i = 1, j = opened.Count; i < j; i++)
					if (opened[i].sum < current.sum)
						current = opened[i];

				opened.Remove(current);

				if (current.world.Match(goal))
					return ReconstructPlan(closed, current); // Done
				closed.Add(current);
				// Get neighbors
				List<Action> neighbors = GetPossibleTransitions(current.world);
				for (int i = 0, j = neighbors.Count; i < j; i++) {
					int cost = current.cost + neighbors[i].cost;

					WorldState neighbor = current.world.Copy();
					neighbor.Act(neighbors[i].post);

					int openedIndex = FindEqual(opened, neighbor);
					int closedIndex = FindEqual(closed, neighbor);

					if (openedIndex > -1 && cost < opened[openedIndex].cost) {
						opened.RemoveAt(openedIndex);
						openedIndex = -1;
					}
					if (closedIndex > -1 && cost < closed[closedIndex].cost) {
						closed.RemoveAt(closedIndex);
						closedIndex = -1;
					}
					if (openedIndex == -1 && closedIndex == -1) {
						opened.Add(new Node() {
							world = neighbor,
							cost = cost,
							heuristic = neighbor.Heuristic(goal),
							sum = cost + neighbor.Heuristic(goal),
							action = neighbors[i].name,
							parent = current.world
						});
					}
				}
			}
			return null;
		}

		public void Clear() {
			atoms.Clear();
			actions.Clear();
		}

		public string NameIt(bool[] bits) {
			string result = "";
			for (int i = 0, j = atoms.Count; i < j; i++) {
				result += (bits[i]?atoms[i].ToUpper():atoms[i].ToLower()) + " ";
			}
			return result;
		}

		private List<Action> GetPossibleTransitions(WorldState from) {
			List<Action> possible = new List<Action>();
			for (int i = 0, j = actions.Count; i < j; i++)
				if (actions[i].pre.Match(from))
					possible.Add(actions[i]);
			return possible;
		}

		private int FindEqual(List<Node> list, WorldState ws) {
			for (int i = 0, j = list.Count; i < j; i++)
				if (list[i].world.Equals(ws))
					return i;
			return -1;
		}

		private List<string> ReconstructPlan(List<Node> closed, Node goal) {
			List<string> result = new List<string>();
			Node current = goal;
			while (current != null && current.parent != null) {
				result.Insert(0, current.action);
				int i = FindEqual(closed, current.parent);
				current = ( i == -1 ) ? closed[0] : closed[i];
			}
			return result;
		}


	}
}