using UnityEngine;
using Anthill.Core;

using Game.Components;
using Game.Nodes;

namespace Game.Systems
{
	/// <summary>
	/// Данная система отвечает за работу AI.
	/// </summary>
	public class AIControlSystem : ISystem, IExecuteSystem
	{
		private AntNodeList<AIControlNode> _aiNodes;
		
		#region ISystem Implementation
		
		public void AddedToEngine(AntEngine aEngine)
		{
			_aiNodes = aEngine.GetNodes<AIControlNode>();
		}

		public void RemovedFromEngine(AntEngine aEngine)
		{
			_aiNodes = null;
		}
		
		#endregion
		#region IExecuteSystem Implementation

		public void Execute()
		{
			AIControl ai;
			for (int i = 0, n = _aiNodes.Count; i < n; i++)
			{
				ai = _aiNodes[i].AIControl;
				ai.currentTime -= Time.deltaTime;
				if (ai.currentTime <= 0.0f)
				{
					ai.Agent.Think();
					ai.currentTime = ai.updateInterval;
				}

				ai.Agent.UpdateState(Time.deltaTime);
			}
		}

		#endregion
	}
}