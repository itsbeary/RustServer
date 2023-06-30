using System;

// Token: 0x02000525 RID: 1317
public static class HitAreaUtil
{
	// Token: 0x06002A03 RID: 10755 RVA: 0x00101893 File Offset: 0x000FFA93
	public static string Format(HitArea area)
	{
		if (area == (HitArea)0)
		{
			return "None";
		}
		if (area == (HitArea)(-1))
		{
			return "Generic";
		}
		return area.ToString();
	}
}
