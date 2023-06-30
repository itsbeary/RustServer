using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001AA RID: 426
public interface IAIPathNode
{
	// Token: 0x1700020F RID: 527
	// (get) Token: 0x060018D5 RID: 6357
	Vector3 Position { get; }

	// Token: 0x17000210 RID: 528
	// (get) Token: 0x060018D6 RID: 6358
	bool Straightaway { get; }

	// Token: 0x17000211 RID: 529
	// (get) Token: 0x060018D7 RID: 6359
	IEnumerable<IAIPathNode> Linked { get; }

	// Token: 0x060018D8 RID: 6360
	bool IsValid();

	// Token: 0x060018D9 RID: 6361
	void AddLink(IAIPathNode link);
}
