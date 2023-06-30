using System;
using UnityEngine;

// Token: 0x02000609 RID: 1545
public class ItemModUpgrade : ItemMod
{
	// Token: 0x06002DE4 RID: 11748 RVA: 0x00114588 File Offset: 0x00112788
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "upgrade_item")
		{
			if (item.amount < this.numForUpgrade)
			{
				return;
			}
			if (UnityEngine.Random.Range(0f, 1f) <= this.upgradeSuccessChance)
			{
				item.UseItem(this.numForUpgrade);
				Item item2 = ItemManager.Create(this.upgradedItem, this.numUpgradedItem, 0UL);
				if (!item2.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
				{
					item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
				}
				if (this.successEffect.isValid)
				{
					Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
					return;
				}
			}
			else
			{
				item.UseItem(this.numToLoseOnFail);
				if (this.failEffect.isValid)
				{
					Effect.server.Run(this.failEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
				}
			}
		}
	}

	// Token: 0x040025A3 RID: 9635
	public int numForUpgrade = 10;

	// Token: 0x040025A4 RID: 9636
	public float upgradeSuccessChance = 1f;

	// Token: 0x040025A5 RID: 9637
	public int numToLoseOnFail = 2;

	// Token: 0x040025A6 RID: 9638
	public ItemDefinition upgradedItem;

	// Token: 0x040025A7 RID: 9639
	public int numUpgradedItem = 1;

	// Token: 0x040025A8 RID: 9640
	public GameObjectRef successEffect;

	// Token: 0x040025A9 RID: 9641
	public GameObjectRef failEffect;
}
