using System;
using ConVar;
using UnityEngine;

// Token: 0x0200057A RID: 1402
public class NPCSpawner : SpawnGroup
{
	// Token: 0x06002B08 RID: 11016 RVA: 0x00105E5E File Offset: 0x0010405E
	public override void SpawnInitial()
	{
		this.fillOnSpawn = this.shouldFillOnSpawn;
		if (this.WaitingForNavMesh())
		{
			base.Invoke(new Action(this.LateSpawn), 10f);
			return;
		}
		base.SpawnInitial();
	}

	// Token: 0x06002B09 RID: 11017 RVA: 0x00105E92 File Offset: 0x00104092
	public bool WaitingForNavMesh()
	{
		if (this.monumentNavMesh != null)
		{
			return this.monumentNavMesh.IsBuilding;
		}
		return !DungeonNavmesh.NavReady() || !AI.move;
	}

	// Token: 0x06002B0A RID: 11018 RVA: 0x00105EBF File Offset: 0x001040BF
	public void LateSpawn()
	{
		if (!this.WaitingForNavMesh())
		{
			this.SpawnInitial();
			Debug.Log("Navmesh complete, spawning");
			return;
		}
		base.Invoke(new Action(this.LateSpawn), 5f);
	}

	// Token: 0x06002B0B RID: 11019 RVA: 0x00105EF4 File Offset: 0x001040F4
	protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
		base.PostSpawnProcess(entity, spawnPoint);
		BaseNavigator component = entity.GetComponent<BaseNavigator>();
		HumanNPC humanNPC;
		if (this.AdditionalLOSBlockingLayer != 0 && entity != null && (humanNPC = entity as HumanNPC) != null)
		{
			humanNPC.AdditionalLosBlockingLayer = this.AdditionalLOSBlockingLayer;
		}
		HumanNPC humanNPC2 = entity as HumanNPC;
		if (humanNPC2 != null)
		{
			if (this.Loadouts != null && this.Loadouts.Length != 0)
			{
				humanNPC2.EquipLoadout(this.Loadouts);
			}
			this.ModifyHumanBrainStats(humanNPC2.Brain);
		}
		if (this.VirtualInfoZone != null)
		{
			if (this.VirtualInfoZone.Virtual)
			{
				NPCPlayer npcplayer = entity as NPCPlayer;
				if (npcplayer != null)
				{
					npcplayer.VirtualInfoZone = this.VirtualInfoZone;
					if (humanNPC2 != null)
					{
						humanNPC2.VirtualInfoZone.RegisterSleepableEntity(humanNPC2.Brain);
					}
				}
			}
			else
			{
				Debug.LogError("NPCSpawner trying to set a virtual info zone without the Virtual property!");
			}
		}
		if (component != null)
		{
			component.Path = this.Path;
			component.AStarGraph = this.AStarGraph;
		}
	}

	// Token: 0x06002B0C RID: 11020 RVA: 0x00105FF0 File Offset: 0x001041F0
	private void ModifyHumanBrainStats(BaseAIBrain brain)
	{
		if (!this.UseStatModifiers)
		{
			return;
		}
		if (brain == null)
		{
			return;
		}
		brain.SenseRange = this.SenseRange;
		brain.TargetLostRange *= this.TargetLostRange;
		brain.AttackRangeMultiplier = this.AttackRangeMultiplier;
		brain.ListenRange = this.ListenRange;
		brain.CheckLOS = this.CheckLOS;
		if (this.CanUseHealingItemsChance > 0f)
		{
			brain.CanUseHealingItems = UnityEngine.Random.Range(0f, 1f) <= this.CanUseHealingItemsChance;
		}
	}

	// Token: 0x04002321 RID: 8993
	public int AdditionalLOSBlockingLayer;

	// Token: 0x04002322 RID: 8994
	public MonumentNavMesh monumentNavMesh;

	// Token: 0x04002323 RID: 8995
	public bool shouldFillOnSpawn;

	// Token: 0x04002324 RID: 8996
	[Header("InfoZone Config")]
	public AIInformationZone VirtualInfoZone;

	// Token: 0x04002325 RID: 8997
	[Header("Navigator Config")]
	public AIMovePointPath Path;

	// Token: 0x04002326 RID: 8998
	public BasePath AStarGraph;

	// Token: 0x04002327 RID: 8999
	[Header("Human Stat Replacements")]
	public bool UseStatModifiers;

	// Token: 0x04002328 RID: 9000
	public float SenseRange = 30f;

	// Token: 0x04002329 RID: 9001
	public bool CheckLOS = true;

	// Token: 0x0400232A RID: 9002
	public float TargetLostRange = 50f;

	// Token: 0x0400232B RID: 9003
	public float AttackRangeMultiplier = 1f;

	// Token: 0x0400232C RID: 9004
	public float ListenRange = 10f;

	// Token: 0x0400232D RID: 9005
	public float CanUseHealingItemsChance;

	// Token: 0x0400232E RID: 9006
	[Header("Loadout Replacements")]
	public PlayerInventoryProperties[] Loadouts;
}
