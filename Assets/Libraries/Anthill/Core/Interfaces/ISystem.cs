namespace Anthill.Core
{
	public interface ISystem
	{
		void AddedToEngine(AntEngine aEngine);
		void RemovedFromEngine(AntEngine aEngine);
	}
}