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
			_control.schedules = new AntAISchedule[]
			{
				new ScheduleIdle(),
				new ScheduleSearchGun(),
				new SchedulePickupGun(),
				new ScheduleSearchAmmo(),
				new SchedulePickupAmmo(),
				new SchedulePickupBomb(),
				new ScheduleSearchHeal(),
				new SchedulePickupHeal(),
				new ScheduleScout(),
				new ScheduleAim(),
				new ScheduleShot(),
				new ScheduleApproach(),
				new ScheduleDetonateBomb()
			};
		}
	}
}