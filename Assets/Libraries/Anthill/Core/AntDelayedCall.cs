using System;

namespace Anthill.Core
{
	public class DelayedCall
	{
		public float delay;
		private Action _process;

		public virtual bool Update(float aDeltaTime)
		{
			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_process();
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action aProcess)
		{
			_process = aProcess;
		}
	}

	public class DelayedCall<T1> : DelayedCall
	{
		private Action<T1> _process;
		protected T1 _arg1;

		public override bool Update(float aDeltaTime)
		{
			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_process(_arg1);
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action<T1> aProcess)
		{
			_process = aProcess;
		}

		public void SetArgumens(T1 aArg1)
		{
			_arg1 = aArg1;
		}
	}

	public class DelayedCall<T1, T2> : DelayedCall<T1>
	{
		private Action<T1, T2> _process;
		protected T2 _arg2;

		public override bool Update(float aDeltaTime)
		{
			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_process(_arg1, _arg2);
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action<T1, T2> aProcess)
		{
			_process = aProcess;
		}

		public void SetArgumens(T1 aArg1, T2 aArg2)
		{
			_arg1 = aArg1;
			_arg2 = aArg2;
		}
	}

	public class DelayedCall<T1, T2, T3> : DelayedCall<T1, T2>
	{
		private Action<T1, T2, T3> _process;
		protected T3 _arg3;

		public override bool Update(float aDeltaTime)
		{
			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_process(_arg1, _arg2, _arg3);
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action<T1, T2, T3> aProcess)
		{
			_process = aProcess;
		}

		public void SetArgumens(T1 aArg1, T2 aArg2, T3 aArg3)
		{
			_arg1 = aArg1;
			_arg2 = aArg2;
			_arg3 = aArg3;
		}
	}

	public class DelayedCall<T1, T2, T3, T4> : DelayedCall<T1, T2, T3>
	{
		private Action<T1, T2, T3, T4> _process;
		protected T4 _arg4;

		public override bool Update(float aDeltaTime)
		{
			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_process(_arg1, _arg2, _arg3, _arg4);
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action<T1, T2, T3, T4> aProcess)
		{
			_process = aProcess;
		}

		public void SetArgumens(T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4)
		{
			_arg1 = aArg1;
			_arg2 = aArg2;
			_arg3 = aArg3;
			_arg4 = aArg4;
		}
	}
}