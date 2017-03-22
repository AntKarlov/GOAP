using UnityEngine;
using Game.Map;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Состояние поиска хилки.
	/// </summary>
	public class StateSearchHeal : StateMove
	{
		public StateSearchHeal(GameObject aObject) : base(aObject, "SearchHeal")
		{
			AddInterrupt("HasObstacle");
		}

		public override void Start()
		{
			base.Start();
			WayPoint target = WayMap.Current.GetRandomPoint();

			if (_blackboard["HealInlineOfSight"].AsBool)
			{
				target = WayMap.Current.FindNearestPoint(_blackboard["HealInlineOfSight_Pos"].AsVector2);
				_blackboard["HealInlineOfSight"].AsBool = false;
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