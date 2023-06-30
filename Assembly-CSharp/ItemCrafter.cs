using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using ProtoBuf;
using UnityEngine;

// Token: 0x020005CC RID: 1484
public class ItemCrafter : EntityComponent<global::BasePlayer>
{
	// Token: 0x06002C89 RID: 11401 RVA: 0x0010DB58 File Offset: 0x0010BD58
	public void AddContainer(global::ItemContainer container)
	{
		this.containers.Add(container);
	}

	// Token: 0x06002C8A RID: 11402 RVA: 0x0010DB68 File Offset: 0x0010BD68
	public static float GetScaledDuration(ItemBlueprint bp, float workbenchLevel)
	{
		float num = workbenchLevel - (float)bp.workbenchLevelRequired;
		if (num == 1f)
		{
			return bp.time * 0.5f;
		}
		if (num >= 2f)
		{
			return bp.time * 0.25f;
		}
		return bp.time;
	}

	// Token: 0x06002C8B RID: 11403 RVA: 0x0010DBB0 File Offset: 0x0010BDB0
	public void ServerUpdate(float delta)
	{
		if (this.queue.Count == 0)
		{
			return;
		}
		ItemCraftTask value = this.queue.First.Value;
		if (value.cancelled)
		{
			value.owner.Command("note.craft_done", new object[] { value.taskUID, 0 });
			this.queue.RemoveFirst();
			return;
		}
		float currentCraftLevel = value.owner.currentCraftLevel;
		if (value.endTime > UnityEngine.Time.realtimeSinceStartup)
		{
			return;
		}
		if (value.endTime == 0f)
		{
			float scaledDuration = ItemCrafter.GetScaledDuration(value.blueprint, currentCraftLevel);
			value.endTime = UnityEngine.Time.realtimeSinceStartup + scaledDuration;
			value.workbenchEntity = value.owner.GetCachedCraftLevelWorkbench();
			if (value.owner != null)
			{
				value.owner.Command("note.craft_start", new object[] { value.taskUID, scaledDuration, value.amount });
				if (value.owner.IsAdmin && Craft.instant)
				{
					value.endTime = UnityEngine.Time.realtimeSinceStartup + 1f;
				}
			}
			return;
		}
		this.FinishCrafting(value);
		if (value.amount <= 0)
		{
			this.queue.RemoveFirst();
			return;
		}
		value.endTime = 0f;
	}

	// Token: 0x06002C8C RID: 11404 RVA: 0x0010DD08 File Offset: 0x0010BF08
	private void CollectIngredient(int item, int amount, List<global::Item> collect)
	{
		foreach (global::ItemContainer itemContainer in this.containers)
		{
			amount -= itemContainer.Take(collect, item, amount);
			if (amount <= 0)
			{
				break;
			}
		}
	}

	// Token: 0x06002C8D RID: 11405 RVA: 0x0010DD68 File Offset: 0x0010BF68
	private void CollectIngredients(ItemBlueprint bp, ItemCraftTask task, int amount = 1, global::BasePlayer player = null)
	{
		List<global::Item> list = new List<global::Item>();
		foreach (ItemAmount itemAmount in bp.ingredients)
		{
			this.CollectIngredient(itemAmount.itemid, (int)itemAmount.amount * amount, list);
		}
		task.potentialOwners = new List<ulong>();
		foreach (global::Item item in list)
		{
			item.CollectedForCrafting(player);
			if (!task.potentialOwners.Contains(player.userID))
			{
				task.potentialOwners.Add(player.userID);
			}
		}
		task.takenItems = list;
	}

	// Token: 0x06002C8E RID: 11406 RVA: 0x0010DE44 File Offset: 0x0010C044
	public bool CraftItem(ItemBlueprint bp, global::BasePlayer owner, ProtoBuf.Item.InstanceData instanceData = null, int amount = 1, int skinID = 0, global::Item fromTempBlueprint = null, bool free = false)
	{
		if (!this.CanCraft(bp, amount, free))
		{
			return false;
		}
		this.taskUID++;
		ItemCraftTask itemCraftTask = Facepunch.Pool.Get<ItemCraftTask>();
		itemCraftTask.blueprint = bp;
		if (!free)
		{
			this.CollectIngredients(bp, itemCraftTask, amount, owner);
		}
		itemCraftTask.endTime = 0f;
		itemCraftTask.taskUID = this.taskUID;
		itemCraftTask.owner = owner;
		itemCraftTask.instanceData = instanceData;
		if (itemCraftTask.instanceData != null)
		{
			itemCraftTask.instanceData.ShouldPool = false;
		}
		itemCraftTask.amount = amount;
		itemCraftTask.skinID = skinID;
		if (fromTempBlueprint != null && itemCraftTask.takenItems != null)
		{
			fromTempBlueprint.RemoveFromContainer();
			itemCraftTask.takenItems.Add(fromTempBlueprint);
			itemCraftTask.conditionScale = 0.5f;
		}
		this.queue.AddLast(itemCraftTask);
		if (itemCraftTask.owner != null)
		{
			itemCraftTask.owner.Command("note.craft_add", new object[]
			{
				itemCraftTask.taskUID,
				itemCraftTask.blueprint.targetItem.itemid,
				amount,
				itemCraftTask.skinID
			});
		}
		return true;
	}

	// Token: 0x06002C8F RID: 11407 RVA: 0x0010DF70 File Offset: 0x0010C170
	private void FinishCrafting(ItemCraftTask task)
	{
		task.amount--;
		task.numCrafted++;
		ulong num = ItemDefinition.FindSkin(task.blueprint.targetItem.itemid, task.skinID);
		global::Item item = ItemManager.CreateByItemID(task.blueprint.targetItem.itemid, 1, num);
		item.amount = task.blueprint.amountToCreate;
		int amount = item.amount;
		float currentCraftLevel = task.owner.currentCraftLevel;
		bool flag = task.owner.InSafeZone();
		if (item.hasCondition && task.conditionScale != 1f)
		{
			item.maxCondition *= task.conditionScale;
			item.condition = item.maxCondition;
		}
		item.OnVirginSpawn();
		foreach (ItemAmount itemAmount in task.blueprint.ingredients)
		{
			int num2 = (int)itemAmount.amount;
			if (task.takenItems != null)
			{
				foreach (global::Item item2 in task.takenItems)
				{
					if (item2.info == itemAmount.itemDef)
					{
						int num3 = Mathf.Min(item2.amount, num2);
						Analytics.Azure.OnCraftMaterialConsumed(item2.info.shortname, num2, base.baseEntity, task.workbenchEntity, flag, item.info.shortname);
						item2.UseItem(num2);
						num2 -= num3;
					}
				}
			}
		}
		Analytics.Server.Crafting(task.blueprint.targetItem.shortname, task.skinID);
		Analytics.Azure.OnCraftItem(item.info.shortname, item.amount, base.baseEntity, task.workbenchEntity, flag);
		task.owner.Command("note.craft_done", new object[] { task.taskUID, 1, task.amount });
		if (task.instanceData != null)
		{
			item.instanceData = task.instanceData;
		}
		if (!string.IsNullOrEmpty(task.blueprint.UnlockAchievment))
		{
			task.owner.GiveAchievement(task.blueprint.UnlockAchievment);
		}
		if (task.owner.inventory.GiveItem(item, null, false))
		{
			task.owner.Command("note.inv", new object[]
			{
				item.info.itemid,
				amount
			});
			return;
		}
		global::ItemContainer itemContainer = this.containers.First<global::ItemContainer>();
		task.owner.Command("note.inv", new object[]
		{
			item.info.itemid,
			amount
		});
		task.owner.Command("note.inv", new object[]
		{
			item.info.itemid,
			-item.amount
		});
		item.Drop(itemContainer.dropPosition, itemContainer.dropVelocity, default(Quaternion));
	}

	// Token: 0x06002C90 RID: 11408 RVA: 0x0010E2D0 File Offset: 0x0010C4D0
	public bool CancelTask(int iID, bool ReturnItems)
	{
		if (this.queue.Count == 0)
		{
			return false;
		}
		ItemCraftTask itemCraftTask = this.queue.FirstOrDefault((ItemCraftTask x) => x.taskUID == iID && !x.cancelled);
		if (itemCraftTask == null)
		{
			return false;
		}
		itemCraftTask.cancelled = true;
		if (itemCraftTask.owner == null)
		{
			return true;
		}
		itemCraftTask.owner.Command("note.craft_done", new object[] { itemCraftTask.taskUID, 0 });
		if (itemCraftTask.takenItems != null && itemCraftTask.takenItems.Count > 0 && ReturnItems)
		{
			foreach (global::Item item in itemCraftTask.takenItems)
			{
				if (item != null && item.amount > 0)
				{
					if (item.IsBlueprint() && item.blueprintTargetDef == itemCraftTask.blueprint.targetItem)
					{
						item.UseItem(itemCraftTask.numCrafted);
					}
					if (item.amount > 0 && !item.MoveToContainer(itemCraftTask.owner.inventory.containerMain, -1, true, false, null, true))
					{
						item.Drop(itemCraftTask.owner.inventory.containerMain.dropPosition + UnityEngine.Random.value * Vector3.down + UnityEngine.Random.insideUnitSphere, itemCraftTask.owner.inventory.containerMain.dropVelocity, default(Quaternion));
						itemCraftTask.owner.Command("note.inv", new object[]
						{
							item.info.itemid,
							-item.amount
						});
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06002C91 RID: 11409 RVA: 0x0010E4C8 File Offset: 0x0010C6C8
	public bool CancelBlueprint(int itemid)
	{
		if (this.queue.Count == 0)
		{
			return false;
		}
		ItemCraftTask itemCraftTask = this.queue.FirstOrDefault((ItemCraftTask x) => x.blueprint.targetItem.itemid == itemid && !x.cancelled);
		return itemCraftTask != null && this.CancelTask(itemCraftTask.taskUID, true);
	}

	// Token: 0x06002C92 RID: 11410 RVA: 0x0010E51C File Offset: 0x0010C71C
	public void CancelAll(bool returnItems)
	{
		foreach (ItemCraftTask itemCraftTask in this.queue)
		{
			this.CancelTask(itemCraftTask.taskUID, returnItems);
		}
	}

	// Token: 0x06002C93 RID: 11411 RVA: 0x0010E578 File Offset: 0x0010C778
	private bool DoesHaveUsableItem(int item, int iAmount)
	{
		int num = 0;
		foreach (global::ItemContainer itemContainer in this.containers)
		{
			num += itemContainer.GetAmount(item, true);
		}
		return num >= iAmount;
	}

	// Token: 0x06002C94 RID: 11412 RVA: 0x0010E5D8 File Offset: 0x0010C7D8
	public bool CanCraft(ItemBlueprint bp, int amount = 1, bool free = false)
	{
		float num = (float)amount / (float)bp.targetItem.craftingStackable;
		foreach (ItemCraftTask itemCraftTask in this.queue)
		{
			if (!itemCraftTask.cancelled)
			{
				num += (float)itemCraftTask.amount / (float)itemCraftTask.blueprint.targetItem.craftingStackable;
			}
		}
		if (num > 8f)
		{
			return false;
		}
		if (amount < 1 || amount > bp.targetItem.craftingStackable)
		{
			return false;
		}
		foreach (ItemAmount itemAmount in bp.ingredients)
		{
			if (!this.DoesHaveUsableItem(itemAmount.itemid, (int)itemAmount.amount * amount))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002C95 RID: 11413 RVA: 0x0010E6D4 File Offset: 0x0010C8D4
	public bool CanCraft(ItemDefinition def, int amount = 1, bool free = false)
	{
		ItemBlueprint component = def.GetComponent<ItemBlueprint>();
		return this.CanCraft(component, amount, free);
	}

	// Token: 0x06002C96 RID: 11414 RVA: 0x0010E6F8 File Offset: 0x0010C8F8
	public bool FastTrackTask(int taskID)
	{
		if (this.queue.Count == 0)
		{
			return false;
		}
		ItemCraftTask value = this.queue.First.Value;
		if (value == null)
		{
			return false;
		}
		ItemCraftTask itemCraftTask = this.queue.FirstOrDefault((ItemCraftTask x) => x.taskUID == taskID && !x.cancelled);
		if (itemCraftTask == null)
		{
			return false;
		}
		if (itemCraftTask == value)
		{
			return false;
		}
		value.endTime = 0f;
		this.queue.Remove(itemCraftTask);
		this.queue.AddFirst(itemCraftTask);
		itemCraftTask.owner.Command("note.craft_fasttracked", new object[] { taskID });
		return true;
	}

	// Token: 0x0400247D RID: 9341
	public List<global::ItemContainer> containers = new List<global::ItemContainer>();

	// Token: 0x0400247E RID: 9342
	public LinkedList<ItemCraftTask> queue = new LinkedList<ItemCraftTask>();

	// Token: 0x0400247F RID: 9343
	public int taskUID;
}
