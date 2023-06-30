using System;
using System.Linq;

// Token: 0x020006D4 RID: 1748
public class GenerateRiverTopology : ProceduralComponent
{
	// Token: 0x0600320A RID: 12810 RVA: 0x00130F20 File Offset: 0x0012F120
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rivers.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTopology();
		}
		this.MarkRiverside();
	}

	// Token: 0x0600320B RID: 12811 RVA: 0x00130F80 File Offset: 0x0012F180
	public void MarkRiverside()
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int[] map = topomap.dst;
		int res = topomap.res;
		ImageProcessing.Dilate2D(map, res, res, 49152, 6, delegate(int x, int y)
		{
			if ((map[x * res + y] & 49) != 0)
			{
				map[x * res + y] |= 32768;
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
