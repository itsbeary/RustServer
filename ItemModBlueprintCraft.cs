using System;
using UnityEngine;

// Token: 0x020005E0 RID: 1504
public class ItemModBlueprintCraft : ItemMod
{
	// Token: 0x06002D6E RID: 11630 RVA: 0x0011249C File Offset: 0x0011069C
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (item.GetOwnerPlayer() != player)
		{
			return;
		}
		if (command == "craft")
		{
			if (!item.IsBlueprint())
			{
				return;
			}
			if (!player.inventory.crafting.CanCraft(item.blueprintTargetDef.Blueprint, 1, false))
			{
				return;
			}
			Item item2 = item;
			if (item.amount > 1)
			{
				item2 = item.SplitItem(1);
			}
			player.inventory.crafting.CraftItem(item.blueprintTargetDef.Blueprint, player, null, 1, 0, item2, false);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
		if (command == "craft_all")
		{
			if (!item.IsBlueprint())
			{
				return;
			}
			if (!player.inventory.crafting.CanCraft(item.blueprintTargetDef.Blueprint, item.amount, false))
			{
				return;
			}
			player.inventory.crafting.CraftItem(item.blueprintTargetDef.Blueprint, player, null, item.amount, 0, item, false);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}

	// Token: 0x04002518 RID: 9496
	public GameObjectRef successEffect;
}
