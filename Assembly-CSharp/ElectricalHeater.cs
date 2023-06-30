using System;
using UnityEngine;

// Token: 0x0200011A RID: 282
public class ElectricalHeater : IOEntity
{
	// Token: 0x0600167E RID: 5758 RVA: 0x000037BE File Offset: 0x000019BE
	public override int ConsumptionAmount()
	{
		return 3;
	}

	// Token: 0x0600167F RID: 5759 RVA: 0x00025634 File Offset: 0x00023834
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x06001680 RID: 5760 RVA: 0x000AE4F8 File Offset: 0x000AC6F8
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = next.HasFlag(BaseEntity.Flags.Reserved8);
		if (old.HasFlag(BaseEntity.Flags.Reserved8) != flag && this.growableHeatSource != null)
		{
			this.growableHeatSource.ForceUpdateGrowablesInRange();
		}
	}

	// Token: 0x06001681 RID: 5761 RVA: 0x000AE554 File Offset: 0x000AC754
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		if (this.growableHeatSource != null)
		{
			this.growableHeatSource.ForceUpdateGrowablesInRange();
		}
	}

	// Token: 0x04000E9F RID: 3743
	public float fadeDuration = 1f;

	// Token: 0x04000EA0 RID: 3744
	public Light sourceLight;

	// Token: 0x04000EA1 RID: 3745
	public GrowableHeatSource growableHeatSource;
}
