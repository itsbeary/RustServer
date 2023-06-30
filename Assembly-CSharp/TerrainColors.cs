using System;
using UnityEngine;

// Token: 0x020006A0 RID: 1696
public class TerrainColors : TerrainExtension
{
	// Token: 0x06003044 RID: 12356 RVA: 0x00122218 File Offset: 0x00120418
	public override void Setup()
	{
		this.splatMap = this.terrain.GetComponent<TerrainSplatMap>();
		this.biomeMap = this.terrain.GetComponent<TerrainBiomeMap>();
	}

	// Token: 0x06003045 RID: 12357 RVA: 0x0012223C File Offset: 0x0012043C
	public Color GetColor(Vector3 worldPos, int mask = -1)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetColor(num, num2, mask);
	}

	// Token: 0x06003046 RID: 12358 RVA: 0x0012226C File Offset: 0x0012046C
	public Color GetColor(float normX, float normZ, int mask = -1)
	{
		float biome = this.biomeMap.GetBiome(normX, normZ, 1);
		float biome2 = this.biomeMap.GetBiome(normX, normZ, 2);
		float biome3 = this.biomeMap.GetBiome(normX, normZ, 4);
		float biome4 = this.biomeMap.GetBiome(normX, normZ, 8);
		int num = TerrainSplat.TypeToIndex(this.splatMap.GetSplatMaxType(normX, normZ, mask));
		TerrainConfig.SplatType splatType = this.config.Splats[num];
		return biome * splatType.AridColor + biome2 * splatType.TemperateColor + biome3 * splatType.TundraColor + biome4 * splatType.ArcticColor;
	}

	// Token: 0x040027F8 RID: 10232
	private TerrainSplatMap splatMap;

	// Token: 0x040027F9 RID: 10233
	private TerrainBiomeMap biomeMap;
}
