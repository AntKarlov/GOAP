using UnityEngine;
using Anthill.AI;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Данный компонент позволяет связать определенный набор действий 
	/// непосредственно с игровым объектом. Для связи необходимо назначить
	/// этот класс как компонент к игровому объекту.
	///
	/// Все настройки бота можно реализовать в классе-компоненте Game/Components/AIControl.
	/// Но, поскольку данный пример использует Entity-архитектуру — это сделать не возможно,
	/// так как для всех ботов класс AIControl должен быть общим, потому что он является
	/// основой по которой определяется, что конкретный объект управляется ИИ. Но, при этом
	/// отдельные боты могут иметь уникальные состояния (State) и сценарии, и именно поэтому
	/// для каждого типа ботов должен быть свой BotSetup класс.
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

			// Регистриуем возможные состояния.
			control.states = new AntAIState[] 
			{
				new StateIdle(gameObject),
				new StateSearchGun(gameObject),
				new StatePickupGun(gameObject),
				new StateSearchAmmo(gameObject),
				new StatePickupAmmo(gameObject),
				new StatePickupBomb(gameObject),
				new StateSearchHeal(gameObject),
				new StatePickupHeal(gameObject),
				new StateScout(gameObject),
				new StateAim(gameObject),
				new StateShot(gameObject),
				new StateApproach(gameObject),
				new StateDetonateBomb(gameObject)
			};

			control.DefaultStateIs("Idle");
		}
	}
}