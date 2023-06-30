using System;
using UnityEngine;

// Token: 0x02000937 RID: 2359
public class FixedRateStepped
{
	// Token: 0x06003884 RID: 14468 RVA: 0x001505B8 File Offset: 0x0014E7B8
	public bool ShouldStep()
	{
		if (this.nextCall > Time.time)
		{
			return false;
		}
		if (this.nextCall == 0f)
		{
			this.nextCall = Time.time;
		}
		if (this.nextCall + this.rate * (float)this.maxSteps < Time.time)
		{
			this.nextCall = Time.time - this.rate * (float)this.maxSteps;
		}
		this.nextCall += this.rate;
		return true;
	}

	// Token: 0x040033A2 RID: 13218
	public float rate = 0.1f;

	// Token: 0x040033A3 RID: 13219
	public int maxSteps = 3;

	// Token: 0x040033A4 RID: 13220
	internal float nextCall;
}
