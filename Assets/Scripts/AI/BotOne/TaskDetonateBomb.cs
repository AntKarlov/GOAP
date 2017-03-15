using UnityEngine;
using Anthill.AI;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для активации бомбы.
	/// </summary>
	public class TaskDetonateBomb : AntAITask
	{
		private TankControl _control;

		public TaskDetonateBomb(GameObject aObject) : base("DetonateBomb")
		{
			_control = aObject.GetComponent<TankControl>();
		}

		public override void Start()
		{
			_control.isFire = true;
		}
	}
}