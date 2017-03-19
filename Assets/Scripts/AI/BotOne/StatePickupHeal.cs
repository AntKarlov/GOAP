using UnityEngine;
using Game.Map;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Состояние подбора хилки.
	/// </summary>
	public class StatePickupHeal : StateMove
	{
		public StatePickupHeal(GameObject aObject) : base(aObject, "PickupHeal")
		{
			// ..
		}

		public override void Start()
		{
			base.Start();
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

			// Включаем магнит.
			_magnet.magnetKind = ItemKind.Heal;
		}

		public override void Update(float aDeltaTime)
		{
			_isFinished = OnMove();
		}

		public override void Stop()
		{
			base.Stop();
			// Выключаем магнит.
			_magnet.magnetKind = ItemKind.None;
		}
	}
}