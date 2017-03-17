namespace Anthill.AI
{
	/// <summary>
	/// Узел данных сети мировых состояний.
	/// </summary>
	public class AntAINode
	{
		public AntAICondition parent; // Состояние из которого мы пришли.
		public AntAICondition world;  // Состояние мира для этого узла.
		public string action;         // Действие которое привело к этому узлу.
		public int heuristic;         // Остаточная стоимость.
		public int cost;              // Стоимость узла.
		public int sum;               // Сумма heruistic и cost.
	}
}