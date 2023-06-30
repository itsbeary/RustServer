using System;

// Token: 0x020003BF RID: 959
public static class BaseNetworkableEx
{
	// Token: 0x060021B9 RID: 8633 RVA: 0x000DC1DE File Offset: 0x000DA3DE
	public static bool IsValid(this BaseNetworkable ent)
	{
		return !(ent == null) && ent.net != null;
	}

	// Token: 0x060021BA RID: 8634 RVA: 0x000DC1F6 File Offset: 0x000DA3F6
	public static bool IsRealNull(this BaseNetworkable ent)
	{
		return ent == null;
	}
}
