using System;

// Token: 0x020005EA RID: 1514
public class ItemModConditionIsSleeping : ItemMod
{
	// Token: 0x06002D82 RID: 11650 RVA: 0x001127FC File Offset: 0x001109FC
	public override bool Passes(Item item)
	{
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		return !(ownerPlayer == null) && ownerPlayer.IsSleeping() == this.requiredState;
	}

	// Token: 0x0400252B RID: 9515
	public bool requiredState;
}
