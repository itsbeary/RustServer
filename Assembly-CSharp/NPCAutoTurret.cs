using System;
using UnityEngine;

// Token: 0x0200040B RID: 1035
public class NPCAutoTurret : AutoTurret
{
	// Token: 0x06002352 RID: 9042 RVA: 0x000E22AC File Offset: 0x000E04AC
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetOnline();
		base.SetPeacekeepermode(true);
	}

	// Token: 0x06002353 RID: 9043 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool HasAmmo()
	{
		return true;
	}

	// Token: 0x06002354 RID: 9044 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool CheckPeekers()
	{
		return false;
	}

	// Token: 0x06002355 RID: 9045 RVA: 0x000E22C1 File Offset: 0x000E04C1
	public override float TargetScanRate()
	{
		return 1.25f;
	}

	// Token: 0x06002356 RID: 9046 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return true;
	}

	// Token: 0x06002357 RID: 9047 RVA: 0x00070DF8 File Offset: 0x0006EFF8
	public override float GetMaxAngleForEngagement()
	{
		return 15f;
	}

	// Token: 0x06002358 RID: 9048 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool HasFallbackWeapon()
	{
		return true;
	}

	// Token: 0x06002359 RID: 9049 RVA: 0x000E22C8 File Offset: 0x000E04C8
	public override Transform GetCenterMuzzle()
	{
		return this.centerMuzzle;
	}

	// Token: 0x0600235A RID: 9050 RVA: 0x000E22D0 File Offset: 0x000E04D0
	public override void FireGun(Vector3 targetPos, float aimCone, Transform muzzleToUse = null, BaseCombatEntity target = null)
	{
		muzzleToUse = this.muzzleRight;
		base.FireGun(targetPos, aimCone, muzzleToUse, target);
	}

	// Token: 0x0600235B RID: 9051 RVA: 0x000E22E5 File Offset: 0x000E04E5
	protected override bool Ignore(BasePlayer player)
	{
		return player is ScientistNPC || player is BanditGuard;
	}

	// Token: 0x0600235C RID: 9052 RVA: 0x000E22FC File Offset: 0x000E04FC
	public override bool IsEntityHostile(BaseCombatEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (basePlayer != null)
		{
			if (basePlayer.IsNpc)
			{
				return !(basePlayer is ScientistNPC) && !(basePlayer is BanditGuard) && !(basePlayer is NPCShopKeeper) && (!(basePlayer is BasePet) || base.IsEntityHostile(basePlayer));
			}
			if (basePlayer.IsSleeping() && basePlayer.secondsSleeping >= NPCAutoTurret.sleeperhostiledelay)
			{
				return true;
			}
		}
		return base.IsEntityHostile(ent);
	}

	// Token: 0x04001B30 RID: 6960
	public Transform centerMuzzle;

	// Token: 0x04001B31 RID: 6961
	public Transform muzzleLeft;

	// Token: 0x04001B32 RID: 6962
	public Transform muzzleRight;

	// Token: 0x04001B33 RID: 6963
	private bool useLeftMuzzle;

	// Token: 0x04001B34 RID: 6964
	[ServerVar(Help = "How many seconds until a sleeping player is considered hostile")]
	public static float sleeperhostiledelay = 1200f;
}
