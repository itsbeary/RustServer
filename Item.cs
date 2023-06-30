using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020005CE RID: 1486
public class Item
{
	// Token: 0x170003A0 RID: 928
	// (get) Token: 0x06002C9B RID: 11419 RVA: 0x0010E887 File Offset: 0x0010CA87
	// (set) Token: 0x06002C9A RID: 11418 RVA: 0x0010E840 File Offset: 0x0010CA40
	public float condition
	{
		get
		{
			return this._condition;
		}
		set
		{
			float condition = this._condition;
			this._condition = Mathf.Clamp(value, 0f, this.maxCondition);
			if (this.isServer && Mathf.Ceil(value) != Mathf.Ceil(condition))
			{
				this.MarkDirty();
			}
		}
	}

	// Token: 0x170003A1 RID: 929
	// (get) Token: 0x06002C9D RID: 11421 RVA: 0x0010E8C0 File Offset: 0x0010CAC0
	// (set) Token: 0x06002C9C RID: 11420 RVA: 0x0010E88F File Offset: 0x0010CA8F
	public float maxCondition
	{
		get
		{
			return this._maxCondition;
		}
		set
		{
			this._maxCondition = Mathf.Clamp(value, 0f, this.info.condition.max);
			if (this.isServer)
			{
				this.MarkDirty();
			}
		}
	}

	// Token: 0x170003A2 RID: 930
	// (get) Token: 0x06002C9E RID: 11422 RVA: 0x0010E8C8 File Offset: 0x0010CAC8
	public float maxConditionNormalized
	{
		get
		{
			return this._maxCondition / this.info.condition.max;
		}
	}

	// Token: 0x170003A3 RID: 931
	// (get) Token: 0x06002C9F RID: 11423 RVA: 0x0010E8E1 File Offset: 0x0010CAE1
	// (set) Token: 0x06002CA0 RID: 11424 RVA: 0x0010E8FE File Offset: 0x0010CAFE
	public float conditionNormalized
	{
		get
		{
			if (!this.hasCondition)
			{
				return 1f;
			}
			return this.condition / this.maxCondition;
		}
		set
		{
			if (!this.hasCondition)
			{
				return;
			}
			this.condition = value * this.maxCondition;
		}
	}

	// Token: 0x170003A4 RID: 932
	// (get) Token: 0x06002CA1 RID: 11425 RVA: 0x0010E917 File Offset: 0x0010CB17
	public bool hasCondition
	{
		get
		{
			return this.info != null && this.info.condition.enabled && this.info.condition.max > 0f;
		}
	}

	// Token: 0x170003A5 RID: 933
	// (get) Token: 0x06002CA2 RID: 11426 RVA: 0x0010E952 File Offset: 0x0010CB52
	public bool isBroken
	{
		get
		{
			return this.hasCondition && this.condition <= 0f;
		}
	}

	// Token: 0x06002CA3 RID: 11427 RVA: 0x0010E970 File Offset: 0x0010CB70
	public void LoseCondition(float amount)
	{
		if (!this.hasCondition)
		{
			return;
		}
		if (Debugging.disablecondition)
		{
			return;
		}
		float condition = this.condition;
		this.condition -= amount;
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				this.info.shortname,
				" was damaged by: ",
				amount,
				"cond is: ",
				this.condition,
				"/",
				this.maxCondition
			}));
		}
		if (this.condition <= 0f && this.condition < condition)
		{
			this.OnBroken();
		}
	}

	// Token: 0x06002CA4 RID: 11428 RVA: 0x0010EA22 File Offset: 0x0010CC22
	public void RepairCondition(float amount)
	{
		if (!this.hasCondition)
		{
			return;
		}
		this.condition += amount;
	}

	// Token: 0x06002CA5 RID: 11429 RVA: 0x0010EA3C File Offset: 0x0010CC3C
	public void DoRepair(float maxLossFraction)
	{
		if (!this.hasCondition)
		{
			return;
		}
		if (this.info.condition.maintainMaxCondition)
		{
			maxLossFraction = 0f;
		}
		float num = 1f - this.condition / this.maxCondition;
		maxLossFraction = Mathf.Clamp(maxLossFraction, 0f, this.info.condition.max);
		this.maxCondition *= 1f - maxLossFraction * num;
		this.condition = this.maxCondition;
		global::BaseEntity baseEntity = this.GetHeldEntity();
		if (baseEntity != null)
		{
			baseEntity.SetFlag(global::BaseEntity.Flags.Broken, false, false, true);
		}
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				this.info.shortname,
				" was repaired! new cond is: ",
				this.condition,
				"/",
				this.maxCondition
			}));
		}
	}

	// Token: 0x06002CA6 RID: 11430 RVA: 0x0010EB30 File Offset: 0x0010CD30
	public global::ItemContainer GetRootContainer()
	{
		global::ItemContainer itemContainer = this.parent;
		int num = 0;
		while (itemContainer != null && num <= 8 && itemContainer.parent != null && itemContainer.parent.parent != null)
		{
			itemContainer = itemContainer.parent.parent;
			num++;
		}
		if (num == 8)
		{
			Debug.LogWarning("GetRootContainer failed with 8 iterations");
		}
		return itemContainer;
	}

	// Token: 0x06002CA7 RID: 11431 RVA: 0x0010EB84 File Offset: 0x0010CD84
	public virtual void OnBroken()
	{
		if (!this.hasCondition)
		{
			return;
		}
		global::BaseEntity baseEntity = this.GetHeldEntity();
		if (baseEntity != null)
		{
			baseEntity.SetFlag(global::BaseEntity.Flags.Broken, true, false, true);
		}
		global::BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer)
		{
			if (ownerPlayer.GetActiveItem() == this)
			{
				Effect.server.Run("assets/bundled/prefabs/fx/item_break.prefab", ownerPlayer, 0U, Vector3.zero, Vector3.zero, null, false);
				ownerPlayer.ChatMessage("Your active item was broken!");
			}
			ItemModWearable itemModWearable;
			if (this.info.TryGetComponent<ItemModWearable>(out itemModWearable) && ownerPlayer.inventory.containerWear.itemList.Contains(this))
			{
				if (itemModWearable.breakEffect.isValid)
				{
					Effect.server.Run(itemModWearable.breakEffect.resourcePath, ownerPlayer, 0U, Vector3.zero, Vector3.zero, null, false);
				}
				else
				{
					Effect.server.Run("assets/bundled/prefabs/fx/armor_break.prefab", ownerPlayer, 0U, Vector3.zero, Vector3.zero, null, false);
				}
			}
		}
		if ((!this.info.condition.repairable && !this.info.GetComponent<ItemModRepair>()) || this.maxCondition <= 5f)
		{
			this.Remove(0f);
		}
		else if (this.parent != null && this.parent.HasFlag(global::ItemContainer.Flag.NoBrokenItems))
		{
			global::ItemContainer rootContainer = this.GetRootContainer();
			if (rootContainer.HasFlag(global::ItemContainer.Flag.NoBrokenItems))
			{
				this.Remove(0f);
			}
			else
			{
				global::BasePlayer playerOwner = rootContainer.playerOwner;
				if (playerOwner != null && !this.MoveToContainer(playerOwner.inventory.containerMain, -1, true, false, null, true))
				{
					this.Drop(playerOwner.transform.position, playerOwner.eyes.BodyForward() * 1.5f, default(Quaternion));
				}
			}
		}
		this.MarkDirty();
	}

	// Token: 0x06002CA8 RID: 11432 RVA: 0x0010ED3F File Offset: 0x0010CF3F
	public string GetName(bool? streamerModeOverride = null)
	{
		if (streamerModeOverride == null)
		{
			return this.name;
		}
		if (!streamerModeOverride.Value)
		{
			return this.name;
		}
		return this.streamerName ?? this.name;
	}

	// Token: 0x170003A6 RID: 934
	// (get) Token: 0x06002CA9 RID: 11433 RVA: 0x0010ED74 File Offset: 0x0010CF74
	public int despawnMultiplier
	{
		get
		{
			Rarity rarity = this.info.despawnRarity;
			if (rarity == Rarity.None)
			{
				rarity = this.info.rarity;
			}
			if (!(this.info != null))
			{
				return 1;
			}
			return Mathf.Clamp((rarity - Rarity.Common) * 4, 1, 100);
		}
	}

	// Token: 0x170003A7 RID: 935
	// (get) Token: 0x06002CAA RID: 11434 RVA: 0x0010EDB9 File Offset: 0x0010CFB9
	public ItemDefinition blueprintTargetDef
	{
		get
		{
			if (!this.IsBlueprint())
			{
				return null;
			}
			return ItemManager.FindItemDefinition(this.blueprintTarget);
		}
	}

	// Token: 0x170003A8 RID: 936
	// (get) Token: 0x06002CAB RID: 11435 RVA: 0x0010EDD0 File Offset: 0x0010CFD0
	// (set) Token: 0x06002CAC RID: 11436 RVA: 0x0010EDE7 File Offset: 0x0010CFE7
	public int blueprintTarget
	{
		get
		{
			if (this.instanceData == null)
			{
				return 0;
			}
			return this.instanceData.blueprintTarget;
		}
		set
		{
			if (this.instanceData == null)
			{
				this.instanceData = new ProtoBuf.Item.InstanceData();
			}
			this.instanceData.ShouldPool = false;
			this.instanceData.blueprintTarget = value;
		}
	}

	// Token: 0x170003A9 RID: 937
	// (get) Token: 0x06002CAD RID: 11437 RVA: 0x0010EE14 File Offset: 0x0010D014
	// (set) Token: 0x06002CAE RID: 11438 RVA: 0x0010EE1C File Offset: 0x0010D01C
	public int blueprintAmount
	{
		get
		{
			return this.amount;
		}
		set
		{
			this.amount = value;
		}
	}

	// Token: 0x06002CAF RID: 11439 RVA: 0x0010EE25 File Offset: 0x0010D025
	public bool IsBlueprint()
	{
		return this.blueprintTarget != 0;
	}

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x06002CB0 RID: 11440 RVA: 0x0010EE30 File Offset: 0x0010D030
	// (remove) Token: 0x06002CB1 RID: 11441 RVA: 0x0010EE68 File Offset: 0x0010D068
	public event Action<global::Item> OnDirty;

	// Token: 0x06002CB2 RID: 11442 RVA: 0x0010EE9D File Offset: 0x0010D09D
	public bool HasFlag(global::Item.Flag f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x06002CB3 RID: 11443 RVA: 0x0010EEAA File Offset: 0x0010D0AA
	public void SetFlag(global::Item.Flag f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	// Token: 0x06002CB4 RID: 11444 RVA: 0x0010EECD File Offset: 0x0010D0CD
	public bool IsOn()
	{
		return this.HasFlag(global::Item.Flag.IsOn);
	}

	// Token: 0x06002CB5 RID: 11445 RVA: 0x0010EED6 File Offset: 0x0010D0D6
	public bool IsOnFire()
	{
		return this.HasFlag(global::Item.Flag.OnFire);
	}

	// Token: 0x06002CB6 RID: 11446 RVA: 0x0010EEDF File Offset: 0x0010D0DF
	public bool IsCooking()
	{
		return this.HasFlag(global::Item.Flag.Cooking);
	}

	// Token: 0x06002CB7 RID: 11447 RVA: 0x0010EEE9 File Offset: 0x0010D0E9
	public bool IsLocked()
	{
		return this.HasFlag(global::Item.Flag.IsLocked) || (this.parent != null && this.parent.IsLocked());
	}

	// Token: 0x170003AA RID: 938
	// (get) Token: 0x06002CB8 RID: 11448 RVA: 0x0010EF0B File Offset: 0x0010D10B
	public global::Item parentItem
	{
		get
		{
			if (this.parent == null)
			{
				return null;
			}
			return this.parent.parent;
		}
	}

	// Token: 0x06002CB9 RID: 11449 RVA: 0x0010EF22 File Offset: 0x0010D122
	public void MarkDirty()
	{
		this.OnChanged();
		this.dirty = true;
		if (this.parent != null)
		{
			this.parent.MarkDirty();
		}
		if (this.OnDirty != null)
		{
			this.OnDirty(this);
		}
	}

	// Token: 0x06002CBA RID: 11450 RVA: 0x0010EF58 File Offset: 0x0010D158
	public void OnChanged()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnChanged(this);
		}
		if (this.contents != null)
		{
			this.contents.OnChanged();
		}
	}

	// Token: 0x06002CBB RID: 11451 RVA: 0x0010EF9C File Offset: 0x0010D19C
	public void CollectedForCrafting(global::BasePlayer crafter)
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].CollectedForCrafting(this, crafter);
		}
	}

	// Token: 0x06002CBC RID: 11452 RVA: 0x0010EFD0 File Offset: 0x0010D1D0
	public void ReturnedFromCancelledCraft(global::BasePlayer crafter)
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].ReturnedFromCancelledCraft(this, crafter);
		}
	}

	// Token: 0x06002CBD RID: 11453 RVA: 0x0010F004 File Offset: 0x0010D204
	public void Initialize(ItemDefinition template)
	{
		this.uid = new ItemId(Network.Net.sv.TakeUID());
		this.condition = (this.maxCondition = this.info.condition.max);
		this.OnItemCreated();
	}

	// Token: 0x06002CBE RID: 11454 RVA: 0x0010F04C File Offset: 0x0010D24C
	public void OnItemCreated()
	{
		this.onCycle = null;
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnItemCreated(this);
		}
	}

	// Token: 0x06002CBF RID: 11455 RVA: 0x0010F084 File Offset: 0x0010D284
	public void OnVirginSpawn()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnVirginItem(this);
		}
	}

	// Token: 0x06002CC0 RID: 11456 RVA: 0x0010F0B4 File Offset: 0x0010D2B4
	public float GetDespawnDuration()
	{
		if (this.info.quickDespawn)
		{
			return ConVar.Server.itemdespawn_quick;
		}
		return ConVar.Server.itemdespawn * (float)this.despawnMultiplier;
	}

	// Token: 0x06002CC1 RID: 11457 RVA: 0x0010F0D8 File Offset: 0x0010D2D8
	protected void RemoveFromWorld()
	{
		global::BaseEntity worldEntity = this.GetWorldEntity();
		if (worldEntity == null)
		{
			return;
		}
		this.SetWorldEntity(null);
		this.OnRemovedFromWorld();
		if (this.contents != null)
		{
			this.contents.OnRemovedFromWorld();
		}
		if (!worldEntity.IsValid())
		{
			return;
		}
		global::WorldItem worldItem;
		if ((worldItem = worldEntity as global::WorldItem) != null)
		{
			worldItem.RemoveItem();
		}
		worldEntity.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002CC2 RID: 11458 RVA: 0x0010F138 File Offset: 0x0010D338
	public void OnRemovedFromWorld()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnRemovedFromWorld(this);
		}
	}

	// Token: 0x06002CC3 RID: 11459 RVA: 0x0010F168 File Offset: 0x0010D368
	public void RemoveFromContainer()
	{
		if (this.parent == null)
		{
			return;
		}
		this.SetParent(null);
	}

	// Token: 0x06002CC4 RID: 11460 RVA: 0x0010F17A File Offset: 0x0010D37A
	public bool DoItemSlotsConflict(global::Item other)
	{
		return (this.info.occupySlots & other.info.occupySlots) > (ItemSlot)0;
	}

	// Token: 0x06002CC5 RID: 11461 RVA: 0x0010F198 File Offset: 0x0010D398
	public void SetParent(global::ItemContainer target)
	{
		if (target == this.parent)
		{
			return;
		}
		if (this.parent != null)
		{
			this.parent.Remove(this);
			this.parent = null;
		}
		if (target == null)
		{
			this.position = 0;
		}
		else
		{
			this.parent = target;
			if (!this.parent.Insert(this))
			{
				this.Remove(0f);
				Debug.LogError("Item.SetParent caused remove - this shouldn't ever happen");
			}
		}
		this.MarkDirty();
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnParentChanged(this);
		}
	}

	// Token: 0x06002CC6 RID: 11462 RVA: 0x0010F22C File Offset: 0x0010D42C
	public void OnAttacked(HitInfo hitInfo)
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnAttacked(this, hitInfo);
		}
	}

	// Token: 0x06002CC7 RID: 11463 RVA: 0x0010F25D File Offset: 0x0010D45D
	public global::BaseEntity GetEntityOwner()
	{
		global::ItemContainer itemContainer = this.parent;
		if (itemContainer == null)
		{
			return null;
		}
		return itemContainer.GetEntityOwner(false);
	}

	// Token: 0x06002CC8 RID: 11464 RVA: 0x0010F274 File Offset: 0x0010D474
	public bool IsChildContainer(global::ItemContainer c)
	{
		if (this.contents == null)
		{
			return false;
		}
		if (this.contents == c)
		{
			return true;
		}
		using (List<global::Item>.Enumerator enumerator = this.contents.itemList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsChildContainer(c))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002CC9 RID: 11465 RVA: 0x0010F2E8 File Offset: 0x0010D4E8
	public bool CanMoveTo(global::ItemContainer newcontainer, int iTargetPos = -1)
	{
		return !this.IsChildContainer(newcontainer) && newcontainer.CanAcceptItem(this, iTargetPos) == global::ItemContainer.CanAcceptResult.CanAccept && iTargetPos < newcontainer.capacity && (this.parent == null || newcontainer != this.parent || iTargetPos != this.position);
	}

	// Token: 0x06002CCA RID: 11466 RVA: 0x0010F334 File Offset: 0x0010D534
	public bool MoveToContainer(global::ItemContainer newcontainer, int iTargetPos = -1, bool allowStack = true, bool ignoreStackLimit = false, global::BasePlayer sourcePlayer = null, bool allowSwap = true)
	{
		bool flag3;
		using (TimeWarning.New("MoveToContainer", 0))
		{
			bool flag = iTargetPos == -1;
			global::ItemContainer itemContainer = this.parent;
			if (iTargetPos == -1)
			{
				if (allowStack && this.info.stackable > 1)
				{
					foreach (global::Item item in from x in newcontainer.FindItemsByItemID(this.info.itemid)
						orderby x.position
						select x)
					{
						if (item.CanStack(this) && (ignoreStackLimit || item.amount < item.MaxStackable()))
						{
							iTargetPos = item.position;
						}
					}
				}
				if (iTargetPos == -1)
				{
					IItemContainerEntity itemContainerEntity = newcontainer.GetEntityOwner(true) as IItemContainerEntity;
					if (itemContainerEntity != null)
					{
						iTargetPos = itemContainerEntity.GetIdealSlot(sourcePlayer, this);
						if (iTargetPos == -2147483648)
						{
							return false;
						}
					}
				}
				if (iTargetPos == -1)
				{
					if (newcontainer == this.parent)
					{
						return false;
					}
					bool flag2 = newcontainer.HasFlag(global::ItemContainer.Flag.Clothing) && this.info.isWearable;
					ItemModWearable itemModWearable = this.info.ItemModWearable;
					for (int i = 0; i < newcontainer.capacity; i++)
					{
						global::Item slot = newcontainer.GetSlot(i);
						if (slot == null)
						{
							if (this.CanMoveTo(newcontainer, i))
							{
								iTargetPos = i;
								break;
							}
						}
						else
						{
							if (flag2 && slot != null && !slot.info.ItemModWearable.CanExistWith(itemModWearable))
							{
								iTargetPos = i;
								break;
							}
							if (newcontainer.availableSlots != null && newcontainer.availableSlots.Count > 0 && this.DoItemSlotsConflict(slot))
							{
								iTargetPos = i;
								break;
							}
						}
					}
					if (flag2 && iTargetPos == -1)
					{
						iTargetPos = newcontainer.capacity - 1;
					}
				}
			}
			if (iTargetPos == -1)
			{
				flag3 = false;
			}
			else if (!this.CanMoveTo(newcontainer, iTargetPos))
			{
				flag3 = false;
			}
			else if (iTargetPos >= 0 && newcontainer.SlotTaken(this, iTargetPos))
			{
				global::Item slot2 = newcontainer.GetSlot(iTargetPos);
				if (slot2 == this)
				{
					flag3 = false;
				}
				else
				{
					if (allowStack && slot2 != null)
					{
						int num = slot2.MaxStackable();
						if (slot2.CanStack(this))
						{
							if (ignoreStackLimit)
							{
								num = int.MaxValue;
							}
							if (slot2.amount >= num)
							{
								return false;
							}
							int num2 = Mathf.Min(num - slot2.amount, this.amount);
							slot2.amount += num2;
							this.amount -= num2;
							slot2.MarkDirty();
							this.MarkDirty();
							if (this.amount <= 0)
							{
								this.RemoveFromWorld();
								this.RemoveFromContainer();
								this.Remove(0f);
								return true;
							}
							if (flag)
							{
								return this.MoveToContainer(newcontainer, -1, allowStack, ignoreStackLimit, sourcePlayer, true);
							}
							return false;
						}
					}
					if (this.parent != null && allowSwap && slot2 != null)
					{
						global::ItemContainer itemContainer2 = this.parent;
						int num3 = this.position;
						global::ItemContainer itemContainer3 = slot2.parent;
						int num4 = slot2.position;
						if (!slot2.CanMoveTo(itemContainer2, num3))
						{
							flag3 = false;
						}
						else
						{
							global::BaseEntity entityOwner = this.GetEntityOwner();
							global::BaseEntity entityOwner2 = slot2.GetEntityOwner();
							this.RemoveFromContainer();
							slot2.RemoveFromContainer();
							this.RemoveConflictingSlots(newcontainer, entityOwner, sourcePlayer);
							slot2.RemoveConflictingSlots(itemContainer2, entityOwner2, sourcePlayer);
							if (!slot2.MoveToContainer(itemContainer2, num3, true, false, sourcePlayer, true) || !this.MoveToContainer(newcontainer, iTargetPos, true, false, sourcePlayer, true))
							{
								this.RemoveFromContainer();
								slot2.RemoveFromContainer();
								this.SetParent(itemContainer2);
								this.position = num3;
								slot2.SetParent(itemContainer3);
								slot2.position = num4;
								flag3 = true;
							}
							else
							{
								flag3 = true;
							}
						}
					}
					else
					{
						flag3 = false;
					}
				}
			}
			else if (this.parent == newcontainer)
			{
				if (iTargetPos >= 0 && iTargetPos != this.position && !this.parent.SlotTaken(this, iTargetPos))
				{
					this.position = iTargetPos;
					this.MarkDirty();
					flag3 = true;
				}
				else
				{
					flag3 = false;
				}
			}
			else if (newcontainer.maxStackSize > 0 && newcontainer.maxStackSize < this.amount)
			{
				global::Item item2 = this.SplitItem(newcontainer.maxStackSize);
				if (item2 != null && !item2.MoveToContainer(newcontainer, iTargetPos, false, false, sourcePlayer, true) && (itemContainer == null || !item2.MoveToContainer(itemContainer, -1, true, false, sourcePlayer, true)))
				{
					item2.Drop(newcontainer.dropPosition, newcontainer.dropVelocity, default(Quaternion));
				}
				flag3 = true;
			}
			else if (!newcontainer.CanAccept(this))
			{
				flag3 = false;
			}
			else
			{
				global::BaseEntity entityOwner3 = this.GetEntityOwner();
				this.RemoveFromContainer();
				this.RemoveFromWorld();
				this.RemoveConflictingSlots(newcontainer, entityOwner3, sourcePlayer);
				this.position = iTargetPos;
				this.SetParent(newcontainer);
				flag3 = true;
			}
		}
		return flag3;
	}

	// Token: 0x06002CCB RID: 11467 RVA: 0x0010F828 File Offset: 0x0010DA28
	private void RemoveConflictingSlots(global::ItemContainer container, global::BaseEntity entityOwner, global::BasePlayer sourcePlayer)
	{
		if (this.isServer && container.availableSlots != null && container.availableSlots.Count > 0)
		{
			List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
			list.AddRange(container.itemList);
			foreach (global::Item item in list)
			{
				if (item.DoItemSlotsConflict(this))
				{
					item.RemoveFromContainer();
					global::BasePlayer basePlayer;
					IItemContainerEntity itemContainerEntity;
					if ((basePlayer = entityOwner as global::BasePlayer) != null)
					{
						basePlayer.GiveItem(item, global::BaseEntity.GiveItemReason.Generic);
					}
					else if ((itemContainerEntity = entityOwner as IItemContainerEntity) != null)
					{
						item.MoveToContainer(itemContainerEntity.inventory, -1, true, false, sourcePlayer, true);
					}
				}
			}
			Facepunch.Pool.FreeList<global::Item>(ref list);
		}
	}

	// Token: 0x06002CCC RID: 11468 RVA: 0x0010F8F0 File Offset: 0x0010DAF0
	public global::BaseEntity CreateWorldObject(Vector3 pos, Quaternion rotation = default(Quaternion), global::BaseEntity parentEnt = null, uint parentBone = 0U)
	{
		global::BaseEntity baseEntity = this.GetWorldEntity();
		if (baseEntity != null)
		{
			return baseEntity;
		}
		baseEntity = GameManager.server.CreateEntity("assets/prefabs/misc/burlap sack/generic_world.prefab", pos, rotation, true);
		if (baseEntity == null)
		{
			Debug.LogWarning("Couldn't create world object for prefab: items/generic_world");
			return null;
		}
		global::WorldItem worldItem = baseEntity as global::WorldItem;
		if (worldItem != null)
		{
			worldItem.InitializeItem(this);
		}
		if (parentEnt != null)
		{
			baseEntity.SetParent(parentEnt, parentBone, false, false);
		}
		baseEntity.Spawn();
		this.SetWorldEntity(baseEntity);
		return this.GetWorldEntity();
	}

	// Token: 0x06002CCD RID: 11469 RVA: 0x0010F978 File Offset: 0x0010DB78
	public global::BaseEntity Drop(Vector3 vPos, Vector3 vVelocity, Quaternion rotation = default(Quaternion))
	{
		this.RemoveFromWorld();
		global::BaseEntity baseEntity = null;
		if (vPos != Vector3.zero && !this.info.HasFlag(ItemDefinition.Flag.NoDropping))
		{
			baseEntity = this.CreateWorldObject(vPos, rotation, null, 0U);
			if (baseEntity)
			{
				baseEntity.SetVelocity(vVelocity);
			}
		}
		else
		{
			this.Remove(0f);
		}
		this.RemoveFromContainer();
		return baseEntity;
	}

	// Token: 0x06002CCE RID: 11470 RVA: 0x0010F9D8 File Offset: 0x0010DBD8
	public global::BaseEntity DropAndTossUpwards(Vector3 vPos, float force = 2f)
	{
		float num = UnityEngine.Random.value * 3.1415927f * 2f;
		Vector3 vector = new Vector3(Mathf.Sin(num), 1f, Mathf.Cos(num));
		return this.Drop(vPos + Vector3.up * 0.1f, vector * force, default(Quaternion));
	}

	// Token: 0x06002CCF RID: 11471 RVA: 0x0010FA3A File Offset: 0x0010DC3A
	public bool IsBusy()
	{
		return this.busyTime > UnityEngine.Time.time;
	}

	// Token: 0x06002CD0 RID: 11472 RVA: 0x0010FA4C File Offset: 0x0010DC4C
	public void BusyFor(float fTime)
	{
		this.busyTime = UnityEngine.Time.time + fTime;
	}

	// Token: 0x06002CD1 RID: 11473 RVA: 0x0010FA5C File Offset: 0x0010DC5C
	public void Remove(float fTime = 0f)
	{
		if (this.removeTime > 0f)
		{
			return;
		}
		if (this.isServer)
		{
			ItemMod[] itemMods = this.info.itemMods;
			for (int i = 0; i < itemMods.Length; i++)
			{
				itemMods[i].OnRemove(this);
			}
		}
		this.onCycle = null;
		this.removeTime = UnityEngine.Time.time + fTime;
		this.OnDirty = null;
		this.position = -1;
		if (this.isServer)
		{
			ItemManager.RemoveItem(this, fTime);
		}
	}

	// Token: 0x06002CD2 RID: 11474 RVA: 0x0010FAD4 File Offset: 0x0010DCD4
	internal void DoRemove()
	{
		this.OnDirty = null;
		this.onCycle = null;
		if (this.isServer && this.uid.IsValid && Network.Net.sv != null)
		{
			Network.Net.sv.ReturnUID(this.uid.Value);
			this.uid = default(ItemId);
		}
		if (this.contents != null)
		{
			this.contents.Kill();
			this.contents = null;
		}
		if (this.isServer)
		{
			this.RemoveFromWorld();
			this.RemoveFromContainer();
		}
		global::BaseEntity baseEntity = this.GetHeldEntity();
		if (baseEntity.IsValid())
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Item's Held Entity not removed!",
				this.info.displayName.english,
				" -> ",
				baseEntity
			}), baseEntity);
		}
	}

	// Token: 0x06002CD3 RID: 11475 RVA: 0x0010FBA1 File Offset: 0x0010DDA1
	public void SwitchOnOff(bool bNewState)
	{
		if (this.HasFlag(global::Item.Flag.IsOn) == bNewState)
		{
			return;
		}
		this.SetFlag(global::Item.Flag.IsOn, bNewState);
		this.MarkDirty();
	}

	// Token: 0x06002CD4 RID: 11476 RVA: 0x0010FBBC File Offset: 0x0010DDBC
	public void LockUnlock(bool bNewState)
	{
		if (this.HasFlag(global::Item.Flag.IsLocked) == bNewState)
		{
			return;
		}
		this.SetFlag(global::Item.Flag.IsLocked, bNewState);
		this.MarkDirty();
	}

	// Token: 0x170003AB RID: 939
	// (get) Token: 0x06002CD5 RID: 11477 RVA: 0x0010FBD7 File Offset: 0x0010DDD7
	public float temperature
	{
		get
		{
			if (this.parent != null)
			{
				return this.parent.GetTemperature(this.position);
			}
			return 15f;
		}
	}

	// Token: 0x06002CD6 RID: 11478 RVA: 0x0010FBF8 File Offset: 0x0010DDF8
	public global::BasePlayer GetOwnerPlayer()
	{
		if (this.parent == null)
		{
			return null;
		}
		return this.parent.GetOwnerPlayer();
	}

	// Token: 0x06002CD7 RID: 11479 RVA: 0x0010FC10 File Offset: 0x0010DE10
	public global::Item SplitItem(int split_Amount)
	{
		Assert.IsTrue(split_Amount > 0, "split_Amount <= 0");
		if (split_Amount <= 0)
		{
			return null;
		}
		if (split_Amount >= this.amount)
		{
			return null;
		}
		this.amount -= split_Amount;
		global::Item item = ItemManager.CreateByItemID(this.info.itemid, 1, 0UL);
		item.amount = split_Amount;
		item.skin = this.skin;
		if (this.IsBlueprint())
		{
			item.blueprintTarget = this.blueprintTarget;
		}
		if (this.info.amountType == ItemDefinition.AmountType.Genetics && this.instanceData != null && this.instanceData.dataInt != 0)
		{
			item.instanceData = new ProtoBuf.Item.InstanceData();
			item.instanceData.dataInt = this.instanceData.dataInt;
			item.instanceData.ShouldPool = false;
		}
		if (this.instanceData != null && this.instanceData.dataInt > 0 && this.info != null && this.info.Blueprint != null && this.info.Blueprint.workbenchLevelRequired == 3)
		{
			item.instanceData = new ProtoBuf.Item.InstanceData();
			item.instanceData.dataInt = this.instanceData.dataInt;
			item.instanceData.ShouldPool = false;
			item.SetFlag(global::Item.Flag.IsOn, this.IsOn());
		}
		this.MarkDirty();
		return item;
	}

	// Token: 0x06002CD8 RID: 11480 RVA: 0x0010FD60 File Offset: 0x0010DF60
	public bool CanBeHeld()
	{
		return !this.isBroken;
	}

	// Token: 0x06002CD9 RID: 11481 RVA: 0x0010FD70 File Offset: 0x0010DF70
	public bool CanStack(global::Item item)
	{
		if (item == this)
		{
			return false;
		}
		if (this.MaxStackable() <= 1)
		{
			return false;
		}
		if (item.MaxStackable() <= 1)
		{
			return false;
		}
		if (item.info.itemid != this.info.itemid)
		{
			return false;
		}
		if (this.hasCondition && this.condition != item.info.condition.max)
		{
			return false;
		}
		if (item.hasCondition && item.condition != item.info.condition.max)
		{
			return false;
		}
		if (!this.IsValid())
		{
			return false;
		}
		if (this.IsBlueprint() && this.blueprintTarget != item.blueprintTarget)
		{
			return false;
		}
		if (item.skin != this.skin)
		{
			return false;
		}
		if (item.info.amountType == ItemDefinition.AmountType.Genetics || this.info.amountType == ItemDefinition.AmountType.Genetics)
		{
			int num = ((item.instanceData != null) ? item.instanceData.dataInt : (-1));
			int num2 = ((this.instanceData != null) ? this.instanceData.dataInt : (-1));
			if (num != num2)
			{
				return false;
			}
		}
		return (item.instanceData == null || this.instanceData == null || (item.IsOn() == this.IsOn() && (item.instanceData.dataInt == this.instanceData.dataInt || !(item.info.Blueprint != null) || item.info.Blueprint.workbenchLevelRequired != 3))) && (this.instanceData == null || !this.instanceData.subEntity.IsValid || !this.info.GetComponent<ItemModSign>()) && (item.instanceData == null || !item.instanceData.subEntity.IsValid || !item.info.GetComponent<ItemModSign>());
	}

	// Token: 0x06002CDA RID: 11482 RVA: 0x0010FF32 File Offset: 0x0010E132
	public bool IsValid()
	{
		return this.removeTime <= 0f;
	}

	// Token: 0x06002CDB RID: 11483 RVA: 0x0010FF44 File Offset: 0x0010E144
	public void SetWorldEntity(global::BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			this.worldEnt.Set(null);
			this.MarkDirty();
			return;
		}
		if (this.worldEnt.uid == ent.net.ID)
		{
			return;
		}
		this.worldEnt.Set(ent);
		this.MarkDirty();
		this.OnMovedToWorld();
		if (this.contents != null)
		{
			this.contents.OnMovedToWorld();
		}
	}

	// Token: 0x06002CDC RID: 11484 RVA: 0x0010FFB8 File Offset: 0x0010E1B8
	public void OnMovedToWorld()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnMovedToWorld(this);
		}
	}

	// Token: 0x06002CDD RID: 11485 RVA: 0x0010FFE8 File Offset: 0x0010E1E8
	public global::BaseEntity GetWorldEntity()
	{
		return this.worldEnt.Get(this.isServer);
	}

	// Token: 0x06002CDE RID: 11486 RVA: 0x0010FFFC File Offset: 0x0010E1FC
	public void SetHeldEntity(global::BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			this.heldEntity.Set(null);
			this.MarkDirty();
			return;
		}
		if (this.heldEntity.uid == ent.net.ID)
		{
			return;
		}
		this.heldEntity.Set(ent);
		this.MarkDirty();
		if (ent.IsValid())
		{
			global::HeldEntity heldEntity = ent as global::HeldEntity;
			if (heldEntity != null)
			{
				heldEntity.SetupHeldEntity(this);
			}
		}
	}

	// Token: 0x06002CDF RID: 11487 RVA: 0x00110073 File Offset: 0x0010E273
	public global::BaseEntity GetHeldEntity()
	{
		return this.heldEntity.Get(this.isServer);
	}

	// Token: 0x14000003 RID: 3
	// (add) Token: 0x06002CE0 RID: 11488 RVA: 0x00110088 File Offset: 0x0010E288
	// (remove) Token: 0x06002CE1 RID: 11489 RVA: 0x001100C0 File Offset: 0x0010E2C0
	public event Action<global::Item, float> onCycle;

	// Token: 0x06002CE2 RID: 11490 RVA: 0x001100F5 File Offset: 0x0010E2F5
	public void OnCycle(float delta)
	{
		if (this.onCycle != null)
		{
			this.onCycle(this, delta);
		}
	}

	// Token: 0x06002CE3 RID: 11491 RVA: 0x0011010C File Offset: 0x0010E30C
	public void ServerCommand(string command, global::BasePlayer player)
	{
		global::HeldEntity heldEntity = this.GetHeldEntity() as global::HeldEntity;
		if (heldEntity != null)
		{
			heldEntity.ServerCommand(this, command, player);
		}
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].ServerCommand(this, command, player);
		}
	}

	// Token: 0x06002CE4 RID: 11492 RVA: 0x0011015C File Offset: 0x0010E35C
	public void UseItem(int amountToConsume = 1)
	{
		if (amountToConsume <= 0)
		{
			return;
		}
		this.amount -= amountToConsume;
		if (this.amount <= 0)
		{
			this.amount = 0;
			this.Remove(0f);
			return;
		}
		this.MarkDirty();
	}

	// Token: 0x06002CE5 RID: 11493 RVA: 0x00110194 File Offset: 0x0010E394
	public bool HasAmmo(AmmoTypes ammoType)
	{
		ItemModProjectile itemModProjectile;
		return (this.info.TryGetComponent<ItemModProjectile>(out itemModProjectile) && itemModProjectile.IsAmmo(ammoType)) || (this.contents != null && this.contents.HasAmmo(ammoType));
	}

	// Token: 0x06002CE6 RID: 11494 RVA: 0x001101D4 File Offset: 0x0010E3D4
	public void FindAmmo(List<global::Item> list, AmmoTypes ammoType)
	{
		ItemModProjectile itemModProjectile;
		if (this.info.TryGetComponent<ItemModProjectile>(out itemModProjectile) && itemModProjectile.IsAmmo(ammoType))
		{
			list.Add(this);
			return;
		}
		if (this.contents != null)
		{
			this.contents.FindAmmo(list, ammoType);
		}
	}

	// Token: 0x06002CE7 RID: 11495 RVA: 0x00110218 File Offset: 0x0010E418
	public int GetAmmoAmount(AmmoTypes ammoType)
	{
		int num = 0;
		ItemModProjectile itemModProjectile;
		if (this.info.TryGetComponent<ItemModProjectile>(out itemModProjectile) && itemModProjectile.IsAmmo(ammoType))
		{
			num += this.amount;
		}
		if (this.contents != null)
		{
			num += this.contents.GetAmmoAmount(ammoType);
		}
		return num;
	}

	// Token: 0x06002CE8 RID: 11496 RVA: 0x00110260 File Offset: 0x0010E460
	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"Item.",
			this.info.shortname,
			"x",
			this.amount,
			".",
			this.uid
		});
	}

	// Token: 0x06002CE9 RID: 11497 RVA: 0x001102BA File Offset: 0x0010E4BA
	public global::Item FindItem(ItemId iUID)
	{
		if (this.uid == iUID)
		{
			return this;
		}
		if (this.contents == null)
		{
			return null;
		}
		return this.contents.FindItemByUID(iUID);
	}

	// Token: 0x06002CEA RID: 11498 RVA: 0x001102E4 File Offset: 0x0010E4E4
	public int MaxStackable()
	{
		int num = this.info.stackable;
		if (this.parent != null && this.parent.maxStackSize > 0)
		{
			num = Mathf.Min(this.parent.maxStackSize, num);
		}
		return num;
	}

	// Token: 0x170003AC RID: 940
	// (get) Token: 0x06002CEB RID: 11499 RVA: 0x00110326 File Offset: 0x0010E526
	public global::BaseEntity.TraitFlag Traits
	{
		get
		{
			return this.info.Traits;
		}
	}

	// Token: 0x06002CEC RID: 11500 RVA: 0x00110334 File Offset: 0x0010E534
	public virtual ProtoBuf.Item Save(bool bIncludeContainer = false, bool bIncludeOwners = true)
	{
		this.dirty = false;
		ProtoBuf.Item item = Facepunch.Pool.Get<ProtoBuf.Item>();
		item.UID = this.uid;
		item.itemid = this.info.itemid;
		item.slot = this.position;
		item.amount = this.amount;
		item.flags = (int)this.flags;
		item.removetime = this.removeTime;
		item.locktime = this.busyTime;
		item.instanceData = this.instanceData;
		item.worldEntity = this.worldEnt.uid;
		item.heldEntity = this.heldEntity.uid;
		item.skinid = this.skin;
		item.name = this.name;
		item.streamerName = this.streamerName;
		item.text = this.text;
		item.cooktime = this.cookTimeLeft;
		if (this.hasCondition)
		{
			item.conditionData = Facepunch.Pool.Get<ProtoBuf.Item.ConditionData>();
			item.conditionData.maxCondition = this._maxCondition;
			item.conditionData.condition = this._condition;
		}
		if (this.contents != null && bIncludeContainer)
		{
			item.contents = this.contents.Save();
		}
		return item;
	}

	// Token: 0x06002CED RID: 11501 RVA: 0x00110468 File Offset: 0x0010E668
	public virtual void Load(ProtoBuf.Item load)
	{
		if (this.info == null || this.info.itemid != load.itemid)
		{
			this.info = ItemManager.FindItemDefinition(load.itemid);
		}
		this.uid = load.UID;
		this.name = load.name;
		this.streamerName = load.streamerName;
		this.text = load.text;
		this.cookTimeLeft = load.cooktime;
		this.amount = load.amount;
		this.position = load.slot;
		this.busyTime = load.locktime;
		this.removeTime = load.removetime;
		this.flags = (global::Item.Flag)load.flags;
		this.worldEnt.uid = load.worldEntity;
		this.heldEntity.uid = load.heldEntity;
		if (this.isServer)
		{
			Network.Net.sv.RegisterUID(this.uid.Value);
		}
		if (this.instanceData != null)
		{
			this.instanceData.ShouldPool = true;
			this.instanceData.ResetToPool();
			this.instanceData = null;
		}
		this.instanceData = load.instanceData;
		if (this.instanceData != null)
		{
			this.instanceData.ShouldPool = false;
		}
		this.skin = load.skinid;
		if (this.info == null || this.info.itemid != load.itemid)
		{
			this.info = ItemManager.FindItemDefinition(load.itemid);
		}
		if (this.info == null)
		{
			return;
		}
		this._condition = 0f;
		this._maxCondition = 0f;
		if (load.conditionData != null)
		{
			this._condition = load.conditionData.condition;
			this._maxCondition = load.conditionData.maxCondition;
		}
		else if (this.info.condition.enabled)
		{
			this._condition = this.info.condition.max;
			this._maxCondition = this.info.condition.max;
		}
		if (load.contents != null)
		{
			if (this.contents == null)
			{
				this.contents = new global::ItemContainer();
				if (this.isServer)
				{
					this.contents.ServerInitialize(this, load.contents.slots);
				}
			}
			this.contents.Load(load.contents);
		}
		if (this.isServer)
		{
			this.removeTime = 0f;
			this.OnItemCreated();
		}
	}

	// Token: 0x04002485 RID: 9349
	private const string DefaultArmourBreakEffectPath = "assets/bundled/prefabs/fx/armor_break.prefab";

	// Token: 0x04002486 RID: 9350
	private float _condition;

	// Token: 0x04002487 RID: 9351
	private float _maxCondition = 100f;

	// Token: 0x04002488 RID: 9352
	public ItemDefinition info;

	// Token: 0x04002489 RID: 9353
	public ItemId uid;

	// Token: 0x0400248A RID: 9354
	public bool dirty;

	// Token: 0x0400248B RID: 9355
	public int amount = 1;

	// Token: 0x0400248C RID: 9356
	public int position;

	// Token: 0x0400248D RID: 9357
	public float busyTime;

	// Token: 0x0400248E RID: 9358
	public float removeTime;

	// Token: 0x0400248F RID: 9359
	public float fuel;

	// Token: 0x04002490 RID: 9360
	public bool isServer;

	// Token: 0x04002491 RID: 9361
	public ProtoBuf.Item.InstanceData instanceData;

	// Token: 0x04002492 RID: 9362
	public ulong skin;

	// Token: 0x04002493 RID: 9363
	public string name;

	// Token: 0x04002494 RID: 9364
	public string streamerName;

	// Token: 0x04002495 RID: 9365
	public string text;

	// Token: 0x04002496 RID: 9366
	public float cookTimeLeft;

	// Token: 0x04002498 RID: 9368
	public global::Item.Flag flags;

	// Token: 0x04002499 RID: 9369
	public global::ItemContainer contents;

	// Token: 0x0400249A RID: 9370
	public global::ItemContainer parent;

	// Token: 0x0400249B RID: 9371
	private EntityRef worldEnt;

	// Token: 0x0400249C RID: 9372
	private EntityRef heldEntity;

	// Token: 0x02000D8E RID: 3470
	[Flags]
	public enum Flag
	{
		// Token: 0x04004866 RID: 18534
		None = 0,
		// Token: 0x04004867 RID: 18535
		Placeholder = 1,
		// Token: 0x04004868 RID: 18536
		IsOn = 2,
		// Token: 0x04004869 RID: 18537
		OnFire = 4,
		// Token: 0x0400486A RID: 18538
		IsLocked = 8,
		// Token: 0x0400486B RID: 18539
		Cooking = 16
	}
}
