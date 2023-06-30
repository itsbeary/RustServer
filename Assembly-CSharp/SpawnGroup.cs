using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using UnityEngine;

// Token: 0x0200057E RID: 1406
public class SpawnGroup : BaseMonoBehaviour, IServerComponent, ISpawnPointUser, ISpawnGroup
{
	// Token: 0x17000396 RID: 918
	// (get) Token: 0x06002B18 RID: 11032 RVA: 0x0010621D File Offset: 0x0010441D
	public int currentPopulation
	{
		get
		{
			return this.spawnInstances.Count;
		}
	}

	// Token: 0x06002B19 RID: 11033 RVA: 0x0010622A File Offset: 0x0010442A
	public virtual bool WantsInitialSpawn()
	{
		return this.wantsInitialSpawn;
	}

	// Token: 0x06002B1A RID: 11034 RVA: 0x00106232 File Offset: 0x00104432
	public virtual bool WantsTimedSpawn()
	{
		return this.respawnDelayMax != float.PositiveInfinity;
	}

	// Token: 0x06002B1B RID: 11035 RVA: 0x00106244 File Offset: 0x00104444
	public float GetSpawnDelta()
	{
		return (this.respawnDelayMax + this.respawnDelayMin) * 0.5f / SpawnHandler.PlayerScale(ConVar.Spawn.player_scale);
	}

	// Token: 0x06002B1C RID: 11036 RVA: 0x00106264 File Offset: 0x00104464
	public float GetSpawnVariance()
	{
		return (this.respawnDelayMax - this.respawnDelayMin) * 0.5f / SpawnHandler.PlayerScale(ConVar.Spawn.player_scale);
	}

	// Token: 0x06002B1D RID: 11037 RVA: 0x00106284 File Offset: 0x00104484
	protected void Awake()
	{
		if (TerrainMeta.TopologyMap == null)
		{
			return;
		}
		int topology = TerrainMeta.TopologyMap.GetTopology(base.transform.position);
		int num = 469762048;
		int num2 = MonumentInfo.TierToMask(this.Tier);
		if (num2 != num && (num2 & topology) == 0)
		{
			return;
		}
		this.spawnPoints = base.GetComponentsInChildren<BaseSpawnPoint>();
		if (this.WantsTimedSpawn())
		{
			this.spawnClock.Add(this.GetSpawnDelta(), this.GetSpawnVariance(), new Action(this.Spawn));
		}
		if (!this.temporary && SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add(this);
		}
		if (this.forceInitialSpawn)
		{
			base.Invoke(new Action(this.SpawnInitial), 1f);
		}
		this.Monument = this.FindMonument();
	}

	// Token: 0x06002B1E RID: 11038 RVA: 0x00105B57 File Offset: 0x00103D57
	protected void OnDestroy()
	{
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Remove(this);
			return;
		}
		Debug.LogWarning(base.GetType().Name + ": SpawnHandler instance not found.");
	}

	// Token: 0x06002B1F RID: 11039 RVA: 0x00106358 File Offset: 0x00104558
	public void Fill()
	{
		if (this.isSpawnerActive)
		{
			this.Spawn(this.maxPopulation);
		}
	}

	// Token: 0x06002B20 RID: 11040 RVA: 0x00106370 File Offset: 0x00104570
	public void Clear()
	{
		for (int i = this.spawnInstances.Count - 1; i >= 0; i--)
		{
			SpawnPointInstance spawnPointInstance = this.spawnInstances[i];
			BaseEntity baseEntity = spawnPointInstance.gameObject.ToBaseEntity();
			if (this.setFreeIfMovedBeyond != null && !this.setFreeIfMovedBeyond.bounds.Contains(baseEntity.transform.position))
			{
				spawnPointInstance.Retire();
			}
			else if (baseEntity)
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		this.spawnInstances.Clear();
	}

	// Token: 0x06002B21 RID: 11041 RVA: 0x00106400 File Offset: 0x00104600
	public bool HasSpawned(uint prefabID)
	{
		foreach (SpawnPointInstance spawnPointInstance in this.spawnInstances)
		{
			BaseEntity baseEntity = spawnPointInstance.gameObject.ToBaseEntity();
			if (baseEntity && baseEntity.prefabID == prefabID)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002B22 RID: 11042 RVA: 0x00106470 File Offset: 0x00104670
	public virtual void SpawnInitial()
	{
		if (!this.wantsInitialSpawn)
		{
			return;
		}
		if (this.isSpawnerActive)
		{
			if (this.fillOnSpawn)
			{
				this.Spawn(this.maxPopulation);
				return;
			}
			this.Spawn();
		}
	}

	// Token: 0x06002B23 RID: 11043 RVA: 0x001064A0 File Offset: 0x001046A0
	public void SpawnRepeating()
	{
		for (int i = 0; i < this.spawnClock.events.Count; i++)
		{
			LocalClock.TimedEvent timedEvent = this.spawnClock.events[i];
			if (UnityEngine.Time.time > timedEvent.time)
			{
				timedEvent.delta = this.GetSpawnDelta();
				timedEvent.variance = this.GetSpawnVariance();
				this.spawnClock.events[i] = timedEvent;
			}
		}
		this.spawnClock.Tick();
	}

	// Token: 0x06002B24 RID: 11044 RVA: 0x0010651E File Offset: 0x0010471E
	public void ObjectSpawned(SpawnPointInstance instance)
	{
		this.spawnInstances.Add(instance);
	}

	// Token: 0x06002B25 RID: 11045 RVA: 0x0010652C File Offset: 0x0010472C
	public void ObjectRetired(SpawnPointInstance instance)
	{
		this.spawnInstances.Remove(instance);
	}

	// Token: 0x06002B26 RID: 11046 RVA: 0x0010653B File Offset: 0x0010473B
	public void DelayedSpawn()
	{
		base.Invoke(new Action(this.Spawn), 1f);
	}

	// Token: 0x06002B27 RID: 11047 RVA: 0x00106554 File Offset: 0x00104754
	public void Spawn()
	{
		if (this.isSpawnerActive)
		{
			this.Spawn(UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1));
		}
	}

	// Token: 0x06002B28 RID: 11048 RVA: 0x00106578 File Offset: 0x00104778
	protected virtual void Spawn(int numToSpawn)
	{
		numToSpawn = Mathf.Min(numToSpawn, this.maxPopulation - this.currentPopulation);
		for (int i = 0; i < numToSpawn; i++)
		{
			GameObjectRef prefab = this.GetPrefab();
			if (prefab != null && !string.IsNullOrEmpty(prefab.guid))
			{
				Vector3 vector;
				Quaternion quaternion;
				BaseSpawnPoint spawnPoint = this.GetSpawnPoint(prefab, out vector, out quaternion);
				if (spawnPoint)
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(prefab.resourcePath, vector, quaternion, false);
					if (baseEntity)
					{
						if (baseEntity.enableSaving && !(spawnPoint is SpaceCheckingSpawnPoint))
						{
							baseEntity.enableSaving = false;
						}
						baseEntity.gameObject.AwakeFromInstantiate();
						baseEntity.Spawn();
						this.PostSpawnProcess(baseEntity, spawnPoint);
						SpawnPointInstance spawnPointInstance = baseEntity.gameObject.AddComponent<SpawnPointInstance>();
						spawnPointInstance.parentSpawnPointUser = this;
						spawnPointInstance.parentSpawnPoint = spawnPoint;
						spawnPointInstance.Notify();
					}
				}
			}
		}
	}

	// Token: 0x06002B29 RID: 11049 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
	}

	// Token: 0x06002B2A RID: 11050 RVA: 0x00106654 File Offset: 0x00104854
	protected GameObjectRef GetPrefab()
	{
		float num = (float)this.prefabs.Sum(delegate(SpawnGroup.SpawnEntry x)
		{
			if (!this.preventDuplicates || !this.HasSpawned(x.prefab.resourceID))
			{
				return x.weight;
			}
			return 0;
		});
		if (num == 0f)
		{
			return null;
		}
		float num2 = UnityEngine.Random.Range(0f, num);
		foreach (SpawnGroup.SpawnEntry spawnEntry in this.prefabs)
		{
			int num3 = ((this.preventDuplicates && this.HasSpawned(spawnEntry.prefab.resourceID)) ? 0 : spawnEntry.weight);
			if ((num2 -= (float)num3) <= 0f)
			{
				return spawnEntry.prefab;
			}
		}
		return this.prefabs[this.prefabs.Count - 1].prefab;
	}

	// Token: 0x06002B2B RID: 11051 RVA: 0x00106730 File Offset: 0x00104930
	protected virtual BaseSpawnPoint GetSpawnPoint(GameObjectRef prefabRef, out Vector3 pos, out Quaternion rot)
	{
		BaseSpawnPoint baseSpawnPoint = null;
		pos = Vector3.zero;
		rot = Quaternion.identity;
		int num = UnityEngine.Random.Range(0, this.spawnPoints.Length);
		for (int i = 0; i < this.spawnPoints.Length; i++)
		{
			BaseSpawnPoint baseSpawnPoint2 = this.spawnPoints[(num + i) % this.spawnPoints.Length];
			if (!(baseSpawnPoint2 == null) && baseSpawnPoint2.IsAvailableTo(prefabRef) && !baseSpawnPoint2.HasPlayersIntersecting())
			{
				baseSpawnPoint = baseSpawnPoint2;
				break;
			}
		}
		if (baseSpawnPoint)
		{
			baseSpawnPoint.GetLocation(out pos, out rot);
		}
		return baseSpawnPoint;
	}

	// Token: 0x06002B2C RID: 11052 RVA: 0x001067BA File Offset: 0x001049BA
	private MonumentInfo FindMonument()
	{
		return base.GetComponentInParent<MonumentInfo>();
	}

	// Token: 0x06002B2D RID: 11053 RVA: 0x001067C2 File Offset: 0x001049C2
	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 0f, 1f);
		Gizmos.DrawSphere(base.transform.position, 0.25f);
	}

	// Token: 0x04002333 RID: 9011
	[InspectorFlags]
	public MonumentTier Tier = (MonumentTier)(-1);

	// Token: 0x04002334 RID: 9012
	public List<SpawnGroup.SpawnEntry> prefabs;

	// Token: 0x04002335 RID: 9013
	public int maxPopulation = 5;

	// Token: 0x04002336 RID: 9014
	public int numToSpawnPerTickMin = 1;

	// Token: 0x04002337 RID: 9015
	public int numToSpawnPerTickMax = 2;

	// Token: 0x04002338 RID: 9016
	public float respawnDelayMin = 10f;

	// Token: 0x04002339 RID: 9017
	public float respawnDelayMax = 20f;

	// Token: 0x0400233A RID: 9018
	public bool wantsInitialSpawn = true;

	// Token: 0x0400233B RID: 9019
	public bool temporary;

	// Token: 0x0400233C RID: 9020
	public bool forceInitialSpawn;

	// Token: 0x0400233D RID: 9021
	public bool preventDuplicates;

	// Token: 0x0400233E RID: 9022
	public bool isSpawnerActive = true;

	// Token: 0x0400233F RID: 9023
	public BoxCollider setFreeIfMovedBeyond;

	// Token: 0x04002340 RID: 9024
	public string category;

	// Token: 0x04002341 RID: 9025
	[NonSerialized]
	public MonumentInfo Monument;

	// Token: 0x04002342 RID: 9026
	protected bool fillOnSpawn;

	// Token: 0x04002343 RID: 9027
	protected BaseSpawnPoint[] spawnPoints;

	// Token: 0x04002344 RID: 9028
	private List<SpawnPointInstance> spawnInstances = new List<SpawnPointInstance>();

	// Token: 0x04002345 RID: 9029
	private LocalClock spawnClock = new LocalClock();

	// Token: 0x02000D6C RID: 3436
	[Serializable]
	public class SpawnEntry
	{
		// Token: 0x040047E2 RID: 18402
		public GameObjectRef prefab;

		// Token: 0x040047E3 RID: 18403
		public int weight = 1;

		// Token: 0x040047E4 RID: 18404
		public bool mobile;
	}
}
