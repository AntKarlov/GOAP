namespace Anthill.AI
{
	public interface ILogic
	{
		string SelectNewState(AntAICondition aCondition);
		AntAIPlanner Planner { get; }
		AntAICondition CurrentGoal { get; }
		AntAIPlan CurrentPlan { get; }
	}
}