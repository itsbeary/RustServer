using System;
using System.Collections.Generic;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x020003EE RID: 1006
public class EntityFuelSystem
{
	// Token: 0x0600229C RID: 8860 RVA: 0x000DF3B4 File Offset: 0x000DD5B4
	public EntityFuelSystem(bool isServer, GameObjectRef fuelStoragePrefab, List<BaseEntity> children, bool editorGiveFreeFuel = true)
	{
		this.isServer = isServer;
		this.editorGiveFreeFuel = editorGiveFreeFuel;
		this.fuelStorageID = fuelStoragePrefab.GetEntity().prefabID;
		if (isServer)
		{
			foreach (BaseEntity baseEntity in children)
			{
				this.CheckNewChild(baseEntity);
			}
		}
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x000DF42C File Offset: 0x000DD62C
	public bool IsInFuelInteractionRange(BasePlayer player)
	{
		StorageContainer fuelContainer = this.GetFuelContainer();
		if (fuelContainer != null)
		{
			float num = 0f;
			if (this.isServer)
			{
				num = 3f;
			}
			return fuelContainer.Distance(player.eyes.position) <= num;
		}
		return false;
	}

	// Token: 0x0600229E RID: 8862 RVA: 0x000DF478 File Offset: 0x000DD678
	private StorageContainer GetFuelContainer()
	{
		StorageContainer storageContainer = this.fuelStorageInstance.Get(this.isServer);
		if (storageContainer.IsValid())
		{
			return storageContainer;
		}
		return null;
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x000DF4A2 File Offset: 0x000DD6A2
	public void CheckNewChild(BaseEntity child)
	{
		if (child.prefabID == this.fuelStorageID)
		{
			this.fuelStorageInstance.Set((StorageContainer)child);
		}
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x000DF4C4 File Offset: 0x000DD6C4
	public Item GetFuelItem()
	{
		StorageContainer fuelContainer = this.GetFuelContainer();
		if (fuelContainer == null)
		{
			return null;
		}
		return fuelContainer.inventory.GetSlot(0);
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x000DF4F0 File Offset: 0x000DD6F0
	public int GetFuelAmount()
	{
		Item fuelItem = this.GetFuelItem();
		if (fuelItem == null || fuelItem.amount < 1)
		{
			return 0;
		}
		return fuelItem.amount;
	}

	// Token: 0x060022A2 RID: 8866 RVA: 0x000DF518 File Offset: 0x000DD718
	public float GetFuelFraction()
	{
		Item fuelItem = this.GetFuelItem();
		if (fuelItem == null || fuelItem.amount < 1)
		{
			return 0f;
		}
		return Mathf.Clamp01((float)fuelItem.amount / (float)fuelItem.MaxStackable());
	}

	// Token: 0x060022A3 RID: 8867 RVA: 0x000DF554 File Offset: 0x000DD754
	public bool HasFuel(bool forceCheck = false)
	{
		if (Time.time > this.nextFuelCheckTime || forceCheck)
		{
			this.cachedHasFuel = (float)this.GetFuelAmount() > 0f;
			this.nextFuelCheckTime = Time.time + UnityEngine.Random.Range(1f, 2f);
		}
		return this.cachedHasFuel;
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x000DF5A8 File Offset: 0x000DD7A8
	public int TryUseFuel(float seconds, float fuelUsedPerSecond)
	{
		StorageContainer fuelContainer = this.GetFuelContainer();
		if (fuelContainer == null)
		{
			return 0;
		}
		Item slot = fuelContainer.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		this.pendingFuel += seconds * fuelUsedPerSecond;
		if (this.pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(this.pendingFuel);
			slot.UseItem(num);
			Analytics.Azure.AddPendingItems(((fuelContainer != null) ? fuelContainer.GetParentEntity() : null) ?? fuelContainer, slot.info.shortname, num, "fuel_system", true, false);
			this.pendingFuel -= (float)num;
			return num;
		}
		return 0;
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x000DF64D File Offset: 0x000DD84D
	public void LootFuel(BasePlayer player)
	{
		if (this.IsInFuelInteractionRange(player))
		{
			this.GetFuelContainer().PlayerOpenLoot(player, "", true);
		}
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x000DF66B File Offset: 0x000DD86B
	public void AddStartingFuel(int amount)
	{
		this.GetFuelContainer().inventory.AddItem(this.GetFuelContainer().allowedItem, Mathf.FloorToInt((float)amount), 0UL, ItemContainer.LimitStack.Existing);
	}

	// Token: 0x060022A7 RID: 8871 RVA: 0x000DF692 File Offset: 0x000DD892
	public void AdminAddFuel()
	{
		this.GetFuelContainer().inventory.AddItem(this.GetFuelContainer().allowedItem, this.GetFuelContainer().allowedItem.stackable, 0UL, ItemContainer.LimitStack.Existing);
	}

	// Token: 0x060022A8 RID: 8872 RVA: 0x000DF6C2 File Offset: 0x000DD8C2
	public int GetFuelCapacity()
	{
		return this.GetFuelContainer().allowedItem.stackable;
	}

	// Token: 0x04001A98 RID: 6808
	private readonly bool isServer;

	// Token: 0x04001A99 RID: 6809
	private readonly bool editorGiveFreeFuel;

	// Token: 0x04001A9A RID: 6810
	private readonly uint fuelStorageID;

	// Token: 0x04001A9B RID: 6811
	public EntityRef<StorageContainer> fuelStorageInstance;

	// Token: 0x04001A9C RID: 6812
	private float nextFuelCheckTime;

	// Token: 0x04001A9D RID: 6813
	private bool cachedHasFuel;

	// Token: 0x04001A9E RID: 6814
	private float pendingFuel;
}
