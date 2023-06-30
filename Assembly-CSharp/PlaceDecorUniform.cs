using System;
using UnityEngine;

// Token: 0x020006E1 RID: 1761
public class PlaceDecorUniform : ProceduralComponent
{
	// Token: 0x06003231 RID: 12849 RVA: 0x001331FC File Offset: 0x001313FC
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
		float x = position.x;
		float z = position.z;
		float num = position.x + size.x;
		float num2 = position.z + size.z;
		for (float num3 = z; num3 < num2; num3 += this.ObjectDistance)
		{
			for (float num4 = x; num4 < num; num4 += this.ObjectDistance)
			{
				float num5 = num4 + SeedRandom.Range(ref seed, -this.ObjectDithering, this.ObjectDithering);
				float num6 = num3 + SeedRandom.Range(ref seed, -this.ObjectDithering, this.ObjectDithering);
				float num7 = TerrainMeta.NormalizeX(num5);
				float num8 = TerrainMeta.NormalizeZ(num6);
				float num9 = SeedRandom.Value(ref seed);
				float factor = this.Filter.GetFactor(num7, num8, true);
				Prefab random = array.GetRandom(ref seed);
				if (factor * factor >= num9)
				{
					float height = heightMap.GetHeight(num7, num8);
					Vector3 vector = new Vector3(num5, height, num6);
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
	}

	// Token: 0x040028F9 RID: 10489
	public SpawnFilter Filter;

	// Token: 0x040028FA RID: 10490
	public string ResourceFolder = string.Empty;

	// Token: 0x040028FB RID: 10491
	public float ObjectDistance = 10f;

	// Token: 0x040028FC RID: 10492
	public float ObjectDithering = 5f;
}
