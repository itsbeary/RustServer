using System;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000107 RID: 263
public class Composter : StorageContainer
{
	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x060015D1 RID: 5585 RVA: 0x000ABB63 File Offset: 0x000A9D63
	protected float UpdateInterval
	{
		get
		{
			return Server.composterUpdateInterval;
		}
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x000ABB6C File Offset: 0x000A9D6C
	public override void ServerInit()
	{
		base.ServerInit();
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.InventoryItemFilter));
		base.InvokeRandomized(new Action(this.UpdateComposting), this.UpdateInterval, this.UpdateInterval, this.UpdateInterval * 0.1f);
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x000ABBD0 File Offset: 0x000A9DD0
	public bool InventoryItemFilter(global::Item item, int targetSlot)
	{
		return item != null && (item.info.GetComponent<ItemModCompostable>() != null || this.ItemIsFertilizer(item));
	}

	// Token: 0x060015D4 RID: 5588 RVA: 0x000ABBF6 File Offset: 0x000A9DF6
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.composter = Facepunch.Pool.Get<ProtoBuf.Composter>();
		info.msg.composter.fertilizerProductionProgress = this.fertilizerProductionProgress;
	}

	// Token: 0x060015D5 RID: 5589 RVA: 0x000ABC25 File Offset: 0x000A9E25
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.composter != null)
		{
			this.fertilizerProductionProgress = info.msg.composter.fertilizerProductionProgress;
		}
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x0008429B File Offset: 0x0008249B
	private bool ItemIsFertilizer(global::Item item)
	{
		return item.info.shortname == "fertilizer";
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x000ABC54 File Offset: 0x000A9E54
	public void UpdateComposting()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				this.CompostItem(slot);
			}
		}
	}

	// Token: 0x060015D8 RID: 5592 RVA: 0x000ABC90 File Offset: 0x000A9E90
	private void CompostItem(global::Item item)
	{
		if (this.ItemIsFertilizer(item))
		{
			return;
		}
		ItemModCompostable component = item.info.GetComponent<ItemModCompostable>();
		if (component == null)
		{
			return;
		}
		int num = (this.CompostEntireStack ? item.amount : 1);
		item.UseItem(num);
		this.fertilizerProductionProgress += (float)num * component.TotalFertilizerProduced;
		this.ProduceFertilizer(Mathf.FloorToInt(this.fertilizerProductionProgress));
	}

	// Token: 0x060015D9 RID: 5593 RVA: 0x000ABD00 File Offset: 0x000A9F00
	private void ProduceFertilizer(int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		global::Item item = ItemManager.Create(this.FertilizerDef, amount, 0UL);
		if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
		{
			item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity, default(Quaternion));
		}
		this.fertilizerProductionProgress -= (float)amount;
	}

	// Token: 0x04000DFC RID: 3580
	[Header("Composter")]
	public ItemDefinition FertilizerDef;

	// Token: 0x04000DFD RID: 3581
	[Tooltip("If enabled, entire item stacks will be composted each tick, instead of a single item of a stack.")]
	public bool CompostEntireStack;

	// Token: 0x04000DFE RID: 3582
	private float fertilizerProductionProgress;
}
