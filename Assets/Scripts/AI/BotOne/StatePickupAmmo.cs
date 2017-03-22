using UnityEngine;
using Game.Map;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Состояние подбора патронов.
	/// </summary>
	public class StatePickupAmmo : StateMove
	{
		public StatePickupAmmo(GameObject aObject) : base(aObject, "PickupAmmo")
		{
			// ..
		}

		public override void Start()
		{
			base.Start();
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Бежим к патронам!
			if (_blackboard["AmmoInlineOfSight"].AsBool)
			{
				target = WayMap.Current.FindNearestPoint(_blackboard["AmmoInlineOfSight_Pos"].AsVector2);
			}
			
			// Строим маршрут.
			BuildWay(WayMap.Current.FindNearestPoint(_control.Position), target);
			
			// Включаем магнит.
			_magnet.magnetKind = ItemKind.Ammo;
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