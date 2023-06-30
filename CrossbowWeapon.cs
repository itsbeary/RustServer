using System;

// Token: 0x020003B2 RID: 946
public class CrossbowWeapon : BaseProjectile
{
	// Token: 0x06002154 RID: 8532 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ForceSendMagazine(BaseNetworkable.SaveInfo saveInfo)
	{
		return true;
	}

	// Token: 0x06002155 RID: 8533 RVA: 0x000DACFA File Offset: 0x000D8EFA
	public override void DidAttackServerside()
	{
		base.SendNetworkUpdateImmediate(false);
	}
}
