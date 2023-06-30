using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;

// Token: 0x0200042D RID: 1069
public class VendingMachineMapMarker : MapMarker
{
	// Token: 0x0600243B RID: 9275 RVA: 0x000E7190 File Offset: 0x000E5390
	public void SetVendingMachine(global::VendingMachine vm, string shopName)
	{
		this.server_vendingMachine = vm;
		this.markerShopName = shopName;
		if (!base.IsInvoking(new Action(this.TryUpdatePosition)))
		{
			base.InvokeRandomized(new Action(this.TryUpdatePosition), 30f, 30f, 10f);
		}
	}

	// Token: 0x0600243C RID: 9276 RVA: 0x000E71E0 File Offset: 0x000E53E0
	private void TryUpdatePosition()
	{
		if (this.server_vendingMachine != null && this.server_vendingMachine.GetParentEntity() != null)
		{
			base.transform.position = this.server_vendingMachine.transform.position;
			try
			{
				this.syncPosition = true;
				base.NetworkPositionTick();
			}
			finally
			{
				this.syncPosition = false;
			}
		}
	}

	// Token: 0x0600243D RID: 9277 RVA: 0x000E7250 File Offset: 0x000E5450
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vendingMachine = new ProtoBuf.VendingMachine();
		info.msg.vendingMachine.shopName = this.markerShopName;
		if (this.server_vendingMachine != null)
		{
			info.msg.vendingMachine.networkID = this.server_vendingMachine.net.ID;
			info.msg.vendingMachine.sellOrderContainer = new ProtoBuf.VendingMachine.SellOrderContainer();
			info.msg.vendingMachine.sellOrderContainer.ShouldPool = false;
			info.msg.vendingMachine.sellOrderContainer.sellOrders = new List<ProtoBuf.VendingMachine.SellOrder>();
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.server_vendingMachine.sellOrders.sellOrders)
			{
				ProtoBuf.VendingMachine.SellOrder sellOrder2 = new ProtoBuf.VendingMachine.SellOrder();
				sellOrder2.ShouldPool = false;
				sellOrder.CopyTo(sellOrder2);
				info.msg.vendingMachine.sellOrderContainer.sellOrders.Add(sellOrder2);
			}
		}
	}

	// Token: 0x0600243E RID: 9278 RVA: 0x000E7378 File Offset: 0x000E5578
	public override AppMarker GetAppMarkerData()
	{
		AppMarker appMarkerData = base.GetAppMarkerData();
		appMarkerData.name = this.markerShopName ?? "";
		appMarkerData.outOfStock = !base.HasFlag(global::BaseEntity.Flags.Busy);
		if (this.server_vendingMachine != null)
		{
			appMarkerData.sellOrders = Pool.GetList<AppMarker.SellOrder>();
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.server_vendingMachine.sellOrders.sellOrders)
			{
				AppMarker.SellOrder sellOrder2 = Pool.Get<AppMarker.SellOrder>();
				sellOrder2.itemId = sellOrder.itemToSellID;
				sellOrder2.quantity = sellOrder.itemToSellAmount;
				sellOrder2.currencyId = sellOrder.currencyID;
				sellOrder2.costPerItem = sellOrder.currencyAmountPerItem;
				sellOrder2.amountInStock = sellOrder.inStock;
				sellOrder2.itemIsBlueprint = sellOrder.itemToSellIsBP;
				sellOrder2.currencyIsBlueprint = sellOrder.currencyIsBP;
				sellOrder2.itemCondition = sellOrder.itemCondition;
				sellOrder2.itemConditionMax = sellOrder.itemConditionMax;
				appMarkerData.sellOrders.Add(sellOrder2);
			}
		}
		return appMarkerData;
	}

	// Token: 0x04001C35 RID: 7221
	public string markerShopName;

	// Token: 0x04001C36 RID: 7222
	public global::VendingMachine server_vendingMachine;

	// Token: 0x04001C37 RID: 7223
	public ProtoBuf.VendingMachine client_vendingMachine;

	// Token: 0x04001C38 RID: 7224
	[NonSerialized]
	public NetworkableId client_vendingMachineNetworkID;

	// Token: 0x04001C39 RID: 7225
	public GameObjectRef clusterMarkerObj;
}
