using System;
using System.Collections;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000079 RID: 121
public class FrankensteinPet : BasePet, IAISenses, IAIAttack
{
	// Token: 0x06000B4B RID: 2891 RVA: 0x000654D0 File Offset: 0x000636D0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FrankensteinPet.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x00065510 File Offset: 0x00063710
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		base.InvokeRandomized(new Action(this.TickDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x0006554C File Offset: 0x0006374C
	public IEnumerator DelayEquipWeapon(ItemDefinition item, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (this.inventory == null)
		{
			yield break;
		}
		if (this.inventory.containerBelt == null)
		{
			yield break;
		}
		if (item == null)
		{
			yield break;
		}
		this.inventory.GiveItem(ItemManager.Create(item, 1, 0UL), this.inventory.containerBelt, false);
		this.EquipWeapon(false);
		yield break;
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x0006556C File Offset: 0x0006376C
	private void TickDecay()
	{
		BasePlayer basePlayer = BasePlayer.FindByID(base.OwnerID);
		if (basePlayer != null && !basePlayer.IsSleeping())
		{
			return;
		}
		if (base.healthFraction <= 0f || base.IsDestroyed)
		{
			return;
		}
		float num = 1f / FrankensteinPet.decayminutes;
		float num2 = this.MaxHealth() * num;
		base.Hurt(num2, DamageType.Decay, this, false);
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x000655D0 File Offset: 0x000637D0
	public float EngagementRange()
	{
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			return attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f) * base.Brain.AttackRangeMultiplier;
		}
		return base.Brain.SenseRange;
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x0006561F File Offset: 0x0006381F
	public bool IsThreat(BaseEntity entity)
	{
		return this.IsTarget(entity);
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x00065628 File Offset: 0x00063828
	public bool IsTarget(BaseEntity entity)
	{
		return entity is BasePlayer && !entity.IsNpc;
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool IsFriendly(BaseEntity entity)
	{
		return false;
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x00065640 File Offset: 0x00063840
	public bool CanAttack(BaseEntity entity)
	{
		float num;
		BasePlayer basePlayer;
		return !(entity == null) && entity.gameObject.layer != 21 && entity.gameObject.layer != 8 && !this.NeedsToReload() && !this.IsOnCooldown() && this.IsTargetInRange(entity, out num) && !base.InSafeZone() && ((basePlayer = entity as BasePlayer) == null || !basePlayer.InSafeZone()) && this.CanSeeTarget(entity);
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x000656C0 File Offset: 0x000638C0
	public bool IsTargetInRange(BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.transform.position);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x000656EC File Offset: 0x000638EC
	public bool CanSeeTarget(BaseEntity entity)
	{
		return !(entity == null) && entity.IsVisible(this.GetEntity().CenterPoint(), entity.CenterPoint(), float.PositiveInfinity);
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x0000441C File Offset: 0x0000261C
	public bool Reload()
	{
		return true;
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x00065715 File Offset: 0x00063915
	public float CooldownDuration()
	{
		return this.BaseAttackRate;
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x0006571D File Offset: 0x0006391D
	public bool IsOnCooldown()
	{
		return Time.realtimeSinceStartup < this.nextAttackTime;
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x0006572C File Offset: 0x0006392C
	public bool StartAttacking(BaseEntity target)
	{
		BaseCombatEntity baseCombatEntity = target as BaseCombatEntity;
		if (baseCombatEntity == null)
		{
			return false;
		}
		this.Attack(baseCombatEntity);
		return true;
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x00065754 File Offset: 0x00063954
	private void Attack(BaseCombatEntity target)
	{
		if (target == null)
		{
			return;
		}
		Vector3 vector = target.ServerPosition - this.ServerPosition;
		if (vector.magnitude > 0.001f)
		{
			this.ServerRotation = Quaternion.LookRotation(vector.normalized);
		}
		target.Hurt(this.BaseAttackDamge, this.AttackDamageType, this, true);
		base.SignalBroadcast(BaseEntity.Signal.Attack, null);
		base.ClientRPC(null, "OnAttack");
		this.nextAttackTime = Time.realtimeSinceStartup + this.CooldownDuration();
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopAttacking()
	{
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x000657D7 File Offset: 0x000639D7
	public float GetAmmoFraction()
	{
		return this.AmmoFractionRemaining();
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	public BaseEntity GetBestTarget()
	{
		return null;
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x000063A5 File Offset: 0x000045A5
	public void AttackTick(float delta, BaseEntity target, bool targetIsLOS)
	{
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x000657E0 File Offset: 0x000639E0
	public override BaseCorpse CreateCorpse()
	{
		BaseCorpse baseCorpse;
		using (TimeWarning.New("Create corpse", 0))
		{
			NPCPlayerCorpse npcplayerCorpse = base.DropCorpse("assets/rust.ai/agents/NPCPlayer/pet/frankensteinpet_corpse.prefab") as NPCPlayerCorpse;
			if (npcplayerCorpse)
			{
				npcplayerCorpse.transform.position = npcplayerCorpse.transform.position + Vector3.down * this.NavAgent.baseOffset;
				npcplayerCorpse.SetLootableIn(2f);
				npcplayerCorpse.SetFlag(BaseEntity.Flags.Reserved5, base.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash), false, true);
				npcplayerCorpse.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
				npcplayerCorpse.TakeFrom(new ItemContainer[]
				{
					this.inventory.containerMain,
					this.inventory.containerWear,
					this.inventory.containerBelt
				});
				npcplayerCorpse.playerName = this.OverrideCorpseName();
				npcplayerCorpse.playerSteamID = this.userID;
				npcplayerCorpse.Spawn();
				ItemContainer[] containers = npcplayerCorpse.containers;
				for (int i = 0; i < containers.Length; i++)
				{
					containers[i].Clear();
				}
			}
			baseCorpse = npcplayerCorpse;
		}
		return baseCorpse;
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x00065908 File Offset: 0x00063B08
	protected virtual string OverrideCorpseName()
	{
		return "Frankenstein";
	}

	// Token: 0x0400076E RID: 1902
	[Header("Frankenstein")]
	[ServerVar(Help = "How long before a Frankenstein Pet dies un controlled and not asleep on table")]
	public static float decayminutes = 180f;

	// Token: 0x0400076F RID: 1903
	[Header("Audio")]
	public SoundDefinition AttackVocalSFX;

	// Token: 0x04000770 RID: 1904
	private float nextAttackTime;
}
