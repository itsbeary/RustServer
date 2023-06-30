using System;

// Token: 0x02000178 RID: 376
public class CoverageQueryFlare : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04001070 RID: 4208
	public bool isDynamic;

	// Token: 0x04001071 RID: 4209
	public bool timeShimmer;

	// Token: 0x04001072 RID: 4210
	public bool positionalShimmer;

	// Token: 0x04001073 RID: 4211
	public bool rotate;

	// Token: 0x04001074 RID: 4212
	public float maxVisibleDistance = 30f;

	// Token: 0x04001075 RID: 4213
	public bool lightScaled;

	// Token: 0x04001076 RID: 4214
	public float dotMin = -1f;

	// Token: 0x04001077 RID: 4215
	public float dotMax = -1f;

	// Token: 0x04001078 RID: 4216
	public CoverageQueries.RadiusSpace coverageRadiusSpace;

	// Token: 0x04001079 RID: 4217
	public float coverageRadius = 0.01f;

	// Token: 0x0400107A RID: 4218
	public LODDistanceMode DistanceMode;
}
