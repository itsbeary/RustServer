using System;
using UnityEngine;

// Token: 0x020006C0 RID: 1728
public class GenerateCliffTopology : ProceduralComponent
{
	// Token: 0x060031C3 RID: 12739 RVA: 0x00129C18 File Offset: 0x00127E18
	public static void Process(int x, int z)
	{
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float num = topologyMap.Coordinate(z);
		float num2 = topologyMap.Coordinate(x);
		if ((topologyMap.GetTopology(x, z) & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(num2, num);
			float splat = TerrainMeta.SplatMap.GetSplat(num2, num, 8);
			if (slope > 40f || splat > 0.4f)
			{
				topologyMap.AddTopology(x, z, 2);
				return;
			}
			topologyMap.RemoveTopology(x, z, 2);
		}
	}

	// Token: 0x060031C4 RID: 12740 RVA: 0x00129C88 File Offset: 0x00127E88
	private static void Process(int x, int z, bool keepExisting)
	{
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float num = topologyMap.Coordinate(z);
		float num2 = topologyMap.Coordinate(x);
		int topology = topologyMap.GetTopology(x, z);
		if (!World.Procedural || (topology & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(num2, num);
			float splat = TerrainMeta.SplatMap.GetSplat(num2, num, 8);
			if (slope > 40f || splat > 0.4f)
			{
				topologyMap.AddTopology(x, z, 2);
				return;
			}
			if (!keepExisting)
			{
				topologyMap.RemoveTopology(x, z, 2);
			}
		}
	}

	// Token: 0x060031C5 RID: 12741 RVA: 0x00129D08 File Offset: 0x00127F08
	public override void Process(uint seed)
	{
		int[] map = TerrainMeta.TopologyMap.dst;
		int res = TerrainMeta.TopologyMap.res;
		Parallel.For(0, res, delegate(int z)
		{
			for (int i = 0; i < res; i++)
			{
				GenerateCliffTopology.Process(i, z, this.KeepExisting);
			}
		});
		ImageProcessing.Dilate2D(map, res, res, 4194306, 1, delegate(int x, int y)
		{
			if ((map[x * res + y] & 2) == 0)
			{
				map[x * res + y] |= 4194304;
			}
		});
	}

	// Token: 0x04002866 RID: 10342
	public bool KeepExisting = true;

	// Token: 0x04002867 RID: 10343
	private const int filter = 8389632;
}
