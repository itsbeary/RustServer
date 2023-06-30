using System;
using Facepunch;
using Network;
using ProtoBuf;

// Token: 0x020004CB RID: 1227
public class StorageMonitor : AppIOEntity
{
	// Token: 0x17000365 RID: 869
	// (get) Token: 0x06002814 RID: 10260 RVA: 0x000037BE File Offset: 0x000019BE
	public override AppEntityType Type
	{
		get
		{
			return AppEntityType.StorageMonitor;
		}
	}

	// Token: 0x17000366 RID: 870
	// (get) Token: 0x06002815 RID: 10261 RVA: 0x00007649 File Offset: 0x00005849
	// (set) Token: 0x06002816 RID: 10262 RVA: 0x000063A5 File Offset: 0x000045A5
	public override bool Value
	{
		get
		{
			return base.IsOn();
		}
		set
		{
		}
	}

	// Token: 0x06002817 RID: 10263 RVA: 0x000F9AC1 File Offset: 0x000F7CC1
	public StorageMonitor()
	{
		this._onContainerChangedHandler = new Action<global::Item, bool>(this.OnContainerChanged);
		this._resetSwitchHandler = new Action(this.ResetSwitch);
	}

	// Token: 0x06002818 RID: 10264 RVA: 0x000F9AF0 File Offset: 0x000F7CF0
	internal override void FillEntityPayload(AppEntityPayload payload)
	{
		base.FillEntityPayload(payload);
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer == null || !base.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			return;
		}
		payload.items = Pool.GetList<AppEntityPayload.Item>();
		foreach (global::Item item in storageContainer.inventory.itemList)
		{
			AppEntityPayload.Item item2 = Pool.Get<AppEntityPayload.Item>();
			item2.itemId = (item.IsBlueprint() ? item.blueprintTargetDef.itemid : item.info.itemid);
			item2.quantity = item.amount;
			item2.itemIsBlueprint = item.IsBlueprint();
			payload.items.Add(item2);
		}
		payload.capacity = storageContainer.inventory.capacity;
		BuildingPrivlidge buildingPrivlidge;
		if ((buildingPrivlidge = storageContainer as BuildingPrivlidge) != null)
		{
			payload.hasProtection = true;
			float protectedMinutes = buildingPrivlidge.GetProtectedMinutes(false);
			if (protectedMinutes > 0f)
			{
				payload.protectionExpiry = (uint)DateTimeOffset.UtcNow.AddMinutes((double)protectedMinutes).ToUnixTimeSeconds();
			}
		}
	}

	// Token: 0x06002819 RID: 10265 RVA: 0x000F9C1C File Offset: 0x000F7E1C
	public override void Init()
	{
		base.Init();
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer != null && storageContainer.inventory != null)
		{
			global::ItemContainer inventory = storageContainer.inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, this._onContainerChangedHandler);
		}
	}

	// Token: 0x0600281A RID: 10266 RVA: 0x000F9C68 File Offset: 0x000F7E68
	public override void DestroyShared()
	{
		base.DestroyShared();
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer != null && storageContainer.inventory != null)
		{
			global::ItemContainer inventory = storageContainer.inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Remove(inventory.onItemAddedRemoved, this._onContainerChangedHandler);
		}
	}

	// Token: 0x0600281B RID: 10267 RVA: 0x000F9CB4 File Offset: 0x000F7EB4
	private StorageContainer GetStorageContainer()
	{
		return base.GetParentEntity() as StorageContainer;
	}

	// Token: 0x0600281C RID: 10268 RVA: 0x00062B15 File Offset: 0x00060D15
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x0600281D RID: 10269 RVA: 0x000F9CC4 File Offset: 0x000F7EC4
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		bool flag = base.HasFlag(global::BaseEntity.Flags.Reserved8);
		base.UpdateHasPower(inputAmount, inputSlot);
		if (inputSlot == 0)
		{
			bool flag2 = inputAmount >= this.ConsumptionAmount();
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			if (flag2 && !flag && this._lastPowerOnUpdate < realtimeSinceStartup - 1.0)
			{
				this._lastPowerOnUpdate = realtimeSinceStartup;
				base.BroadcastValueChange();
			}
		}
	}

	// Token: 0x0600281E RID: 10270 RVA: 0x000F9D20 File Offset: 0x000F7F20
	private void OnContainerChanged(global::Item item, bool added)
	{
		if (!base.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			return;
		}
		base.Invoke(this._resetSwitchHandler, 0.5f);
		if (base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
		base.BroadcastValueChange();
	}

	// Token: 0x0600281F RID: 10271 RVA: 0x000F9D72 File Offset: 0x000F7F72
	private void ResetSwitch()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
		base.BroadcastValueChange();
	}

	// Token: 0x0400209D RID: 8349
	private readonly Action<global::Item, bool> _onContainerChangedHandler;

	// Token: 0x0400209E RID: 8350
	private readonly Action _resetSwitchHandler;

	// Token: 0x0400209F RID: 8351
	private double _lastPowerOnUpdate;
}
