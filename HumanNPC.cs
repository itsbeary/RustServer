using System;
using System.Collections;
using ProtoBuf;
using UnityEngine;

// Token: 0x020001F5 RID: 501
public class HumanNPC : NPCPlayer, IAISenses, IAIAttack, IThinker
{
	// Token: 0x06001A3D RID: 6717 RVA: 0x0002A08D File Offset: 0x0002828D
	public override float StartHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06001A3E RID: 6718 RVA: 0x0002A08D File Offset: 0x0002828D
	public override float StartMaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06001A3F RID: 6719 RVA: 0x0002A08D File Offset: 0x0002828D
	public override float MaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x17000233 RID: 563
	// (get) Token: 0x06001A40 RID: 6720 RVA: 0x000BE04D File Offset: 0x000BC24D
	// (set) Token: 0x06001A41 RID: 6721 RVA: 0x000BE055 File Offset: 0x000BC255
	public ScientistBrain Brain { get; private set; }

	// Token: 0x06001A42 RID: 6722 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsLoadBalanced()
	{
		return true;
	}

	// Token: 0x06001A43 RID: 6723 RVA: 0x000BE05E File Offset: 0x000BC25E
	public override void ServerInit()
	{
		base.ServerInit();
		this.Brain = base.GetComponent<ScientistBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.Add(this);
	}

	// Token: 0x06001A44 RID: 6724 RVA: 0x000BE081 File Offset: 0x000BC281
	internal override void DoServerDestroy()
	{
		AIThinkManager.Remove(this);
		base.DoServerDestroy();
	}

	// Token: 0x06001A45 RID: 6725 RVA: 0x000BE08F File Offset: 0x000BC28F
	public void LightCheck()
	{
		if ((TOD_Sky.Instance.IsNight && !this.lightsOn) || (TOD_Sky.Instance.IsDay && this.lightsOn))
		{
			base.LightToggle(true);
			this.lightsOn = !this.lightsOn;
		}
	}

	// Token: 0x06001A46 RID: 6726 RVA: 0x000BE0CF File Offset: 0x000BC2CF
	public override float GetAimConeScale()
	{
		return this.aimConeScale;
	}

	// Token: 0x06001A47 RID: 6727 RVA: 0x000BE0D7 File Offset: 0x000BC2D7
	public override void EquipWeapon(bool skipDeployDelay = false)
	{
		base.EquipWeapon(skipDeployDelay);
	}

	// Token: 0x06001A48 RID: 6728 RVA: 0x000BE0E0 File Offset: 0x000BC2E0
	public override void DismountObject()
	{
		base.DismountObject();
		this.lastDismountTime = Time.time;
	}

	// Token: 0x06001A49 RID: 6729 RVA: 0x000BE0F3 File Offset: 0x000BC2F3
	public bool RecentlyDismounted()
	{
		return Time.time < this.lastDismountTime + 10f;
	}

	// Token: 0x06001A4A RID: 6730 RVA: 0x000BE108 File Offset: 0x000BC308
	public virtual float GetIdealDistanceFromTarget()
	{
		return Mathf.Max(5f, this.EngagementRange() * 0.75f);
	}

	// Token: 0x06001A4B RID: 6731 RVA: 0x000BE120 File Offset: 0x000BC320
	public AIInformationZone GetInformationZone(Vector3 pos)
	{
		if (this.VirtualInfoZone != null)
		{
			return this.VirtualInfoZone;
		}
		if (this.cachedInfoZone == null || Time.time > this.nextZoneSearchTime)
		{
			this.cachedInfoZone = AIInformationZone.GetForPoint(pos, true);
			this.nextZoneSearchTime = Time.time + 5f;
		}
		return this.cachedInfoZone;
	}

	// Token: 0x06001A4C RID: 6732 RVA: 0x000BE184 File Offset: 0x000BC384
	public float EngagementRange()
	{
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			return attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f) * this.Brain.AttackRangeMultiplier;
		}
		return this.Brain.SenseRange;
	}

	// Token: 0x06001A4D RID: 6733 RVA: 0x000BE1D3 File Offset: 0x000BC3D3
	public void SetDucked(bool flag)
	{
		this.modelState.ducked = flag;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001A4E RID: 6734 RVA: 0x000BD9E2 File Offset: 0x000BBBE2
	public virtual void TryThink()
	{
		base.ServerThink_Internal();
	}

	// Token: 0x06001A4F RID: 6735 RVA: 0x000BE1E8 File Offset: 0x000BC3E8
	public override void ServerThink(float delta)
	{
		base.ServerThink(delta);
		if (this.Brain.ShouldServerThink())
		{
			this.Brain.DoThink();
		}
	}

	// Token: 0x06001A50 RID: 6736 RVA: 0x000BE20C File Offset: 0x000BC40C
	public void TickAttack(float delta, BaseCombatEntity target, bool targetIsLOS)
	{
		if (target == null)
		{
			return;
		}
		float num = Vector3.Dot(this.eyes.BodyForward(), (target.CenterPoint() - this.eyes.position).normalized);
		if (targetIsLOS)
		{
			if (num > 0.2f)
			{
				this.targetAimedDuration += delta;
			}
		}
		else
		{
			if (num < 0.5f)
			{
				this.targetAimedDuration = 0f;
			}
			base.CancelBurst(0.2f);
		}
		if (this.targetAimedDuration >= 0.2f && targetIsLOS)
		{
			bool flag = false;
			float num2 = 0f;
			if (this != null)
			{
				flag = ((IAIAttack)this).IsTargetInRange(target, out num2);
			}
			else
			{
				AttackEntity attackEntity = base.GetAttackEntity();
				if (attackEntity)
				{
					num2 = ((target != null) ? Vector3.Distance(base.transform.position, target.transform.position) : (-1f));
					flag = num2 < attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f);
				}
			}
			if (flag)
			{
				this.ShotTest(num2);
				return;
			}
		}
		else
		{
			base.CancelBurst(0.2f);
		}
	}

	// Token: 0x06001A51 RID: 6737 RVA: 0x000BE334 File Offset: 0x000BC534
	public override void Hurt(HitInfo info)
	{
		if (base.isMounted)
		{
			info.damageTypes.ScaleAll(0.1f);
		}
		base.Hurt(info);
		global::BaseEntity initiator = info.Initiator;
		if (initiator != null && !initiator.EqualNetID(this))
		{
			this.Brain.Senses.Memory.SetKnown(initiator, this, null);
		}
	}

	// Token: 0x06001A52 RID: 6738 RVA: 0x000BE391 File Offset: 0x000BC591
	public float GetAimSwayScalar()
	{
		return 1f - Mathf.InverseLerp(1f, 3f, Time.time - this.lastGunShotTime);
	}

	// Token: 0x06001A53 RID: 6739 RVA: 0x000BE3B4 File Offset: 0x000BC5B4
	public override Vector3 GetAimDirection()
	{
		if (this.Brain != null && this.Brain.Navigator != null && this.Brain.Navigator.IsOverridingFacingDirection)
		{
			return this.Brain.Navigator.FacingDirectionOverride;
		}
		return base.GetAimDirection();
	}

	// Token: 0x06001A54 RID: 6740 RVA: 0x000BE40C File Offset: 0x000BC60C
	public override void SetAimDirection(Vector3 newAim)
	{
		if (newAim == Vector3.zero)
		{
			return;
		}
		float num = Time.time - this.lastAimSetTime;
		this.lastAimSetTime = Time.time;
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			newAim = attackEntity.ModifyAIAim(newAim, this.GetAimSwayScalar());
		}
		if (base.isMounted)
		{
			BaseMountable mounted = base.GetMounted();
			Vector3 eulerAngles = mounted.transform.eulerAngles;
			Quaternion quaternion = Quaternion.Euler(Quaternion.LookRotation(newAim, mounted.transform.up).eulerAngles);
			Vector3 vector = Quaternion.LookRotation(base.transform.InverseTransformDirection(quaternion * Vector3.forward), base.transform.up).eulerAngles;
			vector = BaseMountable.ConvertVector(vector);
			Quaternion quaternion2 = Quaternion.Euler(Mathf.Clamp(vector.x, mounted.pitchClamp.x, mounted.pitchClamp.y), Mathf.Clamp(vector.y, mounted.yawClamp.x, mounted.yawClamp.y), eulerAngles.z);
			newAim = BaseMountable.ConvertVector(Quaternion.LookRotation(base.transform.TransformDirection(quaternion2 * Vector3.forward), base.transform.up).eulerAngles);
		}
		else
		{
			global::BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity)
			{
				Vector3 vector2 = parentEntity.transform.InverseTransformDirection(newAim);
				Vector3 vector3 = new Vector3(newAim.x, vector2.y, newAim.z);
				this.eyes.rotation = Quaternion.Lerp(this.eyes.rotation, Quaternion.LookRotation(vector3, parentEntity.transform.up), num * 25f);
				this.viewAngles = this.eyes.bodyRotation.eulerAngles;
				this.ServerRotation = this.eyes.bodyRotation;
				return;
			}
		}
		this.eyes.rotation = (base.isMounted ? Quaternion.Slerp(this.eyes.rotation, Quaternion.Euler(newAim), num * 70f) : Quaternion.Lerp(this.eyes.rotation, Quaternion.LookRotation(newAim, base.transform.up), num * 25f));
		this.viewAngles = this.eyes.rotation.eulerAngles;
		this.ServerRotation = this.eyes.rotation;
	}

	// Token: 0x06001A55 RID: 6741 RVA: 0x000BE687 File Offset: 0x000BC887
	public void SetStationaryAimPoint(Vector3 aimAt)
	{
		this.aimOverridePosition = aimAt;
	}

	// Token: 0x06001A56 RID: 6742 RVA: 0x000BE690 File Offset: 0x000BC890
	public void ClearStationaryAimPoint()
	{
		this.aimOverridePosition = Vector3.zero;
	}

	// Token: 0x06001A57 RID: 6743 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	// Token: 0x06001A58 RID: 6744 RVA: 0x000BE6A0 File Offset: 0x000BC8A0
	public override BaseCorpse CreateCorpse()
	{
		BaseCorpse baseCorpse;
		using (TimeWarning.New("Create corpse", 0))
		{
			NPCPlayerCorpse npcplayerCorpse = base.DropCorpse("assets/prefabs/npc/scientist/scientist_corpse.prefab") as NPCPlayerCorpse;
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
				npcplayerCorpse.playerName = this.OverrideCorpseName();
				npcplayerCorpse.playerSteamID = this.userID;
				npcplayerCorpse.Spawn();
				npcplayerCorpse.TakeChildren(this);
				for (int i = 0; i < npcplayerCorpse.containers.Length; i++)
				{
					global::ItemContainer itemContainer = npcplayerCorpse.containers[i];
					if (i != 1)
					{
						itemContainer.Clear();
					}
				}
				if (this.LootSpawnSlots.Length != 0)
				{
					foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
					{
						for (int k = 0; k < lootSpawnSlot.numberToSpawn; k++)
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

	// Token: 0x06001A59 RID: 6745 RVA: 0x000BE858 File Offset: 0x000BCA58
	protected virtual string OverrideCorpseName()
	{
		return base.displayName;
	}

	// Token: 0x06001A5A RID: 6746 RVA: 0x000BE860 File Offset: 0x000BCA60
	public override void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		base.AttackerInfo(info);
		info.inflictorName = this.inventory.containerBelt.GetSlot(0).info.shortname;
		info.attackerName = base.ShortPrefabName;
	}

	// Token: 0x06001A5B RID: 6747 RVA: 0x000BE896 File Offset: 0x000BCA96
	public bool IsThreat(global::BaseEntity entity)
	{
		return this.IsTarget(entity);
	}

	// Token: 0x06001A5C RID: 6748 RVA: 0x000BE89F File Offset: 0x000BCA9F
	public bool IsTarget(global::BaseEntity entity)
	{
		return (entity is global::BasePlayer && !entity.IsNpc) || entity is BasePet || entity is ScarecrowNPC;
	}

	// Token: 0x06001A5D RID: 6749 RVA: 0x000BE8C8 File Offset: 0x000BCAC8
	public bool IsFriendly(global::BaseEntity entity)
	{
		return !(entity == null) && entity.prefabID == this.prefabID;
	}

	// Token: 0x06001A5E RID: 6750 RVA: 0x0000441C File Offset: 0x0000261C
	public bool CanAttack(global::BaseEntity entity)
	{
		return true;
	}

	// Token: 0x06001A5F RID: 6751 RVA: 0x000BE8E3 File Offset: 0x000BCAE3
	public bool IsTargetInRange(global::BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.transform.position);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x000BE910 File Offset: 0x000BCB10
	public bool CanSeeTarget(global::BaseEntity entity)
	{
		global::BasePlayer basePlayer = entity as global::BasePlayer;
		if (basePlayer == null)
		{
			return true;
		}
		if (this.AdditionalLosBlockingLayer == 0)
		{
			return base.IsPlayerVisibleToUs(basePlayer, 1218519041);
		}
		return base.IsPlayerVisibleToUs(basePlayer, 1218519041 | (1 << this.AdditionalLosBlockingLayer));
	}

	// Token: 0x06001A61 RID: 6753 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06001A62 RID: 6754 RVA: 0x0000441C File Offset: 0x0000261C
	public bool Reload()
	{
		return true;
	}

	// Token: 0x06001A63 RID: 6755 RVA: 0x000BE95C File Offset: 0x000BCB5C
	public float CooldownDuration()
	{
		return 5f;
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool IsOnCooldown()
	{
		return false;
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x0000441C File Offset: 0x0000261C
	public bool StartAttacking(global::BaseEntity entity)
	{
		return true;
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopAttacking()
	{
	}

	// Token: 0x06001A67 RID: 6759 RVA: 0x000657D7 File Offset: 0x000639D7
	public float GetAmmoFraction()
	{
		return this.AmmoFractionRemaining();
	}

	// Token: 0x06001A68 RID: 6760 RVA: 0x000BE964 File Offset: 0x000BCB64
	public global::BaseEntity GetBestTarget()
	{
		global::BaseEntity baseEntity = null;
		float num = -1f;
		foreach (global::BaseEntity baseEntity2 in this.Brain.Senses.Players)
		{
			if (!(baseEntity2 == null) && baseEntity2.Health() > 0f)
			{
				float num2 = Vector3.Distance(baseEntity2.transform.position, base.transform.position);
				float num3 = 1f - Mathf.InverseLerp(1f, this.Brain.SenseRange, num2);
				float num4 = Vector3.Dot((baseEntity2.transform.position - this.eyes.position).normalized, this.eyes.BodyForward());
				num3 += Mathf.InverseLerp(this.Brain.VisionCone, 1f, num4) / 2f;
				num3 += (this.Brain.Senses.Memory.IsLOS(baseEntity2) ? 2f : 0f);
				if (num3 > num)
				{
					baseEntity = baseEntity2;
					num = num3;
				}
			}
		}
		return baseEntity;
	}

	// Token: 0x06001A69 RID: 6761 RVA: 0x000BEAAC File Offset: 0x000BCCAC
	public void AttackTick(float delta, global::BaseEntity target, bool targetIsLOS)
	{
		BaseCombatEntity baseCombatEntity = target as BaseCombatEntity;
		this.TickAttack(delta, baseCombatEntity, targetIsLOS);
	}

	// Token: 0x06001A6A RID: 6762 RVA: 0x000BEAC9 File Offset: 0x000BCCC9
	public void UseHealingItem(global::Item item)
	{
		base.StartCoroutine(this.Heal(item));
	}

	// Token: 0x06001A6B RID: 6763 RVA: 0x000BEAD9 File Offset: 0x000BCCD9
	private IEnumerator Heal(global::Item item)
	{
		base.UpdateActiveItem(item.uid);
		global::Item activeItem = base.GetActiveItem();
		MedicalTool heldItem = activeItem.GetHeldEntity() as MedicalTool;
		if (heldItem == null)
		{
			yield break;
		}
		yield return new WaitForSeconds(1f);
		heldItem.ServerUse();
		this.Heal(this.MaxHealth());
		yield return new WaitForSeconds(2f);
		this.EquipWeapon(false);
		yield break;
	}

	// Token: 0x06001A6C RID: 6764 RVA: 0x000BEAF0 File Offset: 0x000BCCF0
	public global::Item FindHealingItem()
	{
		if (this.Brain == null)
		{
			return null;
		}
		if (!this.Brain.CanUseHealingItems)
		{
			return null;
		}
		if (this.inventory == null || this.inventory.containerBelt == null)
		{
			return null;
		}
		for (int i = 0; i < this.inventory.containerBelt.capacity; i++)
		{
			global::Item slot = this.inventory.containerBelt.GetSlot(i);
			if (slot != null && slot.amount > 1 && slot.GetHeldEntity() as MedicalTool != null)
			{
				return slot;
			}
		}
		return null;
	}

	// Token: 0x06001A6D RID: 6765 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsOnGround()
	{
		return true;
	}

	// Token: 0x040012C1 RID: 4801
	[Header("LOS")]
	public int AdditionalLosBlockingLayer;

	// Token: 0x040012C2 RID: 4802
	[Header("Loot")]
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	// Token: 0x040012C3 RID: 4803
	[Header("Damage")]
	public float aimConeScale = 2f;

	// Token: 0x040012C4 RID: 4804
	public float lastDismountTime;

	// Token: 0x040012C6 RID: 4806
	[NonSerialized]
	protected bool lightsOn;

	// Token: 0x040012C7 RID: 4807
	private float nextZoneSearchTime;

	// Token: 0x040012C8 RID: 4808
	private AIInformationZone cachedInfoZone;

	// Token: 0x040012C9 RID: 4809
	private float targetAimedDuration;

	// Token: 0x040012CA RID: 4810
	private float lastAimSetTime;

	// Token: 0x040012CB RID: 4811
	private Vector3 aimOverridePosition = Vector3.zero;
}
