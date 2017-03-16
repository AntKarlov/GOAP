namespace Anthill.AI
{
	public interface IAIProvider
	{
		AntAITask[] Tasks { get; }
		AntAITask DefaultTask { get; }
		ILogic Logic { get; }
		ISense Sense { get; }
		//Blackboard Blackboard { get; } // todo
	}
}