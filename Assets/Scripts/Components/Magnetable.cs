using UnityEngine;

namespace Game.Components
{
	/// <summary>
	/// Данный компонент определяет что игровой объект обладающий этим компонентом
	/// может быть притянут другим игровым объектом который обладает компонентом Magnet.
	/// Реализация магнитизма в Systems/MagnetSystem.cs
	/// </summary>
	public class Magnetable : MonoBehaviour
	{
		[Tooltip("Тип вещи.")]
		public ItemKind kind = ItemKind.None;
		[Tooltip("Модификатор влияющий на силу притяжения.")]
		public float forceScale = 1.0f;

		[HideInInspector] public Rigidbody2D body;
		[HideInInspector] public float lifeTime;

		private void Awake()
		{
			body = GetComponent<Rigidbody2D>();
			lifeTime = 0.0f;
		}
	}

	public enum ItemKind
	{
		None,
		All,
		Heal,
		Ammo,
		Gun,
		Bomb
	}
}