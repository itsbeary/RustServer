using System;

// Token: 0x02000608 RID: 1544
public class ItemModSwitchFlag : ItemMod
{
	// Token: 0x06002DE2 RID: 11746 RVA: 0x0011454E File Offset: 0x0011274E
	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		if (item.HasFlag(this.flag) == this.state)
		{
			return;
		}
		item.SetFlag(this.flag, this.state);
		item.MarkDirty();
	}

	// Token: 0x040025A1 RID: 9633
	public Item.Flag flag;

	// Token: 0x040025A2 RID: 9634
	public bool state;
}
