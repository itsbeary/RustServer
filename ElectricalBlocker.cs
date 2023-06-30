using System;

// Token: 0x0200011C RID: 284
public class ElectricalBlocker : IOEntity
{
	// Token: 0x06001686 RID: 5766 RVA: 0x000AE6D7 File Offset: 0x000AC8D7
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return base.GetPassthroughAmount(outputSlot) * (base.IsOn() ? 0 : 1);
	}

	// Token: 0x06001687 RID: 5767 RVA: 0x00050C19 File Offset: 0x0004EE19
	public override bool WantsPower()
	{
		return !base.IsOn();
	}

	// Token: 0x06001688 RID: 5768 RVA: 0x000AE6ED File Offset: 0x000AC8ED
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x06001689 RID: 5769 RVA: 0x000582CD File Offset: 0x000564CD
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x0600168A RID: 5770 RVA: 0x000AE71C File Offset: 0x000AC91C
	public virtual void UpdateBlocked()
	{
		bool flag = base.IsOn();
		base.SetFlag(BaseEntity.Flags.On, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved8, base.IsOn(), false, false);
		this.UpdateHasPower(this.input1Amount + this.input2Amount, 1);
		if (flag != base.IsOn())
		{
			this.MarkDirty();
		}
	}

	// Token: 0x0600168B RID: 5771 RVA: 0x000AE776 File Offset: 0x000AC976
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1)
		{
			this.input1Amount = inputAmount;
			this.UpdateBlocked();
			return;
		}
		if (inputSlot == 0)
		{
			this.input2Amount = inputAmount;
			base.UpdateFromInput(inputAmount, inputSlot);
		}
	}

	// Token: 0x04000EA3 RID: 3747
	protected int input1Amount;

	// Token: 0x04000EA4 RID: 3748
	protected int input2Amount;
}
