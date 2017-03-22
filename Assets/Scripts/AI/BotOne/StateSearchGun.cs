using UnityEngine;
using Game.Map;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Состояние поиска пушки.
	/// </summary>
	public class StateSearchGun : StateMove
	{
		public StateSearchGun(GameObject aObject) : base(aObject, "SearchGun")
		{
			AddInterrupt("HasObstacle");
		}

		public override void Start()
		{
			base.Start();
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Если доводилось видеть раньше патроны, то пробуем сгонять туда где их видели.
			if (_blackboard["GunInlineOfSight"].AsBool)
			{
				target = WayMap.Current.FindNearestPoint(_blackboard["GunInlineOfSight_Pos"].AsVector2);
				_blackboard["GunInlineOfSight"].AsBool = false;
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