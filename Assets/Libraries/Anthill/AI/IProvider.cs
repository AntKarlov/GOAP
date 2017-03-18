namespace Anthill.AI
{
	public interface IAIProvider
	{
		string Name { get; }
		AntAITask[] Tasks { get; }
		AntAITask DefaultTask { get; }
		ILogic Logic { get; }
		ISense Sense { get; }
		//Blackboard Blackboard { get; } // todo
	}
}