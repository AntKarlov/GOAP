using UnityEngine;
using Anthill.Core;
using Game.Nodes;

namespace Game.Systems
{
	/// <summary>
	/// Данная система пользовательский ввод.
	/// </summary>
	public class PlayerControlSystem : ISystem, IExecuteSystem
	{
		private AntNodeList<PlayerControlNode> _playerNodes;

		#region ISystem Implementation
		
		public void AddedToEngine(AntEngine aEngine)
		{
			_playerNodes = aEngine.GetNodes<PlayerControlNode>();
		}

		public void RemovedFromEngine(AntEngine aEngine)
		{
			_playerNodes = null;
		}
		
		#endregion
		#region IExecuteSystem Implementation
		
		public void Execute()
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
		
		#endregion
	}
}