using UnityEngine;
using Anthill.Core;
using Game.Nodes;

namespace Game.Systems
{
	/// <summary>
	/// Данная система обрабатывает управление танком игрока.
	/// </summary>
	public class PlayerControlSystem : AntSystem
	{
		private AntNodeList<PlayerControlNode> _playerNodes;

		public override void AddedToEngine(AntEngine aEngine)
		{
			_playerNodes = aEngine.GetNodes<PlayerControlNode>();
		}

		public override void RemovedFromEngine(AntEngine aEngine)
		{
			base.RemovedFromEngine(aEngine);
			_playerNodes = null;
		}

		public override void Update(float aDeltaTime)
		{
			PlayerControlNode node;
			for (int i = 0, n = _playerNodes.Count; i < n; i++)
			{
				node = _playerNodes[i];
				node.TankControl.isLeft = (Input.GetAxis("Horizontal") < -0.1f);
				node.TankControl.isRight = (Input.GetAxis("Horizontal") > 0.1f);
				node.TankControl.isForward = (Input.GetAxis("Vertical") > 0.1f);
				node.TankControl.isBackward = (Input.GetAxis("Vertical") < -0.1f);
				node.TankControl.isTowerLeft = Input.GetButton("TowerLeft");
				node.TankControl.isTowerRight = Input.GetButton("TowerRight");
				node.TankControl.isFire = Input.GetButtonDown("Fire1");
			}
		}
	}
}