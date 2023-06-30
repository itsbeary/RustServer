using System;
using UnityEngine;

// Token: 0x02000603 RID: 1539
public class ItemModReveal : ItemMod
{
	// Token: 0x06002DD8 RID: 11736 RVA: 0x00113F8C File Offset: 0x0011218C
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "reveal")
		{
			if (item.amount < this.numForReveal)
			{
				return;
			}
			int position = item.position;
			item.UseItem(this.numForReveal);
			Item item2 = null;
			if (this.revealedItemOverride)
			{
				item2 = ItemManager.Create(this.revealedItemOverride, this.revealedItemAmount, 0UL);
			}
			if (item2 != null && !item2.MoveToContainer(player.inventory.containerMain, (item.amount == 0) ? position : (-1), true, false, null, true))
			{
				item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
			}
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}

	// Token: 0x04002590 RID: 9616
	public int numForReveal = 10;

	// Token: 0x04002591 RID: 9617
	public ItemDefinition revealedItemOverride;

	// Token: 0x04002592 RID: 9618
	public int revealedItemAmount = 1;

	// Token: 0x04002593 RID: 9619
	public LootSpawn revealList;

	// Token: 0x04002594 RID: 9620
	public GameObjectRef successEffect;
}
