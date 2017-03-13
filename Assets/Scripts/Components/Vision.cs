using UnityEngine;
using Anthill.Utils;
using Game.Core;

namespace Game.Components
{
	public class Vision : MonoBehaviour
	{
		[Tooltip("Дистанция/радиус зрения.")]
		public float radius = 2.0f;
		[Tooltip("Минимальный угол зрения.")]
		public float lowerLimit = -90.0f;
		[Tooltip("Максимальный угол зрения.")]
		public float upperLimit = 90.0f;
		[Tooltip("Группа к которой относится данный объект.")]
		public GroupEnum group = GroupEnum.None;
		[Tooltip("Вражеская группа для данного объекта.")]
		public GroupEnum enemyGroup = GroupEnum.None;

		private Transform _t;

		// -----------------------------------------------------
		// Unity Callbacks
		// -----------------------------------------------------

		private void Awake()
		{
			_t = GetComponent<Transform>();
		}

		private void Update()
		{
			if (Config.Instance.showVision)
			{
				AntDrawer.DrawCircle(_t.position.x, _t.position.y, radius, Color.white);

				float ang = AntMath.DegToRad(Angle + lowerLimit);
				Vector2 p = new Vector2();
				p.x = _t.position.x + radius * Mathf.Cos(ang);
				p.y = _t.position.y + radius * Mathf.Sin(ang);
				AntDrawer.DrawLine(_t.position.x, _t.position.y, p.x, p.y, Color.red);

				ang = AntMath.DegToRad(Angle + upperLimit);
				p.x = _t.position.x + radius * Mathf.Cos(ang);
				p.y = _t.position.y + radius * Mathf.Sin(ang);
				AntDrawer.DrawLine(_t.position.x, _t.position.y, p.x, p.y, Color.red);
			}
		}

		// -----------------------------------------------------
		// Public Methods
		// -----------------------------------------------------

		public bool IsSee(Vector2 aPoint)
		{
			if (AntMath.Distance(_t.position.x, _t.position.y, aPoint.x, aPoint.y) < radius)
			{
				float angle = AntMath.Angle(AntMath.AngleDeg(_t.position.x, _t.position.y, aPoint.x, aPoint.y));
				float diff = AntMath.AngleDifferenceDeg(angle, Angle);
				if (AntMath.InRange(diff, lowerLimit, upperLimit))
				{
					if (Config.Instance.showVision)
					{
						AntDrawer.DrawLine(_t.position.x, _t.position.y, aPoint.x, aPoint.y, Color.green);
					}
					return true;
				}
			}
			return false;
		}

		// -----------------------------------------------------
		// Gettets/Setters
		// -----------------------------------------------------

		public float Angle
		{
			get { return _t.rotation.eulerAngles.z; }
		}
	}
}