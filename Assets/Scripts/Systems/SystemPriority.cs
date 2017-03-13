namespace Game.Systems
{
	/// <summary>
	/// Приоритет игровых систем.
	/// </summary>
	public class SystemPriority
	{
		public static int movementSystem = 0;
		public static int playerControlSystem = 1;
		public static int aiControlSystem = 2;
		public static int magnetSystem = 3;
		public static int healthSystem = 4;
	}
}