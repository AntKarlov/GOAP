using UnityEngine;

namespace Game.Components
{
	/// <summary>
	/// Игровой объект обладающий этим компонентом может притягивать к себе игровые объекты
	/// обладающие компонентом Magnetable. Реализация магнитизма в Systems/MagnetSystem.cs
	/// </summary>
	public class Magnet : MonoBehaviour
	{
		[Tooltip("Расстояние на котором магнит начинает реагировать.")]
		public float distance = 0.5f;

		[Tooltip("Сила притяжения магнита.")]
		public float force = 1.0f;

		[Tooltip("Дистанция при достижении которой объект считается собранным.")]
		public float collectDistance = 0.25f;
		
		[Tooltip("Тип объектов которые притягивает магнит. Боты автоматически выбирают тип объекта, для игрока нужен режим All.")]
		public ItemKind magnetKind = ItemKind.None;
	}
}