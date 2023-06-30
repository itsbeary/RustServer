using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000958 RID: 2392
public struct Timing
{
	// Token: 0x06003957 RID: 14679 RVA: 0x001542AF File Offset: 0x001524AF
	public static Timing Start(string name)
	{
		return new Timing(name);
	}

	// Token: 0x06003958 RID: 14680 RVA: 0x001542B8 File Offset: 0x001524B8
	public void End()
	{
		if (this.sw.Elapsed.TotalSeconds > 0.30000001192092896)
		{
			UnityEngine.Debug.Log("[" + this.sw.Elapsed.TotalSeconds.ToString("0.0") + "s] " + this.name);
		}
	}

	// Token: 0x06003959 RID: 14681 RVA: 0x0015431D File Offset: 0x0015251D
	public Timing(string name)
	{
		this.sw = Stopwatch.StartNew();
		this.name = name;
	}

	// Token: 0x040033EC RID: 13292
	private Stopwatch sw;

	// Token: 0x040033ED RID: 13293
	private string name;
}
