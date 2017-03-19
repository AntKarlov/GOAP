using UnityEngine;
using Anthill.Core;

namespace Game.Components
{
	public class Spawner : MonoBehaviour
	{
		[Tooltip("Префаб танка который будет заспавнен этим спавнером.")]
		public GameObject tankPrefab;
		
		[Tooltip("Задержка перед спавном танка после его гибили.")]
		public float spawnDelay = 5.0f;

		public AntEntity entity { get; set; }
		public bool IsActive { get; set; }
		public float Delay { get; set; }

		private Transform _t;
		
		private void Awake()
		{
			_t = GetComponent<Transform>();
			IsActive = true;
		}

		public void Spawn()
		{
			GameObject go = GameObject.Instantiate((GameObject) tankPrefab);
			go.GetComponent<Transform>().rotation = _t.rotation;
			go.GetComponent<Transform>().position = _t.position;
			entity = go.GetComponent<AntEntity>();
			AntEngine.Current.AddEntity(entity);
		}
	}
}