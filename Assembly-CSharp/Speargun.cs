using System;
using UnityEngine;

// Token: 0x020003BC RID: 956
public class Speargun : CrossbowWeapon
{
	// Token: 0x06002164 RID: 8548 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ForceSendMagazine(BaseNetworkable.SaveInfo saveInfo)
	{
		return true;
	}

	// Token: 0x06002165 RID: 8549 RVA: 0x000DAE44 File Offset: 0x000D9044
	protected override bool VerifyClientAttack(BasePlayer player)
	{
		return player.WaterFactor() >= 1f && base.VerifyClientAttack(player);
	}

	// Token: 0x06002166 RID: 8550 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool CanBeUsedInWater()
	{
		return true;
	}

	// Token: 0x04001A18 RID: 6680
	public GameObject worldAmmoModel;
}
