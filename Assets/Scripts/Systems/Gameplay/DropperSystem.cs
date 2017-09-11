using UnityEngine;
using Anthill.Core;
using Anthill.Utils;
using Game.Nodes;
using Game.Core;

namespace Game.Systems
{
	/// <summary>
	/// Данная система обрабатывает точки спавна бонусов вроде оружия, бомб, лечилок или патронов для танков.
	/// </summary>
	public class DropperSystem : ISystem, IExecuteSystem
	{
		private AntNodeList<DropperNode> _dropperNodes;
		private AntNodeList<MagnetableNode> _magnetableNodes;

		#region ISystem Implementation
		
		public void AddedToEngine(AntEngine aEngine)
		{
			_dropperNodes = aEngine.GetNodes<DropperNode>();
			_magnetableNodes = aEngine.GetNodes<MagnetableNode>();
		}

		public void RemovedFromEngine(AntEngine aEngine)
		{
			_dropperNodes = null;
			_magnetableNodes = null;
		}
		
		#endregion
		#region IExecuteSystem Implementation
		
		public void Execute()
		{
			DropperNode dropper;
			MagnetableNode item;
			float dist;
			for (int i = 0, n = _dropperNodes.Count; i < n; i++)
			{
				dropper = _dropperNodes[i];
				dropper.Dropper.IsEmpty = true;

				// Проверяем все вещи на предмет их попадания в область дроппера.
				for (int j = 0, jn = _magnetableNodes.Count; j < jn; j++)
				{
					item = _magnetableNodes[j];
					dist = AntMath.Distance(dropper.entity.Position, item.entity.Position);
					if (dist <= dropper.Dropper.observeRadius)
					{
						dropper.Dropper.IsEmpty = false;
						break;
					}
				}

				// Если дроппер пустой.
				if (dropper.Dropper.IsEmpty)
				{
					dropper.Dropper.Delay -= Time.deltaTime;
					if (dropper.Dropper.Delay < 0.0f)
					{
						dropper.Dropper.DropItem();
						dropper.Dropper.Delay = dropper.Dropper.rechargeTime;
					}
				}

				if (Config.Instance.showDropperRecharge && dropper.Dropper.IsEmpty)
				{
					AntDrawer.DrawPie(dropper.entity.Position, 0.3f, 90.0f, 0.0f, 
						(1 - (dropper.Dropper.Delay / dropper.Dropper.rechargeTime)) * 360.0f, Color.grey);
				}
			}
		}
		
		#endregion
	}
}