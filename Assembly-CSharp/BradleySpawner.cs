using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020001A8 RID: 424
public class BradleySpawner : MonoBehaviour, IServerComponent
{
	// Token: 0x060018C7 RID: 6343 RVA: 0x000B871C File Offset: 0x000B691C
	public void Start()
	{
		BradleySpawner.singleton = this;
		base.Invoke("DelayedStart", 3f);
	}

	// Token: 0x060018C8 RID: 6344 RVA: 0x000B8734 File Offset: 0x000B6934
	public void DelayedStart()
	{
		if (this.initialSpawn)
		{
			this.DoRespawn();
		}
		base.InvokeRepeating("CheckIfRespawnNeeded", 0f, 5f);
	}

	// Token: 0x060018C9 RID: 6345 RVA: 0x000B8759 File Offset: 0x000B6959
	public void CheckIfRespawnNeeded()
	{
		if (!this.pendingRespawn && (this.spawned == null || !this.spawned.IsAlive()))
		{
			this.ScheduleRespawn();
		}
	}

	// Token: 0x060018CA RID: 6346 RVA: 0x000B8784 File Offset: 0x000B6984
	public void ScheduleRespawn()
	{
		base.CancelInvoke("DoRespawn");
		base.Invoke("DoRespawn", UnityEngine.Random.Range(Bradley.respawnDelayMinutes - Bradley.respawnDelayVariance, Bradley.respawnDelayMinutes + Bradley.respawnDelayVariance) * 60f);
		this.pendingRespawn = true;
	}

	// Token: 0x060018CB RID: 6347 RVA: 0x000B87C4 File Offset: 0x000B69C4
	public void DoRespawn()
	{
		if (!Rust.Application.isLoading && !Rust.Application.isLoadingSave)
		{
			this.SpawnBradley();
		}
		this.pendingRespawn = false;
	}

	// Token: 0x060018CC RID: 6348 RVA: 0x000B87E4 File Offset: 0x000B69E4
	public void SpawnBradley()
	{
		if (this.spawned != null)
		{
			Debug.LogWarning("Bradley attempting to spawn but one already exists!");
			return;
		}
		if (!Bradley.enabled)
		{
			return;
		}
		Vector3 position = this.path.interestZones[UnityEngine.Random.Range(0, this.path.interestZones.Count)].transform.position;
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.bradleyPrefab.resourcePath, position, default(Quaternion), true);
		BradleyAPC component = baseEntity.GetComponent<BradleyAPC>();
		if (component)
		{
			baseEntity.Spawn();
			component.InstallPatrolPath(this.path);
		}
		else
		{
			baseEntity.Kill(BaseNetworkable.DestroyMode.None);
		}
		Debug.Log("BradleyAPC Spawned at :" + position);
		this.spawned = component;
	}

	// Token: 0x0400116F RID: 4463
	public BasePath path;

	// Token: 0x04001170 RID: 4464
	public GameObjectRef bradleyPrefab;

	// Token: 0x04001171 RID: 4465
	[NonSerialized]
	public BradleyAPC spawned;

	// Token: 0x04001172 RID: 4466
	public bool initialSpawn;

	// Token: 0x04001173 RID: 4467
	public float minRespawnTimeMinutes = 5f;

	// Token: 0x04001174 RID: 4468
	public float maxRespawnTimeMinutes = 5f;

	// Token: 0x04001175 RID: 4469
	public static BradleySpawner singleton;

	// Token: 0x04001176 RID: 4470
	private bool pendingRespawn;
}
