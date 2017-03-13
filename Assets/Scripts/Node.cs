namespace Gpgoap
{
	public class Node {
		//!< A node in our network of world states.
		public WorldState world;    //!< The state of the world at this node.
		public int cost;        //!< The cost so far.
		public int heuristic;        //!< The heuristic for remaining cost (don't overestimate!)
		public int sum;        //!< g+h combined.
		public string action;    //!< How did we get to this node?
		public WorldState parent;    //!< Where did we come from?
	}
}