using UnityEngine;
using Anthill.Core;
using Anthill.Utils;
using Game.Core;

namespace Game.Components
{
	public class Dropper : MonoBehaviour
	{
		[Tooltip("Тип вещи которую будет сбрасывать данная точка.")]
		public ItemKind dropKind = ItemKind.None;
		[Tooltip("Будет сбрасываться случайная вещь (dropKind игнорируется).")]
		public bool randomDrop = false;
		[Tooltip("Время между подбором существующей вещи и сбросом новой.")]
		public float rechargeTime = 5.0f;
		[Tooltip("Зона в пределах которой дроппер отслеживает вещь.")]
		public float observeRadius = 0.5f;

		public bool IsEmpty { get; set; }
		public float Delay { get; set; }

		private Transform _t;
		private GameCore _gameCore;
		private ItemKind[] _rndList = new ItemKind[] { 
			ItemKind.Gun,
			ItemKind.Bomb,
			ItemKind.Ammo,
			ItemKind.Heal };

		private void Awake()
		{
			_t = GetComponent<Transform>();
			_gameCore = GameObject.Find("Game").GetComponent<GameCore>();
		}

		public void DropItem()
		{
			GameObject go = null;
			ItemKind kind = (randomDrop) ? _rndList[AntMath.RandomRangeInt(0, _rndList.Length - 1)] : dropKind;
			switch (kind)
			{
				case ItemKind.Bomb :
					go = GameObject.Instantiate((GameObject) _gameCore.bombItemPrefab);
				break;

				case ItemKind.Gun :
					go = GameObject.Instantiate((GameObject) _gameCore.gunItemPrefab);
				break;

				case ItemKind.Ammo :
					go = GameObject.Instantiate((GameObject) _gameCore.ammoItemPrefab);
				break;

				case ItemKind.Heal :
					go = GameObject.Instantiate((GameObject) _gameCore.healItemPrefab);
				break;
			}

			if (go != null)
			{
				go.GetComponent<Transform>().position = _t.position;
				AntEngine.Current.AddEntity(go.GetComponent<AntEntity>());
			}
		}
	}
}