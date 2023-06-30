using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile
{
	// Token: 0x02000B54 RID: 2900
	public class ScientistJunkpileSpawner : MonoBehaviour, IServerComponent, ISpawnGroup
	{
		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x06004628 RID: 17960 RVA: 0x00198EAF File Offset: 0x001970AF
		public int currentPopulation
		{
			get
			{
				return this.Spawned.Count;
			}
		}

		// Token: 0x06004629 RID: 17961 RVA: 0x00198EBC File Offset: 0x001970BC
		private void Awake()
		{
			this.SpawnPoints = base.GetComponentsInChildren<BaseSpawnPoint>();
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add(this);
			}
		}

		// Token: 0x0600462A RID: 17962 RVA: 0x00198EE6 File Offset: 0x001970E6
		protected void OnDestroy()
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Remove(this);
				return;
			}
			Debug.LogWarning(base.GetType().Name + ": SpawnHandler instance not found.");
		}

		// Token: 0x0600462B RID: 17963 RVA: 0x00198F20 File Offset: 0x00197120
		public void Fill()
		{
			this.DoRespawn();
		}

		// Token: 0x0600462C RID: 17964 RVA: 0x00198F28 File Offset: 0x00197128
		public void Clear()
		{
			if (this.Spawned == null)
			{
				return;
			}
			foreach (BaseCombatEntity baseCombatEntity in this.Spawned)
			{
				if (!(baseCombatEntity == null) && !(baseCombatEntity.gameObject == null) && !(baseCombatEntity.transform == null))
				{
					BaseEntity baseEntity = baseCombatEntity.gameObject.ToBaseEntity();
					if (baseEntity)
					{
						baseEntity.Kill(BaseNetworkable.DestroyMode.None);
					}
				}
			}
			this.Spawned.Clear();
		}

		// Token: 0x0600462D RID: 17965 RVA: 0x00198FC8 File Offset: 0x001971C8
		public void SpawnInitial()
		{
			this.nextRespawnTime = UnityEngine.Time.time + UnityEngine.Random.Range(3f, 4f);
			this.pendingRespawn = true;
		}

		// Token: 0x0600462E RID: 17966 RVA: 0x00198FEC File Offset: 0x001971EC
		public void SpawnRepeating()
		{
			this.CheckIfRespawnNeeded();
		}

		// Token: 0x0600462F RID: 17967 RVA: 0x00198FF4 File Offset: 0x001971F4
		public void CheckIfRespawnNeeded()
		{
			if (!this.pendingRespawn)
			{
				if (this.Spawned == null || this.Spawned.Count == 0 || this.IsAllSpawnedDead())
				{
					this.ScheduleRespawn();
					return;
				}
			}
			else if ((this.Spawned == null || this.Spawned.Count == 0 || this.IsAllSpawnedDead()) && UnityEngine.Time.time >= this.nextRespawnTime)
			{
				this.DoRespawn();
			}
		}

		// Token: 0x06004630 RID: 17968 RVA: 0x00199060 File Offset: 0x00197260
		private bool IsAllSpawnedDead()
		{
			for (int i = 0; i < this.Spawned.Count; i++)
			{
				BaseCombatEntity baseCombatEntity = this.Spawned[i];
				if (!(baseCombatEntity == null) && !(baseCombatEntity.transform == null) && !baseCombatEntity.IsDestroyed && !baseCombatEntity.IsDead())
				{
					return false;
				}
				this.Spawned.RemoveAt(i);
				i--;
			}
			return true;
		}

		// Token: 0x06004631 RID: 17969 RVA: 0x001990CC File Offset: 0x001972CC
		public void ScheduleRespawn()
		{
			this.nextRespawnTime = UnityEngine.Time.time + UnityEngine.Random.Range(this.MinRespawnTimeMinutes, this.MaxRespawnTimeMinutes) * 60f;
			this.pendingRespawn = true;
		}

		// Token: 0x06004632 RID: 17970 RVA: 0x001990F8 File Offset: 0x001972F8
		public void DoRespawn()
		{
			if (!Application.isLoading && !Application.isLoadingSave)
			{
				this.SpawnScientist();
			}
			this.pendingRespawn = false;
		}

		// Token: 0x06004633 RID: 17971 RVA: 0x00199118 File Offset: 0x00197318
		public void SpawnScientist()
		{
			if (!AI.npc_enable)
			{
				return;
			}
			if (this.Spawned == null || this.Spawned.Count >= this.MaxPopulation)
			{
				return;
			}
			float num = this.SpawnBaseChance;
			ScientistJunkpileSpawner.JunkpileType spawnType = this.SpawnType;
			if (spawnType != ScientistJunkpileSpawner.JunkpileType.A)
			{
				if (spawnType == ScientistJunkpileSpawner.JunkpileType.G)
				{
					num = AI.npc_junkpile_g_spawn_chance;
				}
			}
			else
			{
				num = AI.npc_junkpile_a_spawn_chance;
			}
			if (UnityEngine.Random.value > num)
			{
				return;
			}
			int num2 = this.MaxPopulation - this.Spawned.Count;
			for (int i = 0; i < num2; i++)
			{
				Vector3 vector;
				Quaternion quaternion;
				if (!(this.GetSpawnPoint(out vector, out quaternion) == null))
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(this.ScientistPrefab.resourcePath, vector, quaternion, false);
					if (!(baseEntity != null))
					{
						return;
					}
					baseEntity.enableSaving = false;
					baseEntity.gameObject.AwakeFromInstantiate();
					baseEntity.Spawn();
					this.Spawned.Add((BaseCombatEntity)baseEntity);
				}
			}
		}

		// Token: 0x06004634 RID: 17972 RVA: 0x00199200 File Offset: 0x00197400
		private BaseSpawnPoint GetSpawnPoint(out Vector3 pos, out Quaternion rot)
		{
			BaseSpawnPoint baseSpawnPoint = null;
			pos = Vector3.zero;
			rot = Quaternion.identity;
			int num = UnityEngine.Random.Range(0, this.SpawnPoints.Length);
			for (int i = 0; i < this.SpawnPoints.Length; i++)
			{
				baseSpawnPoint = this.SpawnPoints[(num + i) % this.SpawnPoints.Length];
				if (baseSpawnPoint && baseSpawnPoint.gameObject.activeSelf)
				{
					break;
				}
			}
			if (baseSpawnPoint)
			{
				baseSpawnPoint.GetLocation(out pos, out rot);
			}
			return baseSpawnPoint;
		}

		// Token: 0x04003F1C RID: 16156
		public GameObjectRef ScientistPrefab;

		// Token: 0x04003F1D RID: 16157
		[NonSerialized]
		public List<BaseCombatEntity> Spawned = new List<BaseCombatEntity>();

		// Token: 0x04003F1E RID: 16158
		[NonSerialized]
		public BaseSpawnPoint[] SpawnPoints;

		// Token: 0x04003F1F RID: 16159
		public int MaxPopulation = 1;

		// Token: 0x04003F20 RID: 16160
		public bool InitialSpawn;

		// Token: 0x04003F21 RID: 16161
		public float MinRespawnTimeMinutes = 120f;

		// Token: 0x04003F22 RID: 16162
		public float MaxRespawnTimeMinutes = 120f;

		// Token: 0x04003F23 RID: 16163
		public float MovementRadius = -1f;

		// Token: 0x04003F24 RID: 16164
		public bool ReducedLongRangeAccuracy;

		// Token: 0x04003F25 RID: 16165
		public ScientistJunkpileSpawner.JunkpileType SpawnType;

		// Token: 0x04003F26 RID: 16166
		[Range(0f, 1f)]
		public float SpawnBaseChance = 1f;

		// Token: 0x04003F27 RID: 16167
		private float nextRespawnTime;

		// Token: 0x04003F28 RID: 16168
		private bool pendingRespawn;

		// Token: 0x02000FB5 RID: 4021
		public enum JunkpileType
		{
			// Token: 0x0400512C RID: 20780
			A,
			// Token: 0x0400512D RID: 20781
			B,
			// Token: 0x0400512E RID: 20782
			C,
			// Token: 0x0400512F RID: 20783
			D,
			// Token: 0x04005130 RID: 20784
			E,
			// Token: 0x04005131 RID: 20785
			F,
			// Token: 0x04005132 RID: 20786
			G
		}
	}
}
