using UnityEngine;

namespace Game.Components
{
	public class Sensor : MonoBehaviour
	{
		public bool HasObstacle { get; set; }

		private void OnTriggerEnter2D(Collider2D aCollider)
		{
			HasObstacle = true;
		}
	}
}