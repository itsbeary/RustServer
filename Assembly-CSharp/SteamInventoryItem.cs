using System;
using UnityEngine;

// Token: 0x02000767 RID: 1895
public class SteamInventoryItem : ScriptableObject
{
	// Token: 0x1700044A RID: 1098
	// (get) Token: 0x060034A0 RID: 13472 RVA: 0x00144DF8 File Offset: 0x00142FF8
	public ItemDefinition itemDefinition
	{
		get
		{
			return ItemManager.FindItemDefinition(this.itemname);
		}
	}

	// Token: 0x060034A1 RID: 13473 RVA: 0x00144E05 File Offset: 0x00143005
	public virtual bool HasUnlocked(ulong playerId)
	{
		return this.DlcItem != null && this.DlcItem.HasLicense(playerId);
	}

	// Token: 0x04002B35 RID: 11061
	public int id;

	// Token: 0x04002B36 RID: 11062
	public Sprite icon;

	// Token: 0x04002B37 RID: 11063
	public Translate.Phrase displayName;

	// Token: 0x04002B38 RID: 11064
	public Translate.Phrase displayDescription;

	// Token: 0x04002B39 RID: 11065
	[Header("Steam Inventory")]
	public SteamInventoryItem.Category category;

	// Token: 0x04002B3A RID: 11066
	public SteamInventoryItem.SubCategory subcategory;

	// Token: 0x04002B3B RID: 11067
	public SteamInventoryCategory steamCategory;

	// Token: 0x04002B3C RID: 11068
	public bool isLimitedTimeOffer = true;

	// Token: 0x04002B3D RID: 11069
	[Tooltip("Stop this item being broken down into cloth etc")]
	public bool PreventBreakingDown;

	// Token: 0x04002B3E RID: 11070
	[Header("Meta")]
	public string itemname;

	// Token: 0x04002B3F RID: 11071
	public ulong workshopID;

	// Token: 0x04002B40 RID: 11072
	public SteamDLCItem DlcItem;

	// Token: 0x04002B41 RID: 11073
	[Tooltip("Does nothing currently")]
	public bool forceCraftableItemDesc;

	// Token: 0x02000E82 RID: 3714
	public enum Category
	{
		// Token: 0x04004C49 RID: 19529
		None,
		// Token: 0x04004C4A RID: 19530
		Clothing,
		// Token: 0x04004C4B RID: 19531
		Weapon,
		// Token: 0x04004C4C RID: 19532
		Decoration,
		// Token: 0x04004C4D RID: 19533
		Crate,
		// Token: 0x04004C4E RID: 19534
		Resource
	}

	// Token: 0x02000E83 RID: 3715
	public enum SubCategory
	{
		// Token: 0x04004C50 RID: 19536
		None,
		// Token: 0x04004C51 RID: 19537
		Shirt,
		// Token: 0x04004C52 RID: 19538
		Pants,
		// Token: 0x04004C53 RID: 19539
		Jacket,
		// Token: 0x04004C54 RID: 19540
		Hat,
		// Token: 0x04004C55 RID: 19541
		Mask,
		// Token: 0x04004C56 RID: 19542
		Footwear,
		// Token: 0x04004C57 RID: 19543
		Weapon,
		// Token: 0x04004C58 RID: 19544
		Misc,
		// Token: 0x04004C59 RID: 19545
		Crate,
		// Token: 0x04004C5A RID: 19546
		Resource,
		// Token: 0x04004C5B RID: 19547
		CrateUncraftable
	}
}
