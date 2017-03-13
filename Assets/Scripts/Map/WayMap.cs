using UnityEngine;
using System.Collections.Generic;
using Anthill.Utils;
using Game.Core;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Map
{
	public class WayMap : MonoBehaviour
	{
		public static WayMap Current { get; private set; }

		[Tooltip("Определяет расстояние при котором точки карты связываются между собой.")]
		public float linkRadius = 0.5f;
		[Tooltip("Определяет расстояние между танком и точкой при котором засчитывается прибытие на точку.")]
		public float approachRadius = 0.4f;

		[HideInInspector] public List<Vector2> wayPoints = new List<Vector2>();
		[HideInInspector] public List<WayPoint> points;

		private List<Vector2> _way;

		// -----------------------------------------------------
		// Unity Callbacks
		// -----------------------------------------------------

		private void Awake()
		{
			Current = this;
		}

		private void Start()
		{
			points = new List<WayPoint>();
			CreateMap();
			LinkMap();
		}

		private void Update()
		{
			if (Config.Instance.showWayMap)
			{
				for (int i = 0, n = points.Count; i < n; i++)
				{
					points[i].DrawLinks();
				}
			}
		}

		// -----------------------------------------------------
		// Public Methods
		// -----------------------------------------------------

		public List<Vector2> FindWay(WayPoint aCurrent, WayPoint aGoal)
		{
			List<WayPoint> opened = new List<WayPoint>();
			List<WayPoint> closed = new List<WayPoint>();

			aCurrent.parent = null;
			aCurrent.cost = 0.0f;
			aCurrent.heuristic = aCurrent.Heuristic(aGoal);
			aCurrent.sum = aCurrent.heuristic;
			opened.Add(aCurrent);

			WayPoint current;
			while (opened.Count > 0)
			{
				current = opened[0];
				for (int i = 1, n = opened.Count; i < n; i++)
				{
					if (opened[i].sum < current.sum)
					{
						current = opened[i];
					}
				}

				opened.Remove(current);

				if (AntMath.Equal(current.position, aGoal.position))
				{
					return GetPath(current);
				}

				closed.Add(current);

				WayPoint neighbor;
				int openedIndex = -1;
				int closedIndex = -1;
				float cost = 0.0f;
				for (int i = 0, n = current.neighbors.Count; i < n; i++)
				{
					cost = current.cost + current.neighbors[i].cost;
					
					neighbor = current.neighbors[i];

					openedIndex = opened.IndexOf(neighbor);
					closedIndex = closed.IndexOf(neighbor);

					if (openedIndex > -1 && cost < opened[openedIndex].cost)
					{
						opened.RemoveAt(openedIndex);
						openedIndex = -1;
					}

					if (closedIndex > -1 && cost < closed[closedIndex].cost)
					{
						closed.RemoveAt(closedIndex);
						closedIndex = -1;
					}

					if (openedIndex == -1 && closedIndex == -1)
					{
						neighbor.cost = cost;
						neighbor.heuristic = neighbor.Heuristic(aGoal);
						neighbor.sum = cost + neighbor.heuristic;
						neighbor.parent = current;
						opened.Add(neighbor);
					}
				}
			}
			return null;
		}

		public WayPoint FindNearestPoint(Vector2 aPosition)
		{
			AntSorter<WayPoint> sorter = new AntSorter<WayPoint>();
			WayPoint point;
			float dist;
			for (int i = 0, n = points.Count; i < n; i++)
			{
				point = points[i];
				dist = AntMath.Distance(aPosition, point.position);
				if (dist <= linkRadius)
				{
					sorter.Add(point, dist);
				}
			}
			
			if (sorter.Count > 0)
			{
				sorter.Sort(AntSorterOrder.ASC);
				return sorter[0];
			}
			
			return null;
		}

		public WayPoint GetRandomPoint()
		{
			int index = AntMath.RandomRangeInt(0, points.Count - 1);
			return (index >= 0 && index < points.Count) ? points[index] : null;
		}

		// -----------------------------------------------------
		// Private Methods
		// -----------------------------------------------------

		private void CreateMap()
		{
			points = new List<WayPoint>();
			for (int i = 0, n = wayPoints.Count; i < n; i++)
			{
				points.Add(new WayPoint()
				{
					position = wayPoints[i]
				});
			}
		}

		private void LinkMap()
		{
			WayPoint current;
			WayPoint other;
			for (int i = 0, n = points.Count; i < n; i++)
			{
				current = points[i];
				for (int j = 0; j < n; j++)
				{
					other = points[j];
					if (AntMath.Distance(current.position, other.position) <= linkRadius)
					{
						current.AddNeighbor(other);
					}
				}
			}
		}

		private List<Vector2> GetPath(WayPoint aFromPoint)
		{
			WayPoint current = aFromPoint;
			List<Vector2> result = new List<Vector2>();
			result.Add(current.position);
			while (current.parent != null)
			{
				current = current.parent;
				result.Add(current.position);
			}
			result.Reverse();
			return result;
		}
	}

	// -----------------------------------------------------
	// WayMap Editor
	// -----------------------------------------------------

	#if UNITY_EDITOR
	[CustomEditor(typeof(WayMap))]
	public class WayMapEditor : Editor
	{
		private WayMap _self;

		private void OnEnable()
		{
			_self = (WayMap)target;
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
					if (Handles.Button(_self.wayPoints[i], Quaternion.identity, 0.05f, 0.05f, Handles.DotCap))
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
				AntDrawer.DrawCircle((Vector2)p, _self.linkRadius, Color.yellow);
				AntDrawer.EndHandles();

				if (Handles.Button(p, Quaternion.identity, 0.05f, 0.05f, Handles.DotCap))
				{
					_self.wayPoints.Add((Vector2)p);
					dirty = true;
				}

				Handles.color = Color.white;
				float s = HandleUtility.GetHandleSize(p) * 0.05f;
				for (int i = 0, n = _self.wayPoints.Count; i < n; i++)
				{
					Handles.DotCap(0, _self.wayPoints[i], Quaternion.identity, s);
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
			return Handles.FreeMoveHandle(aPosition, Quaternion.identity, s, Vector3.zero, Handles.DotCap);
		}
	}
	#endif
}