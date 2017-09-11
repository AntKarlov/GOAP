namespace Anthill.Core
{
	public class AntPriorityPair<T>
	{
		public T System { get; private set; }
		public int Priority { get; private set; }

		public AntPriorityPair(T aSystem, int aPriority)
		{
			System = aSystem;
			Priority = aPriority;
		}
	}
}