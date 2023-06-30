using System;

// Token: 0x020005DD RID: 1501
public class ItemModAnimalEquipment : ItemMod
{
	// Token: 0x0400250F RID: 9487
	public BaseEntity.Flags WearableFlag;

	// Token: 0x04002510 RID: 9488
	public bool hideHair;

	// Token: 0x04002511 RID: 9489
	public ProtectionProperties animalProtection;

	// Token: 0x04002512 RID: 9490
	public ProtectionProperties riderProtection;

	// Token: 0x04002513 RID: 9491
	public int additionalInventorySlots;

	// Token: 0x04002514 RID: 9492
	public float speedModifier;

	// Token: 0x04002515 RID: 9493
	public float staminaUseModifier;

	// Token: 0x04002516 RID: 9494
	public ItemModAnimalEquipment.SlotType slot;

	// Token: 0x02000D9A RID: 3482
	public enum SlotType
	{
		// Token: 0x0400489C RID: 18588
		Basic,
		// Token: 0x0400489D RID: 18589
		Armor,
		// Token: 0x0400489E RID: 18590
		Saddle,
		// Token: 0x0400489F RID: 18591
		Bit,
		// Token: 0x040048A0 RID: 18592
		Feet,
		// Token: 0x040048A1 RID: 18593
		SaddleDouble
	}
}
