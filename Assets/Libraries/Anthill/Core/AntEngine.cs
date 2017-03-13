using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Anthill.Core
{
	public class AntEngine
	{
		internal enum PendingChange
		{
			Add,
			Remove
		}

		public static AntEngine Current { get; private set; }

		private List<AntSystemPriority> _systems;
		private Dictionary<Type, IFamily> _families;
		private List<AntEntity> _entities;
		private List<KeyValuePair<ISystem, PendingChange>> _pending;
		private int _lockCount;

		public AntEngine()
		{
			Current = this;
			_systems = new List<AntSystemPriority>();
			_families = new Dictionary<Type, IFamily>();
			_entities = new List<AntEntity>();
			_pending = new List<KeyValuePair<ISystem, PendingChange>>();
		}

		public void AddEntitiesFromHierarchy(Transform aParent)
		{
			Transform child;
			AntEntity entity;
			for (int i = 0, n = aParent.childCount; i < n; i++)
			{
				child = aParent.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					entity = child.GetComponent<AntEntity>();
					if (entity != null)
					{
						AddEntity(entity);
					}

					if (child.childCount > 0)
					{
						AddEntitiesFromHierarchy(child);
					}
				}
			}
		}

		public void AddEntity(GameObject aObjectWithEntity, bool aIncludeChildren = false)
		{
			AddEntity(aObjectWithEntity.transform, aIncludeChildren);
		}

		public void AddEntity(Transform aObjectWithEntity, bool aIncludeChildren = false)
		{
			AntEntity entity = aObjectWithEntity.GetComponent<AntEntity>();
			if (entity != null)
			{
				AddEntity(entity);
				if (aIncludeChildren && aObjectWithEntity.childCount > 0)
				{
					for (int i = 0, n = aObjectWithEntity.childCount; i < n; i++)
					{
						AddEntity(aObjectWithEntity.GetChild(i), aIncludeChildren);
					}
				}
			}
		}

		public void AddEntity(AntEntity aEntity)
		{
			foreach (var pair in _families)
			{
				pair.Value.EntityAdded(aEntity);
			}

			aEntity.EventComponentAdded += OnComponentAdded;
			aEntity.EventComponentRemoved += OnComponentRemoved;
			_entities.Add(aEntity);
			aEntity.OnAddedToEngine();
		}

		public void RemoveEntity(AntEntity aEntity)
		{
			foreach (var pair in _families)
			{
				pair.Value.EntityRemoved(aEntity);
			}

			aEntity.EventComponentAdded -= OnComponentAdded;
			aEntity.EventComponentRemoved -= OnComponentRemoved;
			_entities.Remove(aEntity);
			aEntity.OnRemovedFromEngine();
		}

		private void OnComponentAdded(AntEntity aEntity, Type aComponent)
		{
			foreach (var pair in _families)
			{
				pair.Value.ComponentAdded(aEntity, aComponent);
			}
		}

		private void OnComponentRemoved(AntEntity aEntity, Type aComponent)
		{
			foreach (var pair in _families)
			{
				pair.Value.ComponentRemoved(aEntity, aComponent);
			}
		}

		public void AddSystem(ISystem aSystem, int aPriority)
		{
			aSystem.Priority = aPriority;
			if (!IsLocked)
			{
				_systems.Add(new AntSystemPriority(aSystem, aPriority));
				_systems = _systems.OrderBy(sys => sys.Priority).ToList();
				aSystem.Engine = this;
				aSystem.AddedToEngine(this);
			}
			else
			{
				_pending.Add(new KeyValuePair<ISystem, PendingChange>(aSystem, PendingChange.Add));
			}
		}

		public void RemoveSystem<T>()
		{
			ISystem system = (ISystem) GetSystem<T>();
			if (system != null)
			{
				RemoveSystem(system);
			}
		}

		public void RemoveSystem(ISystem aSystem)
		{
			if (!IsLocked)
			{
				_systems.RemoveAll(sys => sys.System == aSystem);
				aSystem.RemovedFromEngine(this);
				aSystem.Engine = null;
			}
			else
			{
				_pending.Add(new KeyValuePair<ISystem, PendingChange>(aSystem, PendingChange.Remove));
			}
		}

		public void PauseSystem<T>()
		{
			ISystem system = (ISystem) GetSystem<T>();
			if (system != null && !system.IsPaused)
			{
				system.Pause();
			}
		}

		public void ResumeSystem<T>()
		{
			ISystem system = (ISystem) GetSystem<T>();
			if (system != null && system.IsPaused)
			{
				system.Resume();
			}
		}

		public bool IsSystemPaused<T>()
		{
			ISystem system = (ISystem) GetSystem<T>();
			return (system != null) ? system.IsPaused : false;
		}

		public void PauseAllSystems()
		{
			foreach (var pair in _systems)
			{
				if (!pair.System.IsPaused)
				{
					pair.System.Pause();
				}
			}
		}

		public void ResumeAllSystems()
		{
			foreach (var pair in _systems)
			{
				if (pair.System.IsPaused)
				{
					pair.System.Resume();
				}
			}
		}

		public void ResetAllSystems()
		{
			foreach (var pair in _systems)
			{
				pair.System.Reset();
			}
		}

		public T GetSystem<T>()
		{
			foreach (var pair in _systems)
			{
				if (pair.System is T)
				{
					return (T) pair.System;
				}
			}
			return default(T);
		}

		public AntNodeList<T> GetNodes<T>()
		{
			var type = typeof(T);
			AntFamily<T> family;
			if (!_families.ContainsKey(type))
			{
				family = new AntFamily<T>();
				_families[type] = family;
				for (int i = 0, n = _entities.Count; i < n; i++)
				{
					family.EntityAdded(_entities[i]);
				}
			}
			else
			{
				family = (AntFamily<T>) _families[type];
			}

			return family.Nodes;
		}

		public void ReleaseNodes<T>(AntNodeList<T> aNodes)
		{
			var type = typeof(T);
			if (_families.ContainsKey(type))
			{
				_families.Remove(type);
			}
		}

		public void Update(float aDeltaTime)
		{
			for (int i = 0, n = _systems.Count; i < n; i++)
			{
				if (!_systems[i].System.IsPaused)
				{
					_systems[i].System.Update(aDeltaTime);
				}
			}
		}

		public void Lock()
		{
			_lockCount++;
		}

		public void Unlock()
		{
			_lockCount--;
			if (_lockCount <= 0)
			{
				_lockCount = 0;
				ApplyPending();
			}
		}

		public void ApplyPending()
		{
			KeyValuePair<ISystem, PendingChange> pair;
			for (int i = 0, n = _pending.Count; i < n; i++)
			{
				pair = _pending[i];
				if (pair.Value == PendingChange.Add)
				{
					AddSystem(pair.Key, pair.Key.Priority);
				}
				else
				{
					RemoveSystem(pair.Key);
				}
			}
			_pending.Clear();
		}

		public bool IsLocked
		{
			get { return (_lockCount > 0); }
		}
	}
}