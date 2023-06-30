using System;
using UnityEngine;

// Token: 0x020005D8 RID: 1496
public class ItemSelector : PropertyAttribute
{
	// Token: 0x06002D41 RID: 11585 RVA: 0x00112069 File Offset: 0x00110269
	public ItemSelector(ItemCategory category = ItemCategory.All)
	{
		this.category = category;
	}

	// Token: 0x0400250A RID: 9482
	public ItemCategory category = ItemCategory.All;
}
