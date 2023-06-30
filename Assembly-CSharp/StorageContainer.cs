using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000DC RID: 220
public class StorageContainer : global::DecayEntity, IItemContainerEntity, IIdealSlotEntity, ILootableEntity, LootPanel.IHasLootPanel, IContainerSounds, global::PlayerInventory.ICanMoveFrom
{
	// Token: 0x06001344 RID: 4932 RVA: 0x0009AB54 File Offset: 0x00098D54
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StorageContainer.OnRpcMessage", 0))
		{
			if (rpc == 331989034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenLoot ");
				}
				using (TimeWarning.New("RPC_OpenLoot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(331989034U, "RPC_OpenLoot", this, player, 3f))
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
							this.RPC_OpenLoot(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001BA RID: 442
	// (get) Token: 0x06001345 RID: 4933 RVA: 0x0009ACBC File Offset: 0x00098EBC
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return this.panelTitle;
		}
	}

	// Token: 0x06001346 RID: 4934 RVA: 0x0009ACC4 File Offset: 0x00098EC4
	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer && this.inventory != null)
		{
			this.inventory.Clear();
			this.inventory = null;
		}
	}

	// Token: 0x06001347 RID: 4935 RVA: 0x0009ACF0 File Offset: 0x00098EF0
	public virtual void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(this.dropPosition, Vector3.one * 0.1f);
		Gizmos.color = Color.white;
		Gizmos.DrawRay(this.dropPosition, this.dropVelocity);
	}

	// Token: 0x170001BB RID: 443
	// (get) Token: 0x06001348 RID: 4936 RVA: 0x0009AD4C File Offset: 0x00098F4C
	// (set) Token: 0x06001349 RID: 4937 RVA: 0x0009AD54 File Offset: 0x00098F54
	public global::ItemContainer inventory { get; private set; }

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x0600134A RID: 4938 RVA: 0x0005DDD5 File Offset: 0x0005BFD5
	public Transform Transform
	{
		get
		{
			return base.transform;
		}
	}

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x0600134B RID: 4939 RVA: 0x0009AD5D File Offset: 0x00098F5D
	public bool DropsLoot
	{
		get
		{
			return this.dropsLoot;
		}
	}

	// Token: 0x170001BE RID: 446
	// (get) Token: 0x0600134C RID: 4940 RVA: 0x0009AD65 File Offset: 0x00098F65
	public bool DropFloats
	{
		get
		{
			return this.dropFloats;
		}
	}

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x0600134D RID: 4941 RVA: 0x0009AD6D File Offset: 0x00098F6D
	public float DestroyLootPercent
	{
		get
		{
			return this.dropLootDestroyPercent;
		}
	}

	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x0600134E RID: 4942 RVA: 0x0009AD75 File Offset: 0x00098F75
	// (set) Token: 0x0600134F RID: 4943 RVA: 0x0009AD7D File Offset: 0x00098F7D
	public ulong LastLootedBy { get; set; }

	// Token: 0x06001350 RID: 4944 RVA: 0x0009AD86 File Offset: 0x00098F86
	public bool MoveAllInventoryItems(global::ItemContainer from)
	{
		return StorageContainer.MoveAllInventoryItems(from, this.inventory);
	}

	// Token: 0x06001351 RID: 4945 RVA: 0x0009AD94 File Offset: 0x00098F94
	public static bool MoveAllInventoryItems(global::ItemContainer source, global::ItemContainer dest)
	{
		bool flag = true;
		for (int i = 0; i < Mathf.Min(source.capacity, dest.capacity); i++)
		{
			global::Item slot = source.GetSlot(i);
			if (slot != null)
			{
				bool flag2 = slot.MoveToContainer(dest, -1, true, false, null, true);
				if (flag && !flag2)
				{
					flag = false;
				}
			}
		}
		return flag;
	}

	// Token: 0x06001352 RID: 4946 RVA: 0x0009ADE0 File Offset: 0x00098FE0
	public virtual void ReceiveInventoryFromItem(global::Item item)
	{
		if (item.contents != null)
		{
			StorageContainer.MoveAllInventoryItems(item.contents, this.inventory);
		}
	}

	// Token: 0x06001353 RID: 4947 RVA: 0x0009ADFC File Offset: 0x00098FFC
	public override bool CanPickup(global::BasePlayer player)
	{
		bool flag = base.GetSlot(global::BaseEntity.Slot.Lock) != null;
		if (base.isClient)
		{
			return base.CanPickup(player) && !flag;
		}
		return (!this.pickup.requireEmptyInv || this.inventory == null || this.inventory.itemList.Count == 0) && base.CanPickup(player) && !flag;
	}

	// Token: 0x06001354 RID: 4948 RVA: 0x0009AE68 File Offset: 0x00099068
	public override void OnPickedUp(global::Item createdItem, global::BasePlayer player)
	{
		base.OnPickedUp(createdItem, player);
		if (createdItem != null && createdItem.contents != null)
		{
			StorageContainer.MoveAllInventoryItems(this.inventory, createdItem.contents);
			return;
		}
		for (int i = 0; i < this.inventory.capacity; i++)
		{
			global::Item slot = this.inventory.GetSlot(i);
			if (slot != null)
			{
				slot.RemoveFromContainer();
				player.GiveItem(slot, global::BaseEntity.GiveItemReason.PickedUp);
			}
		}
	}

	// Token: 0x06001355 RID: 4949 RVA: 0x0009AECF File Offset: 0x000990CF
	public override void ServerInit()
	{
		if (this.inventory == null)
		{
			this.CreateInventory(true);
			this.OnInventoryFirstCreated(this.inventory);
		}
		base.ServerInit();
	}

	// Token: 0x06001356 RID: 4950 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnInventoryFirstCreated(global::ItemContainer container)
	{
	}

	// Token: 0x06001357 RID: 4951 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnItemAddedOrRemoved(global::Item item, bool added)
	{
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x0009AEF2 File Offset: 0x000990F2
	public virtual bool ItemFilter(global::Item item, int targetSlot)
	{
		return this.onlyAcceptCategory == ItemCategory.All || item.info.category == this.onlyAcceptCategory;
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x0009AF14 File Offset: 0x00099114
	public void CreateInventory(bool giveUID)
	{
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.allowedContents = ((this.allowedContents == (global::ItemContainer.ContentsType)0) ? global::ItemContainer.ContentsType.Generic : this.allowedContents);
		this.inventory.SetOnlyAllowedItems(new ItemDefinition[] { this.allowedItem, this.allowedItem2 });
		this.inventory.maxStackSize = this.maxStackSize;
		this.inventory.ServerInitialize(null, this.inventorySlots);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
		this.inventory.onDirty += this.OnInventoryDirty;
		this.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		this.inventory.canAcceptItem = new Func<global::Item, int, bool>(this.ItemFilter);
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x0009AFF0 File Offset: 0x000991F0
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.CreateInventory(false);
	}

	// Token: 0x0600135B RID: 4955 RVA: 0x0009AFFF File Offset: 0x000991FF
	protected virtual void OnInventoryDirty()
	{
		base.InvalidateNetworkCache();
	}

	// Token: 0x0600135C RID: 4956 RVA: 0x0009B007 File Offset: 0x00099207
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.inventory != null && !this.inventory.uid.IsValid)
		{
			this.inventory.GiveUID();
		}
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x0009B03E File Offset: 0x0009923E
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.inventory != null)
		{
			this.inventory.Kill();
			this.inventory = null;
		}
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x0009B060 File Offset: 0x00099260
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage rpc)
	{
		if (!this.isLootable)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		this.PlayerOpenLoot(player, "", true);
	}

	// Token: 0x0600135F RID: 4959 RVA: 0x0009B09C File Offset: 0x0009929C
	public virtual string GetPanelName()
	{
		return this.panelName;
	}

	// Token: 0x06001360 RID: 4960 RVA: 0x0009B0A4 File Offset: 0x000992A4
	public virtual bool CanMoveFrom(global::BasePlayer player, global::Item item)
	{
		return !this.inventory.IsLocked();
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x0009B0B4 File Offset: 0x000992B4
	public virtual bool CanOpenLootPanel(global::BasePlayer player, string panelName)
	{
		if (!this.CanBeLooted(player))
		{
			return false;
		}
		BaseLock baseLock = base.GetSlot(global::BaseEntity.Slot.Lock) as BaseLock;
		if (baseLock != null && !baseLock.OnTryToOpen(player))
		{
			player.ChatMessage("It is locked...");
			return false;
		}
		return true;
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x0009B0F9 File Offset: 0x000992F9
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return (!this.needsBuildingPrivilegeToUse || player.CanBuild()) && (!this.mustBeMountedToUse || player.isMounted) && base.CanBeLooted(player);
	}

	// Token: 0x06001363 RID: 4963 RVA: 0x0009B126 File Offset: 0x00099326
	public virtual void AddContainers(PlayerLoot loot)
	{
		loot.AddContainer(this.inventory);
	}

	// Token: 0x06001364 RID: 4964 RVA: 0x0009B134 File Offset: 0x00099334
	public virtual bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (base.IsLocked())
		{
			player.ShowToast(GameTip.Styles.Red_Normal, StorageContainer.LockedMessage, Array.Empty<string>());
			return false;
		}
		if (this.onlyOneUser && base.IsOpen())
		{
			player.ShowToast(GameTip.Styles.Red_Normal, StorageContainer.InUseMessage, Array.Empty<string>());
			return false;
		}
		if (panelToOpen == "")
		{
			panelToOpen = this.panelName;
		}
		if (!this.CanOpenLootPanel(player, panelToOpen))
		{
			return false;
		}
		if (player.inventory.loot.StartLootingEntity(this, doPositionChecks))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			this.AddContainers(player.inventory.loot);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", panelToOpen);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return true;
		}
		return false;
	}

	// Token: 0x06001365 RID: 4965 RVA: 0x0005E0C2 File Offset: 0x0005C2C2
	public virtual void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001366 RID: 4966 RVA: 0x0009B1F8 File Offset: 0x000993F8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x06001367 RID: 4967 RVA: 0x0009B25D File Offset: 0x0009945D
	public override void OnKilled(HitInfo info)
	{
		this.DropItems((info != null) ? info.Initiator : null);
		base.OnKilled(info);
	}

	// Token: 0x06001368 RID: 4968 RVA: 0x0005DFC5 File Offset: 0x0005C1C5
	public void DropItems(global::BaseEntity initiator = null)
	{
		StorageContainer.DropItems(this, initiator);
	}

	// Token: 0x06001369 RID: 4969 RVA: 0x0009B278 File Offset: 0x00099478
	public static void DropItems(IItemContainerEntity containerEntity, global::BaseEntity initiator = null)
	{
		global::ItemContainer inventory = containerEntity.inventory;
		if (inventory == null || inventory.itemList == null || inventory.itemList.Count == 0)
		{
			return;
		}
		if (!containerEntity.DropsLoot)
		{
			return;
		}
		if (containerEntity.ShouldDropItemsIndividually() || (inventory.itemList.Count == 1 && !containerEntity.DropFloats))
		{
			if (initiator != null)
			{
				containerEntity.DropBonusItems(initiator, inventory);
			}
			DropUtil.DropItems(inventory, containerEntity.GetDropPosition());
			return;
		}
		string text = (containerEntity.DropFloats ? "assets/prefabs/misc/item drop/item_drop_buoyant.prefab" : "assets/prefabs/misc/item drop/item_drop.prefab");
		inventory.Drop(text, containerEntity.GetDropPosition(), containerEntity.Transform.rotation, containerEntity.DestroyLootPercent) != null;
	}

	// Token: 0x0600136A RID: 4970 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void DropBonusItems(global::BaseEntity initiator, global::ItemContainer container)
	{
	}

	// Token: 0x0600136B RID: 4971 RVA: 0x0009B324 File Offset: 0x00099524
	public override Vector3 GetDropPosition()
	{
		return base.transform.localToWorldMatrix.MultiplyPoint(this.dropPosition);
	}

	// Token: 0x0600136C RID: 4972 RVA: 0x0009B34C File Offset: 0x0009954C
	public override Vector3 GetDropVelocity()
	{
		return this.GetInheritedDropVelocity() + base.transform.localToWorldMatrix.MultiplyVector(this.dropPosition);
	}

	// Token: 0x0600136D RID: 4973 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool ShouldDropItemsIndividually()
	{
		return false;
	}

	// Token: 0x0600136E RID: 4974 RVA: 0x0009B380 File Offset: 0x00099580
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				this.inventory.capacity = this.inventorySlots;
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x0600136F RID: 4975 RVA: 0x0004485D File Offset: 0x00042A5D
	public virtual int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		return -1;
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x0009B3EC File Offset: 0x000995EC
	public virtual ItemContainerId GetIdealContainer(global::BasePlayer player, global::Item item, bool altMove)
	{
		return default(ItemContainerId);
	}

	// Token: 0x06001371 RID: 4977 RVA: 0x0009B402 File Offset: 0x00099602
	public override bool HasSlot(global::BaseEntity.Slot slot)
	{
		return (this.isLockable && slot == global::BaseEntity.Slot.Lock) || (this.isMonitorable && slot == global::BaseEntity.Slot.StorageMonitor) || base.HasSlot(slot);
	}

	// Token: 0x06001372 RID: 4978 RVA: 0x0009B426 File Offset: 0x00099626
	public bool OccupiedCheck(global::BasePlayer player = null)
	{
		return (player != null && player.inventory.loot.entitySource == this) || !this.onlyOneUser || !base.IsOpen();
	}

	// Token: 0x06001373 RID: 4979 RVA: 0x0009B460 File Offset: 0x00099660
	protected bool HasAttachedStorageAdaptor()
	{
		if (this.children == null)
		{
			return false;
		}
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is IndustrialStorageAdaptor)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04000C0B RID: 3083
	[Header("Storage Container")]
	public static readonly Translate.Phrase LockedMessage = new Translate.Phrase("storage.locked", "Can't loot right now");

	// Token: 0x04000C0C RID: 3084
	public static readonly Translate.Phrase InUseMessage = new Translate.Phrase("storage.in_use", "Already in use");

	// Token: 0x04000C0D RID: 3085
	public int inventorySlots = 6;

	// Token: 0x04000C0E RID: 3086
	public bool dropsLoot = true;

	// Token: 0x04000C0F RID: 3087
	public float dropLootDestroyPercent;

	// Token: 0x04000C10 RID: 3088
	public bool dropFloats;

	// Token: 0x04000C11 RID: 3089
	public bool isLootable = true;

	// Token: 0x04000C12 RID: 3090
	public bool isLockable = true;

	// Token: 0x04000C13 RID: 3091
	public bool isMonitorable;

	// Token: 0x04000C14 RID: 3092
	public string panelName = "generic";

	// Token: 0x04000C15 RID: 3093
	public Translate.Phrase panelTitle = new Translate.Phrase("loot", "Loot");

	// Token: 0x04000C16 RID: 3094
	public global::ItemContainer.ContentsType allowedContents;

	// Token: 0x04000C17 RID: 3095
	public ItemDefinition allowedItem;

	// Token: 0x04000C18 RID: 3096
	public ItemDefinition allowedItem2;

	// Token: 0x04000C19 RID: 3097
	public int maxStackSize;

	// Token: 0x04000C1A RID: 3098
	public bool needsBuildingPrivilegeToUse;

	// Token: 0x04000C1B RID: 3099
	public bool mustBeMountedToUse;

	// Token: 0x04000C1C RID: 3100
	public SoundDefinition openSound;

	// Token: 0x04000C1D RID: 3101
	public SoundDefinition closeSound;

	// Token: 0x04000C1E RID: 3102
	[Header("Item Dropping")]
	public Vector3 dropPosition;

	// Token: 0x04000C1F RID: 3103
	public Vector3 dropVelocity = Vector3.forward;

	// Token: 0x04000C20 RID: 3104
	public ItemCategory onlyAcceptCategory = ItemCategory.All;

	// Token: 0x04000C21 RID: 3105
	public bool onlyOneUser;
}
