using System;

// Token: 0x020003B3 RID: 947
public class FlintStrikeWeapon : BaseProjectile
{
	// Token: 0x06002157 RID: 8535 RVA: 0x000DAD03 File Offset: 0x000D8F03
	public override RecoilProperties GetRecoil()
	{
		return this.strikeRecoil;
	}

	// Token: 0x04001A0F RID: 6671
	public float successFraction = 0.5f;

	// Token: 0x04001A10 RID: 6672
	public RecoilProperties strikeRecoil;
}
