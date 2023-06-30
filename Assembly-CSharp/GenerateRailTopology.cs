using System;
using System.Linq;

// Token: 0x020006CF RID: 1743
public class GenerateRailTopology : ProceduralComponent
{
	// Token: 0x060031FE RID: 12798 RVA: 0x00130590 File Offset: 0x0012E790
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rails.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTopology();
		}
		this.MarkRailside();
		TerrainMeta.PlacementMap.Reset();
	}

	// Token: 0x060031FF RID: 12799 RVA: 0x001305F8 File Offset: 0x0012E7F8
	private void MarkRailside()
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int[] map = topomap.dst;
		int res = topomap.res;
		ImageProcessing.Dilate2D(map, res, res, 1572864, 6, delegate(int x, int y)
		{
			if ((map[x * res + y] & 49) != 0)
			{
				map[x * res + y] |= 1048576;
			}
			float num = topomap.Coordinate(x);
			float num2 = topomap.Coordinate(y);
			if (heightmap.GetSlope(num, num2) > 40f)
			{
				map[x * res + y] |= 2;
			}
		});
	}
}
