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

					if (ai.currentSchedule == null)
					{
						// Если текущего набора действий нет, то ставим набор действий по умолчанию.
						ai.SetDefaultSchedule();
					}
					else
					{
						if (ai.currentSchedule.IsFinished(ai.logic, ai.conditions))
						{
							// Если текущий набор действий завершен или был прерван,
							// то выбираем новый набор действий и принудительно устанавливаем его.
							ai.SetSchedule(ai.logic.SelectNewSchedule(ai.conditions), true);
						}
						else
						{
							// По ходу выполнения текущего набора действий, обдумываем ситуацию
							// и меняем набор действий только если он будет отличаться от уже текущего.
							ai.SetSchedule(ai.logic.SelectNewSchedule(ai.conditions));
						}
					}

					/* Старый код обработки игровой логики.
					if (ai.currentSchedule == null)
					{
						// Если текущего набора действий нет, ставим тот что по дефолту.
						ai.SetDefaultSchedule();
					}
					else if (ai.currentSchedule.IsFinished(ai.logic, ai.conditions))
					{
						// Если текущий набор действий завершил свою работу или был прерван,
						// выбираем новый набор действий на основе текущего состояния игрового мира.
						ai.SetSchedule(ai.logic.SelectNewSchedule(ai.conditions));
					}
					//*/

					ai.currentTime = _aiNodes[i].AIControl.updateInterval;
				}

				// Обновляем текущий набор действий независимо от всего остального.
				ai.currentSchedule.Update();
			}
		}
	}
}