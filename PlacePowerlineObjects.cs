using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

// Token: 0x020006E9 RID: 1769
public class PlacePowerlineObjects : ProceduralComponent
{
	// Token: 0x06003247 RID: 12871 RVA: 0x00136114 File Offset: 0x00134314
	public override void Process(uint seed)
	{
		List<PathList> powerlines = TerrainMeta.Path.Powerlines;
		if (World.Networked)
		{
			using (List<PathList>.Enumerator enumerator = powerlines.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PathList pathList = enumerator.Current;
					World.Spawn(pathList.Name, "assets/bundled/prefabs/autospawn/");
				}
				return;
			}
		}
		foreach (PathList pathList2 in powerlines)
		{
			foreach (PathList.BasicObject basicObject in this.Start)
			{
				pathList2.TrimStart(basicObject);
			}
			foreach (PathList.BasicObject basicObject2 in this.End)
			{
				pathList2.TrimEnd(basicObject2);
			}
			foreach (PathList.BasicObject basicObject3 in this.Start)
			{
				pathList2.SpawnStart(ref seed, basicObject3);
			}
			foreach (PathList.BasicObject basicObject4 in this.End)
			{
				pathList2.SpawnEnd(ref seed, basicObject4);
			}
			foreach (PathList.PathObject pathObject in this.Path)
			{
				pathList2.SpawnAlong(ref seed, pathObject);
			}
			foreach (PathList.SideObject sideObject in this.Side)
			{
				pathList2.SpawnSide(ref seed, sideObject);
			}
			pathList2.ResetTrims();
		}
	}

	// Token: 0x04002934 RID: 10548
	public PathList.BasicObject[] Start;

	// Token: 0x04002935 RID: 10549
	public PathList.BasicObject[] End;

	// Token: 0x04002936 RID: 10550
	public PathList.SideObject[] Side;

	// Token: 0x04002937 RID: 10551
	[FormerlySerializedAs("PowerlineObjects")]
	public PathList.PathObject[] Path;
}
