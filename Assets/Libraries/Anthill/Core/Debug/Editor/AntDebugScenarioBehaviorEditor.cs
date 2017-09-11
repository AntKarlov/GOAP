using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Anthill.Core
{
	[CustomEditor(typeof(AntDebugScenarioBehaviour))]
	public class AntDebugScenarioBehaviourEditor : UnityEditor.Editor
	{
		private enum SortMode
		{
			OrderOfOccurrence,
			Name,
			NameDescending,
			ExecutionTime,
			ExecutionTimeDescending
		}

		private AntDebugScenarioBehaviour _self;
		private float _threshold;
		private SortMode _systemSortMode;

		/*private AntSystemMonitor _systemMonitor;
		private Queue<float> _systemMonitorData;
		private int _lastRenderedFrameCount;
		private const int SYSTEM_MONITOR_DATA_LENGTH = 60;*/

		private static bool _showInitializeSystems = true;
		private static bool _showExecuteSystems = true;
		private static string _systemNameSearchTerm = string.Empty;

		private void OnEnable()
		{
			_self = (AntDebugScenarioBehaviour) target;
		}

		public override void OnInspectorGUI()
		{
			//DrawSystemMonitor();
			DrawSystemList();
			EditorGUILayout.HelpBox("To disable debug tools just add the constant \"ANTHILL_DISABLE_DEBUG\" to the Player Settings.", MessageType.Info);

			EditorUtility.SetDirty(target);
		}

		/*private void DrawSystemMonitor()
		{
			if (_systemMonitor == null)
			{
				_systemMonitor = new AntSystemMonitor(SYSTEM_MONITOR_DATA_LENGTH);
				_systemMonitorData = new Queue<float>(new float[SYSTEM_MONITOR_DATA_LENGTH]);
			}

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				EditorGUILayout.LabelField("Execute Duration", EditorStyles.boldLabel);
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField("Total", _self.Scenario.TotalDuration.ToString());

					var buttonStyle = new GUIStyle(GUI.skin.button);
					if (_self.Scenario.isPaused)
					{
						buttonStyle.normal = GUI.skin.button.active;
					}

					if (GUILayout.Button("Pause", buttonStyle, GUILayout.Width(50.0f)))
					{
						_self.Scenario.isPaused = !_self.Scenario.isPaused;
					}

					if (GUILayout.Button("Step", GUILayout.Width(50.0f)))
					{
						_self.Scenario.isPaused = true;
						_self.Scenario.Step();
						AddDuration((float) _self.Scenario.TotalDuration);
						_systemMonitor.Draw(_systemMonitorData.ToArray(), 80.0f);
					}
				}
				EditorGUILayout.EndHorizontal();

				if (!EditorApplication.isPaused && _self.Scenario.isPaused)
				{
					AddDuration((float) _self.Scenario.TotalDuration);
				}

				_systemMonitor.Draw(_systemMonitorData.ToArray(), 80.0f);
			}
			EditorGUILayout.EndVertical();
		}

		private void AddDuration(float aDuration)
		{
			if (Time.renderedFrameCount != _lastRenderedFrameCount)
			{
				_lastRenderedFrameCount = Time.renderedFrameCount;
				if (_systemMonitorData.Count >= SYSTEM_MONITOR_DATA_LENGTH)
				{
					_systemMonitorData.Dequeue();
				}
				_systemMonitorData.Enqueue(aDuration);
			}
		}*/

		private void DrawSystemList()
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				EditorGUILayout.BeginHorizontal();
				{
					AntDebugScenario.avgResetDuration = (AvgResetDuration) EditorGUILayout.EnumPopup("Reset average duration", AntDebugScenario.avgResetDuration);
					if (GUILayout.Button("Reset now!", GUILayout.Width(88.0f), GUILayout.Height(14.0f)))
					{
						_self.Scenario.ResetDurations();
					}
				}
				EditorGUILayout.EndHorizontal();

				_threshold = EditorGUILayout.Slider("Threshold MS", _threshold, 0.0f, 33.0f);
				_systemSortMode = (SortMode) EditorGUILayout.EnumPopup("Sort by", _systemSortMode);
				EditorGUILayout.Space();

				EditorGUILayout.BeginHorizontal();
				{
					_systemNameSearchTerm = EditorGUILayout.TextField("Search", _systemNameSearchTerm);
					const string clearButtonControlName = "Clear Button";
					GUI.SetNextControlName(clearButtonControlName);
					if (GUILayout.Button("x", GUILayout.Width(18.0f), GUILayout.Height(14.0f)))
					{
						_systemNameSearchTerm = string.Empty;
						GUI.FocusControl(clearButtonControlName);
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();

			string label = string.Format("Initialize Systems ({0})", _self.Scenario.InitializeSystemsCount.ToString());
			_showInitializeSystems = EditorGUILayout.Foldout(_showInitializeSystems, label);
			if (_showInitializeSystems)
			{
				EditorGUILayout.BeginVertical(GUI.skin.box);
				{
					if (DrawSystemInfos(_self.Scenario, true, false) == 0)
					{
						EditorGUILayout.LabelField("<No Systems>");
					}
				}
				EditorGUILayout.EndVertical();
			}

			label = string.Format("Execute Systems ({0})", _self.Scenario.ExecuteSystemsCount.ToString());
			_showExecuteSystems = EditorGUILayout.Foldout(_showExecuteSystems, label);
			if (_showExecuteSystems)
			{
				EditorGUILayout.BeginVertical(GUI.skin.box);
				{
					if (DrawSystemInfos(_self.Scenario, false, false) == 0)
					{
						EditorGUILayout.LabelField("<No Systems>");
					}
				}
				EditorGUILayout.EndVertical();
			}
		}

		private int DrawSystemInfos(AntDebugScenario aScenario, bool aInitOnly, bool aIsChildSystem)
		{
			var systemInfosList = (aInitOnly)
				? aScenario.InitializeSystemsInfos
				: aScenario.ExecuteSystemInfos;
			
			var systemInfos = systemInfosList
				.Where(sys => sys.AverageExecutionDuration >= _threshold)
				.ToArray();

			systemInfos = SortSystemInfos(systemInfos, _systemSortMode);

			int count = 0;
			AntDebugScenario debugScenario;
			AntSystemInfo systemInfo;
			for (int i = 0, n = systemInfos.Length; i < n; i++)
			{
				systemInfo = systemInfos[i];
				debugScenario = systemInfo.System as AntDebugScenario;
				/*if (debugScenario != null)
				{
					if (aInitOnly && debugScenario.InitializeSystemsCount == 0)
					{
						continue;
					}
				}*/

				if (systemInfo.Name.ToLower().Contains(_systemNameSearchTerm.ToLower()))
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUI.BeginDisabledGroup(aIsChildSystem);
						{
							systemInfo.isActive = EditorGUILayout.Toggle(systemInfo.isActive, GUILayout.Width(20.0f));
						}
						EditorGUI.EndDisabledGroup();

						var avg = string.Format("- {0:00.000}", systemInfo.AverageExecutionDuration).PadRight(12);
						var min = string.Format("▼ {0:00.000}", systemInfo.MinExecutionDuration).PadRight(12);
						var max = string.Format("▲ {0:00.000}", systemInfo.MaxExecutionDuration);

						EditorGUILayout.LabelField(systemInfo.Name, avg + min + max);
					}
					EditorGUILayout.EndHorizontal();
				}

				if (debugScenario != null)
				{
					EditorGUI.indentLevel++;
					count += DrawSystemInfos(debugScenario, aInitOnly, true);
					EditorGUI.indentLevel--;
				}

				count++;
			}

			return count;
		}

		private AntSystemInfo[] SortSystemInfos(AntSystemInfo[] aSystems, SortMode aSortMode)
		{
			AntSystemInfo[] result = aSystems;
			switch (aSortMode)
			{
				case SortMode.Name :
					result = aSystems
						.OrderBy(sys => sys.Name)
						.ToArray();
				break;

				case SortMode.NameDescending :
					result = aSystems
						.OrderByDescending(sys => sys.Name)
						.ToArray();
				break;

				case SortMode.ExecutionTime :
					result = aSystems
						.OrderBy(sys => sys.AverageExecutionDuration)
						.ToArray();
				break;

				case SortMode.ExecutionTimeDescending :
					result = aSystems
						.OrderByDescending(sys => sys.AverageExecutionDuration)
						.ToArray();
				break;
			}

			return result;
		}
	}
}