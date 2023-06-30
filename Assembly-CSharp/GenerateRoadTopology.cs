using System;
using System.Linq;

// Token: 0x020006DA RID: 1754
public class GenerateRoadTopology : ProceduralComponent
{
	// Token: 0x06003219 RID: 12825 RVA: 0x00132730 File Offset: 0x00130930
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Roads.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTopology();
		}
		this.MarkRoadside();
		TerrainMeta.PlacementMap.Reset();
	}

	// Token: 0x0600321A RID: 12826 RVA: 0x00132798 File Offset: 0x00130998
	private void MarkRoadside()
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int[] map = topomap.dst;
		int res = topomap.res;
		ImageProcessing.Dilate2D(map, res, res, 6144, 6, delegate(int x, int y)
		{
			if ((map[x * res + y] & 49) != 0)
			{
				map[x * res + y] |= 4096;
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
