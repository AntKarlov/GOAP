using UnityEngine;
using Game.Map;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для подбора оружия.
	/// </summary>
	public class TaskPickupGun : TaskMove
	{
		public TaskPickupGun(GameObject aObject) : base(aObject, "PickupGun")
		{
			// ..
		}

		public override void Start()
		{
			base.Start();
			WayPoint target = WayMap.Current.GetRandomPoint();

			// Бежим к пушке!
			BackboardData data = _backboard.Find("GunInlineOfSight");
			if (data.isValid)
			{
				target = WayMap.Current.FindNearestPoint(data.position);
				_backboard.Remove(data);
			}
			
			// Строим маршрут.
			BuildWay(WayMap.Current.FindNearestPoint(_control.Position), target);
			
			// Включаем магнит.
			_magnet.magnetKind = ItemKind.Gun;
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