using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000183 RID: 387
public class NPCDwelling : BaseEntity
{
	// Token: 0x060017DC RID: 6108 RVA: 0x000B4018 File Offset: 0x000B2218
	public override void ServerInit()
	{
		base.ServerInit();
		this.UpdateInformationZone(false);
		if (this.npcSpawner != null && UnityEngine.Random.Range(0f, 1f) <= this.NPCSpawnChance)
		{
			this.npcSpawner.SpawnInitial();
		}
		SpawnGroup[] array = this.spawnGroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpawnInitial();
		}
	}

	// Token: 0x060017DD RID: 6109 RVA: 0x000B407F File Offset: 0x000B227F
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			this.CleanupSpawned();
		}
		base.DestroyShared();
		if (base.isServer)
		{
			this.UpdateInformationZone(true);
		}
	}

	// Token: 0x060017DE RID: 6110 RVA: 0x000B40A4 File Offset: 0x000B22A4
	public bool ValidateAIPoint(Vector3 pos)
	{
		base.gameObject.SetActive(false);
		bool flag = !GamePhysics.CheckSphere(pos + Vector3.up * 0.6f, 0.5f, 65537, QueryTriggerInteraction.UseGlobal);
		base.gameObject.SetActive(true);
		return flag;
	}

	// Token: 0x060017DF RID: 6111 RVA: 0x000B40F4 File Offset: 0x000B22F4
	public void UpdateInformationZone(bool remove)
	{
		AIInformationZone forPoint = AIInformationZone.GetForPoint(base.transform.position, true);
		if (forPoint == null)
		{
			return;
		}
		if (remove)
		{
			forPoint.RemoveDynamicAIPoints(this.movePoints, this.coverPoints);
			return;
		}
		forPoint.AddDynamicAIPoints(this.movePoints, this.coverPoints, new Func<Vector3, bool>(this.ValidateAIPoint));
	}

	// Token: 0x060017E0 RID: 6112 RVA: 0x000B4151 File Offset: 0x000B2351
	public void CheckDespawn()
	{
		if (this.PlayersNearby())
		{
			return;
		}
		if (this.npcSpawner && this.npcSpawner.currentPopulation > 0)
		{
			return;
		}
		this.CleanupSpawned();
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060017E1 RID: 6113 RVA: 0x000B4188 File Offset: 0x000B2388
	public void CleanupSpawned()
	{
		if (this.spawnGroups != null)
		{
			SpawnGroup[] array = this.spawnGroups;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Clear();
			}
		}
		if (this.npcSpawner)
		{
			this.npcSpawner.Clear();
		}
	}

	// Token: 0x060017E2 RID: 6114 RVA: 0x000B41D4 File Offset: 0x000B23D4
	public bool PlayersNearby()
	{
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(base.transform.position, this.TimeoutPlayerCheckRadius(), list, 131072, QueryTriggerInteraction.Collide);
		bool flag = false;
		foreach (BasePlayer basePlayer in list)
		{
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive())
			{
				flag = true;
				break;
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
		return flag;
	}

	// Token: 0x060017E3 RID: 6115 RVA: 0x0004DD46 File Offset: 0x0004BF46
	public virtual float TimeoutPlayerCheckRadius()
	{
		return 10f;
	}

	// Token: 0x040010A4 RID: 4260
	public NPCSpawner npcSpawner;

	// Token: 0x040010A5 RID: 4261
	public float NPCSpawnChance = 1f;

	// Token: 0x040010A6 RID: 4262
	public SpawnGroup[] spawnGroups;

	// Token: 0x040010A7 RID: 4263
	public AIMovePoint[] movePoints;

	// Token: 0x040010A8 RID: 4264
	public AICoverPoint[] coverPoints;
}
