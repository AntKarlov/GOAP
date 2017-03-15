using UnityEngine;
using Game.Map;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для поиска врага (патрулирования).
	/// </summary>
	public class TaskScout : TaskMove
	{
		public TaskScout(GameObject aObject) : base(aObject, "Scout")
		{
			AddInterrupt("HasObstacle");
		}

		public override void Start()
		{
			base.Start();
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Проверяем в памяти, может ранее доводилось видеть врага!?
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