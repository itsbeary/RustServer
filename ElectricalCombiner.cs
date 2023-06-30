using System;
using UnityEngine;

// Token: 0x0200011D RID: 285
public class ElectricalCombiner : IOEntity
{
	// Token: 0x0600168D RID: 5773 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x0600168E RID: 5774 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool BlockFluidDraining
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600168F RID: 5775 RVA: 0x000AE79C File Offset: 0x000AC99C
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int num = this.input1Amount + this.input2Amount + this.input3Amount;
		Mathf.Clamp(num - 1, 0, num);
		return num;
	}

	// Token: 0x06001690 RID: 5776 RVA: 0x000AE7CA File Offset: 0x000AC9CA
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x06001691 RID: 5777 RVA: 0x000AE7F0 File Offset: 0x000AC9F0
	public override void UpdateFromInput(int inputAmount, int slot)
	{
		if (inputAmount > 0 && base.IsConnectedTo(this, slot, IOEntity.backtracking * 2, true))
		{
			inputAmount = 0;
			base.SetFlag(BaseEntity.Flags.Reserved7, true, false, true);
		}
		else
		{
			base.SetFlag(BaseEntity.Flags.Reserved7, false, false, true);
		}
		if (slot == 0)
		{
			this.input1Amount = inputAmount;
		}
		else if (slot == 1)
		{
			this.input2Amount = inputAmount;
		}
		else if (slot == 2)
		{
			this.input3Amount = inputAmount;
		}
		int num = this.input1Amount + this.input2Amount + this.input3Amount;
		bool flag = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, flag, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 || this.input2Amount > 0 || this.input3Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
		base.UpdateFromInput(num, slot);
	}

	// Token: 0x04000EA5 RID: 3749
	public int input1Amount;

	// Token: 0x04000EA6 RID: 3750
	public int input2Amount;

	// Token: 0x04000EA7 RID: 3751
	public int input3Amount;
}
