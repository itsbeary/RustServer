using System;
using UnityEngine;

// Token: 0x02000766 RID: 1894
[CreateAssetMenu(menuName = "Rust/Generic Steam Inventory Category")]
public class SteamInventoryCategory : ScriptableObject
{
	// Token: 0x04002B2F RID: 11055
	[Header("Steam Inventory")]
	public bool canBeSoldToOtherUsers;

	// Token: 0x04002B30 RID: 11056
	public bool canBeTradedWithOtherUsers;

	// Token: 0x04002B31 RID: 11057
	public bool isCommodity;

	// Token: 0x04002B32 RID: 11058
	public SteamInventoryCategory.Price price;

	// Token: 0x04002B33 RID: 11059
	public SteamInventoryCategory.DropChance dropChance;

	// Token: 0x04002B34 RID: 11060
	public bool CanBeInCrates = true;

	// Token: 0x02000E80 RID: 3712
	public enum Price
	{
		// Token: 0x04004C15 RID: 19477
		CannotBuy,
		// Token: 0x04004C16 RID: 19478
		VLV25,
		// Token: 0x04004C17 RID: 19479
		VLV50,
		// Token: 0x04004C18 RID: 19480
		VLV75,
		// Token: 0x04004C19 RID: 19481
		VLV100,
		// Token: 0x04004C1A RID: 19482
		VLV150,
		// Token: 0x04004C1B RID: 19483
		VLV200,
		// Token: 0x04004C1C RID: 19484
		VLV250,
		// Token: 0x04004C1D RID: 19485
		VLV300,
		// Token: 0x04004C1E RID: 19486
		VLV350,
		// Token: 0x04004C1F RID: 19487
		VLV400,
		// Token: 0x04004C20 RID: 19488
		VLV450,
		// Token: 0x04004C21 RID: 19489
		VLV500,
		// Token: 0x04004C22 RID: 19490
		VLV550,
		// Token: 0x04004C23 RID: 19491
		VLV600,
		// Token: 0x04004C24 RID: 19492
		VLV650,
		// Token: 0x04004C25 RID: 19493
		VLV700,
		// Token: 0x04004C26 RID: 19494
		VLV750,
		// Token: 0x04004C27 RID: 19495
		VLV800,
		// Token: 0x04004C28 RID: 19496
		VLV850,
		// Token: 0x04004C29 RID: 19497
		VLV900,
		// Token: 0x04004C2A RID: 19498
		VLV950,
		// Token: 0x04004C2B RID: 19499
		VLV1000,
		// Token: 0x04004C2C RID: 19500
		VLV1100,
		// Token: 0x04004C2D RID: 19501
		VLV1200,
		// Token: 0x04004C2E RID: 19502
		VLV1300,
		// Token: 0x04004C2F RID: 19503
		VLV1400,
		// Token: 0x04004C30 RID: 19504
		VLV1500,
		// Token: 0x04004C31 RID: 19505
		VLV1600,
		// Token: 0x04004C32 RID: 19506
		VLV1700,
		// Token: 0x04004C33 RID: 19507
		VLV1800,
		// Token: 0x04004C34 RID: 19508
		VLV1900,
		// Token: 0x04004C35 RID: 19509
		VLV2000,
		// Token: 0x04004C36 RID: 19510
		VLV2500,
		// Token: 0x04004C37 RID: 19511
		VLV3000,
		// Token: 0x04004C38 RID: 19512
		VLV3500,
		// Token: 0x04004C39 RID: 19513
		VLV4000,
		// Token: 0x04004C3A RID: 19514
		VLV4500,
		// Token: 0x04004C3B RID: 19515
		VLV5000,
		// Token: 0x04004C3C RID: 19516
		VLV6000,
		// Token: 0x04004C3D RID: 19517
		VLV7000,
		// Token: 0x04004C3E RID: 19518
		VLV8000,
		// Token: 0x04004C3F RID: 19519
		VLV9000,
		// Token: 0x04004C40 RID: 19520
		VLV10000
	}

	// Token: 0x02000E81 RID: 3713
	public enum DropChance
	{
		// Token: 0x04004C42 RID: 19522
		NeverDrop,
		// Token: 0x04004C43 RID: 19523
		VeryRare,
		// Token: 0x04004C44 RID: 19524
		Rare,
		// Token: 0x04004C45 RID: 19525
		Common,
		// Token: 0x04004C46 RID: 19526
		VeryCommon,
		// Token: 0x04004C47 RID: 19527
		ExtremelyRare
	}
}
