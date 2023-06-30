using System;
using ConVar;
using UnityEngine;

// Token: 0x020003AF RID: 943
public class AttackEntity : HeldEntity
{
	// Token: 0x0600213B RID: 8507 RVA: 0x0002C05D File Offset: 0x0002A25D
	public virtual Vector3 GetInheritedVelocity(BasePlayer player, Vector3 direction)
	{
		return Vector3.zero;
	}

	// Token: 0x0600213C RID: 8508 RVA: 0x00029EBC File Offset: 0x000280BC
	public virtual float AmmoFraction()
	{
		return 0f;
	}

	// Token: 0x0600213D RID: 8509 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool CanReload()
	{
		return false;
	}

	// Token: 0x0600213E RID: 8510 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool ServerIsReloading()
	{
		return false;
	}

	// Token: 0x0600213F RID: 8511 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ServerReload()
	{
	}

	// Token: 0x06002140 RID: 8512 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void TopUpAmmo()
	{
	}

	// Token: 0x06002141 RID: 8513 RVA: 0x00036ECC File Offset: 0x000350CC
	public virtual Vector3 ModifyAIAim(Vector3 eulerInput, float swayModifier = 1f)
	{
		return eulerInput;
	}

	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x06002142 RID: 8514 RVA: 0x000DA378 File Offset: 0x000D8578
	protected bool UsingInfiniteAmmoCheat
	{
		get
		{
			BasePlayer ownerPlayer = base.GetOwnerPlayer();
			return !(ownerPlayer == null) && (ownerPlayer.IsAdmin || ownerPlayer.IsDeveloper) && ownerPlayer.GetInfoBool("player.infiniteammo", false);
		}
	}

	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x06002143 RID: 8515 RVA: 0x000DA3B3 File Offset: 0x000D85B3
	public float NextAttackTime
	{
		get
		{
			return this.nextAttackTime;
		}
	}

	// Token: 0x06002144 RID: 8516 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void GetAttackStats(HitInfo info)
	{
	}

	// Token: 0x06002145 RID: 8517 RVA: 0x000DA3BB File Offset: 0x000D85BB
	protected void StartAttackCooldownRaw(float cooldown)
	{
		this.nextAttackTime = UnityEngine.Time.time + cooldown;
	}

	// Token: 0x06002146 RID: 8518 RVA: 0x000DA3CA File Offset: 0x000D85CA
	protected void StartAttackCooldown(float cooldown)
	{
		this.nextAttackTime = this.CalculateCooldownTime(this.nextAttackTime, cooldown, true);
	}

	// Token: 0x06002147 RID: 8519 RVA: 0x000DA3E0 File Offset: 0x000D85E0
	public void ResetAttackCooldown()
	{
		this.nextAttackTime = float.NegativeInfinity;
	}

	// Token: 0x06002148 RID: 8520 RVA: 0x000DA3ED File Offset: 0x000D85ED
	public bool HasAttackCooldown()
	{
		return UnityEngine.Time.time < this.nextAttackTime;
	}

	// Token: 0x06002149 RID: 8521 RVA: 0x000DA3FC File Offset: 0x000D85FC
	protected float GetAttackCooldown()
	{
		return Mathf.Max(this.nextAttackTime - UnityEngine.Time.time, 0f);
	}

	// Token: 0x0600214A RID: 8522 RVA: 0x000DA414 File Offset: 0x000D8614
	protected float GetAttackIdle()
	{
		return Mathf.Max(UnityEngine.Time.time - this.nextAttackTime, 0f);
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x000DA42C File Offset: 0x000D862C
	protected float CalculateCooldownTime(float nextTime, float cooldown, bool catchup)
	{
		float time = UnityEngine.Time.time;
		float num = 0f;
		if (base.isServer)
		{
			BasePlayer ownerPlayer = base.GetOwnerPlayer();
			num += 0.1f;
			num += cooldown * 0.1f;
			num += (ownerPlayer ? ownerPlayer.desyncTimeClamped : 0.1f);
			num += Mathf.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime);
		}
		if (nextTime < 0f)
		{
			nextTime = Mathf.Max(0f, time + cooldown - num);
		}
		else if (time - nextTime <= num)
		{
			nextTime = Mathf.Min(nextTime + cooldown, time + cooldown);
		}
		else
		{
			nextTime = Mathf.Max(nextTime + cooldown, time + cooldown - num);
		}
		return nextTime;
	}

	// Token: 0x0600214C RID: 8524 RVA: 0x000DA4D0 File Offset: 0x000D86D0
	protected bool VerifyClientRPC(BasePlayer player)
	{
		if (player == null)
		{
			Debug.LogWarning("Received RPC from null player");
			return false;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Owner not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "owner_missing");
			return false;
		}
		if (ownerPlayer != player)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Player mismatch (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_mismatch");
			return false;
		}
		if (player.IsDead())
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Player dead (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_dead");
			return false;
		}
		if (player.IsWounded())
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Player down (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_down");
			return false;
		}
		if (player.IsSleeping())
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Player sleeping (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_sleeping");
			return false;
		}
		if (player.desyncTimeRaw > ConVar.AntiHack.maxdesync)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, string.Concat(new object[] { "Player stalled (", base.ShortPrefabName, " with ", player.desyncTimeRaw, "s)" }));
			player.stats.combat.LogInvalid(player, this, "player_stalled");
			return false;
		}
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Item not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "item_missing");
			return false;
		}
		if (ownerItem.isBroken)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Item broken (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "item_broken");
			return false;
		}
		return true;
	}

	// Token: 0x0600214D RID: 8525 RVA: 0x000DA710 File Offset: 0x000D8910
	protected virtual bool VerifyClientAttack(BasePlayer player)
	{
		if (!this.VerifyClientRPC(player))
		{
			return false;
		}
		if (this.HasAttackCooldown())
		{
			global::AntiHack.Log(player, AntiHackType.CooldownHack, string.Concat(new object[]
			{
				"T-",
				this.GetAttackCooldown(),
				"s (",
				base.ShortPrefabName,
				")"
			}));
			player.stats.combat.LogInvalid(player, this, "attack_cooldown");
			return false;
		}
		return true;
	}

	// Token: 0x0600214E RID: 8526 RVA: 0x000DA78C File Offset: 0x000D898C
	protected bool ValidateEyePos(BasePlayer player, Vector3 eyePos)
	{
		bool flag = true;
		if (eyePos.IsNaNOrInfinity())
		{
			string shortPrefabName = base.ShortPrefabName;
			global::AntiHack.Log(player, AntiHackType.EyeHack, "Contains NaN (" + shortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "eye_nan");
			flag = false;
		}
		if (ConVar.AntiHack.eye_protection > 0)
		{
			float num = 1f + ConVar.AntiHack.eye_forgiveness;
			float eye_clientframes = ConVar.AntiHack.eye_clientframes;
			float eye_serverframes = ConVar.AntiHack.eye_serverframes;
			float num2 = eye_clientframes / 60f;
			float num3 = eye_serverframes * Mathx.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime, UnityEngine.Time.fixedDeltaTime);
			float num4 = (player.desyncTimeClamped + num2 + num3) * num;
			int num5 = (ConVar.AntiHack.eye_terraincheck ? 10551296 : 2162688);
			if (ConVar.AntiHack.eye_protection >= 1)
			{
				float num6 = player.MaxVelocity() + player.GetParentVelocity().magnitude;
				float num7 = player.BoundsPadding() + num4 * num6;
				float num8 = Vector3.Distance(player.eyes.position, eyePos);
				if (num8 > num7)
				{
					string shortPrefabName2 = base.ShortPrefabName;
					global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[] { "Distance (", shortPrefabName2, " on attack with ", num8, "m > ", num7, "m)" }));
					player.stats.combat.LogInvalid(player, this, "eye_distance");
					flag = false;
				}
			}
			if (ConVar.AntiHack.eye_protection >= 3)
			{
				float num9 = Mathf.Abs(player.GetMountVelocity().y + player.GetParentVelocity().y);
				float num10 = player.BoundsPadding() + num4 * num9 + player.GetJumpHeight();
				float num11 = Mathf.Abs(player.eyes.position.y - eyePos.y);
				if (num11 > num10)
				{
					string shortPrefabName3 = base.ShortPrefabName;
					global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[] { "Altitude (", shortPrefabName3, " on attack with ", num11, "m > ", num10, "m)" }));
					player.stats.combat.LogInvalid(player, this, "eye_altitude");
					flag = false;
				}
			}
			if (ConVar.AntiHack.eye_protection >= 2)
			{
				Vector3 center = player.eyes.center;
				Vector3 position = player.eyes.position;
				if (!GamePhysics.LineOfSightRadius(center, position, num5, ConVar.AntiHack.eye_losradius, null) || !GamePhysics.LineOfSightRadius(position, eyePos, num5, ConVar.AntiHack.eye_losradius, null))
				{
					string shortPrefabName4 = base.ShortPrefabName;
					global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[] { "Line of sight (", shortPrefabName4, " on attack) ", center, " ", position, " ", eyePos }));
					player.stats.combat.LogInvalid(player, this, "eye_los");
					flag = false;
				}
			}
			if (ConVar.AntiHack.eye_protection >= 4 && !player.HasParent())
			{
				Vector3 position2 = player.eyes.position;
				float num12 = Vector3.Distance(position2, eyePos);
				Collider collider;
				if (num12 > ConVar.AntiHack.eye_noclip_cutoff)
				{
					if (global::AntiHack.TestNoClipping(position2, eyePos, player.NoClipRadius(ConVar.AntiHack.eye_noclip_margin), ConVar.AntiHack.eye_noclip_backtracking, ConVar.AntiHack.noclip_protection >= 2, out collider, false, null))
					{
						string shortPrefabName5 = base.ShortPrefabName;
						global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[] { "NoClip (", shortPrefabName5, " on attack) ", position2, " ", eyePos }));
						player.stats.combat.LogInvalid(player, this, "eye_noclip");
						flag = false;
					}
				}
				else if (num12 > 0.01f && global::AntiHack.TestNoClipping(position2, eyePos, 0.01f, ConVar.AntiHack.eye_noclip_backtracking, ConVar.AntiHack.noclip_protection >= 2, out collider, false, null))
				{
					string shortPrefabName6 = base.ShortPrefabName;
					global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[] { "NoClip (", shortPrefabName6, " on attack) ", position2, " ", eyePos }));
					player.stats.combat.LogInvalid(player, this, "eye_noclip");
					flag = false;
				}
			}
			if (!flag)
			{
				global::AntiHack.AddViolation(player, AntiHackType.EyeHack, ConVar.AntiHack.eye_penalty);
			}
			else if (ConVar.AntiHack.eye_protection >= 5 && !player.HasParent() && !player.isMounted)
			{
				player.eyeHistory.PushBack(eyePos);
			}
		}
		return flag;
	}

	// Token: 0x0600214F RID: 8527 RVA: 0x000DAC29 File Offset: 0x000D8E29
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		this.StartAttackCooldown(this.deployDelay * 0.9f);
	}

	// Token: 0x040019F7 RID: 6647
	[Header("Attack Entity")]
	public float deployDelay = 1f;

	// Token: 0x040019F8 RID: 6648
	public float repeatDelay = 0.5f;

	// Token: 0x040019F9 RID: 6649
	public float animationDelay;

	// Token: 0x040019FA RID: 6650
	[Header("NPCUsage")]
	public float effectiveRange = 1f;

	// Token: 0x040019FB RID: 6651
	public float npcDamageScale = 1f;

	// Token: 0x040019FC RID: 6652
	public float attackLengthMin = -1f;

	// Token: 0x040019FD RID: 6653
	public float attackLengthMax = -1f;

	// Token: 0x040019FE RID: 6654
	public float attackSpacing;

	// Token: 0x040019FF RID: 6655
	public float aiAimSwayOffset;

	// Token: 0x04001A00 RID: 6656
	public float aiAimCone;

	// Token: 0x04001A01 RID: 6657
	public bool aiOnlyInRange;

	// Token: 0x04001A02 RID: 6658
	public float CloseRangeAddition;

	// Token: 0x04001A03 RID: 6659
	public float MediumRangeAddition;

	// Token: 0x04001A04 RID: 6660
	public float LongRangeAddition;

	// Token: 0x04001A05 RID: 6661
	public bool CanUseAtMediumRange = true;

	// Token: 0x04001A06 RID: 6662
	public bool CanUseAtLongRange = true;

	// Token: 0x04001A07 RID: 6663
	public SoundDefinition[] reloadSounds;

	// Token: 0x04001A08 RID: 6664
	public SoundDefinition thirdPersonMeleeSound;

	// Token: 0x04001A09 RID: 6665
	[Header("Recoil Compensation")]
	public float recoilCompDelayOverride;

	// Token: 0x04001A0A RID: 6666
	public bool wantsRecoilComp;

	// Token: 0x04001A0B RID: 6667
	private float nextAttackTime = float.NegativeInfinity;
}
