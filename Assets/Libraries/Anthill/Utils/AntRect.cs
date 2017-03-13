using UnityEngine;

namespace Anthill.Utils
{
	public class AntRect
	{
		public float x;
		public float y;
		public float width;
		public float height;
		public float left;
		public float top;
		public float right;
		public float bottom;

		public AntRect(float aX = 0.0f, float aY = 0.0f, float aWidth = 0.0f, float aHeight = 0.0f)
		{
			x = aX;
			y = aY;
			width = aWidth;
			height = aHeight;
			UpdateBounds();
		}

		public void UpdateBounds()
		{
			left = x;
			right = x + width;
			top = y + height;
			bottom = y;
		}

		public bool IsInside(float aX, float aY)
		{
			UpdateBounds();
			return (aX >= left && aX <= right && aY >= bottom && aY <= top);
		}

		public bool IsInside(Vector2 aPoint)
		{
			return IsInside(aPoint.x, aPoint.y);
		}

		public float X
		{
			get { return x; }
			set
			{
				x = value;
				UpdateBounds();
			}
		}

		public float Y
		{
			get { return y; }
			set
			{
				y = value;
				UpdateBounds();
			}
		}

		public float Width
		{
			get { return width; }
			set
			{
				width = value;
				UpdateBounds();
			}
		}

		public float Height
		{
			get { return height; }
			set
			{
				height = value;
				UpdateBounds();
			}
		}

		public float Left
		{
			get { return left; }
		}

		public float Top
		{
			get { return top; }
		}

		public float Right
		{
			get { return right; }
		}

		public float Bottom
		{
			get { return bottom; }
		}
	}
}