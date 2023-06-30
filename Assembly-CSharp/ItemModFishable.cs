using System;
using UnityEngine;

// Token: 0x020005F6 RID: 1526
public class ItemModFishable : ItemMod
{
	// Token: 0x04002554 RID: 9556
	public bool CanBeFished = true;

	// Token: 0x04002555 RID: 9557
	[Header("Catching Behaviour")]
	public float StrainModifier = 1f;

	// Token: 0x04002556 RID: 9558
	public float MoveMultiplier = 1f;

	// Token: 0x04002557 RID: 9559
	public float ReelInSpeedMultiplier = 1f;

	// Token: 0x04002558 RID: 9560
	public float CatchWaitTimeMultiplier = 1f;

	// Token: 0x04002559 RID: 9561
	[Header("Catch Criteria")]
	public float MinimumBaitLevel;

	// Token: 0x0400255A RID: 9562
	public float MaximumBaitLevel;

	// Token: 0x0400255B RID: 9563
	public float MinimumWaterDepth;

	// Token: 0x0400255C RID: 9564
	public float MaximumWaterDepth;

	// Token: 0x0400255D RID: 9565
	[InspectorFlags]
	public WaterBody.FishingTag RequiredTag;

	// Token: 0x0400255E RID: 9566
	[Range(0f, 1f)]
	public float Chance;

	// Token: 0x0400255F RID: 9567
	public string SteamStatName;
}
