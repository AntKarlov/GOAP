namespace Anthill.AI
{
	public class AntAIAction
	{
		public int cost;
		public string name;
		public AntAICondition pre;
		public AntAICondition post;

		public AntAIAction(string aName, int aCost = 1)
		{
			cost = aCost;
			name = aName;
			pre = new AntAICondition();
			post = new AntAICondition();
		}
	}
}