using System;

namespace Gpgoap
{
	[Serializable]
	public class Action {

		public string name;
		public WorldState pre;
		public WorldState post;
		public int cost;

		public Action(string name, int cost = 1) {
			this.name = name;
			pre = new WorldState();
			post = new WorldState();
			this.cost = cost;
		}

	}
}