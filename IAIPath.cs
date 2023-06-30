using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A9 RID: 425
public interface IAIPath
{
	// Token: 0x1700020D RID: 525
	// (get) Token: 0x060018CE RID: 6350
	IEnumerable<IAIPathSpeedZone> SpeedZones { get; }

	// Token: 0x1700020E RID: 526
	// (get) Token: 0x060018CF RID: 6351
	IEnumerable<IAIPathInterestNode> InterestNodes { get; }

	// Token: 0x060018D0 RID: 6352
	void GetNodesNear(Vector3 point, ref List<IAIPathNode> nearNodes, float dist = 10f);

	// Token: 0x060018D1 RID: 6353
	IAIPathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f);

	// Token: 0x060018D2 RID: 6354
	IAIPathNode GetClosestToPoint(Vector3 point);

	// Token: 0x060018D3 RID: 6355
	void AddInterestNode(IAIPathInterestNode interestNode);

	// Token: 0x060018D4 RID: 6356
	void AddSpeedZone(IAIPathSpeedZone speedZone);
}
