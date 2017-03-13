using UnityEngine;
using Anthill.Core;
using Game.Systems;

namespace Game.Core
{
	/// <summary>
	/// Ядро игры.
	/// </summary>
	public class GameCore : MonoBehaviour
	{
		[Header("Some Prefabs")]
		public GameObject healthBarPrefab;
		public GameObject bombItemPrefab;
		public GameObject ammoItemPrefab;
		public GameObject gunItemPrefab;
		public GameObject healItemPrefab;
		
		public AntEngine engine;

		// -----------------------------------------------------
		// Unity callbacks
		// -----------------------------------------------------

		private void Start()
		{
			engine = new AntEngine();

			// Инициализация игровых систем в движок.
			engine.AddSystem(new MovementSystem(), SystemPriority.movementSystem);
			engine.AddSystem(new PlayerControlSystem(), SystemPriority.playerControlSystem);
			engine.AddSystem(new AIControlSystem(), SystemPriority.aiControlSystem);
			engine.AddSystem(new MagnetSystem(), SystemPriority.magnetSystem);
			engine.AddSystem(new HealthSystem(), SystemPriority.healthSystem);

			// Инициализация игровых объектов находящихся на сцене.
			engine.AddEntitiesFromHierarchy(transform);
		}

		private void Update()
		{
			// Обработка всех игровых объектов добавленых в движок.
			engine.Update(Time.deltaTime);
		}
	}
}