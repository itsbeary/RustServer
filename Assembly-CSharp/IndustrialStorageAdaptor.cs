using System;
using UnityEngine;

// Token: 0x020004DF RID: 1247
public class IndustrialStorageAdaptor : IndustrialEntity, IIndustrialStorage
{
	// Token: 0x1700036E RID: 878
	// (get) Token: 0x0600287F RID: 10367 RVA: 0x000FAA86 File Offset: 0x000F8C86
	public BaseEntity cachedParent
	{
		get
		{
			if (this._cachedParent == null)
			{
				this._cachedParent = base.GetParentEntity();
			}
			return this._cachedParent;
		}
	}

	// Token: 0x1700036F RID: 879
	// (get) Token: 0x06002880 RID: 10368 RVA: 0x000FAAA8 File Offset: 0x000F8CA8
	public ItemContainer Container
	{
		get
		{
			if (this.cachedContainer == null)
			{
				StorageContainer storageContainer = this.cachedParent as StorageContainer;
				this.cachedContainer = ((storageContainer != null) ? storageContainer.inventory : null);
			}
			return this.cachedContainer;
		}
	}

	// Token: 0x06002881 RID: 10369 RVA: 0x000FAAD5 File Offset: 0x000F8CD5
	public override void ServerInit()
	{
		base.ServerInit();
		this._cachedParent = null;
		this.cachedContainer = null;
	}

	// Token: 0x06002882 RID: 10370 RVA: 0x000FAAEC File Offset: 0x000F8CEC
	public Vector2i InputSlotRange(int slotIndex)
	{
		if (this.cachedParent != null)
		{
			IIndustrialStorage industrialStorage;
			if ((industrialStorage = this.cachedParent as IIndustrialStorage) != null)
			{
				return industrialStorage.InputSlotRange(slotIndex);
			}
			Locker locker;
			if ((locker = this.cachedParent as Locker) != null)
			{
				Vector3 localPosition = base.transform.localPosition;
				return locker.GetIndustrialSlotRange(localPosition);
			}
		}
		if (this.Container != null)
		{
			return new Vector2i(0, this.Container.capacity - 1);
		}
		return new Vector2i(0, 0);
	}

	// Token: 0x06002883 RID: 10371 RVA: 0x000FAB64 File Offset: 0x000F8D64
	public Vector2i OutputSlotRange(int slotIndex)
	{
		if (this.cachedParent != null)
		{
			if (this.cachedParent is DropBox && this.Container != null)
			{
				return new Vector2i(0, this.Container.capacity - 2);
			}
			IIndustrialStorage industrialStorage;
			if ((industrialStorage = this.cachedParent as IIndustrialStorage) != null)
			{
				return industrialStorage.OutputSlotRange(slotIndex);
			}
			Locker locker;
			if ((locker = this.cachedParent as Locker) != null)
			{
				Vector3 localPosition = base.transform.localPosition;
				return locker.GetIndustrialSlotRange(localPosition);
			}
		}
		if (this.Container != null)
		{
			return new Vector2i(0, this.Container.capacity - 1);
		}
		return new Vector2i(0, 0);
	}

	// Token: 0x06002884 RID: 10372 RVA: 0x000FAC08 File Offset: 0x000F8E08
	public void OnStorageItemTransferBegin()
	{
		VendingMachine vendingMachine;
		if (this.cachedParent != null && (vendingMachine = this.cachedParent as VendingMachine) != null)
		{
			vendingMachine.OnIndustrialItemTransferBegins();
		}
	}

	// Token: 0x06002885 RID: 10373 RVA: 0x000FAC38 File Offset: 0x000F8E38
	public void OnStorageItemTransferEnd()
	{
		VendingMachine vendingMachine;
		if (this.cachedParent != null && (vendingMachine = this.cachedParent as VendingMachine) != null)
		{
			vendingMachine.OnIndustrialItemTransferEnds();
		}
	}

	// Token: 0x17000370 RID: 880
	// (get) Token: 0x06002886 RID: 10374 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity IndustrialEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06002887 RID: 10375 RVA: 0x00007A44 File Offset: 0x00005C44
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x06002888 RID: 10376 RVA: 0x000FAC68 File Offset: 0x000F8E68
	public void ClientNotifyItemAddRemoved(bool add)
	{
		if (add)
		{
			this.GreenLight.SetActive(false);
			this.GreenLight.SetActive(true);
			return;
		}
		this.RedLight.SetActive(false);
		this.RedLight.SetActive(true);
	}

	// Token: 0x040020D7 RID: 8407
	public GameObject GreenLight;

	// Token: 0x040020D8 RID: 8408
	public GameObject RedLight;

	// Token: 0x040020D9 RID: 8409
	private BaseEntity _cachedParent;

	// Token: 0x040020DA RID: 8410
	private ItemContainer cachedContainer;
}
