namespace Anthill.Core
{
	public class AntNode
	{
		public AntEntity entity { get; set; }
	}

	public class AntNode<T> : AntNode
	{
		public T Component1 { get; set; }
	}

	public class AntNode<T1, T2> : AntNode<T1>
	{
		public T2 Component2 { get; set; }
	}

	public class AntNode<T1, T2, T3> : AntNode<T1, T2>
	{
		public T3 Component3 { get; set; }
	}

	public class AntNode<T1, T2, T3, T4> : AntNode<T1, T2, T3>
	{
		public T4 Component4 { get; set; }
	}

	public class AntNode<T1, T2, T3, T4, T5> : AntNode<T1, T2, T3, T4>
	{
		public T5 Component5 { get; set; }
	}

	public class AntNode<T1, T2, T3, T4, T5, T6> : AntNode<T1, T2, T3, T4, T5>
	{
		public T6 Component6 { get; set; }
	}
}