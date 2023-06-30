using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009E RID: 158
public class MixingTable : StorageContainer
{
	// Token: 0x06000E2C RID: 3628 RVA: 0x00077F88 File Offset: 0x00076188
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MixingTable.OnRpcMessage", 0))
		{
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVSwitch ");
				}
				using (TimeWarning.New("SVSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4167839872U, "SVSwitch", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SVSwitch(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SVSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000E2D RID: 3629 RVA: 0x000780F0 File Offset: 0x000762F0
	// (set) Token: 0x06000E2E RID: 3630 RVA: 0x000780F8 File Offset: 0x000762F8
	public float RemainingMixTime { get; private set; }

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x06000E2F RID: 3631 RVA: 0x00078101 File Offset: 0x00076301
	// (set) Token: 0x06000E30 RID: 3632 RVA: 0x00078109 File Offset: 0x00076309
	public float TotalMixTime { get; private set; }

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x06000E31 RID: 3633 RVA: 0x00078112 File Offset: 0x00076312
	// (set) Token: 0x06000E32 RID: 3634 RVA: 0x0007811A File Offset: 0x0007631A
	public global::BasePlayer MixStartingPlayer { get; private set; }

	// Token: 0x06000E33 RID: 3635 RVA: 0x00078124 File Offset: 0x00076324
	public override void ServerInit()
	{
		base.ServerInit();
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
		base.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		RecipeDictionary.CacheRecipes(this.Recipes);
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x00078184 File Offset: 0x00076384
	private bool CanAcceptItem(global::Item item, int targetSlot)
	{
		if (item == null)
		{
			return false;
		}
		if (!this.OnlyAcceptValidIngredients)
		{
			return true;
		}
		if (this.GetItemWaterAmount(item) > 0)
		{
			item = item.contents.itemList[0];
		}
		return item.info == this.currentProductionItem || RecipeDictionary.ValidIngredientForARecipe(item, this.Recipes);
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x000781DE File Offset: 0x000763DE
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		if (base.IsOn())
		{
			this.StopMixing();
		}
	}

	// Token: 0x06000E36 RID: 3638 RVA: 0x000781F4 File Offset: 0x000763F4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void SVSwitch(global::BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (flag == base.IsOn())
		{
			return;
		}
		if (msg.player == null)
		{
			return;
		}
		if (flag)
		{
			this.StartMixing(msg.player);
			return;
		}
		this.StopMixing();
	}

	// Token: 0x06000E37 RID: 3639 RVA: 0x0007823C File Offset: 0x0007643C
	private void StartMixing(global::BasePlayer player)
	{
		if (base.IsOn())
		{
			return;
		}
		if (!this.CanStartMixing(player))
		{
			return;
		}
		this.MixStartingPlayer = player;
		bool flag;
		List<global::Item> orderedContainerItems = this.GetOrderedContainerItems(base.inventory, out flag);
		int num;
		this.currentRecipe = RecipeDictionary.GetMatchingRecipeAndQuantity(this.Recipes, orderedContainerItems, out num);
		this.currentQuantity = num;
		if (this.currentRecipe == null || !flag)
		{
			return;
		}
		if (this.currentRecipe.RequiresBlueprint && this.currentRecipe.ProducedItem != null && !player.blueprints.HasUnlocked(this.currentRecipe.ProducedItem))
		{
			return;
		}
		if (base.isServer)
		{
			this.lastTickTimestamp = UnityEngine.Time.realtimeSinceStartup;
		}
		this.RemainingMixTime = this.currentRecipe.MixingDuration * (float)this.currentQuantity;
		this.TotalMixTime = this.RemainingMixTime;
		this.ReturnExcessItems(orderedContainerItems, player);
		if (this.RemainingMixTime == 0f)
		{
			this.ProduceItem(this.currentRecipe, this.currentQuantity);
			return;
		}
		base.InvokeRepeating(new Action(this.TickMix), 1f, 1f);
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000E38 RID: 3640 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool CanStartMixing(global::BasePlayer player)
	{
		return true;
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x00078368 File Offset: 0x00076568
	public void StopMixing()
	{
		this.currentRecipe = null;
		this.currentQuantity = 0;
		this.RemainingMixTime = 0f;
		base.CancelInvoke(new Action(this.TickMix));
		if (!base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x000783BC File Offset: 0x000765BC
	private void TickMix()
	{
		if (this.currentRecipe == null)
		{
			this.StopMixing();
			return;
		}
		if (base.isServer)
		{
			this.lastTickTimestamp = UnityEngine.Time.realtimeSinceStartup;
			this.RemainingMixTime -= 1f;
		}
		base.SendNetworkUpdateImmediate(false);
		if (this.RemainingMixTime <= 0f)
		{
			this.ProduceItem(this.currentRecipe, this.currentQuantity);
		}
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x00078429 File Offset: 0x00076629
	private void ProduceItem(Recipe recipe, int quantity)
	{
		this.StopMixing();
		this.ConsumeInventory(recipe, quantity);
		this.CreateRecipeItems(recipe, quantity);
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x00078444 File Offset: 0x00076644
	private void ConsumeInventory(Recipe recipe, int quantity)
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item item = base.inventory.GetSlot(i);
			if (item != null)
			{
				if (this.GetItemWaterAmount(item) > 0)
				{
					item = item.contents.itemList[0];
				}
				int num = recipe.Ingredients[i].Count * quantity;
				if (num > 0)
				{
					string shortname = item.info.shortname;
					int amount = item.amount;
					global::BasePlayer mixStartingPlayer = this.MixStartingPlayer;
					bool flag = false;
					ItemDefinition producedItem = recipe.ProducedItem;
					Analytics.Azure.OnCraftMaterialConsumed(shortname, amount, mixStartingPlayer, this, flag, (producedItem != null) ? producedItem.shortname : null);
					item.UseItem(num);
				}
			}
		}
		ItemManager.DoRemoves();
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x000784EC File Offset: 0x000766EC
	private void ReturnExcessItems(List<global::Item> orderedContainerItems, global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		if (this.currentRecipe == null)
		{
			return;
		}
		if (orderedContainerItems == null)
		{
			return;
		}
		if (orderedContainerItems.Count != this.currentRecipe.Ingredients.Length)
		{
			return;
		}
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot == null)
			{
				break;
			}
			int num = slot.amount - this.currentRecipe.Ingredients[i].Count * this.currentQuantity;
			if (num > 0)
			{
				global::Item item = slot.SplitItem(num);
				if (!item.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true) && !item.MoveToContainer(player.inventory.containerBelt, -1, true, false, null, true))
				{
					item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity, default(Quaternion));
				}
			}
		}
		ItemManager.DoRemoves();
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x000785E8 File Offset: 0x000767E8
	protected virtual void CreateRecipeItems(Recipe recipe, int quantity)
	{
		if (recipe == null)
		{
			return;
		}
		if (recipe.ProducedItem == null)
		{
			return;
		}
		int num = quantity * recipe.ProducedItemCount;
		int stackable = recipe.ProducedItem.stackable;
		int num2 = Mathf.CeilToInt((float)num / (float)stackable);
		this.currentProductionItem = recipe.ProducedItem;
		for (int i = 0; i < num2; i++)
		{
			int num3 = ((num > stackable) ? stackable : num);
			global::Item item = ItemManager.Create(recipe.ProducedItem, num3, 0UL);
			Analytics.Azure.OnCraftItem(item.info.shortname, item.amount, this.MixStartingPlayer, this, false);
			if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
			{
				item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity, default(Quaternion));
			}
			num -= num3;
			if (num <= 0)
			{
				break;
			}
		}
		this.currentProductionItem = null;
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x000786D4 File Offset: 0x000768D4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.mixingTable = Facepunch.Pool.Get<ProtoBuf.MixingTable>();
		if (info.forDisk)
		{
			info.msg.mixingTable.remainingMixTime = this.RemainingMixTime;
		}
		else
		{
			info.msg.mixingTable.remainingMixTime = this.RemainingMixTime - Mathf.Max(UnityEngine.Time.realtimeSinceStartup - this.lastTickTimestamp, 0f);
		}
		info.msg.mixingTable.totalMixTime = this.TotalMixTime;
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x0007875C File Offset: 0x0007695C
	private int GetItemWaterAmount(global::Item item)
	{
		if (item == null)
		{
			return 0;
		}
		if (item.contents != null && item.contents.capacity == 1 && item.contents.allowedContents == global::ItemContainer.ContentsType.Liquid && item.contents.itemList.Count > 0)
		{
			return item.contents.itemList[0].amount;
		}
		return 0;
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x000787C0 File Offset: 0x000769C0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.mixingTable == null)
		{
			return;
		}
		this.RemainingMixTime = info.msg.mixingTable.remainingMixTime;
		this.TotalMixTime = info.msg.mixingTable.totalMixTime;
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x00078810 File Offset: 0x00076A10
	public List<global::Item> GetOrderedContainerItems(global::ItemContainer container, out bool itemsAreContiguous)
	{
		itemsAreContiguous = true;
		if (container == null)
		{
			return null;
		}
		if (container.itemList == null)
		{
			return null;
		}
		if (container.itemList.Count == 0)
		{
			return null;
		}
		this.inventoryItems.Clear();
		bool flag = false;
		for (int i = 0; i < container.capacity; i++)
		{
			global::Item item = container.GetSlot(i);
			if (item != null && flag)
			{
				itemsAreContiguous = false;
				break;
			}
			if (item == null)
			{
				flag = true;
			}
			else
			{
				if (this.GetItemWaterAmount(item) > 0)
				{
					item = item.contents.itemList[0];
				}
				this.inventoryItems.Add(item);
			}
		}
		return this.inventoryItems;
	}

	// Token: 0x0400093D RID: 2365
	public GameObject Particles;

	// Token: 0x0400093E RID: 2366
	public RecipeList Recipes;

	// Token: 0x0400093F RID: 2367
	public bool OnlyAcceptValidIngredients;

	// Token: 0x04000942 RID: 2370
	private float lastTickTimestamp;

	// Token: 0x04000943 RID: 2371
	private List<global::Item> inventoryItems = new List<global::Item>();

	// Token: 0x04000945 RID: 2373
	private const float mixTickInterval = 1f;

	// Token: 0x04000946 RID: 2374
	private Recipe currentRecipe;

	// Token: 0x04000947 RID: 2375
	private int currentQuantity;

	// Token: 0x04000948 RID: 2376
	protected ItemDefinition currentProductionItem;
}
