using System;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x02000605 RID: 1541
public class ItemModStudyBlueprint : ItemMod
{
	// Token: 0x06002DDC RID: 11740 RVA: 0x00114108 File Offset: 0x00112308
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (item.GetOwnerPlayer() != player)
		{
			bool flag = false;
			foreach (ItemContainer itemContainer in player.inventory.loot.containers)
			{
				if (item.GetRootContainer() == itemContainer)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
		}
		if (command == "study")
		{
			if (!item.IsBlueprint())
			{
				return;
			}
			ItemDefinition blueprintTargetDef = item.blueprintTargetDef;
			ItemBlueprint blueprint = blueprintTargetDef.Blueprint;
			bool flag2 = player.blueprints.IsUnlocked(blueprintTargetDef);
			if (flag2 && blueprint != null && blueprint.additionalUnlocks != null && blueprint.additionalUnlocks.Count > 0)
			{
				foreach (ItemDefinition itemDefinition in blueprint.additionalUnlocks)
				{
					if (!player.blueprints.IsUnlocked(itemDefinition))
					{
						flag2 = false;
					}
				}
			}
			if (blueprint != null && blueprint.defaultBlueprint)
			{
				flag2 = true;
			}
			if (flag2)
			{
				return;
			}
			Item item2 = item;
			if (item.amount > 1)
			{
				item2 = item.SplitItem(1);
			}
			item2.UseItem(1);
			player.blueprints.Unlock(blueprintTargetDef);
			Analytics.Azure.OnBlueprintLearned(player, blueprintTargetDef, "blueprint", null);
			if (blueprint != null && blueprint.additionalUnlocks != null && blueprint.additionalUnlocks.Count > 0)
			{
				foreach (ItemDefinition itemDefinition2 in blueprint.additionalUnlocks)
				{
					player.blueprints.Unlock(itemDefinition2);
					Analytics.Azure.OnBlueprintLearned(player, itemDefinition2, "blueprint", null);
				}
			}
			if (this.studyEffect.isValid)
			{
				Effect.server.Run(this.studyEffect.resourcePath, player, StringPool.Get("head"), Vector3.zero, Vector3.zero, null, false);
			}
		}
	}

	// Token: 0x04002597 RID: 9623
	public GameObjectRef studyEffect;
}
