using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020006C6 RID: 1734
public class GeneratePowerlineLayout : ProceduralComponent
{
	// Token: 0x060031E2 RID: 12770 RVA: 0x0012DA44 File Offset: 0x0012BC44
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			TerrainMeta.Path.Powerlines.Clear();
			TerrainMeta.Path.Powerlines.AddRange(World.GetPaths("Powerline"));
			return;
		}
		List<PathList> list = new List<PathList>();
		List<MonumentInfo> monuments = TerrainMeta.Path.Monuments;
		int[,] array = TerrainPath.CreatePowerlineCostmap(ref seed);
		PathFinder pathFinder = new PathFinder(array, true, true);
		int length = array.GetLength(0);
		List<GeneratePowerlineLayout.PathSegment> list2 = new List<GeneratePowerlineLayout.PathSegment>();
		List<GeneratePowerlineLayout.PathNode> list3 = new List<GeneratePowerlineLayout.PathNode>();
		List<GeneratePowerlineLayout.PathNode> list4 = new List<GeneratePowerlineLayout.PathNode>();
		List<PathFinder.Point> list5 = new List<PathFinder.Point>();
		List<PathFinder.Point> list6 = new List<PathFinder.Point>();
		List<PathFinder.Point> list7 = new List<PathFinder.Point>();
		foreach (PathList pathList in TerrainMeta.Path.Roads)
		{
			if (pathList.ProcgenStartNode != null && pathList.ProcgenEndNode != null && pathList.Hierarchy == 0)
			{
				int num = 1;
				for (PathFinder.Node node = pathList.ProcgenStartNode; node != null; node = node.next)
				{
					if (num % 8 == 0)
					{
						list5.Add(node.point);
					}
					num++;
				}
			}
		}
		using (List<MonumentInfo>.Enumerator enumerator2 = monuments.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				MonumentInfo monumentInfo = enumerator2.Current;
				foreach (TerrainPathConnect terrainPathConnect in monumentInfo.GetComponentsInChildren<TerrainPathConnect>(true))
				{
					if (terrainPathConnect.Type == InfrastructureType.Power)
					{
						PathFinder.Point pathFinderPoint = terrainPathConnect.GetPathFinderPoint(length);
						PathFinder.Node node2 = pathFinder.FindClosestWalkable(pathFinderPoint, 100000);
						if (node2 != null)
						{
							list4.Add(new GeneratePowerlineLayout.PathNode
							{
								monument = monumentInfo,
								node = node2
							});
						}
					}
				}
			}
			goto IL_355;
		}
		IL_1A3:
		list7.Clear();
		list7.AddRange(list4.Select((GeneratePowerlineLayout.PathNode x) => x.node.point));
		list6.Clear();
		list6.AddRange(list3.Select((GeneratePowerlineLayout.PathNode x) => x.node.point));
		list6.AddRange(list5);
		PathFinder.Node node3 = pathFinder.FindPathUndirected(list6, list7, 100000);
		if (node3 == null)
		{
			GeneratePowerlineLayout.PathNode copy2 = list4[0];
			list3.AddRange(list4.Where((GeneratePowerlineLayout.PathNode x) => x.monument == copy2.monument));
			list4.RemoveAll((GeneratePowerlineLayout.PathNode x) => x.monument == copy2.monument);
		}
		else
		{
			GeneratePowerlineLayout.PathSegment segment = new GeneratePowerlineLayout.PathSegment();
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
			GeneratePowerlineLayout.PathNode copy = list4.Find((GeneratePowerlineLayout.PathNode x) => x.node.point == segment.start.point || x.node.point == segment.end.point);
			list3.AddRange(list4.Where((GeneratePowerlineLayout.PathNode x) => x.monument == copy.monument));
			list4.RemoveAll((GeneratePowerlineLayout.PathNode x) => x.monument == copy.monument);
			int num2 = 1;
			for (PathFinder.Node node5 = node3; node5 != null; node5 = node5.next)
			{
				if (num2 % 8 == 0)
				{
					list5.Add(node5.point);
				}
				num2++;
			}
		}
		IL_355:
		if (list4.Count == 0)
		{
			List<Vector3> list8 = new List<Vector3>();
			foreach (GeneratePowerlineLayout.PathSegment pathSegment in list2)
			{
				for (PathFinder.Node node6 = pathSegment.start; node6 != null; node6 = node6.next)
				{
					float num3 = ((float)node6.point.x + 0.5f) / (float)length;
					float num4 = ((float)node6.point.y + 0.5f) / (float)length;
					float height = TerrainMeta.HeightMap.GetHeight01(num3, num4);
					list8.Add(TerrainMeta.Denormalize(new Vector3(num3, height, num4)));
				}
				if (list8.Count != 0)
				{
					if (list8.Count >= 8)
					{
						int num5 = TerrainMeta.Path.Powerlines.Count + list.Count;
						list.Add(new PathList("Powerline " + num5, list8.ToArray())
						{
							Start = true,
							End = true,
							ProcgenStartNode = pathSegment.start,
							ProcgenEndNode = pathSegment.end
						});
					}
					list8.Clear();
				}
			}
			foreach (PathList pathList2 in list)
			{
				pathList2.Path.RecalculateTangents();
			}
			TerrainMeta.Path.Powerlines.AddRange(list);
			return;
		}
		goto IL_1A3;
	}

	// Token: 0x04002881 RID: 10369
	private const int MaxDepth = 100000;

	// Token: 0x02000E0E RID: 3598
	private class PathNode
	{
		// Token: 0x04004A69 RID: 19049
		public MonumentInfo monument;

		// Token: 0x04004A6A RID: 19050
		public PathFinder.Node node;
	}

	// Token: 0x02000E0F RID: 3599
	private class PathSegment
	{
		// Token: 0x04004A6B RID: 19051
		public PathFinder.Node start;

		// Token: 0x04004A6C RID: 19052
		public PathFinder.Node end;
	}
}
