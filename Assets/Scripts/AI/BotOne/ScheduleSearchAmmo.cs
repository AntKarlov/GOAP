using Game.Map;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для поиска патронов.
	/// </summary>
	public class ScheduleSearchAmmo : ScheduleMove
	{
		public ScheduleSearchAmmo() : base("SearchAmmo")
		{
			AddTask(OnFindPath);
			AddTask(OnMove);
			AddInterrupt("HasObstacle");
		}

		private bool OnFindPath()
		{
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Проверяем в памяти, может ранее доводилось видеть патроны!?
			BackboardData data = _backboard.Find("AmmoInlineOfSight");
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