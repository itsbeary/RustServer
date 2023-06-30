using System;

// Token: 0x020005E5 RID: 1509
public class ItemModConditionContainerFlag : ItemMod
{
	// Token: 0x06002D77 RID: 11639 RVA: 0x00112650 File Offset: 0x00110850
	public override bool Passes(Item item)
	{
		if (item.parent == null)
		{
			return !this.requiredState;
		}
		if (!item.parent.HasFlag(this.flag))
		{
			return !this.requiredState;
		}
		return this.requiredState;
	}

	// Token: 0x04002521 RID: 9505
	public ItemContainer.Flag flag;

	// Token: 0x04002522 RID: 9506
	public bool requiredState;
}
