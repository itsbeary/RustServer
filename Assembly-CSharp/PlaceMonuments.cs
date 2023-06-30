using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006E5 RID: 1765
public class PlaceMonuments : ProceduralComponent
{
	// Token: 0x06003239 RID: 12857 RVA: 0x00133AB0 File Offset: 0x00131CB0
	public override void Process(uint seed)
	{
		string[] array = (from folder in this.ResourceFolder.Split(new char[] { ',' })
			select "assets/bundled/prefabs/autospawn/" + folder + "/").ToArray<string>();
		if (World.Networked)
		{
			World.Spawn("Monument", array);
			return;
		}
		if ((ulong)World.Size < (ulong)((long)this.MinWorldSize))
		{
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		PathFinder pathFinder = null;
		List<PathFinder.Point> list = null;
		List<Prefab<MonumentInfo>> list2 = new List<Prefab<MonumentInfo>>();
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Prefab<MonumentInfo>[] array3 = Prefab.Load<MonumentInfo>(array2[i], null, null, true);
			array3.Shuffle(ref seed);
			list2.AddRange(array3);
		}
		Prefab<MonumentInfo>[] array4 = list2.ToArray();
		if (array4 == null || array4.Length == 0)
		{
			return;
		}
		array4.BubbleSort<Prefab<MonumentInfo>>();
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float num = position.x + size.x;
		float num2 = position.z + size.z;
		List<PlaceMonuments.SpawnInfo> list3 = new List<PlaceMonuments.SpawnInfo>();
		int num3 = 0;
		List<PlaceMonuments.SpawnInfo> list4 = new List<PlaceMonuments.SpawnInfo>();
		for (int j = 0; j < 8; j++)
		{
			int num4 = 0;
			list3.Clear();
			foreach (Prefab<MonumentInfo> prefab in array4)
			{
				MonumentInfo component = prefab.Component;
				if (!(component == null) && (ulong)World.Size >= (ulong)((long)component.MinWorldSize))
				{
					DungeonGridInfo dungeonEntrance = component.DungeonEntrance;
					int num5 = (int)(prefab.Parameters ? (prefab.Parameters.Priority + 1) : PrefabPriority.Low);
					int num6 = 100000 * num5 * num5 * num5 * num5;
					int num7 = 0;
					int num8 = 0;
					PlaceMonuments.SpawnInfo spawnInfo = default(PlaceMonuments.SpawnInfo);
					for (int k = 0; k < 10000; k++)
					{
						float num9 = SeedRandom.Range(ref seed, x, num);
						float num10 = SeedRandom.Range(ref seed, z, num2);
						float num11 = TerrainMeta.NormalizeX(num9);
						float num12 = TerrainMeta.NormalizeZ(num10);
						float num13 = SeedRandom.Value(ref seed);
						float factor = this.Filter.GetFactor(num11, num12, true);
						if (factor * factor >= num13)
						{
							float height = heightMap.GetHeight(num11, num12);
							Vector3 vector = new Vector3(num9, height, num10);
							Quaternion localRotation = prefab.Object.transform.localRotation;
							Vector3 localScale = prefab.Object.transform.localScale;
							Vector3 vector2 = vector;
							prefab.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
							PlaceMonuments.DistanceInfo distanceInfo = this.GetDistanceInfo(list3, prefab, vector, localRotation, localScale, vector2);
							if (distanceInfo.minDistanceSameType >= (float)this.MinDistanceSameType && distanceInfo.minDistanceDifferentType >= (float)this.MinDistanceDifferentType && (!dungeonEntrance || distanceInfo.minDistanceDungeonEntrance >= dungeonEntrance.MinDistance))
							{
								int num14 = num6;
								if (distanceInfo.minDistanceSameType != 3.4028235E+38f)
								{
									if (this.DistanceSameType == PlaceMonuments.DistanceMode.Min)
									{
										num14 -= Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
									}
									else if (this.DistanceSameType == PlaceMonuments.DistanceMode.Max)
									{
										num14 += Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
									}
								}
								if (distanceInfo.minDistanceDifferentType != 3.4028235E+38f)
								{
									if (this.DistanceDifferentType == PlaceMonuments.DistanceMode.Min)
									{
										num14 -= Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
									}
									else if (this.DistanceDifferentType == PlaceMonuments.DistanceMode.Max)
									{
										num14 += Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
									}
								}
								if (num14 > num8 && prefab.ApplyTerrainAnchors(ref vector, localRotation, localScale, this.Filter) && component.CheckPlacement(vector, localRotation, localScale))
								{
									if (dungeonEntrance)
									{
										Vector3 vector3 = vector + localRotation * Vector3.Scale(localScale, dungeonEntrance.transform.position);
										Vector3 vector4 = dungeonEntrance.SnapPosition(vector3);
										vector += vector4 - vector3;
										if (!dungeonEntrance.IsValidSpawnPosition(vector4))
										{
											goto IL_5E2;
										}
										vector2 = vector4;
									}
									if (prefab.ApplyTerrainChecks(vector, localRotation, localScale, this.Filter) && prefab.ApplyTerrainFilters(vector, localRotation, localScale, null) && prefab.ApplyWaterChecks(vector, localRotation, localScale) && !prefab.CheckEnvironmentVolumes(vector, localRotation, localScale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
									{
										bool flag = false;
										foreach (TerrainPathConnect terrainPathConnect in prefab.Object.GetComponentsInChildren<TerrainPathConnect>(true))
										{
											if (terrainPathConnect.Type == InfrastructureType.Boat)
											{
												if (pathFinder == null)
												{
													int[,] array6 = TerrainPath.CreateBoatCostmap(2f);
													int length = array6.GetLength(0);
													pathFinder = new PathFinder(array6, true, true);
													list = new List<PathFinder.Point>
													{
														new PathFinder.Point(0, 0),
														new PathFinder.Point(0, length / 2),
														new PathFinder.Point(0, length - 1),
														new PathFinder.Point(length / 2, 0),
														new PathFinder.Point(length / 2, length - 1),
														new PathFinder.Point(length - 1, 0),
														new PathFinder.Point(length - 1, length / 2),
														new PathFinder.Point(length - 1, length - 1)
													};
												}
												PathFinder.Point pathFinderPoint = terrainPathConnect.GetPathFinderPoint(pathFinder.GetResolution(0), vector + localRotation * Vector3.Scale(localScale, terrainPathConnect.transform.localPosition));
												if (pathFinder.FindPathUndirected(new List<PathFinder.Point> { pathFinderPoint }, list, 100000) == null)
												{
													flag = true;
													break;
												}
											}
										}
										if (!flag)
										{
											PlaceMonuments.SpawnInfo spawnInfo2 = default(PlaceMonuments.SpawnInfo);
											spawnInfo2.prefab = prefab;
											spawnInfo2.position = vector;
											spawnInfo2.rotation = localRotation;
											spawnInfo2.scale = localScale;
											if (dungeonEntrance)
											{
												spawnInfo2.dungeonEntrance = true;
												spawnInfo2.dungeonEntrancePos = vector2;
											}
											num8 = num14;
											spawnInfo = spawnInfo2;
											num7++;
											if (num7 >= 8 || this.DistanceDifferentType == PlaceMonuments.DistanceMode.Any)
											{
												break;
											}
										}
									}
								}
							}
						}
						IL_5E2:;
					}
					if (num8 > 0)
					{
						list3.Add(spawnInfo);
						num4 += num8;
					}
					if (this.TargetCount > 0 && list3.Count >= this.TargetCount)
					{
						break;
					}
				}
			}
			if (num4 > num3)
			{
				num3 = num4;
				GenericsUtil.Swap<List<PlaceMonuments.SpawnInfo>>(ref list3, ref list4);
			}
		}
		foreach (PlaceMonuments.SpawnInfo spawnInfo3 in list4)
		{
			World.AddPrefab("Monument", spawnInfo3.prefab, spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale);
		}
	}

	// Token: 0x0600323A RID: 12858 RVA: 0x00134174 File Offset: 0x00132374
	private PlaceMonuments.DistanceInfo GetDistanceInfo(List<PlaceMonuments.SpawnInfo> spawns, Prefab<MonumentInfo> prefab, Vector3 monumentPos, Quaternion monumentRot, Vector3 monumentScale, Vector3 dungeonPos)
	{
		PlaceMonuments.DistanceInfo distanceInfo = default(PlaceMonuments.DistanceInfo);
		distanceInfo.minDistanceSameType = float.MaxValue;
		distanceInfo.maxDistanceSameType = float.MinValue;
		distanceInfo.minDistanceDifferentType = float.MaxValue;
		distanceInfo.maxDistanceDifferentType = float.MinValue;
		distanceInfo.minDistanceDungeonEntrance = float.MaxValue;
		distanceInfo.maxDistanceDungeonEntrance = float.MinValue;
		OBB obb = new OBB(monumentPos, monumentScale, monumentRot, prefab.Component.Bounds);
		if (spawns != null)
		{
			foreach (PlaceMonuments.SpawnInfo spawnInfo in spawns)
			{
				float num = new OBB(spawnInfo.position, spawnInfo.scale, spawnInfo.rotation, spawnInfo.prefab.Component.Bounds).SqrDistance(obb);
				if (spawnInfo.prefab.Folder == prefab.Folder)
				{
					if (num < distanceInfo.minDistanceSameType)
					{
						distanceInfo.minDistanceSameType = num;
					}
					if (num > distanceInfo.maxDistanceSameType)
					{
						distanceInfo.maxDistanceSameType = num;
					}
				}
				else
				{
					if (num < distanceInfo.minDistanceDifferentType)
					{
						distanceInfo.minDistanceDifferentType = num;
					}
					if (num > distanceInfo.maxDistanceDifferentType)
					{
						distanceInfo.maxDistanceDifferentType = num;
					}
				}
			}
			foreach (PlaceMonuments.SpawnInfo spawnInfo2 in spawns)
			{
				if (spawnInfo2.dungeonEntrance)
				{
					float sqrMagnitude = (spawnInfo2.dungeonEntrancePos - dungeonPos).sqrMagnitude;
					if (sqrMagnitude < distanceInfo.minDistanceDungeonEntrance)
					{
						distanceInfo.minDistanceDungeonEntrance = sqrMagnitude;
					}
					if (sqrMagnitude > distanceInfo.maxDistanceDungeonEntrance)
					{
						distanceInfo.maxDistanceDungeonEntrance = sqrMagnitude;
					}
				}
			}
		}
		if (TerrainMeta.Path != null)
		{
			foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
			{
				float num2 = monumentInfo.SqrDistance(obb);
				if (num2 < distanceInfo.minDistanceDifferentType)
				{
					distanceInfo.minDistanceDifferentType = num2;
				}
				if (num2 > distanceInfo.maxDistanceDifferentType)
				{
					distanceInfo.maxDistanceDifferentType = num2;
				}
			}
			foreach (DungeonGridInfo dungeonGridInfo in TerrainMeta.Path.DungeonGridEntrances)
			{
				float num3 = dungeonGridInfo.SqrDistance(dungeonPos);
				if (num3 < distanceInfo.minDistanceDungeonEntrance)
				{
					distanceInfo.minDistanceDungeonEntrance = num3;
				}
				if (num3 > distanceInfo.maxDistanceDungeonEntrance)
				{
					distanceInfo.maxDistanceDungeonEntrance = num3;
				}
			}
		}
		if (distanceInfo.minDistanceSameType != 3.4028235E+38f)
		{
			distanceInfo.minDistanceSameType = Mathf.Sqrt(distanceInfo.minDistanceSameType);
		}
		if (distanceInfo.maxDistanceSameType != -3.4028235E+38f)
		{
			distanceInfo.maxDistanceSameType = Mathf.Sqrt(distanceInfo.maxDistanceSameType);
		}
		if (distanceInfo.minDistanceDifferentType != 3.4028235E+38f)
		{
			distanceInfo.minDistanceDifferentType = Mathf.Sqrt(distanceInfo.minDistanceDifferentType);
		}
		if (distanceInfo.maxDistanceDifferentType != -3.4028235E+38f)
		{
			distanceInfo.maxDistanceDifferentType = Mathf.Sqrt(distanceInfo.maxDistanceDifferentType);
		}
		if (distanceInfo.minDistanceDungeonEntrance != 3.4028235E+38f)
		{
			distanceInfo.minDistanceDungeonEntrance = Mathf.Sqrt(distanceInfo.minDistanceDungeonEntrance);
		}
		if (distanceInfo.maxDistanceDungeonEntrance != -3.4028235E+38f)
		{
			distanceInfo.maxDistanceDungeonEntrance = Mathf.Sqrt(distanceInfo.maxDistanceDungeonEntrance);
		}
		return distanceInfo;
	}

	// Token: 0x04002907 RID: 10503
	public SpawnFilter Filter;

	// Token: 0x04002908 RID: 10504
	public string ResourceFolder = string.Empty;

	// Token: 0x04002909 RID: 10505
	public int TargetCount;

	// Token: 0x0400290A RID: 10506
	[FormerlySerializedAs("MinDistance")]
	public int MinDistanceSameType = 500;

	// Token: 0x0400290B RID: 10507
	public int MinDistanceDifferentType;

	// Token: 0x0400290C RID: 10508
	[FormerlySerializedAs("MinSize")]
	public int MinWorldSize;

	// Token: 0x0400290D RID: 10509
	[Tooltip("Distance to monuments of the same type")]
	public PlaceMonuments.DistanceMode DistanceSameType = PlaceMonuments.DistanceMode.Max;

	// Token: 0x0400290E RID: 10510
	[Tooltip("Distance to monuments of a different type")]
	public PlaceMonuments.DistanceMode DistanceDifferentType;

	// Token: 0x0400290F RID: 10511
	private const int GroupCandidates = 8;

	// Token: 0x04002910 RID: 10512
	private const int IndividualCandidates = 8;

	// Token: 0x04002911 RID: 10513
	private const int Attempts = 10000;

	// Token: 0x04002912 RID: 10514
	private const int MaxDepth = 100000;

	// Token: 0x02000E2C RID: 3628
	private struct SpawnInfo
	{
		// Token: 0x04004ABA RID: 19130
		public Prefab<MonumentInfo> prefab;

		// Token: 0x04004ABB RID: 19131
		public Vector3 position;

		// Token: 0x04004ABC RID: 19132
		public Quaternion rotation;

		// Token: 0x04004ABD RID: 19133
		public Vector3 scale;

		// Token: 0x04004ABE RID: 19134
		public bool dungeonEntrance;

		// Token: 0x04004ABF RID: 19135
		public Vector3 dungeonEntrancePos;
	}

	// Token: 0x02000E2D RID: 3629
	private struct DistanceInfo
	{
		// Token: 0x04004AC0 RID: 19136
		public float minDistanceSameType;

		// Token: 0x04004AC1 RID: 19137
		public float maxDistanceSameType;

		// Token: 0x04004AC2 RID: 19138
		public float minDistanceDifferentType;

		// Token: 0x04004AC3 RID: 19139
		public float maxDistanceDifferentType;

		// Token: 0x04004AC4 RID: 19140
		public float minDistanceDungeonEntrance;

		// Token: 0x04004AC5 RID: 19141
		public float maxDistanceDungeonEntrance;
	}

	// Token: 0x02000E2E RID: 3630
	public enum DistanceMode
	{
		// Token: 0x04004AC7 RID: 19143
		Any,
		// Token: 0x04004AC8 RID: 19144
		Min,
		// Token: 0x04004AC9 RID: 19145
		Max
	}
}
