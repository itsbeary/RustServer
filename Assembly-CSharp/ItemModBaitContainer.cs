using System;

// Token: 0x020005DF RID: 1503
public class ItemModBaitContainer : ItemModContainer
{
	// Token: 0x170003BF RID: 959
	// (get) Token: 0x06002D6A RID: 11626 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool ForceAcceptItemCheck
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002D6B RID: 11627 RVA: 0x00112450 File Offset: 0x00110650
	protected override bool CanAcceptItem(Item item, int count)
	{
		ItemModCompostable component = item.info.GetComponent<ItemModCompostable>();
		return component != null && component.BaitValue > 0f;
	}

	// Token: 0x06002D6C RID: 11628 RVA: 0x00112481 File Offset: 0x00110681
	protected override void SetAllowedItems(ItemContainer container)
	{
		FishLookup.LoadFish();
		container.SetOnlyAllowedItems(FishLookup.BaitItems);
	}
}
