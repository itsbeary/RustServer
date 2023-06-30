using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006E7 RID: 1767
public class PlaceMonumentsRailside : ProceduralComponent
{
	// Token: 0x0600323F RID: 12863 RVA: 0x001349C4 File Offset: 0x00132BC4
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
		PlaceMonumentsRailside.SpawnInfoGroup[] array5 = new PlaceMonumentsRailside.SpawnInfoGroup[array4.Length];
		for (int j = 0; j < array4.Length; j++)
		{
			Prefab<MonumentInfo> prefab = array4[j];
			PlaceMonumentsRailside.SpawnInfoGroup spawnInfoGroup = null;
			for (int k = 0; k < j; k++)
			{
				PlaceMonumentsRailside.SpawnInfoGroup spawnInfoGroup2 = array5[k];
				Prefab<MonumentInfo> prefab2 = spawnInfoGroup2.prefab;
				if (prefab == prefab2)
				{
					spawnInfoGroup = spawnInfoGroup2;
					break;
				}
			}
			if (spawnInfoGroup == null)
			{
				spawnInfoGroup = new PlaceMonumentsRailside.SpawnInfoGroup();
				spawnInfoGroup.prefab = array4[j];
				spawnInfoGroup.candidates = new List<PlaceMonumentsRailside.SpawnInfo>();
			}
			array5[j] = spawnInfoGroup;
		}
		foreach (PlaceMonumentsRailside.SpawnInfoGroup spawnInfoGroup3 in array5)
		{
			if (!spawnInfoGroup3.processed)
			{
				Prefab<MonumentInfo> prefab3 = spawnInfoGroup3.prefab;
				MonumentInfo component = prefab3.Component;
				if (!(component == null) && (ulong)World.Size >= (ulong)((long)component.MinWorldSize))
				{
					int num = 0;
					Vector3 vector = Vector3.zero;
					Vector3 vector2 = Vector3.zero;
					Vector3 vector3 = Vector3.zero;
					foreach (TerrainPathConnect terrainPathConnect in prefab3.Object.GetComponentsInChildren<TerrainPathConnect>(true))
					{
						if (terrainPathConnect.Type == InfrastructureType.Rail)
						{
							if (num == 0)
							{
								vector2 = terrainPathConnect.transform.position;
							}
							else if (num == 1)
							{
								vector3 = terrainPathConnect.transform.position;
							}
							vector += terrainPathConnect.transform.position;
							num++;
						}
					}
					Vector3 normalized = (vector3 - vector2).normalized;
					Vector3 vector4 = PlaceMonumentsRailside.rot90 * normalized;
					if (num > 1)
					{
						vector /= (float)num;
					}
					if (this.PositionOffset > 0)
					{
						vector += vector4 * (float)this.PositionOffset;
					}
					foreach (PathList pathList in TerrainMeta.Path.Rails)
					{
						PathInterpolator path = pathList.Path;
						float num2 = (float)(this.TangentInterval / 2);
						float num3 = 5f;
						float num4 = 5f;
						float num5 = path.StartOffset + num4;
						float num6 = path.Length - path.EndOffset - num4;
						for (float num7 = num5; num7 <= num6; num7 += num3)
						{
							Vector3 vector5 = (pathList.Spline ? path.GetPointCubicHermite(num7) : path.GetPoint(num7));
							Vector3 normalized2 = (path.GetPoint(num7 + num2) - path.GetPoint(num7 - num2)).normalized;
							for (int m = -1; m <= 1; m += 2)
							{
								Quaternion quaternion = Quaternion.LookRotation((float)m * normalized2.XZ3D());
								Vector3 vector6 = vector5;
								Quaternion quaternion2 = quaternion;
								Vector3 localScale = prefab3.Object.transform.localScale;
								quaternion2 *= Quaternion.LookRotation(normalized);
								vector6 -= quaternion2 * vector;
								PlaceMonumentsRailside.SpawnInfo spawnInfo = default(PlaceMonumentsRailside.SpawnInfo);
								spawnInfo.prefab = prefab3;
								spawnInfo.position = vector6;
								spawnInfo.rotation = quaternion2;
								spawnInfo.scale = localScale;
								spawnInfoGroup3.candidates.Add(spawnInfo);
							}
						}
					}
					spawnInfoGroup3.processed = true;
				}
			}
		}
		List<PlaceMonumentsRailside.SpawnInfo> list2 = new List<PlaceMonumentsRailside.SpawnInfo>();
		int num8 = 0;
		List<PlaceMonumentsRailside.SpawnInfo> list3 = new List<PlaceMonumentsRailside.SpawnInfo>();
		for (int n = 0; n < 8; n++)
		{
			int num9 = 0;
			list2.Clear();
			array5.Shuffle(ref seed);
			foreach (PlaceMonumentsRailside.SpawnInfoGroup spawnInfoGroup4 in array5)
			{
				Prefab<MonumentInfo> prefab4 = spawnInfoGroup4.prefab;
				MonumentInfo component2 = prefab4.Component;
				if (!(component2 == null) && (ulong)World.Size >= (ulong)((long)component2.MinWorldSize))
				{
					DungeonGridInfo dungeonEntrance = component2.DungeonEntrance;
					int num10 = (int)(prefab4.Parameters ? (prefab4.Parameters.Priority + 1) : PrefabPriority.Low);
					int num11 = 100000 * num10 * num10 * num10 * num10;
					int num12 = 0;
					int num13 = 0;
					PlaceMonumentsRailside.SpawnInfo spawnInfo2 = default(PlaceMonumentsRailside.SpawnInfo);
					spawnInfoGroup4.candidates.Shuffle(ref seed);
					for (int num14 = 0; num14 < spawnInfoGroup4.candidates.Count; num14++)
					{
						PlaceMonumentsRailside.SpawnInfo spawnInfo3 = spawnInfoGroup4.candidates[num14];
						PlaceMonumentsRailside.DistanceInfo distanceInfo = this.GetDistanceInfo(list2, prefab4, spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale);
						if (distanceInfo.minDistanceSameType >= (float)this.MinDistanceSameType && distanceInfo.minDistanceDifferentType >= (float)this.MinDistanceDifferentType)
						{
							int num15 = num11;
							if (distanceInfo.minDistanceSameType != 3.4028235E+38f)
							{
								if (this.DistanceSameType == PlaceMonumentsRailside.DistanceMode.Min)
								{
									num15 -= Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
								}
								else if (this.DistanceSameType == PlaceMonumentsRailside.DistanceMode.Max)
								{
									num15 += Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
								}
							}
							if (distanceInfo.minDistanceDifferentType != 3.4028235E+38f)
							{
								if (this.DistanceDifferentType == PlaceMonumentsRailside.DistanceMode.Min)
								{
									num15 -= Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
								}
								else if (this.DistanceDifferentType == PlaceMonumentsRailside.DistanceMode.Max)
								{
									num15 += Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
								}
							}
							if (num15 > num13 && prefab4.ApplyTerrainAnchors(ref spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale, this.Filter) && component2.CheckPlacement(spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale))
							{
								if (dungeonEntrance)
								{
									Vector3 vector7 = spawnInfo3.position + spawnInfo3.rotation * Vector3.Scale(spawnInfo3.scale, dungeonEntrance.transform.position);
									Vector3 vector8 = dungeonEntrance.SnapPosition(vector7);
									spawnInfo3.position += vector8 - vector7;
									if (!dungeonEntrance.IsValidSpawnPosition(vector8))
									{
										goto IL_736;
									}
								}
								if (prefab4.ApplyTerrainChecks(spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale, this.Filter) && prefab4.ApplyTerrainFilters(spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale, null) && prefab4.ApplyWaterChecks(spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale) && !prefab4.CheckEnvironmentVolumes(spawnInfo3.position, spawnInfo3.rotation, spawnInfo3.scale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
								{
									num13 = num15;
									spawnInfo2 = spawnInfo3;
									num12++;
									if (num12 >= 8 || this.DistanceDifferentType == PlaceMonumentsRailside.DistanceMode.Any)
									{
										break;
									}
								}
							}
						}
						IL_736:;
					}
					if (num13 > 0)
					{
						list2.Add(spawnInfo2);
						num9 += num13;
					}
					if (this.TargetCount > 0 && list2.Count >= this.TargetCount)
					{
						break;
					}
				}
			}
			if (num9 > num8)
			{
				num8 = num9;
				GenericsUtil.Swap<List<PlaceMonumentsRailside.SpawnInfo>>(ref list2, ref list3);
			}
		}
		foreach (PlaceMonumentsRailside.SpawnInfo spawnInfo4 in list3)
		{
			World.AddPrefab("Monument", spawnInfo4.prefab, spawnInfo4.position, spawnInfo4.rotation, spawnInfo4.scale);
		}
	}

	// Token: 0x06003240 RID: 12864 RVA: 0x00135208 File Offset: 0x00133408
	private PlaceMonumentsRailside.DistanceInfo GetDistanceInfo(List<PlaceMonumentsRailside.SpawnInfo> spawns, Prefab<MonumentInfo> prefab, Vector3 monumentPos, Quaternion monumentRot, Vector3 monumentScale)
	{
		PlaceMonumentsRailside.DistanceInfo distanceInfo = default(PlaceMonumentsRailside.DistanceInfo);
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
			foreach (PlaceMonumentsRailside.SpawnInfo spawnInfo in spawns)
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

	// Token: 0x0400291B RID: 10523
	public SpawnFilter Filter;

	// Token: 0x0400291C RID: 10524
	public string ResourceFolder = string.Empty;

	// Token: 0x0400291D RID: 10525
	public int TargetCount;

	// Token: 0x0400291E RID: 10526
	public int PositionOffset = 100;

	// Token: 0x0400291F RID: 10527
	public int TangentInterval = 100;

	// Token: 0x04002920 RID: 10528
	[FormerlySerializedAs("MinDistance")]
	public int MinDistanceSameType = 500;

	// Token: 0x04002921 RID: 10529
	public int MinDistanceDifferentType;

	// Token: 0x04002922 RID: 10530
	[FormerlySerializedAs("MinSize")]
	public int MinWorldSize;

	// Token: 0x04002923 RID: 10531
	[Tooltip("Distance to monuments of the same type")]
	public PlaceMonumentsRailside.DistanceMode DistanceSameType = PlaceMonumentsRailside.DistanceMode.Max;

	// Token: 0x04002924 RID: 10532
	[Tooltip("Distance to monuments of a different type")]
	public PlaceMonumentsRailside.DistanceMode DistanceDifferentType;

	// Token: 0x04002925 RID: 10533
	private const int GroupCandidates = 8;

	// Token: 0x04002926 RID: 10534
	private const int IndividualCandidates = 8;

	// Token: 0x04002927 RID: 10535
	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x02000E32 RID: 3634
	private struct SpawnInfo
	{
		// Token: 0x04004AD2 RID: 19154
		public Prefab<MonumentInfo> prefab;

		// Token: 0x04004AD3 RID: 19155
		public Vector3 position;

		// Token: 0x04004AD4 RID: 19156
		public Quaternion rotation;

		// Token: 0x04004AD5 RID: 19157
		public Vector3 scale;
	}

	// Token: 0x02000E33 RID: 3635
	private class SpawnInfoGroup
	{
		// Token: 0x04004AD6 RID: 19158
		public bool processed;

		// Token: 0x04004AD7 RID: 19159
		public Prefab<MonumentInfo> prefab;

		// Token: 0x04004AD8 RID: 19160
		public List<PlaceMonumentsRailside.SpawnInfo> candidates;
	}

	// Token: 0x02000E34 RID: 3636
	private struct DistanceInfo
	{
		// Token: 0x04004AD9 RID: 19161
		public float minDistanceSameType;

		// Token: 0x04004ADA RID: 19162
		public float maxDistanceSameType;

		// Token: 0x04004ADB RID: 19163
		public float minDistanceDifferentType;

		// Token: 0x04004ADC RID: 19164
		public float maxDistanceDifferentType;
	}

	// Token: 0x02000E35 RID: 3637
	public enum DistanceMode
	{
		// Token: 0x04004ADE RID: 19166
		Any,
		// Token: 0x04004ADF RID: 19167
		Min,
		// Token: 0x04004AE0 RID: 19168
		Max
	}
}
