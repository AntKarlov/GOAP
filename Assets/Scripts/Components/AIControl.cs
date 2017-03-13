using System;
using UnityEngine;
using Anthill.AI;

namespace Game.Components
{
	public class AIControl : MonoBehaviour
	{
		[Tooltip("Вкл. вывод плана составляемого ботом в консоль.")]
		public bool outputPlan = false;

		[NonSerializedAttribute] public ISense sense;
		[NonSerializedAttribute] public ILogic logic;
		[NonSerializedAttribute] public AntAISchedule[] schedules;
		[NonSerializedAttribute] public AntAISchedule currentSchedule;
		[NonSerializedAttribute] public AntAICondition conditions;
		[NonSerializedAttribute] public float updateInterval;
		[NonSerializedAttribute] public float currentTime;
		[NonSerializedAttribute] public bool active;

		// -----------------------------------------------------
		// Unity Callbacks
		// -----------------------------------------------------

		private void Awake()
		{
			conditions = new AntAICondition();
			currentSchedule = null;
			currentTime = 0.0f;
			active = true;
		}

		// -----------------------------------------------------
		// Public Methods
		// -----------------------------------------------------

		public void SetDefaultSchedule()
		{
			if (currentSchedule != null)
			{
				currentSchedule.Stop(gameObject);
			}

			for (int i = 0, n = schedules.Length; i < n; i++)
			{
				if (schedules[i].isDefault)
				{
					currentSchedule = schedules[i];
					currentSchedule.Reset();
					currentSchedule.Start(gameObject);
				}
			}
		}
		
		public void SetSchedule(string aScheduleName, bool aForce = false)
		{
			if (aForce || !string.Equals(currentSchedule.name, aScheduleName))
			{
				if (currentSchedule != null)
				{
					currentSchedule.Stop(gameObject);
				}

				currentSchedule = FindSchedule(aScheduleName);
				if (currentSchedule != null)
				{
					currentSchedule.Reset();
					currentSchedule.Start(gameObject);
				}
				else
				{
					Debug.LogWarning("Can't find \"" + aScheduleName + "\" schedule.");
					SetDefaultSchedule();
				}
			}
		}

		public AntAISchedule FindSchedule(string aScheduleName)
		{
			int index = Array.FindIndex(schedules, x => string.Equals(x.name, aScheduleName));
			return (index >= 0 && index < schedules.Length) ? schedules[index] : null;
		}
	}
}