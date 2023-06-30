using System;
using System.Linq;

// Token: 0x020006CE RID: 1742
public class GenerateRailTexture : ProceduralComponent
{
	// Token: 0x060031FC RID: 12796 RVA: 0x00130538 File Offset: 0x0012E738
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rails.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTexture();
		}
	}
}
