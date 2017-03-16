using System;
using UnityEngine;

namespace Anthill.AI
{
	[CreateAssetMenuAttribute(fileName = "Scenario", menuName = "New AI Scenario", order = 1)]
	public class AntAIScenario : ScriptableObject
	{
		public AntAIScenarioCondition condition;
		[HideInInspector] public AntAIScenarioAction[] actions = new AntAIScenarioAction[0];
		[HideInInspector] public AntAIScenarioActionItem[] goals = new AntAIScenarioActionItem[0];
	}

	[Serializable]
	public class AntAIScenarioAction
	{
		public string name;
		public string task;
		public int cost;
		public bool showSettings;
		public AntAIScenarioActionItem[] preConditions;
		public AntAIScenarioActionItem[] postConditions;

		public AntAIScenarioAction()
		{
			name = "<Unnamed>";
			task = name;
			cost = 0;
			showSettings = true;
			preConditions = new AntAIScenarioActionItem[0];
			postConditions = new AntAIScenarioActionItem[0];
		}
	}

	[Serializable]
	public struct AntAIScenarioActionItem
	{
		public int id;
		public bool value;
	}

	[Serializable]
	public class AntAIScenarioCondition
	{
		public AntAIScenarioConditionItem[] list = new AntAIScenarioConditionItem[0];
		public int serialId = -1;

		public AntAIScenarioCondition Clone()
		{
			var clone = new AntAIScenarioCondition();
			clone.list = new AntAIScenarioConditionItem[list.Length];
			clone.serialId = serialId;
			for (int i = 0, n = list.Length; i < n; i++)
			{
				clone.list[i] = list[i].Clone();
			}
			return clone;
		}

		public string this[int aIndex]
		{
			get { return (aIndex >= 0 && aIndex < list.Length) ? list[aIndex].name : null; }
		}
	}

	[Serializable]
	public class AntAIScenarioConditionItem
	{
		public int id;
		public string name;

		public AntAIScenarioConditionItem Clone()
		{
			return new AntAIScenarioConditionItem() {
				id = this.id,
				name = this.name
			};
		}
	}
}