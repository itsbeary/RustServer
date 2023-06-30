using System;
using System.Linq;
using UnityEngine;

// Token: 0x020006D2 RID: 1746
public class GenerateRiverTerrain : ProceduralComponent
{
	// Token: 0x06003206 RID: 12806 RVA: 0x00130E08 File Offset: 0x0012F008
	public override void Process(uint seed)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		for (int i = 0; i < 1; i++)
		{
			foreach (PathList pathList in TerrainMeta.Path.Rivers.AsEnumerable<PathList>().Reverse<PathList>())
			{
				if (!World.Networked)
				{
					PathInterpolator path = pathList.Path;
					path.Smoothen(8, Vector3.up, null);
					path.RecalculateTangents();
				}
				heightMap.Push();
				float num = 1f;
				float num2 = 1f / (1f + (float)i / 3f);
				pathList.AdjustTerrainHeight(num, num2);
				heightMap.Pop();
			}
		}
	}

	// Token: 0x040028C4 RID: 10436
	public const int SmoothenLoops = 1;

	// Token: 0x040028C5 RID: 10437
	public const int SmoothenIterations = 8;

	// Token: 0x040028C6 RID: 10438
	public const int SmoothenY = 8;

	// Token: 0x040028C7 RID: 10439
	public const int SmoothenXZ = 4;
}
