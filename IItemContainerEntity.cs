using System;
using UnityEngine;

// Token: 0x020003FB RID: 1019
public interface IItemContainerEntity : IIdealSlotEntity, ILootableEntity
{
	// Token: 0x170002E9 RID: 745
	// (get) Token: 0x0600230A RID: 8970
	ItemContainer inventory { get; }

	// Token: 0x170002EA RID: 746
	// (get) Token: 0x0600230B RID: 8971
	Transform Transform { get; }

	// Token: 0x170002EB RID: 747
	// (get) Token: 0x0600230C RID: 8972
	bool DropsLoot { get; }

	// Token: 0x170002EC RID: 748
	// (get) Token: 0x0600230D RID: 8973
	float DestroyLootPercent { get; }

	// Token: 0x170002ED RID: 749
	// (get) Token: 0x0600230E RID: 8974
	bool DropFloats { get; }

	// Token: 0x0600230F RID: 8975
	void DropItems(BaseEntity initiator = null);

	// Token: 0x06002310 RID: 8976
	bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true);

	// Token: 0x06002311 RID: 8977
	bool ShouldDropItemsIndividually();

	// Token: 0x06002312 RID: 8978
	void DropBonusItems(BaseEntity initiator, ItemContainer container);

	// Token: 0x06002313 RID: 8979
	Vector3 GetDropPosition();
}
