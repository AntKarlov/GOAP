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

			DrawActionList();
			DrawGoalList();

			EditorGUILayout.Separator();
			base.OnInspectorGUI();
			
			EditorUtility.SetDirty(_self);
			serializedObject.ApplyModifiedProperties();
		}

		#endregion
		#region Private Methods

		private void DrawActionList()
		{
			Color c = GUI.color;
			int delIndex = -1;

			GUILayout.BeginVertical();
			EditorGUILayout.LabelField("Action List", EditorStyles.boldLabel);

			AntAIScenarioAction action;
			if (_self.actions.Length == 0)
			{
				GUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("The Action List is Empty.");
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

				if (GUILayout.Button((action.isOpened) ? "-" : "+", GUILayout.MaxWidth(18.0f), GUILayout.MaxHeight(16.0f)))
				{
					action.isOpened = !action.isOpened;
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

				if (action.isOpened)
				{
					GUILayout.Space(2.0f);

					actionName = EditorGUILayout.TextField("Action", action.name);
					if (!string.Equals(actionName, action.name))
					{
						if (string.Equals(action.name, action.state))
						{
							action.state = actionName;
						}
						action.name = actionName;
					}
					action.state = EditorGUILayout.TextField("State", action.state);
					action.cost = EditorGUILayout.IntField("Cost", action.cost);
					
					GUILayout.Space(10.0f);
					DrawConditionsList("Pre Conditions", ref action.pre);
					GUILayout.Space(10.0f);
					DrawConditionsList("Post Conditions", ref action.post);

					GUILayout.Space(2.0f);
				}
				GUILayout.EndVertical();
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
		}

		private void DrawGoalList()
		{
			Color c = GUI.color;
			int delIndex = -1;

			EditorGUILayout.Separator();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("Goal List", EditorStyles.boldLabel);

			AntAIScenarioGoal goal;
			if (_self.goals.Length == 0)
			{
				GUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("The Goal List is Empty.");
				GUILayout.EndVertical();
			}

			for (int i = 0, n = _self.goals.Length; i < n; i++)
			{
				GUILayout.BeginVertical("box");

				goal = _self.goals[i];
				GUILayout.BeginVertical(_rowStyleA);
				GUILayout.BeginHorizontal();
				GUI.color = c * new Color(1.0f, 1.0f, 0.5f);
				EditorGUILayout.LabelField(goal.name, EditorStyles.boldLabel);
				GUI.color = c;

				if (GUILayout.Button((goal.isOpened) ? "-" : "+", GUILayout.MaxWidth(18.0f), GUILayout.MaxHeight(16.0f)))
				{
					goal.isOpened = !goal.isOpened;
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

				if (goal.isOpened)
				{
					GUILayout.Space(2.0f);
					goal.name = EditorGUILayout.TextField("Goal", goal.name);
					GUILayout.Space(10.0f);
					DrawConditionsList("Conditions", ref goal.conditions);
					GUILayout.Space(2.0f);
				}

				GUILayout.EndVertical();
			}

			if (GUILayout.Button("Add Goal"))
			{
				AddToArray<AntAIScenarioGoal>(ref _self.goals, new AntAIScenarioGoal());
			}

			if (delIndex > -1)
			{
				RemoveFromArrayAt<AntAIScenarioGoal>(ref _self.goals, delIndex);
			}

			EditorGUILayout.EndVertical();
		}

		private void DrawConditionsList(string aLabel, ref AntAIScenarioItem[] aConditions)
		{
			Color c = GUI.color;
			GUILayout.BeginVertical(_rowStyleA);
			
			var list = new string[_self.conditions.list.Length + 1];
			list[0] = "- Select to Add -";
			for (int i = 1, n = list.Length; i < n; i++)
			{
				list[i] = _self.conditions.list[i - 1].name;
			}

			int addIndex = EditorGUILayout.Popup(aLabel, 0, list);
			if (addIndex > 0)
			{
				var item = new AntAIScenarioItem() {
					id = _self.conditions.GetID(list[addIndex]),
					value = true
				};
				AddToArray<AntAIScenarioItem>(ref aConditions, item);
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
				GUILayout.Label(_self.conditions.GetName(aConditions[i].id));
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
				RemoveFromArrayAt<AntAIScenarioItem>(ref aConditions, delIndex);
			}
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