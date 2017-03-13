using UnityEngine;

namespace Game.Components
{
	public class Visual : MonoBehaviour
	{
		[Tooltip("Имя условия которое определяет данный визуальный объект когда его видят. Например, если это враг то EnemyVisible.")]
		public string conditionName = "EnemyVisible";
		[Tooltip("Группа к которой относится данный объект.")]
		public GroupEnum group = GroupEnum.None;
	}
}