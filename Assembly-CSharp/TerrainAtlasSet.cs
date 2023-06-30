using System;
using UnityEngine;

// Token: 0x0200069C RID: 1692
[CreateAssetMenu(menuName = "Rust/Terrain Atlas Set")]
public class TerrainAtlasSet : ScriptableObject
{
	// Token: 0x06003033 RID: 12339 RVA: 0x00121C88 File Offset: 0x0011FE88
	public void CheckReset()
	{
		if (this.splatNames == null)
		{
			this.splatNames = new string[] { "Dirt", "Snow", "Sand", "Rock", "Grass", "Forest", "Stones", "Gravel" };
		}
		else if (this.splatNames.Length != 8)
		{
			Array.Resize<string>(ref this.splatNames, 8);
		}
		if (this.albedoHighpass == null)
		{
			this.albedoHighpass = new bool[8];
		}
		else if (this.albedoHighpass.Length != 8)
		{
			Array.Resize<bool>(ref this.albedoHighpass, 8);
		}
		if (this.albedoPaths == null)
		{
			this.albedoPaths = new string[8];
		}
		else if (this.albedoPaths.Length != 8)
		{
			Array.Resize<string>(ref this.albedoPaths, 8);
		}
		if (this.defaultValues == null)
		{
			this.defaultValues = new Color[]
			{
				new Color(1f, 1f, 1f, 0.5f),
				new Color(0.5f, 0.5f, 1f, 0f),
				new Color(0f, 0f, 1f, 0.5f)
			};
		}
		else if (this.defaultValues.Length != 3)
		{
			Array.Resize<Color>(ref this.defaultValues, 3);
		}
		if (this.sourceMaps == null)
		{
			this.sourceMaps = new TerrainAtlasSet.SourceMapSet[3];
		}
		else if (this.sourceMaps.Length != 3)
		{
			Array.Resize<TerrainAtlasSet.SourceMapSet>(ref this.sourceMaps, 3);
		}
		for (int i = 0; i < 3; i++)
		{
			this.sourceMaps[i] = ((this.sourceMaps[i] != null) ? this.sourceMaps[i] : new TerrainAtlasSet.SourceMapSet());
			this.sourceMaps[i].CheckReset();
		}
	}

	// Token: 0x040027DB RID: 10203
	public const int SplatCount = 8;

	// Token: 0x040027DC RID: 10204
	public const int SplatSize = 2048;

	// Token: 0x040027DD RID: 10205
	public const int MaxSplatSize = 2047;

	// Token: 0x040027DE RID: 10206
	public const int SplatPadding = 256;

	// Token: 0x040027DF RID: 10207
	public const int AtlasSize = 8192;

	// Token: 0x040027E0 RID: 10208
	public const int RegionSize = 2560;

	// Token: 0x040027E1 RID: 10209
	public const int SplatsPerLine = 3;

	// Token: 0x040027E2 RID: 10210
	public const int SourceTypeCount = 3;

	// Token: 0x040027E3 RID: 10211
	public const int AtlasMipCount = 10;

	// Token: 0x040027E4 RID: 10212
	public static string[] sourceTypeNames = new string[] { "Albedo", "Normal", "Packed" };

	// Token: 0x040027E5 RID: 10213
	public static string[] sourceTypeNamesExt = new string[] { "Albedo (rgb)", "Normal (rgb)", "Metal[ignored]_Height_AO_Gloss (rgba)" };

	// Token: 0x040027E6 RID: 10214
	public static string[] sourceTypePostfix = new string[] { "_albedo", "_normal", "_metal_hm_ao_gloss" };

	// Token: 0x040027E7 RID: 10215
	public string[] splatNames;

	// Token: 0x040027E8 RID: 10216
	public bool[] albedoHighpass;

	// Token: 0x040027E9 RID: 10217
	public string[] albedoPaths;

	// Token: 0x040027EA RID: 10218
	public Color[] defaultValues;

	// Token: 0x040027EB RID: 10219
	public TerrainAtlasSet.SourceMapSet[] sourceMaps;

	// Token: 0x040027EC RID: 10220
	public bool highQualityCompression = true;

	// Token: 0x040027ED RID: 10221
	public bool generateTextureAtlases = true;

	// Token: 0x040027EE RID: 10222
	public bool generateTextureArrays;

	// Token: 0x040027EF RID: 10223
	public string splatSearchPrefix = "terrain_";

	// Token: 0x040027F0 RID: 10224
	public string splatSearchFolder = "Assets/Content/Nature/Terrain";

	// Token: 0x040027F1 RID: 10225
	public string albedoAtlasSavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_albedo_atlas";

	// Token: 0x040027F2 RID: 10226
	public string normalAtlasSavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_normal_atlas";

	// Token: 0x040027F3 RID: 10227
	public string albedoArraySavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_albedo_array";

	// Token: 0x040027F4 RID: 10228
	public string normalArraySavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_normal_array";

	// Token: 0x02000DD6 RID: 3542
	public enum SourceType
	{
		// Token: 0x040049B7 RID: 18871
		ALBEDO,
		// Token: 0x040049B8 RID: 18872
		NORMAL,
		// Token: 0x040049B9 RID: 18873
		PACKED,
		// Token: 0x040049BA RID: 18874
		COUNT
	}

	// Token: 0x02000DD7 RID: 3543
	[Serializable]
	public class SourceMapSet
	{
		// Token: 0x060051AC RID: 20908 RVA: 0x001AD4BE File Offset: 0x001AB6BE
		internal void CheckReset()
		{
			if (this.maps == null)
			{
				this.maps = new Texture2D[8];
				return;
			}
			if (this.maps.Length != 8)
			{
				Array.Resize<Texture2D>(ref this.maps, 8);
			}
		}

		// Token: 0x040049BB RID: 18875
		public Texture2D[] maps;
	}
}
