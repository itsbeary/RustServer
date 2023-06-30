using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005EF RID: 1519
public class ItemModContainer : ItemMod
{
	// Token: 0x06002D92 RID: 11666 RVA: 0x00112D34 File Offset: 0x00110F34
	public override void OnItemCreated(Item item)
	{
		if (!item.isServer)
		{
			return;
		}
		if (this.capacity <= 0)
		{
			return;
		}
		if (item.contents != null)
		{
			if (this.validItemWhitelist != null && this.validItemWhitelist.Length != 0)
			{
				item.contents.canAcceptItem = new Func<Item, int, bool>(this.CanAcceptItem);
			}
			return;
		}
		item.contents = new ItemContainer();
		item.contents.flags = this.containerFlags;
		item.contents.allowedContents = ((this.onlyAllowedContents == (ItemContainer.ContentsType)0) ? ItemContainer.ContentsType.Generic : this.onlyAllowedContents);
		this.SetAllowedItems(item.contents);
		item.contents.availableSlots = this.availableSlots;
		if ((this.validItemWhitelist != null && this.validItemWhitelist.Length != 0) || this.ForceAcceptItemCheck)
		{
			item.contents.canAcceptItem = new Func<Item, int, bool>(this.CanAcceptItem);
		}
		item.contents.ServerInitialize(item, this.capacity);
		item.contents.maxStackSize = this.maxStackSize;
		item.contents.GiveUID();
	}

	// Token: 0x06002D93 RID: 11667 RVA: 0x00112E3A File Offset: 0x0011103A
	protected virtual void SetAllowedItems(ItemContainer container)
	{
		container.SetOnlyAllowedItem(this.onlyAllowedItemType);
	}

	// Token: 0x170003C1 RID: 961
	// (get) Token: 0x06002D94 RID: 11668 RVA: 0x00007A44 File Offset: 0x00005C44
	protected virtual bool ForceAcceptItemCheck
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002D95 RID: 11669 RVA: 0x00112E48 File Offset: 0x00111048
	protected virtual bool CanAcceptItem(Item item, int count)
	{
		ItemDefinition[] array = this.validItemWhitelist;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].itemid == item.info.itemid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002D96 RID: 11670 RVA: 0x00112E84 File Offset: 0x00111084
	public override void OnVirginItem(Item item)
	{
		base.OnVirginItem(item);
		foreach (ItemAmount itemAmount in this.defaultContents)
		{
			Item item2 = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0UL);
			if (item2 != null)
			{
				item2.MoveToContainer(item.contents, -1, true, false, null, true);
			}
		}
	}

	// Token: 0x06002D97 RID: 11671 RVA: 0x00112F04 File Offset: 0x00111104
	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		if (item.contents == null)
		{
			return;
		}
		for (int i = item.contents.itemList.Count - 1; i >= 0; i--)
		{
			Item item2 = item.contents.itemList[i];
			if (!item2.MoveToContainer(crafter.inventory.containerMain, -1, true, false, null, true))
			{
				item2.Drop(crafter.GetDropPosition(), crafter.GetDropVelocity(), default(Quaternion));
			}
		}
	}

	// Token: 0x04002539 RID: 9529
	public int capacity = 6;

	// Token: 0x0400253A RID: 9530
	public int maxStackSize;

	// Token: 0x0400253B RID: 9531
	[InspectorFlags]
	public ItemContainer.Flag containerFlags;

	// Token: 0x0400253C RID: 9532
	public ItemContainer.ContentsType onlyAllowedContents = ItemContainer.ContentsType.Generic;

	// Token: 0x0400253D RID: 9533
	public ItemDefinition onlyAllowedItemType;

	// Token: 0x0400253E RID: 9534
	public List<ItemSlot> availableSlots = new List<ItemSlot>();

	// Token: 0x0400253F RID: 9535
	public ItemDefinition[] validItemWhitelist = new ItemDefinition[0];

	// Token: 0x04002540 RID: 9536
	public bool openInDeployed = true;

	// Token: 0x04002541 RID: 9537
	public bool openInInventory = true;

	// Token: 0x04002542 RID: 9538
	public List<ItemAmount> defaultContents = new List<ItemAmount>();
}
