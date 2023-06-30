using System;
using UnityEngine;

// Token: 0x02000215 RID: 533
public class BaseAnimalNPC : BaseNpc, IAIAttack, IAITirednessAbove, IAISleep, IAIHungerAbove, IAISenses, IThinker
{
	// Token: 0x06001BAD RID: 7085 RVA: 0x000C39FF File Offset: 0x000C1BFF
	public override void ServerInit()
	{
		base.ServerInit();
		this.brain = base.GetComponent<AnimalBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.AddAnimal(this);
	}

	// Token: 0x06001BAE RID: 7086 RVA: 0x000C3A22 File Offset: 0x000C1C22
	internal override void DoServerDestroy()
	{
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.RemoveAnimal(this);
		base.DoServerDestroy();
	}

	// Token: 0x06001BAF RID: 7087 RVA: 0x000C3A39 File Offset: 0x000C1C39
	public virtual void TryThink()
	{
		if (this.brain.ShouldServerThink())
		{
			this.brain.DoThink();
		}
	}

	// Token: 0x06001BB0 RID: 7088 RVA: 0x000C3A54 File Offset: 0x000C1C54
	public override void OnKilled(HitInfo hitInfo = null)
	{
		if (hitInfo != null)
		{
			BasePlayer initiatorPlayer = hitInfo.InitiatorPlayer;
			if (initiatorPlayer != null)
			{
				initiatorPlayer.GiveAchievement("KILL_ANIMAL");
				if (!string.IsNullOrEmpty(this.deathStatName))
				{
					initiatorPlayer.stats.Add(this.deathStatName, 1, (Stats)5);
					initiatorPlayer.stats.Save(false);
				}
				initiatorPlayer.LifeStoryKill(this);
			}
		}
		base.OnKilled(null);
	}

	// Token: 0x06001BB1 RID: 7089 RVA: 0x000C3AB9 File Offset: 0x000C1CB9
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (base.isServer && info.InitiatorPlayer && !info.damageTypes.IsMeleeType())
		{
			info.InitiatorPlayer.LifeStoryShotHit(info.Weapon);
		}
	}

	// Token: 0x06001BB2 RID: 7090 RVA: 0x000C3AF5 File Offset: 0x000C1CF5
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001BB3 RID: 7091 RVA: 0x000C3B04 File Offset: 0x000C1D04
	public bool CanAttack(BaseEntity entity)
	{
		if (entity == null)
		{
			return false;
		}
		if (this.NeedsToReload())
		{
			return false;
		}
		if (this.IsOnCooldown())
		{
			return false;
		}
		float num;
		if (!this.IsTargetInRange(entity, out num))
		{
			return false;
		}
		if (!this.CanSeeTarget(entity))
		{
			return false;
		}
		BasePlayer basePlayer = entity as BasePlayer;
		BaseVehicle baseVehicle = ((basePlayer != null) ? basePlayer.GetMountedVehicle() : null);
		return !(baseVehicle != null) || !(baseVehicle is BaseModularVehicle);
	}

	// Token: 0x06001BB4 RID: 7092 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06001BB5 RID: 7093 RVA: 0x000C3B76 File Offset: 0x000C1D76
	public float EngagementRange()
	{
		return this.AttackRange * this.brain.AttackRangeMultiplier;
	}

	// Token: 0x06001BB6 RID: 7094 RVA: 0x000C3B8A File Offset: 0x000C1D8A
	public bool IsTargetInRange(BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.AttackPosition);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06001BB7 RID: 7095 RVA: 0x000656EC File Offset: 0x000638EC
	public bool CanSeeTarget(BaseEntity entity)
	{
		return !(entity == null) && entity.IsVisible(this.GetEntity().CenterPoint(), entity.CenterPoint(), float.PositiveInfinity);
	}

	// Token: 0x06001BB8 RID: 7096 RVA: 0x0002A507 File Offset: 0x00028707
	public bool Reload()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001BB9 RID: 7097 RVA: 0x000C3BB4 File Offset: 0x000C1DB4
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

	// Token: 0x06001BBA RID: 7098 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopAttacking()
	{
	}

	// Token: 0x06001BBB RID: 7099 RVA: 0x00036595 File Offset: 0x00034795
	public float CooldownDuration()
	{
		return this.AttackRate;
	}

	// Token: 0x06001BBC RID: 7100 RVA: 0x000C3BDB File Offset: 0x000C1DDB
	public bool IsOnCooldown()
	{
		return !base.AttackReady();
	}

	// Token: 0x06001BBD RID: 7101 RVA: 0x000C3BE6 File Offset: 0x000C1DE6
	public bool IsTirednessAbove(float value)
	{
		return 1f - this.Sleep > value;
	}

	// Token: 0x06001BBE RID: 7102 RVA: 0x000C3BF7 File Offset: 0x000C1DF7
	public void StartSleeping()
	{
		base.SetFact(BaseNpc.Facts.IsSleeping, 1, true, true);
	}

	// Token: 0x06001BBF RID: 7103 RVA: 0x000C3C03 File Offset: 0x000C1E03
	public void StopSleeping()
	{
		base.SetFact(BaseNpc.Facts.IsSleeping, 0, true, true);
	}

	// Token: 0x06001BC0 RID: 7104 RVA: 0x000C3C0F File Offset: 0x000C1E0F
	public bool IsHungerAbove(float value)
	{
		return 1f - this.Energy.Level > value;
	}

	// Token: 0x06001BC1 RID: 7105 RVA: 0x000C3C28 File Offset: 0x000C1E28
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

	// Token: 0x06001BC2 RID: 7106 RVA: 0x000C3C90 File Offset: 0x000C1E90
	public bool IsTarget(BaseEntity entity)
	{
		BaseNpc baseNpc = entity as BaseNpc;
		return (!(baseNpc != null) || baseNpc.Stats.Family != this.Stats.Family) && !this.IsThreat(entity);
	}

	// Token: 0x06001BC3 RID: 7107 RVA: 0x000BE8C8 File Offset: 0x000BCAC8
	public bool IsFriendly(BaseEntity entity)
	{
		return !(entity == null) && entity.prefabID == this.prefabID;
	}

	// Token: 0x06001BC4 RID: 7108 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public float GetAmmoFraction()
	{
		return 1f;
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	public BaseEntity GetBestTarget()
	{
		return null;
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x000063A5 File Offset: 0x000045A5
	public void AttackTick(float delta, BaseEntity target, bool targetIsLOS)
	{
	}

	// Token: 0x0400138C RID: 5004
	public string deathStatName = "";

	// Token: 0x0400138D RID: 5005
	protected AnimalBrain brain;
}
