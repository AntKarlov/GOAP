using UnityEngine;
using Game.Map;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Состояние патрулирования.
	/// </summary>
	public class StateScout : StateMove
	{
		public StateScout(GameObject aObject) : base(aObject, "Scout")
		{
			AddInterrupt("HasObstacle");
		}

		public override void Start()
		{
			base.Start();
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Если доводилось видеть ранее врага, то пробуем сгонять туда где его видели.
			if (_blackboard["EnemyVisible"].AsBool)
			{
				target = WayMap.Current.FindNearestPoint(_blackboard["EnemyVisible_Pos"].AsVector2);
				_blackboard["EnemyVisible"].AsBool = false;
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