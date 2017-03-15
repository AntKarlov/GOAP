using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор задач для состояния IDLE.
	/// </summary>
	public class TaskIdle : AntAITask
	{
		private float _delay;

		public TaskIdle(GameObject aObject) : base("Idle")
		{
			// ..
		}

		public override void Start()
		{
			// Задаем случайный промежуток времени.
			_delay = AntMath.RandomRangeFloat(2.0f, 5.0f);
		}

		public override void Update(float aDeltaTime)
		{
			// Если время истекло, то задача выполнена.
			_delay -= aDeltaTime;
			_isFinished = (_delay < 0.0f);
		}
	}
}