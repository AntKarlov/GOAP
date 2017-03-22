using UnityEngine;
using Game.Map;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Состояние подбора бомбы.
	/// </summary>
	public class StatePickupBomb : StateMove
	{
		public StatePickupBomb(GameObject aObject) : base(aObject, "PickupBomb")
		{
			//..
		}

		public override void Start()
		{
			base.Start();
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Бежим к бомбе!
			if (_blackboard["BombInlineOfSight"].AsBool)
			{
				target = WayMap.Current.FindNearestPoint(_blackboard["BombInlineOfSight_Pos"].AsVector2);
			}
			
			// Строим маршрут.
			BuildWay(WayMap.Current.FindNearestPoint(_control.Position), target);
			
			// Включаем магнит.
			_magnet.magnetKind = ItemKind.Bomb;
		}

		public override void Update(float aDeltaTime)
		{
			OnMove();
		}

		public override void Stop()
		{
			base.Stop();
			// Выключаем магнит.
			_magnet.magnetKind = ItemKind.None;
		}
	}
}