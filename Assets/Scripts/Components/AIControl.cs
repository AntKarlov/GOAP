using System;
using UnityEngine;
using Anthill.AI;

namespace Game.Components
{
	public class AIControl : MonoBehaviour, IAgentProvider
	{
		[NonSerialized] public float currentTime;
		[NonSerialized] public float updateInterval;

		#region Unity Callbacks

		private void Awake()
		{
			currentTime = 0.0f;
		}

		#endregion
		#region IAgentProvider Implementation

		public AntAIAgent Agent { get; set; }

		#endregion
	}
}