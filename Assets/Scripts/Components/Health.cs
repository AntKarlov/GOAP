using UnityEngine;

namespace Game.Components
{
	public class Health : MonoBehaviour
	{
		[Tooltip("Максимальное здоровье.")]
		public float maxHP = 3.0f;
		[Tooltip("Стартовое здоровье.")]
		public float initialHP = 2.0f;

		private float _hp;

		private void Awake()
		{
			_hp = initialHP;
		}

		public float HP
		{
			get { return _hp; }
			set 
			{
				_hp = value;
				if (_hp > maxHP)
				{
					_hp = maxHP;
				}
			}
		}
	}
}