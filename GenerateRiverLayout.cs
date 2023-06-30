using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006D0 RID: 1744
public class GenerateRiverLayout : ProceduralComponent
{
	// Token: 0x06003201 RID: 12801 RVA: 0x0013066C File Offset: 0x0012E86C
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			TerrainMeta.Path.Rivers.Clear();
			TerrainMeta.Path.Rivers.AddRange(World.GetPaths("River"));
			return;
		}
		List<PathList> list = new List<PathList>();
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		List<Vector3> list2 = new List<Vector3>();
		for (float num = TerrainMeta.Position.z; num < TerrainMeta.Position.z + TerrainMeta.Size.z; num += 50f)
		{
			for (float num2 = TerrainMeta.Position.x; num2 < TerrainMeta.Position.x + TerrainMeta.Size.x; num2 += 50f)
			{
				Vector3 vector = new Vector3(num2, 0f, num);
				float num3 = (vector.y = heightMap.GetHeight(vector));
				if (vector.y > 5f)
				{
					Vector3 vector2 = heightMap.GetNormal(vector);
					if (vector2.y > 0.01f)
					{
						Vector2 vector3 = new Vector2(vector2.x, vector2.z).normalized;
						list2.Add(vector);
						float num4 = 18f;
						int num5 = 18;
						int i = 0;
						while (i < 10000)
						{
							vector.x += vector3.x;
							vector.z += vector3.y;
							if (heightMap.GetSlope(vector) > 30f)
							{
								break;
							}
							float height = heightMap.GetHeight(vector);
							if (height > num3 + 10f)
							{
								break;
							}
							float num6 = Mathf.Min(height, num3);
							vector.y = Mathf.Lerp(vector.y, num6, 0.15f);
							int topology = topologyMap.GetTopology(vector, num4);
							int topology2 = topologyMap.GetTopology(vector);
							int num7 = 3742724;
							int num8 = 128;
							if ((topology & num7) != 0)
							{
								list2.Add(vector);
								break;
							}
							if ((topology2 & num8) != 0 && --num5 <= 0)
							{
								list2.Add(vector);
								if (list2.Count >= 25)
								{
									int num9 = TerrainMeta.Path.Rivers.Count + list.Count;
									list.Add(new PathList("River " + num9, list2.ToArray())
									{
										Spline = true,
										Width = 36f,
										InnerPadding = 1f,
										OuterPadding = 1f,
										InnerFade = 10f,
										OuterFade = 20f,
										RandomScale = 0.75f,
										MeshOffset = -0.5f,
										TerrainOffset = -1.5f,
										Topology = 16384,
										Splat = 64,
										Start = true,
										End = true
									});
									break;
								}
								break;
							}
							else
							{
								if (i % 12 == 0)
								{
									list2.Add(vector);
								}
								vector2 = heightMap.GetNormal(vector);
								vector3 = new Vector2(vector3.x + 0.15f * vector2.x, vector3.y + 0.15f * vector2.z).normalized;
								num3 = num6;
								i++;
							}
						}
						list2.Clear();
					}
				}
			}
		}
		list.Sort((PathList a, PathList b) => b.Path.Points.Length.CompareTo(a.Path.Points.Length));
		int num10 = Mathf.RoundToInt(10f * TerrainMeta.Size.x * TerrainMeta.Size.z * 1E-06f);
		int num11 = Mathf.NextPowerOfTwo((int)(World.Size / 36f));
		bool[,] array = new bool[num11, num11];
		for (int j = 0; j < list.Count; j++)
		{
			if (j >= num10)
			{
				list.RemoveUnordered(j--);
			}
			else
			{
				PathList pathList = list[j];
				bool flag = false;
				for (int k = 0; k < j; k++)
				{
					if (Vector3.Distance(list[k].Path.GetStartPoint(), pathList.Path.GetStartPoint()) < 100f)
					{
						list.RemoveUnordered(j--);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					int num12 = -1;
					int num13 = -1;
					for (int l = 0; l < pathList.Path.Points.Length; l++)
					{
						Vector3 vector4 = pathList.Path.Points[l];
						int num14 = Mathf.Clamp((int)(TerrainMeta.NormalizeX(vector4.x) * (float)num11), 0, num11 - 1);
						int num15 = Mathf.Clamp((int)(TerrainMeta.NormalizeZ(vector4.z) * (float)num11), 0, num11 - 1);
						if (num12 != num14 || num13 != num15)
						{
							if (array[num15, num14])
							{
								list.RemoveUnordered(j--);
								break;
							}
							if (num12 != num14 && num13 != num15)
							{
								if (num12 != -1)
								{
									array[num15, num12] = true;
								}
								if (num13 != -1)
								{
									array[num13, num14] = true;
								}
								num12 = num14;
								num13 = num15;
								array[num15, num14] = true;
							}
							else
							{
								num12 = num14;
								num13 = num15;
								array[num15, num14] = true;
							}
						}
					}
				}
			}
		}
		for (int m = 0; m < list.Count; m++)
		{
			list[m].Name = "River " + (TerrainMeta.Path.Rivers.Count + m);
		}
		foreach (PathList pathList2 in list)
		{
			pathList2.Path.Smoothen(4, new Vector3(1f, 0f, 1f), null);
			pathList2.Path.Smoothen(8, new Vector3(0f, 1f, 0f), null);
			pathList2.Path.Resample(7.5f);
			pathList2.Path.RecalculateTangents();
		}
		TerrainMeta.Path.Rivers.AddRange(list);
	}

	// Token: 0x040028B6 RID: 10422
	public const float Width = 36f;

	// Token: 0x040028B7 RID: 10423
	public const float InnerPadding = 1f;

	// Token: 0x040028B8 RID: 10424
	public const float OuterPadding = 1f;

	// Token: 0x040028B9 RID: 10425
	public const float InnerFade = 10f;

	// Token: 0x040028BA RID: 10426
	public const float OuterFade = 20f;

	// Token: 0x040028BB RID: 10427
	public const float RandomScale = 0.75f;

	// Token: 0x040028BC RID: 10428
	public const float MeshOffset = -0.5f;

	// Token: 0x040028BD RID: 10429
	public const float TerrainOffset = -1.5f;
}
