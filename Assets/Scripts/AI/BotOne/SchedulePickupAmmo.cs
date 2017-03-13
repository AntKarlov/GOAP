using UnityEngine;
using Game.Map;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для подбора патронов.
	/// </summary>
	public class SchedulePickupAmmo : ScheduleMove
	{
		public SchedulePickupAmmo() : base("PickupAmmo")
		{
			AddTask(OnFindPath);
			AddTask(OnMove);
		}

		public override void Start(GameObject aObject)
		{
			base.Start(aObject);
			// Включаем магнит.
			_magnet.magnetKind = ItemKind.Ammo;
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

			// Бежим к патронам!
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