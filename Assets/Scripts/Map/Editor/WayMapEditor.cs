using UnityEditor;
using UnityEngine;

using Anthill.Utils;

namespace Game.Map
{
	[CustomEditor(typeof(WayMap))]
	public class WayMapEditor : Editor
	{
		private WayMap _self;

		private void OnEnable()
		{
			_self = (WayMap) target;
		}

		private void OnSceneGUI()
		{
			bool dirty = false;
			bool removePressed = ((Event.current.modifiers & (EventModifiers.Command | EventModifiers.Control)) != 0);
			bool addPressed = ((Event.current.modifiers & (EventModifiers.Shift)) != 0);

			Handles.matrix = _self.transform.localToWorldMatrix;

			if (removePressed)
			{
				Handles.color = Color.red;
				for (int i = 0, n = _self.wayPoints.Count; i < n; i++)
				{
					if (Handles.Button(_self.wayPoints[i], Quaternion.identity, 0.05f, 0.05f, Handles.DotHandleCap))
					{
						_self.wayPoints.RemoveAt(i);
						dirty = true;
						break;
					}
				}
			}
			else if (addPressed)
			{
				var mouse = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
				var p = _self.transform.InverseTransformPoint(mouse);

				AntDrawer.BeginHandles(Handles.matrix);
				AntDrawer.DrawCircle((Vector2) p, _self.linkRadius, Color.yellow);
				AntDrawer.EndHandles();

				if (Handles.Button(p, Quaternion.identity, 0.05f, 0.05f, Handles.DotHandleCap))
				{
					_self.wayPoints.Add((Vector2) p);
					dirty = true;
				}

				Handles.color = Color.white;
				float s = HandleUtility.GetHandleSize(p) * 0.05f;
				for (int i = 0, n = _self.wayPoints.Count; i < n; i++)
				{
					Handles.DotHandleCap(0, _self.wayPoints[i], Quaternion.identity, s, EventType.dragUpdated);
				}
				
				HandleUtility.Repaint();
			}
			else
			{
				Handles.color = Color.white;
				for (int i = 0, n = _self.wayPoints.Count; i < n; i++)
				{
					Vector2 p = _self.wayPoints[i];
					Vector2 delta = DotHandle(p) - p;
					if (delta != Vector2.zero)
					{
						_self.wayPoints[i] = p + delta;
						
						dirty = true;
					}
				}
			}

			DrawLinks();

			if (dirty)
			{
				EditorUtility.SetDirty(target);
			}
		}

		private void DrawLinks()
		{
			Vector2 current;
			AntDrawer.BeginHandles(Handles.matrix);
			for (int i = 0, n = _self.wayPoints.Count; i < n; i++)
			{
				current = _self.wayPoints[i];
				for (int j = 0; j < n; j++)
				{
					if (i != j && AntMath.Distance(current, _self.wayPoints[j]) <= _self.linkRadius)
					{
						AntDrawer.DrawLine(current, _self.wayPoints[j], Color.yellow);
					}
				}
			}
			AntDrawer.EndHandles();
		}

		private Vector2 DotHandle(Vector2 aPosition, float aSize = 0.05f)
		{
			float s = HandleUtility.GetHandleSize(aPosition) * aSize;
			return Handles.FreeMoveHandle(aPosition, Quaternion.identity, s, Vector3.zero, Handles.DotHandleCap);
		}
	}
}