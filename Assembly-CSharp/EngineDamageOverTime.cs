using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004B2 RID: 1202
public class EngineDamageOverTime
{
	// Token: 0x06002776 RID: 10102 RVA: 0x000F6D6C File Offset: 0x000F4F6C
	public EngineDamageOverTime(float triggerDamage, float maxSeconds, Action trigger)
	{
		this.triggerDamage = triggerDamage;
		this.maxSeconds = maxSeconds;
		this.trigger = trigger;
	}

	// Token: 0x06002777 RID: 10103 RVA: 0x000F6D94 File Offset: 0x000F4F94
	public void TakeDamage(float amount)
	{
		this.recentDamage.Add(new EngineDamageOverTime.RecentDamage(Time.time, amount));
		if (this.GetRecentDamage() > this.triggerDamage)
		{
			this.trigger();
			this.recentDamage.Clear();
		}
	}

	// Token: 0x06002778 RID: 10104 RVA: 0x000F6DD0 File Offset: 0x000F4FD0
	private float GetRecentDamage()
	{
		float num = 0f;
		int i;
		for (i = this.recentDamage.Count - 1; i >= 0; i--)
		{
			EngineDamageOverTime.RecentDamage recentDamage = this.recentDamage[i];
			if (Time.time > recentDamage.time + this.maxSeconds)
			{
				break;
			}
			num += recentDamage.amount;
		}
		if (i > 0)
		{
			this.recentDamage.RemoveRange(0, i + 1);
		}
		return num;
	}

	// Token: 0x04001FF5 RID: 8181
	private readonly List<EngineDamageOverTime.RecentDamage> recentDamage = new List<EngineDamageOverTime.RecentDamage>();

	// Token: 0x04001FF6 RID: 8182
	private readonly float maxSeconds;

	// Token: 0x04001FF7 RID: 8183
	private readonly float triggerDamage;

	// Token: 0x04001FF8 RID: 8184
	private readonly Action trigger;

	// Token: 0x02000D27 RID: 3367
	private struct RecentDamage
	{
		// Token: 0x06005075 RID: 20597 RVA: 0x001A9112 File Offset: 0x001A7312
		public RecentDamage(float time, float amount)
		{
			this.time = time;
			this.amount = amount;
		}

		// Token: 0x040046EA RID: 18154
		public readonly float time;

		// Token: 0x040046EB RID: 18155
		public readonly float amount;
	}
}
