namespace Anthill.AI
{
	public interface ILogic
	{
		string SelectNewSchedule(AntAICondition aCondition, bool aForce = false);
		AntAIPlanner Planner { get; }
	}
}