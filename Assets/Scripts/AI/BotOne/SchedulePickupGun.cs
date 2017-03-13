using UnityEngine;
using Game.Map;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для подбора оружия.
	/// </summary>
	public class SchedulePickupGun : ScheduleMove
	{
		public SchedulePickupGun() : base("PickupGun")
		{
			AddTask(OnFindPath);
			AddTask(OnMove);
		}

		public override void Start(GameObject aObject)
		{
			base.Start(aObject);
			// Включаем магнит.
			_magnet.magnetKind = ItemKind.Gun;
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

			// Бежим к пушке!
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