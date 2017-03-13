using UnityEngine;
using Game.Map;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для подбора лечилки.
	/// </summary>
	public class SchedulePickupHeal : ScheduleMove
	{
		public SchedulePickupHeal() : base("PickupHeal")
		{
			AddTask(OnFindPath);
			AddTask(OnMove);
		}

		public override void Start(GameObject aObject)
		{
			base.Start(aObject);
			// Включаем магнит.
			_magnet.magnetKind = ItemKind.Heal;
		}

		public override void Stop(GameObject aObject)
		{
			base.Stop(aObject);
			// Выключаем магнит.
			_magnet.magnetKind = ItemKind.Heal;
		}

		private bool OnFindPath()
		{
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Бежим к лечилке!
			BackboardData data = _backboard.Find("HealInlineOfSight");
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