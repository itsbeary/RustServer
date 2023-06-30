using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AB6 RID: 2742
	[ConsoleSystem.Factory("craft")]
	public class Craft : ConsoleSystem
	{
		// Token: 0x06004196 RID: 16790 RVA: 0x00184680 File Offset: 0x00182880
		[ServerUserVar]
		public static void add(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			int @int = args.GetInt(0, 0);
			int int2 = args.GetInt(1, 1);
			int num = (int)args.GetUInt64(2, 0UL);
			if (int2 < 1)
			{
				return;
			}
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(@int);
			if (itemDefinition == null)
			{
				args.ReplyWith("Item not found");
				return;
			}
			ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(itemDefinition);
			if (!itemBlueprint)
			{
				args.ReplyWith("Blueprint not found");
				return;
			}
			if (!itemBlueprint.userCraftable)
			{
				args.ReplyWith("Item is not craftable");
				return;
			}
			if (!basePlayer.blueprints.CanCraft(@int, num, basePlayer.userID))
			{
				num = 0;
				if (!basePlayer.blueprints.CanCraft(@int, num, basePlayer.userID))
				{
					args.ReplyWith("You can't craft this item");
					return;
				}
				args.ReplyWith("You don't have permission to use this skin, so crafting unskinned");
			}
			if (!basePlayer.inventory.crafting.CraftItem(itemBlueprint, basePlayer, null, int2, num, null, false))
			{
				args.ReplyWith("Couldn't craft!");
				return;
			}
		}

		// Token: 0x06004197 RID: 16791 RVA: 0x00184784 File Offset: 0x00182984
		[ServerUserVar]
		public static void canceltask(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			int @int = args.GetInt(0, 0);
			if (!basePlayer.inventory.crafting.CancelTask(@int, true))
			{
				args.ReplyWith("Couldn't cancel task!");
				return;
			}
		}

		// Token: 0x06004198 RID: 16792 RVA: 0x001847D4 File Offset: 0x001829D4
		[ServerUserVar]
		public static void cancel(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			int @int = args.GetInt(0, 0);
			basePlayer.inventory.crafting.CancelBlueprint(@int);
		}

		// Token: 0x06004199 RID: 16793 RVA: 0x00184818 File Offset: 0x00182A18
		[ServerUserVar]
		public static void fasttracktask(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			int @int = args.GetInt(0, 0);
			if (!basePlayer.inventory.crafting.FastTrackTask(@int))
			{
				args.ReplyWith("Couldn't fast track task!");
			}
		}

		// Token: 0x04003B7E RID: 15230
		[ServerVar]
		public static bool instant;
	}
}
