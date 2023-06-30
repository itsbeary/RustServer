using System;

// Token: 0x020005E2 RID: 1506
public class ItemModCassetteContainer : ItemModContainer
{
	// Token: 0x170003C0 RID: 960
	// (get) Token: 0x06002D72 RID: 11634 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool ForceAcceptItemCheck
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002D73 RID: 11635 RVA: 0x00112624 File Offset: 0x00110824
	protected override void SetAllowedItems(ItemContainer container)
	{
		container.SetOnlyAllowedItems(this.CassetteItems);
	}

	// Token: 0x0400251D RID: 9501
	public ItemDefinition[] CassetteItems;
}
