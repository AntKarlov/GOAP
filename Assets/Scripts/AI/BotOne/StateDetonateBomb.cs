using UnityEngine;
using Anthill.AI;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Состояние активации бомбы.
	/// </summary>
	public class StateDetonateBomb : AntAIState
	{
		private TankControl _control;

		public StateDetonateBomb(GameObject aObject) : base("DetonateBomb")
		{
			_control = aObject.GetComponent<TankControl>();
		}

		public override void Start()
		{
			_control.isFire = true;
		}
	}
}