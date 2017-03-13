using Anthill.Core;
using Anthill.Animation;
using Game.Components;

namespace Game.Nodes
{
	public class MovementNode : AntNode
	{
		public AntActor Actor { get; set; }
		public TankControl TankControl { get; set; }
	}
}