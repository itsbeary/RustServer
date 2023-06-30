using System;
using System.Collections.Generic;
using ProtoBuf;

// Token: 0x020005CB RID: 1483
public class ItemCraftTask
{
	// Token: 0x0400246E RID: 9326
	public ItemBlueprint blueprint;

	// Token: 0x0400246F RID: 9327
	public float endTime;

	// Token: 0x04002470 RID: 9328
	public int taskUID;

	// Token: 0x04002471 RID: 9329
	public global::BasePlayer owner;

	// Token: 0x04002472 RID: 9330
	public bool cancelled;

	// Token: 0x04002473 RID: 9331
	public ProtoBuf.Item.InstanceData instanceData;

	// Token: 0x04002474 RID: 9332
	public int amount = 1;

	// Token: 0x04002475 RID: 9333
	public int skinID;

	// Token: 0x04002476 RID: 9334
	public List<ulong> potentialOwners;

	// Token: 0x04002477 RID: 9335
	public List<global::Item> takenItems;

	// Token: 0x04002478 RID: 9336
	public int numCrafted;

	// Token: 0x04002479 RID: 9337
	public float conditionScale = 1f;

	// Token: 0x0400247A RID: 9338
	public float workSecondsComplete;

	// Token: 0x0400247B RID: 9339
	public float worksecondsRequired;

	// Token: 0x0400247C RID: 9340
	public global::BaseEntity workbenchEntity;
}
