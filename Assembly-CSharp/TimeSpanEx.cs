using System;

// Token: 0x0200092D RID: 2349
public static class TimeSpanEx
{
	// Token: 0x06003858 RID: 14424 RVA: 0x0014F4D1 File Offset: 0x0014D6D1
	public static string ToShortString(this TimeSpan timeSpan)
	{
		return string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
	}

	// Token: 0x06003859 RID: 14425 RVA: 0x0014F502 File Offset: 0x0014D702
	public static string ToShortStringNoHours(this TimeSpan timeSpan)
	{
		return string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
	}
}
