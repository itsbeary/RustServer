using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

// Token: 0x020006EB RID: 1771
public class PlaceRoadObjects : ProceduralComponent
{
	// Token: 0x0600324B RID: 12875 RVA: 0x0013648C File Offset: 0x0013468C
	public override void Process(uint seed)
	{
		List<PathList> roads = TerrainMeta.Path.Roads;
		if (World.Networked)
		{
			using (List<PathList>.Enumerator enumerator = roads.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PathList pathList = enumerator.Current;
					World.Spawn(pathList.Name, "assets/bundled/prefabs/autospawn/");
				}
				return;
			}
		}
		foreach (PathList pathList2 in roads)
		{
			if (pathList2.Hierarchy < 2)
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
	}

	// Token: 0x0400293C RID: 10556
	public PathList.BasicObject[] Start;

	// Token: 0x0400293D RID: 10557
	public PathList.BasicObject[] End;

	// Token: 0x0400293E RID: 10558
	[FormerlySerializedAs("RoadsideObjects")]
	public PathList.SideObject[] Side;

	// Token: 0x0400293F RID: 10559
	[FormerlySerializedAs("RoadObjects")]
	public PathList.PathObject[] Path;
}
