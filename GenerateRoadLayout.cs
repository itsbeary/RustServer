using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020006D5 RID: 1749
public class GenerateRoadLayout : ProceduralComponent
{
	// Token: 0x0600320D RID: 12813 RVA: 0x00130FF4 File Offset: 0x0012F1F4
	private PathList CreateSegment(int number, Vector3[] points)
	{
		PathList pathList = new PathList("Road " + number, points);
		if (this.RoadType == InfrastructureType.Road)
		{
			pathList.Spline = true;
			pathList.Width = 10f;
			pathList.InnerPadding = 1f;
			pathList.OuterPadding = 1f;
			pathList.InnerFade = 1f;
			pathList.OuterFade = 8f;
			pathList.RandomScale = 0.75f;
			pathList.MeshOffset = 0f;
			pathList.TerrainOffset = -0.125f;
			pathList.Topology = 2048;
			pathList.Splat = 128;
			pathList.Hierarchy = 1;
		}
		else
		{
			float num = 0.4f;
			pathList.Spline = true;
			pathList.Width = 4f;
			pathList.InnerPadding = 1f * num;
			pathList.OuterPadding = 1f;
			pathList.InnerFade = 1f;
			pathList.OuterFade = 8f;
			pathList.RandomScale = 0.75f;
			pathList.MeshOffset = 0f;
			pathList.TerrainOffset = -0.125f;
			pathList.Topology = 2048;
			pathList.Splat = 1;
			pathList.Hierarchy = 2;
		}
		return pathList;
	}

	// Token: 0x0600320E RID: 12814 RVA: 0x00131128 File Offset: 0x0012F328
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			TerrainMeta.Path.Roads.Clear();
			TerrainMeta.Path.Roads.AddRange(World.GetPaths("Road"));
			return;
		}
		List<PathList> list = new List<PathList>();
		int[,] array = TerrainPath.CreateRoadCostmap(ref seed);
		PathFinder pathFinder = new PathFinder(array, true, true);
		int length = array.GetLength(0);
		List<GenerateRoadLayout.PathSegment> list2 = new List<GenerateRoadLayout.PathSegment>();
		List<GenerateRoadLayout.PathNode> list3 = new List<GenerateRoadLayout.PathNode>();
		List<GenerateRoadLayout.PathNode> list4 = new List<GenerateRoadLayout.PathNode>();
		List<GenerateRoadLayout.PathNode> list5 = new List<GenerateRoadLayout.PathNode>();
		List<PathFinder.Point> list6 = new List<PathFinder.Point>();
		List<PathFinder.Point> list7 = new List<PathFinder.Point>();
		List<PathFinder.Point> list8 = new List<PathFinder.Point>();
		foreach (PathList pathList in TerrainMeta.Path.Roads)
		{
			if (pathList.ProcgenStartNode != null && pathList.ProcgenEndNode != null)
			{
				int num = 1;
				for (PathFinder.Node node9 = pathList.ProcgenStartNode; node9 != null; node9 = node9.next)
				{
					if (num % 8 == 0)
					{
						list6.Add(node9.point);
					}
					num++;
				}
			}
		}
		using (List<MonumentInfo>.Enumerator enumerator2 = TerrainMeta.Path.Monuments.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				MonumentInfo monumentInfo = enumerator2.Current;
				if (monumentInfo.Type != MonumentType.Roadside)
				{
					foreach (TerrainPathConnect terrainPathConnect in monumentInfo.GetComponentsInChildren<TerrainPathConnect>(true))
					{
						if (terrainPathConnect.Type == this.RoadType)
						{
							PathFinder.Point pathFinderPoint = terrainPathConnect.GetPathFinderPoint(length);
							PathFinder.Node node2 = pathFinder.FindClosestWalkable(pathFinderPoint, 100000);
							if (node2 != null)
							{
								list4.Add(new GenerateRoadLayout.PathNode
								{
									monument = monumentInfo,
									target = terrainPathConnect,
									node = node2
								});
							}
						}
					}
				}
			}
			goto IL_492;
		}
		IL_1BC:
		if (list4.Count == 0)
		{
			GenerateRoadLayout.PathNode node10 = list5[0];
			list4.AddRange(list5.Where((GenerateRoadLayout.PathNode x) => x.monument == node10.monument));
			list5.RemoveAll((GenerateRoadLayout.PathNode x) => x.monument == node10.monument);
			pathFinder.PushPoint = node10.monument.GetPathFinderPoint(length);
			pathFinder.PushRadius = node10.monument.GetPathFinderRadius(length);
			pathFinder.PushDistance = 40;
			pathFinder.PushMultiplier = 1;
		}
		list8.Clear();
		list8.AddRange(list4.Select((GenerateRoadLayout.PathNode x) => x.node.point));
		list7.Clear();
		list7.AddRange(list3.Select((GenerateRoadLayout.PathNode x) => x.node.point));
		list7.AddRange(list5.Select((GenerateRoadLayout.PathNode x) => x.node.point));
		list7.AddRange(list6);
		PathFinder.Node node3 = pathFinder.FindPathUndirected(list7, list8, 100000);
		if (node3 == null)
		{
			GenerateRoadLayout.PathNode node11 = list4[0];
			list5.AddRange(list4.Where((GenerateRoadLayout.PathNode x) => x.monument == node11.monument));
			list4.RemoveAll((GenerateRoadLayout.PathNode x) => x.monument == node11.monument);
			list5.Remove(node11);
			list3.Add(node11);
		}
		else
		{
			GenerateRoadLayout.PathSegment segment = new GenerateRoadLayout.PathSegment();
			for (PathFinder.Node node4 = node3; node4 != null; node4 = node4.next)
			{
				if (node4 == node3)
				{
					segment.start = node4;
				}
				if (node4.next == null)
				{
					segment.end = node4;
				}
			}
			list2.Add(segment);
			GenerateRoadLayout.PathNode node = list4.Find((GenerateRoadLayout.PathNode x) => x.node.point == segment.start.point || x.node.point == segment.end.point);
			list5.AddRange(list4.Where((GenerateRoadLayout.PathNode x) => x.monument == node.monument));
			list4.RemoveAll((GenerateRoadLayout.PathNode x) => x.monument == node.monument);
			list5.Remove(node);
			list3.Add(node);
			GenerateRoadLayout.PathNode pathNode = list5.Find((GenerateRoadLayout.PathNode x) => x.node.point == segment.start.point || x.node.point == segment.end.point);
			if (pathNode != null)
			{
				list5.Remove(pathNode);
				list3.Add(pathNode);
			}
			int num2 = 1;
			for (PathFinder.Node node5 = node3; node5 != null; node5 = node5.next)
			{
				if (num2 % 8 == 0)
				{
					list6.Add(node5.point);
				}
				num2++;
			}
		}
		IL_492:
		if (list4.Count == 0 && list5.Count == 0)
		{
			using (List<GenerateRoadLayout.PathNode>.Enumerator enumerator3 = list3.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					GenerateRoadLayout.PathNode target = enumerator3.Current;
					GenerateRoadLayout.PathSegment pathSegment = list2.Find((GenerateRoadLayout.PathSegment x) => x.start.point == target.node.point || x.end.point == target.node.point);
					if (pathSegment != null)
					{
						if (pathSegment.start.point == target.node.point)
						{
							PathFinder.Node node6 = target.node;
							PathFinder.Node node7 = pathFinder.Reverse(target.node);
							node6.next = pathSegment.start;
							pathSegment.start = node7;
							pathSegment.origin = target.target;
						}
						else if (pathSegment.end.point == target.node.point)
						{
							pathSegment.end.next = target.node;
							pathSegment.end = pathFinder.FindEnd(target.node);
							pathSegment.target = target.target;
						}
					}
				}
			}
			List<Vector3> list9 = new List<Vector3>();
			foreach (GenerateRoadLayout.PathSegment pathSegment2 in list2)
			{
				bool flag = false;
				bool flag2 = false;
				for (PathFinder.Node node8 = pathSegment2.start; node8 != null; node8 = node8.next)
				{
					float num3 = ((float)node8.point.x + 0.5f) / (float)length;
					float num4 = ((float)node8.point.y + 0.5f) / (float)length;
					if (pathSegment2.start == node8 && pathSegment2.origin != null)
					{
						flag = true;
						num3 = TerrainMeta.NormalizeX(pathSegment2.origin.transform.position.x);
						num4 = TerrainMeta.NormalizeZ(pathSegment2.origin.transform.position.z);
					}
					else if (pathSegment2.end == node8 && pathSegment2.target != null)
					{
						flag2 = true;
						num3 = TerrainMeta.NormalizeX(pathSegment2.target.transform.position.x);
						num4 = TerrainMeta.NormalizeZ(pathSegment2.target.transform.position.z);
					}
					float num5 = TerrainMeta.DenormalizeX(num3);
					float num6 = TerrainMeta.DenormalizeZ(num4);
					float num7 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(num3, num4), 1f);
					list9.Add(new Vector3(num5, num7, num6));
				}
				if (list9.Count != 0)
				{
					if (list9.Count >= 2)
					{
						int num8 = TerrainMeta.Path.Roads.Count + list.Count;
						PathList pathList2 = this.CreateSegment(num8, list9.ToArray());
						pathList2.Start = flag;
						pathList2.End = flag2;
						pathList2.ProcgenStartNode = pathSegment2.start;
						pathList2.ProcgenEndNode = pathSegment2.end;
						list.Add(pathList2);
					}
					list9.Clear();
				}
			}
			foreach (PathList pathList3 in list)
			{
				pathList3.Path.Smoothen(4, new Vector3(1f, 0f, 1f), null);
				pathList3.Path.Smoothen(16, new Vector3(0f, 1f, 0f), null);
				pathList3.Path.Resample(7.5f);
				pathList3.Path.RecalculateTangents();
				pathList3.AdjustPlacementMap(20f);
			}
			TerrainMeta.Path.Roads.AddRange(list);
			return;
		}
		goto IL_1BC;
	}

	// Token: 0x040028C8 RID: 10440
	public InfrastructureType RoadType;

	// Token: 0x040028C9 RID: 10441
	public const float RoadWidth = 10f;

	// Token: 0x040028CA RID: 10442
	public const float TrailWidth = 4f;

	// Token: 0x040028CB RID: 10443
	public const float InnerPadding = 1f;

	// Token: 0x040028CC RID: 10444
	public const float OuterPadding = 1f;

	// Token: 0x040028CD RID: 10445
	public const float InnerFade = 1f;

	// Token: 0x040028CE RID: 10446
	public const float OuterFade = 8f;

	// Token: 0x040028CF RID: 10447
	public const float RandomScale = 0.75f;

	// Token: 0x040028D0 RID: 10448
	public const float MeshOffset = 0f;

	// Token: 0x040028D1 RID: 10449
	public const float TerrainOffset = -0.125f;

	// Token: 0x040028D2 RID: 10450
	private const int MaxDepth = 100000;

	// Token: 0x02000E1D RID: 3613
	private class PathNode
	{
		// Token: 0x04004A8E RID: 19086
		public MonumentInfo monument;

		// Token: 0x04004A8F RID: 19087
		public TerrainPathConnect target;

		// Token: 0x04004A90 RID: 19088
		public PathFinder.Node node;
	}

	// Token: 0x02000E1E RID: 3614
	private class PathSegment
	{
		// Token: 0x04004A91 RID: 19089
		public PathFinder.Node start;

		// Token: 0x04004A92 RID: 19090
		public PathFinder.Node end;

		// Token: 0x04004A93 RID: 19091
		public TerrainPathConnect origin;

		// Token: 0x04004A94 RID: 19092
		public TerrainPathConnect target;
	}
}
