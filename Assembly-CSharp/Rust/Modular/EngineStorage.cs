using System;
using System.Linq;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B35 RID: 2869
	public class EngineStorage : StorageContainer
	{
		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x06004565 RID: 17765 RVA: 0x00195ADA File Offset: 0x00193CDA
		// (set) Token: 0x06004566 RID: 17766 RVA: 0x00195AE2 File Offset: 0x00193CE2
		public bool isUsable { get; private set; }

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x06004567 RID: 17767 RVA: 0x00195AEB File Offset: 0x00193CEB
		// (set) Token: 0x06004568 RID: 17768 RVA: 0x00195AF3 File Offset: 0x00193CF3
		public float accelerationBoostPercent { get; private set; }

		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x06004569 RID: 17769 RVA: 0x00195AFC File Offset: 0x00193CFC
		// (set) Token: 0x0600456A RID: 17770 RVA: 0x00195B04 File Offset: 0x00193D04
		public float topSpeedBoostPercent { get; private set; }

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x0600456B RID: 17771 RVA: 0x00195B0D File Offset: 0x00193D0D
		// (set) Token: 0x0600456C RID: 17772 RVA: 0x00195B15 File Offset: 0x00193D15
		public float fuelEconomyBoostPercent { get; private set; }

		// Token: 0x0600456D RID: 17773 RVA: 0x00195B20 File Offset: 0x00193D20
		public VehicleModuleEngine GetEngineModule()
		{
			global::BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity != null)
			{
				return parentEntity.GetComponent<VehicleModuleEngine>();
			}
			return null;
		}

		// Token: 0x0600456E RID: 17774 RVA: 0x00195B45 File Offset: 0x00193D45
		public float GetAveragedLoadoutPercent()
		{
			return (this.accelerationBoostPercent + this.topSpeedBoostPercent + this.fuelEconomyBoostPercent) / 3f;
		}

		// Token: 0x0600456F RID: 17775 RVA: 0x00195B64 File Offset: 0x00193D64
		public override void Load(global::BaseNetworkable.LoadInfo info)
		{
			base.Load(info);
			if (info.msg.engineStorage != null)
			{
				this.isUsable = info.msg.engineStorage.isUsable;
				this.accelerationBoostPercent = info.msg.engineStorage.accelerationBoost;
				this.topSpeedBoostPercent = info.msg.engineStorage.topSpeedBoost;
				this.fuelEconomyBoostPercent = info.msg.engineStorage.fuelEconomyBoost;
			}
			VehicleModuleEngine engineModule = this.GetEngineModule();
			if (engineModule == null)
			{
				return;
			}
			engineModule.RefreshPerformanceStats(this);
		}

		// Token: 0x06004570 RID: 17776 RVA: 0x00195BF0 File Offset: 0x00193DF0
		public override bool CanBeLooted(global::BasePlayer player)
		{
			VehicleModuleEngine engineModule = this.GetEngineModule();
			return engineModule != null && engineModule.CanBeLooted(player);
		}

		// Token: 0x06004571 RID: 17777 RVA: 0x00195C16 File Offset: 0x00193E16
		public override int GetIdealSlot(global::BasePlayer player, global::Item item)
		{
			return this.GetValidSlot(item);
		}

		// Token: 0x06004572 RID: 17778 RVA: 0x00195C20 File Offset: 0x00193E20
		private int GetValidSlot(global::Item item)
		{
			ItemModEngineItem component = item.info.GetComponent<ItemModEngineItem>();
			if (component == null)
			{
				return -1;
			}
			EngineStorage.EngineItemTypes engineItemType = component.engineItemType;
			for (int i = 0; i < this.inventorySlots; i++)
			{
				if (engineItemType == this.slotTypes[i] && !base.inventory.SlotTaken(item, i))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06004573 RID: 17779 RVA: 0x00195C79 File Offset: 0x00193E79
		public override void OnInventoryFirstCreated(global::ItemContainer container)
		{
			this.RefreshLoadoutData();
		}

		// Token: 0x06004574 RID: 17780 RVA: 0x000063A5 File Offset: 0x000045A5
		public void NonUserSpawn()
		{
		}

		// Token: 0x06004575 RID: 17781 RVA: 0x00195C79 File Offset: 0x00193E79
		public override void OnItemAddedOrRemoved(global::Item item, bool added)
		{
			this.RefreshLoadoutData();
		}

		// Token: 0x06004576 RID: 17782 RVA: 0x00195C84 File Offset: 0x00193E84
		public override bool ItemFilter(global::Item item, int targetSlot)
		{
			if (!base.ItemFilter(item, targetSlot))
			{
				return false;
			}
			if (targetSlot < 0 || targetSlot >= this.slotTypes.Length)
			{
				return false;
			}
			ItemModEngineItem component = item.info.GetComponent<ItemModEngineItem>();
			return component != null && component.engineItemType == this.slotTypes[targetSlot];
		}

		// Token: 0x06004577 RID: 17783 RVA: 0x00195CD8 File Offset: 0x00193ED8
		public void RefreshLoadoutData()
		{
			bool flag;
			if (base.inventory.IsFull())
			{
				flag = base.inventory.itemList.All((global::Item item) => !item.isBroken);
			}
			else
			{
				flag = false;
			}
			this.isUsable = flag;
			this.accelerationBoostPercent = this.GetContainerItemsValueFor(new Func<EngineStorage.EngineItemTypes, bool>(EngineItemTypeEx.BoostsAcceleration)) / (float)this.accelerationBoostSlots;
			this.topSpeedBoostPercent = this.GetContainerItemsValueFor(new Func<EngineStorage.EngineItemTypes, bool>(EngineItemTypeEx.BoostsTopSpeed)) / (float)this.topSpeedBoostSlots;
			this.fuelEconomyBoostPercent = this.GetContainerItemsValueFor(new Func<EngineStorage.EngineItemTypes, bool>(EngineItemTypeEx.BoostsFuelEconomy)) / (float)this.fuelEconomyBoostSlots;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			VehicleModuleEngine engineModule = this.GetEngineModule();
			if (engineModule == null)
			{
				return;
			}
			engineModule.RefreshPerformanceStats(this);
		}

		// Token: 0x06004578 RID: 17784 RVA: 0x00195DA4 File Offset: 0x00193FA4
		public override void Save(global::BaseNetworkable.SaveInfo info)
		{
			base.Save(info);
			info.msg.engineStorage = Pool.Get<EngineStorage>();
			info.msg.engineStorage.isUsable = this.isUsable;
			info.msg.engineStorage.accelerationBoost = this.accelerationBoostPercent;
			info.msg.engineStorage.topSpeedBoost = this.topSpeedBoostPercent;
			info.msg.engineStorage.fuelEconomyBoost = this.fuelEconomyBoostPercent;
		}

		// Token: 0x06004579 RID: 17785 RVA: 0x00195E20 File Offset: 0x00194020
		public void OnModuleDamaged(float damageTaken)
		{
			if (damageTaken <= 0f)
			{
				return;
			}
			damageTaken *= this.internalDamageMultiplier;
			float[] array = new float[base.inventory.capacity];
			float num = 0f;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = UnityEngine.Random.value;
				num += array[i];
			}
			float num2 = damageTaken / num;
			for (int j = 0; j < array.Length; j++)
			{
				global::Item slot = base.inventory.GetSlot(j);
				if (slot != null)
				{
					slot.condition -= array[j] * num2;
				}
			}
			this.RefreshLoadoutData();
		}

		// Token: 0x0600457A RID: 17786 RVA: 0x00195EB8 File Offset: 0x001940B8
		public void AdminAddParts(int tier)
		{
			if (base.inventory == null)
			{
				Debug.LogWarning(base.GetType().Name + ": Null inventory on " + base.name);
				return;
			}
			for (int i = 0; i < base.inventory.capacity; i++)
			{
				global::Item slot = base.inventory.GetSlot(i);
				if (slot != null)
				{
					slot.RemoveFromContainer();
					slot.Remove(0f);
				}
			}
			for (int j = 0; j < base.inventory.capacity; j++)
			{
				ItemModEngineItem itemModEngineItem;
				if (base.inventory.GetSlot(j) == null && this.allEngineItems.TryGetItem(tier, this.slotTypes[j], out itemModEngineItem))
				{
					ItemDefinition component = itemModEngineItem.GetComponent<ItemDefinition>();
					global::Item item = ItemManager.Create(component, 1, 0UL);
					if (item != null)
					{
						item.condition = component.condition.max;
						item.MoveToContainer(base.inventory, j, false, false, null, true);
					}
					else
					{
						Debug.LogError(base.GetType().Name + ": Failed to create engine storage item.");
					}
				}
			}
		}

		// Token: 0x0600457B RID: 17787 RVA: 0x00195FC4 File Offset: 0x001941C4
		private float GetContainerItemsValueFor(Func<EngineStorage.EngineItemTypes, bool> boostConditional)
		{
			float num = 0f;
			foreach (global::Item item in base.inventory.itemList)
			{
				ItemModEngineItem component = item.info.GetComponent<ItemModEngineItem>();
				if (component != null && boostConditional(component.engineItemType) && !item.isBroken)
				{
					num += (float)item.amount * this.GetTierValue(component.tier);
				}
			}
			return num;
		}

		// Token: 0x0600457C RID: 17788 RVA: 0x00196060 File Offset: 0x00194260
		private float GetTierValue(int tier)
		{
			switch (tier)
			{
			case 1:
				return 0.6f;
			case 2:
				return 0.8f;
			case 3:
				return 1f;
			default:
				Debug.LogError(base.GetType().Name + ": Unrecognised item tier: " + tier);
				return 0f;
			}
		}

		// Token: 0x04003E6C RID: 15980
		[Header("Engine Storage")]
		public Sprite engineIcon;

		// Token: 0x04003E6D RID: 15981
		public float internalDamageMultiplier = 0.5f;

		// Token: 0x04003E6E RID: 15982
		public EngineStorage.EngineItemTypes[] slotTypes;

		// Token: 0x04003E6F RID: 15983
		[SerializeField]
		private VehicleModuleEngineItems allEngineItems;

		// Token: 0x04003E70 RID: 15984
		[SerializeField]
		[ReadOnly]
		private int accelerationBoostSlots;

		// Token: 0x04003E71 RID: 15985
		[SerializeField]
		[ReadOnly]
		private int topSpeedBoostSlots;

		// Token: 0x04003E72 RID: 15986
		[SerializeField]
		[ReadOnly]
		private int fuelEconomyBoostSlots;

		// Token: 0x02000FA7 RID: 4007
		public enum EngineItemTypes
		{
			// Token: 0x040050F0 RID: 20720
			Crankshaft,
			// Token: 0x040050F1 RID: 20721
			Carburetor,
			// Token: 0x040050F2 RID: 20722
			SparkPlug,
			// Token: 0x040050F3 RID: 20723
			Piston,
			// Token: 0x040050F4 RID: 20724
			Valve
		}
	}
}
