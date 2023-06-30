using System;

// Token: 0x020003FC RID: 1020
public interface IIdealSlotEntity
{
	// Token: 0x06002314 RID: 8980
	int GetIdealSlot(BasePlayer player, Item item);

	// Token: 0x06002315 RID: 8981
	ItemContainerId GetIdealContainer(BasePlayer player, Item item, bool altMove);
}
