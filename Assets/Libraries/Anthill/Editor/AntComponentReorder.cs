using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
[ExecuteInEditMode]
public class AntComponentReorder : EditorWindow
{
	private const float BTN_HEIGHT = 20.0f;
	private const int BTN_PADDING = 4;

	private int _startIndex;
	private int _dragIndex;
	private int _lastDragIndex;
	private bool _isMouseDown;
	private int[] _indexList;

	private Texture2D _btnActiveBG;
	private GUIStyle _btnStyle;
	private GUIStyle _btnDisabledStyle;
	private GUIStyle _btnActiveStyle;
	private GUIStyle _currentStyle;

	private Component[] _components;
	private bool _isInitialized = false;

	// -----------------------------------------------------
	// Initialize Editor Window
	// -----------------------------------------------------

	[MenuItem("Anthill/Component Reorder")]
	private static void ShowWindow()
	{
		AntComponentReorder window = (AntComponentReorder)EditorWindow.GetWindow(typeof(AntComponentReorder), false, "Component Reorder");
		window.autoRepaintOnSceneChange = true;
	}

	// -----------------------------------------------------
	// Unity Methods
	// -----------------------------------------------------

	private void OnInspectorUpdate()
	{
		Repaint();
	}

	private void OnGUI()
	{
		if (!_isInitialized)
		{
			Initialize();
		}

		Transform t = GetActiveTransform();
		if (t != null)
		{
			_components = t.GetComponents<Component>();
			if (!_isMouseDown && Event.current.type == EventType.MouseDown)
			{
				_startIndex = Mathf.FloorToInt(Event.current.mousePosition.y / (BTN_HEIGHT + (float)BTN_PADDING));
				if (_startIndex >= 0 && _startIndex < _components.Length &&
					!string.Equals(_components[_startIndex].GetType().ToString(), "UnityEngine.Transform"))
				{
					_isMouseDown = true;
					_dragIndex = _startIndex;
					_lastDragIndex = _startIndex;
					_indexList = new int[_components.Length];
					for (int i = 0, n = _components.Length; i < n; i++)
					{
						_indexList[i] = i;
					}
				}
				else
				{
					_isMouseDown = false;
				}
			}

			if (_isMouseDown && Event.current.type == EventType.MouseUp)
			{
				_isMouseDown = false;
				int toIndex = Mathf.RoundToInt(Mathf.Abs(_dragIndex - _startIndex));
				if (toIndex > 0)
				{
					int dir = (_dragIndex - _startIndex) / Mathf.Abs(_dragIndex - _startIndex);
					for (int i = 0; i < toIndex; i++)
					{
						if (dir > 0)
						{
							UnityEditorInternal.ComponentUtility.MoveComponentDown(_components[_startIndex + i]);
						}
						else if (dir < 0)
						{
							UnityEditorInternal.ComponentUtility.MoveComponentUp(_components[_startIndex - i]);
						}
						_components = t.GetComponents<Component>();
					}
				}
			}

			// Отрисовка всех компонентов.
			DrawList();
			if (_isMouseDown)
			{
				// Отрисовка перетаскиваемой кнопки.
				Rect btnPos = new Rect(Event.current.mousePosition.x - Screen.width * 0.5f,
					Event.current.mousePosition.y - BTN_HEIGHT * 0.5f,
					Screen.width - 10.0f, BTN_HEIGHT);
				GUI.Button(btnPos, GetComponentName(_components[_startIndex].GetType().ToString()), _btnStyle);

				// Получение индекса для новой позиции кнопки.
				int index = Mathf.FloorToInt(Event.current.mousePosition.y / (BTN_HEIGHT + (float)BTN_PADDING));
				index = (index >= _components.Length) ? _components.Length - 1 : (index < 0) ? 0 : index;

				if (!string.Equals(_components[index].GetType().ToString(), "UnityEngine.Transform"))
				{
					_dragIndex = index;
				}

				if (_dragIndex != _lastDragIndex)
				{
					int last = _indexList[_lastDragIndex];
					_indexList[_lastDragIndex] = _indexList[_dragIndex];
					_indexList[_dragIndex] = last;
					_lastDragIndex = _dragIndex;
				}
			}
		}
	}

	// -----------------------------------------------------
	// Private Methods
	// -----------------------------------------------------

	private void Initialize()
	{
		_btnActiveBG = new Texture2D(1, 1);
		_btnActiveBG.SetPixel(0, 0, Color.grey);
		_btnActiveBG.Apply();

		_btnStyle = new GUIStyle(GUI.skin.button);
		_btnStyle.alignment = TextAnchor.MiddleLeft;
		_btnStyle.fixedHeight = BTN_HEIGHT;
		_btnStyle.margin = new RectOffset(BTN_PADDING, BTN_PADDING, BTN_PADDING, BTN_PADDING);

		_btnDisabledStyle = new GUIStyle(GUI.skin.button);
		_btnDisabledStyle.alignment = TextAnchor.MiddleLeft;
		_btnDisabledStyle.fixedHeight = BTN_HEIGHT;
		_btnDisabledStyle.margin = new RectOffset(BTN_PADDING, BTN_PADDING, BTN_PADDING, BTN_PADDING);
		_btnDisabledStyle.normal.textColor = Color.gray;

		_btnActiveStyle = new GUIStyle();
		_btnActiveStyle.alignment = TextAnchor.MiddleLeft;
		_btnActiveStyle.fixedHeight = BTN_HEIGHT;
		_btnActiveStyle.margin = new RectOffset(BTN_PADDING, BTN_PADDING, BTN_PADDING, BTN_PADDING);
		_btnActiveStyle.normal.background = _btnActiveBG;
		_btnActiveStyle.normal.textColor = Color.gray;
	}

	private void DrawList()
	{
		int index;
		string componentName;
		for (int i = 0, n = _components.Length; i < n; i++)
		{
			index = (_isMouseDown) ? _indexList[i] : i;
			componentName = GetComponentName(_components[index].GetType().ToString());
			_currentStyle = _btnStyle;

			if (_isMouseDown && i == _dragIndex)
			{
				_currentStyle = _btnActiveStyle;
			}
			else if (string.Equals(componentName, "Transform"))
			{
				_currentStyle = _btnDisabledStyle;
			}

			GUILayout.Button(componentName, _currentStyle);
		}
	}

	private string GetComponentName(string aFullName)
	{
		string[] arr = aFullName.Split('.');
		return arr[arr.Length - 1];
	}

	private Transform GetActiveTransform()
	{
		if (Selection.GetFiltered(typeof(Transform), SelectionMode.Unfiltered) != null &&
			Selection.GetFiltered(typeof(Transform), SelectionMode.Unfiltered).Length > 0)
		{
			return (Transform) Selection.GetFiltered(typeof(Transform), SelectionMode.Unfiltered)[0];
		}
		return Selection.activeTransform;
	}
}