using System;

// Token: 0x020005EE RID: 1518
public class ItemModConsumeContents : ItemMod
{
	// Token: 0x06002D8F RID: 11663 RVA: 0x00112C28 File Offset: 0x00110E28
	public override void DoAction(Item item, BasePlayer player)
	{
		foreach (Item item2 in item.contents.itemList)
		{
			ItemModConsume component = item2.info.GetComponent<ItemModConsume>();
			if (!(component == null) && component.CanDoAction(item2, player))
			{
				component.DoAction(item2, player);
				break;
			}
		}
	}

	// Token: 0x06002D90 RID: 11664 RVA: 0x00112CA4 File Offset: 0x00110EA4
	public override bool CanDoAction(Item item, BasePlayer player)
	{
		if (!player.metabolism.CanConsume())
		{
			return false;
		}
		if (item.contents == null)
		{
			return false;
		}
		foreach (Item item2 in item.contents.itemList)
		{
			ItemModConsume component = item2.info.GetComponent<ItemModConsume>();
			if (!(component == null) && component.CanDoAction(item2, player))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002538 RID: 9528
	public GameObjectRef consumeEffect;
}
