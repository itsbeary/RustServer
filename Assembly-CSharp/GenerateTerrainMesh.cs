using System;

// Token: 0x020006DC RID: 1756
public class GenerateTerrainMesh : ProceduralComponent
{
	// Token: 0x0600321F RID: 12831 RVA: 0x001328A7 File Offset: 0x00130AA7
	public override void Process(uint seed)
	{
		if (!World.Cached)
		{
			World.AddMap("terrain", TerrainMeta.HeightMap.ToByteArray());
		}
		TerrainMeta.HeightMap.ApplyToTerrain();
	}

	// Token: 0x17000411 RID: 1041
	// (get) Token: 0x06003220 RID: 12832 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
