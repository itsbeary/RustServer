using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006D7 RID: 1751
public class GenerateRoadRing : ProceduralComponent
{
	// Token: 0x06003213 RID: 12819 RVA: 0x00131B74 File Offset: 0x0012FD74
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			return;
		}
		if ((ulong)World.Size < (ulong)((long)this.MinWorldSize))
		{
			return;
		}
		int[,] array = TerrainPath.CreateRoadCostmap(ref seed);
		PathFinder pathFinder = new PathFinder(array, true, true);
		int length = array.GetLength(0);
		int num = length / 4;
		int num2 = 1;
		int num3 = num / num2;
		int num4 = length / 2;
		int num5 = num;
		int num6 = length - num;
		int num7 = num;
		int num8 = length - num;
		int num9 = 0;
		int num10 = -num2;
		int num11 = num2;
		int num12 = -num2;
		int num13 = num2;
		List<GenerateRoadRing.RingNode> list2;
		if (World.Size < 5000U)
		{
			List<GenerateRoadRing.RingNode> list = new List<GenerateRoadRing.RingNode>();
			list.Add(new GenerateRoadRing.RingNode(num6, num8, num10, num12, num3));
			list.Add(new GenerateRoadRing.RingNode(num6, num7, num10, num13, num3));
			list.Add(new GenerateRoadRing.RingNode(num5, num7, num11, num13, num3));
			list2 = list;
			list.Add(new GenerateRoadRing.RingNode(num5, num8, num11, num12, num3));
		}
		else
		{
			List<GenerateRoadRing.RingNode> list3 = new List<GenerateRoadRing.RingNode>();
			list3.Add(new GenerateRoadRing.RingNode(num4, num8, num9, num12, num3));
			list3.Add(new GenerateRoadRing.RingNode(num6, num8, num10, num12, num3));
			list3.Add(new GenerateRoadRing.RingNode(num6, num4, num10, num9, num3));
			list3.Add(new GenerateRoadRing.RingNode(num6, num7, num10, num13, num3));
			list3.Add(new GenerateRoadRing.RingNode(num4, num7, num9, num13, num3));
			list3.Add(new GenerateRoadRing.RingNode(num5, num7, num11, num13, num3));
			list3.Add(new GenerateRoadRing.RingNode(num5, num4, num11, num9, num3));
			list2 = list3;
			list3.Add(new GenerateRoadRing.RingNode(num5, num8, num11, num12, num3));
		}
		List<GenerateRoadRing.RingNode> list4 = list2;
		for (int i = 0; i < list4.Count; i++)
		{
			GenerateRoadRing.RingNode ringNode = list4[i];
			GenerateRoadRing.RingNode ringNode2 = list4[(i + 1) % list4.Count];
			GenerateRoadRing.RingNode ringNode3 = list4[(i - 1 + list4.Count) % list4.Count];
			ringNode.next = ringNode2;
			ringNode.prev = ringNode3;
			while (!pathFinder.IsWalkable(ringNode.position))
			{
				if (ringNode.attempts <= 0)
				{
					return;
				}
				ringNode.position += ringNode.direction;
				ringNode.attempts--;
			}
		}
		foreach (GenerateRoadRing.RingNode ringNode4 in list4)
		{
			ringNode4.path = pathFinder.FindPath(ringNode4.position, ringNode4.next.position, 250000);
		}
		bool flag = false;
		IL_416:
		while (!flag)
		{
			flag = true;
			PathFinder.Point point = new PathFinder.Point(0, 0);
			foreach (GenerateRoadRing.RingNode ringNode5 in list4)
			{
				point += ringNode5.position;
			}
			point /= list4.Count;
			float num14 = float.MinValue;
			GenerateRoadRing.RingNode ringNode6 = null;
			foreach (GenerateRoadRing.RingNode ringNode7 in list4)
			{
				if (ringNode7.path == null)
				{
					float num15 = new Vector2((float)(ringNode7.position.x - point.x), (float)(ringNode7.position.y - point.y)).magnitude;
					if (ringNode7.prev.path == null)
					{
						num15 *= 1.5f;
					}
					if (num15 > num14)
					{
						num14 = num15;
						ringNode6 = ringNode7;
					}
				}
			}
			if (ringNode6 != null)
			{
				while (ringNode6.attempts > 0)
				{
					ringNode6.position += ringNode6.direction;
					ringNode6.attempts--;
					if (pathFinder.IsWalkable(ringNode6.position))
					{
						ringNode6.path = pathFinder.FindPath(ringNode6.position, ringNode6.next.position, 250000);
						ringNode6.prev.path = pathFinder.FindPath(ringNode6.prev.position, ringNode6.position, 250000);
						flag = false;
						goto IL_416;
					}
				}
				return;
			}
		}
		if (flag)
		{
			for (int j = 0; j < list4.Count; j++)
			{
				GenerateRoadRing.RingNode ringNode8 = list4[j];
				GenerateRoadRing.RingNode ringNode9 = list4[(j + 1) % list4.Count];
				PathFinder.Node node = null;
				PathFinder.Node node2 = null;
				for (PathFinder.Node node3 = ringNode8.path; node3 != null; node3 = node3.next)
				{
					for (PathFinder.Node node4 = ringNode9.path; node4 != null; node4 = node4.next)
					{
						int num16 = Mathf.Abs(node3.point.x - node4.point.x);
						int num17 = Mathf.Abs(node3.point.y - node4.point.y);
						if (num16 <= 15 && num17 <= 15)
						{
							if (node == null || node3.cost > node.cost)
							{
								node = node3;
							}
							if (node2 == null || node4.cost < node2.cost)
							{
								node2 = node4;
							}
						}
					}
				}
				if (node != null && node2 != null)
				{
					PathFinder.Node node5 = pathFinder.FindPath(node.point, node2.point, 250000);
					if (node5 != null && node5.next != null)
					{
						node.next = node5.next;
						ringNode9.path = node2;
					}
				}
			}
			for (int k = 0; k < list4.Count; k++)
			{
				GenerateRoadRing.RingNode ringNode10 = list4[k];
				GenerateRoadRing.RingNode ringNode11 = list4[(k + 1) % list4.Count];
				PathFinder.Node node6 = null;
				PathFinder.Node node7 = null;
				for (PathFinder.Node node8 = ringNode10.path; node8 != null; node8 = node8.next)
				{
					for (PathFinder.Node node9 = ringNode11.path; node9 != null; node9 = node9.next)
					{
						int num18 = Mathf.Abs(node8.point.x - node9.point.x);
						int num19 = Mathf.Abs(node8.point.y - node9.point.y);
						if (num18 <= 1 && num19 <= 1)
						{
							if (node6 == null || node8.cost > node6.cost)
							{
								node6 = node8;
							}
							if (node7 == null || node9.cost < node7.cost)
							{
								node7 = node9;
							}
						}
					}
				}
				if (node6 != null && node7 != null)
				{
					node6.next = null;
					ringNode11.path = node7;
				}
			}
			PathFinder.Node node10 = null;
			PathFinder.Node node11 = null;
			foreach (GenerateRoadRing.RingNode ringNode12 in list4)
			{
				if (node10 == null)
				{
					node10 = ringNode12.path;
					node11 = ringNode12.path;
				}
				else
				{
					node11.next = ringNode12.path;
				}
				while (node11.next != null)
				{
					node11 = node11.next;
				}
			}
			node11.next = new PathFinder.Node(node10.point, node10.cost, node10.heuristic, null);
			List<Vector3> list5 = new List<Vector3>();
			for (PathFinder.Node node12 = node10; node12 != null; node12 = node12.next)
			{
				float num20 = ((float)node12.point.x + 0.5f) / (float)length;
				float num21 = ((float)node12.point.y + 0.5f) / (float)length;
				float num22 = TerrainMeta.DenormalizeX(num20);
				float num23 = TerrainMeta.DenormalizeZ(num21);
				float num24 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(num20, num21), 1f);
				list5.Add(new Vector3(num22, num24, num23));
			}
			if (list5.Count >= 2)
			{
				int count = TerrainMeta.Path.Roads.Count;
				PathList pathList = new PathList("Road " + count, list5.ToArray());
				pathList.Spline = true;
				pathList.Width = 12f;
				pathList.InnerPadding = 1f;
				pathList.OuterPadding = 1f;
				pathList.InnerFade = 1f;
				pathList.OuterFade = 8f;
				pathList.RandomScale = 0.75f;
				pathList.MeshOffset = 0f;
				pathList.TerrainOffset = -0.125f;
				pathList.Topology = 2048;
				pathList.Splat = 128;
				pathList.Start = false;
				pathList.End = false;
				pathList.ProcgenStartNode = node10;
				pathList.ProcgenEndNode = node11;
				pathList.Path.Smoothen(4, new Vector3(1f, 0f, 1f), null);
				pathList.Path.Smoothen(16, new Vector3(0f, 1f, 0f), null);
				pathList.Path.Resample(7.5f);
				pathList.Path.RecalculateTangents();
				pathList.AdjustPlacementMap(24f);
				TerrainMeta.Path.Roads.Add(pathList);
			}
		}
	}

	// Token: 0x040028DA RID: 10458
	public const float Width = 12f;

	// Token: 0x040028DB RID: 10459
	public const float InnerPadding = 1f;

	// Token: 0x040028DC RID: 10460
	public const float OuterPadding = 1f;

	// Token: 0x040028DD RID: 10461
	public const float InnerFade = 1f;

	// Token: 0x040028DE RID: 10462
	public const float OuterFade = 8f;

	// Token: 0x040028DF RID: 10463
	public const float RandomScale = 0.75f;

	// Token: 0x040028E0 RID: 10464
	public const float MeshOffset = 0f;

	// Token: 0x040028E1 RID: 10465
	public const float TerrainOffset = -0.125f;

	// Token: 0x040028E2 RID: 10466
	private const int MaxDepth = 250000;

	// Token: 0x040028E3 RID: 10467
	public int MinWorldSize;

	// Token: 0x02000E25 RID: 3621
	private class RingNode
	{
		// Token: 0x06005241 RID: 21057 RVA: 0x001AE842 File Offset: 0x001ACA42
		public RingNode(int pos_x, int pos_y, int dir_x, int dir_y, int stepcount)
		{
			this.position = new PathFinder.Point(pos_x, pos_y);
			this.direction = new PathFinder.Point(dir_x, dir_y);
			this.attempts = stepcount;
		}

		// Token: 0x04004A9E RID: 19102
		public int attempts;

		// Token: 0x04004A9F RID: 19103
		public PathFinder.Point position;

		// Token: 0x04004AA0 RID: 19104
		public PathFinder.Point direction;

		// Token: 0x04004AA1 RID: 19105
		public GenerateRoadRing.RingNode next;

		// Token: 0x04004AA2 RID: 19106
		public GenerateRoadRing.RingNode prev;

		// Token: 0x04004AA3 RID: 19107
		public PathFinder.Node path;
	}
}
