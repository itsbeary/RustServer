using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000360 RID: 864
public class Profile
{
	// Token: 0x06001FA0 RID: 8096 RVA: 0x000D5917 File Offset: 0x000D3B17
	public Profile(string cat, string nam, float WarnTime = 1f)
	{
		this.category = cat;
		this.name = nam;
		this.warnTime = WarnTime;
	}

	// Token: 0x06001FA1 RID: 8097 RVA: 0x000D593F File Offset: 0x000D3B3F
	public void Start()
	{
		this.watch.Reset();
		this.watch.Start();
	}

	// Token: 0x06001FA2 RID: 8098 RVA: 0x000D5958 File Offset: 0x000D3B58
	public void Stop()
	{
		this.watch.Stop();
		if ((float)this.watch.Elapsed.Seconds > this.warnTime)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				this.category,
				".",
				this.name,
				": Took ",
				this.watch.Elapsed.Seconds,
				" seconds"
			}));
		}
	}

	// Token: 0x040018E5 RID: 6373
	public Stopwatch watch = new Stopwatch();

	// Token: 0x040018E6 RID: 6374
	public string category;

	// Token: 0x040018E7 RID: 6375
	public string name;

	// Token: 0x040018E8 RID: 6376
	public float warnTime;
}
