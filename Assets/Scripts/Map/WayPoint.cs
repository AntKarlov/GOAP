using UnityEngine;
using System.Collections.Generic;
using Anthill.Utils;

namespace Game.Map
{
	public class WayPoint
	{
		public Vector2 position;
		public WayPoint parent;
		public float heuristic;
		public float cost;
		public float sum;

		public List<WayPoint> neighbors;

		public WayPoint()
		{
			neighbors = new List<WayPoint>();
		}

		public void AddNeighbor(WayPoint aPoint)
		{
			if (neighbors.IndexOf(aPoint) == -1)
			{
				neighbors.Add(aPoint);
			}
		}

		public void DrawLinks()
		{
			for (int i = 0, n = neighbors.Count; i < n; i++)
			{
				AntDrawer.DrawLine(position, neighbors[i].position, Color.white);
			}
		}

		public float Heuristic(WayPoint aGoal)
		{
			return AntMath.Distance(position, aGoal.position);
		}
	}
}