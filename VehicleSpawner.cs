using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x020001A1 RID: 417
public class VehicleSpawner : BaseEntity
{
	// Token: 0x06001890 RID: 6288 RVA: 0x000B7246 File Offset: 0x000B5446
	public virtual int GetOccupyLayer()
	{
		return 32768;
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x000B7250 File Offset: 0x000B5450
	public BaseVehicle GetVehicleOccupying()
	{
		BaseVehicle baseVehicle = null;
		List<BaseVehicle> list = Pool.GetList<BaseVehicle>();
		Vis.Entities<BaseVehicle>(this.spawnOffset.transform.position, this.occupyRadius, list, this.GetOccupyLayer(), QueryTriggerInteraction.Ignore);
		if (list.Count > 0)
		{
			baseVehicle = list[0];
		}
		Pool.FreeList<BaseVehicle>(ref list);
		return baseVehicle;
	}

	// Token: 0x06001892 RID: 6290 RVA: 0x000B72A4 File Offset: 0x000B54A4
	public bool IsPadOccupied()
	{
		BaseVehicle vehicleOccupying = this.GetVehicleOccupying();
		return vehicleOccupying != null && !vehicleOccupying.IsDespawnEligable();
	}

	// Token: 0x06001893 RID: 6291 RVA: 0x000B72CC File Offset: 0x000B54CC
	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		BasePlayer basePlayer = null;
		NPCTalking component = from.GetComponent<NPCTalking>();
		if (component)
		{
			basePlayer = component.GetActionPlayer();
		}
		foreach (VehicleSpawner.SpawnPair spawnPair in this.objectsToSpawn)
		{
			if (msg == spawnPair.message)
			{
				this.SpawnVehicle(spawnPair.prefabToSpawn.resourcePath, basePlayer);
				return;
			}
		}
	}

	// Token: 0x06001894 RID: 6292 RVA: 0x000B7330 File Offset: 0x000B5530
	public BaseVehicle SpawnVehicle(string prefabToSpawn, BasePlayer newOwner)
	{
		this.CleanupArea(this.cleanupRadius);
		this.NudgePlayersInRadius(this.spawnNudgeRadius);
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefabToSpawn, this.spawnOffset.transform.position, this.spawnOffset.transform.rotation, true);
		baseEntity.Spawn();
		BaseVehicle component = baseEntity.GetComponent<BaseVehicle>();
		if (newOwner != null)
		{
			component.SetupOwner(newOwner, this.spawnOffset.transform.position, this.safeRadius);
		}
		VehicleSpawnPoint.AddStartingFuel(component);
		if (this.LogAnalytics)
		{
			Analytics.Server.VehiclePurchased(component.ShortPrefabName);
		}
		if (newOwner != null)
		{
			Analytics.Azure.OnVehiclePurchased(newOwner, baseEntity);
		}
		return component;
	}

	// Token: 0x17000207 RID: 519
	// (get) Token: 0x06001895 RID: 6293 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool LogAnalytics
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001896 RID: 6294 RVA: 0x000B73E0 File Offset: 0x000B55E0
	public void CleanupArea(float radius)
	{
		List<BaseVehicle> list = Pool.GetList<BaseVehicle>();
		Vis.Entities<BaseVehicle>(this.spawnOffset.transform.position, radius, list, 32768, QueryTriggerInteraction.Collide);
		foreach (BaseVehicle baseVehicle in list)
		{
			if (!baseVehicle.isClient && !baseVehicle.IsDestroyed)
			{
				baseVehicle.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		List<ServerGib> list2 = Pool.GetList<ServerGib>();
		Vis.Entities<ServerGib>(this.spawnOffset.transform.position, radius, list2, 67108865, QueryTriggerInteraction.Collide);
		foreach (ServerGib serverGib in list2)
		{
			if (!serverGib.isClient)
			{
				serverGib.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		Pool.FreeList<BaseVehicle>(ref list);
		Pool.FreeList<ServerGib>(ref list2);
	}

	// Token: 0x06001897 RID: 6295 RVA: 0x000B74DC File Offset: 0x000B56DC
	public void NudgePlayersInRadius(float radius)
	{
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(this.spawnOffset.transform.position, radius, list, 131072, QueryTriggerInteraction.Collide);
		foreach (BasePlayer basePlayer in list)
		{
			if (!basePlayer.IsNpc && !basePlayer.isMounted && basePlayer.IsConnected)
			{
				Vector3 vector = this.spawnOffset.transform.position;
				vector += Vector3Ex.Direction2D(basePlayer.transform.position, this.spawnOffset.transform.position) * radius;
				vector += Vector3.up * 0.1f;
				basePlayer.MovePosition(vector);
				basePlayer.ClientRPCPlayer<Vector3>(null, basePlayer, "ForcePositionTo", vector);
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
	}

	// Token: 0x04001136 RID: 4406
	public float spawnNudgeRadius = 6f;

	// Token: 0x04001137 RID: 4407
	public float cleanupRadius = 10f;

	// Token: 0x04001138 RID: 4408
	public float occupyRadius = 5f;

	// Token: 0x04001139 RID: 4409
	public VehicleSpawner.SpawnPair[] objectsToSpawn;

	// Token: 0x0400113A RID: 4410
	public Transform spawnOffset;

	// Token: 0x0400113B RID: 4411
	public float safeRadius = 10f;

	// Token: 0x02000C49 RID: 3145
	[Serializable]
	public class SpawnPair
	{
		// Token: 0x04004340 RID: 17216
		public string message;

		// Token: 0x04004341 RID: 17217
		public GameObjectRef prefabToSpawn;
	}
}
