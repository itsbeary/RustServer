using System;

// Token: 0x020005DC RID: 1500
public class ItemModAlterCondition : ItemMod
{
	// Token: 0x06002D58 RID: 11608 RVA: 0x00112186 File Offset: 0x00110386
	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		if (this.conditionChange < 0f)
		{
			item.LoseCondition(this.conditionChange * -1f);
			return;
		}
		item.RepairCondition(this.conditionChange);
	}

	// Token: 0x0400250E RID: 9486
	public float conditionChange;
}
