using UnityEngine;
using Game.Map;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Состояние подбора пушки.
	/// </summary>
	public class StatePickupGun : StateMove
	{
		public StatePickupGun(GameObject aObject) : base(aObject, "PickupGun")
		{
			// ..
		}

		public override void Start()
		{
			base.Start();
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Бежим к пушке!
			if (_blackboard["GunInlineOfSight"].AsBool)
			{
				target = WayMap.Current.FindNearestPoint(_blackboard["GunInlineOfSight_Pos"].AsVector2);
			}
			
			// Строим маршрут.
			BuildWay(WayMap.Current.FindNearestPoint(_control.Position), target);
			
			// Включаем магнит.
			_magnet.magnetKind = ItemKind.Gun;
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