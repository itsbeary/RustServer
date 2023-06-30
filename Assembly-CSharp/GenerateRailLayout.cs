using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006C9 RID: 1737
public class GenerateRailLayout : ProceduralComponent
{
	// Token: 0x060031EB RID: 12779 RVA: 0x0012E8DC File Offset: 0x0012CADC
	private PathList CreateSegment(int number, Vector3[] points)
	{
		return new PathList("Rail " + number, points)
		{
			Spline = true,
			Width = 4f,
			InnerPadding = 1f,
			OuterPadding = 1f,
			InnerFade = 1f,
			OuterFade = 32f,
			RandomScale = 1f,
			MeshOffset = 0f,
			TerrainOffset = -0.125f,
			Topology = 524288,
			Splat = 128,
			Hierarchy = 1
		};
	}

	// Token: 0x060031EC RID: 12780 RVA: 0x0012E97C File Offset: 0x0012CB7C
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			TerrainMeta.Path.Rails.Clear();
			TerrainMeta.Path.Rails.AddRange(World.GetPaths("Rail"));
			return;
		}
		List<PathList> list = new List<PathList>();
		int[,] array = TerrainPath.CreateRailCostmap(ref seed);
		PathFinder pathFinder = new PathFinder(array, true, true);
		int length = array.GetLength(0);
		new List<GenerateRailLayout.PathSegment>();
		List<PathFinder.Node> list2 = new List<PathFinder.Node>();
		List<PathFinder.Point> list3 = new List<PathFinder.Point>();
		List<PathFinder.Point> list4 = new List<PathFinder.Point>();
		List<Vector3> list5 = new List<Vector3>();
		List<Vector3> list6 = new List<Vector3>();
		List<Vector3> list7 = new List<Vector3>();
		List<Vector3> list8 = new List<Vector3>();
		foreach (PathList pathList in TerrainMeta.Path.Rails)
		{
			if (pathList.ProcgenStartNode != null && pathList.ProcgenEndNode != null)
			{
				for (PathFinder.Node node = pathList.ProcgenStartNode; node != null; node = node.next)
				{
					list2.Add(node);
				}
			}
		}
		foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
		{
			pathFinder.PushPoint = monumentInfo.GetPathFinderPoint(length);
			pathFinder.PushRadius = monumentInfo.GetPathFinderRadius(length);
			pathFinder.PushDistance = 60;
			pathFinder.PushMultiplier = 1;
			foreach (TerrainPathConnect terrainPathConnect in monumentInfo.GetComponentsInChildren<TerrainPathConnect>(true))
			{
				list5.Clear();
				list6.Clear();
				list7.Clear();
				list8.Clear();
				if (terrainPathConnect.Type == InfrastructureType.Rail)
				{
					Vector3 vector = terrainPathConnect.transform.position;
					Vector3 vector2 = terrainPathConnect.transform.forward * 7.5f;
					PathFinder.Point point = terrainPathConnect.GetPathFinderPoint(length, vector);
					int num = 0;
					while (num < 8 || !pathFinder.IsWalkable(point))
					{
						list5.Add(vector);
						vector += vector2;
						point = terrainPathConnect.GetPathFinderPoint(length, vector);
						num++;
					}
					list3.Clear();
					list3.Add(point);
					list4.Clear();
					foreach (PathFinder.Node node2 in list2)
					{
						if (pathFinder.Distance(node2.point, pathFinder.PushPoint) >= (float)(pathFinder.PushRadius + pathFinder.PushDistance / 2))
						{
							list4.Add(node2.point);
						}
					}
					PathFinder.Node node3 = pathFinder.FindPathDirected(list3, list4, 250000);
					if (node3 != null)
					{
						PathFinder.Node node4 = null;
						PathFinder.Node node5 = null;
						PathFinder.Node node6 = node3;
						while (node6 != null && node6.next != null)
						{
							if (node6 == node3.next)
							{
								node4 = node6;
							}
							if (node6.next.next == null)
							{
								node5 = node6;
							}
							node6 = node6.next;
						}
						if (node4 != null && node5 != null)
						{
							node5.next = null;
							for (PathFinder.Node node7 = node4; node7 != null; node7 = node7.next)
							{
								float num2 = ((float)node7.point.x + 0.5f) / (float)length;
								float num3 = ((float)node7.point.y + 0.5f) / (float)length;
								float num4 = TerrainMeta.DenormalizeX(num2);
								float num5 = TerrainMeta.DenormalizeZ(num3);
								float num6 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(num2, num3), 1f);
								list6.Add(new Vector3(num4, num6, num5));
							}
							if (list6.Count != 0)
							{
								Vector3 vector3 = list5[0];
								Vector3 vector4 = list6[list6.Count - 1];
								Vector3 normalized = (vector4 - vector3).normalized;
								PathList pathList2 = null;
								float num7 = float.MaxValue;
								int num8 = -1;
								foreach (PathList pathList3 in TerrainMeta.Path.Rails)
								{
									Vector3[] points = pathList3.Path.Points;
									for (int j = 0; j < points.Length; j++)
									{
										float num9 = Vector3.Distance(vector4, points[j]);
										if (num9 < num7)
										{
											num7 = num9;
											pathList2 = pathList3;
											num8 = j;
										}
									}
								}
								Vector3[] points2 = pathList2.Path.Points;
								Vector3 vector5 = pathList2.Path.Tangents[num8];
								int num10 = ((Vector3.Angle(vector5, normalized) < Vector3.Angle(-vector5, normalized)) ? 1 : (-1));
								Vector3 vector6 = Vector3.Normalize(list6[list6.Count - 1] - list6[Mathf.Max(0, list6.Count - 1 - 16)]);
								Vector3 vector7 = Vector3.Normalize(points2[(num8 + num10 * 8 * 2 + points2.Length) % points2.Length] - points2[num8]);
								float num11 = -Vector3.SignedAngle(vector7, vector6, Vector3.up);
								Vector3 vector8 = GenerateRailLayout.rot90 * vector7;
								if (num11 < 0f)
								{
									vector8 = -vector8;
								}
								for (int k = 0; k < 8; k++)
								{
									float num12 = Mathf.InverseLerp(7f, 0f, (float)k);
									float num13 = Mathf.SmoothStep(0f, 2f, num12) * 4f;
									list7.Add(points2[(num8 + num10 * k + points2.Length) % points2.Length] + vector8 * num13);
								}
								list8.AddRange(list5);
								list8.AddRange(list6);
								list8.AddRange(list7);
								if (list8.Count >= 2)
								{
									int num14 = TerrainMeta.Path.Rails.Count + list.Count;
									PathList pathList4 = this.CreateSegment(num14, list8.ToArray());
									pathList4.Start = true;
									pathList4.End = false;
									pathList4.ProcgenStartNode = node4;
									pathList4.ProcgenEndNode = node5;
									list.Add(pathList4);
								}
							}
						}
					}
				}
			}
		}
		using (List<PathList>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PathList rail = enumerator.Current;
				Func<int, float> func = delegate(int i)
				{
					float num15 = Mathf.InverseLerp(0f, 8f, (float)i);
					float num16 = Mathf.InverseLerp((float)rail.Path.DefaultMaxIndex, (float)(rail.Path.DefaultMaxIndex - 8), (float)i);
					return Mathf.SmoothStep(0f, 1f, Mathf.Min(num15, num16));
				};
				rail.Path.Smoothen(32, new Vector3(1f, 0f, 1f), func);
				rail.Path.Smoothen(64, new Vector3(0f, 1f, 0f), func);
				rail.Path.Resample(7.5f);
				rail.Path.RecalculateTangents();
				rail.AdjustPlacementMap(20f);
			}
		}
		TerrainMeta.Path.Rails.AddRange(list);
	}

	// Token: 0x0400288C RID: 10380
	public const float Width = 4f;

	// Token: 0x0400288D RID: 10381
	public const float InnerPadding = 1f;

	// Token: 0x0400288E RID: 10382
	public const float OuterPadding = 1f;

	// Token: 0x0400288F RID: 10383
	public const float InnerFade = 1f;

	// Token: 0x04002890 RID: 10384
	public const float OuterFade = 32f;

	// Token: 0x04002891 RID: 10385
	public const float RandomScale = 1f;

	// Token: 0x04002892 RID: 10386
	public const float MeshOffset = 0f;

	// Token: 0x04002893 RID: 10387
	public const float TerrainOffset = -0.125f;

	// Token: 0x04002894 RID: 10388
	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x04002895 RID: 10389
	private const int MaxDepth = 250000;

	// Token: 0x02000E15 RID: 3605
	private class PathNode
	{
		// Token: 0x04004A74 RID: 19060
		public MonumentInfo monument;

		// Token: 0x04004A75 RID: 19061
		public TerrainPathConnect target;

		// Token: 0x04004A76 RID: 19062
		public PathFinder.Node node;
	}

	// Token: 0x02000E16 RID: 3606
	private class PathSegment
	{
		// Token: 0x04004A77 RID: 19063
		public PathFinder.Node start;

		// Token: 0x04004A78 RID: 19064
		public PathFinder.Node end;

		// Token: 0x04004A79 RID: 19065
		public TerrainPathConnect origin;

		// Token: 0x04004A7A RID: 19066
		public TerrainPathConnect target;
	}
}
