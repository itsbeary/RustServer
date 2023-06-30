using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020005D3 RID: 1491
public class ItemBlueprint : MonoBehaviour
{
	// Token: 0x170003B0 RID: 944
	// (get) Token: 0x06002D2B RID: 11563 RVA: 0x00111D5C File Offset: 0x0010FF5C
	public ItemDefinition targetItem
	{
		get
		{
			return base.GetComponent<ItemDefinition>();
		}
	}

	// Token: 0x170003B1 RID: 945
	// (get) Token: 0x06002D2C RID: 11564 RVA: 0x00111D64 File Offset: 0x0010FF64
	public bool NeedsSteamDLC
	{
		get
		{
			return this.targetItem.steamDlc != null;
		}
	}

	// Token: 0x040024B4 RID: 9396
	public List<ItemAmount> ingredients = new List<ItemAmount>();

	// Token: 0x040024B5 RID: 9397
	public List<ItemDefinition> additionalUnlocks = new List<ItemDefinition>();

	// Token: 0x040024B6 RID: 9398
	public bool defaultBlueprint;

	// Token: 0x040024B7 RID: 9399
	public bool userCraftable = true;

	// Token: 0x040024B8 RID: 9400
	public bool isResearchable = true;

	// Token: 0x040024B9 RID: 9401
	public bool forceShowInConveyorFilter;

	// Token: 0x040024BA RID: 9402
	public Rarity rarity;

	// Token: 0x040024BB RID: 9403
	[Header("Workbench")]
	public int workbenchLevelRequired;

	// Token: 0x040024BC RID: 9404
	[Header("Scrap")]
	public int scrapRequired;

	// Token: 0x040024BD RID: 9405
	public int scrapFromRecycle;

	// Token: 0x040024BE RID: 9406
	[Header("Unlocking")]
	[Tooltip("This item won't show anywhere unless you have the corresponding SteamItem in your inventory - which is defined on the ItemDefinition")]
	public bool NeedsSteamItem;

	// Token: 0x040024BF RID: 9407
	public int blueprintStackSize = -1;

	// Token: 0x040024C0 RID: 9408
	public float time = 1f;

	// Token: 0x040024C1 RID: 9409
	public int amountToCreate = 1;

	// Token: 0x040024C2 RID: 9410
	public string UnlockAchievment;

	// Token: 0x040024C3 RID: 9411
	public string RecycleStat;
}
