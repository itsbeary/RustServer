using System;

// Token: 0x0200060A RID: 1546
public class ItemModUseContent : ItemMod
{
	// Token: 0x06002DE6 RID: 11750 RVA: 0x001146BE File Offset: 0x001128BE
	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.contents == null)
		{
			return;
		}
		if (item.contents.itemList.Count == 0)
		{
			return;
		}
		item.contents.itemList[0].UseItem(this.amountToConsume);
	}

	// Token: 0x040025AA RID: 9642
	public int amountToConsume = 1;
}
