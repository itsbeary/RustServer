using System;
using UnityEngine;

// Token: 0x020004D3 RID: 1235
public class ORSwitch : IOEntity
{
	// Token: 0x06002858 RID: 10328 RVA: 0x00007649 File Offset: 0x00005849
	public override bool WantsPassthroughPower()
	{
		return base.IsOn();
	}

	// Token: 0x06002859 RID: 10329 RVA: 0x000FA6B8 File Offset: 0x000F88B8
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int num = Mathf.Max(this.input1Amount, this.input2Amount);
		return Mathf.Max(0, num - this.ConsumptionAmount());
	}

	// Token: 0x0600285A RID: 10330 RVA: 0x000FA6E5 File Offset: 0x000F88E5
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x0600285B RID: 10331 RVA: 0x000582CD File Offset: 0x000564CD
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x0600285C RID: 10332 RVA: 0x000FA70C File Offset: 0x000F890C
	public override void UpdateFromInput(int inputAmount, int slot)
	{
		if (base.IsConnectedTo(this, slot, IOEntity.backtracking, false))
		{
			inputAmount = 0;
		}
		if (slot == 0)
		{
			this.input1Amount = inputAmount;
		}
		else if (slot == 1)
		{
			this.input2Amount = inputAmount;
		}
		int num = this.input1Amount + this.input2Amount;
		bool flag = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, flag, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 || this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
		base.UpdateFromInput(inputAmount, slot);
	}

	// Token: 0x040020AE RID: 8366
	private int input1Amount;

	// Token: 0x040020AF RID: 8367
	private int input2Amount;
}
