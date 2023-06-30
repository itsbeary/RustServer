using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200018D RID: 397
public class ChristmasTree : StorageContainer
{
	// Token: 0x060017FB RID: 6139 RVA: 0x000B4630 File Offset: 0x000B2830
	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (item.info.GetComponent<ItemModXMasTreeDecoration>() == null)
		{
			return false;
		}
		using (List<Item>.Enumerator enumerator = base.inventory.itemList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.info == item.info)
				{
					return false;
				}
			}
		}
		return base.ItemFilter(item, targetSlot);
	}

	// Token: 0x060017FC RID: 6140 RVA: 0x000B46B8 File Offset: 0x000B28B8
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		ItemModXMasTreeDecoration component = item.info.GetComponent<ItemModXMasTreeDecoration>();
		if (component != null)
		{
			base.SetFlag((BaseEntity.Flags)component.flagsToChange, added, false, true);
		}
		base.OnItemAddedOrRemoved(item, added);
	}

	// Token: 0x040010BB RID: 4283
	public GameObject[] decorations;
}
