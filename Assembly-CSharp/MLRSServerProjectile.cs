using System;

// Token: 0x02000490 RID: 1168
public class MLRSServerProjectile : ServerProjectile
{
	// Token: 0x17000322 RID: 802
	// (get) Token: 0x0600268C RID: 9868 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool HasRangeLimit
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000323 RID: 803
	// (get) Token: 0x0600268D RID: 9869 RVA: 0x000F2FF7 File Offset: 0x000F11F7
	protected override int mask
	{
		get
		{
			return 1235954449;
		}
	}

	// Token: 0x0600268E RID: 9870 RVA: 0x000F2FFE File Offset: 0x000F11FE
	protected override bool IsAValidHit(BaseEntity hitEnt)
	{
		return base.IsAValidHit(hitEnt) && (!hitEnt.IsValid() || !(hitEnt is MLRS));
	}
}
