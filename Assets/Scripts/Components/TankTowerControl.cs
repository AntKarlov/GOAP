using UnityEngine;
using Anthill.Animation;

namespace Game.Components
{
	public class TankTowerControl : MonoBehaviour
	{
		[Tooltip("Префаб пули.")]
		public GameObject bulletPrefab;
		[Tooltip("Расстояние от центра на котором будет создаваться пуля.")]
		public float bulletPosition = 0.25f;
		[Tooltip("Скорость пули.")]
		public float bulletSpeed = 1.0f;

		private AntActor _actor;
		private bool _hasGun;
		private bool _hasBomb;
		private int _ammoCount;
		private Transform _t;

		// -----------------------------------------------------
		// Unity Callbacks
		// -----------------------------------------------------

		private void Awake()
		{
			_t = GetComponent<Transform>();
			_actor = GetComponent<AntActor>();
			_actor.GotoAndStop(1);
		}

		// -----------------------------------------------------
		// Private Methods
		// -----------------------------------------------------

		private void UpdateVisual()
		{
			if (_hasGun)
			{
				_actor.SwitchAnimation("GunTower");
				_actor.GotoAndStop(_ammoCount + 1);
			}
			else if (_hasBomb)
			{
				_actor.SwitchAnimation("BombTower");
				_actor.GotoAndStop(1);
			}
			else if (!_hasGun && !_hasBomb)
			{
				_actor.SwitchAnimation("None");
				_actor.GotoAndStop(1);
			}
		}

		// -----------------------------------------------------
		// Getters/Setters
		// -----------------------------------------------------

		public float Angle
		{
			get { return _t.rotation.eulerAngles.z; }
		}

		public Quaternion Rotation
		{
			get { return _t.rotation; }
			set { _t.rotation = value; }
		}

		public bool HasGun
		{
			get { return _hasGun; }
			set
			{
				_hasGun = value;
				_hasBomb = false;
				UpdateVisual();
			}
		}

		public bool HasBomb
		{
			get { return _hasBomb; }
			set
			{
				_hasBomb = value;
				_hasGun = false;
				UpdateVisual();
			}
		}

		public bool HasAmmo
		{
			get { return _ammoCount > 0; }
		}

		public int AmmoCount
		{
			get { return _ammoCount; }
			set
			{
				_ammoCount = (value < 3) ? value : 3;
				UpdateVisual();
			}
		}
	}
}