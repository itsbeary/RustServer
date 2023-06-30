using System;
using UnityEngine;

// Token: 0x020006E4 RID: 1764
public class PlaceMonument : ProceduralComponent
{
	// Token: 0x06003237 RID: 12855 RVA: 0x00133888 File Offset: 0x00131A88
	public override void Process(uint seed)
	{
		if (!this.Monument.isValid)
		{
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float num = position.x + size.x;
		float num2 = position.z + size.z;
		PlaceMonument.SpawnInfo spawnInfo = default(PlaceMonument.SpawnInfo);
		int num3 = int.MinValue;
		Prefab<MonumentInfo> prefab = Prefab.Load<MonumentInfo>(this.Monument.resourceID, null, null);
		for (int i = 0; i < 10000; i++)
		{
			float num4 = SeedRandom.Range(ref seed, x, num);
			float num5 = SeedRandom.Range(ref seed, z, num2);
			float num6 = TerrainMeta.NormalizeX(num4);
			float num7 = TerrainMeta.NormalizeZ(num5);
			float num8 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(num6, num7, true);
			if (factor * factor >= num8)
			{
				float height = heightMap.GetHeight(num6, num7);
				Vector3 vector = new Vector3(num4, height, num5);
				Quaternion localRotation = prefab.Object.transform.localRotation;
				Vector3 localScale = prefab.Object.transform.localScale;
				prefab.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
				if ((!prefab.Component || prefab.Component.CheckPlacement(vector, localRotation, localScale)) && prefab.ApplyTerrainAnchors(ref vector, localRotation, localScale, this.Filter) && prefab.ApplyTerrainChecks(vector, localRotation, localScale, this.Filter) && prefab.ApplyTerrainFilters(vector, localRotation, localScale, null) && prefab.ApplyWaterChecks(vector, localRotation, localScale) && !prefab.CheckEnvironmentVolumes(vector, localRotation, localScale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
				{
					PlaceMonument.SpawnInfo spawnInfo2 = default(PlaceMonument.SpawnInfo);
					spawnInfo2.prefab = prefab;
					spawnInfo2.position = vector;
					spawnInfo2.rotation = localRotation;
					spawnInfo2.scale = localScale;
					int num9 = -Mathf.RoundToInt(vector.Magnitude2D());
					if (num9 > num3)
					{
						num3 = num9;
						spawnInfo = spawnInfo2;
					}
				}
			}
		}
		if (num3 != -2147483648)
		{
			World.AddPrefab("Monument", spawnInfo.prefab, spawnInfo.position, spawnInfo.rotation, spawnInfo.scale);
		}
	}

	// Token: 0x04002904 RID: 10500
	public SpawnFilter Filter;

	// Token: 0x04002905 RID: 10501
	public GameObjectRef Monument;

	// Token: 0x04002906 RID: 10502
	private const int Attempts = 10000;

	// Token: 0x02000E2B RID: 3627
	private struct SpawnInfo
	{
		// Token: 0x04004AB6 RID: 19126
		public Prefab prefab;

		// Token: 0x04004AB7 RID: 19127
		public Vector3 position;

		// Token: 0x04004AB8 RID: 19128
		public Quaternion rotation;

		// Token: 0x04004AB9 RID: 19129
		public Vector3 scale;
	}
}
