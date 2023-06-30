using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006E6 RID: 1766
public class PlaceMonumentsOffshore : ProceduralComponent
{
	// Token: 0x0600323C RID: 12860 RVA: 0x00134514 File Offset: 0x00132714
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
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float num = position.x - (float)this.MaxDistanceFromTerrain;
		float num2 = position.x - (float)this.MinDistanceFromTerrain;
		float num3 = position.x + size.x + (float)this.MinDistanceFromTerrain;
		float num4 = position.x + size.x + (float)this.MaxDistanceFromTerrain;
		float num5 = position.z - (float)this.MaxDistanceFromTerrain;
		int minDistanceFromTerrain = this.MinDistanceFromTerrain;
		float num6 = position.z + size.z + (float)this.MinDistanceFromTerrain;
		float num7 = position.z + size.z + (float)this.MaxDistanceFromTerrain;
		List<PlaceMonumentsOffshore.SpawnInfo> list2 = new List<PlaceMonumentsOffshore.SpawnInfo>();
		int num8 = 0;
		List<PlaceMonumentsOffshore.SpawnInfo> list3 = new List<PlaceMonumentsOffshore.SpawnInfo>();
		for (int j = 0; j < 10; j++)
		{
			int num9 = 0;
			list2.Clear();
			foreach (Prefab<MonumentInfo> prefab in array4)
			{
				int num10 = (int)(prefab.Parameters ? (prefab.Parameters.Priority + 1) : PrefabPriority.Low);
				int num11 = num10 * num10 * num10 * num10;
				for (int k = 0; k < 10000; k++)
				{
					float num12 = 0f;
					float num13 = 0f;
					switch (seed % 4U)
					{
					case 0U:
						num12 = SeedRandom.Range(ref seed, num, num2);
						num13 = SeedRandom.Range(ref seed, num5, num7);
						break;
					case 1U:
						num12 = SeedRandom.Range(ref seed, num3, num4);
						num13 = SeedRandom.Range(ref seed, num5, num7);
						break;
					case 2U:
						num12 = SeedRandom.Range(ref seed, num, num4);
						num13 = SeedRandom.Range(ref seed, num5, num5);
						break;
					case 3U:
						num12 = SeedRandom.Range(ref seed, num, num4);
						num13 = SeedRandom.Range(ref seed, num6, num7);
						break;
					}
					float num14 = TerrainMeta.NormalizeX(num12);
					float num15 = TerrainMeta.NormalizeZ(num13);
					float height = heightMap.GetHeight(num14, num15);
					Vector3 vector = new Vector3(num12, height, num13);
					Quaternion localRotation = prefab.Object.transform.localRotation;
					Vector3 localScale = prefab.Object.transform.localScale;
					if (!this.CheckRadius(list2, vector, (float)this.DistanceBetweenMonuments))
					{
						prefab.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
						if ((!prefab.Component || prefab.Component.CheckPlacement(vector, localRotation, localScale)) && !prefab.CheckEnvironmentVolumes(vector, localRotation, localScale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
						{
							PlaceMonumentsOffshore.SpawnInfo spawnInfo = new PlaceMonumentsOffshore.SpawnInfo
							{
								prefab = prefab,
								position = vector,
								rotation = localRotation,
								scale = localScale
							};
							list2.Add(spawnInfo);
							num9 += num11;
							break;
						}
					}
				}
				if (this.TargetCount > 0 && list2.Count >= this.TargetCount)
				{
					break;
				}
			}
			if (num9 > num8)
			{
				num8 = num9;
				GenericsUtil.Swap<List<PlaceMonumentsOffshore.SpawnInfo>>(ref list2, ref list3);
			}
		}
		foreach (PlaceMonumentsOffshore.SpawnInfo spawnInfo2 in list3)
		{
			World.AddPrefab("Monument", spawnInfo2.prefab, spawnInfo2.position, spawnInfo2.rotation, spawnInfo2.scale);
		}
	}

	// Token: 0x0600323D RID: 12861 RVA: 0x00134928 File Offset: 0x00132B28
	private bool CheckRadius(List<PlaceMonumentsOffshore.SpawnInfo> spawns, Vector3 pos, float radius)
	{
		float num = radius * radius;
		using (List<PlaceMonumentsOffshore.SpawnInfo>.Enumerator enumerator = spawns.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if ((enumerator.Current.position - pos).sqrMagnitude < num)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04002913 RID: 10515
	public string ResourceFolder = string.Empty;

	// Token: 0x04002914 RID: 10516
	public int TargetCount;

	// Token: 0x04002915 RID: 10517
	public int MinDistanceFromTerrain = 100;

	// Token: 0x04002916 RID: 10518
	public int MaxDistanceFromTerrain = 500;

	// Token: 0x04002917 RID: 10519
	public int DistanceBetweenMonuments = 500;

	// Token: 0x04002918 RID: 10520
	[FormerlySerializedAs("MinSize")]
	public int MinWorldSize;

	// Token: 0x04002919 RID: 10521
	private const int Candidates = 10;

	// Token: 0x0400291A RID: 10522
	private const int Attempts = 10000;

	// Token: 0x02000E30 RID: 3632
	private struct SpawnInfo
	{
		// Token: 0x04004ACC RID: 19148
		public Prefab prefab;

		// Token: 0x04004ACD RID: 19149
		public Vector3 position;

		// Token: 0x04004ACE RID: 19150
		public Quaternion rotation;

		// Token: 0x04004ACF RID: 19151
		public Vector3 scale;
	}
}
