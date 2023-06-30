using System;
using ConVar;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000200 RID: 512
public class ScarecrowNPC : NPCPlayer, IAISenses, IAIAttack, IThinker
{
	// Token: 0x06001AC8 RID: 6856 RVA: 0x0002A08D File Offset: 0x0002828D
	public override float StartHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06001AC9 RID: 6857 RVA: 0x0002A08D File Offset: 0x0002828D
	public override float StartMaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06001ACA RID: 6858 RVA: 0x0002A08D File Offset: 0x0002828D
	public override float MaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x06001ACB RID: 6859 RVA: 0x000BFE5E File Offset: 0x000BE05E
	// (set) Token: 0x06001ACC RID: 6860 RVA: 0x000BFE66 File Offset: 0x000BE066
	public ScarecrowBrain Brain { get; protected set; }

	// Token: 0x1700023A RID: 570
	// (get) Token: 0x06001ACD RID: 6861 RVA: 0x000BFE6F File Offset: 0x000BE06F
	public override BaseNpc.AiStatistics.FamilyEnum Family
	{
		get
		{
			return BaseNpc.AiStatistics.FamilyEnum.Murderer;
		}
	}

	// Token: 0x06001ACE RID: 6862 RVA: 0x000BFE72 File Offset: 0x000BE072
	public override void ServerInit()
	{
		base.ServerInit();
		this.Brain = base.GetComponent<ScarecrowBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.Add(this);
	}

	// Token: 0x06001ACF RID: 6863 RVA: 0x000BE081 File Offset: 0x000BC281
	internal override void DoServerDestroy()
	{
		AIThinkManager.Remove(this);
		base.DoServerDestroy();
	}

	// Token: 0x06001AD0 RID: 6864 RVA: 0x000BD9E2 File Offset: 0x000BBBE2
	public virtual void TryThink()
	{
		base.ServerThink_Internal();
	}

	// Token: 0x06001AD1 RID: 6865 RVA: 0x000BFE95 File Offset: 0x000BE095
	public override void ServerThink(float delta)
	{
		base.ServerThink(delta);
		if (this.Brain.ShouldServerThink())
		{
			this.Brain.DoThink();
		}
	}

	// Token: 0x06001AD2 RID: 6866 RVA: 0x000BFEB6 File Offset: 0x000BE0B6
	public override string Categorize()
	{
		return "Scarecrow";
	}

	// Token: 0x06001AD3 RID: 6867 RVA: 0x000BFEC0 File Offset: 0x000BE0C0
	public override void EquipWeapon(bool skipDeployDelay = false)
	{
		base.EquipWeapon(skipDeployDelay);
		global::HeldEntity heldEntity = base.GetHeldEntity();
		Chainsaw chainsaw;
		if (heldEntity != null && (chainsaw = heldEntity as Chainsaw) != null)
		{
			chainsaw.ServerNPCStart();
		}
	}

	// Token: 0x06001AD4 RID: 6868 RVA: 0x000BFEF4 File Offset: 0x000BE0F4
	public float EngagementRange()
	{
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			return attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f) * this.Brain.AttackRangeMultiplier;
		}
		return this.Brain.SenseRange;
	}

	// Token: 0x06001AD5 RID: 6869 RVA: 0x000BFF43 File Offset: 0x000BE143
	public bool IsThreat(global::BaseEntity entity)
	{
		return this.IsTarget(entity);
	}

	// Token: 0x06001AD6 RID: 6870 RVA: 0x00065628 File Offset: 0x00063828
	public bool IsTarget(global::BaseEntity entity)
	{
		return entity is global::BasePlayer && !entity.IsNpc;
	}

	// Token: 0x06001AD7 RID: 6871 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool IsFriendly(global::BaseEntity entity)
	{
		return false;
	}

	// Token: 0x06001AD8 RID: 6872 RVA: 0x000BFF4C File Offset: 0x000BE14C
	public bool CanAttack(global::BaseEntity entity)
	{
		float num;
		global::BasePlayer basePlayer;
		return !(entity == null) && !this.NeedsToReload() && !this.IsOnCooldown() && this.IsTargetInRange(entity, out num) && !base.InSafeZone() && ((basePlayer = entity as global::BasePlayer) == null || !basePlayer.InSafeZone()) && this.CanSeeTarget(entity);
	}

	// Token: 0x06001AD9 RID: 6873 RVA: 0x000BFFAD File Offset: 0x000BE1AD
	public bool IsTargetInRange(global::BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.transform.position);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06001ADA RID: 6874 RVA: 0x000656EC File Offset: 0x000638EC
	public bool CanSeeTarget(global::BaseEntity entity)
	{
		return !(entity == null) && entity.IsVisible(this.GetEntity().CenterPoint(), entity.CenterPoint(), float.PositiveInfinity);
	}

	// Token: 0x06001ADB RID: 6875 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06001ADC RID: 6876 RVA: 0x0000441C File Offset: 0x0000261C
	public bool Reload()
	{
		return true;
	}

	// Token: 0x06001ADD RID: 6877 RVA: 0x000BFFD9 File Offset: 0x000BE1D9
	public float CooldownDuration()
	{
		return this.BaseAttackRate;
	}

	// Token: 0x06001ADE RID: 6878 RVA: 0x000BFFE4 File Offset: 0x000BE1E4
	public bool IsOnCooldown()
	{
		AttackEntity attackEntity = base.GetAttackEntity();
		return !attackEntity || attackEntity.HasAttackCooldown();
	}

	// Token: 0x06001ADF RID: 6879 RVA: 0x000C0008 File Offset: 0x000BE208
	public bool StartAttacking(global::BaseEntity target)
	{
		BaseCombatEntity baseCombatEntity = target as BaseCombatEntity;
		if (baseCombatEntity == null)
		{
			return false;
		}
		this.Attack(baseCombatEntity);
		return true;
	}

	// Token: 0x06001AE0 RID: 6880 RVA: 0x000C0030 File Offset: 0x000BE230
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
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			attackEntity.ServerUse();
		}
	}

	// Token: 0x06001AE1 RID: 6881 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopAttacking()
	{
	}

	// Token: 0x06001AE2 RID: 6882 RVA: 0x000657D7 File Offset: 0x000639D7
	public float GetAmmoFraction()
	{
		return this.AmmoFractionRemaining();
	}

	// Token: 0x06001AE3 RID: 6883 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	public global::BaseEntity GetBestTarget()
	{
		return null;
	}

	// Token: 0x06001AE4 RID: 6884 RVA: 0x000063A5 File Offset: 0x000045A5
	public void AttackTick(float delta, global::BaseEntity target, bool targetIsLOS)
	{
	}

	// Token: 0x06001AE5 RID: 6885 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	// Token: 0x06001AE6 RID: 6886 RVA: 0x000C0090 File Offset: 0x000BE290
	public override BaseCorpse CreateCorpse()
	{
		BaseCorpse baseCorpse;
		using (TimeWarning.New("Create corpse", 0))
		{
			string text = "assets/prefabs/npc/murderer/murderer_corpse.prefab";
			NPCPlayerCorpse npcplayerCorpse = base.DropCorpse(text) as NPCPlayerCorpse;
			if (npcplayerCorpse)
			{
				npcplayerCorpse.transform.position = npcplayerCorpse.transform.position + Vector3.down * this.NavAgent.baseOffset;
				npcplayerCorpse.SetLootableIn(2f);
				npcplayerCorpse.SetFlag(global::BaseEntity.Flags.Reserved5, base.HasPlayerFlag(global::BasePlayer.PlayerFlags.DisplaySash), false, true);
				npcplayerCorpse.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
				npcplayerCorpse.TakeFrom(new global::ItemContainer[]
				{
					this.inventory.containerMain,
					this.inventory.containerWear,
					this.inventory.containerBelt
				});
				npcplayerCorpse.playerName = "Scarecrow";
				npcplayerCorpse.playerSteamID = this.userID;
				npcplayerCorpse.Spawn();
				global::ItemContainer[] containers = npcplayerCorpse.containers;
				for (int i = 0; i < containers.Length; i++)
				{
					containers[i].Clear();
				}
				if (this.LootSpawnSlots.Length != 0)
				{
					foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
					{
						for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
						{
							if (UnityEngine.Random.Range(0f, 1f) <= lootSpawnSlot.probability)
							{
								lootSpawnSlot.definition.SpawnIntoContainer(npcplayerCorpse.containers[0]);
							}
						}
					}
				}
			}
			baseCorpse = npcplayerCorpse;
		}
		return baseCorpse;
	}

	// Token: 0x06001AE7 RID: 6887 RVA: 0x000C023C File Offset: 0x000BE43C
	public override void Hurt(HitInfo info)
	{
		if (!info.isHeadshot)
		{
			if ((info.InitiatorPlayer != null && !info.InitiatorPlayer.IsNpc) || (info.InitiatorPlayer == null && info.Initiator != null && info.Initiator.IsNpc))
			{
				info.damageTypes.ScaleAll(Halloween.scarecrow_body_dmg_modifier);
			}
			else
			{
				info.damageTypes.ScaleAll(2f);
			}
		}
		base.Hurt(info);
	}

	// Token: 0x06001AE8 RID: 6888 RVA: 0x000BE860 File Offset: 0x000BCA60
	public override void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		base.AttackerInfo(info);
		info.inflictorName = this.inventory.containerBelt.GetSlot(0).info.shortname;
		info.attackerName = base.ShortPrefabName;
	}

	// Token: 0x040012EB RID: 4843
	public float BaseAttackRate = 2f;

	// Token: 0x040012EC RID: 4844
	[Header("Loot")]
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	// Token: 0x040012ED RID: 4845
	public static float NextBeanCanAllowedTime;

	// Token: 0x040012EE RID: 4846
	public bool BlockClothingOnCorpse;

	// Token: 0x040012EF RID: 4847
	public bool RoamAroundHomePoint;
}
