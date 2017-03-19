using System;
using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

namespace Game.Components
{
	public class AIControl : MonoBehaviour, IAIProvider
	{
		[NonSerializedAttribute] public ISense sense;
		[NonSerializedAttribute] public ILogic logic;
		[NonSerializedAttribute] public AntAIState[] states;
		[NonSerializedAttribute] public AntAIState currentState;
		[NonSerializedAttribute] public AntAIState defaultState;
		[NonSerializedAttribute] public AntAICondition conditions;
		[NonSerializedAttribute] public float updateInterval;
		[NonSerializedAttribute] public float currentTime;
		[NonSerializedAttribute] public bool active;

		#region Unity Callbacks

		private void Awake()
		{
			conditions = new AntAICondition();
			currentState = null;
			currentTime = 0.0f;
			active = true;
		}

		#endregion
		#region Public Methods

		public void DefaultStateIs(string aStateName)
		{
			defaultState = FindState(aStateName);
			if (defaultState == null)
			{
				AntLog.Report("[AIControl]", "Can't set \"{0}\" as default State because it is not existing!", aStateName);
			}
		}

		public void SetDefaultState()
		{
			if (currentState != null)
			{
				currentState.Stop();
			}

			if (defaultState != null)
			{
				currentState = defaultState;
				currentState.Reset();
				currentState.Start();
			}
			else
			{
				AntLog.Report("[AIControl]", "Default State is not defined!");
			}
		}
		
		public void SetState(string aStateName, bool aForce = false)
		{
			if (aForce || !string.Equals(currentState.name, aStateName))
			{
				if (currentState != null)
				{
					currentState.Stop();
				}

				currentState = FindState(aStateName);
				if (currentState != null)
				{
					currentState.Reset();
					currentState.Start();
				}
				else
				{
					AntLog.Report("[AIControl]", "Can't find \"{0}\" state.", aStateName);
					SetDefaultState();
				}
			}
		}

		public AntAIState FindState(string aStateName)
		{
			int index = Array.FindIndex(states, x => string.Equals(x.name, aStateName));
			return (index >= 0 && index < states.Length) ? states[index] : null;
		}

		#endregion
		#region IAIProvider Implementation

		public string Name
		{
			get { return gameObject.name; }
		}

		public AntAIState[] States
		{
			get { return states; }
		}

		public AntAIState DefaultState
		{
			get { return defaultState; }
		}

		public ILogic Logic
		{
			get { return logic; }
		}

		public ISense Sense
		{
			get { return sense; }
		}

		#endregion
	}
}