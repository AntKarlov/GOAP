using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Anthill.Core
{
	public class AntBaseScenario : ISystem, IInitializeSystem, IDeinitializeSystem, IExecuteSystem, ICleanupSystem, IEnableSystem, IDisableSystem
	{
		protected enum PendingChange
		{
			Add,
			Remove
		}

		public string Name { get; private set; }
		
		private int _lockCount;
		protected bool _enabled;
		protected AntEngine _engine;

		protected List<AntPriorityPair<ISystem>> _systems;
		protected List<AntPriorityPair<IInitializeSystem>> _initializeSystems;
		protected List<AntPriorityPair<IDeinitializeSystem>> _deinitializeSystems;
		protected List<AntPriorityPair<IExecuteSystem>> _executeSystems;
		protected List<AntPriorityPair<IEnableSystem>> _enableSystems;
		protected List<AntPriorityPair<IDisableSystem>> _disableSystems;
		protected List<AntPriorityPair<ICleanupSystem>> _cleanupSystems;
		protected List<KeyValuePair<AntPriorityPair<ISystem>, PendingChange>> _pending;

		public AntBaseScenario(string aName)
		{
			Name = aName;

			_systems = new List<AntPriorityPair<ISystem>>();
			_initializeSystems = new List<AntPriorityPair<IInitializeSystem>>();
			_deinitializeSystems = new List<AntPriorityPair<IDeinitializeSystem>>();
			_executeSystems = new List<AntPriorityPair<IExecuteSystem>>();
			_enableSystems = new List<AntPriorityPair<IEnableSystem>>();
			_disableSystems = new List<AntPriorityPair<IDisableSystem>>();
			_cleanupSystems = new List<AntPriorityPair<ICleanupSystem>>();
			_pending = new List<KeyValuePair<AntPriorityPair<ISystem>, PendingChange>>();

			_enabled = true;
		}

		#region Public Methods

		public virtual void Add<T>(int aPriority = 0)
		{
			Type type = typeof(T);
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			ISystem system = (ISystem) constructor.Invoke(null);
			AntLog.Assert(system == null, "Class \"" + AntLog.Bold(type.ToString()) + "\" not implemented ISystem interface!");
			Add(system, aPriority);
		}

		public virtual void Add(ISystem aSystem, int aPriority = 0)
		{
			if (IsLocked)
			{
				_pending.Add(new KeyValuePair<AntPriorityPair<ISystem>, PendingChange>(new AntPriorityPair<ISystem>(aSystem, aPriority), PendingChange.Add));
			}
			else
			{
				_systems.Add(new AntPriorityPair<ISystem>(aSystem, aPriority));
				_systems = _systems.OrderBy(x => x.Priority).ToList();

				var initializeSystem = aSystem as IInitializeSystem;
				if (initializeSystem != null)
				{
					_initializeSystems.Add(new AntPriorityPair<IInitializeSystem>(initializeSystem, aPriority));
					_initializeSystems = _initializeSystems.OrderBy(x => x.Priority).ToList();
				}

				var deinitializeSystem = aSystem as IDeinitializeSystem;
				if (deinitializeSystem != null)
				{
					_deinitializeSystems.Add(new AntPriorityPair<IDeinitializeSystem>(deinitializeSystem, aPriority));
					_deinitializeSystems = _deinitializeSystems.OrderBy(x => x.Priority).ToList();
				}

				var disableSystem = aSystem as IDisableSystem;
				if (disableSystem != null)
				{
					_disableSystems.Add(new AntPriorityPair<IDisableSystem>(disableSystem, aPriority));
					_disableSystems = _disableSystems.OrderBy(x => x.Priority).ToList();
				}

				var enableSystem = aSystem as IEnableSystem;
				if (enableSystem != null)
				{
					_enableSystems.Add(new AntPriorityPair<IEnableSystem>(enableSystem, aPriority));
					_enableSystems = _enableSystems.OrderBy(x => x.Priority).ToList();
				}

				var executeSystem = aSystem as IExecuteSystem;
				if (executeSystem != null)
				{
					_executeSystems.Add(new AntPriorityPair<IExecuteSystem>(executeSystem, aPriority));
					_executeSystems = _executeSystems.OrderBy(x => x.Priority).ToList();
				}

				var cleanupSystem = aSystem as ICleanupSystem;
				if (cleanupSystem != null)
				{
					_cleanupSystems.Add(new AntPriorityPair<ICleanupSystem>(cleanupSystem, aPriority));
					_cleanupSystems = _cleanupSystems.OrderBy(x => x.Priority).ToList();
				}
				
				aSystem.AddedToEngine(_engine);
			}
		}

		public virtual void Remove(ISystem aSystem)
		{
			if (IsLocked)
			{
				_pending.Add(new KeyValuePair<AntPriorityPair<ISystem>, PendingChange>(new AntPriorityPair<ISystem>(aSystem, 0), PendingChange.Remove));
			}
			else
			{
				_systems.RemoveAll(x => System.Object.ReferenceEquals(x.System, aSystem));

				var initializeSystem = aSystem as IInitializeSystem;
				if (initializeSystem != null)
				{
					_initializeSystems.RemoveAll(x => System.Object.ReferenceEquals(x.System, initializeSystem));
				}
				
				var deinitializeSystem = aSystem as IDeinitializeSystem;
				if (deinitializeSystem != null)
				{
					_deinitializeSystems.RemoveAll(x => System.Object.ReferenceEquals(x.System, deinitializeSystem));
				}

				var disableSystem = aSystem as IDisableSystem;
				if (disableSystem != null)
				{
					_disableSystems.RemoveAll(x => System.Object.ReferenceEquals(x.System, disableSystem));
				}

				var enableSystem = aSystem as IEnableSystem;
				if (enableSystem != null)
				{
					_enableSystems.RemoveAll(x => System.Object.ReferenceEquals(x.System, enableSystem));
				}

				var executeSystem = aSystem as IExecuteSystem;
				if (executeSystem != null)
				{
					_executeSystems.RemoveAll(x => System.Object.ReferenceEquals(x.System, executeSystem));
				}

				var cleanupSystem = aSystem as ICleanupSystem;
				if (cleanupSystem != null)
				{
					_cleanupSystems.RemoveAll(x => System.Object.ReferenceEquals(x.System, cleanupSystem));
				}

				aSystem.RemovedFromEngine(_engine);
			}
		}

		public T Remove<T>()
		{
			T sys = Get<T>();
			if (sys != null)
			{
				Remove((ISystem) sys);
			}
			return sys;
		}

		public T Get<T>()
		{
			int index = _systems.FindIndex(x => x.System is T);
			return (index >= 0 && index < _systems.Count)
				? (T) _systems[index].System
				: default(T);
		}

		#endregion
		#region Protected Methods

		private void Lock()
		{
			_lockCount++;
		}

		private void Unlock()
		{
			_lockCount--;
			if (_lockCount <= 0)
			{
				_lockCount = 0;
				ApplyPending();
			}
		}

		private void ApplyPending()
		{
			KeyValuePair<AntPriorityPair<ISystem>, PendingChange> pair;
			for (int i = 0, n = _pending.Count; i < n; i++)
			{
				pair = _pending[i];
				if (pair.Value == PendingChange.Add)
				{
					Add(pair.Key.System, pair.Key.Priority);
				}
				else
				{
					Remove(pair.Key.System);
				}
			}

			_pending.Clear();
		}

		#endregion
		#region ISystem Implementation

		public virtual void AddedToEngine(AntEngine aEngine)
		{
			_engine = aEngine;
		}

		public virtual void RemovedFromEngine(AntEngine aEngine)
		{
			_engine = null;
		}

		#endregion
		#region IInitializeSystem Implementation

		public virtual void Initialize()
		{
			for (int i = _initializeSystems.Count - 1 ; i >= 0; i--)
			{
				_initializeSystems[i].System.Initialize();
			}
		}

		#endregion
		#region IDeinitializeSystem Implementation

		public virtual void Deinitialize()
		{
			for (int i = _deinitializeSystems.Count - 1; i >= 0; i--)
			{
				_deinitializeSystems[i].System.Deinitialize();
			}
		}

		#endregion
		#region IExecuteSystem Implementation
		
		public virtual void Execute()
		{
			if (_enabled)
			{
				for (int i = _executeSystems.Count - 1; i >= 0; i--)
				{
					_executeSystems[i].System.Execute();
				}
			}
		}

		#endregion
		#region ICleanupSystem Implementation

		public virtual void Cleanup()
		{
			if (_enabled)
			{
				for (int i = _cleanupSystems.Count - 1; i >= 0; i--)
				{
					_cleanupSystems[i].System.Cleanup();
				}
			}
		}

		#endregion
		#region IDisableSystem Implementation

		public virtual void Disable()
		{
			for (int i = _disableSystems.Count - 1; i >= 0; i--)
			{
				_disableSystems[i].System.Disable();
			}

			_enabled = false;
		}

		#endregion
		#region IEnableSystem Implementation

		public virtual void Enable()
		{
			for (int i = _enableSystems.Count - 1; i >= 0; i--)
			{
				_enableSystems[i].System.Enable();
			}

			_enabled = true;
		}

		#endregion
		#region Getters Setters

		public bool IsLocked
		{
			get { return (_lockCount > 0); }
		}

		#endregion
	}
}