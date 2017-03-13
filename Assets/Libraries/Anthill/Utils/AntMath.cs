using UnityEngine;

namespace Anthill.Utils
{
	public static class AntMath
	{
		private const float DEGREES = 180.0f / Mathf.PI;
		private const float RADIANS = Mathf.PI / 180.0f;

		private static Vector3 _mouseHelper = new Vector3(0, 0, 10.0f);

		public static float ScaleByFOV(float aFOV, float aDistance, float aViewHeight)
		{
			// focal length is the position where objects are seen at their 
			// natural size on the screen
			// fLength = viewHeight / 2 arctan(radianFOV)
			float fl = aViewHeight / (2.0f * Mathf.Tan(aFOV * DEGREES));
			// scale = fLength / (fLength + distanceFromViewer);
			return 1.0f / (fl / (fl + aDistance));
		}

		public static Vector3 GetMouseWorldPosition()
		{
			_mouseHelper.x = Input.mousePosition.x;
			_mouseHelper.y = Input.mousePosition.y;
			return Camera.main.ScreenToWorldPoint(_mouseHelper);
		}

		public static bool IsVisibleFrom(Renderer aRenderer, Camera aCamera)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(aCamera);
			return GeometryUtility.TestPlanesAABB(planes, aRenderer.bounds);
		}

		public static float Angle(float aValue)			
		{
			float ang = 180.0f;
			bool inv = aValue < 0.0f;
			
			aValue = (inv ? -aValue : aValue) % 360;
			
			if (aValue > ang)			
			{
				aValue = -ang + (aValue - ang);
			}
			
			return (inv ? -aValue : aValue);			
		}

		public static float AngleDifferenceRad(float aX, float aY)
		{
			return Mathf.Atan2(Mathf.Sin(aX - aY), Mathf.Cos(aX - aY));
		}

		public static float AngleDifferenceDeg(float aX, float aY)
		{
			aX *= RADIANS;
			aY *= RADIANS;
			return Mathf.Atan2(Mathf.Sin(aX - aY), Mathf.Cos(aX - aY)) * DEGREES;
		}

		public static bool AngleArea(float aAngle, float aTarget, float aArea)
		{
			return Mathf.Abs(AngleDifferenceDeg(aAngle, aTarget)) <= aArea;
		}

		public static float InvertAngle(float aValue)
		{
			return Angle(aValue) + 180.0f;
		}

		public static float AbsAngle(float aValue)
		{
			return (aValue < 0.0f) ? 180.0f + (180.0f + aValue) : aValue;
		}

		public static float LerpAngle(float aLower, float aUpper, float aCoef)
		{
			if (Mathf.Abs(aLower - aUpper) > 180.0f)
			{
				if (aLower > aUpper)
				{
					aUpper += 360.0f;
				}
				else
				{
					aUpper -= 360.0f;
				}
			}

			aLower += (aUpper - aLower) * aCoef;
			return Angle(aLower);
		}

		public static float LimitAngle(float aAngle, float aLimit)
		{
			if (aAngle > aLimit)
			{
				aAngle = aLimit;
			}
			else if (aAngle < -aLimit)
			{
				aAngle = -aLimit;
			}
			return aAngle;
		}

		public static float AngleRad(float aX1, float aY1, float aX2, float aY2)
		{
			return Mathf.Atan2(aY2 - aY1, aX2 - aX1);
		}

		public static float AngleRad(Vector2 aPoint1, Vector2 aPoint2)
		{
			return AngleRad(aPoint1.x, aPoint1.y, aPoint2.x, aPoint2.y);
		}

		public static float AngleRad(float aX, float aY)
		{
			return Mathf.Atan2(aY, aX);
		}

		public static float AngleRad(Vector2 aPoint)
		{
			return Mathf.Atan2(aPoint.y, aPoint.x);
		}

		public static float AngleDeg(float aX1, float aY1, float aX2, float aY2)
		{
			return Mathf.Atan2(aY2 - aY1, aX2 - aX1) * DEGREES;
		}

		public static float AngleDeg(Vector2 aPoint1, Vector2 aPoint2)
		{
			return AngleDeg(aPoint1.x, aPoint1.y, aPoint2.x, aPoint2.y);
		}

		public static float AngleDeg(float aX, float aY)
		{
			return Mathf.Atan2(aY, aX) * DEGREES;
		}

		public static float AngleDeg(Vector2 aPoint)
		{
			return Mathf.Atan2(aPoint.x, aPoint.y);
		}

		public static float NormAngleDeg(float aAngle)
		{
			if (aAngle < 0.0f)
			{
				aAngle = 360.0f + aAngle;
			}
			else if (aAngle >= 360.0f)
			{
				aAngle = aAngle - 360.0f;
			}
			return aAngle;
		}

		public static float NormAngleRad(float aAngle)
		{
			if (aAngle < 0.0f)
			{
				aAngle = Mathf.PI * 2.0f + aAngle;
			}
			else if (aAngle >= Mathf.PI * 2.0f)
			{
				aAngle = aAngle - Mathf.PI * 2.0f;
			}
			return aAngle;
		}

		public static Vector2 RotatePointDeg(float aPointX, float aPointY, float aPivotX, float aPivotY, float aAngle)
		{
			aAngle = -aAngle * RADIANS;
			float dx = aPointX - aPivotX;
			float dy = aPivotY - aPointY;
			Vector2 result = new Vector2();
			result.x = aPivotX + Mathf.Cos(aAngle) * dx - Mathf.Sin(aAngle) * dy;
			result.y = aPivotY - (Mathf.Sin(aAngle) * dx + Mathf.Cos(aAngle) * dy);
			return result;
		}

		public static Vector2 RotatePointDeg(Vector2 aPoint, Vector2 aPivot, float aAngle)
		{
			return RotatePointDeg(aPoint.x, aPoint.y, aPivot.x, aPivot.y, aAngle);
		}

		public static Vector2 RotatePointRad(float aPointX, float aPointY, float aPivotX, float aPivotY, float aAngle)
		{
			aAngle = -aAngle;
			float dx = aPointX - aPivotX;
			float dy = aPivotY - aPointY;
			Vector2 result = new Vector2();
			result.x = aPivotX + Mathf.Cos(aAngle) * dx - Mathf.Sin(aAngle) * dy;
			result.y = aPivotY - (Mathf.Sin(aAngle) * dx + Mathf.Cos(aAngle) * dy);
			return result;
		}

		public static Vector2 RotatePointRad(Vector2 aPoint, Vector2 aPivot, float aAngle)
		{
			return RotatePointRad(aPoint.x, aPoint.y, aPivot.x, aPivot.y, aAngle);
		}

		public static float RadToDeg(float aAngle)
		{
			return aAngle * DEGREES;
		}

		public static float DegToRad(float aAngle)
		{
			return aAngle * RADIANS;
		}

		public static bool InRange(float aValue, float aLower, float aUpper)
		{
			return (aValue > aLower && aValue < aUpper);
		}

		public static float Closest(float aValue, float aOut1, float aOut2)
		{
			return (Mathf.Abs(aValue - aOut1) < Mathf.Abs(aValue - aOut2)) ? aOut1 : aOut2;
		}

		public static float Limit(float aValue, float aLimit)
		{
			aValue = (aValue < 0.0f) ? -aValue : aValue;
			if (aValue - aLimit < 0.0f)
			{
				aValue = 0.0f;
			}
			return aValue;
		}

		public static int RandomRangeInt(int aLower, int aUpper)
		{
			return Mathf.RoundToInt(Random.value * (aUpper - aLower)) + aLower;
		}

		/// <summary>
		/// Возвращает случайное дробное число из заданного диапазона.
		/// </summary>
		/// <param name="aLower">Наименьшее значение диапазона.</param>
		/// <param name="aUpper">Наибольшее значение диапазона.</param>
		/// <returns>Случайное дробное.</returns>
		public static float RandomRangeFloat(float aLower, float aUpper)
		{
			return Random.value * (aUpper - aLower) + aLower;
		}

		/// <summary>
		/// Сравнивает указанные значения с возможной погрешностью.
		/// </summary>
		/// <param name="aValueA">Первое значение.</param>
		/// <param name="aValueB">Второе значение.</param>
		/// <param name="aDiff">Погрешность.</param>
		/// <returns>Возвращает true если значения равны.</returns>
		public static bool Equal(float aValueA, float aValueB, float aDiff = 0.00001f)
		{
			return (Mathf.Abs(aValueA - aValueB) <= aDiff);
		}

		public static bool Equal(Vector2 aValueA, Vector2 aValueB, float aDiff = 0.00001f)
		{
			return (Equal(aValueA.x, aValueB.x, aDiff) && Equal(aValueA.y, aValueB.y, aDiff));
		}

		/// <summary>
		/// Переводит значение из одного диапазона в другой.
		/// </summary>
		/// <param name="aValue">Значение.</param>
		/// <param name="aLower1">Наименьшее значение первого диапазона.</param>
		/// <param name="aUpper1">Наибольшее значение первого диапазона.</param>
		/// <param name="aLower2">Наименьшее значение второго диапазона.</param>
		/// <param name="aUpper2">Наибольшее значение второго диапазона.</param>
		/// <returns>Новое значение.</returns>
		public static float Remap(float aValue, float aLower1, float aUpper1, float aLower2, float aUpper2)
		{
			return aLower2 + (aUpper2 - aLower2) * (aValue - aLower1) / (aUpper1 - aLower1);
		}

		/// <summary>
		///	Ограничивает значение заданным диапазоном.
		/// </summary>
		/// <param name="aValue">Значение.</param>
		/// <param name="aLower">Наименьшее значение диапазона.</param>
		/// <param name="aUpper">Наибольшее значение диапазона.</param>
		/// <returns>Если значение выходит за рамки диапазона, то вернется граница диапазона.</returns>
		public static float TrimToRange(float aValue, float aLower, float aUpper)
		{
			return (aValue > aUpper) ? aUpper : (aValue < aLower) ? aLower : aValue;
		}

		public static float Lerp(float aLower, float aUpper, float aCoef)
		{
			//return aLower + aCoef * (aUpper - aLower);
			return aLower + ((aUpper - aLower) * aCoef);
		}

		public static float Distance(float aX1, float aY1, float aX2, float aY2)
		{
			float dx = aX2 - aX1;
			float dy = aY2 - aY1;
			return Mathf.Sqrt(dx * dx + dy * dy);
		}

		public static float Distance(Vector2 aPoint1, Vector2 aPoint2)
		{
			return Distance(aPoint1.x, aPoint1.y, aPoint2.x, aPoint2.y);
		}

		public static float ToPercent(float aCurrent, float aTotal)
		{
			return (aCurrent / aTotal);
		}

		public static float FromPercent(float aPercent, float aTotal)
		{
			return (aPercent * aTotal);
		}

		public static float Max(float aValueA, float aValueB)
		{
			return (aValueA > aValueB) ? aValueA : aValueB;
		}

		public static int Max(int aValueA, int aValueB)
		{
			return (aValueA > aValueB) ? aValueA : aValueB;
		}

		public static float Min(float aValueA, float aValueB)
		{
			return (aValueA < aValueB) ? aValueA : aValueB;
		}

		public static int Min(int aValueA, int aValueB)
		{
			return (aValueA < aValueB) ? aValueA : aValueB;
		}

		public static Vector3 Vec2ToVec3(Vector2 aSource)
		{
			return new Vector3(aSource.x, aSource.y);
		}

		public static Vector2 Vec3ToVec2(Vector3 aSource)
		{
			return new Vector2(aSource.x, aSource.y);
		}
	}
}