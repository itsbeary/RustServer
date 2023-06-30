using System;

// Token: 0x020006DF RID: 1759
public class GenerateWireMeshes : ProceduralComponent
{
	// Token: 0x06003228 RID: 12840 RVA: 0x001329F3 File Offset: 0x00130BF3
	public override void Process(uint seed)
	{
		TerrainMeta.Path.CreateWires();
	}

	// Token: 0x17000413 RID: 1043
	// (get) Token: 0x06003229 RID: 12841 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
