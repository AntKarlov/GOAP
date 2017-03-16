using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Anthill.AI
{
	[CustomPropertyDrawer(typeof(AntAIScenarioCondition))]
	public class AntAIConditionsInspector : PropertyDrawer
	{
		private const float FIELD_PADDING = 2.0f;
		private const string LIST_PROPERTY_NAME = "list";
		private const string NAME_PROPERTY = "name";
		private const string ID_PROPERTY = "id";
		private const string SERIAL_ID_PROPERTY = "serialId";

		private ReorderableList _list;

		#region Unity Callbacks

		public override void OnGUI(Rect aPosition, SerializedProperty aProperty, GUIContent aLabel)
		{
			var list = GetList(aProperty);
			list.DoList(aPosition);
		}

		public override float GetPropertyHeight(SerializedProperty aProperty, GUIContent aLabel)
		{
			var conditions = GetList(aProperty);
			return conditions.GetHeight();
		}

		#endregion
		#region Private Methods

		private ReorderableList GetList(SerializedProperty aProperty)
		{
			if (_list == null)
			{
				var listProperty = aProperty.FindPropertyRelative(LIST_PROPERTY_NAME);
				_list = new ReorderableList(aProperty.serializedObject, listProperty);
				
				_list.drawHeaderCallback = (Rect aRect) =>
				{
					EditorGUI.LabelField(aRect, "Conditions");
				};

				_list.drawElementCallback = (Rect aRect, int aIndex, bool aIsActive, bool aIsFocused) =>
				{
					aRect.y += FIELD_PADDING;
					string name = listProperty.GetArrayElementAtIndex(aIndex).FindPropertyRelative(NAME_PROPERTY).stringValue;
					Rect r = new Rect(aRect.x, aRect.y, aRect.width, EditorGUIUtility.singleLineHeight);
					listProperty.GetArrayElementAtIndex(aIndex).FindPropertyRelative(NAME_PROPERTY).stringValue = EditorGUI.TextField(r, name);
				};

				_list.onAddCallback = (ReorderableList aList) =>
				{
					var conditions = aProperty.FindPropertyRelative(LIST_PROPERTY_NAME);
					int id = aProperty.FindPropertyRelative(SERIAL_ID_PROPERTY).intValue;
					id++;
					aProperty.FindPropertyRelative(SERIAL_ID_PROPERTY).intValue = id;

					int length = conditions.arraySize;
					conditions.InsertArrayElementAtIndex(length);
					conditions.GetArrayElementAtIndex(length).FindPropertyRelative(ID_PROPERTY).intValue = id;
					conditions.GetArrayElementAtIndex(length).FindPropertyRelative(NAME_PROPERTY).stringValue = "<Unnamed>";
				};
			}

			return _list;
		}

		#endregion
	}
}