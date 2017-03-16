using System;
using UnityEngine;
using UnityEditor;

namespace Anthill.AI
{
	[CustomEditor(typeof(AntAIScenario))]
	public class AntAIScenarioEditor : Editor
	{
		private AntAIScenario _self;
		private GUIStyle _rowStyleA;
		private GUIStyle _rowStyleB;
		private GUIStyle _rowStyleC;

		#region Unity Callbacks

		private void OnEnable()
		{
			_self = (AntAIScenario) target;

			_rowStyleA = new GUIStyle();
			_rowStyleA.alignment = TextAnchor.MiddleLeft;
			_rowStyleA.normal.background = CreateBG(0.225f);

			_rowStyleB = new GUIStyle();
			_rowStyleB.alignment = TextAnchor.MiddleLeft;
			_rowStyleB.normal.background = CreateBG(0.255f); // 0.285f

			_rowStyleC = new GUIStyle();
			_rowStyleC.alignment = TextAnchor.MiddleLeft;
			_rowStyleC.normal.background = CreateBG(0.295f); // 0.325f
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			Color c = GUI.color;
			int delIndex = -1;

			// -- Begin Actions
			GUILayout.BeginVertical();
			EditorGUILayout.LabelField("Actions:", EditorStyles.boldLabel);

			AntAIScenarioAction action;
			if (_self.actions.Length == 0)
			{
				GUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("The Actions List is Empty.");
				GUILayout.EndVertical();
			}

			string actionName = "";
			for (int i = 0, n = _self.actions.Length; i < n; i++)
			{
				GUILayout.BeginVertical("box");

				action = _self.actions[i];
				GUILayout.BeginVertical(_rowStyleA);
				GUILayout.BeginHorizontal();
				GUI.color = c * new Color(1.0f, 1.0f, 0.5f);
				if (action.cost > 0)
				{
					EditorGUILayout.LabelField(string.Format("{0} [{1}]", action.name, action.cost), EditorStyles.boldLabel);
				}
				else
				{
					EditorGUILayout.LabelField(action.name, EditorStyles.boldLabel);
				}
				
				GUI.color = c;

				if (GUILayout.Button((action.showSettings) ? "-" : "+", GUILayout.MaxWidth(18.0f), GUILayout.MaxHeight(16.0f)))
				{
					action.showSettings = !action.showSettings;
				}
				
				GUI.color = c * new Color(1.0f, 1.0f, 0.5f);
				if (GUILayout.Button("x", GUILayout.MaxWidth(18.0f), GUILayout.MaxHeight(16.0f)))
				{
					delIndex = i;
				}
				GUI.color = c;
				GUILayout.EndHorizontal();
				GUILayout.Space(2.0f);
				GUILayout.EndVertical();

				if (action.showSettings)
				{
					GUILayout.Space(2.0f);

					actionName = EditorGUILayout.TextField("Action", action.name);
					if (!string.Equals(actionName, action.name))
					{
						if (string.Equals(action.name, action.task))
						{
							action.task = actionName;
						}
						action.name = actionName;
					}
					action.task = EditorGUILayout.TextField("Task", action.task);
					action.cost = EditorGUILayout.IntField("Cost", action.cost);
					
					GUILayout.Space(10.0f);
					DrawConditionsList("Pre Conditions", ref action.preConditions);
					GUILayout.Space(10.0f);
					DrawConditionsList("Post Conditions", ref action.postConditions);

					GUILayout.Space(2.0f);
				}
				GUILayout.EndVertical();
				//GUILayout.Space(4.0f);
			}

			if (GUILayout.Button("Add Action"))
			{
				AddToArray<AntAIScenarioAction>(ref _self.actions, new AntAIScenarioAction());
			}

			if (delIndex > -1)
			{
				RemoveFromArrayAt<AntAIScenarioAction>(ref _self.actions, delIndex);
			}

			GUILayout.EndVertical();
			// -- End Actions

			// -- Begin Goal
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Goals:", EditorStyles.boldLabel);
			GUILayout.BeginVertical("box");
			DrawConditionsList("Add Goal", ref _self.goals);
			GUILayout.EndVertical();
			// -- End Goal

			EditorGUILayout.Separator();
			base.OnInspectorGUI();
			
			EditorUtility.SetDirty(_self);
			serializedObject.ApplyModifiedProperties();
		}

		#endregion
		#region Private Methods

		private void DrawConditionsList(string aLabel, ref AntAIScenarioActionItem[] aConditions)
		{
			Color c = GUI.color;
			GUILayout.BeginVertical(_rowStyleA);
			
			var list = new string[_self.condition.list.Length + 1];
			list[0] = "- Select to Add -";
			for (int i = 1, n = list.Length; i < n; i++)
			{
				list[i] = _self.condition.list[i - 1].name;
			}

			int addIndex = EditorGUILayout.Popup(aLabel, 0, list);
			if (addIndex > 0)
			{
				int id = GetConditionIndex(list[addIndex]);
				if (id > -1)
				{
					int index = Array.FindIndex(aConditions, x => x.id == id);
					if (index < 0 || index >= aConditions.Length)
					{
						var item = new AntAIScenarioActionItem() {
							id = id,
							value = true
						};
						AddToArray<AntAIScenarioActionItem>(ref aConditions, item);
					}
				}
			}
			GUILayout.EndVertical();
			
			int delIndex = -1;
			for (int i = 0, n = aConditions.Length; i < n; i++)
			{
				GUILayout.BeginVertical((i % 2 == 0) ? _rowStyleB : _rowStyleC);
				GUILayout.BeginHorizontal();
				GUI.color = c * ((aConditions[i].value) ? new Color(0.5f, 1.0f, 0.5f) : new Color(1.0f, 0.5f, 0.5f)); // green : red
				if (GUILayout.Button((aConditions[i].value) ? "I" : "O", GUILayout.MaxWidth(20.0f), GUILayout.MaxHeight(16.0f)))
				{
					aConditions[i].value = !aConditions[i].value;
				}
				GUILayout.Label(_self.condition.list[aConditions[i].id].name);
				GUI.color = c;
				if (GUILayout.Button("x", GUILayout.MaxWidth(20.0f), GUILayout.MaxHeight(16.0f)))
				{
					delIndex = i;
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(2.0f);
				GUILayout.EndVertical();
			}

			if (delIndex > -1)
			{
				RemoveFromArrayAt<AntAIScenarioActionItem>(ref aConditions, delIndex);
			}
		}

		private int GetConditionIndex(string aConditionName)
		{
			int index = Array.FindIndex(_self.condition.list, x => string.Equals(x.name, aConditionName));
			return (index >= 0 && index < _self.condition.list.Length) ? index : -1;
		}

		private void AddToArray<T>(ref T[] aSource, T aItem)
		{
			var newArray = new T[aSource.Length + 1];
			for (int i = 0, n = aSource.Length; i < n; i++)
			{
				newArray[i] = aSource[i];
			}
			newArray[newArray.Length - 1] = aItem;
			aSource = newArray;
		}

		private void RemoveFromArrayAt<T>(ref T[] aSource, int aRemoveIndex)
		{
			int index = 0;
			var newArray = new T[aSource.Length - 1];
			for (int i = 0, n = aSource.Length; i < n; i++)
			{
				if (i != aRemoveIndex)
				{
					newArray[index] = aSource[i];
					index++;
				}
			}
			aSource = newArray;
		}

		private Texture2D CreateBG(float aColorValue)
		{
			Texture2D bg = new Texture2D(1, 1);
			bg.SetPixel(0, 0, GUI.color * new Color(aColorValue, aColorValue, aColorValue));
			bg.Apply();
			return bg;
		}

		#endregion
	}
}