using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000EE RID: 238
public class VendingMachine : StorageContainer
{
	// Token: 0x060014D4 RID: 5332 RVA: 0x000A3D38 File Offset: 0x000A1F38
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VendingMachine.OnRpcMessage", 0))
		{
			if (rpc == 3011053703U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BuyItem ");
				}
				using (TimeWarning.New("BuyItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3011053703U, "BuyItem", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3011053703U, "BuyItem", this, player, 3f))
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
							this.BuyItem(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in BuyItem");
					}
				}
				return true;
			}
			if (rpc == 1626480840U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_AddSellOrder ");
				}
				using (TimeWarning.New("RPC_AddSellOrder", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1626480840U, "RPC_AddSellOrder", this, player, 3f))
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
							this.RPC_AddSellOrder(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_AddSellOrder");
					}
				}
				return true;
			}
			if (rpc == 169239598U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Broadcast ");
				}
				using (TimeWarning.New("RPC_Broadcast", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(169239598U, "RPC_Broadcast", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Broadcast(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_Broadcast");
					}
				}
				return true;
			}
			if (rpc == 3680901137U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_DeleteSellOrder ");
				}
				using (TimeWarning.New("RPC_DeleteSellOrder", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3680901137U, "RPC_DeleteSellOrder", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_DeleteSellOrder(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_DeleteSellOrder");
					}
				}
				return true;
			}
			if (rpc == 2555993359U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenAdmin ");
				}
				using (TimeWarning.New("RPC_OpenAdmin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2555993359U, "RPC_OpenAdmin", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenAdmin(rpcmessage5);
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in RPC_OpenAdmin");
					}
				}
				return true;
			}
			if (rpc == 36164441U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenShop ");
				}
				using (TimeWarning.New("RPC_OpenShop", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(36164441U, "RPC_OpenShop", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenShop(rpcmessage6);
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in RPC_OpenShop");
					}
				}
				return true;
			}
			if (rpc == 3346513099U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RotateVM ");
				}
				using (TimeWarning.New("RPC_RotateVM", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3346513099U, "RPC_RotateVM", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RotateVM(rpcmessage7);
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in RPC_RotateVM");
					}
				}
				return true;
			}
			if (rpc == 1012779214U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_UpdateShopName ");
				}
				using (TimeWarning.New("RPC_UpdateShopName", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1012779214U, "RPC_UpdateShopName", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage8 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_UpdateShopName(rpcmessage8);
						}
					}
					catch (Exception ex8)
					{
						Debug.LogException(ex8);
						player.Kick("RPC Error in RPC_UpdateShopName");
					}
				}
				return true;
			}
			if (rpc == 3559014831U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - TransactionStart ");
				}
				using (TimeWarning.New("TransactionStart", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3559014831U, "TransactionStart", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage9 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.TransactionStart(rpcmessage9);
						}
					}
					catch (Exception ex9)
					{
						Debug.LogException(ex9);
						player.Kick("RPC Error in TransactionStart");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060014D5 RID: 5333 RVA: 0x000A49D0 File Offset: 0x000A2BD0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.vendingMachine != null)
		{
			this.shopName = info.msg.vendingMachine.shopName;
			if (info.msg.vendingMachine.sellOrderContainer != null)
			{
				this.sellOrders = info.msg.vendingMachine.sellOrderContainer;
				this.sellOrders.ShouldPool = false;
			}
			if (info.fromDisk && base.isServer)
			{
				this.RefreshSellOrderStockLevel(null);
			}
		}
	}

	// Token: 0x060014D6 RID: 5334 RVA: 0x000A4A54 File Offset: 0x000A2C54
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vendingMachine = new ProtoBuf.VendingMachine();
		info.msg.vendingMachine.ShouldPool = false;
		info.msg.vendingMachine.shopName = this.shopName;
		if (this.sellOrders != null)
		{
			info.msg.vendingMachine.sellOrderContainer = new ProtoBuf.VendingMachine.SellOrderContainer();
			info.msg.vendingMachine.sellOrderContainer.ShouldPool = false;
			info.msg.vendingMachine.sellOrderContainer.sellOrders = new List<ProtoBuf.VendingMachine.SellOrder>();
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.sellOrders.sellOrders)
			{
				ProtoBuf.VendingMachine.SellOrder sellOrder2 = new ProtoBuf.VendingMachine.SellOrder();
				sellOrder2.ShouldPool = false;
				sellOrder.CopyTo(sellOrder2);
				info.msg.vendingMachine.sellOrderContainer.sellOrders.Add(sellOrder2);
			}
		}
	}

	// Token: 0x060014D7 RID: 5335 RVA: 0x000A4B60 File Offset: 0x000A2D60
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isServer)
		{
			this.InstallDefaultSellOrders();
			base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
			base.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
			this.RefreshSellOrderStockLevel(null);
			global::ItemContainer inventory = base.inventory;
			inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
			this.UpdateMapMarker();
			this.fullUpdateCached = new Action(this.FullUpdate);
		}
	}

	// Token: 0x060014D8 RID: 5336 RVA: 0x000A4BED File Offset: 0x000A2DED
	public override void DestroyShared()
	{
		if (this.myMarker)
		{
			this.myMarker.Kill(global::BaseNetworkable.DestroyMode.None);
			this.myMarker = null;
		}
		base.DestroyShared();
	}

	// Token: 0x060014D9 RID: 5337 RVA: 0x000A4C15 File Offset: 0x000A2E15
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
	}

	// Token: 0x060014DA RID: 5338 RVA: 0x000A4C1F File Offset: 0x000A2E1F
	public void FullUpdate()
	{
		this.RefreshSellOrderStockLevel(null);
		this.UpdateMapMarker();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060014DB RID: 5339 RVA: 0x000A4C35 File Offset: 0x000A2E35
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		base.CancelInvoke(this.fullUpdateCached);
		base.Invoke(this.fullUpdateCached, 0.2f);
	}

	// Token: 0x060014DC RID: 5340 RVA: 0x000A4C5C File Offset: 0x000A2E5C
	public void RefreshSellOrderStockLevel(ItemDefinition itemDef = null)
	{
		foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.sellOrders.sellOrders)
		{
			if (itemDef == null || itemDef.itemid == sellOrder.itemToSellID)
			{
				List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
				this.GetItemsToSell(sellOrder, list);
				ProtoBuf.VendingMachine.SellOrder sellOrder2 = sellOrder;
				int num;
				if (list.Count < 0)
				{
					num = 0;
				}
				else
				{
					num = list.Sum((global::Item x) => x.amount) / sellOrder.itemToSellAmount;
				}
				sellOrder2.inStock = num;
				float num2 = 0f;
				float num3 = 0f;
				int num4 = 0;
				if (list.Count > 0)
				{
					if (list[0].hasCondition)
					{
						num2 = list[0].condition;
						num3 = list[0].maxCondition;
					}
					if (list[0].info != null && list[0].info.amountType == ItemDefinition.AmountType.Genetics && list[0].instanceData != null)
					{
						num4 = list[0].instanceData.dataInt;
						sellOrder.inStock = list[0].amount;
					}
				}
				sellOrder.itemCondition = num2;
				sellOrder.itemConditionMax = num3;
				sellOrder.instanceData = num4;
				Facepunch.Pool.FreeList<global::Item>(ref list);
			}
		}
	}

	// Token: 0x060014DD RID: 5341 RVA: 0x000A4DE4 File Offset: 0x000A2FE4
	public bool OutOfStock()
	{
		using (List<ProtoBuf.VendingMachine.SellOrder>.Enumerator enumerator = this.sellOrders.sellOrders.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.inStock > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060014DE RID: 5342 RVA: 0x000A4E44 File Offset: 0x000A3044
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		this.RefreshSellOrderStockLevel(null);
		this.UpdateMapMarker();
	}

	// Token: 0x060014DF RID: 5343 RVA: 0x000A4E67 File Offset: 0x000A3067
	public void UpdateEmptyFlag()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved1, base.inventory.itemList.Count == 0, false, true);
	}

	// Token: 0x060014E0 RID: 5344 RVA: 0x000A4E89 File Offset: 0x000A3089
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		this.UpdateEmptyFlag();
		if (this.vend_Player != null && this.vend_Player == player)
		{
			this.ClearPendingOrder();
		}
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x000A4EBA File Offset: 0x000A30BA
	public virtual void InstallDefaultSellOrders()
	{
		this.sellOrders = new ProtoBuf.VendingMachine.SellOrderContainer();
		this.sellOrders.ShouldPool = false;
		this.sellOrders.sellOrders = new List<ProtoBuf.VendingMachine.SellOrder>();
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool HasVendingSounds()
	{
		return true;
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x000A4EE3 File Offset: 0x000A30E3
	public virtual float GetBuyDuration()
	{
		return 2.5f;
	}

	// Token: 0x060014E4 RID: 5348 RVA: 0x000A4EEA File Offset: 0x000A30EA
	public void SetPendingOrder(global::BasePlayer buyer, int sellOrderId, int numberOfTransactions)
	{
		this.ClearPendingOrder();
		this.vend_Player = buyer;
		this.vend_sellOrderID = sellOrderId;
		this.vend_numberOfTransactions = numberOfTransactions;
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		if (this.HasVendingSounds())
		{
			base.ClientRPC<int>(null, "CLIENT_StartVendingSounds", sellOrderId);
		}
	}

	// Token: 0x060014E5 RID: 5349 RVA: 0x000A4F2C File Offset: 0x000A312C
	public void ClearPendingOrder()
	{
		base.CancelInvoke(new Action(this.CompletePendingOrder));
		this.vend_Player = null;
		this.vend_sellOrderID = -1;
		this.vend_numberOfTransactions = -1;
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.ClientRPC(null, "CLIENT_CancelVendingSounds");
	}

	// Token: 0x060014E6 RID: 5350 RVA: 0x000A4F7C File Offset: 0x000A317C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void BuyItem(global::BaseEntity.RPCMessage rpc)
	{
		if (!base.OccupiedCheck(rpc.player))
		{
			return;
		}
		int num = rpc.read.Int32();
		int num2 = rpc.read.Int32();
		if (this.IsVending())
		{
			rpc.player.ShowToast(GameTip.Styles.Red_Normal, global::VendingMachine.WaitForVendingMessage, Array.Empty<string>());
			return;
		}
		this.SetPendingOrder(rpc.player, num, num2);
		base.Invoke(new Action(this.CompletePendingOrder), this.GetBuyDuration());
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x000A4FF6 File Offset: 0x000A31F6
	public virtual void CompletePendingOrder()
	{
		this.DoTransaction(this.vend_Player, this.vend_sellOrderID, this.vend_numberOfTransactions, null, null, null, null);
		this.ClearPendingOrder();
		global::Decay.RadialDecayTouch(base.transform.position, 40f, 2097408);
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x000063A5 File Offset: 0x000045A5
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void TransactionStart(global::BaseEntity.RPCMessage rpc)
	{
	}

	// Token: 0x060014E9 RID: 5353 RVA: 0x000A5038 File Offset: 0x000A3238
	private void GetItemsToSell(ProtoBuf.VendingMachine.SellOrder sellOrder, List<global::Item> items)
	{
		if (sellOrder.itemToSellIsBP)
		{
			using (List<global::Item>.Enumerator enumerator = base.inventory.itemList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::Item item = enumerator.Current;
					if (item.info.itemid == this.blueprintBaseDef.itemid && item.blueprintTarget == sellOrder.itemToSellID)
					{
						items.Add(item);
					}
				}
				return;
			}
		}
		foreach (global::Item item2 in base.inventory.itemList)
		{
			if (item2.info.itemid == sellOrder.itemToSellID)
			{
				items.Add(item2);
			}
		}
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x000A5118 File Offset: 0x000A3318
	public bool DoTransaction(global::BasePlayer buyer, int sellOrderId, int numberOfTransactions = 1, global::ItemContainer targetContainer = null, Action<global::BasePlayer, global::Item> onCurrencyRemoved = null, Action<global::BasePlayer, global::Item> onItemPurchased = null, global::MarketTerminal droneMarketTerminal = null)
	{
		if (sellOrderId < 0 || sellOrderId >= this.sellOrders.sellOrders.Count)
		{
			return false;
		}
		if (targetContainer == null && Vector3.Distance(buyer.transform.position, base.transform.position) > 4f)
		{
			return false;
		}
		ProtoBuf.VendingMachine.SellOrder sellOrder = this.sellOrders.sellOrders[sellOrderId];
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		this.GetItemsToSell(sellOrder, list);
		if (list == null || list.Count == 0)
		{
			Facepunch.Pool.FreeList<global::Item>(ref list);
			return false;
		}
		numberOfTransactions = Mathf.Clamp(numberOfTransactions, 1, list[0].hasCondition ? 1 : 1000000);
		int num = sellOrder.itemToSellAmount * numberOfTransactions;
		int num2 = list.Sum((global::Item x) => x.amount);
		if (num > num2)
		{
			Facepunch.Pool.FreeList<global::Item>(ref list);
			return false;
		}
		List<global::Item> list2 = buyer.inventory.FindItemIDs(sellOrder.currencyID);
		if (sellOrder.currencyIsBP)
		{
			list2 = (from x in buyer.inventory.FindItemIDs(this.blueprintBaseDef.itemid)
				where x.blueprintTarget == sellOrder.currencyID
				select x).ToList<global::Item>();
		}
		list2 = list2.Where((global::Item x) => !x.hasCondition || (x.conditionNormalized >= 0.5f && x.maxConditionNormalized > 0.5f)).ToList<global::Item>();
		if (list2.Count == 0)
		{
			Facepunch.Pool.FreeList<global::Item>(ref list);
			return false;
		}
		int num3 = list2.Sum((global::Item x) => x.amount);
		int num4 = sellOrder.currencyAmountPerItem * numberOfTransactions;
		if (num3 < num4)
		{
			Facepunch.Pool.FreeList<global::Item>(ref list);
			return false;
		}
		this.transactionActive = true;
		int num5 = 0;
		foreach (global::Item item in list2)
		{
			int num6 = Mathf.Min(num4 - num5, item.amount);
			global::Item item2;
			if (item.amount <= num6)
			{
				item2 = item;
			}
			else
			{
				item2 = item.SplitItem(num6);
			}
			this.TakeCurrencyItem(item2);
			if (onCurrencyRemoved != null)
			{
				onCurrencyRemoved(buyer, item2);
			}
			num5 += num6;
			if (num5 >= num4)
			{
				break;
			}
		}
		Analytics.Azure.OnBuyFromVendingMachine(buyer, this, sellOrder.itemToSellID, sellOrder.itemToSellAmount * numberOfTransactions, sellOrder.itemToSellIsBP, sellOrder.currencyID, sellOrder.currencyAmountPerItem * numberOfTransactions, sellOrder.currencyIsBP, numberOfTransactions, droneMarketTerminal);
		int num7 = 0;
		foreach (global::Item item3 in list)
		{
			int num8 = num - num7;
			global::Item item4;
			if (item3.amount <= num8)
			{
				item4 = item3;
			}
			else
			{
				item4 = item3.SplitItem(num8);
			}
			if (item4 == null)
			{
				Debug.LogError("Vending machine error, contact developers!");
			}
			else
			{
				num7 += item4.amount;
				this.RecordSaleAnalytics(item4);
				if (targetContainer == null)
				{
					this.GiveSoldItem(item4, buyer);
				}
				else if (!item4.MoveToContainer(targetContainer, -1, true, false, null, true))
				{
					item4.Drop(targetContainer.dropPosition, targetContainer.dropVelocity, default(Quaternion));
				}
				if (onItemPurchased != null)
				{
					onItemPurchased(buyer, item4);
				}
			}
			if (num7 >= num)
			{
				break;
			}
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
		this.UpdateEmptyFlag();
		this.transactionActive = false;
		return true;
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x000A54BC File Offset: 0x000A36BC
	protected virtual void RecordSaleAnalytics(global::Item itemSold)
	{
		Analytics.Server.VendingMachineTransaction(null, itemSold.info, itemSold.amount);
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x000A54D0 File Offset: 0x000A36D0
	public virtual void TakeCurrencyItem(global::Item takenCurrencyItem)
	{
		if (!takenCurrencyItem.MoveToContainer(base.inventory, -1, true, false, null, true))
		{
			takenCurrencyItem.Drop(base.inventory.dropPosition, Vector3.zero, default(Quaternion));
		}
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x000A5510 File Offset: 0x000A3710
	public virtual void GiveSoldItem(global::Item soldItem, global::BasePlayer buyer)
	{
		while (soldItem.amount > soldItem.MaxStackable())
		{
			global::Item item = soldItem.SplitItem(soldItem.MaxStackable());
			buyer.GiveItem(item, global::BaseEntity.GiveItemReason.PickedUp);
		}
		buyer.GiveItem(soldItem, global::BaseEntity.GiveItemReason.PickedUp);
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x000A554A File Offset: 0x000A374A
	public void SendSellOrders(global::BasePlayer player = null)
	{
		if (player)
		{
			base.ClientRPCPlayer<ProtoBuf.VendingMachine.SellOrderContainer>(null, player, "CLIENT_ReceiveSellOrders", this.sellOrders);
			return;
		}
		base.ClientRPC<ProtoBuf.VendingMachine.SellOrderContainer>(null, "CLIENT_ReceiveSellOrders", this.sellOrders);
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x000A557C File Offset: 0x000A377C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Broadcast(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		bool flag = msg.read.Bit();
		if (this.CanPlayerAdmin(player))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved4, flag, false, true);
			this.UpdateMapMarker();
		}
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x000A55BC File Offset: 0x000A37BC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_UpdateShopName(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		string text = msg.read.String(32);
		if (this.CanPlayerAdmin(player))
		{
			this.shopName = text;
			this.UpdateMapMarker();
		}
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x000A55F4 File Offset: 0x000A37F4
	public void UpdateMapMarker()
	{
		if (!this.IsBroadcasting())
		{
			if (this.myMarker)
			{
				this.myMarker.Kill(global::BaseNetworkable.DestroyMode.None);
				this.myMarker = null;
			}
			return;
		}
		bool flag = false;
		if (this.myMarker == null)
		{
			this.myMarker = GameManager.server.CreateEntity(this.mapMarkerPrefab.resourcePath, base.transform.position, Quaternion.identity, true) as VendingMachineMapMarker;
			flag = true;
		}
		this.myMarker.SetFlag(global::BaseEntity.Flags.Busy, this.OutOfStock(), false, true);
		this.myMarker.SetVendingMachine(this, this.shopName);
		if (flag)
		{
			this.myMarker.Spawn();
			return;
		}
		this.myMarker.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x000A56B5 File Offset: 0x000A38B5
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_OpenShop(global::BaseEntity.RPCMessage msg)
	{
		if (!base.OccupiedCheck(msg.player))
		{
			return;
		}
		this.SendSellOrders(msg.player);
		this.PlayerOpenLoot(msg.player, this.customerPanel, true);
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x000A56E8 File Offset: 0x000A38E8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_OpenAdmin(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanPlayerAdmin(player))
		{
			return;
		}
		this.SendSellOrders(player);
		this.PlayerOpenLoot(player, "", true);
		base.ClientRPCPlayer(null, player, "CLIENT_OpenAdminMenu");
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x000A5728 File Offset: 0x000A3928
	public void OnIndustrialItemTransferBegins()
	{
		this.industrialItemIncoming = true;
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x000A5731 File Offset: 0x000A3931
	public void OnIndustrialItemTransferEnds()
	{
		this.industrialItemIncoming = false;
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x000A573C File Offset: 0x000A393C
	public bool CanAcceptItem(global::Item item, int targetSlot)
	{
		global::BasePlayer ownerPlayer = item.GetOwnerPlayer();
		return this.transactionActive || this.industrialItemIncoming || item.parent == null || base.inventory.itemList.Contains(item) || (!(ownerPlayer == null) && this.CanPlayerAdmin(ownerPlayer));
	}

	// Token: 0x060014F7 RID: 5367 RVA: 0x000A5793 File Offset: 0x000A3993
	public override bool CanMoveFrom(global::BasePlayer player, global::Item item)
	{
		return this.CanPlayerAdmin(player);
	}

	// Token: 0x060014F8 RID: 5368 RVA: 0x000A579C File Offset: 0x000A399C
	public override bool CanOpenLootPanel(global::BasePlayer player, string panelName)
	{
		return panelName == this.customerPanel || (base.CanOpenLootPanel(player, panelName) && this.CanPlayerAdmin(player));
	}

	// Token: 0x060014F9 RID: 5369 RVA: 0x000A57C4 File Offset: 0x000A39C4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_DeleteSellOrder(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanPlayerAdmin(player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num >= 0 && num < this.sellOrders.sellOrders.Count)
		{
			ProtoBuf.VendingMachine.SellOrder sellOrder = this.sellOrders.sellOrders[num];
			Analytics.Azure.OnVendingMachineOrderChanged(msg.player, this, sellOrder.itemToSellID, sellOrder.itemToSellAmount, sellOrder.itemToSellIsBP, sellOrder.currencyID, sellOrder.currencyAmountPerItem, sellOrder.currencyIsBP, false);
			this.sellOrders.sellOrders.RemoveAt(num);
		}
		this.RefreshSellOrderStockLevel(null);
		this.UpdateMapMarker();
		this.SendSellOrders(player);
	}

	// Token: 0x060014FA RID: 5370 RVA: 0x000A5870 File Offset: 0x000A3A70
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RotateVM(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanRotate())
		{
			return;
		}
		this.UpdateEmptyFlag();
		if (msg.player.CanBuild() && this.IsInventoryEmpty())
		{
			base.transform.rotation = Quaternion.LookRotation(-base.transform.forward, base.transform.up);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060014FB RID: 5371 RVA: 0x000A58D4 File Offset: 0x000A3AD4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_AddSellOrder(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanPlayerAdmin(player))
		{
			return;
		}
		if (this.sellOrders.sellOrders.Count >= 7)
		{
			player.ChatMessage("Too many sell orders - remove some");
			return;
		}
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		int num3 = msg.read.Int32();
		int num4 = msg.read.Int32();
		byte b = msg.read.UInt8();
		this.AddSellOrder(num, num2, num3, num4, b);
		Analytics.Azure.OnVendingMachineOrderChanged(msg.player, this, num, num2, b == 2 || b == 3, num3, num4, b == 1 || b == 3, true);
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x000A5988 File Offset: 0x000A3B88
	public void AddSellOrder(int itemToSellID, int itemToSellAmount, int currencyToUseID, int currencyAmount, byte bpState)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemToSellID);
		ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition(currencyToUseID);
		if (itemDefinition == null || itemDefinition2 == null)
		{
			return;
		}
		currencyAmount = Mathf.Clamp(currencyAmount, 1, 10000);
		itemToSellAmount = Mathf.Clamp(itemToSellAmount, 1, itemDefinition.stackable);
		ProtoBuf.VendingMachine.SellOrder sellOrder = new ProtoBuf.VendingMachine.SellOrder();
		sellOrder.ShouldPool = false;
		sellOrder.itemToSellID = itemToSellID;
		sellOrder.itemToSellAmount = itemToSellAmount;
		sellOrder.currencyID = currencyToUseID;
		sellOrder.currencyAmountPerItem = currencyAmount;
		sellOrder.currencyIsBP = bpState == 3 || bpState == 2;
		sellOrder.itemToSellIsBP = bpState == 3 || bpState == 1;
		this.sellOrders.sellOrders.Add(sellOrder);
		this.RefreshSellOrderStockLevel(itemDefinition);
		this.UpdateMapMarker();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x000A5A49 File Offset: 0x000A3C49
	public void RefreshAndSendNetworkUpdate()
	{
		this.RefreshSellOrderStockLevel(null);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x000A5A5C File Offset: 0x000A3C5C
	public void UpdateOrCreateSalesSheet()
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("note");
		List<global::Item> list = base.inventory.FindItemsByItemID(itemDefinition.itemid);
		global::Item item = null;
		foreach (global::Item item2 in list)
		{
			if (item2.text.Length == 0)
			{
				item = item2;
				break;
			}
		}
		if (item == null)
		{
			ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition("paper");
			global::Item item3 = base.inventory.FindItemByItemID(itemDefinition2.itemid);
			if (item3 != null)
			{
				item = ItemManager.CreateByItemID(itemDefinition.itemid, 1, 0UL);
				if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
				{
					item.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
				item3.UseItem(1);
			}
		}
		if (item != null)
		{
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.sellOrders.sellOrders)
			{
				ItemDefinition itemDefinition3 = ItemManager.FindItemDefinition(sellOrder.itemToSellID);
				global::Item item4 = item;
				item4.text = item4.text + itemDefinition3.displayName.translated + "\n";
			}
			item.MarkDirty();
		}
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x000A5BB8 File Offset: 0x000A3DB8
	protected virtual bool CanRotate()
	{
		return !base.HasAttachedStorageAdaptor();
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x00023AF4 File Offset: 0x00021CF4
	public bool IsBroadcasting()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved4);
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x000233C8 File Offset: 0x000215C8
	public bool IsInventoryEmpty()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x0000564C File Offset: 0x0000384C
	public bool IsVending()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x06001503 RID: 5379 RVA: 0x000A5BC4 File Offset: 0x000A3DC4
	public bool PlayerBehind(global::BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) <= -0.7f;
	}

	// Token: 0x06001504 RID: 5380 RVA: 0x000A5C10 File Offset: 0x000A3E10
	public bool PlayerInfront(global::BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) >= 0.7f;
	}

	// Token: 0x06001505 RID: 5381 RVA: 0x000A5C5A File Offset: 0x000A3E5A
	public virtual bool CanPlayerAdmin(global::BasePlayer player)
	{
		return this.PlayerBehind(player) && base.OccupiedCheck(player);
	}

	// Token: 0x06001506 RID: 5382 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x04000D14 RID: 3348
	[Header("VendingMachine")]
	public static readonly Translate.Phrase WaitForVendingMessage = new Translate.Phrase("vendingmachine.wait", "Please wait...");

	// Token: 0x04000D15 RID: 3349
	public GameObjectRef adminMenuPrefab;

	// Token: 0x04000D16 RID: 3350
	public string customerPanel = "";

	// Token: 0x04000D17 RID: 3351
	public ProtoBuf.VendingMachine.SellOrderContainer sellOrders;

	// Token: 0x04000D18 RID: 3352
	public SoundPlayer buySound;

	// Token: 0x04000D19 RID: 3353
	public string shopName = "A Shop";

	// Token: 0x04000D1A RID: 3354
	public GameObjectRef mapMarkerPrefab;

	// Token: 0x04000D1B RID: 3355
	public ItemDefinition blueprintBaseDef;

	// Token: 0x04000D1C RID: 3356
	private Action fullUpdateCached;

	// Token: 0x04000D1D RID: 3357
	protected global::BasePlayer vend_Player;

	// Token: 0x04000D1E RID: 3358
	private int vend_sellOrderID;

	// Token: 0x04000D1F RID: 3359
	private int vend_numberOfTransactions;

	// Token: 0x04000D20 RID: 3360
	protected bool transactionActive;

	// Token: 0x04000D21 RID: 3361
	private VendingMachineMapMarker myMarker;

	// Token: 0x04000D22 RID: 3362
	private bool industrialItemIncoming;

	// Token: 0x02000C28 RID: 3112
	public static class VendingMachineFlags
	{
		// Token: 0x040042AD RID: 17069
		public const global::BaseEntity.Flags EmptyInv = global::BaseEntity.Flags.Reserved1;

		// Token: 0x040042AE RID: 17070
		public const global::BaseEntity.Flags IsVending = global::BaseEntity.Flags.Reserved2;

		// Token: 0x040042AF RID: 17071
		public const global::BaseEntity.Flags Broadcasting = global::BaseEntity.Flags.Reserved4;

		// Token: 0x040042B0 RID: 17072
		public const global::BaseEntity.Flags OutOfStock = global::BaseEntity.Flags.Reserved5;

		// Token: 0x040042B1 RID: 17073
		public const global::BaseEntity.Flags NoDirectAccess = global::BaseEntity.Flags.Reserved6;
	}
}
