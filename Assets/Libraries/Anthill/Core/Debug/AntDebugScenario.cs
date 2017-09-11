using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

namespace Anthill.Core
{
	public enum AvgResetDuration
	{
		Always = 1,
		VeryFast = 30,
		Fast = 60,
		Normal = 120,
		Slow = 300,
		Never = int.MaxValue
	}

	public class AntDebugScenario : AntBaseScenario
	{
		public static AvgResetDuration avgResetDuration = AvgResetDuration.Never;

		public bool isPaused;

		private GameObject _container;
		private double _totalDuration;
		private Stopwatch _stopwatch;
		private List<AntSystemInfo> _initializeSystemsInfos;
		private List<AntSystemInfo> _executeSystemsInfos;

		public AntDebugScenario(string aName) : base(aName)
		{
			_container = new GameObject();
			_container.gameObject.AddComponent<AntDebugScenarioBehaviour>().Init(this);
			_totalDuration = 0;
			_stopwatch = new Stopwatch();
			_initializeSystemsInfos = new List<AntSystemInfo>();
			_executeSystemsInfos = new List<AntSystemInfo>();

			UpdateName();
		}

		public override void Add(ISystem aSystem, int aPriority = 0)
		{
			var debugScenario = aSystem as AntDebugScenario;
			if (debugScenario != null)
			{
				debugScenario.Container.transform.SetParent(_container.transform, false);
			}

			var systemInfo = new AntSystemInfo(aSystem);
			if (systemInfo.IsInitializeSystem)
			{
				_initializeSystemsInfos.Add(systemInfo);
			}

			if (systemInfo.IsExecuteSystem)
			{
				_executeSystemsInfos.Add(systemInfo);
			}

			base.Add(aSystem, aPriority);
		}

		public override void Remove(ISystem aSystem)
		{
			var debugScenario = aSystem as AntDebugScenario;
			if (debugScenario != null)
			{
				GameObject.Destroy(debugScenario.Container);
			}

			_initializeSystemsInfos.RemoveAll(x => System.Object.ReferenceEquals(x.System, aSystem));
			_executeSystemsInfos.RemoveAll(x => System.Object.ReferenceEquals(x.System, aSystem));

			base.Remove(aSystem);
		}

		public override void Initialize()
		{
			_totalDuration = 0;
			IInitializeSystem system;
			AntSystemInfo systemInfo;
			double duration;
			for (int i = _initializeSystems.Count; i >= 0; i--)
			{
				system = _initializeSystems[i].System;
				systemInfo = _initializeSystemsInfos[i];
				if (systemInfo.isActive)
				{
					duration = MonitorInitializeSystemDuration(system);
					_totalDuration += duration;
					systemInfo.AddExecutionDuration(duration);
				}
			}

			UpdateName();
		}

		public override void Execute()
		{
			if (!isPaused && _enabled)
			{
				Step();
			}
		}

		public void Step()
		{
			_totalDuration = 0;
			if (Time.frameCount % (int) avgResetDuration == 0)
			{
				ResetDurations();
			}

			IExecuteSystem system;
			AntSystemInfo systemInfo;
			double duration;
			for (int i = _executeSystems.Count - 1; i >= 0; i--)
			{
				system = _executeSystems[i].System;
				systemInfo = _executeSystemsInfos[i];
				if (systemInfo.isActive)
				{
					duration = MonitorExecuteSystemDuration(system);
					_totalDuration += duration;
					systemInfo.AddExecutionDuration(duration);
				}
			}

			UpdateName();
		}

		public void ResetDurations()
		{
			for (int i = 0, n = _initializeSystemsInfos.Count; i < n; i++)
			{
				_initializeSystemsInfos[i].ResetDurations();
			}

			for (int i = 0, n = _executeSystemsInfos.Count; i < n; i++)
			{
				_executeSystemsInfos[i].ResetDurations();
			}
		}

		#region Private Methods

		private void UpdateName()
		{
			if (_container != null)
			{
				_container.name = string.Format("{0} ({1} ini, {2} exe, {3:0.###} ms)", 
					Name, _initializeSystems.Count, _executeSystems.Count, _totalDuration);
			}
		}

		private double MonitorInitializeSystemDuration(IInitializeSystem aSystem)
		{
			_stopwatch.Reset();
			_stopwatch.Start();
			aSystem.Initialize();
			_stopwatch.Stop();
			return _stopwatch.Elapsed.TotalMilliseconds;
		}

		private double MonitorExecuteSystemDuration(IExecuteSystem aSystem)
		{
			_stopwatch.Reset();
			_stopwatch.Start();
			aSystem.Execute();
			_stopwatch.Stop();
			return _stopwatch.Elapsed.TotalMilliseconds;
		}

		#endregion
		#region Getters / Setters

		public GameObject Container
		{
			get { return _container; }
		}

		public int InitializeSystemsCount 
		{
			get { return _initializeSystems.Count; }
		}

		public int ExecuteSystemsCount 
		{
			get { return _executeSystems.Count; }
		}

		public int SystemsCount 
		{
			get { return _systems.Count; } 
		}

		public int TotalInitializeSystemsCount
		{
			get
			{
				int count = 0;
				for (int i = 0, n = _initializeSystems.Count; i < n; i++)
				{
					var debugScenario = _initializeSystems[i].System as AntDebugScenario;
					if (debugScenario != null)
					{
						count += debugScenario.TotalInitializeSystemsCount;
					}
					else
					{
						count++;
					}
				}
				return count;
			}
		}

		public int TotalExecuteSystemsCount
		{
			get
			{
				int count = 0;
				for (int i = 0, n = _executeSystems.Count; i < n; i++)
				{
					var debugScenario = _executeSystems[i].System as AntDebugScenario;
					if (debugScenario != null)
					{
						count += debugScenario.TotalExecuteSystemsCount;
					}
					else
					{
						count++;
					}
				}
				return count;
			}
		}

		public double TotalDuration
		{
			get { return _totalDuration; }
		}

		public List<AntSystemInfo> InitializeSystemsInfos
		{
			get { return _initializeSystemsInfos; }
		}

		public List<AntSystemInfo> ExecuteSystemInfos
		{
			get { return _executeSystemsInfos; }
		}

		#endregion
	}
}