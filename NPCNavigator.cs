using System;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000208 RID: 520
public class NPCNavigator : BaseNavigator
{
	// Token: 0x1700024E RID: 590
	// (get) Token: 0x06001B62 RID: 7010 RVA: 0x000C21E2 File Offset: 0x000C03E2
	// (set) Token: 0x06001B63 RID: 7011 RVA: 0x000C21EA File Offset: 0x000C03EA
	public BaseNpc NPC { get; private set; }

	// Token: 0x06001B64 RID: 7012 RVA: 0x000C21F3 File Offset: 0x000C03F3
	public override void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		base.Init(entity, agent);
		this.NPC = entity as BaseNpc;
		this.sampleFailCount = 0;
	}

	// Token: 0x06001B65 RID: 7013 RVA: 0x000C2210 File Offset: 0x000C0410
	public override void OnFailedToPlaceOnNavmesh()
	{
		base.OnFailedToPlaceOnNavmesh();
		if (SingletonComponent<DynamicNavMesh>.Instance == null || SingletonComponent<DynamicNavMesh>.Instance.IsBuilding)
		{
			return;
		}
		this.sampleFailCount++;
		if (this.DestroyOnFailedSampleCount > 0 && this.sampleFailCount >= this.DestroyOnFailedSampleCount)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Failed to sample navmesh ",
				this.sampleFailCount,
				" times in a row at: ",
				base.transform.position,
				". Destroying: ",
				base.gameObject.name
			}));
			if (this.NPC != null && !this.NPC.IsDestroyed)
			{
				this.NPC.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x06001B66 RID: 7014 RVA: 0x000C22E3 File Offset: 0x000C04E3
	public override void OnPlacedOnNavmesh()
	{
		base.OnPlacedOnNavmesh();
		this.sampleFailCount = 0;
	}

	// Token: 0x06001B67 RID: 7015 RVA: 0x000C22F2 File Offset: 0x000C04F2
	protected override bool CanEnableNavMeshNavigation()
	{
		return base.CanEnableNavMeshNavigation();
	}

	// Token: 0x06001B68 RID: 7016 RVA: 0x000C2300 File Offset: 0x000C0500
	protected override bool CanUpdateMovement()
	{
		if (!base.CanUpdateMovement())
		{
			return false;
		}
		if (this.NPC != null && (this.NPC.IsDormant || !this.NPC.syncPosition) && base.Agent.enabled)
		{
			base.SetDestination(this.NPC.ServerPosition, 1f, 0f, 0f);
			return false;
		}
		return true;
	}

	// Token: 0x06001B69 RID: 7017 RVA: 0x000C2370 File Offset: 0x000C0570
	protected override void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		base.UpdatePositionAndRotation(moveToPosition, delta);
		this.UpdateRotation(moveToPosition, delta);
	}

	// Token: 0x06001B6A RID: 7018 RVA: 0x000C2384 File Offset: 0x000C0584
	private void UpdateRotation(Vector3 moveToPosition, float delta)
	{
		if (this.overrideFacingDirectionMode != BaseNavigator.OverrideFacingDirectionMode.None)
		{
			return;
		}
		if (this.traversingNavMeshLink)
		{
			Vector3 vector = base.Agent.destination - base.BaseEntity.ServerPosition;
			if (vector.sqrMagnitude > 1f)
			{
				vector = this.currentNavMeshLinkEndPos - base.BaseEntity.ServerPosition;
			}
			float sqrMagnitude = vector.sqrMagnitude;
			return;
		}
		if ((base.Agent.destination - base.BaseEntity.ServerPosition).sqrMagnitude > 1f)
		{
			Vector3 normalized = base.Agent.desiredVelocity.normalized;
			if (normalized.sqrMagnitude > 0.001f)
			{
				base.BaseEntity.ServerRotation = Quaternion.LookRotation(normalized);
				return;
			}
		}
	}

	// Token: 0x06001B6B RID: 7019 RVA: 0x000C2450 File Offset: 0x000C0650
	public override void ApplyFacingDirectionOverride()
	{
		base.ApplyFacingDirectionOverride();
		base.BaseEntity.ServerRotation = Quaternion.LookRotation(base.FacingDirectionOverride);
	}

	// Token: 0x06001B6C RID: 7020 RVA: 0x000C246E File Offset: 0x000C066E
	public override bool IsSwimming()
	{
		return AI.npcswimming && this.NPC != null && this.NPC.swimming;
	}

	// Token: 0x04001342 RID: 4930
	public int DestroyOnFailedSampleCount = 5;

	// Token: 0x04001344 RID: 4932
	private int sampleFailCount;
}
