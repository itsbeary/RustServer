using System;

// Token: 0x020005E8 RID: 1512
public class ItemModConditionHasFlag : ItemMod
{
	// Token: 0x06002D7E RID: 11646 RVA: 0x001127B5 File Offset: 0x001109B5
	public override bool Passes(Item item)
	{
		return item.HasFlag(this.flag) == this.requiredState;
	}

	// Token: 0x04002528 RID: 9512
	public Item.Flag flag;

	// Token: 0x04002529 RID: 9513
	public bool requiredState;
}
