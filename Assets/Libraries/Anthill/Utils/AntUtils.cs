#if UNITY_EDITOR
using System.Reflection;
using UnityEditorInternal;

namespace Anthill.Utils
{
	public class AntUtils
	{
		/// <summary>
		/// Возвращает массив имен сортировочных слоев доступных в редакторе.
		/// </summary>
		/// <returns>Массив имен сортировочных слоев.</returns>
		public static string[] GetSortingLayerNames()
		{
			System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			return (string[])sortingLayersProperty.GetValue(null, new object[0]);
		}

		/// <summary>
		/// Вовзаращет массив индексов сортировочных слоев доступных в редакторе.
		/// </summary>
		/// <returns>Массив индексов сортировочных слоев.</returns>
		public static int[] GetSortingLayerUniqueIDs()
		{
			System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
			return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
		}
	}
}
#endif