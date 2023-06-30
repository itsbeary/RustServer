using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000AF RID: 175
public class PlayerInventory : EntityComponent<global::BasePlayer>
{
	// Token: 0x06000FEC RID: 4076 RVA: 0x00084884 File Offset: 0x00082A84
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlayerInventory.OnRpcMessage", 0))
		{
			if (rpc == 3482449460U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ItemCmd ");
				}
				using (TimeWarning.New("ItemCmd", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(3482449460U, "ItemCmd", this.GetBaseEntity(), player))
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
							this.ItemCmd(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ItemCmd");
					}
				}
				return true;
			}
			if (rpc == 3041092525U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - MoveItem ");
				}
				using (TimeWarning.New("MoveItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(3041092525U, "MoveItem", this.GetBaseEntity(), player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.MoveItem(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in MoveItem");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x00084B84 File Offset: 0x00082D84
	protected void Initialize()
	{
		this.containerMain = new global::ItemContainer();
		this.containerMain.SetFlag(global::ItemContainer.Flag.IsPlayer, true);
		this.containerBelt = new global::ItemContainer();
		this.containerBelt.SetFlag(global::ItemContainer.Flag.IsPlayer, true);
		this.containerBelt.SetFlag(global::ItemContainer.Flag.Belt, true);
		this.containerWear = new global::ItemContainer();
		this.containerWear.SetFlag(global::ItemContainer.Flag.IsPlayer, true);
		this.containerWear.SetFlag(global::ItemContainer.Flag.Clothing, true);
		this.crafting = base.GetComponent<ItemCrafter>();
		if (this.crafting != null)
		{
			this.crafting.AddContainer(this.containerMain);
			this.crafting.AddContainer(this.containerBelt);
		}
		this.loot = base.GetComponent<PlayerLoot>();
		if (!this.loot)
		{
			this.loot = base.gameObject.AddComponent<PlayerLoot>();
		}
	}

	// Token: 0x06000FEE RID: 4078 RVA: 0x00084C5C File Offset: 0x00082E5C
	public void DoDestroy()
	{
		if (this.containerMain != null)
		{
			this.containerMain.Kill();
			this.containerMain = null;
		}
		if (this.containerBelt != null)
		{
			this.containerBelt.Kill();
			this.containerBelt = null;
		}
		if (this.containerWear != null)
		{
			this.containerWear.Kill();
			this.containerWear = null;
		}
	}

	// Token: 0x06000FEF RID: 4079 RVA: 0x00084CB8 File Offset: 0x00082EB8
	public void ServerInit(global::BasePlayer owner)
	{
		this.Initialize();
		this.containerMain.ServerInitialize(null, 24);
		if (!this.containerMain.uid.IsValid)
		{
			this.containerMain.GiveUID();
		}
		this.containerBelt.ServerInitialize(null, 6);
		if (!this.containerBelt.uid.IsValid)
		{
			this.containerBelt.GiveUID();
		}
		this.containerWear.ServerInitialize(null, 7);
		if (!this.containerWear.uid.IsValid)
		{
			this.containerWear.GiveUID();
		}
		this.containerMain.playerOwner = owner;
		this.containerBelt.playerOwner = owner;
		this.containerWear.playerOwner = owner;
		this.containerWear.onItemAddedRemoved = new Action<global::Item, bool>(this.OnClothingChanged);
		this.containerWear.canAcceptItem = new Func<global::Item, int, bool>(this.CanWearItem);
		this.containerBelt.canAcceptItem = new Func<global::Item, int, bool>(this.CanEquipItem);
		this.containerMain.onPreItemRemove = new Action<global::Item>(this.OnItemRemoved);
		this.containerWear.onPreItemRemove = new Action<global::Item>(this.OnItemRemoved);
		this.containerBelt.onPreItemRemove = new Action<global::Item>(this.OnItemRemoved);
		this.containerMain.onDirty += this.OnContentsDirty;
		this.containerBelt.onDirty += this.OnContentsDirty;
		this.containerWear.onDirty += this.OnContentsDirty;
		this.containerBelt.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		this.containerMain.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
	}

	// Token: 0x06000FF0 RID: 4080 RVA: 0x00084E6C File Offset: 0x0008306C
	public void OnItemAddedOrRemoved(global::Item item, bool bAdded)
	{
		if (item.info.isHoldable)
		{
			base.Invoke(new Action(this.UpdatedVisibleHolsteredItems), 0.1f);
		}
		if (!bAdded)
		{
			return;
		}
		global::BasePlayer baseEntity = base.baseEntity;
		if (!baseEntity.HasPlayerFlag(global::BasePlayer.PlayerFlags.DisplaySash) && baseEntity.IsHostileItem(item))
		{
			base.baseEntity.SetPlayerFlag(global::BasePlayer.PlayerFlags.DisplaySash, true);
		}
		if (bAdded)
		{
			baseEntity.ProcessMissionEvent(BaseMission.MissionEventType.ACQUIRE_ITEM, item.info.shortname, (float)item.amount);
		}
	}

	// Token: 0x06000FF1 RID: 4081 RVA: 0x00084EEC File Offset: 0x000830EC
	public void UpdatedVisibleHolsteredItems()
	{
		List<global::HeldEntity> list = Facepunch.Pool.GetList<global::HeldEntity>();
		List<global::Item> list2 = Facepunch.Pool.GetList<global::Item>();
		this.AllItemsNoAlloc(ref list2);
		foreach (global::Item item in list2)
		{
			if (item.info.isHoldable && !(item.GetHeldEntity() == null))
			{
				global::HeldEntity component = item.GetHeldEntity().GetComponent<global::HeldEntity>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}
		}
		Facepunch.Pool.FreeList<global::Item>(ref list2);
		IEnumerable<global::HeldEntity> enumerable = list.OrderByDescending((global::HeldEntity x) => x.hostileScore);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		foreach (global::HeldEntity heldEntity in enumerable)
		{
			if (!(heldEntity == null) && heldEntity.holsterInfo.displayWhenHolstered)
			{
				if (flag3 && !heldEntity.IsDeployed() && heldEntity.holsterInfo.slot == global::HeldEntity.HolsterInfo.HolsterSlot.BACK)
				{
					heldEntity.SetVisibleWhileHolstered(true);
					flag3 = false;
				}
				else if (flag2 && !heldEntity.IsDeployed() && heldEntity.holsterInfo.slot == global::HeldEntity.HolsterInfo.HolsterSlot.RIGHT_THIGH)
				{
					heldEntity.SetVisibleWhileHolstered(true);
					flag2 = false;
				}
				else if (flag && !heldEntity.IsDeployed() && heldEntity.holsterInfo.slot == global::HeldEntity.HolsterInfo.HolsterSlot.LEFT_THIGH)
				{
					heldEntity.SetVisibleWhileHolstered(true);
					flag = false;
				}
				else
				{
					heldEntity.SetVisibleWhileHolstered(false);
				}
			}
		}
		Facepunch.Pool.FreeList<global::HeldEntity>(ref list);
	}

	// Token: 0x06000FF2 RID: 4082 RVA: 0x00085098 File Offset: 0x00083298
	private void OnContentsDirty()
	{
		if (base.baseEntity != null)
		{
			base.baseEntity.InvalidateNetworkCache();
		}
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x000850B4 File Offset: 0x000832B4
	private bool CanMoveItemsFrom(global::BaseEntity entity, global::Item item)
	{
		global::PlayerInventory.ICanMoveFrom canMoveFrom;
		return ((canMoveFrom = entity as global::PlayerInventory.ICanMoveFrom) == null || canMoveFrom.CanMoveFrom(base.baseEntity, item)) && (!BaseGameMode.GetActiveGameMode(true) || BaseGameMode.GetActiveGameMode(true).CanMoveItemsFrom(this, entity, item));
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x000850FC File Offset: 0x000832FC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	private void ItemCmd(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != null && msg.player.IsWounded())
		{
			return;
		}
		ItemId itemId = msg.read.ItemID();
		string text = msg.read.String(256);
		global::Item item = this.FindItemUID(itemId);
		if (item == null)
		{
			return;
		}
		if (item.IsLocked() || (item.parent != null && item.parent.IsLocked()))
		{
			return;
		}
		if (!this.CanMoveItemsFrom(item.GetEntityOwner(), item))
		{
			return;
		}
		if (text == "drop")
		{
			int num = item.amount;
			if (msg.read.Unread >= 4)
			{
				num = msg.read.Int32();
			}
			base.baseEntity.stats.Add("item_drop", 1, (global::Stats)5);
			if (num < item.amount)
			{
				global::Item item2 = item.SplitItem(num);
				if (item2 != null)
				{
					DroppedItem droppedItem = item2.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity(), default(Quaternion)) as DroppedItem;
					if (droppedItem != null)
					{
						droppedItem.DropReason = DroppedItem.DropReasonEnum.Player;
						droppedItem.DroppedBy = base.baseEntity.userID;
						Analytics.Azure.OnItemDropped(base.baseEntity, droppedItem, DroppedItem.DropReasonEnum.Player);
					}
				}
			}
			else
			{
				DroppedItem droppedItem2 = item.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity(), default(Quaternion)) as DroppedItem;
				if (droppedItem2 != null)
				{
					droppedItem2.DropReason = DroppedItem.DropReasonEnum.Player;
					droppedItem2.DroppedBy = base.baseEntity.userID;
					Analytics.Azure.OnItemDropped(base.baseEntity, droppedItem2, DroppedItem.DropReasonEnum.Player);
				}
			}
			base.baseEntity.SignalBroadcast(global::BaseEntity.Signal.Gesture, "drop_item", null);
			return;
		}
		item.ServerCommand(text, base.baseEntity);
		ItemManager.DoRemoves();
		this.ServerUpdate(0f);
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x000852D4 File Offset: 0x000834D4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void MoveItem(global::BaseEntity.RPCMessage msg)
	{
		ItemId itemId = msg.read.ItemID();
		ItemContainerId itemContainerId = msg.read.ItemContainerID();
		int num = (int)msg.read.Int8();
		int num2 = (int)msg.read.UInt32();
		bool flag = msg.read.Bit();
		global::Item item = this.FindItemUID(itemId);
		if (item == null)
		{
			msg.player.ChatMessage("Invalid item (" + itemId + ")");
			return;
		}
		global::BaseEntity entityOwner = item.GetEntityOwner();
		if (!this.CanMoveItemsFrom(entityOwner, item))
		{
			msg.player.ChatMessage("Cannot move item!");
			return;
		}
		if (num2 <= 0)
		{
			num2 = item.amount;
		}
		num2 = Mathf.Clamp(num2, 1, item.MaxStackable());
		if (msg.player.GetActiveItem() == item)
		{
			msg.player.UpdateActiveItem(default(ItemId));
		}
		if (!itemContainerId.IsValid)
		{
			global::BaseEntity baseEntity = entityOwner;
			if (this.loot.containers.Count > 0)
			{
				if (entityOwner == base.baseEntity)
				{
					if (!flag)
					{
						baseEntity = this.loot.entitySource;
					}
				}
				else
				{
					baseEntity = base.baseEntity;
				}
			}
			IIdealSlotEntity idealSlotEntity;
			if ((idealSlotEntity = baseEntity as IIdealSlotEntity) != null)
			{
				itemContainerId = idealSlotEntity.GetIdealContainer(base.baseEntity, item, flag);
			}
			global::ItemContainer parent = item.parent;
			if (parent != null && parent.IsLocked())
			{
				msg.player.ChatMessage("Container is locked!");
				return;
			}
			if (!itemContainerId.IsValid)
			{
				if (baseEntity == this.loot.entitySource)
				{
					foreach (global::ItemContainer itemContainer in this.loot.containers)
					{
						if (!itemContainer.PlayerItemInputBlocked() && !itemContainer.IsLocked() && item.MoveToContainer(itemContainer, -1, true, false, base.baseEntity, true))
						{
							break;
						}
					}
					return;
				}
				if (!this.GiveItem(item, null, flag))
				{
					msg.player.ChatMessage("GiveItem failed!");
				}
				return;
			}
		}
		global::ItemContainer itemContainer2 = this.FindContainer(itemContainerId);
		if (itemContainer2 == null)
		{
			msg.player.ChatMessage("Invalid container (" + itemContainerId + ")");
			return;
		}
		if (itemContainer2.IsLocked())
		{
			msg.player.ChatMessage("Container is locked!");
			return;
		}
		if (itemContainer2.PlayerItemInputBlocked())
		{
			msg.player.ChatMessage("Container does not accept player items!");
			return;
		}
		using (TimeWarning.New("Split", 0))
		{
			if (item.amount > num2)
			{
				int num3 = num2;
				if (itemContainer2.maxStackSize > 0)
				{
					num3 = Mathf.Min(num2, itemContainer2.maxStackSize);
				}
				global::Item item2 = item.SplitItem(num3);
				if (!item2.MoveToContainer(itemContainer2, num, true, false, base.baseEntity, true))
				{
					item.amount += item2.amount;
					item2.Remove(0f);
				}
				ItemManager.DoRemoves();
				this.ServerUpdate(0f);
				return;
			}
		}
		if (!item.MoveToContainer(itemContainer2, num, true, false, base.baseEntity, true))
		{
			return;
		}
		ItemManager.DoRemoves();
		this.ServerUpdate(0f);
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x0008561C File Offset: 0x0008381C
	private void OnClothingChanged(global::Item item, bool bAdded)
	{
		base.baseEntity.SV_ClothingChanged();
		ItemManager.DoRemoves();
		this.ServerUpdate(0f);
	}

	// Token: 0x06000FF7 RID: 4087 RVA: 0x00085639 File Offset: 0x00083839
	private void OnItemRemoved(global::Item item)
	{
		base.baseEntity.InvalidateNetworkCache();
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x00085648 File Offset: 0x00083848
	private bool CanEquipItem(global::Item item, int targetSlot)
	{
		ItemModContainerRestriction component = item.info.GetComponent<ItemModContainerRestriction>();
		if (component == null)
		{
			return true;
		}
		foreach (global::Item item2 in this.containerBelt.itemList.ToArray())
		{
			if (item2 != item)
			{
				ItemModContainerRestriction component2 = item2.info.GetComponent<ItemModContainerRestriction>();
				if (!(component2 == null) && !component.CanExistWith(component2) && !item2.MoveToContainer(this.containerMain, -1, true, false, null, true))
				{
					item2.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity(), default(Quaternion));
				}
			}
		}
		return true;
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x000856EF File Offset: 0x000838EF
	private bool CanWearItem(global::Item item, int targetSlot)
	{
		return this.CanWearItem(item, true);
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x000856FC File Offset: 0x000838FC
	private bool CanWearItem(global::Item item, bool canAdjustClothing)
	{
		ItemModWearable component = item.info.GetComponent<ItemModWearable>();
		if (component == null)
		{
			return false;
		}
		if (component.npcOnly && !Inventory.disableAttireLimitations)
		{
			global::BasePlayer baseEntity = base.baseEntity;
			if (baseEntity != null && !baseEntity.IsNpc)
			{
				return false;
			}
		}
		foreach (global::Item item2 in this.containerWear.itemList.ToArray())
		{
			if (item2 != item)
			{
				ItemModWearable component2 = item2.info.GetComponent<ItemModWearable>();
				if (!(component2 == null) && !Inventory.disableAttireLimitations && !component.CanExistWith(component2))
				{
					if (!canAdjustClothing)
					{
						return false;
					}
					bool flag = false;
					if (item.parent == this.containerBelt)
					{
						flag = item2.MoveToContainer(this.containerBelt, -1, true, false, null, true);
					}
					if (!flag && !item2.MoveToContainer(this.containerMain, -1, true, false, null, true))
					{
						item2.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity(), default(Quaternion));
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x00085814 File Offset: 0x00083A14
	public void ServerUpdate(float delta)
	{
		this.loot.Check();
		if (delta > 0f)
		{
			this.crafting.ServerUpdate(delta);
		}
		float currentTemperature = base.baseEntity.currentTemperature;
		this.UpdateContainer(delta, global::PlayerInventory.Type.Main, this.containerMain, false, currentTemperature);
		this.UpdateContainer(delta, global::PlayerInventory.Type.Belt, this.containerBelt, true, currentTemperature);
		this.UpdateContainer(delta, global::PlayerInventory.Type.Wear, this.containerWear, true, currentTemperature);
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x0008587C File Offset: 0x00083A7C
	public void UpdateContainer(float delta, global::PlayerInventory.Type type, global::ItemContainer container, bool bSendInventoryToEveryone, float temperature)
	{
		if (container == null)
		{
			return;
		}
		container.temperature = temperature;
		if (delta > 0f)
		{
			container.OnCycle(delta);
		}
		if (container.dirty)
		{
			this.SendUpdatedInventory(type, container, bSendInventoryToEveryone);
			base.baseEntity.InvalidateNetworkCache();
		}
	}

	// Token: 0x06000FFD RID: 4093 RVA: 0x000858B8 File Offset: 0x00083AB8
	public void SendSnapshot()
	{
		using (TimeWarning.New("PlayerInventory.SendSnapshot", 0))
		{
			this.SendUpdatedInventory(global::PlayerInventory.Type.Main, this.containerMain, false);
			this.SendUpdatedInventory(global::PlayerInventory.Type.Belt, this.containerBelt, true);
			this.SendUpdatedInventory(global::PlayerInventory.Type.Wear, this.containerWear, true);
		}
	}

	// Token: 0x06000FFE RID: 4094 RVA: 0x00085918 File Offset: 0x00083B18
	public void SendUpdatedInventory(global::PlayerInventory.Type type, global::ItemContainer container, bool bSendInventoryToEveryone = false)
	{
		using (UpdateItemContainer updateItemContainer = Facepunch.Pool.Get<UpdateItemContainer>())
		{
			updateItemContainer.type = (int)type;
			if (container != null)
			{
				container.dirty = false;
				updateItemContainer.container = Facepunch.Pool.Get<List<ProtoBuf.ItemContainer>>();
				updateItemContainer.container.Add(container.Save());
			}
			if (bSendInventoryToEveryone)
			{
				base.baseEntity.ClientRPC<UpdateItemContainer>(null, "UpdatedItemContainer", updateItemContainer);
			}
			else
			{
				base.baseEntity.ClientRPCPlayer<UpdateItemContainer>(null, base.baseEntity, "UpdatedItemContainer", updateItemContainer);
			}
		}
	}

	// Token: 0x06000FFF RID: 4095 RVA: 0x000859A4 File Offset: 0x00083BA4
	public global::Item FindItemUID(ItemId id)
	{
		if (!id.IsValid)
		{
			return null;
		}
		if (this.containerMain != null)
		{
			global::Item item = this.containerMain.FindItemByUID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (this.containerBelt != null)
		{
			global::Item item2 = this.containerBelt.FindItemByUID(id);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (this.containerWear != null)
		{
			global::Item item3 = this.containerWear.FindItemByUID(id);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return this.loot.FindItem(id);
	}

	// Token: 0x06001000 RID: 4096 RVA: 0x00085A30 File Offset: 0x00083C30
	public global::Item FindItemID(string itemName)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemName);
		if (itemDefinition == null)
		{
			return null;
		}
		return this.FindItemID(itemDefinition.itemid);
	}

	// Token: 0x06001001 RID: 4097 RVA: 0x00085A5C File Offset: 0x00083C5C
	public global::Item FindItemID(int id)
	{
		if (this.containerMain != null)
		{
			global::Item item = this.containerMain.FindItemByItemID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (this.containerBelt != null)
		{
			global::Item item2 = this.containerBelt.FindItemByItemID(id);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (this.containerWear != null)
		{
			global::Item item3 = this.containerWear.FindItemByItemID(id);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return null;
	}

	// Token: 0x06001002 RID: 4098 RVA: 0x00085AD0 File Offset: 0x00083CD0
	public global::Item FindBySubEntityID(NetworkableId subEntityID)
	{
		if (this.containerMain != null)
		{
			global::Item item = this.containerMain.FindBySubEntityID(subEntityID);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (this.containerBelt != null)
		{
			global::Item item2 = this.containerBelt.FindBySubEntityID(subEntityID);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (this.containerWear != null)
		{
			global::Item item3 = this.containerWear.FindBySubEntityID(subEntityID);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return null;
	}

	// Token: 0x06001003 RID: 4099 RVA: 0x00085B44 File Offset: 0x00083D44
	public List<global::Item> FindItemIDs(int id)
	{
		List<global::Item> list = new List<global::Item>();
		if (this.containerMain != null)
		{
			list.AddRange(this.containerMain.FindItemsByItemID(id));
		}
		if (this.containerBelt != null)
		{
			list.AddRange(this.containerBelt.FindItemsByItemID(id));
		}
		if (this.containerWear != null)
		{
			list.AddRange(this.containerWear.FindItemsByItemID(id));
		}
		return list;
	}

	// Token: 0x06001004 RID: 4100 RVA: 0x00085BA8 File Offset: 0x00083DA8
	public global::ItemContainer FindContainer(ItemContainerId id)
	{
		global::ItemContainer itemContainer2;
		using (TimeWarning.New("FindContainer", 0))
		{
			global::ItemContainer itemContainer = this.containerMain.FindContainer(id);
			if (itemContainer != null)
			{
				itemContainer2 = itemContainer;
			}
			else
			{
				itemContainer = this.containerBelt.FindContainer(id);
				if (itemContainer != null)
				{
					itemContainer2 = itemContainer;
				}
				else
				{
					itemContainer = this.containerWear.FindContainer(id);
					if (itemContainer != null)
					{
						itemContainer2 = itemContainer;
					}
					else
					{
						itemContainer2 = this.loot.FindContainer(id);
					}
				}
			}
		}
		return itemContainer2;
	}

	// Token: 0x06001005 RID: 4101 RVA: 0x00085C28 File Offset: 0x00083E28
	public global::ItemContainer GetContainer(global::PlayerInventory.Type id)
	{
		if (id == global::PlayerInventory.Type.Main)
		{
			return this.containerMain;
		}
		if (global::PlayerInventory.Type.Belt == id)
		{
			return this.containerBelt;
		}
		if (global::PlayerInventory.Type.Wear == id)
		{
			return this.containerWear;
		}
		return null;
	}

	// Token: 0x06001006 RID: 4102 RVA: 0x00085C4C File Offset: 0x00083E4C
	public bool GiveItem(global::Item item, global::ItemContainer container = null, bool tryWearClothing = false)
	{
		if (item == null)
		{
			return false;
		}
		if (container == null)
		{
			this.GetIdealPickupContainer(item, ref container, tryWearClothing);
		}
		return (container != null && item.MoveToContainer(container, -1, true, false, null, true)) || item.MoveToContainer(this.containerMain, -1, true, false, null, true) || item.MoveToContainer(this.containerBelt, -1, true, false, null, true);
	}

	// Token: 0x06001007 RID: 4103 RVA: 0x00085CAC File Offset: 0x00083EAC
	protected void GetIdealPickupContainer(global::Item item, ref global::ItemContainer container, bool tryWearClothing)
	{
		if (item.MaxStackable() > 1)
		{
			if (this.containerBelt != null && this.containerBelt.FindItemByItemID(item.info.itemid) != null)
			{
				container = this.containerBelt;
				return;
			}
			if (this.containerMain != null && this.containerMain.FindItemByItemID(item.info.itemid) != null)
			{
				container = this.containerMain;
				return;
			}
		}
		if (tryWearClothing && item.info.isWearable && this.CanWearItem(item, false))
		{
			container = this.containerWear;
			return;
		}
		if (item.info.isUsable && !item.info.HasFlag(ItemDefinition.Flag.NotStraightToBelt))
		{
			container = this.containerBelt;
			return;
		}
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x00085D5B File Offset: 0x00083F5B
	public void Strip()
	{
		this.containerMain.Clear();
		this.containerBelt.Clear();
		this.containerWear.Clear();
		ItemManager.DoRemoves();
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x00085D84 File Offset: 0x00083F84
	public static bool IsBirthday()
	{
		if (global::PlayerInventory.forceBirthday)
		{
			return true;
		}
		if (UnityEngine.Time.time < global::PlayerInventory.nextCheckTime)
		{
			return global::PlayerInventory.wasBirthday;
		}
		global::PlayerInventory.nextCheckTime = UnityEngine.Time.time + 60f;
		DateTime now = DateTime.Now;
		global::PlayerInventory.wasBirthday = now.Day == 11 && now.Month == 12;
		return global::PlayerInventory.wasBirthday;
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x00085DE5 File Offset: 0x00083FE5
	public static bool IsChristmas()
	{
		return XMas.enabled;
	}

	// Token: 0x0600100B RID: 4107 RVA: 0x00085DEC File Offset: 0x00083FEC
	public void GiveDefaultItems()
	{
		this.Strip();
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode != null && activeGameMode.HasLoadouts())
		{
			BaseGameMode.GetActiveGameMode(true).LoadoutPlayer(base.baseEntity);
			return;
		}
		this.<GiveDefaultItems>g__GiveDefaultItemWithSkin|41_0("client.rockskin", "rock");
		this.<GiveDefaultItems>g__GiveDefaultItemWithSkin|41_0("client.torchskin", "torch");
		if (global::PlayerInventory.IsBirthday())
		{
			this.GiveItem(ItemManager.CreateByName("cakefiveyear", 1, 0UL), this.containerBelt, false);
			this.GiveItem(ItemManager.CreateByName("partyhat", 1, 0UL), this.containerWear, false);
		}
		if (global::PlayerInventory.IsChristmas())
		{
			this.GiveItem(ItemManager.CreateByName("snowball", 1, 0UL), this.containerBelt, false);
			this.GiveItem(ItemManager.CreateByName("snowball", 1, 0UL), this.containerBelt, false);
			this.GiveItem(ItemManager.CreateByName("snowball", 1, 0UL), this.containerBelt, false);
		}
	}

	// Token: 0x0600100C RID: 4108 RVA: 0x00085EE0 File Offset: 0x000840E0
	public ProtoBuf.PlayerInventory Save(bool bForDisk)
	{
		ProtoBuf.PlayerInventory playerInventory = Facepunch.Pool.Get<ProtoBuf.PlayerInventory>();
		if (bForDisk)
		{
			playerInventory.invMain = this.containerMain.Save();
		}
		playerInventory.invBelt = this.containerBelt.Save();
		playerInventory.invWear = this.containerWear.Save();
		return playerInventory;
	}

	// Token: 0x0600100D RID: 4109 RVA: 0x00085F2C File Offset: 0x0008412C
	public void Load(ProtoBuf.PlayerInventory msg)
	{
		if (msg.invMain != null)
		{
			this.containerMain.Load(msg.invMain);
		}
		if (msg.invBelt != null)
		{
			this.containerBelt.Load(msg.invBelt);
		}
		if (msg.invWear != null)
		{
			this.containerWear.Load(msg.invWear);
		}
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x00085F84 File Offset: 0x00084184
	public int Take(List<global::Item> collect, int itemid, int amount)
	{
		int num = 0;
		if (this.containerMain != null)
		{
			int num2 = this.containerMain.Take(collect, itemid, amount);
			num += num2;
			amount -= num2;
		}
		if (amount <= 0)
		{
			return num;
		}
		if (this.containerBelt != null)
		{
			int num3 = this.containerBelt.Take(collect, itemid, amount);
			num += num3;
			amount -= num3;
		}
		if (amount <= 0)
		{
			return num;
		}
		if (this.containerWear != null)
		{
			int num4 = this.containerWear.Take(collect, itemid, amount);
			num += num4;
			amount -= num4;
		}
		return num;
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x00086000 File Offset: 0x00084200
	public int GetAmount(int itemid)
	{
		if (itemid == 0)
		{
			return 0;
		}
		int num = 0;
		if (this.containerMain != null)
		{
			num += this.containerMain.GetAmount(itemid, true);
		}
		if (this.containerBelt != null)
		{
			num += this.containerBelt.GetAmount(itemid, true);
		}
		if (this.containerWear != null)
		{
			num += this.containerWear.GetAmount(itemid, true);
		}
		return num;
	}

	// Token: 0x06001010 RID: 4112 RVA: 0x00086060 File Offset: 0x00084260
	public global::Item[] AllItems()
	{
		List<global::Item> list = new List<global::Item>();
		if (this.containerMain != null)
		{
			list.AddRange(this.containerMain.itemList);
		}
		if (this.containerBelt != null)
		{
			list.AddRange(this.containerBelt.itemList);
		}
		if (this.containerWear != null)
		{
			list.AddRange(this.containerWear.itemList);
		}
		return list.ToArray();
	}

	// Token: 0x06001011 RID: 4113 RVA: 0x000860C4 File Offset: 0x000842C4
	public int AllItemsNoAlloc(ref List<global::Item> items)
	{
		items.Clear();
		if (this.containerMain != null)
		{
			items.AddRange(this.containerMain.itemList);
		}
		if (this.containerBelt != null)
		{
			items.AddRange(this.containerBelt.itemList);
		}
		if (this.containerWear != null)
		{
			items.AddRange(this.containerWear.itemList);
		}
		return items.Count;
	}

	// Token: 0x06001012 RID: 4114 RVA: 0x0008612D File Offset: 0x0008432D
	public void FindAmmo(List<global::Item> list, AmmoTypes ammoType)
	{
		if (this.containerMain != null)
		{
			this.containerMain.FindAmmo(list, ammoType);
		}
		if (this.containerBelt != null)
		{
			this.containerBelt.FindAmmo(list, ammoType);
		}
	}

	// Token: 0x06001013 RID: 4115 RVA: 0x00086159 File Offset: 0x00084359
	public bool HasAmmo(AmmoTypes ammoType)
	{
		return this.containerMain.HasAmmo(ammoType) || this.containerBelt.HasAmmo(ammoType);
	}

	// Token: 0x06001016 RID: 4118 RVA: 0x00086180 File Offset: 0x00084380
	[CompilerGenerated]
	private void <GiveDefaultItems>g__GiveDefaultItemWithSkin|41_0(string convarSkinName, string itemShortName)
	{
		ulong num = 0UL;
		int infoInt = base.baseEntity.GetInfoInt(convarSkinName, 0);
		bool flag = false;
		global::BasePlayer baseEntity = base.baseEntity;
		bool flag2 = baseEntity != null && baseEntity.UnlockAllSkins;
		if (infoInt > 0 && (base.baseEntity.blueprints.CheckSkinOwnership(infoInt, base.baseEntity.userID) || flag2))
		{
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemShortName);
			if (itemDefinition != null && ItemDefinition.FindSkin(itemDefinition.itemid, infoInt) != 0UL)
			{
				IPlayerItemDefinition itemDefinition2 = PlatformService.Instance.GetItemDefinition(infoInt);
				if (itemDefinition2 != null)
				{
					num = itemDefinition2.WorkshopDownload;
				}
				if (num == 0UL && itemDefinition.skins != null)
				{
					foreach (ItemSkinDirectory.Skin skin in itemDefinition.skins)
					{
						ItemSkin itemSkin;
						if (skin.id == infoInt && skin.invItem != null && (itemSkin = skin.invItem as ItemSkin) != null && itemSkin.Redirect != null)
						{
							this.GiveItem(ItemManager.CreateByName(itemSkin.Redirect.shortname, 1, 0UL), this.containerBelt, false);
							flag = true;
							break;
						}
					}
				}
			}
		}
		if (!flag)
		{
			this.GiveItem(ItemManager.CreateByName(itemShortName, 1, num), this.containerBelt, false);
		}
	}

	// Token: 0x04000A4C RID: 2636
	public global::ItemContainer containerMain;

	// Token: 0x04000A4D RID: 2637
	public global::ItemContainer containerBelt;

	// Token: 0x04000A4E RID: 2638
	public global::ItemContainer containerWear;

	// Token: 0x04000A4F RID: 2639
	public ItemCrafter crafting;

	// Token: 0x04000A50 RID: 2640
	public PlayerLoot loot;

	// Token: 0x04000A51 RID: 2641
	[ServerVar]
	public static bool forceBirthday;

	// Token: 0x04000A52 RID: 2642
	private static float nextCheckTime;

	// Token: 0x04000A53 RID: 2643
	private static bool wasBirthday;

	// Token: 0x02000BFB RID: 3067
	public enum Type
	{
		// Token: 0x04004205 RID: 16901
		Main,
		// Token: 0x04004206 RID: 16902
		Belt,
		// Token: 0x04004207 RID: 16903
		Wear
	}

	// Token: 0x02000BFC RID: 3068
	public interface ICanMoveFrom
	{
		// Token: 0x06004E04 RID: 19972
		bool CanMoveFrom(global::BasePlayer player, global::Item item);
	}
}
