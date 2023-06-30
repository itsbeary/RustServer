using System;
using UnityEngine;

// Token: 0x0200019B RID: 411
public class HorseSpawner : VehicleSpawner
{
	// Token: 0x0600186B RID: 6251 RVA: 0x000B6AD0 File Offset: 0x000B4CD0
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.RespawnHorse), UnityEngine.Random.Range(0f, 4f), this.respawnDelay, this.respawnDelayVariance);
	}

	// Token: 0x0600186C RID: 6252 RVA: 0x000B6B05 File Offset: 0x000B4D05
	public override int GetOccupyLayer()
	{
		return 2048;
	}

	// Token: 0x0600186D RID: 6253 RVA: 0x000B6B0C File Offset: 0x000B4D0C
	public void RespawnHorse()
	{
		if (base.GetVehicleOccupying() != null)
		{
			return;
		}
		BaseVehicle baseVehicle = base.SpawnVehicle(this.objectsToSpawn[0].prefabToSpawn.resourcePath, null);
		if (this.spawnForSale)
		{
			RidableHorse ridableHorse = baseVehicle as RidableHorse;
			if (ridableHorse != null)
			{
				ridableHorse.SetForSale();
			}
		}
	}

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x0600186E RID: 6254 RVA: 0x00007A44 File Offset: 0x00005C44
	protected override bool LogAnalytics
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0400112B RID: 4395
	public float respawnDelay = 10f;

	// Token: 0x0400112C RID: 4396
	public float respawnDelayVariance = 5f;

	// Token: 0x0400112D RID: 4397
	public bool spawnForSale = true;
}
