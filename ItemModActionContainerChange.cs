using System;
using UnityEngine;

// Token: 0x020005DB RID: 1499
public class ItemModActionContainerChange : ItemMod
{
	// Token: 0x06002D55 RID: 11605 RVA: 0x00112124 File Offset: 0x00110324
	public override void OnParentChanged(Item item)
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

	// Token: 0x06002D56 RID: 11606 RVA: 0x0011216C File Offset: 0x0011036C
	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null!", base.gameObject);
		}
	}

	// Token: 0x0400250D RID: 9485
	public ItemMod[] actions;
}
