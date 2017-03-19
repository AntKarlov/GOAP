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
		public int RedKills = 0;
		public int GreenKills = 0;

		#region Unity Callbacks

		private void Start()
		{
			engine = new AntEngine();

			// Инициализация игровых систем в движок.
			engine.AddSystem(new MovementSystem(), SystemPriority.movementSystem);
			engine.AddSystem(new PlayerControlSystem(), SystemPriority.playerControlSystem);
			engine.AddSystem(new AIControlSystem(), SystemPriority.aiControlSystem);
			engine.AddSystem(new MagnetSystem(), SystemPriority.magnetSystem);
			engine.AddSystem(new HealthSystem(), SystemPriority.healthSystem);
			engine.AddSystem(new DropperSystem(), SystemPriority.dropperSystem);
			engine.AddSystem(new SpawnSystem(), SystemPriority.spawnSystem);

			// Инициализация игровых объектов находящихся на сцене.
			engine.AddEntitiesFromHierarchy(transform);
		}

		private void Update()
		{
			// Обработка всех игровых объектов добавленых в движок.
			engine.Update(Time.deltaTime);
		}

		private void OnGUI()
		{
			GUIStyle style = new GUIStyle();
			style.fontSize = 20;
			style.normal.textColor = Color.green;
			Rect gRect = new Rect(50.0f, Screen.height - 50.0f, 300.0f, 150.0f);
			GUI.Label(gRect, string.Format("{0}", RedKills), style);
			style.fontSize = 20;
			style.normal.textColor = Color.red;
			Rect rRect = new Rect(Screen.width - 50.0f, Screen.height - 50.0f, 300.0f, 150.0f);
			GUI.Label(rRect, string.Format("{0}", GreenKills), style);
		}

		#endregion
	}
}