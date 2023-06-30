using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020005D1 RID: 1489
public sealed class ItemContainer
{
	// Token: 0x06002CF1 RID: 11505 RVA: 0x001106EF File Offset: 0x0010E8EF
	public bool HasFlag(global::ItemContainer.Flag f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x06002CF2 RID: 11506 RVA: 0x001106FC File Offset: 0x0010E8FC
	public void SetFlag(global::ItemContainer.Flag f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	// Token: 0x06002CF3 RID: 11507 RVA: 0x0011071F File Offset: 0x0010E91F
	public bool IsLocked()
	{
		return this.HasFlag(global::ItemContainer.Flag.IsLocked);
	}

	// Token: 0x06002CF4 RID: 11508 RVA: 0x00110729 File Offset: 0x0010E929
	public bool PlayerItemInputBlocked()
	{
		return this.HasFlag(global::ItemContainer.Flag.NoItemInput);
	}

	// Token: 0x170003AD RID: 941
	// (get) Token: 0x06002CF5 RID: 11509 RVA: 0x00110736 File Offset: 0x0010E936
	public bool HasLimitedAllowedItems
	{
		get
		{
			return this.onlyAllowedItems != null && this.onlyAllowedItems.Length != 0;
		}
	}

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x06002CF6 RID: 11510 RVA: 0x0011074C File Offset: 0x0010E94C
	// (remove) Token: 0x06002CF7 RID: 11511 RVA: 0x00110784 File Offset: 0x0010E984
	public event Action onDirty;

	// Token: 0x06002CF8 RID: 11512 RVA: 0x001107BC File Offset: 0x0010E9BC
	public float GetTemperature(int slot)
	{
		global::BaseOven baseOven;
		if ((baseOven = this.entityOwner as global::BaseOven) != null)
		{
			return baseOven.GetTemperature(slot);
		}
		return this.temperature;
	}

	// Token: 0x06002CF9 RID: 11513 RVA: 0x001107E6 File Offset: 0x0010E9E6
	public void ServerInitialize(global::Item parentItem, int iMaxCapacity)
	{
		this.parent = parentItem;
		this.capacity = iMaxCapacity;
		this.uid = default(ItemContainerId);
		this.isServer = true;
		if (this.allowedContents == (global::ItemContainer.ContentsType)0)
		{
			this.allowedContents = global::ItemContainer.ContentsType.Generic;
		}
		this.MarkDirty();
	}

	// Token: 0x06002CFA RID: 11514 RVA: 0x0011081E File Offset: 0x0010EA1E
	public void GiveUID()
	{
		Assert.IsTrue(!this.uid.IsValid, "Calling GiveUID - but already has a uid!");
		this.uid = new ItemContainerId(Net.sv.TakeUID());
	}

	// Token: 0x06002CFB RID: 11515 RVA: 0x0011084D File Offset: 0x0010EA4D
	public void MarkDirty()
	{
		this.dirty = true;
		if (this.parent != null)
		{
			this.parent.MarkDirty();
		}
		if (this.onDirty != null)
		{
			this.onDirty();
		}
	}

	// Token: 0x06002CFC RID: 11516 RVA: 0x0011087C File Offset: 0x0010EA7C
	public DroppedItemContainer Drop(string prefab, Vector3 pos, Quaternion rot, float destroyPercent)
	{
		if (this.itemList == null || this.itemList.Count == 0)
		{
			return null;
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(prefab, pos, rot, true);
		if (baseEntity == null)
		{
			return null;
		}
		DroppedItemContainer droppedItemContainer = baseEntity as DroppedItemContainer;
		if (droppedItemContainer != null)
		{
			droppedItemContainer.TakeFrom(new global::ItemContainer[] { this }, destroyPercent);
		}
		droppedItemContainer.Spawn();
		return droppedItemContainer;
	}

	// Token: 0x06002CFD RID: 11517 RVA: 0x001108E8 File Offset: 0x0010EAE8
	public static DroppedItemContainer Drop(string prefab, Vector3 pos, Quaternion rot, params global::ItemContainer[] containers)
	{
		int num = 0;
		foreach (global::ItemContainer itemContainer in containers)
		{
			num += ((itemContainer.itemList != null) ? itemContainer.itemList.Count : 0);
		}
		if (num == 0)
		{
			return null;
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(prefab, pos, rot, true);
		if (baseEntity == null)
		{
			return null;
		}
		DroppedItemContainer droppedItemContainer = baseEntity as DroppedItemContainer;
		if (droppedItemContainer != null)
		{
			droppedItemContainer.TakeFrom(containers, 0f);
		}
		droppedItemContainer.Spawn();
		return droppedItemContainer;
	}

	// Token: 0x06002CFE RID: 11518 RVA: 0x00110968 File Offset: 0x0010EB68
	public global::BaseEntity GetEntityOwner(bool returnHeldEntity = false)
	{
		global::ItemContainer itemContainer = this;
		for (int i = 0; i < 10; i++)
		{
			if (itemContainer.entityOwner != null)
			{
				return itemContainer.entityOwner;
			}
			if (itemContainer.playerOwner != null)
			{
				return itemContainer.playerOwner;
			}
			if (returnHeldEntity)
			{
				global::Item item = itemContainer.parent;
				global::BaseEntity baseEntity = ((item != null) ? item.GetHeldEntity() : null);
				if (baseEntity != null)
				{
					return baseEntity;
				}
			}
			global::Item item2 = itemContainer.parent;
			global::ItemContainer itemContainer2 = ((item2 != null) ? item2.parent : null);
			if (itemContainer2 == null || itemContainer2 == itemContainer)
			{
				return null;
			}
			itemContainer = itemContainer2;
		}
		return null;
	}

	// Token: 0x06002CFF RID: 11519 RVA: 0x001109F0 File Offset: 0x0010EBF0
	public void OnChanged()
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].OnChanged();
		}
	}

	// Token: 0x06002D00 RID: 11520 RVA: 0x00110A24 File Offset: 0x0010EC24
	public global::Item FindItemByUID(ItemId iUID)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			global::Item item = this.itemList[i];
			if (item.IsValid())
			{
				global::Item item2 = item.FindItem(iUID);
				if (item2 != null)
				{
					return item2;
				}
			}
		}
		return null;
	}

	// Token: 0x06002D01 RID: 11521 RVA: 0x00110A6A File Offset: 0x0010EC6A
	public bool IsFull()
	{
		return this.itemList.Count >= this.capacity;
	}

	// Token: 0x06002D02 RID: 11522 RVA: 0x00110A82 File Offset: 0x0010EC82
	public bool IsEmpty()
	{
		return this.itemList.Count == 0;
	}

	// Token: 0x06002D03 RID: 11523 RVA: 0x00110A92 File Offset: 0x0010EC92
	public bool CanAccept(global::Item item)
	{
		return !this.IsFull();
	}

	// Token: 0x06002D04 RID: 11524 RVA: 0x00110AA0 File Offset: 0x0010ECA0
	public int GetMaxTransferAmount(ItemDefinition def)
	{
		int num = this.ContainerMaxStackSize();
		foreach (global::Item item in this.itemList)
		{
			if (item.info == def)
			{
				num -= item.amount;
				if (num <= 0)
				{
					return 0;
				}
			}
		}
		return num;
	}

	// Token: 0x06002D05 RID: 11525 RVA: 0x00110B18 File Offset: 0x0010ED18
	public void SetOnlyAllowedItem(ItemDefinition def)
	{
		this.SetOnlyAllowedItems(new ItemDefinition[] { def });
	}

	// Token: 0x06002D06 RID: 11526 RVA: 0x00110B2C File Offset: 0x0010ED2C
	public void SetOnlyAllowedItems(params ItemDefinition[] defs)
	{
		int num = 0;
		for (int i = 0; i < defs.Length; i++)
		{
			if (defs[i] != null)
			{
				num++;
			}
		}
		this.onlyAllowedItems = new ItemDefinition[num];
		int num2 = 0;
		foreach (ItemDefinition itemDefinition in defs)
		{
			if (itemDefinition != null)
			{
				this.onlyAllowedItems[num2] = itemDefinition;
				num2++;
			}
		}
	}

	// Token: 0x06002D07 RID: 11527 RVA: 0x00110B98 File Offset: 0x0010ED98
	internal bool Insert(global::Item item)
	{
		if (this.itemList.Contains(item))
		{
			return false;
		}
		if (this.IsFull())
		{
			return false;
		}
		this.itemList.Add(item);
		item.parent = this;
		if (!this.FindPosition(item))
		{
			return false;
		}
		this.MarkDirty();
		if (this.onItemAddedRemoved != null)
		{
			this.onItemAddedRemoved(item, true);
		}
		return true;
	}

	// Token: 0x06002D08 RID: 11528 RVA: 0x00110BF9 File Offset: 0x0010EDF9
	public bool SlotTaken(global::Item item, int i)
	{
		return (this.slotIsReserved != null && this.slotIsReserved(item, i)) || this.GetSlot(i) != null;
	}

	// Token: 0x06002D09 RID: 11529 RVA: 0x00110C20 File Offset: 0x0010EE20
	public global::Item GetSlot(int slot)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].position == slot)
			{
				return this.itemList[i];
			}
		}
		return null;
	}

	// Token: 0x06002D0A RID: 11530 RVA: 0x00110C68 File Offset: 0x0010EE68
	public global::Item GetNonFullStackWithinRange(global::Item def, Vector2i range)
	{
		int count = this.itemList.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.itemList[i].amount < this.itemList[i].info.stackable && this.itemList[i].position >= range.x && this.itemList[i].position <= range.y)
			{
				if (def.IsBlueprint())
				{
					if (this.itemList[i].blueprintTarget != def.blueprintTarget)
					{
						goto IL_BF;
					}
				}
				else if (this.itemList[i].info != def.info)
				{
					goto IL_BF;
				}
				return this.itemList[i];
			}
			IL_BF:;
		}
		return null;
	}

	// Token: 0x06002D0B RID: 11531 RVA: 0x00110D40 File Offset: 0x0010EF40
	internal bool FindPosition(global::Item item)
	{
		int position = item.position;
		item.position = -1;
		if (position >= 0 && !this.SlotTaken(item, position))
		{
			item.position = position;
			return true;
		}
		for (int i = 0; i < this.capacity; i++)
		{
			if (!this.SlotTaken(item, i))
			{
				item.position = i;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002D0C RID: 11532 RVA: 0x00110D97 File Offset: 0x0010EF97
	public void SetLocked(bool isLocked)
	{
		this.SetFlag(global::ItemContainer.Flag.IsLocked, isLocked);
		this.MarkDirty();
	}

	// Token: 0x06002D0D RID: 11533 RVA: 0x00110DA8 File Offset: 0x0010EFA8
	internal bool Remove(global::Item item)
	{
		if (!this.itemList.Contains(item))
		{
			return false;
		}
		if (this.onPreItemRemove != null)
		{
			this.onPreItemRemove(item);
		}
		this.itemList.Remove(item);
		item.parent = null;
		this.MarkDirty();
		if (this.onItemAddedRemoved != null)
		{
			this.onItemAddedRemoved(item, false);
		}
		return true;
	}

	// Token: 0x06002D0E RID: 11534 RVA: 0x00110E0C File Offset: 0x0010F00C
	internal void Clear()
	{
		global::Item[] array = this.itemList.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Remove(0f);
		}
	}

	// Token: 0x06002D0F RID: 11535 RVA: 0x00110E40 File Offset: 0x0010F040
	public void Kill()
	{
		this.onDirty = null;
		this.canAcceptItem = null;
		this.slotIsReserved = null;
		this.onItemAddedRemoved = null;
		if (Net.sv != null)
		{
			Net.sv.ReturnUID(this.uid.Value);
			this.uid = default(ItemContainerId);
		}
		List<global::Item> list = Pool.GetList<global::Item>();
		foreach (global::Item item in this.itemList)
		{
			list.Add(item);
		}
		foreach (global::Item item2 in list)
		{
			item2.Remove(0f);
		}
		Pool.FreeList<global::Item>(ref list);
		this.itemList.Clear();
	}

	// Token: 0x06002D10 RID: 11536 RVA: 0x00110F30 File Offset: 0x0010F130
	public int GetAmount(int itemid, bool onlyUsableAmounts)
	{
		int num = 0;
		foreach (global::Item item in this.itemList)
		{
			if (item.info.itemid == itemid && (!onlyUsableAmounts || !item.IsBusy()))
			{
				num += item.amount;
			}
		}
		return num;
	}

	// Token: 0x06002D11 RID: 11537 RVA: 0x00110FA4 File Offset: 0x0010F1A4
	public global::Item FindItemByItemID(int itemid)
	{
		foreach (global::Item item in this.itemList)
		{
			if (item.info.itemid == itemid)
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x06002D12 RID: 11538 RVA: 0x00111008 File Offset: 0x0010F208
	public global::Item FindItemsByItemName(string name)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(name);
		if (itemDefinition == null)
		{
			return null;
		}
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].info == itemDefinition)
			{
				return this.itemList[i];
			}
		}
		return null;
	}

	// Token: 0x06002D13 RID: 11539 RVA: 0x00111064 File Offset: 0x0010F264
	public global::Item FindBySubEntityID(NetworkableId subEntityID)
	{
		if (!subEntityID.IsValid)
		{
			return null;
		}
		foreach (global::Item item in this.itemList)
		{
			if (item.instanceData != null && item.instanceData.subEntity == subEntityID)
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x06002D14 RID: 11540 RVA: 0x001110E0 File Offset: 0x0010F2E0
	public List<global::Item> FindItemsByItemID(int itemid)
	{
		return this.itemList.FindAll((global::Item x) => x.info.itemid == itemid);
	}

	// Token: 0x06002D15 RID: 11541 RVA: 0x00111114 File Offset: 0x0010F314
	public ProtoBuf.ItemContainer Save()
	{
		ProtoBuf.ItemContainer itemContainer = Pool.Get<ProtoBuf.ItemContainer>();
		itemContainer.contents = Pool.GetList<ProtoBuf.Item>();
		itemContainer.UID = this.uid;
		itemContainer.slots = this.capacity;
		itemContainer.temperature = this.temperature;
		itemContainer.allowedContents = (int)this.allowedContents;
		if (this.HasLimitedAllowedItems)
		{
			itemContainer.allowedItems = Pool.GetList<int>();
			for (int i = 0; i < this.onlyAllowedItems.Length; i++)
			{
				if (this.onlyAllowedItems[i] != null)
				{
					itemContainer.allowedItems.Add(this.onlyAllowedItems[i].itemid);
				}
			}
		}
		itemContainer.flags = (int)this.flags;
		itemContainer.maxStackSize = this.maxStackSize;
		if (this.availableSlots != null && this.availableSlots.Count > 0)
		{
			itemContainer.availableSlots = Pool.GetList<int>();
			for (int j = 0; j < this.availableSlots.Count; j++)
			{
				itemContainer.availableSlots.Add((int)this.availableSlots[j]);
			}
		}
		for (int k = 0; k < this.itemList.Count; k++)
		{
			global::Item item = this.itemList[k];
			if (item.IsValid())
			{
				itemContainer.contents.Add(item.Save(true, true));
			}
		}
		return itemContainer;
	}

	// Token: 0x06002D16 RID: 11542 RVA: 0x00111258 File Offset: 0x0010F458
	public void Load(ProtoBuf.ItemContainer container)
	{
		using (TimeWarning.New("ItemContainer.Load", 0))
		{
			this.uid = container.UID;
			this.capacity = container.slots;
			List<global::Item> list = this.itemList;
			this.itemList = Pool.GetList<global::Item>();
			this.temperature = container.temperature;
			this.flags = (global::ItemContainer.Flag)container.flags;
			this.allowedContents = (global::ItemContainer.ContentsType)((container.allowedContents == 0) ? 1 : container.allowedContents);
			if (container.allowedItems != null && container.allowedItems.Count > 0)
			{
				this.onlyAllowedItems = new ItemDefinition[container.allowedItems.Count];
				for (int i = 0; i < container.allowedItems.Count; i++)
				{
					this.onlyAllowedItems[i] = ItemManager.FindItemDefinition(container.allowedItems[i]);
				}
			}
			else
			{
				this.onlyAllowedItems = null;
			}
			this.maxStackSize = container.maxStackSize;
			this.availableSlots.Clear();
			for (int j = 0; j < container.availableSlots.Count; j++)
			{
				this.availableSlots.Add((ItemSlot)container.availableSlots[j]);
			}
			using (TimeWarning.New("container.contents", 0))
			{
				foreach (ProtoBuf.Item item in container.contents)
				{
					global::Item item2 = null;
					foreach (global::Item item3 in list)
					{
						if (item3.uid == item.UID)
						{
							item2 = item3;
							break;
						}
					}
					item2 = ItemManager.Load(item, item2, this.isServer);
					if (item2 != null)
					{
						item2.parent = this;
						item2.position = item.slot;
						this.Insert(item2);
					}
				}
			}
			using (TimeWarning.New("Delete old items", 0))
			{
				foreach (global::Item item4 in list)
				{
					if (!this.itemList.Contains(item4))
					{
						item4.Remove(0f);
					}
				}
			}
			this.dirty = true;
			Pool.FreeList<global::Item>(ref list);
		}
	}

	// Token: 0x06002D17 RID: 11543 RVA: 0x00111550 File Offset: 0x0010F750
	public global::BasePlayer GetOwnerPlayer()
	{
		return this.playerOwner;
	}

	// Token: 0x06002D18 RID: 11544 RVA: 0x00111558 File Offset: 0x0010F758
	public int ContainerMaxStackSize()
	{
		if (this.maxStackSize <= 0)
		{
			return int.MaxValue;
		}
		return this.maxStackSize;
	}

	// Token: 0x06002D19 RID: 11545 RVA: 0x00111570 File Offset: 0x0010F770
	public int Take(List<global::Item> collect, int itemid, int iAmount)
	{
		int num = 0;
		if (iAmount == 0)
		{
			return num;
		}
		List<global::Item> list = Pool.GetList<global::Item>();
		foreach (global::Item item in this.itemList)
		{
			if (item.info.itemid == itemid)
			{
				int num2 = iAmount - num;
				if (num2 > 0)
				{
					if (item.amount > num2)
					{
						item.MarkDirty();
						item.amount -= num2;
						num += num2;
						global::Item item2 = ItemManager.CreateByItemID(itemid, 1, 0UL);
						item2.amount = num2;
						item2.CollectedForCrafting(this.playerOwner);
						if (collect != null)
						{
							collect.Add(item2);
							break;
						}
						break;
					}
					else
					{
						if (item.amount <= num2)
						{
							num += item.amount;
							list.Add(item);
							if (collect != null)
							{
								collect.Add(item);
							}
						}
						if (num == iAmount)
						{
							break;
						}
					}
				}
			}
		}
		foreach (global::Item item3 in list)
		{
			item3.RemoveFromContainer();
		}
		Pool.FreeList<global::Item>(ref list);
		return num;
	}

	// Token: 0x170003AE RID: 942
	// (get) Token: 0x06002D1A RID: 11546 RVA: 0x001116A8 File Offset: 0x0010F8A8
	public Vector3 dropPosition
	{
		get
		{
			if (this.playerOwner)
			{
				return this.playerOwner.GetDropPosition();
			}
			if (this.entityOwner)
			{
				return this.entityOwner.GetDropPosition();
			}
			if (this.parent != null)
			{
				global::BaseEntity worldEntity = this.parent.GetWorldEntity();
				if (worldEntity != null)
				{
					return worldEntity.GetDropPosition();
				}
			}
			Debug.LogWarning("ItemContainer.dropPosition dropped through");
			return Vector3.zero;
		}
	}

	// Token: 0x170003AF RID: 943
	// (get) Token: 0x06002D1B RID: 11547 RVA: 0x0011171C File Offset: 0x0010F91C
	public Vector3 dropVelocity
	{
		get
		{
			if (this.playerOwner)
			{
				return this.playerOwner.GetDropVelocity();
			}
			if (this.entityOwner)
			{
				return this.entityOwner.GetDropVelocity();
			}
			if (this.parent != null)
			{
				global::BaseEntity worldEntity = this.parent.GetWorldEntity();
				if (worldEntity != null)
				{
					return worldEntity.GetDropVelocity();
				}
			}
			Debug.LogWarning("ItemContainer.dropVelocity dropped through");
			return Vector3.zero;
		}
	}

	// Token: 0x06002D1C RID: 11548 RVA: 0x00111790 File Offset: 0x0010F990
	public void OnCycle(float delta)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].IsValid())
			{
				this.itemList[i].OnCycle(delta);
			}
		}
	}

	// Token: 0x06002D1D RID: 11549 RVA: 0x001117D8 File Offset: 0x0010F9D8
	public void FindAmmo(List<global::Item> list, AmmoTypes ammoType)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].FindAmmo(list, ammoType);
		}
	}

	// Token: 0x06002D1E RID: 11550 RVA: 0x00111810 File Offset: 0x0010FA10
	public bool HasAmmo(AmmoTypes ammoType)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].HasAmmo(ammoType))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002D1F RID: 11551 RVA: 0x0011184C File Offset: 0x0010FA4C
	public int GetAmmoAmount(AmmoTypes ammoType)
	{
		int num = 0;
		for (int i = 0; i < this.itemList.Count; i++)
		{
			num += this.itemList[i].GetAmmoAmount(ammoType);
		}
		return num;
	}

	// Token: 0x06002D20 RID: 11552 RVA: 0x00111888 File Offset: 0x0010FA88
	public int TotalItemAmount()
	{
		int num = 0;
		for (int i = 0; i < this.itemList.Count; i++)
		{
			num += this.itemList[i].amount;
		}
		return num;
	}

	// Token: 0x06002D21 RID: 11553 RVA: 0x001118C4 File Offset: 0x0010FAC4
	public int GetTotalItemAmount(global::Item item, int slotStartInclusive, int slotEndInclusive)
	{
		int num = 0;
		for (int i = slotStartInclusive; i <= slotEndInclusive; i++)
		{
			global::Item slot = this.GetSlot(i);
			if (slot != null)
			{
				if (item.IsBlueprint())
				{
					if (slot.IsBlueprint() && slot.blueprintTarget == item.blueprintTarget)
					{
						num += slot.amount;
					}
				}
				else if (slot.info == item.info || slot.info.isRedirectOf == item.info || item.info.isRedirectOf == slot.info)
				{
					num += slot.amount;
				}
			}
		}
		return num;
	}

	// Token: 0x06002D22 RID: 11554 RVA: 0x00111968 File Offset: 0x0010FB68
	public int GetTotalCategoryAmount(ItemCategory category, int slotStartInclusive, int slotEndInclusive)
	{
		int num = 0;
		for (int i = slotStartInclusive; i <= slotEndInclusive; i++)
		{
			global::Item slot = this.GetSlot(i);
			if (slot != null && slot.info.category == category)
			{
				num += slot.amount;
			}
		}
		return num;
	}

	// Token: 0x06002D23 RID: 11555 RVA: 0x001119A8 File Offset: 0x0010FBA8
	public void AddItem(ItemDefinition itemToCreate, int amount, ulong skin = 0UL, global::ItemContainer.LimitStack limitStack = global::ItemContainer.LimitStack.Existing)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (amount == 0)
			{
				return;
			}
			if (!(this.itemList[i].info != itemToCreate))
			{
				int num = this.itemList[i].MaxStackable();
				if (num > this.itemList[i].amount || limitStack == global::ItemContainer.LimitStack.None)
				{
					this.MarkDirty();
					this.itemList[i].amount += amount;
					amount -= amount;
					if (this.itemList[i].amount > num && limitStack != global::ItemContainer.LimitStack.None)
					{
						amount = this.itemList[i].amount - num;
						if (amount > 0)
						{
							this.itemList[i].amount -= amount;
						}
					}
				}
			}
		}
		if (amount == 0)
		{
			return;
		}
		int num2 = ((limitStack == global::ItemContainer.LimitStack.All) ? Mathf.Min(itemToCreate.stackable, this.ContainerMaxStackSize()) : int.MaxValue);
		if (num2 > 0)
		{
			while (amount > 0)
			{
				int num3 = Mathf.Min(amount, num2);
				global::Item item = ItemManager.Create(itemToCreate, num3, skin);
				amount -= num3;
				if (!item.MoveToContainer(this, -1, true, false, null, true))
				{
					item.Remove(0f);
				}
			}
		}
	}

	// Token: 0x06002D24 RID: 11556 RVA: 0x00111AE8 File Offset: 0x0010FCE8
	public void OnMovedToWorld()
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].OnMovedToWorld();
		}
	}

	// Token: 0x06002D25 RID: 11557 RVA: 0x00111B1C File Offset: 0x0010FD1C
	public void OnRemovedFromWorld()
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].OnRemovedFromWorld();
		}
	}

	// Token: 0x06002D26 RID: 11558 RVA: 0x00111B50 File Offset: 0x0010FD50
	public uint ContentsHash()
	{
		uint num = 0U;
		for (int i = 0; i < this.capacity; i++)
		{
			global::Item slot = this.GetSlot(i);
			if (slot != null)
			{
				num = CRC.Compute32(num, slot.info.itemid);
				num = CRC.Compute32(num, slot.skin);
			}
		}
		return num;
	}

	// Token: 0x06002D27 RID: 11559 RVA: 0x00111B9C File Offset: 0x0010FD9C
	internal global::ItemContainer FindContainer(ItemContainerId id)
	{
		if (id == this.uid)
		{
			return this;
		}
		for (int i = 0; i < this.itemList.Count; i++)
		{
			global::Item item = this.itemList[i];
			if (item.contents != null)
			{
				global::ItemContainer itemContainer = item.contents.FindContainer(id);
				if (itemContainer != null)
				{
					return itemContainer;
				}
			}
		}
		return null;
	}

	// Token: 0x06002D28 RID: 11560 RVA: 0x00111BF8 File Offset: 0x0010FDF8
	public global::ItemContainer.CanAcceptResult CanAcceptItem(global::Item item, int targetPos)
	{
		if (this.canAcceptItem != null && !this.canAcceptItem(item, targetPos))
		{
			return global::ItemContainer.CanAcceptResult.CannotAccept;
		}
		if (this.isServer && this.availableSlots != null && this.availableSlots.Count > 0)
		{
			if (item.info.occupySlots == (ItemSlot)0 || item.info.occupySlots == ItemSlot.None)
			{
				return global::ItemContainer.CanAcceptResult.CannotAccept;
			}
			if (item.isBroken)
			{
				return global::ItemContainer.CanAcceptResult.CannotAccept;
			}
			int num = 0;
			foreach (ItemSlot itemSlot in this.availableSlots)
			{
				num |= (int)itemSlot;
			}
			if ((num & (int)item.info.occupySlots) != (int)item.info.occupySlots)
			{
				return global::ItemContainer.CanAcceptResult.CannotAcceptRightNow;
			}
		}
		if ((this.allowedContents & item.info.itemType) != item.info.itemType)
		{
			return global::ItemContainer.CanAcceptResult.CannotAccept;
		}
		if (this.HasLimitedAllowedItems)
		{
			bool flag = false;
			for (int i = 0; i < this.onlyAllowedItems.Length; i++)
			{
				if (this.onlyAllowedItems[i] == item.info)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return global::ItemContainer.CanAcceptResult.CannotAccept;
			}
		}
		return global::ItemContainer.CanAcceptResult.CanAccept;
	}

	// Token: 0x0400249E RID: 9374
	public global::ItemContainer.Flag flags;

	// Token: 0x0400249F RID: 9375
	public global::ItemContainer.ContentsType allowedContents;

	// Token: 0x040024A0 RID: 9376
	public ItemDefinition[] onlyAllowedItems;

	// Token: 0x040024A1 RID: 9377
	public List<ItemSlot> availableSlots = new List<ItemSlot>();

	// Token: 0x040024A2 RID: 9378
	public int capacity = 2;

	// Token: 0x040024A3 RID: 9379
	public ItemContainerId uid;

	// Token: 0x040024A4 RID: 9380
	public bool dirty;

	// Token: 0x040024A5 RID: 9381
	public List<global::Item> itemList = new List<global::Item>();

	// Token: 0x040024A6 RID: 9382
	public float temperature = 15f;

	// Token: 0x040024A7 RID: 9383
	public global::Item parent;

	// Token: 0x040024A8 RID: 9384
	public global::BasePlayer playerOwner;

	// Token: 0x040024A9 RID: 9385
	public global::BaseEntity entityOwner;

	// Token: 0x040024AA RID: 9386
	public bool isServer;

	// Token: 0x040024AB RID: 9387
	public int maxStackSize;

	// Token: 0x040024AD RID: 9389
	public Func<global::Item, int, bool> canAcceptItem;

	// Token: 0x040024AE RID: 9390
	public Func<global::Item, int, bool> slotIsReserved;

	// Token: 0x040024AF RID: 9391
	public Action<global::Item, bool> onItemAddedRemoved;

	// Token: 0x040024B0 RID: 9392
	public Action<global::Item> onPreItemRemove;

	// Token: 0x02000D90 RID: 3472
	[Flags]
	public enum Flag
	{
		// Token: 0x0400486F RID: 18543
		IsPlayer = 1,
		// Token: 0x04004870 RID: 18544
		Clothing = 2,
		// Token: 0x04004871 RID: 18545
		Belt = 4,
		// Token: 0x04004872 RID: 18546
		SingleType = 8,
		// Token: 0x04004873 RID: 18547
		IsLocked = 16,
		// Token: 0x04004874 RID: 18548
		ShowSlotsOnIcon = 32,
		// Token: 0x04004875 RID: 18549
		NoBrokenItems = 64,
		// Token: 0x04004876 RID: 18550
		NoItemInput = 128,
		// Token: 0x04004877 RID: 18551
		ContentsHidden = 256
	}

	// Token: 0x02000D91 RID: 3473
	[Flags]
	public enum ContentsType
	{
		// Token: 0x04004879 RID: 18553
		Generic = 1,
		// Token: 0x0400487A RID: 18554
		Liquid = 2
	}

	// Token: 0x02000D92 RID: 3474
	public enum LimitStack
	{
		// Token: 0x0400487C RID: 18556
		None,
		// Token: 0x0400487D RID: 18557
		Existing,
		// Token: 0x0400487E RID: 18558
		All
	}

	// Token: 0x02000D93 RID: 3475
	public enum CanAcceptResult
	{
		// Token: 0x04004880 RID: 18560
		CanAccept,
		// Token: 0x04004881 RID: 18561
		CannotAccept,
		// Token: 0x04004882 RID: 18562
		CannotAcceptRightNow
	}
}
