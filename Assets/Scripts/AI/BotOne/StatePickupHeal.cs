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
			if (_blackboard["HealInlineOfSight"].AsBool)
			{
				target = WayMap.Current.FindNearestPoint(_blackboard["HealInlineOfSight_Pos"].AsVector2);
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