namespace Anthill.AI
{
	public interface IAIProvider
	{
		string Name { get; }
		AntAIState[] States { get; }
		AntAIState DefaultState { get; }
		ILogic Logic { get; }
		ISense Sense { get; }
		//Blackboard Blackboard { get; } // todo
	}
}