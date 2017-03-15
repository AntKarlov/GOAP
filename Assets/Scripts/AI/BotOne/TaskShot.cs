using UnityEngine;
using Anthill.AI;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для стрельбы.
	/// </summary>
	public class TaskShot : AntAITask
	{
		private TankControl _control;
		private float _delay;

		public TaskShot(GameObject aObject) : base("Shot")
		{
			_control = aObject.GetComponent<TankControl>();
		}

		public override void Start()
		{
			_control.isFire = true;
			_delay = 0.3f;
		}

		public override void Update(float aDeltaTime)
		{
			_delay -= aDeltaTime;
			_isFinished = (_delay <= 0.0f);
		}
	}
}