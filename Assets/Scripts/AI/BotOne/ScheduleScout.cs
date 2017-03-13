using Game.Map;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для поиска врага (патрулирования).
	/// </summary>
	public class ScheduleScout : ScheduleMove
	{
		public ScheduleScout() : base("Scout")
		{
			AddTask(OnFindPath);
			AddTask(OnMove);
			AddInterrupt("HasObstacle");
		}

		private bool OnFindPath()
		{
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Проверяем в памяти, может ранее доводилось видеть врага!?
			BackboardData data = _backboard.Find("EnemyVisible");
			if (data.isValid)
			{
				target = WayMap.Current.FindNearestPoint(data.position);
				_backboard.Remove(data);
			}
			
			// Строим маршрут.
			BuildWay(WayMap.Current.FindNearestPoint(_control.Position), target);
			return true;
		}
	}
}