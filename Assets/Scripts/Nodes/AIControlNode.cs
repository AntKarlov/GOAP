using Anthill.Core;
using Game.Components;

namespace Game.Nodes
{
	public class AIControlNode : AntNode
	{
		public TankControl TankControl { get; set; }
		public AIControl AIControl { get; set; }
	}
}