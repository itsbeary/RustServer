using System;

// Token: 0x020004DB RID: 1243
public interface IIndustrialStorage
{
	// Token: 0x1700036C RID: 876
	// (get) Token: 0x06002876 RID: 10358
	ItemContainer Container { get; }

	// Token: 0x06002877 RID: 10359
	Vector2i InputSlotRange(int slotIndex);

	// Token: 0x06002878 RID: 10360
	Vector2i OutputSlotRange(int slotIndex);

	// Token: 0x06002879 RID: 10361
	void OnStorageItemTransferBegin();

	// Token: 0x0600287A RID: 10362
	void OnStorageItemTransferEnd();

	// Token: 0x1700036D RID: 877
	// (get) Token: 0x0600287B RID: 10363
	BaseEntity IndustrialEntity { get; }
}
