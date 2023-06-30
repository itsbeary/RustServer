using System;

// Token: 0x02000189 RID: 393
public class SnowballGun : BaseProjectile
{
	// Token: 0x060017F0 RID: 6128 RVA: 0x000B43DC File Offset: 0x000B25DC
	protected override void ReloadMagazine(int desiredAmount = -1)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		desiredAmount = 1;
		this.primaryMagazine.Reload(ownerPlayer, desiredAmount, this.CanRefundAmmo);
		this.primaryMagazine.contents = this.primaryMagazine.capacity;
		this.primaryMagazine.ammoType = this.OverrideProjectile;
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x060017F1 RID: 6129 RVA: 0x000B4453 File Offset: 0x000B2653
	protected override ItemDefinition PrimaryMagazineAmmo
	{
		get
		{
			if (!(this.OverrideProjectile != null))
			{
				return base.PrimaryMagazineAmmo;
			}
			return this.OverrideProjectile;
		}
	}

	// Token: 0x17000203 RID: 515
	// (get) Token: 0x060017F2 RID: 6130 RVA: 0x00007A44 File Offset: 0x00005C44
	protected override bool CanRefundAmmo
	{
		get
		{
			return false;
		}
	}

	// Token: 0x040010B1 RID: 4273
	public ItemDefinition OverrideProjectile;
}
