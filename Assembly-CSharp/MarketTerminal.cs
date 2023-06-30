using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using ProtoBuf;
using Rust;
using Rust.UI;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009A RID: 154
public class MarketTerminal : StorageContainer
{
	// Token: 0x06000DFD RID: 3581 RVA: 0x00076240 File Offset: 0x00074440
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MarketTerminal.OnRpcMessage", 0))
		{
			if (rpc == 3793918752U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_Purchase ");
				}
				using (TimeWarning.New("Server_Purchase", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3793918752U, "Server_Purchase", this, player, 10UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3793918752U, "Server_Purchase", this, player, 3f))
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
							this.Server_Purchase(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_Purchase");
					}
				}
				return true;
			}
			if (rpc == 1382511247U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_TryOpenMarket ");
				}
				using (TimeWarning.New("Server_TryOpenMarket", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1382511247U, "Server_TryOpenMarket", this, player, 3UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1382511247U, "Server_TryOpenMarket", this, player, 3f))
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
							this.Server_TryOpenMarket(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Server_TryOpenMarket");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x00076578 File Offset: 0x00074778
	public void Setup(Marketplace marketplace)
	{
		this._marketplace = new EntityRef<Marketplace>(marketplace.net.ID);
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x00076590 File Offset: 0x00074790
	public override void ServerInit()
	{
		base.ServerInit();
		this._onCurrencyRemovedCached = new Action<global::BasePlayer, global::Item>(this.OnCurrencyRemoved);
		this._onItemPurchasedCached = new Action<global::BasePlayer, global::Item>(this.OnItemPurchased);
		this._checkForExpiredOrdersCached = new Action(this.CheckForExpiredOrders);
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x000765D0 File Offset: 0x000747D0
	private void RegisterOrder(global::BasePlayer player, global::VendingMachine vendingMachine)
	{
		if (this.pendingOrders == null)
		{
			this.pendingOrders = Facepunch.Pool.GetList<ProtoBuf.MarketTerminal.PendingOrder>();
		}
		if (this.HasPendingOrderFor(vendingMachine.net.ID))
		{
			return;
		}
		Marketplace marketplace;
		if (!this._marketplace.TryGet(true, out marketplace))
		{
			Debug.LogError("Marketplace is not set", this);
			return;
		}
		NetworkableId networkableId = marketplace.SendDrone(player, this, vendingMachine);
		if (!networkableId.IsValid)
		{
			Debug.LogError("Failed to spawn delivery drone");
			return;
		}
		ProtoBuf.MarketTerminal.PendingOrder pendingOrder = Facepunch.Pool.Get<ProtoBuf.MarketTerminal.PendingOrder>();
		pendingOrder.vendingMachineId = vendingMachine.net.ID;
		pendingOrder.timeUntilExpiry = this.orderTimeout;
		pendingOrder.droneId = networkableId;
		this.pendingOrders.Add(pendingOrder);
		this.CheckForExpiredOrders();
		this.UpdateHasItems(false);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x00076690 File Offset: 0x00074890
	public void CompleteOrder(NetworkableId vendingMachineId)
	{
		if (this.pendingOrders == null)
		{
			return;
		}
		int num = this.pendingOrders.FindIndexWith((ProtoBuf.MarketTerminal.PendingOrder o) => o.vendingMachineId, vendingMachineId);
		if (num < 0)
		{
			Debug.LogError("Completed market order that doesn't exist?");
			return;
		}
		this.pendingOrders[num].Dispose();
		this.pendingOrders.RemoveAt(num);
		this.CheckForExpiredOrders();
		this.UpdateHasItems(false);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x00076714 File Offset: 0x00074914
	private void CheckForExpiredOrders()
	{
		if (this.pendingOrders != null && this.pendingOrders.Count > 0)
		{
			bool flag = false;
			float? num = null;
			for (int i = 0; i < this.pendingOrders.Count; i++)
			{
				ProtoBuf.MarketTerminal.PendingOrder pendingOrder = this.pendingOrders[i];
				if (pendingOrder.timeUntilExpiry <= 0f)
				{
					EntityRef<global::DeliveryDrone> entityRef = new EntityRef<global::DeliveryDrone>(pendingOrder.droneId);
					global::DeliveryDrone deliveryDrone;
					if (entityRef.TryGet(true, out deliveryDrone))
					{
						Debug.LogError("Delivery timed out waiting for drone (too slow speed?)", this);
						deliveryDrone.Kill(global::BaseNetworkable.DestroyMode.None);
					}
					else
					{
						Debug.LogError("Delivery timed out waiting for drone, and the drone seems to have been destroyed?", this);
					}
					this.pendingOrders.RemoveAt(i);
					i--;
					flag = true;
				}
				else if (num == null || pendingOrder.timeUntilExpiry < num.Value)
				{
					num = new float?(pendingOrder.timeUntilExpiry);
				}
			}
			if (flag)
			{
				this.UpdateHasItems(false);
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			if (num != null)
			{
				base.Invoke(this._checkForExpiredOrdersCached, num.Value);
				return;
			}
		}
		else
		{
			base.CancelInvoke(this._checkForExpiredOrdersCached);
		}
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x00076838 File Offset: 0x00074A38
	private void RestrictToPlayer(global::BasePlayer player)
	{
		if (this._customerSteamId == player.userID)
		{
			this._timeUntilCustomerExpiry = this.lockToCustomerDuration;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return;
		}
		if (this._customerSteamId != 0UL)
		{
			Debug.LogError("Overwriting player restriction! It should be cleared first.", this);
		}
		this._customerSteamId = player.userID;
		this._customerName = player.displayName;
		this._timeUntilCustomerExpiry = this.lockToCustomerDuration;
		base.SendNetworkUpdateImmediate(false);
		base.ClientRPC<ulong>(null, "Client_CloseMarketUI", this._customerSteamId);
		this.RemoveAnyLooters();
		if (base.IsOpen())
		{
			Debug.LogError("Market terminal's inventory is still open after removing looters!", this);
		}
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x000768DE File Offset: 0x00074ADE
	private void ClearRestriction()
	{
		if (this._customerSteamId == 0UL)
		{
			return;
		}
		this._customerSteamId = 0UL;
		this._customerName = null;
		this._timeUntilCustomerExpiry = 0f;
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x00076910 File Offset: 0x00074B10
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	public void Server_TryOpenMarket(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerInteract(msg.player))
		{
			return;
		}
		if (!this._marketplace.IsValid(true))
		{
			Debug.LogError("Marketplace is not set", this);
			return;
		}
		using (EntityIdList entityIdList = Facepunch.Pool.Get<EntityIdList>())
		{
			entityIdList.entityIds = Facepunch.Pool.GetList<NetworkableId>();
			this.GetDeliveryEligibleVendingMachines(entityIdList.entityIds);
			base.ClientRPCPlayer<EntityIdList>(null, msg.player, "Client_OpenMarket", entityIdList);
		}
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x00076994 File Offset: 0x00074B94
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(10UL)]
	public void Server_Purchase(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerInteract(msg.player))
		{
			return;
		}
		if (!this._marketplace.IsValid(true))
		{
			Debug.LogError("Marketplace is not set", this);
			return;
		}
		NetworkableId networkableId = msg.read.EntityID();
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		global::VendingMachine vendingMachine = global::BaseNetworkable.serverEntities.Find(networkableId) as global::VendingMachine;
		if (vendingMachine == null || !vendingMachine.IsValid() || num < 0 || num >= vendingMachine.sellOrders.sellOrders.Count || num2 <= 0 || base.inventory.IsFull())
		{
			return;
		}
		this.GetDeliveryEligibleVendingMachines(null);
		if (global::MarketTerminal._deliveryEligible == null || !global::MarketTerminal._deliveryEligible.Contains(networkableId))
		{
			return;
		}
		try
		{
			this._transactionActive = true;
			int num3 = this.deliveryFeeAmount;
			ProtoBuf.VendingMachine.SellOrder sellOrder = vendingMachine.sellOrders.sellOrders[num];
			if (this.CanPlayerAffordOrderAndDeliveryFee(msg.player, sellOrder, num2))
			{
				int num4 = msg.player.inventory.Take(null, this.deliveryFeeCurrency.itemid, num3);
				if (num4 != num3)
				{
					Debug.LogError(string.Format("Took an incorrect number of items for the delivery fee (took {0}, should have taken {1})", num4, num3));
				}
				base.ClientRPCPlayer<int, int, bool>(null, msg.player, "Client_ShowItemNotice", this.deliveryFeeCurrency.itemid, -num3, false);
				if (!vendingMachine.DoTransaction(msg.player, num, num2, base.inventory, this._onCurrencyRemovedCached, this._onItemPurchasedCached, this))
				{
					global::Item item = ItemManager.CreateByItemID(this.deliveryFeeCurrency.itemid, num3, 0UL);
					if (!msg.player.inventory.GiveItem(item, null, false))
					{
						item.Drop(msg.player.inventory.containerMain.dropPosition, msg.player.inventory.containerMain.dropVelocity, default(Quaternion));
					}
				}
				else
				{
					this.RestrictToPlayer(msg.player);
					this.RegisterOrder(msg.player, vendingMachine);
				}
			}
		}
		finally
		{
			this._transactionActive = false;
		}
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x00076BC0 File Offset: 0x00074DC0
	private void UpdateHasItems(bool sendNetworkUpdate = true)
	{
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		bool flag = base.inventory.itemList.Count > 0;
		bool flag2 = this.pendingOrders != null && this.pendingOrders.Count != 0;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, flag && !flag2, false, sendNetworkUpdate);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, base.inventory.IsFull(), false, sendNetworkUpdate);
		if (!flag && !flag2)
		{
			this.ClearRestriction();
		}
	}

	// Token: 0x06000E08 RID: 3592 RVA: 0x00076C3E File Offset: 0x00074E3E
	private void OnCurrencyRemoved(global::BasePlayer player, global::Item currencyItem)
	{
		if (player != null && currencyItem != null)
		{
			base.ClientRPCPlayer<int, int, bool>(null, player, "Client_ShowItemNotice", currencyItem.info.itemid, -currencyItem.amount, false);
		}
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x00076C6C File Offset: 0x00074E6C
	private void OnItemPurchased(global::BasePlayer player, global::Item purchasedItem)
	{
		if (player != null && purchasedItem != null)
		{
			base.ClientRPCPlayer<int, int, bool>(null, player, "Client_ShowItemNotice", purchasedItem.info.itemid, purchasedItem.amount, true);
		}
	}

	// Token: 0x06000E0A RID: 3594 RVA: 0x00076C9C File Offset: 0x00074E9C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.marketTerminal = Facepunch.Pool.Get<ProtoBuf.MarketTerminal>();
		info.msg.marketTerminal.customerSteamId = this._customerSteamId;
		info.msg.marketTerminal.customerName = this._customerName;
		info.msg.marketTerminal.timeUntilExpiry = this._timeUntilCustomerExpiry;
		info.msg.marketTerminal.marketplaceId = this._marketplace.uid;
		info.msg.marketTerminal.orders = Facepunch.Pool.GetList<ProtoBuf.MarketTerminal.PendingOrder>();
		if (this.pendingOrders != null)
		{
			foreach (ProtoBuf.MarketTerminal.PendingOrder pendingOrder in this.pendingOrders)
			{
				ProtoBuf.MarketTerminal.PendingOrder pendingOrder2 = pendingOrder.Copy();
				info.msg.marketTerminal.orders.Add(pendingOrder2);
			}
		}
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x00076D94 File Offset: 0x00074F94
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		return this._transactionActive || item.parent == null || item.parent == base.inventory;
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x00076DBB File Offset: 0x00074FBB
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		this.UpdateHasItems(true);
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x00076DC4 File Offset: 0x00074FC4
	public override bool CanOpenLootPanel(global::BasePlayer player, string panelName)
	{
		return this.CanPlayerInteract(player) && base.HasFlag(global::BaseEntity.Flags.Reserved1) && base.CanOpenLootPanel(player, panelName);
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x00076DE8 File Offset: 0x00074FE8
	private void RemoveAnyLooters()
	{
		global::ItemContainer inventory = base.inventory;
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			if (!(basePlayer == null) && !(basePlayer.inventory == null) && !(basePlayer.inventory.loot == null) && basePlayer.inventory.loot.containers.Contains(inventory))
			{
				basePlayer.inventory.loot.Clear();
			}
		}
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x00076E8C File Offset: 0x0007508C
	public void GetDeliveryEligibleVendingMachines(List<NetworkableId> vendingMachineIds)
	{
		if (global::MarketTerminal._deliveryEligibleLastCalculated < 5f)
		{
			if (vendingMachineIds != null)
			{
				foreach (NetworkableId networkableId in global::MarketTerminal._deliveryEligible)
				{
					vendingMachineIds.Add(networkableId);
				}
			}
			return;
		}
		global::MarketTerminal._deliveryEligibleLastCalculated = 0f;
		global::MarketTerminal._deliveryEligible.Clear();
		using (List<MapMarker>.Enumerator enumerator2 = MapMarker.serverMapMarkers.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				VendingMachineMapMarker vendingMachineMapMarker;
				if ((vendingMachineMapMarker = enumerator2.Current as VendingMachineMapMarker) != null && !(vendingMachineMapMarker.server_vendingMachine == null))
				{
					global::VendingMachine server_vendingMachine = vendingMachineMapMarker.server_vendingMachine;
					if (!(server_vendingMachine == null) && (this.<GetDeliveryEligibleVendingMachines>g__IsEligible|24_0(server_vendingMachine, this.config.vendingMachineOffset, 1) || this.<GetDeliveryEligibleVendingMachines>g__IsEligible|24_0(server_vendingMachine, this.config.vendingMachineOffset + Vector3.forward * this.config.maxDistanceFromVendingMachine, 2)))
					{
						global::MarketTerminal._deliveryEligible.Add(server_vendingMachine.net.ID);
					}
				}
			}
		}
		if (vendingMachineIds != null)
		{
			foreach (NetworkableId networkableId2 in global::MarketTerminal._deliveryEligible)
			{
				vendingMachineIds.Add(networkableId2);
			}
		}
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x00077018 File Offset: 0x00075218
	public bool CanPlayerAffordOrderAndDeliveryFee(global::BasePlayer player, ProtoBuf.VendingMachine.SellOrder sellOrder, int numberOfTransactions)
	{
		int num = player.inventory.FindItemIDs(this.deliveryFeeCurrency.itemid).Sum((global::Item i) => i.amount);
		int num2 = this.deliveryFeeAmount;
		if (num < num2)
		{
			return false;
		}
		if (sellOrder != null)
		{
			int num3 = sellOrder.currencyAmountPerItem * numberOfTransactions;
			if (sellOrder.currencyID == this.deliveryFeeCurrency.itemid && !sellOrder.currencyIsBP && num < num2 + num3)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x0007709D File Offset: 0x0007529D
	public bool HasPendingOrderFor(NetworkableId vendingMachineId)
	{
		List<ProtoBuf.MarketTerminal.PendingOrder> list = this.pendingOrders;
		object obj;
		if (list == null)
		{
			obj = null;
		}
		else
		{
			obj = list.FindWith((ProtoBuf.MarketTerminal.PendingOrder o) => o.vendingMachineId, vendingMachineId, null);
		}
		return obj != null;
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x000770D5 File Offset: 0x000752D5
	public bool CanPlayerInteract(global::BasePlayer player)
	{
		return !(player == null) && (this._customerSteamId == 0UL || this._timeUntilCustomerExpiry <= 0f || player.userID == this._customerSteamId);
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x0007710C File Offset: 0x0007530C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.marketTerminal != null)
		{
			this._customerSteamId = info.msg.marketTerminal.customerSteamId;
			this._customerName = info.msg.marketTerminal.customerName;
			this._timeUntilCustomerExpiry = info.msg.marketTerminal.timeUntilExpiry;
			this._marketplace = new EntityRef<Marketplace>(info.msg.marketTerminal.marketplaceId);
			if (this.pendingOrders == null)
			{
				this.pendingOrders = Facepunch.Pool.GetList<ProtoBuf.MarketTerminal.PendingOrder>();
			}
			if (this.pendingOrders.Count > 0)
			{
				foreach (ProtoBuf.MarketTerminal.PendingOrder pendingOrder in this.pendingOrders)
				{
					Facepunch.Pool.Free<ProtoBuf.MarketTerminal.PendingOrder>(ref pendingOrder);
				}
				this.pendingOrders.Clear();
			}
			foreach (ProtoBuf.MarketTerminal.PendingOrder pendingOrder2 in info.msg.marketTerminal.orders)
			{
				ProtoBuf.MarketTerminal.PendingOrder pendingOrder3 = pendingOrder2.Copy();
				this.pendingOrders.Add(pendingOrder3);
			}
		}
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x0007728C File Offset: 0x0007548C
	[CompilerGenerated]
	private bool <GetDeliveryEligibleVendingMachines>g__IsEligible|24_0(global::VendingMachine vendingMachine, Vector3 offset, int n)
	{
		RaycastHit raycastHit;
		return vendingMachine is NPCVendingMachine || (vendingMachine.IsBroadcasting() && this.config.IsVendingMachineAccessible(vendingMachine, offset, out raycastHit));
	}

	// Token: 0x0400090C RID: 2316
	private Action<global::BasePlayer, global::Item> _onCurrencyRemovedCached;

	// Token: 0x0400090D RID: 2317
	private Action<global::BasePlayer, global::Item> _onItemPurchasedCached;

	// Token: 0x0400090E RID: 2318
	private Action _checkForExpiredOrdersCached;

	// Token: 0x0400090F RID: 2319
	private bool _transactionActive;

	// Token: 0x04000910 RID: 2320
	private static readonly List<NetworkableId> _deliveryEligible = new List<NetworkableId>(128);

	// Token: 0x04000911 RID: 2321
	private static RealTimeSince _deliveryEligibleLastCalculated;

	// Token: 0x04000912 RID: 2322
	public const global::BaseEntity.Flags Flag_HasItems = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000913 RID: 2323
	public const global::BaseEntity.Flags Flag_InventoryFull = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000914 RID: 2324
	[Header("Market Terminal")]
	public GameObjectRef menuPrefab;

	// Token: 0x04000915 RID: 2325
	public ulong lockToCustomerDuration = 300UL;

	// Token: 0x04000916 RID: 2326
	public ulong orderTimeout = 180UL;

	// Token: 0x04000917 RID: 2327
	public ItemDefinition deliveryFeeCurrency;

	// Token: 0x04000918 RID: 2328
	public int deliveryFeeAmount;

	// Token: 0x04000919 RID: 2329
	public DeliveryDroneConfig config;

	// Token: 0x0400091A RID: 2330
	public RustText userLabel;

	// Token: 0x0400091B RID: 2331
	private ulong _customerSteamId;

	// Token: 0x0400091C RID: 2332
	private string _customerName;

	// Token: 0x0400091D RID: 2333
	private TimeUntil _timeUntilCustomerExpiry;

	// Token: 0x0400091E RID: 2334
	private EntityRef<Marketplace> _marketplace;

	// Token: 0x0400091F RID: 2335
	public List<ProtoBuf.MarketTerminal.PendingOrder> pendingOrders;
}
