using System;

// Token: 0x020005E1 RID: 1505
public class ItemModBurnable : ItemMod
{
	// Token: 0x06002D70 RID: 11632 RVA: 0x001125F1 File Offset: 0x001107F1
	public override void OnItemCreated(Item item)
	{
		item.fuel = this.fuelAmount;
	}

	// Token: 0x04002519 RID: 9497
	public float fuelAmount = 10f;

	// Token: 0x0400251A RID: 9498
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition byproductItem;

	// Token: 0x0400251B RID: 9499
	public int byproductAmount = 1;

	// Token: 0x0400251C RID: 9500
	public float byproductChance = 0.5f;
}
