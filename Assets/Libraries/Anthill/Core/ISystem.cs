namespace Anthill.Core
{
	public interface ISystem
	{
		void AddedToEngine(AntEngine aEngine);
		void RemovedFromEngine(AntEngine aEngine);
		void Reset();
		void Update(float aDeltaTime);
		void Pause();
		void Resume();
		AntEngine Engine { get; set; }
		int Priority { get; set; }
		bool IsPaused { get; }
	}
}