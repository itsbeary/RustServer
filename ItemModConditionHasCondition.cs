using System;
using UnityEngine;

// Token: 0x020005E6 RID: 1510
public class ItemModConditionHasCondition : ItemMod
{
	// Token: 0x06002D79 RID: 11641 RVA: 0x00112688 File Offset: 0x00110888
	public override bool Passes(Item item)
	{
		if (!item.hasCondition)
		{
			return false;
		}
		if (this.conditionFractionTarget > 0f)
		{
			return (!this.lessThan && item.conditionNormalized > this.conditionFractionTarget) || (this.lessThan && item.conditionNormalized < this.conditionFractionTarget);
		}
		return (!this.lessThan && item.condition >= this.conditionTarget) || (this.lessThan && item.condition < this.conditionTarget);
	}

	// Token: 0x04002523 RID: 9507
	public float conditionTarget = 1f;

	// Token: 0x04002524 RID: 9508
	[Tooltip("If set to above 0 will check for fraction instead of raw value")]
	public float conditionFractionTarget = -1f;

	// Token: 0x04002525 RID: 9509
	public bool lessThan;
}
