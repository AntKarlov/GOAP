using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Anthill.Core
{
	public class AntSystemMonitor
	{
		public float xBorder = 48;
		public float yBorder = 20;
		public int rightLinePadding = -15;
		public string labelFormat = "{0:0.0}";
		public string axisFormat = "{0:0.0}";
		public int gridLines = 1;
		public float axisRounding = 10.0f;
		public float anchorRadius = 1.0f;
		public Color lineColor = Color.magenta;

		private readonly GUIStyle _labelTextStyle;
		private readonly GUIStyle _centeredStyle;
		private readonly Vector3[] _cachedLinePointVerticies;
		private readonly Vector3[] _linePoints;

		public AntSystemMonitor(int aDataLength)
		{
			_labelTextStyle = new GUIStyle(GUI.skin.label);
			_labelTextStyle.alignment = TextAnchor.UpperRight;
			_centeredStyle = new GUIStyle();
			_centeredStyle.alignment = TextAnchor.UpperCenter;
			_centeredStyle.normal.textColor = Color.white;
			_linePoints = new Vector3[aDataLength];
			_cachedLinePointVerticies = new Vector3[]
			{
				new Vector3(-1, 1, 0) * anchorRadius,
				new Vector3(1, 1, 0) * anchorRadius,
				new Vector3(1, -1, 0) * anchorRadius,
				new Vector3(-1, -1, 0) * anchorRadius,
			};
		}

		public void Draw(float[] aData, float aHeight)
		{
			Rect rect = GUILayoutUtility.GetRect(EditorGUILayout.GetControlRect().width, aHeight);
			float top = rect.y + yBorder;
			float floor = rect.y + rect.height - yBorder;
			float availableHeight = floor - top;
			float max = (aData.Length != 0) ? aData.Max() : 0.0f;
			if (max % axisRounding != 0)
			{
				max = max + axisRounding - (max % axisRounding);
			}

			DrawGridLines(top, rect.width, availableHeight, max);
			DrawAvg(aData, top, floor, rect.width, availableHeight, max);
			DrawLine(aData, floor, rect.width, availableHeight, max);
		}

		private void DrawGridLines(float aTop, float aWidth, float aAvailableHeight, float aMax)
		{
			Color c = Handles.color;
			Handles.color = Color.grey;
			int n = gridLines + 1;
			float lineSpacing = aAvailableHeight / n;
			for (int i = 0; i <= n; i++)
			{
				float lineY = aTop + (lineSpacing * i);
				Handles.DrawLine(new Vector2(xBorder, lineY),
					new Vector2(aWidth - rightLinePadding, lineY));
				GUI.Label(new Rect(0.0f, lineY - 8.0f, xBorder - 2.0f, 50.0f),
					string.Format(axisFormat, aMax * (1.0f - ((float) i / (float) n))),
					_labelTextStyle);
			}
			Handles.color = c;
		}

		private void DrawAvg(float[] aData, float aTop, float aFloor, float aWidth, float aAvailableHeight, float aMax)
		{
			Color c = Handles.color;
			Handles.color = Color.yellow;

			float avg = aData.Average();
			float lineY = aFloor - (aAvailableHeight * (avg / aMax));
			Handles.DrawLine(new Vector2(xBorder, lineY),
				new Vector2(aWidth - rightLinePadding, lineY));
			Handles.color = c;
		}

		private void DrawLine(float[] aData, float aFloor, float aWidth, float aAvailableHeight, float aMax)
		{
			float lineWidth = (float) (aWidth - xBorder - rightLinePadding) / aData.Length;
			Color c = Handles.color;
			Rect labelRect = new Rect();
			Vector2 newLine;
			bool mousePositionDiscovered = false;
			float mouseHoverDataValue = 0.0f;
			float linePointScale;
			Handles.color = lineColor;
			Handles.matrix = Matrix4x4.identity;
			HandleUtility.handleMaterial.SetPass(0);
			for (int i = 0, n = aData.Length; i < n; i++)
			{
				float value = aData[i];
				float lineTop = aFloor - (aAvailableHeight * (value / aMax));
				newLine = new Vector2(xBorder + (lineWidth * i), lineTop);
				_linePoints[i] = new Vector3(newLine.x, newLine.y, 0);
				linePointScale = 1.0f;
				if (!mousePositionDiscovered)
				{
					float anchorPosRadius3 = anchorRadius * 3.0f;
					float anchorPosRadius6 = anchorRadius * 6.0f;
					Vector2 anchorPos = newLine - (Vector2.up * 0.5f);
					labelRect = new Rect(anchorPos.x - anchorPosRadius3, 
						anchorPos.y - anchorPosRadius3,
						anchorPosRadius6, anchorPosRadius6);
					if (labelRect.Contains(Event.current.mousePosition))
					{
						mousePositionDiscovered = true;
						mouseHoverDataValue = value;
						linePointScale = 3.0f;
					}
				}
				Handles.matrix = Matrix4x4.TRS(_linePoints[i], Quaternion.identity, Vector3.one * linePointScale);
				Handles.DrawAAConvexPolygon(_cachedLinePointVerticies);
			}

			Handles.matrix = Matrix4x4.identity;
			Handles.DrawAAPolyLine(2.0f, aData.Length, _linePoints);

			if (mousePositionDiscovered)
			{
				labelRect.y -= 16.0f;
				labelRect.width += 50.0f;
				labelRect.x -= 25.0f;
				GUI.Label(labelRect, string.Format(labelFormat, mouseHoverDataValue), _centeredStyle);
			}
			Handles.color = c;
		}
	}
}