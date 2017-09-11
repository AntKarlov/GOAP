using UnityEngine;
using Anthill.Core;
using Anthill.Utils;
using Game.Core;
using Game.Nodes;

namespace Game.Systems
{
	/// <summary>
	/// Данная система обрабатывает респавн погибших ботов с заданным интервалом.
	/// </summary>
	public class SpawnSystem : ISystem, IExecuteSystem
	{
		private AntNodeList<SpawnerNode> _spawnerNodes;
		private AntNodeList<HealthNode> _healthNodes;

		#region ISystem Implementation
		
		public void AddedToEngine(AntEngine aEngine)
		{
			_spawnerNodes = aEngine.GetNodes<SpawnerNode>();
			_healthNodes = aEngine.GetNodes<HealthNode>();
			_healthNodes.EventNodeRemoved += OnHealthNodeRemoved;
		}

		public void RemovedFromEngine(AntEngine aEngine)
		{
			_spawnerNodes = null;
			_healthNodes.EventNodeRemoved -= OnHealthNodeRemoved;
			_healthNodes = null;
		}
		
		#endregion
		#region IExecuteSystem Implementation
		
		public void Execute()
		{
			SpawnerNode spawner;
			for (int i = 0, n = _spawnerNodes.Count; i < n; i++)
			{
				spawner = _spawnerNodes[i];
				if (spawner.Spawner.IsActive)
				{
					spawner.Spawner.Delay -= Time.deltaTime;
					if (spawner.Spawner.Delay <= 0.0f)
					{
						spawner.Spawner.Spawn();
						spawner.Spawner.Delay = spawner.Spawner.spawnDelay;
						spawner.Spawner.IsActive = false;
					}

					if (Config.Instance.showDropperRecharge && spawner.Spawner.IsActive)
					{
						AntDrawer.DrawPie(spawner.entity.Position, 0.5f, 90.0f, 0.0f, 
							(1 - (spawner.Spawner.Delay / spawner.Spawner.spawnDelay)) * 360.0f, Color.grey);
					}
				}
			}
		}
		
		#endregion
		#region Event Handlers
		
		private void OnHealthNodeRemoved(HealthNode aNode)
		{
			SpawnerNode spawner;
			for (int i = 0, n = _spawnerNodes.Count; i < n; i++)
			{
				spawner = _spawnerNodes[i];
				if (System.Object.ReferenceEquals(spawner.Spawner.entity, aNode.entity))
				{
					spawner.Spawner.entity = null;
					spawner.Spawner.IsActive = true;
				}
			}
		}
		
		#endregion
	}
}