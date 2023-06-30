using System;

// Token: 0x020005F0 RID: 1520
public class ItemModContainerRestriction : ItemMod
{
	// Token: 0x06002D99 RID: 11673 RVA: 0x00112FD1 File Offset: 0x001111D1
	public bool CanExistWith(ItemModContainerRestriction other)
	{
		return other == null || (this.slotFlags & other.slotFlags) == (ItemModContainerRestriction.SlotFlags)0;
	}

	// Token: 0x04002543 RID: 9539
	[InspectorFlags]
	public ItemModContainerRestriction.SlotFlags slotFlags;

	// Token: 0x02000D9C RID: 3484
	[Flags]
	public enum SlotFlags
	{
		// Token: 0x040048A7 RID: 18599
		Map = 1
	}
}
