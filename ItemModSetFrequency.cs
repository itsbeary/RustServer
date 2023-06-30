using System;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;

// Token: 0x020001B6 RID: 438
public class ItemModSetFrequency : ItemMod
{
	// Token: 0x0600190B RID: 6411 RVA: 0x000B8CE8 File Offset: 0x000B6EE8
	public override void ServerCommand(global::Item item, string command, global::BasePlayer player)
	{
		base.ServerCommand(item, command, player);
		if (command.Contains("SetFrequency"))
		{
			if (ItemModSetFrequency.itemsOnCooldown.Count > 0 && this.onlyFrequency)
			{
				for (int i = ItemModSetFrequency.itemsOnCooldown.Count - 1; i >= 0; i--)
				{
					if (ItemModSetFrequency.itemsOnCooldown[i].TargetItem == item && ItemModSetFrequency.itemsOnCooldown[i].TimeSinceEdit < 2f)
					{
						return;
					}
					if (ItemModSetFrequency.itemsOnCooldown[i].TimeSinceEdit > 2f)
					{
						ItemModSetFrequency.itemsOnCooldown.RemoveAt(i);
					}
				}
			}
			int num = 0;
			if (int.TryParse(command.Substring(command.IndexOf(":") + 1), out num))
			{
				global::BaseEntity heldEntity = item.GetHeldEntity();
				Detonator detonator;
				if (heldEntity != null && (detonator = heldEntity as Detonator) != null)
				{
					detonator.ServerSetFrequency(player, num);
				}
				else
				{
					item.instanceData.dataInt = num;
					if (this.loseConditionOnChange)
					{
						item.LoseCondition(item.maxCondition * 0.01f);
					}
					item.MarkDirty();
				}
				if (this.onlyFrequency)
				{
					ItemModSetFrequency.itemsOnCooldown.Add(new ItemModSetFrequency.ItemTime
					{
						TargetItem = item,
						TimeSinceEdit = 0f
					});
				}
			}
			else
			{
				Debug.Log("Parse fuckup");
			}
		}
		if (!this.onlyFrequency)
		{
			if (command == "rf_on")
			{
				item.SetFlag(global::Item.Flag.IsOn, true);
				item.MarkDirty();
				return;
			}
			if (command == "rf_off")
			{
				item.SetFlag(global::Item.Flag.IsOn, false);
				item.MarkDirty();
			}
		}
	}

	// Token: 0x0600190C RID: 6412 RVA: 0x000B8E82 File Offset: 0x000B7082
	public override void OnItemCreated(global::Item item)
	{
		if (item.instanceData == null)
		{
			item.instanceData = new ProtoBuf.Item.InstanceData();
			item.instanceData.ShouldPool = false;
			item.instanceData.dataInt = this.defaultFrequency;
		}
	}

	// Token: 0x04001196 RID: 4502
	public GameObjectRef frequencyPanelPrefab;

	// Token: 0x04001197 RID: 4503
	public bool allowArmDisarm;

	// Token: 0x04001198 RID: 4504
	public bool onlyFrequency;

	// Token: 0x04001199 RID: 4505
	public int defaultFrequency = -1;

	// Token: 0x0400119A RID: 4506
	public bool loseConditionOnChange;

	// Token: 0x0400119B RID: 4507
	private static List<ItemModSetFrequency.ItemTime> itemsOnCooldown = new List<ItemModSetFrequency.ItemTime>();

	// Token: 0x02000C4B RID: 3147
	private struct ItemTime
	{
		// Token: 0x04004346 RID: 17222
		public global::Item TargetItem;

		// Token: 0x04004347 RID: 17223
		public TimeSince TimeSinceEdit;
	}
}
