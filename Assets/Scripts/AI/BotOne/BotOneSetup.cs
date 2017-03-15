using UnityEngine;
using Anthill.AI;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Данный компонент позволяет связать определенный набор действий 
	/// непосредственно с игровым объектом. Для связи необходимо назначить
	/// этот класс как компонент к игровому объекту.
	/// </summary>
	[RequireComponent(typeof(AIControl))]
	public class BotOneSetup : MonoBehaviour
	{
		[Tooltip("Задержка между циклами обновления состояния мира и обдумывания нового плана.")]
		public float updateInterval = 0.2f;
		private AIControl _control;

		private void Awake()
		{
			// Настраиваем ИИ.
			_control = GetComponent<AIControl>();
			_control.updateInterval = updateInterval;
			
			// Обертка для всех «органов чувств».
			_control.sense = new BotSense(gameObject);

			// Обертка для принятия решений (сценарий поведения).
			_control.logic = new BotLogic(gameObject);

			// Доступные наборы действий.
			_control.tasks = new AntAITask[]
			{
				new TaskIdle(gameObject),
				new TaskSearchGun(gameObject),
				new TaskPickupGun(gameObject),
				new TaskSearchAmmo(gameObject),
				new TaskPickupAmmo(gameObject),
				new TaskPickupBomb(gameObject),
				new TaskSearchHeal(gameObject),
				new TaskPickupHeal(gameObject),
				new TaskScout(gameObject),
				new TaskAim(gameObject),
				new TaskShot(gameObject),
				new TaskApproach(gameObject),
				new TaskDetonateBomb(gameObject)
			};

			_control.DefaultTaskIs("Idle");
		}
	}
}