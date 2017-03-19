using Anthill.Core;
using Game.Components;
using Game.Nodes;

namespace Game.Systems
{
	/// <summary>
	/// Данная система отвечает за работу Ai (мозгов) для ботов.
	/// Система занимается обновлением состояний, обновлением текущего
	/// набора действий и выбором нового набора действий.
	/// </summary>
	public class AIControlSystem : AntSystem
	{
		private AntNodeList<AIControlNode> _aiNodes;

		public override void AddedToEngine(AntEngine aEngine)
		{
			_aiNodes = aEngine.GetNodes<AIControlNode>();
		}

		public override void RemovedFromEngine(AntEngine aEngine)
		{
			_aiNodes = null;
		}

		public override void Update(float aDeltaTime)
		{
			AIControl ai;
			for (int i = 0, n = _aiNodes.Count; i < n; i++)
			{
				ai = _aiNodes[i].AIControl;
				ai.currentTime -= aDeltaTime;
				if (ai.currentTime <= 0.0f)
				{
					if (ai.active)
					{
						// Собираем информацию о текущем состоянии игрового мира.
						ai.sense.GetConditions(ai.logic, ai.conditions);
					}

					if (ai.currentState == null)
					{
						// Если текущей задачи нет, то ставим задачу по умолчанию.
						ai.SetDefaultState();
					}
					else
					{
						if (ai.currentState.IsFinished(ai.logic, ai.conditions))
						{
							// Если текущая задача завершена или была прервана,
							// то выбираем новую задачу и принудительно устанавливаем её.
							ai.SetState(ai.logic.SelectNewState(ai.conditions), true);
						}
						else
						{
							// По ходу выполнения текущей задачи, обдумываем ситуацию
							// и меняем задачу если она отличается от текущей.
							ai.SetState(ai.logic.SelectNewState(ai.conditions));
						}
					}

					ai.currentTime = ai.updateInterval;
				}

				// Обновляем текущуюу задачу независимо от всего остального.
				ai.currentState.Update(aDeltaTime);
			}
		}
	}
}