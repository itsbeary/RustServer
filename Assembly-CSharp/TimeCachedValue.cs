using System;
using UnityEngine;

// Token: 0x02000957 RID: 2391
public class TimeCachedValue<T>
{
	// Token: 0x06003954 RID: 14676 RVA: 0x00154210 File Offset: 0x00152410
	public T Get(bool force)
	{
		if (this.cooldown < this.refreshCooldown && !force && this.hasRun && !this.forceNextRun)
		{
			return this.cachedValue;
		}
		this.hasRun = true;
		this.forceNextRun = false;
		this.cooldown = 0f - UnityEngine.Random.Range(0f, this.refreshRandomRange);
		if (this.updateValue != null)
		{
			this.cachedValue = this.updateValue();
		}
		else
		{
			this.cachedValue = default(T);
		}
		return this.cachedValue;
	}

	// Token: 0x06003955 RID: 14677 RVA: 0x001542A6 File Offset: 0x001524A6
	public void ForceNextRun()
	{
		this.forceNextRun = true;
	}

	// Token: 0x040033E5 RID: 13285
	public float refreshCooldown;

	// Token: 0x040033E6 RID: 13286
	public float refreshRandomRange;

	// Token: 0x040033E7 RID: 13287
	public Func<T> updateValue;

	// Token: 0x040033E8 RID: 13288
	private T cachedValue;

	// Token: 0x040033E9 RID: 13289
	private TimeSince cooldown;

	// Token: 0x040033EA RID: 13290
	private bool hasRun;

	// Token: 0x040033EB RID: 13291
	private bool forceNextRun;
}
