using System;

// Token: 0x0200071C RID: 1820
[Serializable]
public struct SubsurfaceScatteringParams
{
	// Token: 0x040029C8 RID: 10696
	public bool enabled;

	// Token: 0x040029C9 RID: 10697
	public bool halfResolution;

	// Token: 0x040029CA RID: 10698
	public float radiusScale;

	// Token: 0x040029CB RID: 10699
	public static SubsurfaceScatteringParams Default = new SubsurfaceScatteringParams
	{
		enabled = true,
		halfResolution = true,
		radiusScale = 1f
	};

	// Token: 0x02000E4C RID: 3660
	public enum Quality
	{
		// Token: 0x04004B57 RID: 19287
		Low,
		// Token: 0x04004B58 RID: 19288
		Medium,
		// Token: 0x04004B59 RID: 19289
		High
	}
}
