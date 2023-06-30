using System;
using UnityEngine;

// Token: 0x0200011F RID: 287
public class RANDSwitch : ElectricalBlocker
{
	// Token: 0x0600169A RID: 5786 RVA: 0x000AEAC4 File Offset: 0x000ACCC4
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return base.GetPassthroughAmount(outputSlot) * (base.IsOn() ? 0 : 1);
	}

	// Token: 0x0600169B RID: 5787 RVA: 0x000AEADC File Offset: 0x000ACCDC
	public override void UpdateBlocked()
	{
		bool flag = base.IsOn();
		base.SetFlag(BaseEntity.Flags.On, this.rand, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved8, this.rand, false, false);
		this.UpdateHasPower(this.input1Amount + this.input2Amount, 1);
		if (flag != base.IsOn())
		{
			this.MarkDirty();
		}
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x000AEB33 File Offset: 0x000ACD33
	public bool RandomRoll()
	{
		return UnityEngine.Random.Range(0, 2) == 1;
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x000AEB40 File Offset: 0x000ACD40
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1 && inputAmount > 0)
		{
			this.input1Amount = inputAmount;
			this.rand = this.RandomRoll();
			this.UpdateBlocked();
		}
		if (inputSlot == 2)
		{
			if (inputAmount > 0)
			{
				this.rand = false;
				this.UpdateBlocked();
				return;
			}
		}
		else
		{
			base.UpdateFromInput(inputAmount, inputSlot);
		}
	}

	// Token: 0x04000EAB RID: 3755
	private bool rand;
}
