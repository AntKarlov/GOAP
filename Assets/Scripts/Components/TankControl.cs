using UnityEngine;
using Anthill.Utils;

namespace Game.Components
{
	/// <summary>
	/// Данный компонент осуществляет управление танком. Этот компонент необходим
	/// для любого вида танка независимо от того кем он управляется.
	/// </summary>
	public class TankControl : MonoBehaviour
	{
		public float steering = 100.0f;
		public float speed = 10.0f;

		public bool isLeft = false;
		public bool isRight = false;
		public bool isForward = false;
		public bool isBackward = false;
		public bool isFire = false;
		public bool isTowerLeft = false;
		public bool isTowerRight = false;
		public Vector2 aimTarget = new Vector2();

		private Transform _t;
		private Rigidbody2D _body;
		private TankTowerControl _tower;

		#region Unity Callbacks

		private void Awake()
		{
			_t = GetComponent<Transform>();
			_body = GetComponent<Rigidbody2D>();
			_tower = GetComponentInChildren<TankTowerControl>();
		}
		
		#endregion
		#region Public Methods

		public void Steering(float aDir, float aDeltaTime)
		{
			float angle = _t.rotation.eulerAngles.z + steering * aDir * 0.6f;
			_body.rotation = AntMath.LerpAngle(_body.rotation, angle, aDeltaTime);

			/*
			// Старый код вращения без физ тела.
			float angle = _t.rotation.eulerAngles.z + steering * aDir;
			Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
 			_t.rotation = Quaternion.Slerp(_t.rotation, q, aDeltaTime);
			//*/ 
		}

		public void Move(float aDir, float aDeltaTime)
		{
			float angle = AntMath.DegToRad(_t.rotation.eulerAngles.z);
			Vector2 force = new Vector2();
			force.x = speed * aDir * Mathf.Cos(angle) * aDeltaTime * 50.0f;
			force.y = speed * aDir * Mathf.Sin(angle) * aDeltaTime * 50.0f;
			_body.velocity = force;

			/*
			// Старый код движения без физ тела.
			Vector3 pos = _t.position;
			pos.x += speed * aDir * Mathf.Cos(angle) * aDeltaTime;
			pos.y += speed * aDir * Mathf.Sin(angle) * aDeltaTime;
			_t.position = pos;//*/
		}

		public void TowerRotation(float aDir, float aDeltaTime)
		{
			float angle = _tower.Rotation.eulerAngles.z + steering * aDir;
			Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
 			_tower.Rotation = Quaternion.Slerp(_tower.Rotation, q, aDeltaTime);
		}

		#endregion
		#region Getters / Setters

		public Vector3 Position
		{
			get { return _t.position; }
		}

		public float Angle
		{
			get { return _t.rotation.eulerAngles.z; }
		}

		public TankTowerControl Tower
		{
			get { return _tower; }
		}

		#endregion
	}
}