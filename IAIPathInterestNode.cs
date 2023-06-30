using System;
using UnityEngine;

// Token: 0x020001AC RID: 428
public interface IAIPathInterestNode
{
	// Token: 0x17000212 RID: 530
	// (get) Token: 0x060018DC RID: 6364
	Vector3 Position { get; }

	// Token: 0x17000213 RID: 531
	// (get) Token: 0x060018DD RID: 6365
	// (set) Token: 0x060018DE RID: 6366
	float NextVisitTime { get; set; }
}
