using System;

// Token: 0x02000116 RID: 278
public class BaseDetector : IOEntity
{
	// Token: 0x06001668 RID: 5736 RVA: 0x000AE2F8 File Offset: 0x000AC4F8
	public override int ConsumptionAmount()
	{
		return base.ConsumptionAmount();
	}

	// Token: 0x06001669 RID: 5737 RVA: 0x000AE300 File Offset: 0x000AC500
	public virtual bool ShouldTrigger()
	{
		return this.IsPowered();
	}

	// Token: 0x0600166A RID: 5738 RVA: 0x000AE308 File Offset: 0x000AC508
	public virtual void OnObjects()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		if (this.ShouldTrigger())
		{
			this.OnDetectorTriggered();
			this.MarkDirty();
		}
	}

	// Token: 0x0600166B RID: 5739 RVA: 0x000AE32C File Offset: 0x000AC52C
	public virtual void OnEmpty()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		if (this.ShouldTrigger())
		{
			this.OnDetectorReleased();
			this.MarkDirty();
		}
	}

	// Token: 0x0600166C RID: 5740 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnDetectorTriggered()
	{
	}

	// Token: 0x0600166D RID: 5741 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnDetectorReleased()
	{
	}

	// Token: 0x0600166E RID: 5742 RVA: 0x000AE350 File Offset: 0x000AC550
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			return 0;
		}
		return base.GetPassthroughAmount(0);
	}

	// Token: 0x04000E99 RID: 3737
	public PlayerDetectionTrigger myTrigger;

	// Token: 0x04000E9A RID: 3738
	public const BaseEntity.Flags Flag_HasContents = BaseEntity.Flags.Reserved1;
}
