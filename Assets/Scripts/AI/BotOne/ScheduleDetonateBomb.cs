using UnityEngine;
using Anthill.AI;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для активации бомбы.
	/// </summary>
	public class ScheduleDetonateBomb : AntAISchedule
	{
		private TankControl _control;

		public ScheduleDetonateBomb() : base("DetonateBomb")
		{
			AddTask(OnDetonate);
		}

		public override void Start(GameObject aObject)
		{
			_control = aObject.GetComponent<TankControl>();
		}

		private bool OnDetonate()
		{
			_control.isFire = true;
			return true;
		}
	}
}