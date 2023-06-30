using System;
using UnityEngine;

// Token: 0x020004C9 RID: 1225
public class ANDSwitch : IOEntity
{
	// Token: 0x0600280D RID: 10253 RVA: 0x000F99B9 File Offset: 0x000F7BB9
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.input1Amount <= 0 || this.input2Amount <= 0)
		{
			return 0;
		}
		return Mathf.Max(this.input1Amount, this.input2Amount);
	}

	// Token: 0x0600280E RID: 10254 RVA: 0x000F99E0 File Offset: 0x000F7BE0
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x0600280F RID: 10255 RVA: 0x000F9A04 File Offset: 0x000F7C04
	public override void UpdateFromInput(int inputAmount, int slot)
	{
		if (slot == 0)
		{
			this.input1Amount = inputAmount;
		}
		else if (slot == 1)
		{
			this.input2Amount = inputAmount;
		}
		int num = ((this.input1Amount > 0 && this.input2Amount > 0) ? (this.input1Amount + this.input2Amount) : 0);
		bool flag = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, flag, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 && this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
		base.UpdateFromInput(inputAmount, slot);
	}

	// Token: 0x0400209B RID: 8347
	private int input1Amount;

	// Token: 0x0400209C RID: 8348
	private int input2Amount;
}
