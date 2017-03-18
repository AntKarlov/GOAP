using System;
using UnityEngine;
using Anthill.AI;

namespace Game.Components
{
	public class AIControl : MonoBehaviour, IAIProvider
	{
		[NonSerializedAttribute] public ISense sense;
		[NonSerializedAttribute] public ILogic logic;
		[NonSerializedAttribute] public AntAITask[] tasks;
		[NonSerializedAttribute] public AntAITask currentTask;
		[NonSerializedAttribute] public AntAITask defaultTask;
		[NonSerializedAttribute] public AntAICondition conditions;
		[NonSerializedAttribute] public float updateInterval;
		[NonSerializedAttribute] public float currentTime;
		[NonSerializedAttribute] public bool active;

		#region Unity Callbacks

		private void Awake()
		{
			conditions = new AntAICondition();
			currentTask = null;
			currentTime = 0.0f;
			active = true;
		}

		#endregion
		#region Public Methods

		public void DefaultTaskIs(string aTaskName)
		{
			defaultTask = FindTask(aTaskName);
			if (defaultTask == null)
			{
				Debug.LogWarning("[AIControl] Can't set \"" + aTaskName + "\" as default task because it is not existing!");
			}
		}

		public void SetDefaultTask()
		{
			if (currentTask != null)
			{
				currentTask.Stop();
			}

			if (defaultTask != null)
			{
				currentTask = defaultTask;
				currentTask.Reset();
				currentTask.Start();
			}
			else
			{
				Debug.LogWarning("[AIControl] Default task is not defined!");
			}
		}
		
		public void SetTask(string aTaskName, bool aForce = false)
		{
			if (aForce || !string.Equals(currentTask.name, aTaskName))
			{
				if (currentTask != null)
				{
					currentTask.Stop();
				}

				currentTask = FindTask(aTaskName);
				if (currentTask != null)
				{
					currentTask.Reset();
					currentTask.Start();
				}
				else
				{
					Debug.LogWarning("[AIControl] Can't find \"" + aTaskName + "\" task.");
					SetDefaultTask();
				}
			}
		}

		public AntAITask FindTask(string aScheduleName)
		{
			int index = Array.FindIndex(tasks, x => string.Equals(x.name, aScheduleName));
			return (index >= 0 && index < tasks.Length) ? tasks[index] : null;
		}

		#endregion
		#region IAIProvider Implementation

		public string Name
		{
			get { return gameObject.name; }
		}

		public AntAITask[] Tasks
		{
			get { return tasks; }
		}

		public AntAITask DefaultTask
		{
			get { return defaultTask; }
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