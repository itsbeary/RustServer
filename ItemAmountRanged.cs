using System;
using UnityEngine;

// Token: 0x02000610 RID: 1552
[Serializable]
public class ItemAmountRanged : ItemAmount
{
	// Token: 0x06002E01 RID: 11777 RVA: 0x00114C4C File Offset: 0x00112E4C
	public override void OnAfterDeserialize()
	{
		base.OnAfterDeserialize();
	}

	// Token: 0x06002E02 RID: 11778 RVA: 0x00114C54 File Offset: 0x00112E54
	public ItemAmountRanged(ItemDefinition item = null, float amt = 0f, float max = -1f)
		: base(item, amt)
	{
		this.maxAmount = max;
	}

	// Token: 0x06002E03 RID: 11779 RVA: 0x00114C70 File Offset: 0x00112E70
	public override float GetAmount()
	{
		if (this.maxAmount > 0f && this.maxAmount > this.amount)
		{
			return UnityEngine.Random.Range(this.amount, this.maxAmount);
		}
		return this.amount;
	}

	// Token: 0x040025C3 RID: 9667
	public float maxAmount = -1f;
}
