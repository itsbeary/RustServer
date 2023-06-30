using System;
using UnityEngine;

// Token: 0x020006E3 RID: 1763
public class PlaceDecorWhiteNoise : ProceduralComponent
{
	// Token: 0x06003235 RID: 12853 RVA: 0x001336A0 File Offset: 0x001318A0
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
		for (int i = 0; i < num; i++)
		{
			float num4 = SeedRandom.Range(ref seed, x, num2);
			float num5 = SeedRandom.Range(ref seed, z, num3);
			float num6 = TerrainMeta.NormalizeX(num4);
			float num7 = TerrainMeta.NormalizeZ(num5);
			float num8 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(num6, num7, true);
			Prefab random = array.GetRandom(ref seed);
			if (factor * factor >= num8)
			{
				float height = heightMap.GetHeight(num6, num7);
				Vector3 vector = new Vector3(num4, height, num5);
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

	// Token: 0x04002901 RID: 10497
	public SpawnFilter Filter;

	// Token: 0x04002902 RID: 10498
	public string ResourceFolder = string.Empty;

	// Token: 0x04002903 RID: 10499
	public float ObjectDensity = 100f;
}
