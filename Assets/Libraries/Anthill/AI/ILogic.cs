namespace Anthill.AI
{
	public interface ILogic
	{
		string SelectNewTask(AntAICondition aCondition);
		AntAIPlanner Planner { get; }
		AntAICondition CurrentGoal { get; }
		AntAIPlan CurrentPlan { get; }
	}
}