using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006E8 RID: 1768
public class PlaceMonumentsRoadside : ProceduralComponent
{
	// Token: 0x06003243 RID: 12867 RVA: 0x0013556C File Offset: 0x0013376C
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
		List<Prefab<MonumentInfo>> list = new List<Prefab<MonumentInfo>>();
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Prefab<MonumentInfo>[] array3 = Prefab.Load<MonumentInfo>(array2[i], null, null, true);
			array3.Shuffle(ref seed);
			list.AddRange(array3);
		}
		Prefab<MonumentInfo>[] array4 = list.ToArray();
		if (array4 == null || array4.Length == 0)
		{
			return;
		}
		array4.BubbleSort<Prefab<MonumentInfo>>();
		PlaceMonumentsRoadside.SpawnInfoGroup[] array5 = new PlaceMonumentsRoadside.SpawnInfoGroup[array4.Length];
		for (int j = 0; j < array4.Length; j++)
		{
			Prefab<MonumentInfo> prefab = array4[j];
			PlaceMonumentsRoadside.SpawnInfoGroup spawnInfoGroup = null;
			for (int k = 0; k < j; k++)
			{
				PlaceMonumentsRoadside.SpawnInfoGroup spawnInfoGroup2 = array5[k];
				Prefab<MonumentInfo> prefab2 = spawnInfoGroup2.prefab;
				if (prefab == prefab2)
				{
					spawnInfoGroup = spawnInfoGroup2;
					break;
				}
			}
			if (spawnInfoGroup == null)
			{
				spawnInfoGroup = new PlaceMonumentsRoadside.SpawnInfoGroup();
				spawnInfoGroup.prefab = array4[j];
				spawnInfoGroup.candidates = new List<PlaceMonumentsRoadside.SpawnInfo>();
			}
			array5[j] = spawnInfoGroup;
		}
		foreach (PlaceMonumentsRoadside.SpawnInfoGroup spawnInfoGroup3 in array5)
		{
			if (!spawnInfoGroup3.processed)
			{
				Prefab<MonumentInfo> prefab3 = spawnInfoGroup3.prefab;
				MonumentInfo component = prefab3.Component;
				if (!(component == null) && (ulong)World.Size >= (ulong)((long)component.MinWorldSize))
				{
					int num = 0;
					Vector3 vector = Vector3.zero;
					foreach (TerrainPathConnect terrainPathConnect in prefab3.Object.GetComponentsInChildren<TerrainPathConnect>(true))
					{
						if (terrainPathConnect.Type == InfrastructureType.Road)
						{
							vector += terrainPathConnect.transform.position;
							num++;
						}
					}
					Vector3 vector2 = -vector.XZ3D().normalized;
					Vector3 vector3 = PlaceMonumentsRoadside.rot90 * vector2;
					if (num > 1)
					{
						vector /= (float)num;
					}
					foreach (PathList pathList in TerrainMeta.Path.Roads)
					{
						bool flag = false;
						switch (this.RoadType)
						{
						case PlaceMonumentsRoadside.RoadMode.SideRoadOrRingRoad:
							flag = pathList.Hierarchy == 0 || pathList.Hierarchy == 1;
							break;
						case PlaceMonumentsRoadside.RoadMode.SideRoad:
							flag = pathList.Hierarchy == 1;
							break;
						case PlaceMonumentsRoadside.RoadMode.RingRoad:
							flag = pathList.Hierarchy == 0;
							break;
						case PlaceMonumentsRoadside.RoadMode.SideRoadOrDesireTrail:
							flag = pathList.Hierarchy == 1 || pathList.Hierarchy == 2;
							break;
						case PlaceMonumentsRoadside.RoadMode.DesireTrail:
							flag = pathList.Hierarchy == 2;
							break;
						}
						if (flag)
						{
							PathInterpolator path = pathList.Path;
							float num2 = 5f;
							float num3 = 5f;
							float num4 = path.StartOffset + num3;
							float num5 = path.Length - path.EndOffset - num3;
							for (float num6 = num4; num6 <= num5; num6 += num2)
							{
								Vector3 vector4 = (pathList.Spline ? path.GetPointCubicHermite(num6) : path.GetPoint(num6));
								Vector3 tangent = path.GetTangent(num6);
								for (int m = -1; m <= 1; m += 2)
								{
									Quaternion quaternion = Quaternion.LookRotation((float)m * tangent.XZ3D());
									Vector3 vector5 = vector4;
									Quaternion quaternion2 = quaternion;
									Vector3 localScale = prefab3.Object.transform.localScale;
									quaternion2 *= Quaternion.LookRotation(vector3);
									vector5 -= quaternion2 * vector;
									PlaceMonumentsRoadside.SpawnInfo spawnInfo = default(PlaceMonumentsRoadside.SpawnInfo);
									spawnInfo.prefab = prefab3;
									spawnInfo.position = vector5;
									spawnInfo.rotation = quaternion2;
									spawnInfo.scale = localScale;
									spawnInfoGroup3.candidates.Add(spawnInfo);
								}
							}
						}
					}
					spawnInfoGroup3.processed = true;
				}
			}
		}
		List<PlaceMonumentsRoadside.SpawnInfo> list2 = new List<PlaceMonumentsRoadside.SpawnInfo>();
		int num7 = 0;
		List<PlaceMonumentsRoadside.SpawnInfo> list3 = new List<PlaceMonumentsRoadside.SpawnInfo>();
		for (int n = 0; n < 8; n++)
		{
			int num8 = 0;
			list2.Clear();
			array5.Shuffle(ref seed);
			foreach (PlaceMonumentsRoadside.SpawnInfoGroup spawnInfoGroup4 in array5)
			{
				Prefab<MonumentInfo> prefab4 = spawnInfoGroup4.prefab;
				MonumentInfo component2 = prefab4.Component;
				if (!(component2 == null) && (ulong)World.Size >= (ulong)((long)component2.MinWorldSize))
				{
					DungeonGridInfo dungeonEntrance = component2.DungeonEntrance;
					int num9 = (int)(prefab4.Parameters ? (prefab4.Parameters.Priority + 1) : PrefabPriority.Low);
					int num10 = 100000 * num9 * num9 * num9 * num9;
					int num11 = 0;
					int num12 = 0;
					PlaceMonumentsRoadside.SpawnInfo spawnInfo2 = default(PlaceMonumentsRoadside.SpawnInfo);
					spawnInfoGroup4.candidates.Shuffle(ref seed);
					for (int num13 = 0; num13 < spawnInfoGroup4.candidates.Count; num13++)
					{
						PlaceMonumentsRoadside.SpawnInfo spawnInfo3 = spawnInfoGroup4.candidates[num13];
						PlaceMonumentsRoadside.DistanceInfo distanceInfo = this.GetDistanceInfo(list2, prefab4, spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale);
						if (distanceInfo.minDistanceSameType >= (float)this.MinDistanceSameType && distanceInfo.minDistanceDifferentType >= (float)this.MinDistanceDifferentType)
						{
							int num14 = num10;
							if (distanceInfo.minDistanceSameType != 3.4028235E+38f)
							{
								if (this.DistanceSameType == PlaceMonumentsRoadside.DistanceMode.Min)
								{
									num14 -= Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
								}
								else if (this.DistanceSameType == PlaceMonumentsRoadside.DistanceMode.Max)
								{
									num14 += Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
								}
							}
							if (distanceInfo.minDistanceDifferentType != 3.4028235E+38f)
							{
								if (this.DistanceDifferentType == PlaceMonumentsRoadside.DistanceMode.Min)
								{
									num14 -= Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
								}
								else if (this.DistanceDifferentType == PlaceMonumentsRoadside.DistanceMode.Max)
								{
									num14 += Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
								}
							}
							if (num14 > num12 && prefab4.ApplyTerrainAnchors(ref spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale, this.Filter) && component2.CheckPlacement(spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale))
							{
								if (dungeonEntrance)
								{
									Vector3 vector6 = spawnInfo3.position + spawnInfo3.rotation * Vector3.Scale(spawnInfo3.scale, dungeonEntrance.transform.position);
									Vector3 vector7 = dungeonEntrance.SnapPosition(vector6);
									spawnInfo3.position += vector7 - vector6;
									if (!dungeonEntrance.IsValidSpawnPosition(vector7))
									{
										goto IL_747;
									}
								}
								if (prefab4.ApplyTerrainChecks(spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale, this.Filter) && prefab4.ApplyTerrainFilters(spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale, null) && prefab4.ApplyWaterChecks(spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale) && !prefab4.CheckEnvironmentVolumes(spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
								{
									num12 = num14;
									spawnInfo2 = spawnInfo3;
									num11++;
									if (num11 >= 8 || this.DistanceDifferentType == PlaceMonumentsRoadside.DistanceMode.Any)
									{
										break;
									}
								}
							}
						}
						IL_747:;
					}
					if (num12 > 0)
					{
						list2.Add(spawnInfo2);
						num8 += num12;
					}
					if (this.TargetCount > 0 && list2.Count >= this.TargetCount)
					{
						break;
					}
				}
			}
			if (num8 > num7)
			{
				num7 = num8;
				GenericsUtil.Swap<List<PlaceMonumentsRoadside.SpawnInfo>>(ref list2, ref list3);
			}
		}
		foreach (PlaceMonumentsRoadside.SpawnInfo spawnInfo4 in list3)
		{
			World.AddPrefab("Monument", spawnInfo4.prefab, spawnInfo4.position, spawnInfo4.rotation, spawnInfo4.scale);
		}
	}

	// Token: 0x06003244 RID: 12868 RVA: 0x00135DC0 File Offset: 0x00133FC0
	private PlaceMonumentsRoadside.DistanceInfo GetDistanceInfo(List<PlaceMonumentsRoadside.SpawnInfo> spawns, Prefab<MonumentInfo> prefab, Vector3 monumentPos, Quaternion monumentRot, Vector3 monumentScale)
	{
		PlaceMonumentsRoadside.DistanceInfo distanceInfo = default(PlaceMonumentsRoadside.DistanceInfo);
		distanceInfo.minDistanceDifferentType = float.MaxValue;
		distanceInfo.maxDistanceDifferentType = float.MinValue;
		distanceInfo.minDistanceSameType = float.MaxValue;
		distanceInfo.maxDistanceSameType = float.MinValue;
		OBB obb = new OBB(monumentPos, monumentScale, monumentRot, prefab.Component.Bounds);
		if (TerrainMeta.Path != null)
		{
			foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
			{
				if (!prefab.Component.HasDungeonLink || (!monumentInfo.HasDungeonLink && monumentInfo.WantsDungeonLink))
				{
					float num = monumentInfo.SqrDistance(obb);
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
			if (distanceInfo.minDistanceDifferentType != 3.4028235E+38f)
			{
				distanceInfo.minDistanceDifferentType = Mathf.Sqrt(distanceInfo.minDistanceDifferentType);
			}
			if (distanceInfo.maxDistanceDifferentType != -3.4028235E+38f)
			{
				distanceInfo.maxDistanceDifferentType = Mathf.Sqrt(distanceInfo.maxDistanceDifferentType);
			}
		}
		if (spawns != null)
		{
			foreach (PlaceMonumentsRoadside.SpawnInfo spawnInfo in spawns)
			{
				float num2 = new OBB(spawnInfo.position, spawnInfo.scale, spawnInfo.rotation, spawnInfo.prefab.Component.Bounds).SqrDistance(obb);
				if (num2 < distanceInfo.minDistanceSameType)
				{
					distanceInfo.minDistanceSameType = num2;
				}
				if (num2 > distanceInfo.maxDistanceSameType)
				{
					distanceInfo.maxDistanceSameType = num2;
				}
			}
			if (prefab.Component.HasDungeonLink)
			{
				foreach (MonumentInfo monumentInfo2 in TerrainMeta.Path.Monuments)
				{
					if (monumentInfo2.HasDungeonLink || !monumentInfo2.WantsDungeonLink)
					{
						float num3 = monumentInfo2.SqrDistance(obb);
						if (num3 < distanceInfo.minDistanceSameType)
						{
							distanceInfo.minDistanceSameType = num3;
						}
						if (num3 > distanceInfo.maxDistanceSameType)
						{
							distanceInfo.maxDistanceSameType = num3;
						}
					}
				}
				foreach (DungeonGridInfo dungeonGridInfo in TerrainMeta.Path.DungeonGridEntrances)
				{
					float num4 = dungeonGridInfo.SqrDistance(monumentPos);
					if (num4 < distanceInfo.minDistanceSameType)
					{
						distanceInfo.minDistanceSameType = num4;
					}
					if (num4 > distanceInfo.maxDistanceSameType)
					{
						distanceInfo.maxDistanceSameType = num4;
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
		}
		return distanceInfo;
	}

	// Token: 0x04002928 RID: 10536
	public SpawnFilter Filter;

	// Token: 0x04002929 RID: 10537
	public string ResourceFolder = string.Empty;

	// Token: 0x0400292A RID: 10538
	public int TargetCount;

	// Token: 0x0400292B RID: 10539
	[FormerlySerializedAs("MinDistance")]
	public int MinDistanceSameType = 500;

	// Token: 0x0400292C RID: 10540
	public int MinDistanceDifferentType;

	// Token: 0x0400292D RID: 10541
	[FormerlySerializedAs("MinSize")]
	public int MinWorldSize;

	// Token: 0x0400292E RID: 10542
	[Tooltip("Distance to monuments of the same type")]
	public PlaceMonumentsRoadside.DistanceMode DistanceSameType = PlaceMonumentsRoadside.DistanceMode.Max;

	// Token: 0x0400292F RID: 10543
	[Tooltip("Distance to monuments of a different type")]
	public PlaceMonumentsRoadside.DistanceMode DistanceDifferentType;

	// Token: 0x04002930 RID: 10544
	public PlaceMonumentsRoadside.RoadMode RoadType;

	// Token: 0x04002931 RID: 10545
	private const int GroupCandidates = 8;

	// Token: 0x04002932 RID: 10546
	private const int IndividualCandidates = 8;

	// Token: 0x04002933 RID: 10547
	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x02000E37 RID: 3639
	private struct SpawnInfo
	{
		// Token: 0x04004AE3 RID: 19171
		public Prefab<MonumentInfo> prefab;

		// Token: 0x04004AE4 RID: 19172
		public Vector3 position;

		// Token: 0x04004AE5 RID: 19173
		public Quaternion rotation;

		// Token: 0x04004AE6 RID: 19174
		public Vector3 scale;
	}

	// Token: 0x02000E38 RID: 3640
	private class SpawnInfoGroup
	{
		// Token: 0x04004AE7 RID: 19175
		public bool processed;

		// Token: 0x04004AE8 RID: 19176
		public Prefab<MonumentInfo> prefab;

		// Token: 0x04004AE9 RID: 19177
		public List<PlaceMonumentsRoadside.SpawnInfo> candidates;
	}

	// Token: 0x02000E39 RID: 3641
	private struct DistanceInfo
	{
		// Token: 0x04004AEA RID: 19178
		public float minDistanceSameType;

		// Token: 0x04004AEB RID: 19179
		public float maxDistanceSameType;

		// Token: 0x04004AEC RID: 19180
		public float minDistanceDifferentType;

		// Token: 0x04004AED RID: 19181
		public float maxDistanceDifferentType;
	}

	// Token: 0x02000E3A RID: 3642
	public enum DistanceMode
	{
		// Token: 0x04004AEF RID: 19183
		Any,
		// Token: 0x04004AF0 RID: 19184
		Min,
		// Token: 0x04004AF1 RID: 19185
		Max
	}

	// Token: 0x02000E3B RID: 3643
	public enum RoadMode
	{
		// Token: 0x04004AF3 RID: 19187
		SideRoadOrRingRoad,
		// Token: 0x04004AF4 RID: 19188
		SideRoad,
		// Token: 0x04004AF5 RID: 19189
		RingRoad,
		// Token: 0x04004AF6 RID: 19190
		SideRoadOrDesireTrail,
		// Token: 0x04004AF7 RID: 19191
		DesireTrail
	}
}
