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
	public class BotSetup : MonoBehaviour
	{
		[Tooltip("Сценарий поведения бота и его цели.")]
		public AntAIScenario scenario;
		[Tooltip("Задержка между циклами обновления состояния мира и обдумывания нового плана.")]
		public float updateInterval = 0.2f;

		private void Awake()
		{
			// Настраиваем ИИ.
			var control = GetComponent<AIControl>();
			control.updateInterval = updateInterval;
			
			// Обертка для всех «органов чувств».
			control.sense = new BotSense(gameObject);

			// Обертка для принятия решений (сценарий поведения).
			control.logic = new AntAILogic(scenario);

			// Доступные наборы действий.
			control.tasks = new AntAITask[]
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

			control.DefaultTaskIs("Idle");
		}
	}
}