using System;

// Token: 0x02000127 RID: 295
public class CableTunnel : IOEntity
{
	// Token: 0x060016BF RID: 5823 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool WantsPower()
	{
		return true;
	}

	// Token: 0x060016C0 RID: 5824 RVA: 0x000AF4C0 File Offset: 0x000AD6C0
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		int num = this.inputAmounts[inputSlot];
		this.inputAmounts[inputSlot] = inputAmount;
		if (inputAmount != num)
		{
			this.ensureOutputsUpdated = true;
		}
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x060016C1 RID: 5825 RVA: 0x000AF4F4 File Offset: 0x000AD6F4
	public override void UpdateOutputs()
	{
		if (!base.ShouldUpdateOutputs())
		{
			return;
		}
		if (this.ensureOutputsUpdated)
		{
			for (int i = 0; i < 4; i++)
			{
				IOEntity.IOSlot ioslot = this.outputs[i];
				if (ioslot.connectedTo.Get(true) != null)
				{
					ioslot.connectedTo.Get(true).UpdateFromInput(this.inputAmounts[i], ioslot.connectedToSlot);
				}
			}
		}
	}

	// Token: 0x04000ED4 RID: 3796
	private const int numChannels = 4;

	// Token: 0x04000ED5 RID: 3797
	private int[] inputAmounts = new int[4];
}
