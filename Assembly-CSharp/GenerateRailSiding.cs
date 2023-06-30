using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006CC RID: 1740
public class GenerateRailSiding : ProceduralComponent
{
	// Token: 0x060031F5 RID: 12789 RVA: 0x0012FC8C File Offset: 0x0012DE8C
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
			Hierarchy = 2
		};
	}

	// Token: 0x060031F6 RID: 12790 RVA: 0x0012FD2C File Offset: 0x0012DF2C
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
		int num3 = Mathf.RoundToInt(13.333333f);
		int num4 = Mathf.RoundToInt(20f);
		float num5 = 16f;
		float num6 = num5 * num5;
		List<PathList> list = new List<PathList>();
		int[,] array = TerrainPath.CreateRailCostmap(ref seed);
		new PathFinder(array, true, true);
		array.GetLength(0);
		List<Vector3> list2 = new List<Vector3>();
		List<Vector3> list3 = new List<Vector3>();
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
			int num7 = path.MinIndex + 1 + 16;
			int num8 = path.MaxIndex - 1 - 16;
			for (int k = num7; k <= num8; k++)
			{
				list2.Clear();
				list3.Clear();
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
						Vector3 vector5 = tangents[num11];
						Vector3 vector6 = tangents[num12];
						Vector3 vector7 = Vector3.Normalize(points3[num11 + 8] - points3[num11]);
						Vector3 vector8 = Vector3.Normalize(points3[num12] - points3[num12 - 8]);
						float num13 = Vector3.SignedAngle(vector7, vector5, Vector3.up);
						float num14 = -Vector3.SignedAngle(vector8, vector6, Vector3.up);
						if (Mathf.Sign(num13) == Mathf.Sign(num14) && Mathf.Abs(num13) <= 60f && Mathf.Abs(num14) <= 60f)
						{
							float num15 = 5f;
							Quaternion quaternion = ((num13 > 0f) ? GenerateRailSiding.rotRight : GenerateRailSiding.rotLeft);
							for (int l = num11 - 8; l <= num12 + 8; l++)
							{
								Vector3 vector9 = points3[l];
								if (hashSet.Contains(vector9))
								{
									list2.Clear();
									list3.Clear();
									break;
								}
								Vector3 vector10 = tangents[l];
								Vector3 vector11 = quaternion * vector10;
								if (l < num11 + 8)
								{
									float num16 = Mathf.InverseLerp((float)(num11 - 8), (float)num11, (float)l);
									float num17 = Mathf.SmoothStep(0f, 1f, num16) * num15;
									vector9 += vector11 * num17;
								}
								else if (l > num12 - 8)
								{
									float num18 = Mathf.InverseLerp((float)(num12 + 8), (float)num12, (float)l);
									float num19 = Mathf.SmoothStep(0f, 1f, num18) * num15;
									vector9 += vector11 * num19;
								}
								else
								{
									vector9 += vector11 * num15;
								}
								list2.Add(vector9);
								list3.Add(vector10);
							}
							if (list2.Count >= 2)
							{
								int num20 = TerrainMeta.Path.Rails.Count + list.Count;
								PathList pathList4 = this.CreateSegment(num20, list2.ToArray());
								pathList4.Start = false;
								pathList4.End = false;
								list.Add(pathList4);
								k += num9;
							}
							k += num10;
						}
					}
				}
			}
		}
		foreach (PathList pathList5 in list)
		{
			pathList5.Path.Resample(7.5f);
			pathList5.Path.RecalculateTangents();
			pathList5.AdjustPlacementMap(20f);
		}
		TerrainMeta.Path.Rails.AddRange(list);
	}

	// Token: 0x060031F7 RID: 12791 RVA: 0x00130294 File Offset: 0x0012E494
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

	// Token: 0x040028A6 RID: 10406
	public const float Width = 4f;

	// Token: 0x040028A7 RID: 10407
	public const float InnerPadding = 1f;

	// Token: 0x040028A8 RID: 10408
	public const float OuterPadding = 1f;

	// Token: 0x040028A9 RID: 10409
	public const float InnerFade = 1f;

	// Token: 0x040028AA RID: 10410
	public const float OuterFade = 32f;

	// Token: 0x040028AB RID: 10411
	public const float RandomScale = 1f;

	// Token: 0x040028AC RID: 10412
	public const float MeshOffset = 0f;

	// Token: 0x040028AD RID: 10413
	public const float TerrainOffset = -0.125f;

	// Token: 0x040028AE RID: 10414
	private static Quaternion rotRight = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x040028AF RID: 10415
	private static Quaternion rotLeft = Quaternion.Euler(0f, -90f, 0f);

	// Token: 0x040028B0 RID: 10416
	private const int MaxDepth = 250000;
}
