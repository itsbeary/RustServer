using System;

// Token: 0x020005E9 RID: 1513
public class ItemModConditionInWater : ItemMod
{
	// Token: 0x06002D80 RID: 11648 RVA: 0x001127CC File Offset: 0x001109CC
	public override bool Passes(Item item)
	{
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		return !(ownerPlayer == null) && ownerPlayer.IsHeadUnderwater() == this.requiredState;
	}

	// Token: 0x0400252A RID: 9514
	public bool requiredState;
}
