using System;
using UnityEngine;
using Anthill.AI;

namespace Game.Components
{
	public class AIControl : MonoBehaviour, IAgentProvider
	{
		[NonSerializedAttribute] public float currentTime;
		[NonSerializedAttribute] public float updateInterval;

		#region Unity Callbacks

		private void Awake()
		{
			currentTime = 0.0f;
		}

		#endregion
		#region Public Methods

		// ..

		#endregion
		#region Getters/Setters

		public AntAIAgent Agent { get; set; }

		#endregion
	}
}