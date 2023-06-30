using System;

// Token: 0x02000119 RID: 281
public class PressurePad : BaseDetector
{
	// Token: 0x06001676 RID: 5750 RVA: 0x0000441C File Offset: 0x0000261C
	public override int ConsumptionAmount()
	{
		return 1;
	}

	// Token: 0x06001677 RID: 5751 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06001678 RID: 5752 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldTrigger()
	{
		return true;
	}

	// Token: 0x06001679 RID: 5753 RVA: 0x000AE465 File Offset: 0x000AC665
	public override void OnDetectorTriggered()
	{
		base.OnDetectorTriggered();
		base.Invoke(new Action(this.UnpowerTime), this.pressPowerTime);
		base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
	}

	// Token: 0x0600167A RID: 5754 RVA: 0x000AE493 File Offset: 0x000AC693
	public override void OnDetectorReleased()
	{
		base.OnDetectorReleased();
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
	}

	// Token: 0x0600167B RID: 5755 RVA: 0x00088B46 File Offset: 0x00086D46
	public void UnpowerTime()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x0600167C RID: 5756 RVA: 0x000AE4A9 File Offset: 0x000AC6A9
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			if (base.HasFlag(BaseEntity.Flags.Reserved3))
			{
				return this.pressPowerAmount;
			}
			if (this.IsPowered())
			{
				return base.GetPassthroughAmount(0);
			}
		}
		return 0;
	}

	// Token: 0x04000E9C RID: 3740
	public float pressPowerTime = 0.5f;

	// Token: 0x04000E9D RID: 3741
	public int pressPowerAmount = 2;

	// Token: 0x04000E9E RID: 3742
	public const BaseEntity.Flags Flag_EmittingPower = BaseEntity.Flags.Reserved3;
}
