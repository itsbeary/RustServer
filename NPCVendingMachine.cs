using System;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x0200012C RID: 300
public class NPCVendingMachine : VendingMachine
{
	// Token: 0x060016D2 RID: 5842 RVA: 0x000AF89C File Offset: 0x000ADA9C
	public byte GetBPState(bool sellItemAsBP, bool currencyItemAsBP)
	{
		byte b = 0;
		if (sellItemAsBP)
		{
			b = 1;
		}
		if (currencyItemAsBP)
		{
			b = 2;
		}
		if (sellItemAsBP && currencyItemAsBP)
		{
			b = 3;
		}
		return b;
	}

	// Token: 0x060016D3 RID: 5843 RVA: 0x000AF8BD File Offset: 0x000ADABD
	public override void TakeCurrencyItem(Item takenCurrencyItem)
	{
		takenCurrencyItem.MoveToContainer(base.inventory, -1, true, false, null, true);
		takenCurrencyItem.RemoveFromContainer();
		takenCurrencyItem.Remove(0f);
	}

	// Token: 0x060016D4 RID: 5844 RVA: 0x000AF8E2 File Offset: 0x000ADAE2
	public override void GiveSoldItem(Item soldItem, BasePlayer buyer)
	{
		base.GiveSoldItem(soldItem, buyer);
	}

	// Token: 0x060016D5 RID: 5845 RVA: 0x000AF8EC File Offset: 0x000ADAEC
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.Invoke(new Action(this.InstallFromVendingOrders), 1f);
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x000AF90C File Offset: 0x000ADB0C
	public override void ServerInit()
	{
		base.ServerInit();
		this.skinID = 861142659UL;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		base.Invoke(new Action(this.InstallFromVendingOrders), 1f);
		base.InvokeRandomized(new Action(this.Refill), 1f, 1f, 0.1f);
	}

	// Token: 0x060016D7 RID: 5847 RVA: 0x000AF96C File Offset: 0x000ADB6C
	public virtual void InstallFromVendingOrders()
	{
		if (this.vendingOrders == null)
		{
			Debug.LogError("No vending orders!");
			return;
		}
		this.ClearSellOrders();
		base.inventory.Clear();
		ItemManager.DoRemoves();
		foreach (NPCVendingOrder.Entry entry in this.vendingOrders.orders)
		{
			this.AddItemForSale(entry.sellItem.itemid, entry.sellItemAmount, entry.currencyItem.itemid, entry.currencyAmount, this.GetBPState(entry.sellItemAsBP, entry.currencyAsBP));
		}
	}

	// Token: 0x060016D8 RID: 5848 RVA: 0x000AFA00 File Offset: 0x000ADC00
	public override void InstallDefaultSellOrders()
	{
		base.InstallDefaultSellOrders();
	}

	// Token: 0x060016D9 RID: 5849 RVA: 0x000AFA08 File Offset: 0x000ADC08
	public void Refill()
	{
		if (this.vendingOrders == null || this.vendingOrders.orders == null)
		{
			return;
		}
		if (base.inventory == null)
		{
			return;
		}
		if (this.refillTimes == null)
		{
			this.refillTimes = new float[this.vendingOrders.orders.Length];
		}
		for (int i = 0; i < this.vendingOrders.orders.Length; i++)
		{
			NPCVendingOrder.Entry entry = this.vendingOrders.orders[i];
			if (Time.realtimeSinceStartup > this.refillTimes[i])
			{
				int num = Mathf.FloorToInt((float)(base.inventory.GetAmount(entry.sellItem.itemid, false) / entry.sellItemAmount));
				int num2 = Mathf.Min(10 - num, entry.refillAmount) * entry.sellItemAmount;
				if (num2 > 0)
				{
					this.transactionActive = true;
					Item item;
					if (entry.sellItemAsBP)
					{
						item = ItemManager.Create(this.blueprintBaseDef, num2, 0UL);
						item.blueprintTarget = entry.sellItem.itemid;
					}
					else
					{
						item = ItemManager.Create(entry.sellItem, num2, 0UL);
					}
					item.MoveToContainer(base.inventory, -1, true, false, null, true);
					this.transactionActive = false;
				}
				this.refillTimes[i] = Time.realtimeSinceStartup + entry.refillDelay;
			}
		}
	}

	// Token: 0x060016DA RID: 5850 RVA: 0x000AFB4E File Offset: 0x000ADD4E
	public void ClearSellOrders()
	{
		this.sellOrders.sellOrders.Clear();
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x000AFB60 File Offset: 0x000ADD60
	public void AddItemForSale(int itemID, int amountToSell, int currencyID, int currencyPerTransaction, byte bpState)
	{
		base.AddSellOrder(itemID, amountToSell, currencyID, currencyPerTransaction, bpState);
		this.transactionActive = true;
		int num = 10;
		if (bpState == 1 || bpState == 3)
		{
			for (int i = 0; i < num; i++)
			{
				Item item = ItemManager.CreateByItemID(this.blueprintBaseDef.itemid, 1, 0UL);
				item.blueprintTarget = itemID;
				base.inventory.Insert(item);
			}
		}
		else
		{
			base.inventory.AddItem(ItemManager.FindItemDefinition(itemID), amountToSell * num, 0UL, ItemContainer.LimitStack.Existing);
		}
		this.transactionActive = false;
		base.RefreshSellOrderStockLevel(null);
	}

	// Token: 0x060016DC RID: 5852 RVA: 0x000063A5 File Offset: 0x000045A5
	public void RefreshStock()
	{
	}

	// Token: 0x060016DD RID: 5853 RVA: 0x000AFBE9 File Offset: 0x000ADDE9
	protected override void RecordSaleAnalytics(Item itemSold)
	{
		Analytics.Server.VendingMachineTransaction(this.vendingOrders, itemSold.info, itemSold.amount);
	}

	// Token: 0x060016DE RID: 5854 RVA: 0x00007A44 File Offset: 0x00005C44
	protected override bool CanRotate()
	{
		return false;
	}

	// Token: 0x060016DF RID: 5855 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool CanPlayerAdmin(BasePlayer player)
	{
		return false;
	}

	// Token: 0x04000EE2 RID: 3810
	public NPCVendingOrder vendingOrders;

	// Token: 0x04000EE3 RID: 3811
	private float[] refillTimes;
}
