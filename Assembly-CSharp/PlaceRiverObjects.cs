using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

// Token: 0x020006EA RID: 1770
public class PlaceRiverObjects : ProceduralComponent
{
	// Token: 0x06003249 RID: 12873 RVA: 0x001362D0 File Offset: 0x001344D0
	public override void Process(uint seed)
	{
		List<PathList> rivers = TerrainMeta.Path.Rivers;
		if (World.Networked)
		{
			using (List<PathList>.Enumerator enumerator = rivers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PathList pathList = enumerator.Current;
					World.Spawn(pathList.Name, "assets/bundled/prefabs/autospawn/");
				}
				return;
			}
		}
		foreach (PathList pathList2 in rivers)
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
			foreach (PathList.PathObject pathObject in this.Path)
			{
				pathList2.SpawnAlong(ref seed, pathObject);
			}
			foreach (PathList.SideObject sideObject in this.Side)
			{
				pathList2.SpawnSide(ref seed, sideObject);
			}
			foreach (PathList.BasicObject basicObject4 in this.End)
			{
				pathList2.SpawnEnd(ref seed, basicObject4);
			}
			pathList2.ResetTrims();
		}
	}

	// Token: 0x04002938 RID: 10552
	public PathList.BasicObject[] Start;

	// Token: 0x04002939 RID: 10553
	public PathList.BasicObject[] End;

	// Token: 0x0400293A RID: 10554
	[FormerlySerializedAs("RiversideObjects")]
	public PathList.SideObject[] Side;

	// Token: 0x0400293B RID: 10555
	[FormerlySerializedAs("RiverObjects")]
	public PathList.PathObject[] Path;
}
