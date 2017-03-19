using UnityEngine;
using Anthill.AI;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Состояние стрельбы.
	/// </summary>
	public class StateShot : AntAIState
	{
		private TankControl _control;
		private float _delay;

		public StateShot(GameObject aObject) : base("Shot")
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