using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200075C RID: 1884
[CreateAssetMenu(menuName = "Rust/Player Inventory Properties")]
public class PlayerInventoryProperties : ScriptableObject
{
	// Token: 0x06003479 RID: 13433 RVA: 0x001443AC File Offset: 0x001425AC
	public void GiveToPlayer(BasePlayer player)
	{
		PlayerInventoryProperties.<>c__DisplayClass7_0 CS$<>8__locals1;
		CS$<>8__locals1.player = player;
		if (CS$<>8__locals1.player == null)
		{
			return;
		}
		CS$<>8__locals1.player.inventory.Strip();
		if (this.giveBase != null)
		{
			this.giveBase.GiveToPlayer(CS$<>8__locals1.player);
		}
		foreach (PlayerInventoryProperties.ItemAmountSkinned itemAmountSkinned in this.belt)
		{
			PlayerInventoryProperties.<GiveToPlayer>g__CreateItem|7_0(itemAmountSkinned, CS$<>8__locals1.player.inventory.containerBelt, ref CS$<>8__locals1);
		}
		foreach (PlayerInventoryProperties.ItemAmountSkinned itemAmountSkinned2 in this.main)
		{
			PlayerInventoryProperties.<GiveToPlayer>g__CreateItem|7_0(itemAmountSkinned2, CS$<>8__locals1.player.inventory.containerMain, ref CS$<>8__locals1);
		}
		foreach (PlayerInventoryProperties.ItemAmountSkinned itemAmountSkinned3 in this.wear)
		{
			PlayerInventoryProperties.<GiveToPlayer>g__CreateItem|7_0(itemAmountSkinned3, CS$<>8__locals1.player.inventory.containerWear, ref CS$<>8__locals1);
		}
	}

	// Token: 0x0600347A RID: 13434 RVA: 0x001444F8 File Offset: 0x001426F8
	public static PlayerInventoryProperties GetInventoryConfig(string name)
	{
		if (PlayerInventoryProperties.allInventories == null)
		{
			PlayerInventoryProperties.allInventories = FileSystem.LoadAll<PlayerInventoryProperties>("assets/content/properties/playerinventory", "");
			Debug.Log(string.Format("Found {0} inventories", PlayerInventoryProperties.allInventories.Length));
		}
		if (PlayerInventoryProperties.allInventories != null)
		{
			foreach (PlayerInventoryProperties playerInventoryProperties in PlayerInventoryProperties.allInventories)
			{
				if (playerInventoryProperties.niceName.ToLower() == name.ToLower())
				{
					return playerInventoryProperties;
				}
			}
		}
		return null;
	}

	// Token: 0x0600347D RID: 13437 RVA: 0x00144588 File Offset: 0x00142788
	[CompilerGenerated]
	internal static void <GiveToPlayer>g__CreateItem|7_0(PlayerInventoryProperties.ItemAmountSkinned toCreate, ItemContainer destination, ref PlayerInventoryProperties.<>c__DisplayClass7_0 A_2)
	{
		Item item;
		if (toCreate.blueprint)
		{
			item = ItemManager.Create(ItemManager.blueprintBaseDef, 1, 0UL);
			item.blueprintTarget = ((toCreate.itemDef.isRedirectOf != null) ? toCreate.itemDef.isRedirectOf.itemid : toCreate.itemDef.itemid);
		}
		else
		{
			item = ItemManager.Create(toCreate.itemDef, (int)toCreate.amount, toCreate.skinOverride);
		}
		A_2.player.inventory.GiveItem(item, destination, false);
	}

	// Token: 0x04002AF1 RID: 10993
	public string niceName;

	// Token: 0x04002AF2 RID: 10994
	public int order = 100;

	// Token: 0x04002AF3 RID: 10995
	public List<PlayerInventoryProperties.ItemAmountSkinned> belt;

	// Token: 0x04002AF4 RID: 10996
	public List<PlayerInventoryProperties.ItemAmountSkinned> main;

	// Token: 0x04002AF5 RID: 10997
	public List<PlayerInventoryProperties.ItemAmountSkinned> wear;

	// Token: 0x04002AF6 RID: 10998
	public PlayerInventoryProperties giveBase;

	// Token: 0x04002AF7 RID: 10999
	private static PlayerInventoryProperties[] allInventories;

	// Token: 0x02000E7C RID: 3708
	[Serializable]
	public class ItemAmountSkinned : ItemAmount
	{
		// Token: 0x060052DB RID: 21211 RVA: 0x001B151D File Offset: 0x001AF71D
		public ItemAmountSkinned()
			: base(null, 0f)
		{
		}

		// Token: 0x04004C08 RID: 19464
		public ulong skinOverride;

		// Token: 0x04004C09 RID: 19465
		public bool blueprint;
	}
}
