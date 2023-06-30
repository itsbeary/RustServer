using System;

// Token: 0x020006C1 RID: 1729
public class GenerateClutterTopology : ProceduralComponent
{
	// Token: 0x060031C7 RID: 12743 RVA: 0x00129D94 File Offset: 0x00127F94
	public override void Process(uint seed)
	{
		int[] map = TerrainMeta.TopologyMap.dst;
		int res = TerrainMeta.TopologyMap.res;
		ImageProcessing.Dilate2D(map, res, res, 16777728, 3, delegate(int x, int y)
		{
			if ((map[x * res + y] & 512) == 0)
			{
				map[x * res + y] |= 16777216;
			}
		});
	}
}
