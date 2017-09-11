using System;
using UnityEngine;

namespace Anthill.Core
{
	public class AntEntity : MonoBehaviour
	{
		public delegate void ComponentChangeDelegate(AntEntity aEntity, Type aComponentType);
		public event ComponentChangeDelegate EventComponentAdded;
		public event ComponentChangeDelegate EventComponentRemoved;

		public delegate void EntityDelegate(AntEntity aEntity);
		public event EntityDelegate EventEntityAddedToEngine;
		public event EntityDelegate EventEntityRemovedFromEngine;

		protected Transform _transform;

		#region Unity Calls

		private void Awake()
		{
			_transform = GetComponent<Transform>();
		}

		#endregion
		#region Public Methods

		public bool Has(Type aType)
		{
			return gameObject.GetComponent(aType) != null;
		}

		public object Get(Type aType)
		{
			return gameObject.GetComponent(aType);
		}

		public T Add<T>() where T : Component
		{
			var comp = gameObject.AddComponent<T>();
			if (EventComponentAdded != null)
			{
				EventComponentAdded(this, typeof(T));
			}
			return comp;
		}

		public void Remove(Component aComponent)
		{
			DestroyComponent(aComponent);
			if (EventComponentRemoved != null)
			{
				EventComponentRemoved(this, aComponent.GetType());
			}
		}

		#endregion
		#region Protected Methods

		internal void OnAddedToEngine()
		{
			if (EventEntityAddedToEngine != null)
			{
				EventEntityAddedToEngine(this);
			}
		}

		internal void OnRemovedFromEngine()
		{
			if (EventEntityRemovedFromEngine != null)
			{
				EventEntityRemovedFromEngine(this);
			}
		}

		protected virtual void DestroyComponent(Component aComponent)
		{
			Destroy(aComponent);
		}

		#endregion
		#region Getters Setters

		public Transform Transform
		{
			get { return _transform; }
		}

		public Vector2 Position
		{
			get { return new Vector2(_transform.position.x, _transform.position.y); }
			set { _transform.position = value; }
		}

		public Vector2 Scale
		{
			get { return new Vector2(_transform.localScale.x, _transform.localScale.y); }
			set { _transform.localScale = value; }
		}

		public Vector3 Position3D
		{
			get { return _transform.position; }
			set { _transform.position = value; }
		}

		public float Angle 
		{
			get { return _transform.rotation.eulerAngles.z; }
			set { _transform.rotation = Quaternion.Euler(_transform.rotation.x, _transform.rotation.y, value); }
		}

		#endregion
	}
}