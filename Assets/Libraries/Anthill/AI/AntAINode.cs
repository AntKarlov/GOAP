namespace Anthill.AI
{
	public class AntAINode
	{
		public AntAICondition parent;
		public AntAICondition world;
		public string action;
		public int heuristic;
		public int cost;
		public int sum;
	}
}