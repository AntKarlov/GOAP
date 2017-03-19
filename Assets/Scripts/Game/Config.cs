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

		[Tooltip("Вкл. отображение зрения.")]
		public bool showVision = true;

		[Tooltip("Вкл. отображение перезарядки дропперов.")]
		public bool showDropperRecharge = true;
		
		[Tooltip("Вкл. отображение задержки перед спавном.")]
		public bool showSpawnerDelay = true;

		private void Awake()
		{
			Instance = this;
		}
	}
}