using UnityEngine;
using Anthill.Core;
using Anthill.Utils;
using Game.Nodes;

namespace Game.Systems
{
	/// <summary>
	/// Данная система обрабатывает движение танка исходя их сигналов в компоненте TankControl.
	/// </summary>
	public class MovementSystem : ISystem, IExecuteSystem
	{
		private AntNodeList<MovementNode> _movementNodes;
		private AntNodeList<HealthNode> _healthNodes;

		#region ISystem Implementation
		
		public void AddedToEngine(AntEngine aEngine)
		{
			_movementNodes = aEngine.GetNodes<MovementNode>();
			_healthNodes = aEngine.GetNodes<HealthNode>();
		}

		public void RemovedFromEngine(AntEngine aEngine)
		{
			_movementNodes = null;
			_healthNodes = null;
		}
		
		#endregion
		#region IExecuteSystem Implementation
		
		public void Execute()
		{
			bool playAnim = false;
			MovementNode node;
			for (int i = 0, n = _movementNodes.Count; i < n; i++)
			{
				node = _movementNodes[i];
				playAnim = false;
				if (node.TankControl.isLeft)
				{
					node.TankControl.Steering(1.0f, Time.deltaTime);
					playAnim = true;
				}
				else if (node.TankControl.isRight)
				{
					node.TankControl.Steering(-1.0f, Time.deltaTime);
					playAnim = true;
				}

				if (node.TankControl.isForward)
				{
					node.TankControl.Move(1.0f, Time.deltaTime);
					playAnim = true;
				}
				else if (node.TankControl.isBackward)
				{
					node.TankControl.Move(-1.0f, Time.deltaTime);
					playAnim = true;
				}
				else
				{
					node.TankControl.Move(0.0f, Time.deltaTime);
				}

				if (node.TankControl.isTowerLeft)
				{
					node.TankControl.TowerRotation(1.0f, Time.deltaTime);
				}
				else if (node.TankControl.isTowerRight)
				{
					node.TankControl.TowerRotation(-1.0f, Time.deltaTime);
				}

				if (node.TankControl.isFire)
				{
					Fire(node);
					node.TankControl.isFire = false;
				}

				if (playAnim && !node.Actor.IsPlaying)
				{
					node.Actor.Play();
				}
				else if (!playAnim && node.Actor.IsPlaying)
				{
					node.Actor.Stop();
				}
			}
		}
		
		#endregion
		#region Private Methods
		
		private void Fire(MovementNode aNode)
		{
			if (aNode.TankControl.Tower.HasGun && 
				aNode.TankControl.Tower.HasAmmo)
			{
				Transform go = GameObject.Instantiate((GameObject) aNode.TankControl.Tower.bulletPrefab).GetComponent<Transform>();
				float ang = AntMath.DegToRad(aNode.TankControl.Tower.Angle);
				Vector3 p = aNode.entity.Position3D;
				p.x += aNode.TankControl.Tower.bulletPosition * Mathf.Cos(ang);
				p.y += aNode.TankControl.Tower.bulletPosition * Mathf.Sin(ang);
				go.position = p;

				Vector2 force = new Vector2();
				force.x = aNode.TankControl.Tower.bulletSpeed * Mathf.Cos(ang);
				force.y = aNode.TankControl.Tower.bulletSpeed * Mathf.Sin(ang);
				go.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

				Quaternion q = Quaternion.AngleAxis(aNode.TankControl.Tower.Angle, Vector3.forward);
				go.rotation = q;

				aNode.TankControl.Tower.AmmoCount--;
			}
			else if (aNode.TankControl.Tower.HasBomb)
			{
				float dist;
				HealthNode hpNode;
				for (int i = _healthNodes.Count - 1; i >= 0; i--)
				{
					hpNode = _healthNodes[i];
					dist = AntMath.Distance(aNode.entity.Position, hpNode.entity.Position);
					if (dist < 2.5f)
					{
						hpNode.Health.HP = 0.0f;
					}
				}
			}
		}
		
		#endregion
	}
}