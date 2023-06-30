using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200065D RID: 1629
public class PathList
{
	// Token: 0x06002F32 RID: 12082 RVA: 0x0011C075 File Offset: 0x0011A275
	public PathList(string name, Vector3[] points)
	{
		this.Name = name;
		this.Path = new PathInterpolator(points);
	}

	// Token: 0x06002F33 RID: 12083 RVA: 0x0011C090 File Offset: 0x0011A290
	private void SpawnObjectsNeighborAligned(ref uint seed, Prefab[] prefabs, List<Vector3> positions, SpawnFilter filter = null)
	{
		if (positions.Count < 2)
		{
			return;
		}
		List<Prefab> list = Pool.GetList<Prefab>();
		for (int i = 0; i < positions.Count; i++)
		{
			int num = Mathf.Max(i - 1, 0);
			int num2 = Mathf.Min(i + 1, positions.Count - 1);
			Vector3 vector = positions[i];
			Quaternion quaternion = Quaternion.LookRotation((positions[num2] - positions[num]).XZ3D());
			Prefab prefab;
			this.SpawnObject(ref seed, prefabs, vector, quaternion, list, out prefab, positions.Count, i, filter);
			if (prefab != null)
			{
				list.Add(prefab);
			}
		}
		Pool.FreeList<Prefab>(ref list);
	}

	// Token: 0x06002F34 RID: 12084 RVA: 0x0011C130 File Offset: 0x0011A330
	private bool SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 position, Quaternion rotation, SpawnFilter filter = null)
	{
		Prefab random = prefabs.GetRandom(ref seed);
		Vector3 vector = position;
		Quaternion quaternion = rotation;
		Vector3 localScale = random.Object.transform.localScale;
		random.ApplyDecorComponents(ref vector, ref quaternion, ref localScale);
		if (!random.ApplyTerrainAnchors(ref vector, quaternion, localScale, filter))
		{
			return false;
		}
		World.AddPrefab(this.Name, random, vector, quaternion, localScale);
		return true;
	}

	// Token: 0x06002F35 RID: 12085 RVA: 0x0011C188 File Offset: 0x0011A388
	private bool SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 position, Quaternion rotation, List<Prefab> previousSpawns, out Prefab spawned, int pathLength, int index, SpawnFilter filter = null)
	{
		spawned = null;
		Prefab random = prefabs.GetRandom(ref seed);
		random.ApplySequenceReplacement(previousSpawns, ref random, prefabs, pathLength, index);
		Vector3 vector = position;
		Quaternion quaternion = rotation;
		Vector3 localScale = random.Object.transform.localScale;
		random.ApplyDecorComponents(ref vector, ref quaternion, ref localScale);
		if (!random.ApplyTerrainAnchors(ref vector, quaternion, localScale, filter))
		{
			return false;
		}
		World.AddPrefab(this.Name, random, vector, quaternion, localScale);
		spawned = random;
		return true;
	}

	// Token: 0x06002F36 RID: 12086 RVA: 0x0011C1F8 File Offset: 0x0011A3F8
	private bool CheckObjects(Prefab[] prefabs, Vector3 position, Quaternion rotation, SpawnFilter filter = null)
	{
		foreach (Prefab prefab in prefabs)
		{
			Vector3 vector = position;
			Vector3 localScale = prefab.Object.transform.localScale;
			if (!prefab.ApplyTerrainAnchors(ref vector, rotation, localScale, filter))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002F37 RID: 12087 RVA: 0x0011C23C File Offset: 0x0011A43C
	private void SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 pos, Vector3 dir, PathList.BasicObject obj)
	{
		if (!obj.AlignToNormal)
		{
			dir = dir.XZ3D().normalized;
		}
		SpawnFilter filter = obj.Filter;
		Vector3 vector = (this.Width * 0.5f + obj.Offset) * (PathList.rot90 * dir);
		for (int i = 0; i < PathList.placements.Length; i++)
		{
			if ((obj.Placement != PathList.Placement.Center || i == 0) && (obj.Placement != PathList.Placement.Side || i != 0))
			{
				Vector3 vector2 = pos + PathList.placements[i] * vector;
				if (obj.HeightToTerrain)
				{
					vector2.y = TerrainMeta.HeightMap.GetHeight(vector2);
				}
				if (filter.Test(vector2))
				{
					Quaternion quaternion = ((i == 2) ? Quaternion.LookRotation(PathList.rot180 * dir) : Quaternion.LookRotation(dir));
					if (this.SpawnObject(ref seed, prefabs, vector2, quaternion, filter))
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06002F38 RID: 12088 RVA: 0x0011C32C File Offset: 0x0011A52C
	private bool CheckObjects(Prefab[] prefabs, Vector3 pos, Vector3 dir, PathList.BasicObject obj)
	{
		if (!obj.AlignToNormal)
		{
			dir = dir.XZ3D().normalized;
		}
		SpawnFilter filter = obj.Filter;
		Vector3 vector = (this.Width * 0.5f + obj.Offset) * (PathList.rot90 * dir);
		for (int i = 0; i < PathList.placements.Length; i++)
		{
			if ((obj.Placement != PathList.Placement.Center || i == 0) && (obj.Placement != PathList.Placement.Side || i != 0))
			{
				Vector3 vector2 = pos + PathList.placements[i] * vector;
				if (obj.HeightToTerrain)
				{
					vector2.y = TerrainMeta.HeightMap.GetHeight(vector2);
				}
				if (filter.Test(vector2))
				{
					Quaternion quaternion = ((i == 2) ? Quaternion.LookRotation(PathList.rot180 * dir) : Quaternion.LookRotation(dir));
					if (this.CheckObjects(prefabs, vector2, quaternion, filter))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06002F39 RID: 12089 RVA: 0x0011C41C File Offset: 0x0011A61C
	public void SpawnSide(ref uint seed, PathList.SideObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		PathList.Side side = obj.Side;
		SpawnFilter filter = obj.Filter;
		float density = obj.Density;
		float distance = obj.Distance;
		float num = this.Width * 0.5f + obj.Offset;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		float[] array2 = new float[]
		{
			-num,
			num
		};
		int num2 = 0;
		Vector3 vector = this.Path.GetStartPoint();
		List<Vector3> list = new List<Vector3>();
		float num3 = distance * 0.25f;
		float num4 = distance * 0.5f;
		float num5 = this.Path.StartOffset + num4;
		float num6 = this.Path.Length - this.Path.EndOffset - num4;
		for (float num7 = num5; num7 <= num6; num7 += num3)
		{
			Vector3 vector2 = (this.Spline ? this.Path.GetPointCubicHermite(num7) : this.Path.GetPoint(num7));
			if ((vector2 - vector).magnitude >= distance)
			{
				Vector3 tangent = this.Path.GetTangent(num7);
				Vector3 vector3 = PathList.rot90 * tangent;
				for (int i = 0; i < array2.Length; i++)
				{
					int num8 = (num2 + i) % array2.Length;
					if ((side != PathList.Side.Left || num8 == 0) && (side != PathList.Side.Right || num8 == 1))
					{
						float num9 = array2[num8];
						Vector3 vector4 = vector2;
						vector4.x += vector3.x * num9;
						vector4.z += vector3.z * num9;
						float num10 = TerrainMeta.NormalizeX(vector4.x);
						float num11 = TerrainMeta.NormalizeZ(vector4.z);
						if (filter.GetFactor(num10, num11, true) >= SeedRandom.Value(ref seed))
						{
							if (density >= SeedRandom.Value(ref seed))
							{
								vector4.y = heightMap.GetHeight(num10, num11);
								if (obj.Alignment == PathList.Alignment.None)
								{
									if (!this.SpawnObject(ref seed, array, vector4, Quaternion.LookRotation(Vector3.zero), filter))
									{
										goto IL_284;
									}
								}
								else if (obj.Alignment == PathList.Alignment.Forward)
								{
									if (!this.SpawnObject(ref seed, array, vector4, Quaternion.LookRotation(tangent * num9), filter))
									{
										goto IL_284;
									}
								}
								else if (obj.Alignment == PathList.Alignment.Inward)
								{
									if (!this.SpawnObject(ref seed, array, vector4, Quaternion.LookRotation(tangent * num9) * PathList.rot270, filter))
									{
										goto IL_284;
									}
								}
								else
								{
									list.Add(vector4);
								}
							}
							num2 = num8;
							vector = vector2;
							if (side == PathList.Side.Any)
							{
								break;
							}
						}
					}
					IL_284:;
				}
			}
		}
		if (list.Count > 0)
		{
			this.SpawnObjectsNeighborAligned(ref seed, array, list, filter);
		}
	}

	// Token: 0x06002F3A RID: 12090 RVA: 0x0011C6E4 File Offset: 0x0011A8E4
	public void SpawnAlong(ref uint seed, PathList.PathObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		SpawnFilter filter = obj.Filter;
		float density = obj.Density;
		float distance = obj.Distance;
		float dithering = obj.Dithering;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 vector = this.Path.GetStartPoint();
		List<Vector3> list = new List<Vector3>();
		float num = distance * 0.25f;
		float num2 = distance * 0.5f;
		float num3 = this.Path.StartOffset + num2;
		float num4 = this.Path.Length - this.Path.EndOffset - num2;
		for (float num5 = num3; num5 <= num4; num5 += num)
		{
			Vector3 vector2 = (this.Spline ? this.Path.GetPointCubicHermite(num5) : this.Path.GetPoint(num5));
			if ((vector2 - vector).magnitude >= distance)
			{
				Vector3 tangent = this.Path.GetTangent(num5);
				Vector3 vector3 = PathList.rot90 * tangent;
				Vector3 vector4 = vector2;
				vector4.x += SeedRandom.Range(ref seed, -dithering, dithering);
				vector4.z += SeedRandom.Range(ref seed, -dithering, dithering);
				float num6 = TerrainMeta.NormalizeX(vector4.x);
				float num7 = TerrainMeta.NormalizeZ(vector4.z);
				if (filter.GetFactor(num6, num7, true) >= SeedRandom.Value(ref seed))
				{
					if (density >= SeedRandom.Value(ref seed))
					{
						vector4.y = heightMap.GetHeight(num6, num7);
						if (obj.Alignment == PathList.Alignment.None)
						{
							if (!this.SpawnObject(ref seed, array, vector4, Quaternion.identity, filter))
							{
								goto IL_1FE;
							}
						}
						else if (obj.Alignment == PathList.Alignment.Forward)
						{
							if (!this.SpawnObject(ref seed, array, vector4, Quaternion.LookRotation(tangent), filter))
							{
								goto IL_1FE;
							}
						}
						else if (obj.Alignment == PathList.Alignment.Inward)
						{
							if (!this.SpawnObject(ref seed, array, vector4, Quaternion.LookRotation(vector3), filter))
							{
								goto IL_1FE;
							}
						}
						else
						{
							list.Add(vector4);
						}
					}
					vector = vector2;
				}
			}
			IL_1FE:;
		}
		if (list.Count > 0)
		{
			this.SpawnObjectsNeighborAligned(ref seed, array, list, filter);
		}
	}

	// Token: 0x06002F3B RID: 12091 RVA: 0x0011C914 File Offset: 0x0011AB14
	public void SpawnBridge(ref uint seed, PathList.BridgeObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 vector = this.Path.GetEndPoint() - startPoint;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector / magnitude;
		float num = magnitude / obj.Distance;
		int num2 = Mathf.RoundToInt(num);
		float num3 = 0.5f * (num - (float)num2);
		Vector3 vector3 = obj.Distance * vector2;
		Vector3 vector4 = startPoint + (0.5f + num3) * vector3;
		Quaternion quaternion = Quaternion.LookRotation(vector2);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainWaterMap waterMap = TerrainMeta.WaterMap;
		for (int i = 0; i < num2; i++)
		{
			float num4 = Mathf.Max(heightMap.GetHeight(vector4), waterMap.GetHeight(vector4)) - 1f;
			if (vector4.y > num4)
			{
				this.SpawnObject(ref seed, array, vector4, quaternion, null);
			}
			vector4 += vector3;
		}
	}

	// Token: 0x06002F3C RID: 12092 RVA: 0x0011CA48 File Offset: 0x0011AC48
	public void SpawnStart(ref uint seed, PathList.BasicObject obj)
	{
		if (!this.Start)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 startTangent = this.Path.GetStartTangent();
		this.SpawnObject(ref seed, array, startPoint, startTangent, obj);
	}

	// Token: 0x06002F3D RID: 12093 RVA: 0x0011CAC8 File Offset: 0x0011ACC8
	public void SpawnEnd(ref uint seed, PathList.BasicObject obj)
	{
		if (!this.End)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 vector = -this.Path.GetEndTangent();
		this.SpawnObject(ref seed, array, endPoint, vector, obj);
	}

	// Token: 0x06002F3E RID: 12094 RVA: 0x0011CB4C File Offset: 0x0011AD4C
	public void TrimStart(PathList.BasicObject obj)
	{
		if (!this.Start)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3[] points = this.Path.Points;
		Vector3[] tangents = this.Path.Tangents;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 vector = points[this.Path.MinIndex + i];
			Vector3 vector2 = tangents[this.Path.MinIndex + i];
			if (this.CheckObjects(array, vector, vector2, obj))
			{
				this.Path.MinIndex += i;
				return;
			}
		}
	}

	// Token: 0x06002F3F RID: 12095 RVA: 0x0011CC24 File Offset: 0x0011AE24
	public void TrimEnd(PathList.BasicObject obj)
	{
		if (!this.End)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3[] points = this.Path.Points;
		Vector3[] tangents = this.Path.Tangents;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 vector = points[this.Path.MaxIndex - i];
			Vector3 vector2 = -tangents[this.Path.MaxIndex - i];
			if (this.CheckObjects(array, vector, vector2, obj))
			{
				this.Path.MaxIndex -= i;
				return;
			}
		}
	}

	// Token: 0x06002F40 RID: 12096 RVA: 0x0011CD00 File Offset: 0x0011AF00
	public void TrimTopology(int topology)
	{
		Vector3[] points = this.Path.Points;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 vector = points[this.Path.MinIndex + i];
			if (!TerrainMeta.TopologyMap.GetTopology(vector, topology))
			{
				this.Path.MinIndex += i;
				break;
			}
		}
		for (int j = 0; j < num; j++)
		{
			Vector3 vector2 = points[this.Path.MaxIndex - j];
			if (!TerrainMeta.TopologyMap.GetTopology(vector2, topology))
			{
				this.Path.MaxIndex -= j;
				return;
			}
		}
	}

	// Token: 0x06002F41 RID: 12097 RVA: 0x0011CDAC File Offset: 0x0011AFAC
	public void ResetTrims()
	{
		this.Path.MinIndex = this.Path.DefaultMinIndex;
		this.Path.MaxIndex = this.Path.DefaultMaxIndex;
	}

	// Token: 0x06002F42 RID: 12098 RVA: 0x0011CDDC File Offset: 0x0011AFDC
	public void AdjustTerrainHeight(float intensity = 1f, float fade = 1f)
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float num = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float outerFade = this.OuterFade * fade;
		float innerFade = this.InnerFade;
		float offset01 = this.TerrainOffset * TerrainMeta.OneOverSize.y;
		float num2 = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 startTangent = this.Path.GetStartTangent();
		Vector3 vector = startTangent.XZ3D().normalized;
		Vector3 vector2 = PathList.rot90 * vector;
		Vector3 vector3 = startPoint;
		Vector3 vector4 = startTangent;
		Line prev_line = new Line(startPoint, startPoint + startTangent * num);
		Vector3 vector5 = startPoint - vector2 * (num2 + outerPadding + outerFade);
		Vector3 vector6 = startPoint + vector2 * (num2 + outerPadding + outerFade);
		Vector3 vector7 = vector3;
		Vector3 vector8 = vector4;
		Line cur_line = prev_line;
		float num3 = this.Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 vector9 = (this.Spline ? this.Path.GetPointCubicHermite(num4 + num) : this.Path.GetPoint(num4 + num));
			Vector3 tangent = this.Path.GetTangent(num4 + num);
			Line next_line = new Line(vector9, vector9 + tangent * num);
			float opacity = 1f;
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(vector7.x, vector7.z, 2, 0.005f, 1f, 2f, 0.5f));
			if (!this.Path.Circular)
			{
				float num5 = (startPoint - vector7).Magnitude2D();
				float num6 = (endPoint - vector7).Magnitude2D();
				opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(num5, num6));
			}
			vector = vector8.XZ3D().normalized;
			vector2 = PathList.rot90 * vector;
			Vector3 vector10 = vector7 - vector2 * (radius + outerPadding + outerFade);
			Vector3 vector11 = vector7 + vector2 * (radius + outerPadding + outerFade);
			float yn = TerrainMeta.NormalizeY((vector7.y + vector3.y) * 0.5f);
			heightmap.ForEach(vector5, vector6, vector10, vector11, delegate(int x, int z)
			{
				float num7 = heightmap.Coordinate(x);
				float num8 = heightmap.Coordinate(z);
				Vector3 vector12 = TerrainMeta.Denormalize(new Vector3(num7, yn, num8));
				Vector3 vector13 = prev_line.ClosestPoint2D(vector12);
				Vector3 vector14 = cur_line.ClosestPoint2D(vector12);
				Vector3 vector15 = next_line.ClosestPoint2D(vector12);
				float num9 = (vector12 - vector13).Magnitude2D();
				float num10 = (vector12 - vector14).Magnitude2D();
				float num11 = (vector12 - vector15).Magnitude2D();
				float num12 = num10;
				Vector3 vector16 = vector14;
				if (num10 > num9 || num10 > num11)
				{
					if (num9 <= num11)
					{
						num12 = num9;
						vector16 = vector13;
					}
					else
					{
						num12 = num11;
						vector16 = vector15;
					}
				}
				float num13 = Mathf.InverseLerp(radius + outerPadding + outerFade, radius + outerPadding, num12);
				float num14 = Mathf.InverseLerp(radius - innerPadding, radius - innerPadding - innerFade, num12);
				float num15 = TerrainMeta.NormalizeY(vector16.y);
				heightmap.SetHeight(x, z, num15 + Mathf.SmoothStep(0f, offset01, num14), intensity * opacity * num13);
			});
			vector3 = vector7;
			vector5 = vector10;
			vector6 = vector11;
			prev_line = cur_line;
			vector7 = vector9;
			vector8 = tangent;
			cur_line = next_line;
		}
	}

	// Token: 0x06002F43 RID: 12099 RVA: 0x0011D150 File Offset: 0x0011B350
	public void AdjustTerrainTexture()
	{
		if (this.Splat == 0)
		{
			return;
		}
		TerrainSplatMap splatmap = TerrainMeta.SplatMap;
		float num = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float num2 = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 vector = this.Path.GetStartTangent();
		Vector3 vector2 = vector.XZ3D().normalized;
		Vector3 vector3 = PathList.rot90 * vector2;
		Vector3 vector4 = startPoint - vector3 * (num2 + outerPadding);
		Vector3 vector5 = startPoint + vector3 * (num2 + outerPadding);
		float num3 = this.Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 vector6 = (this.Spline ? this.Path.GetPointCubicHermite(num4) : this.Path.GetPoint(num4));
			float opacity = 1f;
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(vector6.x, vector6.z, 2, 0.005f, 1f, 2f, 0.5f));
			if (!this.Path.Circular)
			{
				float num5 = (startPoint - vector6).Magnitude2D();
				float num6 = (endPoint - vector6).Magnitude2D();
				opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(num5, num6));
			}
			vector = this.Path.GetTangent(num4);
			vector2 = vector.XZ3D().normalized;
			vector3 = PathList.rot90 * vector2;
			Ray ray = new Ray(vector6, vector);
			Vector3 vector7 = vector6 - vector3 * (radius + outerPadding);
			Vector3 vector8 = vector6 + vector3 * (radius + outerPadding);
			float yn = TerrainMeta.NormalizeY(vector6.y);
			splatmap.ForEach(vector4, vector5, vector7, vector8, delegate(int x, int z)
			{
				float num7 = splatmap.Coordinate(x);
				float num8 = splatmap.Coordinate(z);
				Vector3 vector9 = TerrainMeta.Denormalize(new Vector3(num7, yn, num8));
				Vector3 vector10 = ray.ClosestPoint(vector9);
				float num9 = (vector9 - vector10).Magnitude2D();
				float num10 = Mathf.InverseLerp(radius + outerPadding, radius - innerPadding, num9);
				splatmap.SetSplat(x, z, this.Splat, num10 * opacity);
			});
			vector4 = vector7;
			vector5 = vector8;
		}
	}

	// Token: 0x06002F44 RID: 12100 RVA: 0x0011D3DC File Offset: 0x0011B5DC
	public void AdjustTerrainTopology()
	{
		if (this.Topology == 0)
		{
			return;
		}
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		float num = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float num2 = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 vector = this.Path.GetStartTangent();
		Vector3 vector2 = vector.XZ3D().normalized;
		Vector3 vector3 = PathList.rot90 * vector2;
		Vector3 vector4 = startPoint - vector3 * (num2 + outerPadding);
		Vector3 vector5 = startPoint + vector3 * (num2 + outerPadding);
		float num3 = this.Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 vector6 = (this.Spline ? this.Path.GetPointCubicHermite(num4) : this.Path.GetPoint(num4));
			float opacity = 1f;
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(vector6.x, vector6.z, 2, 0.005f, 1f, 2f, 0.5f));
			if (!this.Path.Circular)
			{
				float num5 = (startPoint - vector6).Magnitude2D();
				float num6 = (endPoint - vector6).Magnitude2D();
				opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(num5, num6));
			}
			vector = this.Path.GetTangent(num4);
			vector2 = vector.XZ3D().normalized;
			vector3 = PathList.rot90 * vector2;
			Ray ray = new Ray(vector6, vector);
			Vector3 vector7 = vector6 - vector3 * (radius + outerPadding);
			Vector3 vector8 = vector6 + vector3 * (radius + outerPadding);
			float yn = TerrainMeta.NormalizeY(vector6.y);
			topomap.ForEach(vector4, vector5, vector7, vector8, delegate(int x, int z)
			{
				float num7 = topomap.Coordinate(x);
				float num8 = topomap.Coordinate(z);
				Vector3 vector9 = TerrainMeta.Denormalize(new Vector3(num7, yn, num8));
				Vector3 vector10 = ray.ClosestPoint(vector9);
				float num9 = (vector9 - vector10).Magnitude2D();
				if (Mathf.InverseLerp(radius + outerPadding, radius - innerPadding, num9) * opacity > 0.3f)
				{
					topomap.AddTopology(x, z, this.Topology);
				}
			});
			vector4 = vector7;
			vector5 = vector8;
		}
	}

	// Token: 0x06002F45 RID: 12101 RVA: 0x0011D668 File Offset: 0x0011B868
	public void AdjustPlacementMap(float width)
	{
		TerrainPlacementMap placementmap = TerrainMeta.PlacementMap;
		float num = 1f;
		float radius = width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		this.Path.GetEndPoint();
		Vector3 vector = this.Path.GetStartTangent();
		Vector3 vector2 = vector.XZ3D().normalized;
		Vector3 vector3 = PathList.rot90 * vector2;
		Vector3 vector4 = startPoint - vector3 * radius;
		Vector3 vector5 = startPoint + vector3 * radius;
		float num2 = this.Path.Length + num;
		for (float num3 = 0f; num3 < num2; num3 += num)
		{
			Vector3 vector6 = (this.Spline ? this.Path.GetPointCubicHermite(num3) : this.Path.GetPoint(num3));
			vector = this.Path.GetTangent(num3);
			vector2 = vector.XZ3D().normalized;
			vector3 = PathList.rot90 * vector2;
			Ray ray = new Ray(vector6, vector);
			Vector3 vector7 = vector6 - vector3 * radius;
			Vector3 vector8 = vector6 + vector3 * radius;
			float yn = TerrainMeta.NormalizeY(vector6.y);
			placementmap.ForEach(vector4, vector5, vector7, vector8, delegate(int x, int z)
			{
				float num4 = placementmap.Coordinate(x);
				float num5 = placementmap.Coordinate(z);
				Vector3 vector9 = TerrainMeta.Denormalize(new Vector3(num4, yn, num5));
				Vector3 vector10 = ray.ClosestPoint(vector9);
				if ((vector9 - vector10).Magnitude2D() <= radius)
				{
					placementmap.SetBlocked(x, z);
				}
			});
			vector4 = vector7;
			vector5 = vector8;
		}
	}

	// Token: 0x06002F46 RID: 12102 RVA: 0x0011D814 File Offset: 0x0011BA14
	public List<PathList.MeshObject> CreateMesh(Mesh[] meshes, float normalSmoothing, bool snapToTerrain, bool snapStartToTerrain, bool snapEndToTerrain)
	{
		MeshCache.Data[] array = new MeshCache.Data[meshes.Length];
		MeshData[] array2 = new MeshData[meshes.Length];
		for (int i = 0; i < meshes.Length; i++)
		{
			array[i] = MeshCache.Get(meshes[i]);
			array2[i] = new MeshData();
		}
		MeshData[] array3 = array2;
		for (int j = 0; j < array3.Length; j++)
		{
			array3[j].AllocMinimal();
		}
		Bounds bounds = meshes[meshes.Length - 1].bounds;
		Vector3 min = bounds.min;
		Vector3 size = bounds.size;
		float num = this.Width / bounds.size.x;
		List<PathList.MeshObject> list = new List<PathList.MeshObject>();
		int num2 = (int)(this.Path.Length / (num * bounds.size.z));
		int num3 = 5;
		float num4 = this.Path.Length / (float)num2;
		float randomScale = this.RandomScale;
		float meshOffset = this.MeshOffset;
		float num5 = this.Width * 0.5f;
		int num6 = array[0].vertices.Length;
		int num7 = array[0].triangles.Length;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		for (int k = 0; k < num2; k += num3)
		{
			float num8 = (float)k * num4 + 0.5f * (float)num3 * num4;
			Vector3 vector = (this.Spline ? this.Path.GetPointCubicHermite(num8) : this.Path.GetPoint(num8));
			int num9 = 0;
			while (num9 < num3 && k + num9 < num2)
			{
				float num10 = (float)(k + num9) * num4;
				for (int l = 0; l < meshes.Length; l++)
				{
					MeshCache.Data data = array[l];
					MeshData meshData = array2[l];
					int count = meshData.vertices.Count;
					for (int m = 0; m < data.vertices.Length; m++)
					{
						Vector2 vector2 = data.uv[m];
						Vector3 vector3 = data.vertices[m];
						Vector3 vector4 = data.normals[m];
						Vector4 vector5 = data.tangents[m];
						float num11 = (vector3.x - min.x) / size.x;
						float num12 = vector3.y - min.y;
						float num13 = (vector3.z - min.z) / size.z;
						float num14 = num10 + num13 * num4;
						Vector3 vector6 = (this.Spline ? this.Path.GetPointCubicHermite(num14) : this.Path.GetPoint(num14));
						Vector3 tangent = this.Path.GetTangent(num14);
						Vector3 normalized = tangent.XZ3D().normalized;
						Vector3 vector7 = PathList.rot90 * normalized;
						Vector3 vector8 = Vector3.Cross(tangent, vector7);
						Quaternion quaternion = Quaternion.LookRotation(normalized, vector8);
						float num15 = Mathf.Lerp(num5, num5 * randomScale, Noise.Billow(vector6.x, vector6.z, 2, 0.005f, 1f, 2f, 0.5f));
						Vector3 vector9 = vector6 - vector7 * num15;
						Vector3 vector10 = vector6 + vector7 * num15;
						if (snapToTerrain)
						{
							vector9.y = heightMap.GetHeight(vector9);
							vector10.y = heightMap.GetHeight(vector10);
						}
						vector9 += vector8 * meshOffset;
						vector10 += vector8 * meshOffset;
						vector3 = Vector3.Lerp(vector9, vector10, num11);
						if ((snapStartToTerrain && num14 < 0.1f) || (snapEndToTerrain && num14 > this.Path.Length - 0.1f))
						{
							vector3.y = heightMap.GetHeight(vector3);
						}
						else
						{
							vector3.y += num12;
						}
						vector3 -= vector;
						vector4 = quaternion * vector4;
						vector5 = quaternion * vector5;
						if (normalSmoothing > 0f)
						{
							vector4 = Vector3.Slerp(vector4, Vector3.up, normalSmoothing);
						}
						meshData.vertices.Add(vector3);
						meshData.normals.Add(vector4);
						meshData.tangents.Add(vector5);
						meshData.uv.Add(vector2);
					}
					for (int n = 0; n < data.triangles.Length; n++)
					{
						int num16 = data.triangles[n];
						meshData.triangles.Add(count + num16);
					}
				}
				num9++;
			}
			list.Add(new PathList.MeshObject(vector, array2));
			array3 = array2;
			for (int j = 0; j < array3.Length; j++)
			{
				array3[j].Clear();
			}
		}
		array3 = array2;
		for (int j = 0; j < array3.Length; j++)
		{
			array3[j].Free();
		}
		return list;
	}

	// Token: 0x040026E3 RID: 9955
	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x040026E4 RID: 9956
	private static Quaternion rot180 = Quaternion.Euler(0f, 180f, 0f);

	// Token: 0x040026E5 RID: 9957
	private static Quaternion rot270 = Quaternion.Euler(0f, 270f, 0f);

	// Token: 0x040026E6 RID: 9958
	public string Name;

	// Token: 0x040026E7 RID: 9959
	public PathInterpolator Path;

	// Token: 0x040026E8 RID: 9960
	public bool Spline;

	// Token: 0x040026E9 RID: 9961
	public bool Start;

	// Token: 0x040026EA RID: 9962
	public bool End;

	// Token: 0x040026EB RID: 9963
	public float Width;

	// Token: 0x040026EC RID: 9964
	public float InnerPadding;

	// Token: 0x040026ED RID: 9965
	public float OuterPadding;

	// Token: 0x040026EE RID: 9966
	public float InnerFade;

	// Token: 0x040026EF RID: 9967
	public float OuterFade;

	// Token: 0x040026F0 RID: 9968
	public float RandomScale;

	// Token: 0x040026F1 RID: 9969
	public float MeshOffset;

	// Token: 0x040026F2 RID: 9970
	public float TerrainOffset;

	// Token: 0x040026F3 RID: 9971
	public int Topology;

	// Token: 0x040026F4 RID: 9972
	public int Splat;

	// Token: 0x040026F5 RID: 9973
	public int Hierarchy;

	// Token: 0x040026F6 RID: 9974
	public PathFinder.Node ProcgenStartNode;

	// Token: 0x040026F7 RID: 9975
	public PathFinder.Node ProcgenEndNode;

	// Token: 0x040026F8 RID: 9976
	public const float StepSize = 1f;

	// Token: 0x040026F9 RID: 9977
	private static float[] placements = new float[] { 0f, -1f, 1f };

	// Token: 0x02000DBD RID: 3517
	public enum Side
	{
		// Token: 0x04004938 RID: 18744
		Both,
		// Token: 0x04004939 RID: 18745
		Left,
		// Token: 0x0400493A RID: 18746
		Right,
		// Token: 0x0400493B RID: 18747
		Any
	}

	// Token: 0x02000DBE RID: 3518
	public enum Placement
	{
		// Token: 0x0400493D RID: 18749
		Center,
		// Token: 0x0400493E RID: 18750
		Side
	}

	// Token: 0x02000DBF RID: 3519
	public enum Alignment
	{
		// Token: 0x04004940 RID: 18752
		None,
		// Token: 0x04004941 RID: 18753
		Neighbor,
		// Token: 0x04004942 RID: 18754
		Forward,
		// Token: 0x04004943 RID: 18755
		Inward
	}

	// Token: 0x02000DC0 RID: 3520
	[Serializable]
	public class BasicObject
	{
		// Token: 0x04004944 RID: 18756
		public string Folder;

		// Token: 0x04004945 RID: 18757
		public SpawnFilter Filter;

		// Token: 0x04004946 RID: 18758
		public PathList.Placement Placement;

		// Token: 0x04004947 RID: 18759
		public bool AlignToNormal = true;

		// Token: 0x04004948 RID: 18760
		public bool HeightToTerrain = true;

		// Token: 0x04004949 RID: 18761
		public float Offset;
	}

	// Token: 0x02000DC1 RID: 3521
	[Serializable]
	public class SideObject
	{
		// Token: 0x0400494A RID: 18762
		public string Folder;

		// Token: 0x0400494B RID: 18763
		public SpawnFilter Filter;

		// Token: 0x0400494C RID: 18764
		public PathList.Side Side;

		// Token: 0x0400494D RID: 18765
		public PathList.Alignment Alignment;

		// Token: 0x0400494E RID: 18766
		public float Density = 1f;

		// Token: 0x0400494F RID: 18767
		public float Distance = 25f;

		// Token: 0x04004950 RID: 18768
		public float Offset = 2f;
	}

	// Token: 0x02000DC2 RID: 3522
	[Serializable]
	public class PathObject
	{
		// Token: 0x04004951 RID: 18769
		public string Folder;

		// Token: 0x04004952 RID: 18770
		public SpawnFilter Filter;

		// Token: 0x04004953 RID: 18771
		public PathList.Alignment Alignment;

		// Token: 0x04004954 RID: 18772
		public float Density = 1f;

		// Token: 0x04004955 RID: 18773
		public float Distance = 5f;

		// Token: 0x04004956 RID: 18774
		public float Dithering = 5f;
	}

	// Token: 0x02000DC3 RID: 3523
	[Serializable]
	public class BridgeObject
	{
		// Token: 0x04004957 RID: 18775
		public string Folder;

		// Token: 0x04004958 RID: 18776
		public float Distance = 10f;
	}

	// Token: 0x02000DC4 RID: 3524
	public class MeshObject
	{
		// Token: 0x0600518C RID: 20876 RVA: 0x001ACBEC File Offset: 0x001AADEC
		public MeshObject(Vector3 meshPivot, MeshData[] meshData)
		{
			this.Position = meshPivot;
			this.Meshes = new Mesh[meshData.Length];
			for (int i = 0; i < this.Meshes.Length; i++)
			{
				MeshData meshData2 = meshData[i];
				Mesh mesh = (this.Meshes[i] = new Mesh());
				meshData2.Apply(mesh);
			}
		}

		// Token: 0x04004959 RID: 18777
		public Vector3 Position;

		// Token: 0x0400495A RID: 18778
		public Mesh[] Meshes;
	}
}
