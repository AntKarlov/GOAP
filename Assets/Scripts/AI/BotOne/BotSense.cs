using UnityEngine;
using Anthill.AI;
using Anthill.Core;
using Anthill.Utils;
using Game.Components;
using Game.Nodes;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Данный класс реализует органы чувств для бота: ощущения и зрение.
	/// На основе органов чувств мы обновляем "картину мира" (WORLD STATE).
	/// </summary>
	public class BotSense : ISense
	{
		public TankControl control;        // Управление танком.
		public AntAIBlackboard blackboard; // Память танка.
		public Health health;              // Здоровье.
		public Vision vision;              // Зрение танка.
		public Sensor sensor;              // Сенсор танка.

		private AntNodeList<VisualNode> _visualNodes;

		public BotSense(GameObject aObject)
		{
			control = aObject.GetComponent<TankControl>();
			blackboard = aObject.GetComponent<AntAIBlackboard>();
			health = aObject.GetComponent<Health>();
			vision = aObject.GetComponent<Vision>();
			sensor = aObject.GetComponent<Sensor>();
		}

		public void GetConditions(AntAIAgent aAgent, AntAICondition aWorldState)
		{
			// Получаем список всех объектов которые необходимо отслеживать.
			if (_visualNodes == null)
			{
				_visualNodes = AntEngine.Current.GetNodes<VisualNode>();
			}

			// 1. Обновляем информацию о себе.
			// -------------------------------
			aWorldState.Set(aAgent.planner, "ArmedWithGun", control.Tower.HasGun);
			aWorldState.Set(aAgent.planner, "ArmedWithBomb", control.Tower.HasBomb);
			aWorldState.Set(aAgent.planner, "HasAmmo", control.Tower.HasAmmo);
			aWorldState.Set(aAgent.planner, "Injured", (health.HP != health.maxHP));
			aWorldState.Set(aAgent.planner, "EnemyAlive", true); // Наш враг всегда жив, потому что респавнится.
			aWorldState.Set(aAgent.planner, "Alive", (health.HP > 0.0f));
			aWorldState.Set(aAgent.planner, "HasObstacle", sensor.HasObstacle);

			if (sensor.HasObstacle)
			{
				sensor.HasObstacle = false;
			}

			// 2. Обрабатываем зрение.
			// -----------------------
			// Сбрасываем состояние всех возможных значений зрения.
			aWorldState.Set(aAgent.planner, "NearEnemy", false);
			aWorldState.Set(aAgent.planner, "EnemyVisible", false);
			aWorldState.Set(aAgent.planner, "GunInlineOfSight", false);
			aWorldState.Set(aAgent.planner, "AmmoInlineOfSight", false);
			aWorldState.Set(aAgent.planner, "BombInlineOfSight", false);
			aWorldState.Set(aAgent.planner, "HealInlineOfSight", false);

			VisualNode visual;
			// Перебираем все объекты которые можно просматривать.
			for (int i = 0, n = _visualNodes.Count; i < n; i++)
			{
				visual = _visualNodes[i];
				// Если объект из другой группы.
				if (vision.group != visual.Visual.group)
				{
					// Если объект попадает в область зрения.
					if (vision.IsSee(visual.entity.Position))
					{
						blackboard[visual.Visual.conditionName].AsBool = true;
						blackboard[string.Concat(visual.Visual.conditionName, "_Pos")].AsVector2 = visual.entity.Position;

						// Отмечаем что видим.
						aWorldState.Set(aAgent.planner, visual.Visual.conditionName, true);

						if (vision.enemyGroup == visual.Visual.group &&
							AntMath.Distance(control.Position, visual.entity.Position) < 1.5f)
						{
							aWorldState.Set(aAgent.planner, "NearEnemy", true);
						}
					}
				}
			}

			// 3. Обрабатываем наведение пушки на врага.
			// -----------------------------------------
			aWorldState.Set(aAgent.planner, "EnemyLinedUp", false);
			if (aWorldState.Has(aAgent.planner, "EnemyVisible") && control.Tower.HasGun)
			{
				if (blackboard["EnemyVisible"].AsBool)
				{
					float angle = AntMath.AngleDeg((Vector2) control.Position, blackboard["EnemyVisible_Pos"].AsVector2);
					if (AntMath.Equal(AntMath.Angle(control.Tower.Angle), AntMath.Angle(angle), 10.0f))
					{
						aWorldState.Set(aAgent.planner, "EnemyLinedUp", true);
					}
				}
			}
		}
	}
}