using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class ElectricalDFlipFlop : IOEntity
{
	// Token: 0x06001693 RID: 5779 RVA: 0x00087D4D File Offset: 0x00085F4D
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
		}
	}

	// Token: 0x06001694 RID: 5780 RVA: 0x000AE8EC File Offset: 0x000ACAEC
	public bool GetDesiredState()
	{
		if (this.setAmount > 0 && this.resetAmount == 0)
		{
			return true;
		}
		if (this.setAmount > 0 && this.resetAmount > 0)
		{
			return true;
		}
		if (this.setAmount == 0 && this.resetAmount > 0)
		{
			return false;
		}
		if (this.toggleAmount > 0)
		{
			return !base.IsOn();
		}
		return this.setAmount == 0 && this.resetAmount == 0 && base.IsOn();
	}

	// Token: 0x06001695 RID: 5781 RVA: 0x000AE960 File Offset: 0x000ACB60
	public void UpdateState()
	{
		if (this.IsPowered())
		{
			bool flag = base.IsOn();
			bool desiredState = this.GetDesiredState();
			base.SetFlag(BaseEntity.Flags.On, desiredState, false, true);
			if (flag != base.IsOn())
			{
				base.MarkDirtyForceUpdateOutputs();
			}
		}
	}

	// Token: 0x06001696 RID: 5782 RVA: 0x000AE99C File Offset: 0x000ACB9C
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1)
		{
			this.setAmount = inputAmount;
			this.UpdateState();
			return;
		}
		if (inputSlot == 2)
		{
			this.resetAmount = inputAmount;
			this.UpdateState();
			return;
		}
		if (inputSlot == 3)
		{
			this.toggleAmount = inputAmount;
			this.UpdateState();
			return;
		}
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
			this.UpdateState();
		}
	}

	// Token: 0x06001697 RID: 5783 RVA: 0x000AE9F0 File Offset: 0x000ACBF0
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x06001698 RID: 5784 RVA: 0x000AE9FC File Offset: 0x000ACBFC
	public override void UpdateOutputs()
	{
		if (!base.ShouldUpdateOutputs())
		{
			return;
		}
		if (this.ensureOutputsUpdated)
		{
			int num = Mathf.Max(0, this.currentEnergy - 1);
			if (this.outputs[0].connectedTo.Get(true) != null)
			{
				this.outputs[0].connectedTo.Get(true).UpdateFromInput(base.IsOn() ? num : 0, this.outputs[0].connectedToSlot);
			}
			if (this.outputs[1].connectedTo.Get(true) != null)
			{
				this.outputs[1].connectedTo.Get(true).UpdateFromInput(base.IsOn() ? 0 : num, this.outputs[1].connectedToSlot);
			}
		}
	}

	// Token: 0x04000EA8 RID: 3752
	[NonSerialized]
	private int setAmount;

	// Token: 0x04000EA9 RID: 3753
	[NonSerialized]
	private int resetAmount;

	// Token: 0x04000EAA RID: 3754
	[NonSerialized]
	private int toggleAmount;
}
