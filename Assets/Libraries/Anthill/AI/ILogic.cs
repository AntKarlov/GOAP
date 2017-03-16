namespace Anthill.AI
{
	public interface ILogic
	{
		string SelectNewTask(AntAICondition aCondition);
		AntAIPlan CurrentPlan { get; }
		AntAIPlanner Planner { get; }
	}
}