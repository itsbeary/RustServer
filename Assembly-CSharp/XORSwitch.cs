using System;
using UnityEngine;

// Token: 0x020004D7 RID: 1239
public class XORSwitch : IOEntity
{
	// Token: 0x06002866 RID: 10342 RVA: 0x000FA7E9 File Offset: 0x000F89E9
	public override void ResetState()
	{
		base.ResetState();
		this.firstRun = true;
	}

	// Token: 0x06002867 RID: 10343 RVA: 0x000FA7F8 File Offset: 0x000F89F8
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.input1Amount > 0 && this.input2Amount > 0)
		{
			return 0;
		}
		int num = Mathf.Max(this.input1Amount, this.input2Amount);
		return Mathf.Max(0, num - this.ConsumptionAmount());
	}

	// Token: 0x06002868 RID: 10344 RVA: 0x000FA839 File Offset: 0x000F8A39
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x06002869 RID: 10345 RVA: 0x000582CD File Offset: 0x000564CD
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x0600286A RID: 10346 RVA: 0x000FA860 File Offset: 0x000F8A60
	public override void UpdateFromInput(int inputAmount, int slot)
	{
		if (inputAmount > 0 && base.IsConnectedTo(this, slot, IOEntity.backtracking, false))
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
		if (this.firstRun)
		{
			if (!base.IsInvoking(new Action(this.UpdateFlags)))
			{
				base.Invoke(new Action(this.UpdateFlags), 0.1f);
			}
		}
		else
		{
			this.UpdateFlags();
		}
		this.firstRun = false;
		base.UpdateFromInput(inputAmount, slot);
	}

	// Token: 0x0600286B RID: 10347 RVA: 0x000FA904 File Offset: 0x000F8B04
	private void UpdateFlags()
	{
		int num = ((this.input1Amount > 0 && this.input2Amount > 0) ? 0 : Mathf.Max(this.input1Amount, this.input2Amount));
		bool flag = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, flag, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 || this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
	}

	// Token: 0x040020B0 RID: 8368
	private int input1Amount;

	// Token: 0x040020B1 RID: 8369
	private int input2Amount;

	// Token: 0x040020B2 RID: 8370
	private bool firstRun = true;
}
