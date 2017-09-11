using System;
using System.Collections.Generic;
using UnityEngine;

namespace Anthill.Core
{
	public class AntTaskManager : ISystem, IExecuteSystem
	{
		public delegate void TaskManagerDelegate(AntTaskManager aTaskManager);
		public event TaskManagerDelegate EventStart;
		public event TaskManagerDelegate EventStop;
		public event TaskManagerDelegate EventComplete;

		private bool _isStarted;
		private bool _isPaused;
		private bool _isCycled;
		
		private List<AntTask> _taskList;
		private AntTask _currentTask;
		public float _delay;

		#region ISystem Implementation

		public void AddedToEngine(AntEngine aEngine)
		{
			// ..
		}

		public void RemovedFromEngine(AntEngine aEngine)
		{
			// ..
		}

		#endregion
		#region IExecuteSystem Implementation

		public void Execute()
		{
			if (_currentTask != null && _isStarted)
			{
				if (!_isPaused)
				{
					_currentTask.Update(Time.deltaTime);
					if (_currentTask.IsFinished)
					{
						if (_isCycled && !_currentTask.IsIgnoreCycle)
						{
							_currentTask.IsFinished = false;
							Push(_currentTask);
						}

						_currentTask = Shift();
					}
				}
			}
			else
			{
				Stop();
				if (EventComplete != null)
				{
					EventComplete(this);
				}
			}
		}

		#endregion
		#region Public Methods

		public AntTaskManager(bool aIsCycled = false)
		{
			_isStarted = false;
			_isPaused = false;
			_isCycled = aIsCycled;
			_taskList = new List<AntTask>();
			_currentTask = null;
			_delay = 0.0f;
		}

		public void AddDelay(float aDelay, bool aIgnoreCycle = false)
		{
			var task = new AntTask<AntTask, float>();
			task.SetProcess(OnDelay);
			task.SetArguments(task, aDelay);
			task.IsIgnoreCycle = aIgnoreCycle;
			Push(task);
			Start();
		}

		public bool OnDelay(AntTask aTask, float aDelay)
		{
			_delay += aTask.DeltaTime;
			if (_delay > aDelay)
			{
				_delay = 0.0f;
				return true;
			}
			return false;
		}

		public void AddTask(Func<bool> aFunc, bool aIgnoreCycle = false)
		{
			var task = new AntTask();
			task.SetProcess(aFunc);
			task.IsIgnoreCycle = aIgnoreCycle;
			Push(task);
			Start();
		}

		public void AddTask<T1>(Func<T1, bool> aFunc, 
			T1 aArg1, bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1>();
			task.SetProcess(aFunc);
			task.SetArguments(aArg1);
			task.IsIgnoreCycle = aIgnoreCycle;
			Push(task);
			Start();
		}

		public void AddTask<T1, T2>(Func<T1, T2, bool> aFunc, 
			T1 aArg1, T2 aArg2, bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2>();
			task.SetProcess(aFunc);
			task.SetArguments(aArg1, aArg2);
			task.IsIgnoreCycle = aIgnoreCycle;
			Push(task);
			Start();
		}

		public void AddTask<T1, T2, T3>(Func<T1, T2, T3, bool> aFunc, 
			T1 aArg1, T2 aArg2, T3 aArg3, bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2, T3>();
			task.SetProcess(aFunc);
			task.SetArguments(aArg1, aArg2, aArg3);
			task.IsIgnoreCycle = aIgnoreCycle;
			Push(task);
			Start();
		}

		public void AddTask<T1, T2, T3, T4>(Func<T1, T2, T3, T4, bool> aFunc, 
			T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4, bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2, T3, T4>();
			task.SetProcess(aFunc);
			task.SetArguments(aArg1, aArg2, aArg3, aArg4);
			task.IsIgnoreCycle = aIgnoreCycle;
			Push(task);
			Start();
		}

		public void AddUrgentTask(Func<bool> aFunc, bool aIgnoreCycle = false)
		{
			var task = new AntTask();
			task.SetProcess(aFunc);
			task.IsIgnoreCycle = aIgnoreCycle;
			Unshift(task);
			Start();
		}

		public void AddUrgentTask<T1>(Func<T1, bool> aFunc, 
			T1 aArg1, bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1>();
			task.SetProcess(aFunc);
			task.SetArguments(aArg1);
			task.IsIgnoreCycle = aIgnoreCycle;
			Unshift(task);
			Start();
		}

		public void AddUrgentTask<T1, T2>(Func<T1, T2, bool> aFunc, 
			T1 aArg1, T2 aArg2, bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2>();
			task.SetProcess(aFunc);
			task.SetArguments(aArg1, aArg2);
			task.IsIgnoreCycle = aIgnoreCycle;
			Unshift(task);
			Start();
		}

		public void AddUrgentTask<T1, T2, T3>(Func<T1, T2, T3, bool> aFunc, 
			T1 aArg1, T2 aArg2, T3 aArg3, bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2, T3>();
			task.SetProcess(aFunc);
			task.SetArguments(aArg1, aArg2, aArg3);
			task.IsIgnoreCycle = aIgnoreCycle;
			Unshift(task);
			Start();
		}

		public void AddUrgentTask<T1, T2, T3, T4>(Func<T1, T2, T3, T4, bool> aFunc, 
			T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4, bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2, T3, T4>();
			task.SetProcess(aFunc);
			task.SetArguments(aArg1, aArg2, aArg3, aArg4);
			task.IsIgnoreCycle = aIgnoreCycle;
			Unshift(task);
			Start();
		}

		public void Clear()
		{
			Stop();
			_taskList.Clear();
			_currentTask = null;
		}

		protected void Push(AntTask aTask)
		{
			_taskList.Add(aTask);
		}

		protected void Unshift(AntTask aTask)
		{
			if (_currentTask != null)
			{
				int index = _taskList.IndexOf(_currentTask);
				if (index >= 0 && index < _taskList.Count && 
					index + 1 < _taskList.Count)
				{
					_taskList.Insert(index + 1, aTask);
				}
				else
				{
					_taskList.Insert(0, aTask);
				}
			}
			else
			{
				_taskList.Insert(0, aTask);
			}
		}

		protected AntTask Shift()
		{
			AntTask result = null;
			if (_taskList.Count > 0)
			{
				result = _taskList[0];
				_taskList.RemoveAt(0);
			}
			return result;
		}

		protected void Start()
		{
			if (!_isStarted && _taskList.Count > 0)
			{
				if (AntEngine.Current != null)
				{
					AntEngine.Current.Add((ISystem) this);
					_currentTask = Shift();
					_isStarted = true;
					_isPaused = false;

					if (EventStart != null)
					{
						EventStart(this);
					}
				}
				else
				{
					AntLog.Report("AntTaskManager", "AntEngine not initialized!");
				}
			}
		}

		protected void Stop()
		{
			if (_isStarted)
			{
				if (AntEngine.Current != null)
				{
					AntEngine.Current.Remove((ISystem) this);
					_isStarted = false;
					if (EventStop != null)
					{
						EventStop(this);
					}	
				}
				else
				{
					AntLog.Report("AntTaskManager", "AntEngine not initialized!");
				}
			}
		}

		public virtual void Pause()
		{
			_isPaused = true;
		}

		public virtual void Resume()
		{
			_isPaused = false;
		}

		public bool IsStarted
		{
			get { return _isStarted; }
		}

		public bool IsPaused 
		{
			get { return _isPaused; }
		}

		#endregion
	}

	public class AntTask
	{
		public bool IsFinished { get; set; }
		public bool IsIgnoreCycle { get; set; }
		public float DeltaTime { get; set; }

		private Func<bool> _process;

		public AntTask()
		{
			IsFinished = false;
			IsIgnoreCycle = false;
		}

		public virtual void Update(float aDeltaTime)
		{
			DeltaTime = aDeltaTime;
			IsFinished = _process();
		}

		public void SetProcess(Func<bool> aProcess)
		{
			_process = aProcess;
		}
	}

	public class AntTask<T1> : AntTask
	{
		private Func<T1, bool> _process;
		protected T1 _arg1;

		public override void Update(float aDeltaTime)
		{
			DeltaTime = aDeltaTime;
			IsFinished = _process(_arg1);
		}

		public void SetProcess(Func<T1, bool> aProcess)
		{
			_process = aProcess;
		}

		public void SetArguments(T1 aArg1)
		{
			_arg1 = aArg1;
		}
	}

	public class AntTask<T1, T2> : AntTask<T1>
	{
		private Func<T1, T2, bool> _process;
		protected T2 _arg2;

		public override void Update(float aDeltaTime)
		{
			DeltaTime = aDeltaTime;
			IsFinished = _process(_arg1, _arg2);
		}

		public void SetProcess(Func<T1, T2, bool> aProcess)
		{
			_process = aProcess;
		}

		public void SetArguments(T1 aArg1, T2 aArg2)
		{
			_arg1 = aArg1;
			_arg2 = aArg2;
		}
	}

	public class AntTask<T1, T2, T3> : AntTask<T1, T2>
	{
		private Func<T1, T2, T3, bool> _process;
		protected T3 _arg3;

		public override void Update(float aDeltaTime)
		{
			DeltaTime = aDeltaTime;
			IsFinished = _process(_arg1, _arg2, _arg3);
		}

		public void SetProcess(Func<T1, T2, T3, bool> aProcess)
		{
			_process = aProcess;
		}

		public void SetArguments(T1 aArg1, T2 aArg2, T3 aArg3)
		{
			_arg1 = aArg1;
			_arg2 = aArg2;
			_arg3 = aArg3;
		}
	}

	public class AntTask<T1, T2, T3, T4> : AntTask<T1, T2, T3>
	{
		private Func<T1, T2, T3, T4, bool> _process;
		protected T4 _arg4;

		public override void Update(float aDeltaTime)
		{
			DeltaTime = aDeltaTime;
			IsFinished = _process(_arg1, _arg2, _arg3, _arg4);
		}

		public void SetProcess(Func<T1, T2, T3, T4, bool> aProcess)
		{
			_process = aProcess;
		}

		public void SetArguments(T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4)
		{
			_arg1 = aArg1;
			_arg2 = aArg2;
			_arg3 = aArg3;
			_arg4 = aArg4;
		}
	}
}