using System;
using System.Linq;
using UnityEngine;

// Token: 0x020005E7 RID: 1511
public class ItemModConditionHasContents : ItemMod
{
	// Token: 0x06002D7B RID: 11643 RVA: 0x0011272C File Offset: 0x0011092C
	public override bool Passes(Item item)
	{
		if (item.contents == null)
		{
			return !this.requiredState;
		}
		if (item.contents.itemList.Count == 0)
		{
			return !this.requiredState;
		}
		if (this.itemDef && !item.contents.itemList.Any((Item x) => x.info == this.itemDef))
		{
			return !this.requiredState;
		}
		return this.requiredState;
	}

	// Token: 0x04002526 RID: 9510
	[Tooltip("Can be null to mean any item")]
	public ItemDefinition itemDef;

	// Token: 0x04002527 RID: 9511
	public bool requiredState;
}
