using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор задач для состояния IDLE.
	/// </summary>
	public class ScheduleIdle : AntAISchedule
	{
		private float _delay;

		public ScheduleIdle() : base("Idle", true)
		{
			AddTask(OnUpdate);
		}

		public override void Start(GameObject aOwner)
		{
			// Задаем случайный промежуток времени.
			_delay = AntMath.RandomRangeFloat(2.0f, 5.0f);
		}

		private bool OnUpdate()
		{
			// Если время истекло, то задача выполнена.
			_delay -= Time.deltaTime;
			return (_delay < 0.0f);
		}
	}
}