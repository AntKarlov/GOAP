using UnityEngine;
using Anthill.Animation;

namespace Game.Components
{
	public class HealthBar : MonoBehaviour
	{
		[Tooltip("Смещение панели здоровья относительно юнита.")]
		public Vector2 offset = new Vector2();

		private Transform _t;
		private AntActor _actor;
		private SpriteRenderer _sprite;
		private float _maxHP;
		private float _hp;

		private void Awake()
		{
			_t = GetComponent<Transform>();
			_actor = GetComponent<AntActor>();
			_sprite = GetComponent<SpriteRenderer>();
		}

		private void UpdateVisual()
		{
			int f = Mathf.FloorToInt((_hp / _maxHP) * (float) _actor.TotalFrames) + 1;
			f = (f <= 0) ? 1 : (f > _actor.TotalFrames) ? _actor.TotalFrames : f;
			_sprite.enabled = (f == _actor.TotalFrames) ? false : true;
			_actor.GotoAndStop(f);
		}

		public void Destroy()
		{
			DestroyObject(this.gameObject);
		}

		public Vector3 Position
		{
			get { return _t.position; }
			set
			{
				var p = _t.position;
				p.x = offset.x + value.x;
				p.y = offset.y + value.y;
				_t.position = p;
			}
		}

		public float HP
		{
			get { return _hp; }
			set
			{
				_hp = value;
				UpdateVisual();
			}
		}

		public float MaxHP
		{
			get { return _maxHP; }
			set 
			{
				_maxHP = value;
				UpdateVisual();
			}
		}
	}
}