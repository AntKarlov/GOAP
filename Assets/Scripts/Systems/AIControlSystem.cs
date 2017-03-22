using Anthill.Core;
using Game.Components;
using Game.Nodes;

namespace Game.Systems
{
	/// <summary>
	/// Данная система отвечает за работу AI.
	/// </summary>
	public class AIControlSystem : AntSystem
	{
		private AntNodeList<AIControlNode> _aiNodes;

		public override void AddedToEngine(AntEngine aEngine)
		{
			_aiNodes = aEngine.GetNodes<AIControlNode>();
		}

		public override void RemovedFromEngine(AntEngine aEngine)
		{
			_aiNodes = null;
		}

		public override void Update(float aDeltaTime)
		{
			AIControl ai;
			for (int i = 0, n = _aiNodes.Count; i < n; i++)
			{
				ai = _aiNodes[i].AIControl;
				ai.currentTime -= aDeltaTime;
				if (ai.currentTime <= 0.0f)
				{
					ai.Agent.Think();
					ai.currentTime = ai.updateInterval;
				}

				ai.Agent.UpdateState(aDeltaTime);
			}
		}
	}
}