using System;

namespace Anthill.AI
{
	public class AntAITask
	{
		public bool isFinished;
		private Func<bool> _func;

		public AntAITask()
		{
			isFinished = false;
		}

		public void SetFunc(Func<bool> aFunction)
		{
			_func = aFunction;
		}

		public virtual void Update()
		{
			isFinished = _func();
		}
	}

	public class AntAITask<T1> : AntAITask
	{
		private Func<T1, bool> _func;
		protected T1 _arg1;

		public void SetFunc(Func<T1, bool> aFunction)
		{
			_func = aFunction;
		}

		public void SetArg(T1 aArg1)
		{
			_arg1 = aArg1;
		}

		public override void Update()
		{
			isFinished = _func(_arg1);
		}
	}

	public class AntAITask<T1, T2> : AntAITask<T1>
	{
		private Func<T1, T2, bool> _func;
		protected T2 _arg2;

		public void SetFunc(Func<T1, T2, bool> aFunction)
		{
			_func = aFunction;
		}

		public void SetArg(T1 aArg1, T2 aArg2)
		{
			_arg1 = aArg1;
			_arg2 = aArg2;
		}

		public override void Update()
		{
			isFinished = _func(_arg1, _arg2);
		}
	}
}