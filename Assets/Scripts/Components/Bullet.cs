using UnityEngine;

namespace Game.Components
{
	public class Bullet : MonoBehaviour
	{
		private void OnCollisionEnter2D(Collision2D aCollision)
		{
			// Пуля дамажит и удаляется прям здесь, но, по хорошему, обработку урона и пуль
			// следует реализовать через системы.
			Health health = aCollision.contacts[0].collider.GetComponent<Health>();
			if (health != null)
			{
				health.HP -= 1.0f;
			}
			DestroyObject(this.gameObject);
		}
	}
}