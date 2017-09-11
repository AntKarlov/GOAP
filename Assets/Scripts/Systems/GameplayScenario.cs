using Anthill.Core;

namespace Game.Systems
{
	/// <summary>
	/// Данный класс представляет собой некоторый набор систем которые решают определенную задачу.
	/// Например в данном случае сценарий реализует работу геймплея. По мимо геймплейного сценария
	/// могут быть любые другие сценарии, например для обработки игровых меню, загрузки уровней и т.п.
	///
	/// В игре может работать одновременно произвольное количество разных сценариев, так же по мере
	/// необходимости можно ставить на паузу одни сценарии и активировать другие.
	/// </summary>
	public class GameplayScenario : AntScenario
	{
		public GameplayScenario() : base("GameplayScenario")
		{
			// ..
		}

		public override void AddedToEngine(AntEngine aEngine)
		{
			base.AddedToEngine(aEngine);
			Add<AIControlSystem>();
			Add<DropperSystem>();
			Add<HealthSystem>();
			Add<MagnetSystem>();
			Add<MovementSystem>();
			Add<PlayerControlSystem>();
			Add<SpawnSystem>();
		}

		public override void RemovedFromEngine(AntEngine aEngine)
		{
			Remove<AIControlSystem>();
			Remove<DropperSystem>();
			Remove<HealthSystem>();
			Remove<MagnetSystem>();
			Remove<MovementSystem>();
			Remove<PlayerControlSystem>();
			Remove<SpawnSystem>();
			base.RemovedFromEngine(aEngine);
		}
	}
}