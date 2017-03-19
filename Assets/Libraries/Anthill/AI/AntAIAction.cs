namespace Anthill.AI
{
	public class AntAIAction
	{
		public int cost;            // Цена действия.
		public string name;         // Имя действия.
		public string state;        // Имя состояния связанного с этим действием.
		public AntAICondition pre;  // Предстоящие условия.
		public AntAICondition post; // Последующие условия.

		public AntAIAction(string aName, int aCost = 1)
		{
			cost = aCost;
			name = aName;
			state = aName;
			pre = new AntAICondition();
			post = new AntAICondition();
		}
	}
}