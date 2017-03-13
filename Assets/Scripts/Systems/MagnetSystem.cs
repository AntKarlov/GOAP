using UnityEngine;
using Anthill.Core;
using Anthill.Utils;
using Game.Nodes;
using Game.Components;
using Game.Core;

namespace Game.Systems
{
	/// <summary>
	/// Данная система обрабатывает примагничивание и сбор игровых вещей.
	/// </summary>
	public class MagnetSystem : AntSystem
	{
		private AntNodeList<MagnetNode> _magnetNodes;
		private AntNodeList<MagnetableNode> _magnetableNodes;

		private GameCore _gameCore;

		public override void AddedToEngine(AntEngine aEngine)
		{
			base.AddedToEngine(aEngine);
			_magnetNodes = aEngine.GetNodes<MagnetNode>();
			_magnetableNodes = aEngine.GetNodes<MagnetableNode>();

			_gameCore = GameObject.Find("Game").GetComponent<GameCore>();
		}

		public override void RemovedFromEngine(AntEngine aEngine)
		{
			base.RemovedFromEngine(aEngine);
			_magnetNodes = null;
			_magnetableNodes = null;
		}

		public override void Update(float aDeltaTime)
		{
			float dist = 0.0f;
			float ang = 0.0f;
			Vector2 force = new Vector2();
			MagnetNode magnet;
			MagnetableNode item;
			for (int i = 0, n = _magnetNodes.Count; i < n; i++)
			{
				magnet = _magnetNodes[i];
				for (int j = _magnetableNodes.Count - 1; j >= 0; j--)
				{
					item = _magnetableNodes[j];
					item.Magnetable.lifeTime += aDeltaTime;
					dist = AntMath.Distance(magnet.entity.Position, item.entity.Position);
					if ((magnet.Magnet.magnetKind == item.Magnetable.kind || 
						magnet.Magnet.magnetKind == ItemKind.All) &&
						dist < magnet.Magnet.distance &&
						item.Magnetable.lifeTime > 2.0f)
					{
						ang = AntMath.AngleRad(item.entity.Position, magnet.entity.Position);
						force.x = magnet.Magnet.force * Mathf.Cos(ang);
						force.y = magnet.Magnet.force * Mathf.Sin(ang);
						item.Magnetable.body.AddForce(force, ForceMode2D.Impulse);

						if (dist <= magnet.Magnet.collectDistance)
						{
							CollectItem(magnet, item);
							Engine.RemoveEntity(item.entity);
							GameObject.Destroy(item.entity.gameObject);
						}
					}
				}
			}
		}

		private void CollectItem(MagnetNode aTankNode, MagnetableNode aItemNode)
		{
			TankControl control = aTankNode.entity.GetComponent<TankControl>();
			if (control)
			{
				switch (aItemNode.Magnetable.kind)
				{
					case ItemKind.Gun :
						if (control.Tower.HasBomb)
						{
							// Сбрасываем с бота бомбу если есть.
							DropItem(ItemKind.Bomb, aTankNode.entity.Position);
							control.Tower.HasBomb = false;
						}
						control.Tower.HasGun = true;
					break;

					case ItemKind.Bomb :
						if (control.Tower.HasGun)
						{
							// Сбрасываем с бота пушку если есть.
							DropItem(ItemKind.Gun, aTankNode.entity.Position);
							control.Tower.HasGun = false;
						}

						if (control.Tower.HasAmmo)
						{
							// Сбрасываем с бота патроны если есть.
							DropItem(ItemKind.Ammo, aTankNode.entity.Position);
							control.Tower.AmmoCount = 0;
						}
						control.Tower.HasBomb = true;
					break;

					case ItemKind.Ammo :
						control.Tower.AmmoCount = 3;
					break;

					case ItemKind.Heal :
						aTankNode.entity.GetComponent<Health>().HP += 1.0f;
					break;
				}
			}
		}

		private void DropItem(ItemKind aKind, Vector3 aPosition)
		{
			GameObject go = null;
			switch (aKind)
			{
				case ItemKind.Bomb :
					go = GameObject.Instantiate((GameObject)_gameCore.bombItemPrefab);
				break;

				case ItemKind.Gun :
					go = GameObject.Instantiate((GameObject)_gameCore.gunItemPrefab);
				break;

				case ItemKind.Ammo :
					go = GameObject.Instantiate((GameObject)_gameCore.ammoItemPrefab);
				break;

				case ItemKind.Heal :
					go = GameObject.Instantiate((GameObject)_gameCore.healItemPrefab);
				break;
			}

			if (go != null)
			{
				float angle = AntMath.DegToRad(AntMath.RandomRangeFloat(-180, 180));
				Vector2 force = new Vector2();
				force.x = 0.5f * Mathf.Cos(angle);
				force.y = 0.5f * Mathf.Sin(angle);

				go.GetComponent<Transform>().position = aPosition;
				go.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
				Engine.AddEntity(go.GetComponent<AntEntity>());
			}
		}
	}
}