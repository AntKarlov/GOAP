using UnityEngine;
using Game.Map;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для подбора патронов.
	/// </summary>
	public class SchedulePickupBomb : ScheduleMove
	{
		public SchedulePickupBomb() : base("PickupBomb")
		{
			AddTask(OnFindPath);
			AddTask(OnMove);
		}

		public override void Start(GameObject aObject)
		{
			base.Start(aObject);
			// Включаем магнит.
			_magnet.magnetKind = ItemKind.Bomb;
		}

		public override void Stop(GameObject aObject)
		{
			base.Stop(aObject);
			// Выключаем магнит.
			_magnet.magnetKind = ItemKind.None;
		}

		private bool OnFindPath()
		{
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Бежим к бомбе!
			BackboardData data = _backboard.Find("BombInlineOfSight");
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