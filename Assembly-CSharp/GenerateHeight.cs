using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020006C5 RID: 1733
public class GenerateHeight : ProceduralComponent
{
	// Token: 0x060031DF RID: 12767
	[DllImport("RustNative", EntryPoint = "generate_height")]
	public static extern void Native_GenerateHeight(short[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle);

	// Token: 0x060031E0 RID: 12768 RVA: 0x0012D9FC File Offset: 0x0012BBFC
	public override void Process(uint seed)
	{
		short[] dst = TerrainMeta.HeightMap.dst;
		int res = TerrainMeta.HeightMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		GenerateHeight.Native_GenerateHeight(dst, res, position, size, seed, lootAxisAngle, biomeAxisAngle);
	}
}
