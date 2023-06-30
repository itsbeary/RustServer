using System;
using UnityEngine;

// Token: 0x02000607 RID: 1543
public class ItemModSwap : ItemMod
{
	// Token: 0x06002DE0 RID: 11744 RVA: 0x00114394 File Offset: 0x00112594
	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		foreach (ItemAmount itemAmount in this.becomeItem)
		{
			Item item2 = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0UL);
			if (item2 != null)
			{
				if (!item2.MoveToContainer(item.parent, -1, true, false, null, true))
				{
					player.GiveItem(item2, BaseEntity.GiveItemReason.Generic);
				}
				if (this.sendPlayerPickupNotification)
				{
					player.Command("note.inv", new object[]
					{
						item2.info.itemid,
						item2.amount
					});
				}
			}
		}
		if (this.RandomOptions.Length != 0)
		{
			int num = UnityEngine.Random.Range(0, this.RandomOptions.Length);
			Item item3 = ItemManager.Create(this.RandomOptions[num].itemDef, (int)this.RandomOptions[num].amount, 0UL);
			if (item3 != null)
			{
				if (!item3.MoveToContainer(item.parent, -1, true, false, null, true))
				{
					player.GiveItem(item3, BaseEntity.GiveItemReason.Generic);
				}
				if (this.sendPlayerPickupNotification)
				{
					player.Command("note.inv", new object[]
					{
						item3.info.itemid,
						item3.amount
					});
				}
			}
		}
		if (this.sendPlayerDropNotification)
		{
			player.Command("note.inv", new object[]
			{
				item.info.itemid,
				-1
			});
		}
		if (this.actionEffect.isValid)
		{
			Effect.server.Run(this.actionEffect.resourcePath, player.transform.position, Vector3.up, null, false);
		}
		item.UseItem(1);
	}

	// Token: 0x0400259B RID: 9627
	public GameObjectRef actionEffect;

	// Token: 0x0400259C RID: 9628
	public ItemAmount[] becomeItem;

	// Token: 0x0400259D RID: 9629
	public bool sendPlayerPickupNotification;

	// Token: 0x0400259E RID: 9630
	public bool sendPlayerDropNotification;

	// Token: 0x0400259F RID: 9631
	public float xpScale = 1f;

	// Token: 0x040025A0 RID: 9632
	public ItemAmount[] RandomOptions;
}
