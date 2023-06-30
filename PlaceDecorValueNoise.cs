using System;
using UnityEngine;

// Token: 0x020006E2 RID: 1762
public class PlaceDecorValueNoise : ProceduralComponent
{
	// Token: 0x06003233 RID: 12851 RVA: 0x00133408 File Offset: 0x00131608
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			World.Spawn("Decor", "assets/bundled/prefabs/autospawn/" + this.ResourceFolder + "/");
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, null, null, true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		int num = Mathf.RoundToInt(this.ObjectDensity * size.x * size.z * 1E-06f);
		float x = position.x;
		float z = position.z;
		float num2 = position.x + size.x;
		float num3 = position.z + size.z;
		float num4 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
		float num5 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
		int octaves = this.Cluster.Octaves;
		float offset = this.Cluster.Offset;
		float num6 = this.Cluster.Frequency * 0.01f;
		float amplitude = this.Cluster.Amplitude;
		for (int i = 0; i < num; i++)
		{
			float num7 = SeedRandom.Range(ref seed, x, num2);
			float num8 = SeedRandom.Range(ref seed, z, num3);
			float num9 = TerrainMeta.NormalizeX(num7);
			float num10 = TerrainMeta.NormalizeZ(num8);
			float num11 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(num9, num10, true);
			Prefab random = array.GetRandom(ref seed);
			if (factor > 0f && (offset + Noise.Turbulence(num4 + num7, num5 + num8, octaves, num6, amplitude, 2f, 0.5f)) * factor * factor >= num11)
			{
				float height = heightMap.GetHeight(num9, num10);
				Vector3 vector = new Vector3(num7, height, num8);
				Quaternion localRotation = random.Object.transform.localRotation;
				Vector3 localScale = random.Object.transform.localScale;
				random.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
				if (random.ApplyTerrainAnchors(ref vector, localRotation, localScale, this.Filter) && random.ApplyTerrainChecks(vector, localRotation, localScale, this.Filter) && random.ApplyTerrainFilters(vector, localRotation, localScale, null) && random.ApplyWaterChecks(vector, localRotation, localScale))
				{
					World.AddPrefab("Decor", random, vector, localRotation, localScale);
				}
			}
		}
	}

	// Token: 0x040028FD RID: 10493
	public SpawnFilter Filter;

	// Token: 0x040028FE RID: 10494
	public string ResourceFolder = string.Empty;

	// Token: 0x040028FF RID: 10495
	public NoiseParameters Cluster = new NoiseParameters(2, 0.5f, 1f, 0f);

	// Token: 0x04002900 RID: 10496
	public float ObjectDensity = 100f;
}
