using System;
using UnityEngine;

// Token: 0x020006BF RID: 1727
public class GenerateCliffSplat : ProceduralComponent
{
	// Token: 0x060031C0 RID: 12736 RVA: 0x00129B70 File Offset: 0x00127D70
	public static void Process(int x, int z)
	{
		TerrainSplatMap splatMap = TerrainMeta.SplatMap;
		float num = splatMap.Coordinate(z);
		float num2 = splatMap.Coordinate(x);
		if ((TerrainMeta.TopologyMap.GetTopology(num2, num) & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(num2, num);
			if (slope > 30f)
			{
				splatMap.SetSplat(x, z, 8, Mathf.InverseLerp(30f, 50f, slope));
			}
		}
	}

	// Token: 0x060031C1 RID: 12737 RVA: 0x00129BD8 File Offset: 0x00127DD8
	public override void Process(uint seed)
	{
		TerrainSplatMap splatMap = TerrainMeta.SplatMap;
		int splatres = splatMap.res;
		Parallel.For(0, splatres, delegate(int z)
		{
			for (int i = 0; i < splatres; i++)
			{
				GenerateCliffSplat.Process(i, z);
			}
		});
	}

	// Token: 0x04002865 RID: 10341
	private const int filter = 8389632;
}
