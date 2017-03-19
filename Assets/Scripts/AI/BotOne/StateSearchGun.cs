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

			// Проверяем в памяти, может ранее доводилось видеть пушку!?
			BackboardData data = _backboard.Find("GunInlineOfSight");
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