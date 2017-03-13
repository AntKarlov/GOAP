using UnityEngine;
using Anthill.AI;
using Anthill.Utils;
using Game.Components;

namespace Game.AI.BotOne
{
	/// <summary>
	/// Набор действий для прицеливания.
	/// </summary>
	public class ScheduleAim : AntAISchedule
	{
		private TankControl _control;
		private Backboard _backboard;
		private float _targetAngle;

		public ScheduleAim() : base("Aim")
		{
			AddTask(OnFindTarget);
			AddTask(OnAim);
		}

		public override void Start(GameObject aObject)
		{
			_control = aObject.GetComponent<TankControl>();
			_backboard = aObject.GetComponent<Backboard>();
		}

		public override void Stop(GameObject aObject)
		{
			_control.isTowerLeft = false;
			_control.isTowerRight = false;
		}

		private bool OnFindTarget()
		{
			_targetAngle = 0.0f;

			// Считываем из памяти информацию о положении врага.
			BackboardData data = _backboard.Find("EnemyVisible");
			if (data.isValid)
			{
				_targetAngle = AntMath.AngleDeg((Vector2) _control.Position, data.position);
			}

			return true;
		}

		private bool OnAim()
		{
			// Процесс наведения на цель.
			if (!AntMath.Equal(AntMath.Angle(_control.Tower.Angle), AntMath.Angle(_targetAngle), 1.0f))
			{
				float curAng = AntMath.Angle(_control.Tower.Angle);
				float tarAng = AntMath.Angle(_targetAngle);
				if (Mathf.Abs(curAng - tarAng) > 180.0f)
				{
					if (curAng > tarAng)
					{
						tarAng += 360.0f;
					}
					else
					{
						tarAng -= 360.0f;
					}
				}

				if (curAng < tarAng)
				{
					_control.isTowerLeft = true;
					_control.isTowerRight = false;
				}
				else if (curAng > tarAng)
				{
					_control.isTowerLeft = false;
					_control.isTowerRight = true;
				}
			}
			else
			{
				_control.isTowerLeft = false;
				_control.isTowerRight = false;
			}
			return false;
		}
	}
}