namespace Anthill.Core
{
	public class AntSystem : ISystem
	{
		public AntEngine Engine { get; set; }
		public int Priority { get; set; }
		protected bool _isPaused = false;

		public virtual void AddedToEngine(AntEngine aEngine)
		{
			// ..
		}

		public virtual void RemovedFromEngine(AntEngine aEngine)
		{
			// ..
		}

		public virtual void Update(float aDeltaTime)
		{
			// ..
		}

		public virtual void Reset()
		{
			// ..
		}

		public virtual void Pause()
		{
			_isPaused = true;
		}

		public virtual void Resume()
		{
			_isPaused = false;
		}

		public bool IsPaused
		{
			get { return _isPaused; }
		}
	}

	public class AntSystem<T1> : AntSystem
	{
		private AntNodeList<AntNode<T1>> _nodes;
		
		public override void AddedToEngine(AntEngine aEngine)
		{
			_nodes = aEngine.GetNodes<AntNode<T1>>();
		}

		public override void RemovedFromEngine(AntEngine aEngine)
		{
			aEngine.ReleaseNodes(_nodes);
		}

		public override void Update(float aDeltaTime)
		{
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				UpdateNode(aDeltaTime, _nodes[i].Component1);
			}
		}

		protected virtual void UpdateNode(float aDeltaTime, T1 aComp1)
		{
			// ..
		}
	}

	public class AntSystem<T1, T2> : AntSystem
	{
		private AntNodeList<AntNode<T1, T2>> _nodes;

		public override void AddedToEngine(AntEngine aEngine)
		{
			_nodes = aEngine.GetNodes<AntNode<T1, T2>>();
		}

		public override void RemovedFromEngine(AntEngine aEngine)
		{
			aEngine.ReleaseNodes(_nodes);
		}

		public override void Update(float aDeltaTime)
		{
			AntNode<T1, T2> node;
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				node = _nodes[i];
				UpdateNode(aDeltaTime, node.Component1, node.Component2);
			}
		}

		protected virtual void UpdateNode(float aDeltaTime, T1 aComp1, T2 aComp2)
		{
			// ..
		}
	}

	public class AntSystem<T1, T2, T3> : AntSystem
	{
		private AntNodeList<AntNode<T1, T2, T3>> _nodes;

		public override void AddedToEngine(AntEngine aEngine)
		{
			_nodes = aEngine.GetNodes<AntNode<T1, T2, T3>>();
		}

		public override void RemovedFromEngine(AntEngine aEngine)
		{
			aEngine.ReleaseNodes(_nodes);
		}

		public override void Update(float aDeltaTime)
		{
			AntNode<T1, T2, T3> node;
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				node = _nodes[i];
				UpdateNode(aDeltaTime, node.Component1, node.Component2, node.Component3);
			}
		}

		protected virtual void UpdateNode(float aDeltaTime, T1 aComp1, T2 aComp2, T3 aComp3)
		{
			// ..
		}
	}

	public class AntSystem<T1, T2, T3, T4> : AntSystem
	{
		private AntNodeList<AntNode<T1, T2, T3, T4>> _nodes;

		public override void AddedToEngine(AntEngine aEngine)
		{
			_nodes = aEngine.GetNodes<AntNode<T1, T2, T3, T4>>();
		}

		public override void RemovedFromEngine(AntEngine aEngine)
		{
			aEngine.ReleaseNodes(_nodes);
		}

		public override void Update(float aDeltaTime)
		{
			AntNode<T1, T2, T3, T4> node;
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				node = _nodes[i];
				UpdateNode(aDeltaTime, node.Component1, node.Component2, node.Component3, node.Component4);
			}
		}

		protected virtual void UpdateNode(float aDeltaTime, T1 aComp1, T2 aComp2, T3 aComp3, T4 aComp4)
		{
			// ..
		}
	}

	public class AntSystem<T1, T2, T3, T4, T5> : AntSystem
	{
		private AntNodeList<AntNode<T1, T2, T3, T4, T5>> _nodes;

		public override void AddedToEngine(AntEngine aEngine)
		{
			_nodes = aEngine.GetNodes<AntNode<T1, T2, T3, T4, T5>>();
		}

		public override void RemovedFromEngine(AntEngine aEngine)
		{
			aEngine.ReleaseNodes(_nodes);
		}

		public override void Update(float aDeltaTime)
		{
			AntNode<T1, T2, T3, T4, T5> node;
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				node = _nodes[i];
				UpdateNode(aDeltaTime, node.Component1, node.Component2, node.Component3, node.Component4, node.Component5);
			}
		}

		protected virtual void UpdateNode(float aDeltaTime, T1 aComp1, T2 aComp2, T3 aComp3, T4 aComp4, T5 aComp5)
		{
			// ..
		}
	}

	public class AntSystem<T1, T2, T3, T4, T5, T6> : AntSystem
	{
		private AntNodeList<AntNode<T1, T2, T3, T4, T5, T6>> _nodes;

		public override void AddedToEngine(AntEngine aEngine)
		{
			_nodes = aEngine.GetNodes<AntNode<T1, T2, T3, T4, T5, T6>>();
		}

		public override void RemovedFromEngine(AntEngine aEngine)
		{
			aEngine.ReleaseNodes(_nodes);
		}

		public override void Update(float aDeltaTime)
		{
			AntNode<T1, T2, T3, T4, T5, T6> node;
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				node = _nodes[i];
				UpdateNode(aDeltaTime, node.Component1, node.Component2, node.Component3, node.Component4, node.Component5, node.Component6);
			}
			/*foreach (var node in _nodes)
			{
				UpdateNode(aDeltaTime, node.Component1, node.Component2, node.Component3, node.Component4, node.Component5, node.Component6);
			}*/
		}

		protected virtual void UpdateNode(float aDeltaTime, T1 aComp1, T2 aComp2, T3 aComp3, T4 aComp4, T5 aComp5, T6 aComp6)
		{
			// ..
		}
	}
}