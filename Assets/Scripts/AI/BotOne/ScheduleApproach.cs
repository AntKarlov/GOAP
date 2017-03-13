using Game.Map;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для приближения к врагу.
	/// </summary>
	public class ScheduleApproach : ScheduleMove
	{
		public ScheduleApproach() : base("Approach")
		{
			AddTask(OnFindPath);
			AddTask(OnMove);
		}

		private bool OnFindPath()
		{
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Извлекаем из памяти последнее положение врага.
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