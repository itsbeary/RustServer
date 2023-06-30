using System;
using ConVar;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B52 RID: 2898
	public class AiLocationSpawner : SpawnGroup
	{
		// Token: 0x0600461F RID: 17951 RVA: 0x00198AB4 File Offset: 0x00196CB4
		public override void SpawnInitial()
		{
			if (this.IsMainSpawner)
			{
				if (this.Location == AiLocationSpawner.SquadSpawnerLocation.MilitaryTunnels)
				{
					this.maxPopulation = AI.npc_max_population_military_tunnels;
					this.numToSpawnPerTickMax = AI.npc_spawn_per_tick_max_military_tunnels;
					this.numToSpawnPerTickMin = AI.npc_spawn_per_tick_min_military_tunnels;
					this.respawnDelayMax = AI.npc_respawn_delay_max_military_tunnels;
					this.respawnDelayMin = AI.npc_respawn_delay_min_military_tunnels;
				}
				else
				{
					this.defaultMaxPopulation = this.maxPopulation;
					this.defaultNumToSpawnPerTickMax = this.numToSpawnPerTickMax;
					this.defaultNumToSpawnPerTickMin = this.numToSpawnPerTickMin;
				}
			}
			else
			{
				this.defaultMaxPopulation = this.maxPopulation;
				this.defaultNumToSpawnPerTickMax = this.numToSpawnPerTickMax;
				this.defaultNumToSpawnPerTickMin = this.numToSpawnPerTickMin;
			}
			base.SpawnInitial();
		}

		// Token: 0x06004620 RID: 17952 RVA: 0x00198B5C File Offset: 0x00196D5C
		protected override void Spawn(int numToSpawn)
		{
			if (!AI.npc_enable)
			{
				this.maxPopulation = 0;
				this.numToSpawnPerTickMax = 0;
				this.numToSpawnPerTickMin = 0;
				return;
			}
			if (numToSpawn == 0)
			{
				if (this.IsMainSpawner)
				{
					if (this.Location == AiLocationSpawner.SquadSpawnerLocation.MilitaryTunnels)
					{
						this.maxPopulation = AI.npc_max_population_military_tunnels;
						this.numToSpawnPerTickMax = AI.npc_spawn_per_tick_max_military_tunnels;
						this.numToSpawnPerTickMin = AI.npc_spawn_per_tick_min_military_tunnels;
						numToSpawn = UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
					}
					else
					{
						this.maxPopulation = this.defaultMaxPopulation;
						this.numToSpawnPerTickMax = this.defaultNumToSpawnPerTickMax;
						this.numToSpawnPerTickMin = this.defaultNumToSpawnPerTickMin;
						numToSpawn = UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
					}
				}
				else
				{
					this.maxPopulation = this.defaultMaxPopulation;
					this.numToSpawnPerTickMax = this.defaultNumToSpawnPerTickMax;
					this.numToSpawnPerTickMin = this.defaultNumToSpawnPerTickMin;
					numToSpawn = UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
				}
			}
			float num = this.chance;
			AiLocationSpawner.SquadSpawnerLocation location = this.Location;
			if (location != AiLocationSpawner.SquadSpawnerLocation.JunkpileA)
			{
				if (location == AiLocationSpawner.SquadSpawnerLocation.JunkpileG)
				{
					num = AI.npc_junkpile_g_spawn_chance;
				}
			}
			else
			{
				num = AI.npc_junkpile_a_spawn_chance;
			}
			if (numToSpawn == 0 || UnityEngine.Random.value > num)
			{
				return;
			}
			numToSpawn = Mathf.Min(numToSpawn, this.maxPopulation - base.currentPopulation);
			for (int i = 0; i < numToSpawn; i++)
			{
				GameObjectRef prefab = base.GetPrefab();
				Vector3 vector;
				Quaternion quaternion;
				BaseSpawnPoint spawnPoint = this.GetSpawnPoint(prefab, out vector, out quaternion);
				if (spawnPoint)
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(prefab.resourcePath, vector, quaternion, true);
					if (baseEntity)
					{
						baseEntity.Spawn();
						SpawnPointInstance spawnPointInstance = baseEntity.gameObject.AddComponent<SpawnPointInstance>();
						spawnPointInstance.parentSpawnPointUser = this;
						spawnPointInstance.parentSpawnPoint = spawnPoint;
						spawnPointInstance.Notify();
					}
				}
			}
		}

		// Token: 0x06004621 RID: 17953 RVA: 0x00198D03 File Offset: 0x00196F03
		protected override BaseSpawnPoint GetSpawnPoint(GameObjectRef prefabRef, out Vector3 pos, out Quaternion rot)
		{
			return base.GetSpawnPoint(prefabRef, out pos, out rot);
		}

		// Token: 0x04003F12 RID: 16146
		public AiLocationSpawner.SquadSpawnerLocation Location;

		// Token: 0x04003F13 RID: 16147
		public AiLocationManager Manager;

		// Token: 0x04003F14 RID: 16148
		public JunkPile Junkpile;

		// Token: 0x04003F15 RID: 16149
		public bool IsMainSpawner = true;

		// Token: 0x04003F16 RID: 16150
		public float chance = 1f;

		// Token: 0x04003F17 RID: 16151
		private int defaultMaxPopulation;

		// Token: 0x04003F18 RID: 16152
		private int defaultNumToSpawnPerTickMax;

		// Token: 0x04003F19 RID: 16153
		private int defaultNumToSpawnPerTickMin;

		// Token: 0x02000FB2 RID: 4018
		public enum SquadSpawnerLocation
		{
			// Token: 0x0400511C RID: 20764
			MilitaryTunnels,
			// Token: 0x0400511D RID: 20765
			JunkpileA,
			// Token: 0x0400511E RID: 20766
			JunkpileG,
			// Token: 0x0400511F RID: 20767
			CH47,
			// Token: 0x04005120 RID: 20768
			None,
			// Token: 0x04005121 RID: 20769
			Compound,
			// Token: 0x04005122 RID: 20770
			BanditTown,
			// Token: 0x04005123 RID: 20771
			CargoShip
		}
	}
}
