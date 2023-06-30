using System;

// Token: 0x020006DD RID: 1757
public class GenerateTextures : ProceduralComponent
{
	// Token: 0x06003222 RID: 12834 RVA: 0x001328D0 File Offset: 0x00130AD0
	public override void Process(uint seed)
	{
		if (!World.Cached)
		{
			World.AddMap("height", TerrainMeta.HeightMap.ToByteArray());
			World.AddMap("splat", TerrainMeta.SplatMap.ToByteArray());
			World.AddMap("biome", TerrainMeta.BiomeMap.ToByteArray());
			World.AddMap("topology", TerrainMeta.TopologyMap.ToByteArray());
			World.AddMap("alpha", TerrainMeta.AlphaMap.ToByteArray());
			World.AddMap("water", TerrainMeta.WaterMap.ToByteArray());
			return;
		}
		TerrainMeta.HeightMap.FromByteArray(World.GetMap("height"));
	}

	// Token: 0x17000412 RID: 1042
	// (get) Token: 0x06003223 RID: 12835 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
