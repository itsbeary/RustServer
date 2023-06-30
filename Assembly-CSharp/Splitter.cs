using System;

// Token: 0x020004D6 RID: 1238
public class Splitter : IOEntity
{
	// Token: 0x1700036B RID: 875
	// (get) Token: 0x06002862 RID: 10338 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool BlockFluidDraining
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002863 RID: 10339 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06002864 RID: 10340 RVA: 0x000FA7E1 File Offset: 0x000F89E1
	public override void OnCircuitChanged(bool forceUpdate)
	{
		base.MarkDirtyForceUpdateOutputs();
	}
}
