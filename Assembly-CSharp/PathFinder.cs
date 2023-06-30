using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200094D RID: 2381
public class PathFinder
{
	// Token: 0x060038F5 RID: 14581 RVA: 0x00152384 File Offset: 0x00150584
	public PathFinder(int[,] costmap, bool diagonals = true, bool directional = true)
	{
		this.costmap = costmap;
		this.neighbors = (diagonals ? PathFinder.mooreNeighbors : PathFinder.neumannNeighbors);
		this.directional = directional;
	}

	// Token: 0x060038F6 RID: 14582 RVA: 0x001523AF File Offset: 0x001505AF
	public int GetResolution(int index)
	{
		return this.costmap.GetLength(index);
	}

	// Token: 0x060038F7 RID: 14583 RVA: 0x001523BD File Offset: 0x001505BD
	public PathFinder.Node FindPath(PathFinder.Point start, PathFinder.Point end, int depth = 2147483647)
	{
		return this.FindPathReversed(end, start, depth);
	}

	// Token: 0x060038F8 RID: 14584 RVA: 0x001523C8 File Offset: 0x001505C8
	private PathFinder.Node FindPathReversed(PathFinder.Point start, PathFinder.Point end, int depth = 2147483647)
	{
		if (this.visited == null)
		{
			this.visited = new int[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		int num = 0;
		int num2 = this.costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = this.costmap.GetLength(1) - 1;
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = default(IntrusiveMinHeap<PathFinder.Node>);
		int num5 = this.Cost(start);
		int num6 = this.Heuristic(start, end);
		intrusiveMinHeap.Add(new PathFinder.Node(start, num5, num6, null));
		this.visited[start.x, start.y] = num5;
		while (!intrusiveMinHeap.Empty && depth-- > 0)
		{
			PathFinder.Node node = intrusiveMinHeap.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < this.neighbors.Length; i++)
			{
				PathFinder.Point point = node.point + this.neighbors[i];
				if (point.x >= num && point.x <= num2 && point.y >= num3 && point.y <= num4)
				{
					int num7 = this.Cost(point, node);
					if (num7 != 2147483647)
					{
						int num8 = this.visited[point.x, point.y];
						if (num8 == 0 || num7 < num8)
						{
							int num9 = node.cost + num7;
							int num10 = this.Heuristic(point, end);
							intrusiveMinHeap.Add(new PathFinder.Node(point, num9, num10, node));
							this.visited[point.x, point.y] = num7;
						}
					}
					else
					{
						this.visited[point.x, point.y] = -1;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x060038F9 RID: 14585 RVA: 0x001525B9 File Offset: 0x001507B9
	public PathFinder.Node FindPathDirected(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		if (startList.Count == 0 || endList.Count == 0)
		{
			return null;
		}
		return this.FindPathReversed(endList, startList, depth);
	}

	// Token: 0x060038FA RID: 14586 RVA: 0x001525D6 File Offset: 0x001507D6
	public PathFinder.Node FindPathUndirected(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		if (startList.Count == 0 || endList.Count == 0)
		{
			return null;
		}
		if (startList.Count > endList.Count)
		{
			return this.FindPathReversed(endList, startList, depth);
		}
		return this.FindPathReversed(startList, endList, depth);
	}

	// Token: 0x060038FB RID: 14587 RVA: 0x0015260C File Offset: 0x0015080C
	private PathFinder.Node FindPathReversed(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		if (this.visited == null)
		{
			this.visited = new int[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		int num = 0;
		int num2 = this.costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = this.costmap.GetLength(1) - 1;
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = default(IntrusiveMinHeap<PathFinder.Node>);
		using (List<PathFinder.Point>.Enumerator enumerator = startList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PathFinder.Point point = enumerator.Current;
				int num5 = this.Cost(point);
				int num6 = this.Heuristic(point, endList);
				intrusiveMinHeap.Add(new PathFinder.Node(point, num5, num6, null));
				this.visited[point.x, point.y] = num5;
			}
			goto IL_1FD;
		}
		IL_E0:
		PathFinder.Node node = intrusiveMinHeap.Pop();
		if (node.heuristic == 0)
		{
			return node;
		}
		for (int i = 0; i < this.neighbors.Length; i++)
		{
			PathFinder.Point point2 = node.point + this.neighbors[i];
			if (point2.x >= num && point2.x <= num2 && point2.y >= num3 && point2.y <= num4)
			{
				int num7 = this.Cost(point2, node);
				if (num7 != 2147483647)
				{
					int num8 = this.visited[point2.x, point2.y];
					if (num8 == 0 || num7 < num8)
					{
						int num9 = node.cost + num7;
						int num10 = this.Heuristic(point2, endList);
						intrusiveMinHeap.Add(new PathFinder.Node(point2, num9, num10, node));
						this.visited[point2.x, point2.y] = num7;
					}
				}
				else
				{
					this.visited[point2.x, point2.y] = -1;
				}
			}
		}
		IL_1FD:
		if (intrusiveMinHeap.Empty || depth-- <= 0)
		{
			return null;
		}
		goto IL_E0;
	}

	// Token: 0x060038FC RID: 14588 RVA: 0x0015283C File Offset: 0x00150A3C
	public PathFinder.Node FindClosestWalkable(PathFinder.Point start, int depth = 2147483647)
	{
		if (this.visited == null)
		{
			this.visited = new int[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		int num = 0;
		int num2 = this.costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = this.costmap.GetLength(1) - 1;
		if (start.x < num)
		{
			return null;
		}
		if (start.x > num2)
		{
			return null;
		}
		if (start.y < num3)
		{
			return null;
		}
		if (start.y > num4)
		{
			return null;
		}
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = default(IntrusiveMinHeap<PathFinder.Node>);
		int num5 = 1;
		int num6 = this.Heuristic(start);
		intrusiveMinHeap.Add(new PathFinder.Node(start, num5, num6, null));
		this.visited[start.x, start.y] = num5;
		while (!intrusiveMinHeap.Empty && depth-- > 0)
		{
			PathFinder.Node node = intrusiveMinHeap.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < this.neighbors.Length; i++)
			{
				PathFinder.Point point = node.point + this.neighbors[i];
				if (point.x >= num && point.x <= num2 && point.y >= num3 && point.y <= num4)
				{
					int num7 = 1;
					if (this.visited[point.x, point.y] == 0)
					{
						int num8 = node.cost + num7;
						int num9 = this.Heuristic(point);
						intrusiveMinHeap.Add(new PathFinder.Node(point, num8, num9, node));
						this.visited[point.x, point.y] = num7;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x060038FD RID: 14589 RVA: 0x00152A10 File Offset: 0x00150C10
	public bool IsWalkable(PathFinder.Point point)
	{
		return this.costmap[point.x, point.y] != int.MaxValue;
	}

	// Token: 0x060038FE RID: 14590 RVA: 0x00152A34 File Offset: 0x00150C34
	public bool IsWalkableWithNeighbours(PathFinder.Point point)
	{
		if (this.costmap[point.x, point.y] == 2147483647)
		{
			return false;
		}
		for (int i = 0; i < this.neighbors.Length; i++)
		{
			PathFinder.Point point2 = point + this.neighbors[i];
			if (this.costmap[point2.x, point2.y] == 2147483647)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060038FF RID: 14591 RVA: 0x00152AA8 File Offset: 0x00150CA8
	public PathFinder.Node Reverse(PathFinder.Node start)
	{
		PathFinder.Node node = null;
		PathFinder.Node node2 = null;
		for (PathFinder.Node node3 = start; node3 != null; node3 = node3.next)
		{
			if (node != null)
			{
				node.next = node2;
			}
			node2 = node;
			node = node3;
		}
		if (node != null)
		{
			node.next = node2;
		}
		return node;
	}

	// Token: 0x06003900 RID: 14592 RVA: 0x00152AE0 File Offset: 0x00150CE0
	public PathFinder.Node FindEnd(PathFinder.Node start)
	{
		for (PathFinder.Node node = start; node != null; node = node.next)
		{
			if (node.next == null)
			{
				return node;
			}
		}
		return start;
	}

	// Token: 0x06003901 RID: 14593 RVA: 0x00152B08 File Offset: 0x00150D08
	public int Cost(PathFinder.Point a)
	{
		int num = this.costmap[a.x, a.y];
		int num2 = 0;
		if (num != 2147483647 && this.PushMultiplier > 0)
		{
			int num3 = Mathf.Max(0, this.Heuristic(a, this.PushPoint) - this.PushRadius * this.PushRadius);
			int num4 = Mathf.Max(0, this.PushDistance * this.PushDistance - num3);
			num2 = this.PushMultiplier * num4;
		}
		return num + num2;
	}

	// Token: 0x06003902 RID: 14594 RVA: 0x00152B84 File Offset: 0x00150D84
	public int Cost(PathFinder.Point a, PathFinder.Node neighbour)
	{
		int num = this.Cost(a);
		int num2 = 0;
		if (num != 2147483647 && this.directional && neighbour != null && neighbour.next != null && this.Heuristic(a, neighbour.next.point) <= 2)
		{
			num2 = 10000;
		}
		return num + num2;
	}

	// Token: 0x06003903 RID: 14595 RVA: 0x00152BD2 File Offset: 0x00150DD2
	public int Heuristic(PathFinder.Point a)
	{
		if (this.costmap[a.x, a.y] != 2147483647)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x06003904 RID: 14596 RVA: 0x00152BF8 File Offset: 0x00150DF8
	public int Heuristic(PathFinder.Point a, PathFinder.Point b)
	{
		int num = a.x - b.x;
		int num2 = a.y - b.y;
		return num * num + num2 * num2;
	}

	// Token: 0x06003905 RID: 14597 RVA: 0x00152C28 File Offset: 0x00150E28
	public int Heuristic(PathFinder.Point a, List<PathFinder.Point> b)
	{
		int num = int.MaxValue;
		for (int i = 0; i < b.Count; i++)
		{
			num = Mathf.Min(num, this.Heuristic(a, b[i]));
		}
		return num;
	}

	// Token: 0x06003906 RID: 14598 RVA: 0x00152C64 File Offset: 0x00150E64
	public float Distance(PathFinder.Point a, PathFinder.Point b)
	{
		int num = a.x - b.x;
		int num2 = a.y - b.y;
		return Mathf.Sqrt((float)(num * num + num2 * num2));
	}

	// Token: 0x040033C3 RID: 13251
	private int[,] costmap;

	// Token: 0x040033C4 RID: 13252
	private int[,] visited;

	// Token: 0x040033C5 RID: 13253
	private PathFinder.Point[] neighbors;

	// Token: 0x040033C6 RID: 13254
	private bool directional;

	// Token: 0x040033C7 RID: 13255
	public PathFinder.Point PushPoint;

	// Token: 0x040033C8 RID: 13256
	public int PushRadius;

	// Token: 0x040033C9 RID: 13257
	public int PushDistance;

	// Token: 0x040033CA RID: 13258
	public int PushMultiplier;

	// Token: 0x040033CB RID: 13259
	private static PathFinder.Point[] mooreNeighbors = new PathFinder.Point[]
	{
		new PathFinder.Point(0, 1),
		new PathFinder.Point(-1, 0),
		new PathFinder.Point(1, 0),
		new PathFinder.Point(0, -1),
		new PathFinder.Point(-1, 1),
		new PathFinder.Point(1, 1),
		new PathFinder.Point(-1, -1),
		new PathFinder.Point(1, -1)
	};

	// Token: 0x040033CC RID: 13260
	private static PathFinder.Point[] neumannNeighbors = new PathFinder.Point[]
	{
		new PathFinder.Point(0, 1),
		new PathFinder.Point(-1, 0),
		new PathFinder.Point(1, 0),
		new PathFinder.Point(0, -1)
	};

	// Token: 0x02000ED1 RID: 3793
	public struct Point : IEquatable<PathFinder.Point>
	{
		// Token: 0x0600536D RID: 21357 RVA: 0x001B277C File Offset: 0x001B097C
		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x0600536E RID: 21358 RVA: 0x001B278C File Offset: 0x001B098C
		public static PathFinder.Point operator +(PathFinder.Point a, PathFinder.Point b)
		{
			return new PathFinder.Point(a.x + b.x, a.y + b.y);
		}

		// Token: 0x0600536F RID: 21359 RVA: 0x001B27AD File Offset: 0x001B09AD
		public static PathFinder.Point operator -(PathFinder.Point a, PathFinder.Point b)
		{
			return new PathFinder.Point(a.x - b.x, a.y - b.y);
		}

		// Token: 0x06005370 RID: 21360 RVA: 0x001B27CE File Offset: 0x001B09CE
		public static PathFinder.Point operator *(PathFinder.Point p, int i)
		{
			return new PathFinder.Point(p.x * i, p.y * i);
		}

		// Token: 0x06005371 RID: 21361 RVA: 0x001B27E5 File Offset: 0x001B09E5
		public static PathFinder.Point operator /(PathFinder.Point p, int i)
		{
			return new PathFinder.Point(p.x / i, p.y / i);
		}

		// Token: 0x06005372 RID: 21362 RVA: 0x001B27FC File Offset: 0x001B09FC
		public static bool operator ==(PathFinder.Point a, PathFinder.Point b)
		{
			return a.Equals(b);
		}

		// Token: 0x06005373 RID: 21363 RVA: 0x001B2806 File Offset: 0x001B0A06
		public static bool operator !=(PathFinder.Point a, PathFinder.Point b)
		{
			return !a.Equals(b);
		}

		// Token: 0x06005374 RID: 21364 RVA: 0x001B2813 File Offset: 0x001B0A13
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode();
		}

		// Token: 0x06005375 RID: 21365 RVA: 0x001B282C File Offset: 0x001B0A2C
		public override bool Equals(object other)
		{
			return other is PathFinder.Point && this.Equals((PathFinder.Point)other);
		}

		// Token: 0x06005376 RID: 21366 RVA: 0x001B2844 File Offset: 0x001B0A44
		public bool Equals(PathFinder.Point other)
		{
			return this.x == other.x && this.y == other.y;
		}

		// Token: 0x04004D72 RID: 19826
		public int x;

		// Token: 0x04004D73 RID: 19827
		public int y;
	}

	// Token: 0x02000ED2 RID: 3794
	public class Node : IMinHeapNode<PathFinder.Node>, ILinkedListNode<PathFinder.Node>
	{
		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x06005377 RID: 21367 RVA: 0x001B2864 File Offset: 0x001B0A64
		// (set) Token: 0x06005378 RID: 21368 RVA: 0x001B286C File Offset: 0x001B0A6C
		public PathFinder.Node next { get; set; }

		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x06005379 RID: 21369 RVA: 0x001B2875 File Offset: 0x001B0A75
		// (set) Token: 0x0600537A RID: 21370 RVA: 0x001B287D File Offset: 0x001B0A7D
		public PathFinder.Node child { get; set; }

		// Token: 0x17000704 RID: 1796
		// (get) Token: 0x0600537B RID: 21371 RVA: 0x001B2886 File Offset: 0x001B0A86
		public int order
		{
			get
			{
				return this.cost + this.heuristic;
			}
		}

		// Token: 0x0600537C RID: 21372 RVA: 0x001B2895 File Offset: 0x001B0A95
		public Node(PathFinder.Point point, int cost, int heuristic, PathFinder.Node next = null)
		{
			this.point = point;
			this.cost = cost;
			this.heuristic = heuristic;
			this.next = next;
		}

		// Token: 0x04004D74 RID: 19828
		public PathFinder.Point point;

		// Token: 0x04004D75 RID: 19829
		public int cost;

		// Token: 0x04004D76 RID: 19830
		public int heuristic;
	}
}
