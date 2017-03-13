using Game.Map;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для поиска пушки.
	/// </summary>
	public class ScheduleSearchGun : ScheduleMove
	{
		public ScheduleSearchGun() : base("SearchGun")
		{
			AddTask(OnFindPath);
			AddTask(OnMove);
			AddInterrupt("HasObstacle");
		}

		private bool OnFindPath()
		{
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Проверяем в памяти, может ранее доводилось видеть пушку!?
			BackboardData data = _backboard.Find("GunInlineOfSight");
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