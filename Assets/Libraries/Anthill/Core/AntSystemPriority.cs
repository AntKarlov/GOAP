namespace Anthill.Core
{
	public class AntSystemPriority
	{
		public ISystem System { get; private set; }
		public int Priority { get; private set; }

		public AntSystemPriority(ISystem aSystem, int aPriority)
		{
			System = aSystem;
			Priority = aPriority;
		}
	}
}