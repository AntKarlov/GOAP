namespace Anthill.AI
{
	public class AntAIAction
	{
		public int cost; // Цена действия.
		public string name; // Имя действия.
		public string task; // Имя задачи для действия.
		public AntAICondition pre; // Предстоящие действию состояния.
		public AntAICondition post; // Последующие состояния.

		public AntAIAction(string aName, int aCost = 1)
		{
			cost = aCost;
			name = aName;
			task = aName;
			pre = new AntAICondition();
			post = new AntAICondition();
		}
	}
}