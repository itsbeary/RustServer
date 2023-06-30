using System;
using UnityEngine;

// Token: 0x020002E2 RID: 738
public class QueryVis : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04001740 RID: 5952
	public Collider checkCollider;

	// Token: 0x04001741 RID: 5953
	private CoverageQueries.Query query;

	// Token: 0x04001742 RID: 5954
	public CoverageQueries.RadiusSpace coverageRadiusSpace = CoverageQueries.RadiusSpace.World;

	// Token: 0x04001743 RID: 5955
	public float coverageRadius = 0.2f;
}
