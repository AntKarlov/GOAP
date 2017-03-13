using UnityEngine;
using Anthill.AI;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для стрельбы.
	/// </summary>
	public class ScheduleShot : AntAISchedule
	{
		private TankControl _control;
		private float _delay;

		public ScheduleShot() : base("Shot")
		{
			AddTask(OnShot);
			AddTask(OnDelay);
		}

		public override void Start(GameObject aObject)
		{
			_control = aObject.GetComponent<TankControl>();
			_delay = 0.3f;
		}

		private bool OnShot()
		{
			_control.isFire = true;
			return true;
		}

		private bool OnDelay()
		{
			_delay -= Time.deltaTime;
			return (_delay <= 0.0f);
		}
	}
}