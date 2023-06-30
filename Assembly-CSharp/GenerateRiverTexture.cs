using System;
using System.Linq;

// Token: 0x020006D3 RID: 1747
public class GenerateRiverTexture : ProceduralComponent
{
	// Token: 0x06003208 RID: 12808 RVA: 0x00130EC8 File Offset: 0x0012F0C8
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rivers.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTexture();
		}
	}
}
