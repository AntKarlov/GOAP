using UnityEngine;
using Game.Map;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для приближения к врагу.
	/// </summary>
	public class TaskApproach : TaskMove
	{
		public TaskApproach(GameObject aObject) : base(aObject, "Approach")
		{
			// ..
		}

		public override void Start()
		{
			base.Start();
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Извлекаем из памяти последнее положение врага.
			BackboardData data = _backboard.Find("EnemyVisible");
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