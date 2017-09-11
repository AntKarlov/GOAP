namespace Anthill.Core
{
#if UNITY_EDITOR && !ANTHILL_DISABLE_DEBUG
	public class AntScenario : AntDebugScenario
	{
#else
	public class AntScenario : AntBaseScenario
	{
#endif
		public AntScenario(string aName = "Systems") : base(aName)
		{
			//..
		}
	}
}