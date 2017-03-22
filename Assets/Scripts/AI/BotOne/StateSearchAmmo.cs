using UnityEngine;
using Game.Map;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Состояние поиска патронов.
	/// </summary>
	public class StateSearchAmmo : StateMove
	{
		public StateSearchAmmo(GameObject aObject) : base(aObject, "SearchAmmo")
		{
			AddInterrupt("HasObstacle");
		}

		public override void Start()
		{
			base.Start();
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Если доводилось видеть ранее патроны, то пробуем сгонять туда где их видели.
			if (_blackboard["AmmoInlineOfSight"].AsBool)
			{
				target = WayMap.Current.FindNearestPoint(_blackboard["AmmoInlineOfSight_Pos"].AsVector2);
				_blackboard["AmmoInlineOfSight"].AsBool = false;
			}
			
			// Строим маршрут.
			BuildWay(WayMap.Current.FindNearestPoint(_control.Position), target);
		}

		public override void Update(float aDeltaTime)
		{
			_isFinished = OnMove();
		}
	}
}