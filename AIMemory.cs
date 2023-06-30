using System;
using UnityEngine;

// Token: 0x02000367 RID: 871
public class AIMemory
{
	// Token: 0x06001FD4 RID: 8148 RVA: 0x000D68FC File Offset: 0x000D4AFC
	public void Clear()
	{
		this.Entity.Clear();
		this.Position.Clear();
		this.AIPoint.Clear();
	}

	// Token: 0x04001932 RID: 6450
	public AIMemoryBank<BaseEntity> Entity = new AIMemoryBank<BaseEntity>(MemoryBankType.Entity, 8);

	// Token: 0x04001933 RID: 6451
	public AIMemoryBank<Vector3> Position = new AIMemoryBank<Vector3>(MemoryBankType.Position, 8);

	// Token: 0x04001934 RID: 6452
	public AIMemoryBank<AIPoint> AIPoint = new AIMemoryBank<AIPoint>(MemoryBankType.AIPoint, 8);
}
