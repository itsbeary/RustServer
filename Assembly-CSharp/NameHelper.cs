using System;

// Token: 0x0200075B RID: 1883
public static class NameHelper
{
	// Token: 0x06003477 RID: 13431 RVA: 0x00036ECC File Offset: 0x000350CC
	public static string Get(ulong userId, string name, bool isClient = true)
	{
		return name;
	}

	// Token: 0x06003478 RID: 13432 RVA: 0x00144395 File Offset: 0x00142595
	public static string Get(IPlayerInfo playerInfo, bool isClient = true)
	{
		return NameHelper.Get(playerInfo.UserId, playerInfo.UserName, isClient);
	}
}
