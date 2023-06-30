using System;

// Token: 0x020003A5 RID: 933
public class MicrophoneStandIOEntity : IOEntity, IAudioConnectionSource
{
	// Token: 0x060020CE RID: 8398 RVA: 0x000D89BC File Offset: 0x000D6BBC
	public override int DesiredPower()
	{
		return this.PowerCost;
	}

	// Token: 0x060020CF RID: 8399 RVA: 0x000D89C4 File Offset: 0x000D6BC4
	public override int MaximalPowerOutput()
	{
		if (this.IsStatic)
		{
			return 100;
		}
		return base.MaximalPowerOutput();
	}

	// Token: 0x060020D0 RID: 8400 RVA: 0x000D89D7 File Offset: 0x000D6BD7
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (this.IsStatic)
		{
			return 100;
		}
		return base.CalculateCurrentEnergy(inputAmount, inputSlot);
	}

	// Token: 0x060020D1 RID: 8401 RVA: 0x000D89EC File Offset: 0x000D6BEC
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.IsStatic)
		{
			return 100;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x060020D2 RID: 8402 RVA: 0x000D8A00 File Offset: 0x000D6C00
	public override bool IsRootEntity()
	{
		return this.IsStatic || base.IsRootEntity();
	}

	// Token: 0x060020D3 RID: 8403 RVA: 0x000037E7 File Offset: 0x000019E7
	public IOEntity ToEntity()
	{
		return this;
	}

	// Token: 0x040019BE RID: 6590
	public int PowerCost = 5;

	// Token: 0x040019BF RID: 6591
	public TriggerBase InstrumentTrigger;

	// Token: 0x040019C0 RID: 6592
	public bool IsStatic;
}
