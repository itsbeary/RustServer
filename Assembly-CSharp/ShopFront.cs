using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CA RID: 202
public class ShopFront : StorageContainer
{
	// Token: 0x06001200 RID: 4608 RVA: 0x00091B60 File Offset: 0x0008FD60
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ShopFront.OnRpcMessage", 0))
		{
			if (rpc == 1159607245U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AcceptClicked ");
				}
				using (TimeWarning.New("AcceptClicked", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1159607245U, "AcceptClicked", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.AcceptClicked(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AcceptClicked");
					}
				}
				return true;
			}
			if (rpc == 3168107540U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - CancelClicked ");
				}
				using (TimeWarning.New("CancelClicked", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3168107540U, "CancelClicked", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CancelClicked(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in CancelClicked");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700019C RID: 412
	// (get) Token: 0x06001201 RID: 4609 RVA: 0x00091E60 File Offset: 0x00090060
	private float AngleDotProduct
	{
		get
		{
			return 1f - this.maxUseAngle / 90f;
		}
	}

	// Token: 0x1700019D RID: 413
	// (get) Token: 0x06001202 RID: 4610 RVA: 0x000381F0 File Offset: 0x000363F0
	public ItemContainer vendorInventory
	{
		get
		{
			return base.inventory;
		}
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool TradeLocked()
	{
		return false;
	}

	// Token: 0x06001204 RID: 4612 RVA: 0x00091E74 File Offset: 0x00090074
	public bool IsTradingPlayer(BasePlayer player)
	{
		return player != null && (this.IsPlayerCustomer(player) || this.IsPlayerVendor(player));
	}

	// Token: 0x06001205 RID: 4613 RVA: 0x00091E93 File Offset: 0x00090093
	public bool IsPlayerCustomer(BasePlayer player)
	{
		return player == this.customerPlayer;
	}

	// Token: 0x06001206 RID: 4614 RVA: 0x00091EA1 File Offset: 0x000900A1
	public bool IsPlayerVendor(BasePlayer player)
	{
		return player == this.vendorPlayer;
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x00091EB0 File Offset: 0x000900B0
	public bool PlayerInVendorPos(BasePlayer player)
	{
		return Vector3.Dot(base.transform.right, (player.transform.position - base.transform.position).normalized) <= -this.AngleDotProduct;
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x00091EFC File Offset: 0x000900FC
	public bool PlayerInCustomerPos(BasePlayer player)
	{
		return Vector3.Dot(base.transform.right, (player.transform.position - base.transform.position).normalized) >= this.AngleDotProduct;
	}

	// Token: 0x06001209 RID: 4617 RVA: 0x00091F47 File Offset: 0x00090147
	public bool LootEligable(BasePlayer player)
	{
		return !(player == null) && ((this.PlayerInVendorPos(player) && this.vendorPlayer == null) || (this.PlayerInCustomerPos(player) && this.customerPlayer == null));
	}

	// Token: 0x0600120A RID: 4618 RVA: 0x00091F88 File Offset: 0x00090188
	public void ResetTrade()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		this.vendorInventory.SetLocked(false);
		this.customerInventory.SetLocked(false);
		base.CancelInvoke(new Action(this.CompleteTrade));
	}

	// Token: 0x0600120B RID: 4619 RVA: 0x00091FEC File Offset: 0x000901EC
	public void CompleteTrade()
	{
		if (this.vendorPlayer != null && this.customerPlayer != null && base.HasFlag(BaseEntity.Flags.Reserved1) && base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			try
			{
				this.swappingItems = true;
				for (int i = this.vendorInventory.capacity - 1; i >= 0; i--)
				{
					Item slot = this.vendorInventory.GetSlot(i);
					Item slot2 = this.customerInventory.GetSlot(i);
					if (this.customerPlayer && slot != null)
					{
						this.customerPlayer.GiveItem(slot, BaseEntity.GiveItemReason.Generic);
					}
					if (this.vendorPlayer && slot2 != null)
					{
						this.vendorPlayer.GiveItem(slot2, BaseEntity.GiveItemReason.Generic);
					}
				}
			}
			finally
			{
				this.swappingItems = false;
			}
			Effect.server.Run(this.transactionCompleteEffect.resourcePath, this, 0U, new Vector3(0f, 1f, 0f), Vector3.zero, null, false);
		}
		this.ResetTrade();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600120C RID: 4620 RVA: 0x00092104 File Offset: 0x00090304
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void AcceptClicked(BaseEntity.RPCMessage msg)
	{
		if (!this.IsTradingPlayer(msg.player))
		{
			return;
		}
		if (this.vendorPlayer == null || this.customerPlayer == null)
		{
			return;
		}
		if (this.IsPlayerVendor(msg.player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
			this.vendorInventory.SetLocked(true);
		}
		else if (this.IsPlayerCustomer(msg.player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
			this.customerInventory.SetLocked(true);
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved1) && base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
			base.Invoke(new Action(this.CompleteTrade), 2f);
		}
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x000921CE File Offset: 0x000903CE
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void CancelClicked(BaseEntity.RPCMessage msg)
	{
		if (!this.IsTradingPlayer(msg.player))
		{
			return;
		}
		this.vendorPlayer;
		this.customerPlayer;
		this.ResetTrade();
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x000921FD File Offset: 0x000903FD
	public override void PreServerLoad()
	{
		base.PreServerLoad();
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x00092208 File Offset: 0x00090408
	public override void ServerInit()
	{
		base.ServerInit();
		ItemContainer vendorInventory = this.vendorInventory;
		vendorInventory.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(vendorInventory.canAcceptItem, new Func<Item, int, bool>(this.CanAcceptVendorItem));
		if (this.customerInventory == null)
		{
			this.customerInventory = new ItemContainer();
			this.customerInventory.allowedContents = ((this.allowedContents == (ItemContainer.ContentsType)0) ? ItemContainer.ContentsType.Generic : this.allowedContents);
			this.customerInventory.SetOnlyAllowedItem(this.allowedItem);
			this.customerInventory.entityOwner = this;
			this.customerInventory.maxStackSize = this.maxStackSize;
			this.customerInventory.ServerInitialize(null, this.inventorySlots);
			this.customerInventory.GiveUID();
			this.customerInventory.onDirty += this.OnInventoryDirty;
			this.customerInventory.onItemAddedRemoved = new Action<Item, bool>(this.OnItemAddedOrRemoved);
			ItemContainer itemContainer = this.customerInventory;
			itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(this.CanAcceptCustomerItem));
			this.OnInventoryFirstCreated(this.customerInventory);
		}
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x00092322 File Offset: 0x00090522
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		this.ResetTrade();
	}

	// Token: 0x06001211 RID: 4625 RVA: 0x00092334 File Offset: 0x00090534
	private bool CanAcceptVendorItem(Item item, int targetSlot)
	{
		return this.swappingItems || (this.vendorPlayer != null && item.GetOwnerPlayer() == this.vendorPlayer) || this.vendorInventory.itemList.Contains(item);
	}

	// Token: 0x06001212 RID: 4626 RVA: 0x00092380 File Offset: 0x00090580
	private bool CanAcceptCustomerItem(Item item, int targetSlot)
	{
		return this.swappingItems || (this.customerPlayer != null && item.GetOwnerPlayer() == this.customerPlayer) || this.customerInventory.itemList.Contains(item);
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x000923CC File Offset: 0x000905CC
	public override bool CanMoveFrom(BasePlayer player, Item item)
	{
		if (this.TradeLocked())
		{
			return false;
		}
		if (this.IsTradingPlayer(player))
		{
			if (this.IsPlayerCustomer(player) && this.customerInventory.itemList.Contains(item) && !this.customerInventory.IsLocked())
			{
				return true;
			}
			if (this.IsPlayerVendor(player) && this.vendorInventory.itemList.Contains(item) && !this.vendorInventory.IsLocked())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x00092443 File Offset: 0x00090643
	public override bool CanOpenLootPanel(BasePlayer player, string panelName)
	{
		return base.CanOpenLootPanel(player, panelName) && this.LootEligable(player);
	}

	// Token: 0x06001215 RID: 4629 RVA: 0x00092458 File Offset: 0x00090658
	public void ReturnPlayerItems(BasePlayer player)
	{
		if (this.IsTradingPlayer(player))
		{
			ItemContainer itemContainer = null;
			if (this.IsPlayerVendor(player))
			{
				itemContainer = this.vendorInventory;
			}
			else if (this.IsPlayerCustomer(player))
			{
				itemContainer = this.customerInventory;
			}
			if (itemContainer != null)
			{
				for (int i = itemContainer.itemList.Count - 1; i >= 0; i--)
				{
					Item item = itemContainer.itemList[i];
					player.GiveItem(item, BaseEntity.GiveItemReason.Generic);
				}
			}
		}
	}

	// Token: 0x06001216 RID: 4630 RVA: 0x000924C4 File Offset: 0x000906C4
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		if (!this.IsTradingPlayer(player))
		{
			return;
		}
		this.ReturnPlayerItems(player);
		if (player == this.vendorPlayer)
		{
			this.vendorPlayer = null;
		}
		if (player == this.customerPlayer)
		{
			this.customerPlayer = null;
		}
		this.UpdatePlayers();
		this.ResetTrade();
		base.PlayerStoppedLooting(player);
	}

	// Token: 0x06001217 RID: 4631 RVA: 0x00092520 File Offset: 0x00090720
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		bool flag = base.PlayerOpenLoot(player, panelToOpen, true);
		if (flag)
		{
			player.inventory.loot.AddContainer(this.customerInventory);
			player.inventory.loot.SendImmediate();
		}
		if (this.PlayerInVendorPos(player) && this.vendorPlayer == null)
		{
			this.vendorPlayer = player;
		}
		else
		{
			if (!this.PlayerInCustomerPos(player) || !(this.customerPlayer == null))
			{
				return false;
			}
			this.customerPlayer = player;
		}
		this.ResetTrade();
		this.UpdatePlayers();
		return flag;
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x000925B0 File Offset: 0x000907B0
	public void UpdatePlayers()
	{
		base.ClientRPC<NetworkableId, NetworkableId>(null, "CLIENT_ReceivePlayers", (this.vendorPlayer == null) ? default(NetworkableId) : this.vendorPlayer.net.ID, (this.customerPlayer == null) ? default(NetworkableId) : this.customerPlayer.net.ID);
	}

	// Token: 0x04000B48 RID: 2888
	public float maxUseAngle = 27f;

	// Token: 0x04000B49 RID: 2889
	public BasePlayer vendorPlayer;

	// Token: 0x04000B4A RID: 2890
	public BasePlayer customerPlayer;

	// Token: 0x04000B4B RID: 2891
	public GameObjectRef transactionCompleteEffect;

	// Token: 0x04000B4C RID: 2892
	public ItemContainer customerInventory;

	// Token: 0x04000B4D RID: 2893
	private bool swappingItems;

	// Token: 0x02000C07 RID: 3079
	public static class ShopFrontFlags
	{
		// Token: 0x04004229 RID: 16937
		public const BaseEntity.Flags VendorAccepted = BaseEntity.Flags.Reserved1;

		// Token: 0x0400422A RID: 16938
		public const BaseEntity.Flags CustomerAccepted = BaseEntity.Flags.Reserved2;

		// Token: 0x0400422B RID: 16939
		public const BaseEntity.Flags Exchanging = BaseEntity.Flags.Reserved3;
	}
}
