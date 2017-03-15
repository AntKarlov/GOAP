using UnityEngine;
using Game.Map;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для поиска патронов.
	/// </summary>
	public class TaskSearchAmmo : TaskMove
	{
		public TaskSearchAmmo(GameObject aObject) : base(aObject, "SearchAmmo")
		{
			AddInterrupt("HasObstacle");
		}

		public override void Start()
		{
			base.Start();
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Проверяем в памяти, может ранее доводилось видеть патроны!?
			BackboardData data = _backboard.Find("AmmoInlineOfSight");
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