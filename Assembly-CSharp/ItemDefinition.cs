using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x020005D4 RID: 1492
public class ItemDefinition : MonoBehaviour
{
	// Token: 0x170003B2 RID: 946
	// (get) Token: 0x06002D2E RID: 11566 RVA: 0x00111DC8 File Offset: 0x0010FFC8
	public IPlayerItemDefinition[] skins2
	{
		get
		{
			if (this._skins2 != null)
			{
				return this._skins2;
			}
			if (PlatformService.Instance.IsValid && PlatformService.Instance.ItemDefinitions != null)
			{
				string prefabname = base.name;
				this._skins2 = PlatformService.Instance.ItemDefinitions.Where((IPlayerItemDefinition x) => (x.ItemShortName == this.shortname || x.ItemShortName == prefabname) && x.WorkshopId > 0UL).ToArray<IPlayerItemDefinition>();
			}
			return this._skins2;
		}
	}

	// Token: 0x06002D2F RID: 11567 RVA: 0x00111E41 File Offset: 0x00110041
	public void InvalidateWorkshopSkinCache()
	{
		this._skins2 = null;
	}

	// Token: 0x06002D30 RID: 11568 RVA: 0x00111E4C File Offset: 0x0011004C
	public static ulong FindSkin(int itemID, int skinID)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemID);
		if (itemDefinition == null)
		{
			return 0UL;
		}
		IPlayerItemDefinition itemDefinition2 = PlatformService.Instance.GetItemDefinition(skinID);
		if (itemDefinition2 != null)
		{
			ulong workshopDownload = itemDefinition2.WorkshopDownload;
			if (workshopDownload != 0UL)
			{
				string itemShortName = itemDefinition2.ItemShortName;
				if (itemShortName == itemDefinition.shortname || itemShortName == itemDefinition.name)
				{
					return workshopDownload;
				}
			}
		}
		for (int i = 0; i < itemDefinition.skins.Length; i++)
		{
			if (itemDefinition.skins[i].id == skinID)
			{
				return (ulong)((long)skinID);
			}
		}
		return 0UL;
	}

	// Token: 0x170003B3 RID: 947
	// (get) Token: 0x06002D31 RID: 11569 RVA: 0x00111EDB File Offset: 0x001100DB
	public ItemBlueprint Blueprint
	{
		get
		{
			return base.GetComponent<ItemBlueprint>();
		}
	}

	// Token: 0x170003B4 RID: 948
	// (get) Token: 0x06002D32 RID: 11570 RVA: 0x00111EE3 File Offset: 0x001100E3
	public int craftingStackable
	{
		get
		{
			return Mathf.Max(10, this.stackable);
		}
	}

	// Token: 0x06002D33 RID: 11571 RVA: 0x00111EF2 File Offset: 0x001100F2
	public bool HasFlag(ItemDefinition.Flag f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x06002D34 RID: 11572 RVA: 0x00111F00 File Offset: 0x00110100
	public void Initialize(List<ItemDefinition> itemList)
	{
		if (this.itemMods != null)
		{
			Debug.LogError("Item Definition Initializing twice: " + base.name);
		}
		this.skins = ItemSkinDirectory.ForItem(this);
		this.itemMods = base.GetComponentsInChildren<ItemMod>(true);
		ItemMod[] array = this.itemMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ModInit();
		}
		this.Children = itemList.Where((ItemDefinition x) => x.Parent == this).ToArray<ItemDefinition>();
		this.ItemModWearable = base.GetComponent<ItemModWearable>();
		this.isHoldable = base.GetComponent<ItemModEntity>() != null;
		this.isUsable = base.GetComponent<ItemModEntity>() != null || base.GetComponent<ItemModConsume>() != null;
	}

	// Token: 0x170003B5 RID: 949
	// (get) Token: 0x06002D35 RID: 11573 RVA: 0x00111FBE File Offset: 0x001101BE
	public bool isWearable
	{
		get
		{
			return this.ItemModWearable != null;
		}
	}

	// Token: 0x170003B6 RID: 950
	// (get) Token: 0x06002D36 RID: 11574 RVA: 0x00111FCC File Offset: 0x001101CC
	// (set) Token: 0x06002D37 RID: 11575 RVA: 0x00111FD4 File Offset: 0x001101D4
	public ItemModWearable ItemModWearable { get; private set; }

	// Token: 0x170003B7 RID: 951
	// (get) Token: 0x06002D38 RID: 11576 RVA: 0x00111FDD File Offset: 0x001101DD
	// (set) Token: 0x06002D39 RID: 11577 RVA: 0x00111FE5 File Offset: 0x001101E5
	public bool isHoldable { get; private set; }

	// Token: 0x170003B8 RID: 952
	// (get) Token: 0x06002D3A RID: 11578 RVA: 0x00111FEE File Offset: 0x001101EE
	// (set) Token: 0x06002D3B RID: 11579 RVA: 0x00111FF6 File Offset: 0x001101F6
	public bool isUsable { get; private set; }

	// Token: 0x170003B9 RID: 953
	// (get) Token: 0x06002D3C RID: 11580 RVA: 0x00111FFF File Offset: 0x001101FF
	public bool HasSkins
	{
		get
		{
			return (this.skins2 != null && this.skins2.Length != 0) || (this.skins != null && this.skins.Length != 0);
		}
	}

	// Token: 0x170003BA RID: 954
	// (get) Token: 0x06002D3D RID: 11581 RVA: 0x00112028 File Offset: 0x00110228
	// (set) Token: 0x06002D3E RID: 11582 RVA: 0x00112030 File Offset: 0x00110230
	public bool CraftableWithSkin { get; private set; }

	// Token: 0x040024C4 RID: 9412
	[Header("Item")]
	[ReadOnly]
	public int itemid;

	// Token: 0x040024C5 RID: 9413
	[Tooltip("The shortname should be unique. A hash will be generated from it to identify the item type. If this name changes at any point it will make all saves incompatible")]
	public string shortname;

	// Token: 0x040024C6 RID: 9414
	[Header("Appearance")]
	public Translate.Phrase displayName;

	// Token: 0x040024C7 RID: 9415
	public Translate.Phrase displayDescription;

	// Token: 0x040024C8 RID: 9416
	public Sprite iconSprite;

	// Token: 0x040024C9 RID: 9417
	public ItemCategory category;

	// Token: 0x040024CA RID: 9418
	public ItemSelectionPanel selectionPanel;

	// Token: 0x040024CB RID: 9419
	[Header("Containment")]
	public int maxDraggable;

	// Token: 0x040024CC RID: 9420
	public ItemContainer.ContentsType itemType = ItemContainer.ContentsType.Generic;

	// Token: 0x040024CD RID: 9421
	public ItemDefinition.AmountType amountType;

	// Token: 0x040024CE RID: 9422
	[InspectorFlags]
	public ItemSlot occupySlots = ItemSlot.None;

	// Token: 0x040024CF RID: 9423
	public int stackable;

	// Token: 0x040024D0 RID: 9424
	public bool quickDespawn;

	// Token: 0x040024D1 RID: 9425
	[Header("Spawn Tables")]
	[Tooltip("How rare this item is and how much it costs to research")]
	public Rarity rarity;

	// Token: 0x040024D2 RID: 9426
	public Rarity despawnRarity;

	// Token: 0x040024D3 RID: 9427
	public bool spawnAsBlueprint;

	// Token: 0x040024D4 RID: 9428
	[Header("Sounds")]
	public SoundDefinition inventoryGrabSound;

	// Token: 0x040024D5 RID: 9429
	public SoundDefinition inventoryDropSound;

	// Token: 0x040024D6 RID: 9430
	public SoundDefinition physImpactSoundDef;

	// Token: 0x040024D7 RID: 9431
	public ItemDefinition.Condition condition;

	// Token: 0x040024D8 RID: 9432
	[Header("Misc")]
	public bool hidden;

	// Token: 0x040024D9 RID: 9433
	[InspectorFlags]
	public ItemDefinition.Flag flags;

	// Token: 0x040024DA RID: 9434
	[Tooltip("User can craft this item on any server if they have this steam item")]
	public SteamInventoryItem steamItem;

	// Token: 0x040024DB RID: 9435
	[Tooltip("User can craft this item if they have this DLC purchased")]
	public SteamDLCItem steamDlc;

	// Token: 0x040024DC RID: 9436
	[Tooltip("Can only craft this item if the parent is craftable (tech tree)")]
	public ItemDefinition Parent;

	// Token: 0x040024DD RID: 9437
	public GameObjectRef worldModelPrefab;

	// Token: 0x040024DE RID: 9438
	public ItemDefinition isRedirectOf;

	// Token: 0x040024DF RID: 9439
	public ItemDefinition.RedirectVendingBehaviour redirectVendingBehaviour;

	// Token: 0x040024E0 RID: 9440
	[NonSerialized]
	public ItemMod[] itemMods;

	// Token: 0x040024E1 RID: 9441
	public BaseEntity.TraitFlag Traits;

	// Token: 0x040024E2 RID: 9442
	[NonSerialized]
	public ItemSkinDirectory.Skin[] skins;

	// Token: 0x040024E3 RID: 9443
	[NonSerialized]
	private IPlayerItemDefinition[] _skins2;

	// Token: 0x040024E4 RID: 9444
	[Tooltip("Panel to show in the inventory menu when selected")]
	public GameObject panel;

	// Token: 0x040024E9 RID: 9449
	[NonSerialized]
	public ItemDefinition[] Children = new ItemDefinition[0];

	// Token: 0x02000D95 RID: 3477
	[Serializable]
	public struct Condition
	{
		// Token: 0x04004884 RID: 18564
		public bool enabled;

		// Token: 0x04004885 RID: 18565
		[Tooltip("The maximum condition this item type can have, new items will start with this value")]
		public float max;

		// Token: 0x04004886 RID: 18566
		[Tooltip("If false then item will destroy when condition reaches 0")]
		public bool repairable;

		// Token: 0x04004887 RID: 18567
		[Tooltip("If true, never lose max condition when repaired")]
		public bool maintainMaxCondition;

		// Token: 0x04004888 RID: 18568
		public bool ovenCondition;

		// Token: 0x04004889 RID: 18569
		public ItemDefinition.Condition.WorldSpawnCondition foundCondition;

		// Token: 0x02000FE0 RID: 4064
		[Serializable]
		public class WorldSpawnCondition
		{
			// Token: 0x0400518A RID: 20874
			public float fractionMin = 1f;

			// Token: 0x0400518B RID: 20875
			public float fractionMax = 1f;
		}
	}

	// Token: 0x02000D96 RID: 3478
	public enum RedirectVendingBehaviour
	{
		// Token: 0x0400488B RID: 18571
		NoListing,
		// Token: 0x0400488C RID: 18572
		ListAsUniqueItem
	}

	// Token: 0x02000D97 RID: 3479
	[Flags]
	public enum Flag
	{
		// Token: 0x0400488E RID: 18574
		NoDropping = 1,
		// Token: 0x0400488F RID: 18575
		NotStraightToBelt = 2
	}

	// Token: 0x02000D98 RID: 3480
	public enum AmountType
	{
		// Token: 0x04004891 RID: 18577
		Count,
		// Token: 0x04004892 RID: 18578
		Millilitre,
		// Token: 0x04004893 RID: 18579
		Feet,
		// Token: 0x04004894 RID: 18580
		Genetics,
		// Token: 0x04004895 RID: 18581
		OxygenSeconds,
		// Token: 0x04004896 RID: 18582
		Frequency,
		// Token: 0x04004897 RID: 18583
		Generic,
		// Token: 0x04004898 RID: 18584
		BagLimit
	}
}
