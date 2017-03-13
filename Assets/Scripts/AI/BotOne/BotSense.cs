using UnityEngine;
using Anthill.AI;
using Anthill.Core;
using Anthill.Utils;
using Game.Components;
using Game.Nodes;

namespace Game.AI.BotOne
{
	/// <summary>
	/// В данном классе мы собираем все необходимые состояния мира и состояния самого бота.
	///
	/// Для бота может быть использовано произвольное количество органов чувств. Например,
	/// зрение, ощущения и т.п. Все эти органы желательно реализовывать отдельными компонентами.
	/// Данный класс помогает реализовать работу с отдельными органами чувств и построить «картинку мира».
	/// </summary>
	public class BotSense : ISense
	{
		public TankControl control; // Управление танком.
		public Backboard backboard; // Память танка.
		public Health health; // Здоровье.
		public Vision vision; // Зрение танка.
		//public Sense sense; // Сенсор танка.

		private AntNodeList<VisualNode> _visualNodes;

		public BotSense(GameObject aObject)
		{
			control = aObject.GetComponent<TankControl>();
			backboard = aObject.GetComponent<Backboard>();
			health = aObject.GetComponent<Health>();
			vision = aObject.GetComponent<Vision>();
			//sense = aObject.GetComponent<Sense>(); // todo
		}

		public void GetConditions(ILogic aLogic, AntAICondition aConditions)
		{
			// Получаем список всех объектов которые необходимо отслеживать.
			if (_visualNodes == null)
			{
				_visualNodes = AntEngine.Current.GetNodes<VisualNode>();
			}

			// 1. Обновляем информацию о себе.
			// -------------------------------
			aConditions.Set(aLogic.Planner, "ArmedWithGun", control.Tower.HasGun);
			aConditions.Set(aLogic.Planner, "ArmedWithBomb", control.Tower.HasBomb);
			aConditions.Set(aLogic.Planner, "HasAmmo", control.Tower.HasAmmo);
			aConditions.Set(aLogic.Planner, "Injured", (health.HP != health.maxHP));
			aConditions.Set(aLogic.Planner, "EnemyAlive", true); // todo
			aConditions.Set(aLogic.Planner, "Alive", (health.HP > 0.0f));

			// 2. Обрабатываем зрение.
			// -----------------------
			// Сбрасываем состояние всех возможных значений зрения.
			aConditions.Set(aLogic.Planner, "NearEnemy", false);
			aConditions.Set(aLogic.Planner, "EnemyVisible", false);
			aConditions.Set(aLogic.Planner, "GunInlineOfSight", false);
			aConditions.Set(aLogic.Planner, "AmmoInlineOfSight", false);
			aConditions.Set(aLogic.Planner, "BombInlineOfSight", false);
			aConditions.Set(aLogic.Planner, "HealInlineOfSight", false);

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
						// Записываем его в память.
						backboard.AddData(new BackboardData() {
							conditionName = visual.Visual.conditionName,
							position = visual.entity.Position,
							isValid = true
						});

						// Отмечаем что видим.
						aConditions.Set(aLogic.Planner, visual.Visual.conditionName, true);

						if (vision.enemyGroup == visual.Visual.group &&
							AntMath.Distance(control.Position, visual.entity.Position) < 1.5f)
						{
							aConditions.Set(aLogic.Planner, "NearEnemy", true);
						}
					}
				}
			}

			// 3. Обрабатываем наведение пушки на врага.
			// -----------------------------------------
			aConditions.Set(aLogic.Planner, "EnemyLinedUp", false);
			if (aConditions.Has(aLogic.Planner, "EnemyVisible") && control.Tower.HasGun)
			{
				BackboardData data = backboard.Find("EnemyVisible");
				if (data.isValid)
				{
					float angle = AntMath.AngleDeg((Vector2) control.Position, data.position);
					if (AntMath.Equal(AntMath.Angle(control.Tower.Angle), AntMath.Angle(angle), 10.0f))
					{
						aConditions.Set(aLogic.Planner, "EnemyLinedUp", true);
					}
				}	
			}
		}
	}
}