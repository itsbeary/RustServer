using System;
using UnityEngine;

// Token: 0x020001AF RID: 431
public class RuntimeInterestNode : IAIPathInterestNode
{
	// Token: 0x17000216 RID: 534
	// (get) Token: 0x060018E8 RID: 6376 RVA: 0x000B89E6 File Offset: 0x000B6BE6
	// (set) Token: 0x060018E9 RID: 6377 RVA: 0x000B89EE File Offset: 0x000B6BEE
	public Vector3 Position { get; set; }

	// Token: 0x17000217 RID: 535
	// (get) Token: 0x060018EA RID: 6378 RVA: 0x000B89F7 File Offset: 0x000B6BF7
	// (set) Token: 0x060018EB RID: 6379 RVA: 0x000B89FF File Offset: 0x000B6BFF
	public float NextVisitTime { get; set; }

	// Token: 0x060018EC RID: 6380 RVA: 0x000B8A08 File Offset: 0x000B6C08
	public RuntimeInterestNode(Vector3 position)
	{
		this.Position = position;
	}
}
