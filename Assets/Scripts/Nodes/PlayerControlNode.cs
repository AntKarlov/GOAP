using Anthill.Core;
using Game.Components;

namespace Game.Nodes
{
	public class PlayerControlNode : AntNode
	{
		public TankControl TankControl { get; set; }
		public PlayerControl PlayerControl { get; set; }
	}
}