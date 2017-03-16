using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anthill.Utils
{
	public class AntDrawer
	{
		private static bool _useHandles = false;
		private static Texture2D _aaLineTexture = null;
		private static Texture2D _lineTexture = null;
		private static Material _blitMaterial = null;
		private static Material _blendMaterial = null;
		private static Rect _lineRect = new Rect(0, 0, 1, 1);
		private static Matrix4x4 _matrixBackup;

		public static void BeginHandles(Matrix4x4 aMatrix)
		{
			_useHandles = true;
			#if UNITY_EDITOR
			_matrixBackup = Handles.matrix;
			Handles.matrix = aMatrix;
			#endif
		}

		public static void EndHandles()
		{
			#if UNITY_EDITOR
			if (_useHandles)
			{
				Handles.matrix = _matrixBackup;
			}
			#endif
			_useHandles = false;
		}

		private static void Draw(float aX1, float aY1, float aX2, float aY2, Color aColor)
		{
			#if UNITY_EDITOR
			if (_useHandles)
			{
				Handles.color = aColor;
				Handles.DrawLine(new Vector3(aX1, aY1, 0.0f), new Vector3(aX2, aY2, 0.0f));
				return;
			}
			#endif

			if (!_useHandles)
			{
				Debug.DrawLine(new Vector3(aX1, aY1, 0.0f), new Vector3(aX2, aY2, 0.0f), aColor);
			}
		}

		public static void DrawLine(float aX1, float aY1, float aX2, float aY2, Color aColor)
		{
			Draw(aX1, aY1, aX2, aY2, aColor);
		}

		public static void DrawLine(Vector2 aPoint1, Vector2 aPoint2, Color aColor)
		{
			Draw(aPoint1.x, aPoint1.y, aPoint2.x, aPoint2.y, aColor);
		}

		public static void DrawCross(float aX, float aY, Color aColor, float aSize = 0.2f)
		{
			Draw(aX, aY - aSize, aX, aY + aSize, aColor);
			Draw(aX - aSize, aY, aX + aSize, aY, aColor);
		}

		public static void DrawCross(Vector2 aPoint, Color aColor, float aSize = 0.2f)
		{
			DrawCross(aPoint.x, aPoint.y, aColor, aSize);
		}

		public static void DrawSolidRect(float aX, float aY, float aWidth, float aHeight, Color aColor)
		{
			#if UNITY_EDITOR
			if (_useHandles)
			{
				Handles.color = aColor;
				Handles.DrawSolidRectangleWithOutline(new Vector3[]{
					new Vector3(aX, aY + aHeight, 0.0f),
					new Vector3(aX + aWidth, aY + aHeight, 0.0f),
					new Vector3(aX + aWidth, aY, 0.0f),
					new Vector3(aX, aY, 0.0f),
				}, new Color(0.5f, 0.5f, 0.5f, 0.1f), new Color(0.5f, 0.5f, 0.5f, 0.0f));
			}
			#endif
		}

		public static void DrawRect(float aX, float aY, float aWidth, float aHeight, Color aColor, float aAlpha = 0.1f)
		{
			#if UNITY_EDITOR
			if (_useHandles)
			{
				Handles.color = aColor;
				Handles.DrawSolidRectangleWithOutline(new Vector3[]{
					new Vector3(aX, aY + aHeight, 0.0f),
					new Vector3(aX + aWidth, aY + aHeight, 0.0f),
					new Vector3(aX + aWidth, aY, 0.0f),
					new Vector3(aX, aY, 0.0f),
				}, new Color(0.5f, 0.5f, 0.5f, aAlpha), Color.white);
			}
			#endif

			if (!_useHandles)
			{
				Draw(aX, aY + aHeight, aX + aWidth, aY + aHeight, aColor);
				Draw(aX + aWidth, aY + aHeight, aX + aWidth, aY, aColor);
				Draw(aX + aWidth, aY, aX, aY, aColor);
				Draw(aX, aY, aX, aY + aHeight, aColor);
			}
		}

		public static void DrawRect(Rect aRect, Color aColor)
		{
			DrawRect(aRect.x, aRect.y, aRect.width, aRect.height, aColor);
		}

		public static void DrawRotatedCircle(float aX, float aY, float aRadius, float aAngleDeg,
				Color aColor, int aVertices = 12)
		{
			if (aVertices >= 3)
			{
				Vector2[] vertices = new Vector2[aVertices];
				float rotation = AntMath.DegToRad(aAngleDeg);
				float angle = 0.0f;
				for (int i = 0; i < aVertices; i++)
				{
					angle = ((i / (float)aVertices) * 360f) / 180f * Mathf.PI;
					float dx = aRadius;
					float dy = 0f;

					vertices[i] = new Vector2(aX + Mathf.Cos(angle - rotation) * dx - Mathf.Sin(angle - rotation) * dy,
							aY - (Mathf.Sin(angle - rotation) * dx + Mathf.Cos(angle - rotation) * dy));

					if (i == 0)
					{
						DrawLine(aX, aY, vertices[i].x, vertices[i].y, aColor);
					}
				}

				DrawPath(vertices, aColor, true);
			}
		}

		public static void DrawRotatedCircle(Vector2 aPoint, float aRadius, float aAngleDeg,
				Color aColor, int aVertices = 12)
		{
			DrawRotatedCircle(aPoint.x, aPoint.y, aRadius, aAngleDeg, aColor, aVertices);
		}

		public static void DrawCircle(float aX, float aY, float aRadius, Color aColor, int aVertices = 12)
		{
			if (aVertices >= 3)
			{
				Vector2[] vertices = new Vector2[aVertices];
				float angle = 0.0f;
				for (int i = 0; i < aVertices; i++)
				{
					angle = ((i / (float)aVertices) * 360.0f) / 180.0f * Mathf.PI;
					float dx = aRadius;
					float dy = 0.0f;
					vertices[i] = new Vector2(aX + Mathf.Cos(angle) * dx - Mathf.Sin(angle) * dy,
							aY - (Mathf.Sin(angle) * dx + Mathf.Cos(angle) * dy));
				}
				DrawPath(vertices, aColor, true);
			}
		}

		public static void DrawCircle(Vector2 aPoint, float aRadius, Color aColor, int aVertices = 12)
		{
			DrawCircle(aPoint.x, aPoint.y, aRadius, aColor, aVertices);
		}

		public static void DrawPie(float aX, float aY, float aRadius, float aAngle, 
			float aLowerAngle, float aUpperAngle, Color aColor, int aVertices = 12)
		{
			if (aVertices >= 3)
			{
				Vector2[] vertices = new Vector2[aVertices + 1];
				float sum = Mathf.Abs(aLowerAngle) + Mathf.Abs(aUpperAngle);
				float angle = 0.0f;
				for (int i = 0; i < aVertices; i++)
				{
					angle = ((aLowerAngle + ((i / (float)(aVertices - 1)) * sum)) - aAngle) / 180.0f * Mathf.PI;
					float dx = aRadius;
					float dy = 0.0f;
					vertices[i] = new Vector2(aX + Mathf.Cos(angle) * dx - Mathf.Sin(angle) * dy,
							aY - (Mathf.Sin(angle) * dx + Mathf.Cos(angle) * dy));
				}
				vertices[vertices.Length - 1] = new Vector2(aX, aY);
				DrawPath(vertices, aColor, true);
			}
		}

		public static void DrawPie(Vector2 aPoint, float aRadius, float aAngle,
			float aLowerAngle, float aUpperAngle, Color aColor, int aVertices = 12)
		{
			DrawPie(aPoint.x, aPoint.y, aRadius, aAngle, aLowerAngle, aUpperAngle, aColor, aVertices);
		}

		public static void DrawConnection(float aX1, float aY1, float aX2, float aY2, Color aColor,
				int aDir = 1, float aTanOffset = 5.0f)
		{
			Vector2 startPos = new Vector2(aX1, aY1);
			Vector2 endPos = new Vector2(aX2, aY2);
			Vector2 startTan = (aDir == 1) ? startPos + Vector2.right * aTanOffset : startPos + Vector2.left * aTanOffset;
			Vector2 endTan = (aDir == 1) ? endPos + Vector2.left * aTanOffset : endPos + Vector2.right * aTanOffset;
			DrawPath(GetBezierCurve(new Vector2[4] { startPos, startTan, endTan, endPos }), aColor);
		}

		public static void DrawConnection(Vector2 aPoint1, Vector2 aPoint2, Color aColor,
				int aDir = 1, float aTanOffset = 5f)
		{
			DrawConnection(aPoint1.x, aPoint1.y, aPoint2.x, aPoint2.y, aColor, aDir, aTanOffset);
		}

		public static Vector2[] MakeConnection(float aX1, float aY1, float aX2, float aY2,
				int aDir = 1, float aTanOffset = 5.0f)
		{
			Vector2 startPos = new Vector2(aX1, aY1);
			Vector2 endPos = new Vector2(aX2, aY2);
			Vector2 startTan = (aDir == 1) ? startPos + Vector2.right * aTanOffset : startPos + Vector2.left * aTanOffset;
			Vector2 endTan = (aDir == 1) ? endPos + Vector2.left * aTanOffset : endPos + Vector2.right * aTanOffset;
			return GetBezierCurve(new Vector2[4] { startPos, startTan, endTan, endPos });
		}

		public static Vector2[] MakeConnection(Vector2 aPoint1, Vector2 aPoint2, int aDir = 1, float aTanOffset = 5.0f)
		{
			return MakeConnection(aPoint1.x, aPoint1.y, aPoint2.x, aPoint2.y, aDir, aTanOffset);
		}

		public static void DrawArrow(float aX1, float aY1, float aX2, float aY2, Color aColor, float aSize = 0.5f)
		{
			Vector2 p1 = new Vector2(aX1, aY1);
			Vector2 p2 = new Vector2(aX2, aY2);
			Vector2 p3 = AntMath.RotatePointDeg(p2.x + aSize, p2.y, p2.x, p2.y, AntMath.AngleDeg(aX1, aY1, aX2, aY2) - 145f);
			Vector2 p4 = AntMath.RotatePointDeg(p2.x + aSize, p2.y, p2.x, p2.y, AntMath.AngleDeg(aX1, aY1, aX2, aY2) + 145f);
			DrawPath(new Vector2[5] { p1, p2, p3, p4, p2 }, aColor);
		}

		public static void DrawArrow(Vector2 aPoint1, Vector2 aPoint2, Color aColor, float aSize = 0.5f)
		{
			DrawArrow(aPoint1.x, aPoint1.y, aPoint2.x, aPoint2.y, aColor, aSize);
		}

		public static void DrawRotatedPath(float aPivotX, float aPivotY, Vector2[] aPoints,
				float aAngleDeg, Color aColor, bool aLoop = false)
		{
			Vector2[] vertices = new Vector2[aPoints.Length];
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = AntMath.RotatePointDeg(aPoints[i].x, aPoints[i].y, aPivotX, aPivotY, aAngleDeg);
			}

			DrawPath(vertices, aColor, aLoop);
		}

		public static void DrawRotatedPath(Vector2 aPivot, Vector2[] aPoints, float aAngleDeg,
				Color aColor, bool aLoop = false)
		{
			DrawRotatedPath(aPivot.x, aPivot.y, aPoints, aAngleDeg, aColor, aLoop);
		}

		public static void DrawPath(Vector2[] aPoints, Color aColor, bool aLoop = false)
		{
			if (aPoints.Length >= 2)
			{
				Vector2 actual = aPoints[0];
				for (int i = 1; i < aPoints.Length; i++)
				{
					DrawLine(actual.x, actual.y, aPoints[i].x, aPoints[i].y, aColor);
					actual = aPoints[i];
				}

				if (aLoop)
				{
					DrawLine(actual.x, actual.y, aPoints[0].x, aPoints[0].y, aColor);
				}
			}
		}

		public static Vector2[] GetBezierCurve(Vector2[] aKeyPoints)
		{
			float length = 0f;
			float step = 0.025f;
			int n = aKeyPoints.Length;

			int k = 0;
			Vector2[] result = new Vector2[Mathf.RoundToInt(1.0f / step) + 1];
			while (length <= 1.0f)
			{
				Vector2 point = new Vector2();
				for (int i = 0; i < n; i++)
				{
					float b = GetBezierBasics(i, n - 1, length);
					point.x += aKeyPoints[i].x * b;
					point.y += aKeyPoints[i].y * b;
				}

				result[k++] = point;
				length += 0.025f;
			}

			result[result.Length - 1] = aKeyPoints[aKeyPoints.Length - 1];
			return result;
		}

		private static float GetBezierBasics(float aVertex, float aCount, float aStep)
		{
			return (Factorial(aCount) / (Factorial(aVertex) * Factorial(aCount - aVertex))) * Mathf.Pow(aStep, aVertex) * Mathf.Pow(1f - aStep, aCount - aVertex);
		}

		private static float Factorial(float aValue)
		{
			return (aValue <= 1f) ? 1f : aValue * Factorial(aValue - 1f);
		}

		#region Solid Lines

		public static void DrawSolidConnection(float aX1, float aY1, float aX2, float aY2, Color aColor,
				int aDir = 1, float aTanOffset = 5.0f)
		{
			DrawSolidConnection(new Vector2(aX1, aY1), new Vector2(aX2, aY2), aColor, aDir, aTanOffset);
		}

		public static void DrawSolidConnection(Vector2 aStartPos, Vector2 aEndPos, Color aColor, 
			int aDir = 1, float aTanOffset = 5.0f, float aWidth = 2.0f, bool aAntialias = true)
		{
			Vector2 startTan = (aDir == 1) ? aStartPos + Vector2.right * aTanOffset : aStartPos + Vector2.left * aTanOffset;
			Vector2 endTan = (aDir == 1) ? aEndPos + Vector2.left * aTanOffset : aEndPos + Vector2.right * aTanOffset;
			DrawSolidPath(GetBezierCurve(new Vector2[4] { aStartPos, startTan, endTan, aEndPos }), aColor, false, aWidth, aAntialias);
		}

		public static void DrawVerticalSolidConnection(Vector2 aStartPos, Vector2 aEndPos, Color aColor, 
			int aDir = 1, float aTanOffset = 5.0f, float aWidth = 2.0f, bool aAntialias = true)
		{
			Vector2 startTan = (aDir == 1) ? aStartPos + Vector2.up * aTanOffset : aStartPos + Vector2.down * aTanOffset;
			Vector2 endTan = (aDir == 1) ? aEndPos + Vector2.down * aTanOffset : aEndPos + Vector2.up * aTanOffset;
			DrawSolidPath(GetBezierCurve(new Vector2[4] { aStartPos, startTan, endTan, aEndPos }), aColor, false, aWidth, aAntialias);
		}

		public static void DrawSolidPath(Vector2[] aPoints, Color aColor, bool aLoop = false, 
			float aWidth = 2.0f, bool aAntialias = true)
		{
			if (aPoints.Length >= 2)
			{
				Vector2 actual = aPoints[0];
				for (int i = 1; i < aPoints.Length; i++)
				{
					DrawSolidLine(actual, aPoints[i], aColor, aWidth, aAntialias);
					actual = aPoints[i];
				}

				if (aLoop)
				{
					DrawSolidLine(actual, aPoints[0], aColor, aWidth, aAntialias);
				}
			}
		}

		public static void DrawSolidLine(Vector2 aPointA, Vector2 aPointB, Color aColor,
			float aWidth = 2.0f, bool aAntialias = true)
		{
			#if UNITY_EDITOR
			if (_lineTexture == null)
			{
				InitializeSolid();
			}
			#endif

			// Note that theta = atan2(dy, dx) is the angle we want to rotate by, but instead
        	// of calculating the angle we just use the sine (dy/len) and cosine (dx/len).
			float dx = aPointB.x - aPointA.x;
			float dy = aPointB.y - aPointA.y;
			float len = Mathf.Sqrt(dx * dx + dy * dy);

			// Early out on tiny lines to avoid divide by zero.
        	// Plus what's the point of drawing a line 1/1000th of a pixel long??
			if (len < 0.001f)
			{
				return;
			}

			// Pick texture and material (and tweak width) based on anti-alias setting.
			Texture2D tex;
			Material mat;
			if (aAntialias)
			{
				// Multiplying by three is fine for anti-aliasing width-1 lines, but make a wide "fringe"
            	// for thicker lines, which may or may not be desirable.
				aWidth *= 3.0f;
				tex = _aaLineTexture;
				mat = _blendMaterial;
			}
			else
			{
				tex = _lineTexture;
				mat = _blitMaterial;
			}

			float wdx = aWidth * dy / len;
			float wdy = aWidth * dx / len;

			var m = Matrix4x4.identity;
			m.m00 = dx;
			m.m01 = -wdx;
			m.m03 = aPointA.x + 0.5f * wdx;
			m.m10 = dy;
			m.m11 = wdy;
			m.m13 = aPointA.y - 0.5f * wdy;

			// Use GL matrix and Graphics.DrawTexture rather than GUI.matrix and GUI.DrawTexture,
        	// for better performance. (Setting GUI.matrix is slow, and GUI.DrawTexture is just a
        	// wrapper on Graphics.DrawTexture.)
			GL.PushMatrix();
			GL.MultMatrix(m);
			Graphics.DrawTexture(_lineRect, tex, _lineRect, 0, 0, 0, 0, aColor, mat);
			GL.PopMatrix();
		}

		private static void InitializeSolid()
		{
			if (_lineTexture == null)
			{
				_lineTexture = new Texture2D(1, 1, TextureFormat.ARGB32, true);
				_lineTexture.SetPixel(0, 1, Color.white);
				_lineTexture.Apply();
			}

			if (_aaLineTexture == null)
			{
				_aaLineTexture = new Texture2D(1, 3, TextureFormat.ARGB32, true);
				_aaLineTexture.SetPixel(0, 0, new Color(1.0f, 1.0f, 1.0f, 0.0f));
				_aaLineTexture.SetPixel(0, 1, Color.white);
				_aaLineTexture.SetPixel(0, 2, new Color(1.0f, 1.0f, 1.0f, 0.0f));
				_aaLineTexture.Apply();
			}

			// GUI.blitMaterial and GUI.blendMaterial are used internally by GUI.DrawTexture,
        	// depending on the alphaBlend parameter. Use reflection to "borrow" these references.
			_blitMaterial = (Material) typeof(GUI).GetMethod("get_blitMaterial", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
			_blendMaterial = (Material)typeof(GUI).GetMethod("get_blendMaterial", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
		}
		#endregion
	}
}