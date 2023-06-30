using System;
using UnityEngine;

// Token: 0x02000578 RID: 1400
public class IndividualSpawner : BaseMonoBehaviour, IServerComponent, ISpawnPointUser, ISpawnGroup
{
	// Token: 0x17000394 RID: 916
	// (get) Token: 0x06002AF7 RID: 10999 RVA: 0x00105AFD File Offset: 0x00103CFD
	public int currentPopulation
	{
		get
		{
			if (!(this.spawnInstance == null))
			{
				return 1;
			}
			return 0;
		}
	}

	// Token: 0x17000395 RID: 917
	// (get) Token: 0x06002AF8 RID: 11000 RVA: 0x00105B10 File Offset: 0x00103D10
	private bool IsSpawned
	{
		get
		{
			return this.spawnInstance != null;
		}
	}

	// Token: 0x06002AF9 RID: 11001 RVA: 0x00105B1E File Offset: 0x00103D1E
	protected void Awake()
	{
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add(this);
			return;
		}
		Debug.LogWarning(base.GetType().Name + ": SpawnHandler instance not found.");
	}

	// Token: 0x06002AFA RID: 11002 RVA: 0x00105B57 File Offset: 0x00103D57
	protected void OnDestroy()
	{
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Remove(this);
			return;
		}
		Debug.LogWarning(base.GetType().Name + ": SpawnHandler instance not found.");
	}

	// Token: 0x06002AFB RID: 11003 RVA: 0x00105B94 File Offset: 0x00103D94
	protected void OnDrawGizmosSelected()
	{
		Bounds bounds;
		if (this.TryGetEntityBounds(out bounds))
		{
			Gizmos.color = Color.yellow;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawCube(bounds.center, bounds.size);
		}
	}

	// Token: 0x06002AFC RID: 11004 RVA: 0x00105BD8 File Offset: 0x00103DD8
	public void ObjectSpawned(SpawnPointInstance instance)
	{
		this.spawnInstance = instance;
	}

	// Token: 0x06002AFD RID: 11005 RVA: 0x00105BE1 File Offset: 0x00103DE1
	public void ObjectRetired(SpawnPointInstance instance)
	{
		this.spawnInstance = null;
		this.nextSpawnTime = Time.time + UnityEngine.Random.Range(this.respawnDelayMin, this.respawnDelayMax);
	}

	// Token: 0x06002AFE RID: 11006 RVA: 0x00105C07 File Offset: 0x00103E07
	public void Fill()
	{
		if (this.oneTimeSpawner)
		{
			return;
		}
		this.TrySpawnEntity();
	}

	// Token: 0x06002AFF RID: 11007 RVA: 0x00105C18 File Offset: 0x00103E18
	public void SpawnInitial()
	{
		this.TrySpawnEntity();
	}

	// Token: 0x06002B00 RID: 11008 RVA: 0x00105C20 File Offset: 0x00103E20
	public void Clear()
	{
		if (this.IsSpawned)
		{
			BaseEntity baseEntity = this.spawnInstance.gameObject.ToBaseEntity();
			if (baseEntity != null)
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x06002B01 RID: 11009 RVA: 0x00105C56 File Offset: 0x00103E56
	public void SpawnRepeating()
	{
		if (this.IsSpawned || this.oneTimeSpawner)
		{
			return;
		}
		if (Time.time >= this.nextSpawnTime)
		{
			this.TrySpawnEntity();
		}
	}

	// Token: 0x06002B02 RID: 11010 RVA: 0x00105C7C File Offset: 0x00103E7C
	public bool HasSpaceToSpawn()
	{
		if (this.useCustomBoundsCheckMask)
		{
			return SpawnHandler.CheckBounds(this.entityPrefab.Get(), base.transform.position, base.transform.rotation, Vector3.one, this.customBoundsCheckMask);
		}
		return SingletonComponent<SpawnHandler>.Instance.CheckBounds(this.entityPrefab.Get(), base.transform.position, base.transform.rotation, Vector3.one);
	}

	// Token: 0x06002B03 RID: 11011 RVA: 0x00105CF4 File Offset: 0x00103EF4
	private void TrySpawnEntity()
	{
		if (!this.isSpawnerActive)
		{
			return;
		}
		if (this.IsSpawned)
		{
			return;
		}
		if (!this.HasSpaceToSpawn())
		{
			this.nextSpawnTime = Time.time + UnityEngine.Random.Range(this.respawnDelayMin, this.respawnDelayMax);
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, base.transform.position, base.transform.rotation, false);
		if (baseEntity != null)
		{
			if (!this.oneTimeSpawner)
			{
				baseEntity.enableSaving = false;
			}
			baseEntity.gameObject.AwakeFromInstantiate();
			baseEntity.Spawn();
			SpawnPointInstance spawnPointInstance = baseEntity.gameObject.AddComponent<SpawnPointInstance>();
			spawnPointInstance.parentSpawnPointUser = this;
			spawnPointInstance.Notify();
			return;
		}
		Debug.LogError("IndividualSpawner failed to spawn entity.", base.gameObject);
	}

	// Token: 0x06002B04 RID: 11012 RVA: 0x00105DB8 File Offset: 0x00103FB8
	private bool TryGetEntityBounds(out Bounds result)
	{
		if (this.entityPrefab != null)
		{
			GameObject gameObject = this.entityPrefab.Get();
			if (gameObject != null)
			{
				BaseEntity component = gameObject.GetComponent<BaseEntity>();
				if (component != null)
				{
					result = component.bounds;
					return true;
				}
			}
		}
		result = default(Bounds);
		return false;
	}

	// Token: 0x04002317 RID: 8983
	public GameObjectRef entityPrefab;

	// Token: 0x04002318 RID: 8984
	public float respawnDelayMin = 10f;

	// Token: 0x04002319 RID: 8985
	public float respawnDelayMax = 20f;

	// Token: 0x0400231A RID: 8986
	public bool useCustomBoundsCheckMask;

	// Token: 0x0400231B RID: 8987
	public LayerMask customBoundsCheckMask;

	// Token: 0x0400231C RID: 8988
	[Tooltip("Simply spawns the entity once. No respawning. Entity can be saved if desired.")]
	[SerializeField]
	private bool oneTimeSpawner;

	// Token: 0x0400231D RID: 8989
	internal bool isSpawnerActive = true;

	// Token: 0x0400231E RID: 8990
	private SpawnPointInstance spawnInstance;

	// Token: 0x0400231F RID: 8991
	private float nextSpawnTime = -1f;
}
