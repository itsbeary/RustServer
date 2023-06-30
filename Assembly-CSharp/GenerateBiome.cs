using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020006BE RID: 1726
public class GenerateBiome : ProceduralComponent
{
	// Token: 0x060031BD RID: 12733
	[DllImport("RustNative", EntryPoint = "generate_biome")]
	public static extern void Native_GenerateBiome(byte[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle, short[] heightmap, int heightres);

	// Token: 0x060031BE RID: 12734 RVA: 0x00129B0C File Offset: 0x00127D0C
	public override void Process(uint seed)
	{
		byte[] dst = TerrainMeta.BiomeMap.dst;
		int res = TerrainMeta.BiomeMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		short[] src = TerrainMeta.HeightMap.src;
		int res2 = TerrainMeta.HeightMap.res;
		GenerateBiome.Native_GenerateBiome(dst, res, position, size, seed, lootAxisAngle, biomeAxisAngle, src, res2);
	}
}
