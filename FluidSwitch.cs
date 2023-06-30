using System;
using UnityEngine;

// Token: 0x020004D1 RID: 1233
public class FluidSwitch : ElectricSwitch
{
	// Token: 0x0600284C RID: 10316 RVA: 0x00025634 File Offset: 0x00023834
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x0600284D RID: 10317 RVA: 0x000FA5B4 File Offset: 0x000F87B4
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1 && this.lastToggleInput != inputAmount)
		{
			this.lastToggleInput = inputAmount;
			this.SetSwitch(inputAmount > 0);
		}
		if (inputSlot == 2)
		{
			bool flag = this.pumpEnabled;
			this.pumpEnabled = inputAmount > 0;
			if (flag != this.pumpEnabled)
			{
				this.lastPassthroughEnergy = -1;
				base.SetFlag(this.Flag_PumpPowered, this.pumpEnabled, false, true);
				this.SendChangedToRoot(true);
			}
		}
	}

	// Token: 0x0600284E RID: 10318 RVA: 0x000FA61D File Offset: 0x000F881D
	public override void SetSwitch(bool wantsOn)
	{
		base.SetSwitch(wantsOn);
		base.Invoke(new Action(this.DelayedSendChanged), IOEntity.responsetime * 2f);
	}

	// Token: 0x0600284F RID: 10319 RVA: 0x000FA643 File Offset: 0x000F8843
	private void DelayedSendChanged()
	{
		this.SendChangedToRoot(true);
	}

	// Token: 0x06002850 RID: 10320 RVA: 0x000FA64C File Offset: 0x000F884C
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x06002851 RID: 10321 RVA: 0x00007A44 File Offset: 0x00005C44
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x17000369 RID: 873
	// (get) Token: 0x06002852 RID: 10322 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsGravitySource
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700036A RID: 874
	// (get) Token: 0x06002853 RID: 10323 RVA: 0x000FA663 File Offset: 0x000F8863
	protected override bool DisregardGravityRestrictionsOnLiquid
	{
		get
		{
			return base.HasFlag(this.Flag_PumpPowered);
		}
	}

	// Token: 0x06002854 RID: 10324 RVA: 0x000FA671 File Offset: 0x000F8871
	public override bool AllowLiquidPassthrough(IOEntity fromSource, Vector3 sourceWorldPosition, bool forPlacement = false)
	{
		return (forPlacement || base.IsOn()) && base.AllowLiquidPassthrough(fromSource, sourceWorldPosition, false);
	}

	// Token: 0x040020AA RID: 8362
	private BaseEntity.Flags Flag_PumpPowered = BaseEntity.Flags.Reserved6;

	// Token: 0x040020AB RID: 8363
	public Animator PumpAnimator;

	// Token: 0x040020AC RID: 8364
	private bool pumpEnabled;

	// Token: 0x040020AD RID: 8365
	private int lastToggleInput;
}
