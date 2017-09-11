using UnityEngine;

namespace Anthill.Core
{
	public class AntDebugScenarioBehaviour : MonoBehaviour
	{
		public AntDebugScenario Scenario { get; private set; }

		public void Init(AntDebugScenario aScenario)
		{
			Scenario = aScenario;
		}
	}
}