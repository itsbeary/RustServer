using System;
using UnityEngine;

// Token: 0x020005DA RID: 1498
public class ItemModActionChange : ItemMod
{
	// Token: 0x06002D52 RID: 11602 RVA: 0x001120C0 File Offset: 0x001102C0
	public override void OnChanged(Item item)
	{
		if (!item.isServer)
		{
			return;
		}
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		foreach (ItemMod itemMod in this.actions)
		{
			if (itemMod.CanDoAction(item, ownerPlayer))
			{
				itemMod.DoAction(item, ownerPlayer);
			}
		}
	}

	// Token: 0x06002D53 RID: 11603 RVA: 0x00112108 File Offset: 0x00110308
	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null!", base.gameObject);
		}
	}

	// Token: 0x0400250C RID: 9484
	public ItemMod[] actions;
}
