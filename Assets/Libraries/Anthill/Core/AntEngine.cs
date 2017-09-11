using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Anthill.Core
{
	public class AntEngine : AntScenario
	{
		public static AntEngine Current { get; private set; }

		private Dictionary<Type, IFamily> _families;
		private List<AntEntity> _entities;
		private List<DelayedCall> _delayedCalls;

		#region Public Methods

		public AntEngine() : base()
		{
			Current = this;
			_families = new Dictionary<Type, IFamily>();
			_entities = new List<AntEntity>();
			_delayedCalls = new List<DelayedCall>();
			_engine = this;
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

		public void AddEntity(GameObject aGameObject, bool aIncludeChildren = false)
		{
			AddEntity(aGameObject.transform, aIncludeChildren);
		}

		public void AddEntity(Transform aTransform, bool aIncludeChildren = false)
		{
			AntEntity entity = aTransform.GetComponent<AntEntity>();
			if (entity != null)
			{
				AddEntity(entity);
				if (aIncludeChildren && aTransform.childCount > 0)
				{
					for (int i = 0, n = aTransform.childCount; i < n; i++)
					{
						AddEntity(aTransform.GetChild(i), aIncludeChildren);
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

		public override void Execute()
		{
			base.Execute();
			for (int i = _delayedCalls.Count - 1; i >= 0; i--)
			{
				if (_delayedCalls[i].Update(Time.deltaTime))
				{
					_delayedCalls.RemoveAt(i);
				}
			}
		}

		public void DelayedCall(float aDelay, Action aFunc)
		{
			var call = new DelayedCall();
			call.SetProcess(aFunc);
			call.delay = aDelay;
			_delayedCalls.Add(call);
		}

		public void DelayedCall<T1>(float aDelay, Action<T1> aFunc, T1 aArg1)
		{
			var call = new DelayedCall<T1>();
			call.SetProcess(aFunc);
			call.SetArgumens(aArg1);
			call.delay = aDelay;
			_delayedCalls.Add(call);
		}

		public void DelayedCall<T1, T2>(float aDelay, Action<T1, T2> aFunc, T1 aArg1, T2 aArg2)
		{
			var call = new DelayedCall<T1, T2>();
			call.SetProcess(aFunc);
			call.SetArgumens(aArg1, aArg2);
			call.delay = aDelay;
			_delayedCalls.Add(call);
		}

		public void DelayedCall<T1, T2, T3>(float aDelay, Action<T1, T2, T3> aFunc, T1 aArg1, T2 aArg2, T3 aArg3)
		{
			var call = new DelayedCall<T1, T2, T3>();
			call.SetProcess(aFunc);
			call.SetArgumens(aArg1, aArg2, aArg3);
			call.delay = aDelay;
			_delayedCalls.Add(call);
		}

		public void DelayedCall<T1, T2, T3, T4>(float aDelay, Action<T1, T2, T3, T4> aFunc, T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4)
		{
			var call = new DelayedCall<T1, T2, T3, T4>();
			call.SetProcess(aFunc);
			call.SetArgumens(aArg1, aArg2, aArg3, aArg4);
			call.delay = aDelay;
			_delayedCalls.Add(call);
		}

		#endregion
		#region Event Handlers

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

		#endregion
	}
}