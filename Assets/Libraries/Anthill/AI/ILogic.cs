namespace Anthill.AI
{
	public interface ILogic
	{
		string SelectNewSchedule(AntAICondition aCondition);
		AntAIPlanner Planner { get; }
	}
}