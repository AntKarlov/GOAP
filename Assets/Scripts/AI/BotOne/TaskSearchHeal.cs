using UnityEngine;
using Game.Map;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для поиска патронов.
	/// </summary>
	public class TaskSearchHeal : TaskMove
	{
		public TaskSearchHeal(GameObject aObject) : base(aObject, "SearchHeal")
		{
			AddInterrupt("HasObstacle");
		}

		public override void Start()
		{
			base.Start();
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Проверяем в памяти, может ранее доводилось видеть патроны!?
			BackboardData data = _backboard.Find("HealInlineOfSight");
			if (data.isValid)
			{
				target = WayMap.Current.FindNearestPoint(data.position);
				_backboard.Remove(data);
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