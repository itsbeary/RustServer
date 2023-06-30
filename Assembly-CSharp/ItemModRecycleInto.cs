using System;
using UnityEngine;

// Token: 0x02000601 RID: 1537
public class ItemModRecycleInto : ItemMod
{
	// Token: 0x06002DD3 RID: 11731 RVA: 0x00113E0C File Offset: 0x0011200C
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "recycle_item")
		{
			int num = UnityEngine.Random.Range(this.numRecycledItemMin, this.numRecycledItemMax + 1);
			item.UseItem(1);
			if (num > 0)
			{
				Item item2 = ItemManager.Create(this.recycleIntoItem, num, 0UL);
				if (!item2.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
				{
					item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
				}
				if (this.successEffect.isValid)
				{
					Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
				}
			}
		}
	}

	// Token: 0x04002589 RID: 9609
	public ItemDefinition recycleIntoItem;

	// Token: 0x0400258A RID: 9610
	public int numRecycledItemMin = 1;

	// Token: 0x0400258B RID: 9611
	public int numRecycledItemMax = 1;

	// Token: 0x0400258C RID: 9612
	public GameObjectRef successEffect;
}
