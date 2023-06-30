using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B37 RID: 2871
	public class ModularVehicleInventory : IDisposable
	{
		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x0600457F RID: 17791 RVA: 0x0019613B File Offset: 0x0019433B
		public ItemContainer ModuleContainer { get; }

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x06004580 RID: 17792 RVA: 0x00196143 File Offset: 0x00194343
		public ItemContainer ChassisContainer { get; }

		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x06004581 RID: 17793 RVA: 0x0019614B File Offset: 0x0019434B
		public ItemContainerId UID
		{
			get
			{
				return this.ModuleContainer.uid;
			}
		}

		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x06004582 RID: 17794 RVA: 0x00196158 File Offset: 0x00194358
		private int TotalSockets
		{
			get
			{
				return this.vehicle.TotalSockets;
			}
		}

		// Token: 0x06004583 RID: 17795 RVA: 0x00196168 File Offset: 0x00194368
		public ModularVehicleInventory(BaseModularVehicle vehicle, ItemDefinition chassisItemDef, bool giveUID)
		{
			this.vehicle = vehicle;
			this.ModuleContainer = this.CreateModuleInventory(vehicle, giveUID);
			this.ChassisContainer = this.CreateChassisInventory(vehicle, giveUID);
			vehicle.AssociatedItemInstance = ItemManager.Create(chassisItemDef, 1, 0UL);
			if (!Application.isLoadingSave)
			{
				vehicle.AssociatedItemInstance.MoveToContainer(this.ChassisContainer, 0, false, false, null, true);
			}
		}

		// Token: 0x06004584 RID: 17796 RVA: 0x001961CC File Offset: 0x001943CC
		public void Dispose()
		{
			foreach (Item item in this.ModuleContainer.itemList)
			{
				item.OnDirty -= this.OnModuleItemChanged;
			}
		}

		// Token: 0x06004585 RID: 17797 RVA: 0x00196230 File Offset: 0x00194430
		public void GiveUIDs()
		{
			this.ModuleContainer.GiveUID();
			this.ChassisContainer.GiveUID();
		}

		// Token: 0x06004586 RID: 17798 RVA: 0x00196248 File Offset: 0x00194448
		public bool SocketIsFree(int socketIndex, Item moduleItem = null)
		{
			Item item = null;
			int num = socketIndex;
			while (item == null && num >= 0)
			{
				item = this.ModuleContainer.GetSlot(num);
				if (item != null)
				{
					if (item == moduleItem)
					{
						return true;
					}
					ItemModVehicleModule component = item.info.GetComponent<ItemModVehicleModule>();
					return num + component.socketsTaken - 1 < socketIndex;
				}
				else
				{
					num--;
				}
			}
			return true;
		}

		// Token: 0x06004587 RID: 17799 RVA: 0x00196297 File Offset: 0x00194497
		public bool SocketIsTaken(int socketIndex)
		{
			return !this.SocketIsFree(socketIndex, null);
		}

		// Token: 0x06004588 RID: 17800 RVA: 0x001962A4 File Offset: 0x001944A4
		public bool TryAddModuleItem(Item moduleItem, int socketIndex)
		{
			if (moduleItem == null)
			{
				Debug.LogError(base.GetType().Name + ": Can't add null item.");
				return false;
			}
			return moduleItem.MoveToContainer(this.ModuleContainer, socketIndex, false, false, null, true);
		}

		// Token: 0x06004589 RID: 17801 RVA: 0x001962D6 File Offset: 0x001944D6
		public bool RemoveAndDestroy(Item itemToRemove)
		{
			bool flag = this.ModuleContainer.Remove(itemToRemove);
			itemToRemove.Remove(0f);
			return flag;
		}

		// Token: 0x0600458A RID: 17802 RVA: 0x001962EF File Offset: 0x001944EF
		public int TryGetFreeSocket(int socketsTaken)
		{
			return this.TryGetFreeSocket(null, socketsTaken);
		}

		// Token: 0x0600458B RID: 17803 RVA: 0x001962FC File Offset: 0x001944FC
		public int TryGetFreeSocket(Item moduleItem, int socketsTaken)
		{
			for (int i = 0; i <= this.TotalSockets - socketsTaken; i++)
			{
				if (this.SocketsAreFree(i, socketsTaken, moduleItem))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600458C RID: 17804 RVA: 0x0019632C File Offset: 0x0019452C
		public bool SocketsAreFree(int firstIndex, int socketsTaken, Item moduleItem = null)
		{
			if (firstIndex < 0 || firstIndex + socketsTaken > this.TotalSockets)
			{
				return false;
			}
			for (int i = firstIndex; i < firstIndex + socketsTaken; i++)
			{
				if (!this.SocketIsFree(i, moduleItem))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600458D RID: 17805 RVA: 0x00196368 File Offset: 0x00194568
		public bool TrySyncModuleInventory(BaseVehicleModule moduleEntity, int firstSocketIndex)
		{
			if (firstSocketIndex < 0)
			{
				Debug.LogError(string.Format("{0}: Invalid socket index ({1}) for new module entity.", base.GetType().Name, firstSocketIndex), this.vehicle.gameObject);
				return false;
			}
			Item slot = this.ModuleContainer.GetSlot(firstSocketIndex);
			int numSocketsTaken = moduleEntity.GetNumSocketsTaken();
			if (!this.SocketsAreFree(firstSocketIndex, numSocketsTaken, null) && (slot == null || moduleEntity.AssociatedItemInstance != slot))
			{
				Debug.LogError(string.Format("{0}: Sockets are not free for new module entity. First: {1} Taken: {2}", base.GetType().Name, firstSocketIndex, numSocketsTaken), this.vehicle.gameObject);
				return false;
			}
			if (slot != null)
			{
				return true;
			}
			Item item = ItemManager.Create(moduleEntity.AssociatedItemDef, 1, 0UL);
			item.condition = moduleEntity.health;
			moduleEntity.AssociatedItemInstance = item;
			bool flag = this.TryAddModuleItem(item, firstSocketIndex);
			if (flag)
			{
				this.vehicle.SetUpModule(moduleEntity, item);
				return flag;
			}
			item.Remove(0f);
			return flag;
		}

		// Token: 0x0600458E RID: 17806 RVA: 0x0019644F File Offset: 0x0019464F
		private bool SocketIsUsed(Item item, int slotIndex)
		{
			return !this.SocketIsFree(slotIndex, item);
		}

		// Token: 0x0600458F RID: 17807 RVA: 0x0019645C File Offset: 0x0019465C
		private ItemContainer CreateModuleInventory(BaseModularVehicle vehicle, bool giveUID)
		{
			if (this.ModuleContainer != null)
			{
				return this.ModuleContainer;
			}
			ItemContainer itemContainer = new ItemContainer
			{
				entityOwner = vehicle,
				allowedContents = ItemContainer.ContentsType.Generic,
				maxStackSize = 1
			};
			itemContainer.ServerInitialize(null, this.TotalSockets);
			if (giveUID)
			{
				itemContainer.GiveUID();
			}
			itemContainer.onItemAddedRemoved = new Action<Item, bool>(this.OnSocketInventoryAddRemove);
			itemContainer.canAcceptItem = new Func<Item, int, bool>(this.ItemFilter);
			itemContainer.slotIsReserved = new Func<Item, int, bool>(this.SocketIsUsed);
			return itemContainer;
		}

		// Token: 0x06004590 RID: 17808 RVA: 0x001964E0 File Offset: 0x001946E0
		private ItemContainer CreateChassisInventory(BaseModularVehicle vehicle, bool giveUID)
		{
			if (this.ChassisContainer != null)
			{
				return this.ChassisContainer;
			}
			ItemContainer itemContainer = new ItemContainer
			{
				entityOwner = vehicle,
				allowedContents = ItemContainer.ContentsType.Generic,
				maxStackSize = 1
			};
			itemContainer.ServerInitialize(null, 1);
			if (giveUID)
			{
				itemContainer.GiveUID();
			}
			return itemContainer;
		}

		// Token: 0x06004591 RID: 17809 RVA: 0x00196529 File Offset: 0x00194729
		private void OnSocketInventoryAddRemove(Item moduleItem, bool added)
		{
			if (added)
			{
				this.ModuleItemAdded(moduleItem, moduleItem.position);
				return;
			}
			this.ModuleItemRemoved(moduleItem);
		}

		// Token: 0x06004592 RID: 17810 RVA: 0x00196544 File Offset: 0x00194744
		private void ModuleItemAdded(Item moduleItem, int socketIndex)
		{
			ItemModVehicleModule component = moduleItem.info.GetComponent<ItemModVehicleModule>();
			if (!Application.isLoadingSave && this.vehicle.GetModuleForItem(moduleItem) == null)
			{
				this.vehicle.CreatePhysicalModuleEntity(moduleItem, component, socketIndex);
			}
			moduleItem.OnDirty += this.OnModuleItemChanged;
		}

		// Token: 0x06004593 RID: 17811 RVA: 0x0019659C File Offset: 0x0019479C
		private void ModuleItemRemoved(Item moduleItem)
		{
			if (moduleItem == null)
			{
				Debug.LogError("Null module item removed.", this.vehicle.gameObject);
				return;
			}
			moduleItem.OnDirty -= this.OnModuleItemChanged;
			BaseVehicleModule moduleForItem = this.vehicle.GetModuleForItem(moduleItem);
			if (moduleForItem != null)
			{
				if (!moduleForItem.IsFullySpawned())
				{
					Debug.LogError("Module entity being removed before it's fully spawned. This could cause errors.", this.vehicle.gameObject);
				}
				moduleForItem.Kill(BaseNetworkable.DestroyMode.None);
				return;
			}
			Debug.Log("Couldn't find entity for this item.");
		}

		// Token: 0x06004594 RID: 17812 RVA: 0x0019661C File Offset: 0x0019481C
		private void OnModuleItemChanged(Item moduleItem)
		{
			BaseVehicleModule moduleForItem = this.vehicle.GetModuleForItem(moduleItem);
			if (moduleForItem != null)
			{
				moduleForItem.SetHealth(moduleItem.condition);
				if (moduleForItem.FirstSocketIndex != moduleItem.position)
				{
					this.ModuleItemRemoved(moduleItem);
					this.ModuleItemAdded(moduleItem, moduleItem.position);
				}
			}
		}

		// Token: 0x06004595 RID: 17813 RVA: 0x00196670 File Offset: 0x00194870
		private bool ItemFilter(Item item, int targetSlot)
		{
			string text;
			return this.vehicle.ModuleCanBeAdded(item, targetSlot, out text);
		}

		// Token: 0x04003E8E RID: 16014
		private readonly BaseModularVehicle vehicle;
	}
}
