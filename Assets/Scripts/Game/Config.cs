using UnityEngine;

namespace Game.Core
{
	/// <summary>
	/// Класс содержащий различные отладочные опции.
	/// </summary>
	public class Config : MonoBehaviour
	{
		public static Config Instance { get; private set; }

		[Tooltip("Вкл. отображение карты перемещений.")]
		public bool showWayMap = false;
		[Tooltip("Вкл. отображение текущего маршрута.")]
		public bool showCurrentWay = true;
		[Tooltip("Вкл. отображения зрения.")]
		public bool showVision = true;

		private void Awake()
		{
			Instance = this;
		}
	}
}