using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using Anthill.Utils;

namespace Anthill.AI
{
	// + Выводить доступные действия (сценарий) (выделять текущую) 
	// - Выводить доступные цели (выделять текущую)
	// + Выводить доступные задачи (выделять текущую)
	// - Выводить текущий план
	// - Выводить текущее состояние мира
	// - Выводить содержимое памяти
	public class AntAIDebugger : EditorWindow
	{
		private struct TitleData
		{
			public string text;
			public Rect rect;
		}

		private GUIStyle _titleStyle;
		private GUIStyle _nodeStyle;
		private GUIStyle _taskNodeStyle;
		private GUIStyle _activeTaskNodeStyle;
		private GUIStyle _warningNodeStyle;

		private IAIProvider _provider;
		private List<AntAIDebuggerNode> _nodes;
		private List<TitleData> _titles;

		private Vector2 _offset;
		private Vector2 _drag;
		private Vector2 _totalDrag;

		#region Initialize Window

		[MenuItem("Anthill/AI Debugger")]
		private static void ShowWindow()
		{
			AntAIDebugger window = (AntAIDebugger) EditorWindow.GetWindow(typeof(AntAIDebugger), false, "AI Debugger");
			window.autoRepaintOnSceneChange = true;
		}

		#endregion
		#region Unity Callbacks

		private void OnEnable()
		{
			var textOffset = new RectOffset(12, 0, 10, 0);
			var border = new RectOffset(12, 12, 12, 12);
			bool richText = true;

			_titleStyle = new GUIStyle();
			_titleStyle.fontSize = 22;
			_titleStyle.normal.textColor = Color.gray;

			_nodeStyle = new GUIStyle();
			_nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
			_nodeStyle.border = border;
			_nodeStyle.richText = richText;
			_nodeStyle.padding = textOffset;

			_taskNodeStyle = new GUIStyle();
			_taskNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D;
			_taskNodeStyle.border = border;
			_taskNodeStyle.richText = richText;
			_taskNodeStyle.padding = textOffset;

			_activeTaskNodeStyle = new GUIStyle();
			_activeTaskNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3 on.png") as Texture2D;
			_activeTaskNodeStyle.border = border;
			_activeTaskNodeStyle.richText = richText;
			_activeTaskNodeStyle.padding = textOffset;

			_warningNodeStyle = new GUIStyle();
			_warningNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node6.png") as Texture2D;
			_warningNodeStyle.border = border;
			_warningNodeStyle.richText = richText;
			_warningNodeStyle.padding = textOffset;

			_nodes = new List<AntAIDebuggerNode>();
			_titles = new List<TitleData>();
		}

		private void OnGUI()
		{
			if (Selection.activeGameObject != null)
			{
				var p = Selection.activeGameObject.GetComponent<IAIProvider>();
				if (!System.Object.ReferenceEquals(p, _provider))
				{
					_provider = p;
					InitializeActionNodes(_totalDrag);
				}
			}

			if (_provider != null)
			{
				DrawGrid(20, Color.gray, 0.05f);
				DrawGrid(100, Color.gray, 0.05f);

				DrawNodes();

				ProcessEvents(Event.current);
				Repaint();
			}
		}

		#endregion
		#region Private Methods

		private void InitializeActionNodes(Vector2 aNodePosition)
		{
			_titles.Clear();
			_nodes.Clear();

			CreateTitle(0.0f, -35.0f, "Scenario: Actions and Goals");

			// Список всех действий, необходим чтобы определить
			// какие действия были связаны с задачами, а какие нет.
			AntAIAction action;
			var unusedActions = new List<KeyValuePair<AntAIAction, bool>>();
			for (int i = 0, n = _provider.Logic.Planner.actions.Count; i < n; i++)
			{
				action = _provider.Logic.Planner.actions[i];
				unusedActions.Add(new KeyValuePair<AntAIAction, bool>(action, false));
			}

			float totalWidth = 0.0f;
			float totalHeight = 0.0f;
			int foundCount = 0;
			bool isDefaultTask;

			AntAITask task;
			AntAIDebuggerNode taskNode;
			AntAIDebuggerNode actionNode;
			Vector2 taskPos;

			var unusedTasks = new List<AntAITask>(); // Список задач которые не связаны с действиями.
			var toLinkNodes = new List<AntAIDebuggerNode>();

			for (int i = 0, n = _provider.Tasks.Length; i < n; i++)
			{
				task = _provider.Tasks[i];
				toLinkNodes.Clear();
				foundCount = 0;
				totalWidth = 0.0f;
				totalHeight = 0.0f;
				for (int j = 0, nj = _provider.Logic.Planner.actions.Count; j < nj; j++)
				{
					action = _provider.Logic.Planner.actions[j];
					if (action.task.Equals(task.name))
					{
						int id = unusedActions.FindIndex(x => System.Object.ReferenceEquals(x.Key, action));
						unusedActions[id] = new KeyValuePair<AntAIAction, bool>(action, true);

						actionNode = CreateActionNode(action, ref aNodePosition);
						actionNode.SetOutput(actionNode.rect.width * 0.5f, actionNode.rect.height - 10.0f);
						totalWidth += actionNode.rect.width;
						totalHeight = (actionNode.rect.height > totalHeight) ? actionNode.rect.height : totalHeight;
						toLinkNodes.Add(actionNode);
						foundCount++;
					}
				}
				
				if (foundCount > 0)
				{
					if (foundCount > 1)
					{
						taskPos = new Vector2(aNodePosition.x - totalWidth * 0.75f, aNodePosition.y + totalHeight);
					}
					else
					{
						taskPos = new Vector2(aNodePosition.x - totalWidth, aNodePosition.y + totalHeight);
					}

					isDefaultTask = System.Object.ReferenceEquals(_provider.DefaultTask, task);
					taskNode = CreateTaskNode(task, taskPos, isDefaultTask);
					taskNode.SetInput(taskNode.rect.width * 0.5f, 10.0f);
					for (int j = 0, nj = toLinkNodes.Count; j < nj; j++)
					{
						toLinkNodes[j].LinkTo(taskNode, new Color(0.3f, 0.7f, 0.4f));
					}
				}
				else
				{
					unusedTasks.Add(task);
				}
			}

			// Проверяем какие действия не удалось связать с задачами и создаем нотификейшены.
			for (int i = 0, n = unusedActions.Count; i < n; i++)
			{
				if (!unusedActions[i].Value)
				{
					actionNode = CreateActionNode(unusedActions[i].Key, ref aNodePosition);
					actionNode.SetOutput(actionNode.rect.width * 0.5f, actionNode.rect.height - 10.0f);
					totalWidth = actionNode.rect.width;
					totalHeight = actionNode.rect.height;

					taskPos = new Vector2(aNodePosition.x - totalWidth, aNodePosition.y + totalHeight);
					taskNode = CreateMissingTaskNode(unusedActions[i].Key.task, taskPos);
					taskNode.SetInput(taskNode.rect.width * 0.5f, 10.0f);
					actionNode.LinkTo(taskNode, new Color(0.7f, 0.2f, 0.3f));
				}
			}

			// Создаем оставшиеся задачи.
			taskPos = aNodePosition;
			for (int i = 0, n = unusedTasks.Count; i < n; i++)
			{
				isDefaultTask = System.Object.ReferenceEquals(_provider.DefaultTask, unusedTasks[i]);
				taskNode = CreateTaskNode(unusedTasks[i], taskPos, isDefaultTask);
				taskNode.SetInput(taskNode.rect.width * 0.5f, 10.0f);
				taskPos.y += taskNode.rect.height;
			}
		}

		private void CreateTitle(float aX, float aY, string aTitle)
		{
			_titles.Add(new TitleData() {
				text = "Scenario: Actions and Goals",
				rect = new Rect(0.0f, -35.0f, 500.0f, 50.0f)
			});
		}

		private AntAIDebuggerNode CreateMissingTaskNode(string aTitle, Vector2 aNodePosition)
		{
			return AddNode(string.Format("<color={1}><b>TASK: '{0}'</b>\n\r   Non-existent task!</color>", aTitle, "#FFFFFF"), 
				220.0f, 54.0f, _warningNodeStyle, _warningNodeStyle, ref aNodePosition);
		}

		private AntAIDebuggerNode CreateTaskNode(AntAITask aTask, Vector2 aNodePosition, bool isDefault)
		{
			string title;
			float height = 40.0f;
			if (isDefault)
			{
				title = string.Format("<b>TASK: '{0}'</b>\n\r   This is Default Task", aTask.name);
				height += 14.0f;
			}
			else
			{
				title = string.Format("<b>TASK: '{0}'</b>", aTask.name);
			}
			return AddNode(title, 220.0f, height, _taskNodeStyle, _taskNodeStyle, ref aNodePosition);
		}

		private AntAIDebuggerNode CreateActionNode(AntAIAction aAction, ref Vector2 aNodePosition)
		{
			bool value = false;
			StringBuilder text = new StringBuilder();
			text.Append(string.Format("<b>ACTION: '{0}'</b> [{2}]\n\r", 
				aAction.name, aAction.task, aAction.cost));
			int lines = 1;
			
			text.Append("<b>Pre Conditions</b>\n\r");
			lines++;
			for (int j = 0; j < AntAIPlanner.MAX_ATOMS; j++)
			{
				if (aAction.pre.GetMask(j))
				{
					value = aAction.pre.GetValue(j);
					text.Append(string.Format("   <color={2}>'{0}' = {1}</color>\n\r", 
						_provider.Logic.Planner.atoms[j], value, (value) ? "#004d00" : "#800000"));
					lines++;
				}
			}
			text.Append("<b>Post Conditions</b>\n\r");
			lines++;
			for (int j = 0; j < AntAIPlanner.MAX_ATOMS; j++)
			{
				if (aAction.post.GetMask(j))
				{
					value = aAction.post.GetValue(j);
					text.Append(string.Format("   <color={2}>'{0}' = {1}</color>\n\r", 
						_provider.Logic.Planner.atoms[j], value, (value) ? "#004d00" : "#800000"));
					lines++;
				}
			}

			GUIStyle style = _nodeStyle;
			// todo менять стиль в зависимости от текущей задачи
			return AddNode(text.ToString(), 220.0f, 14.0f * (float) (lines + 1) + 7.0f, style, style, ref aNodePosition);
		}

		private void DrawNodes()
		{
			if (_nodes != null)
			{
				if (Event.current.type == EventType.Repaint)
				{
					AntAIDebuggerNode node;
					for (int i = 0, n = _nodes.Count; i < n; i++)
					{
						node = _nodes[i];
						if (node.links.Count > 0)
						{
							for (int j = 0, nj = node.links.Count; j < nj; j++)
							{
								AntDrawer.DrawVerticalSolidConnection(node.Output, node.links[j].Key.Input, node.links[j].Value, 1, 15.0f);
							}
						}
					}

					for (int i = 0, n = _nodes.Count; i < n; i++)
					{
						_nodes[i].Draw();
					}
				}
			}

			if (_titles != null)
			{
				if (Event.current.type == EventType.Repaint)
				{
					TitleData t;
					for (int i = 0, n = _titles.Count; i < n; i++)
					{
						t = _titles[i];
						GUI.Label(new Rect(t.rect.x + _totalDrag.x, t.rect.y + _totalDrag.y, 
							t.rect.width, t.rect.height), t.text, _titleStyle);
					}
				}
			}
		}

		private AntAIDebuggerNode AddNode(string aText, float aWidth, float aHeight, 
			GUIStyle aStyle, GUIStyle aActiveStyle, ref Vector2 aPosition)
		{
			AntAIDebuggerNode node = new AntAIDebuggerNode(aPosition.x, aPosition.y, aWidth, aHeight, aStyle, aActiveStyle);
			node.title = aText;
			_nodes.Add(node);
			aPosition.x += aWidth;
			return node;
		}

		private void DrawGrid(float aCellSize, Color aColor, float aOpacity)
		{
			int cols = Mathf.CeilToInt(position.width / aCellSize);
			int rows = Mathf.CeilToInt(position.height / aCellSize);

			Handles.BeginGUI();
			Color c = Handles.color;
			Handles.color = new Color(aColor.r, aColor.g, aColor.b, aOpacity);

			_offset += _drag * 0.5f;
			Vector3 newOffset = new Vector3(_offset.x % aCellSize, _offset.y % aCellSize, 0.0f);

			for (int i = 0; i < cols; i++)
			{
				Handles.DrawLine(new Vector3(aCellSize * i, -aCellSize, 0.0f) + newOffset,
					new Vector3(aCellSize * i, position.height, 0.0f) + newOffset);
			}

			for (int i = 0; i < rows; i++)
			{
				Handles.DrawLine(new Vector3(-aCellSize, aCellSize * i, 0.0f) + newOffset,
					new Vector3(position.width, aCellSize * i, 0.0f) + newOffset);
			}

			Handles.color = c;
			Handles.EndGUI();
		}

		private void ProcessEvents(Event aEvent)
		{
			_drag = Vector2.zero;
			
			if (_nodes != null)
			{
				for (int i = 0; i < _nodes.Count; i++)
				{
					if (_nodes[i].ProcessEvents(aEvent))
					{
						return;
					}
				}
			}
			
			switch (aEvent.type)
			{
				case EventType.mouseDrag :
					if (aEvent.button == 0)
					{
						OnDrag(aEvent.delta);		
					}
				break;
			}
		}

		private void OnDrag(Vector2 aDelta)
		{
			_totalDrag += aDelta;
			_drag = aDelta;

			if (_nodes != null)
			{
				for (int i = 0, n = _nodes.Count; i < n; i++)
				{
					_nodes[i].Drag(aDelta);
				}
			}

			GUI.changed = true;
		}

		#endregion
	}
}