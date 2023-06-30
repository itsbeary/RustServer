using System;
using Network;

// Token: 0x02000952 RID: 2386
public struct RealTimeSinceEx
{
	// Token: 0x06003930 RID: 14640 RVA: 0x00153B54 File Offset: 0x00151D54
	public static implicit operator double(RealTimeSinceEx ts)
	{
		return TimeEx.realtimeSinceStartup - ts.time;
	}

	// Token: 0x06003931 RID: 14641 RVA: 0x00153B64 File Offset: 0x00151D64
	public static implicit operator RealTimeSinceEx(double ts)
	{
		return new RealTimeSinceEx
		{
			time = TimeEx.realtimeSinceStartup - ts
		};
	}

	// Token: 0x06003932 RID: 14642 RVA: 0x00153B88 File Offset: 0x00151D88
	public override string ToString()
	{
		return (TimeEx.realtimeSinceStartup - this.time).ToString();
	}

	// Token: 0x040033D8 RID: 13272
	private double time;
}
