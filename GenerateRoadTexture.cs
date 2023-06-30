using System;
using System.Linq;

// Token: 0x020006D9 RID: 1753
public class GenerateRoadTexture : ProceduralComponent
{
	// Token: 0x06003217 RID: 12823 RVA: 0x001326D8 File Offset: 0x001308D8
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Roads.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTexture();
		}
	}
}
