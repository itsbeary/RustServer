using System;
using UnityEngine;

// Token: 0x02000216 RID: 534
public class BaseFishNPC : BaseNpc, IAIAttack, IAISenses, IThinker
{
	// Token: 0x06001BC8 RID: 7112 RVA: 0x000C3CE4 File Offset: 0x000C1EE4
	public override void ServerInit()
	{
		base.ServerInit();
		this.brain = base.GetComponent<FishBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.AddAnimal(this);
	}

	// Token: 0x06001BC9 RID: 7113 RVA: 0x000C3A22 File Offset: 0x000C1C22
	internal override void DoServerDestroy()
	{
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.RemoveAnimal(this);
		base.DoServerDestroy();
	}

	// Token: 0x06001BCA RID: 7114 RVA: 0x000C3D07 File Offset: 0x000C1F07
	public virtual void TryThink()
	{
		if (this.brain.ShouldServerThink())
		{
			this.brain.DoThink();
		}
	}

	// Token: 0x06001BCB RID: 7115 RVA: 0x000C3D24 File Offset: 0x000C1F24
	public bool CanAttack(BaseEntity entity)
	{
		float num;
		return !this.IsOnCooldown() && this.IsTargetInRange(entity, out num) && this.CanSeeTarget(entity);
	}

	// Token: 0x06001BCC RID: 7116 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06001BCD RID: 7117 RVA: 0x000C3D54 File Offset: 0x000C1F54
	public float EngagementRange()
	{
		return this.AttackRange * this.brain.AttackRangeMultiplier;
	}

	// Token: 0x06001BCE RID: 7118 RVA: 0x000C3D68 File Offset: 0x000C1F68
	public bool IsTargetInRange(BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.AttackPosition);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06001BCF RID: 7119 RVA: 0x000656EC File Offset: 0x000638EC
	public bool CanSeeTarget(BaseEntity entity)
	{
		return !(entity == null) && entity.IsVisible(this.GetEntity().CenterPoint(), entity.CenterPoint(), float.PositiveInfinity);
	}

	// Token: 0x06001BD0 RID: 7120 RVA: 0x0000441C File Offset: 0x0000261C
	public bool Reload()
	{
		return true;
	}

	// Token: 0x06001BD1 RID: 7121 RVA: 0x000C3D90 File Offset: 0x000C1F90
	public bool StartAttacking(BaseEntity target)
	{
		BaseCombatEntity baseCombatEntity = target as BaseCombatEntity;
		if (baseCombatEntity == null)
		{
			return false;
		}
		base.Attack(baseCombatEntity);
		return true;
	}

	// Token: 0x06001BD2 RID: 7122 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopAttacking()
	{
	}

	// Token: 0x06001BD3 RID: 7123 RVA: 0x00036595 File Offset: 0x00034795
	public float CooldownDuration()
	{
		return this.AttackRate;
	}

	// Token: 0x06001BD4 RID: 7124 RVA: 0x000C3BDB File Offset: 0x000C1DDB
	public bool IsOnCooldown()
	{
		return !base.AttackReady();
	}

	// Token: 0x06001BD5 RID: 7125 RVA: 0x000C3DB8 File Offset: 0x000C1FB8
	public bool IsThreat(BaseEntity entity)
	{
		BaseNpc baseNpc = entity as BaseNpc;
		if (baseNpc != null)
		{
			return baseNpc.Stats.Family != this.Stats.Family && base.IsAfraidOf(baseNpc.Stats.Family);
		}
		BasePlayer basePlayer = entity as BasePlayer;
		return basePlayer != null && base.IsAfraidOf(basePlayer.Family);
	}

	// Token: 0x06001BD6 RID: 7126 RVA: 0x000C3E20 File Offset: 0x000C2020
	public bool IsTarget(BaseEntity entity)
	{
		BaseNpc baseNpc = entity as BaseNpc;
		return (!(baseNpc != null) || baseNpc.Stats.Family != this.Stats.Family) && !this.IsThreat(entity);
	}

	// Token: 0x06001BD7 RID: 7127 RVA: 0x000BE8C8 File Offset: 0x000BCAC8
	public bool IsFriendly(BaseEntity entity)
	{
		return !(entity == null) && entity.prefabID == this.prefabID;
	}

	// Token: 0x06001BD8 RID: 7128 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public float GetAmmoFraction()
	{
		return 1f;
	}

	// Token: 0x06001BD9 RID: 7129 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	public BaseEntity GetBestTarget()
	{
		return null;
	}

	// Token: 0x06001BDA RID: 7130 RVA: 0x000063A5 File Offset: 0x000045A5
	public void AttackTick(float delta, BaseEntity target, bool targetIsLOS)
	{
	}

	// Token: 0x0400138E RID: 5006
	protected FishBrain brain;
}
