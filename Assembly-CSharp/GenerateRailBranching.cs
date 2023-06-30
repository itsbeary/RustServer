using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006C8 RID: 1736
public class GenerateRailBranching : ProceduralComponent
{
	// Token: 0x060031E6 RID: 12774 RVA: 0x0012DF88 File Offset: 0x0012C188
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

	// Token: 0x060031E7 RID: 12775 RVA: 0x0012E028 File Offset: 0x0012C228
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			TerrainMeta.Path.Rails.Clear();
			TerrainMeta.Path.Rails.AddRange(World.GetPaths("Rail"));
			return;
		}
		int num = Mathf.RoundToInt(40f);
		int num2 = Mathf.RoundToInt(53.333332f);
		int num3 = Mathf.RoundToInt(40f);
		int num4 = Mathf.RoundToInt(120f);
		float num5 = 120f;
		float num6 = num5 * num5;
		List<PathList> list = new List<PathList>();
		int[,] array = TerrainPath.CreateRailCostmap(ref seed);
		PathFinder pathFinder = new PathFinder(array, true, true);
		int length = array.GetLength(0);
		List<Vector3> list2 = new List<Vector3>();
		List<Vector3> list3 = new List<Vector3>();
		List<Vector3> list4 = new List<Vector3>();
		HashSet<Vector3> hashSet = new HashSet<Vector3>();
		foreach (PathList pathList in TerrainMeta.Path.Rails)
		{
			foreach (PathList pathList2 in TerrainMeta.Path.Rails)
			{
				if (pathList != pathList2)
				{
					foreach (Vector3 vector in pathList.Path.Points)
					{
						foreach (Vector3 vector2 in pathList2.Path.Points)
						{
							if ((vector - vector2).sqrMagnitude < num6)
							{
								hashSet.Add(vector);
								break;
							}
						}
					}
				}
			}
		}
		foreach (PathList pathList3 in TerrainMeta.Path.Rails)
		{
			PathInterpolator path = pathList3.Path;
			Vector3[] points3 = path.Points;
			Vector3[] tangents = path.Tangents;
			int num7 = path.MinIndex + 1 + 8;
			int num8 = path.MaxIndex - 1 - 8;
			for (int k = num7; k <= num8; k++)
			{
				list2.Clear();
				list3.Clear();
				list4.Clear();
				int num9 = SeedRandom.Range(ref seed, num3, num4);
				int num10 = SeedRandom.Range(ref seed, num, num2);
				int num11 = k;
				int num12 = k + num9;
				if (num12 < num8)
				{
					Vector3 vector3 = tangents[num11];
					Vector3 vector4 = tangents[num12];
					if (Vector3.Angle(vector3, vector4) <= 30f)
					{
						Vector3 vector5 = points3[num11];
						Vector3 vector6 = points3[num12];
						if (!hashSet.Contains(vector5) && !hashSet.Contains(vector6))
						{
							PathFinder.Point pathFinderPoint = this.GetPathFinderPoint(vector5, length);
							PathFinder.Point pathFinderPoint2 = this.GetPathFinderPoint(vector6, length);
							k += num10;
							PathFinder.Node node = pathFinder.FindPath(pathFinderPoint, pathFinderPoint2, 250000);
							if (node != null)
							{
								PathFinder.Node node2 = null;
								PathFinder.Node node3 = null;
								PathFinder.Node node4 = node;
								while (node4 != null && node4.next != null)
								{
									if (node4 == node.next)
									{
										node2 = node4;
									}
									if (node4.next.next == null)
									{
										node3 = node4;
									}
									node4 = node4.next;
								}
								if (node2 != null && node3 != null)
								{
									node3.next = null;
									for (int l = 0; l < 8; l++)
									{
										int num13 = num11 + (l + 1 - 8);
										int num14 = num12 + l;
										list2.Add(points3[num13]);
										list3.Add(points3[num14]);
									}
									list4.AddRange(list2);
									for (PathFinder.Node node5 = node2; node5 != null; node5 = node5.next)
									{
										float num15 = ((float)node5.point.x + 0.5f) / (float)length;
										float num16 = ((float)node5.point.y + 0.5f) / (float)length;
										float num17 = TerrainMeta.DenormalizeX(num15);
										float num18 = TerrainMeta.DenormalizeZ(num16);
										float num19 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(num15, num16), 1f);
										list4.Add(new Vector3(num17, num19, num18));
									}
									list4.AddRange(list3);
									int num20 = 8;
									int num21 = list4.Count - 1 - 8;
									Vector3 vector7 = Vector3.Normalize(list4[num20 + 16] - list4[num20]);
									Vector3 vector8 = Vector3.Normalize(list4[num21] - list4[num21 - 16]);
									Vector3 vector9 = Vector3.Normalize(points3[num11 + 16] - points3[num11]);
									Vector3 vector10 = Vector3.Normalize(points3[num12] - points3[(num12 - 16 + points3.Length) % points3.Length]);
									float num22 = Vector3.SignedAngle(vector9, vector7, Vector3.up);
									float num23 = -Vector3.SignedAngle(vector10, vector8, Vector3.up);
									if (Mathf.Sign(num22) == Mathf.Sign(num23) && Mathf.Abs(num22) <= 60f && Mathf.Abs(num23) <= 60f)
									{
										Vector3 vector11 = GenerateRailBranching.rot90 * vector9;
										Vector3 vector12 = GenerateRailBranching.rot90 * vector10;
										if (num22 < 0f)
										{
											vector11 = -vector11;
										}
										if (num23 < 0f)
										{
											vector12 = -vector12;
										}
										for (int m = 0; m < 16; m++)
										{
											int num24 = m;
											int num25 = list4.Count - m - 1;
											float num26 = Mathf.InverseLerp(0f, 8f, (float)m);
											float num27 = Mathf.SmoothStep(0f, 2f, num26) * 4f;
											List<Vector3> list5 = list4;
											int n = num24;
											list5[n] += vector11 * num27;
											list5 = list4;
											n = num25;
											list5[n] += vector12 * num27;
										}
										bool flag = false;
										bool flag2 = false;
										foreach (Vector3 vector13 in list4)
										{
											bool blocked = TerrainMeta.PlacementMap.GetBlocked(vector13);
											if (!flag2 && !flag && !blocked)
											{
												flag = true;
											}
											if (flag && !flag2 && blocked)
											{
												flag2 = true;
											}
											if (flag && flag2 && !blocked)
											{
												list4.Clear();
												break;
											}
										}
										if (list4.Count != 0)
										{
											if (list4.Count >= 2)
											{
												int num28 = TerrainMeta.Path.Rails.Count + list.Count;
												PathList pathList4 = this.CreateSegment(num28, list4.ToArray());
												pathList4.Start = false;
												pathList4.End = false;
												pathList4.ProcgenStartNode = node2;
												pathList4.ProcgenEndNode = node3;
												list.Add(pathList4);
											}
											k += num9;
										}
									}
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
					float num29 = Mathf.InverseLerp(0f, 8f, (float)i);
					float num30 = Mathf.InverseLerp((float)rail.Path.DefaultMaxIndex, (float)(rail.Path.DefaultMaxIndex - 8), (float)i);
					return Mathf.SmoothStep(0f, 1f, Mathf.Min(num29, num30));
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

	// Token: 0x060031E8 RID: 12776 RVA: 0x0012E868 File Offset: 0x0012CA68
	public PathFinder.Point GetPathFinderPoint(Vector3 worldPos, int res)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return new PathFinder.Point
		{
			x = Mathf.Clamp((int)(num * (float)res), 0, res - 1),
			y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1)
		};
	}

	// Token: 0x04002882 RID: 10370
	public const float Width = 4f;

	// Token: 0x04002883 RID: 10371
	public const float InnerPadding = 1f;

	// Token: 0x04002884 RID: 10372
	public const float OuterPadding = 1f;

	// Token: 0x04002885 RID: 10373
	public const float InnerFade = 1f;

	// Token: 0x04002886 RID: 10374
	public const float OuterFade = 32f;

	// Token: 0x04002887 RID: 10375
	public const float RandomScale = 1f;

	// Token: 0x04002888 RID: 10376
	public const float MeshOffset = 0f;

	// Token: 0x04002889 RID: 10377
	public const float TerrainOffset = -0.125f;

	// Token: 0x0400288A RID: 10378
	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x0400288B RID: 10379
	private const int MaxDepth = 250000;
}
