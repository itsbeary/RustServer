using System;

// Token: 0x020006EE RID: 1774
public class ResetPlacementMap : ProceduralComponent
{
	// Token: 0x06003252 RID: 12882 RVA: 0x001366E4 File Offset: 0x001348E4
	public override void Process(uint seed)
	{
		TerrainMeta.PlacementMap.Reset();
	}
}
