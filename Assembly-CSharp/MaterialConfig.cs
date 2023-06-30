using System;
using UnityEngine;

// Token: 0x02000909 RID: 2313
[CreateAssetMenu(menuName = "Rust/Material Config")]
public class MaterialConfig : ScriptableObject
{
	// Token: 0x060037F0 RID: 14320 RVA: 0x0014DCA0 File Offset: 0x0014BEA0
	public MaterialPropertyBlock GetMaterialPropertyBlock(Material mat, Vector3 pos, Vector3 scale)
	{
		if (this.properties == null)
		{
			this.properties = new MaterialPropertyBlock();
		}
		this.properties.Clear();
		for (int i = 0; i < this.Floats.Length; i++)
		{
			MaterialConfig.ShaderParametersFloat shaderParametersFloat = this.Floats[i];
			float num2;
			float num3;
			float num = shaderParametersFloat.FindBlendParameters(pos, out num2, out num3);
			this.properties.SetFloat(shaderParametersFloat.Name, Mathf.Lerp(num2, num3, num));
		}
		for (int j = 0; j < this.Colors.Length; j++)
		{
			MaterialConfig.ShaderParametersColor shaderParametersColor = this.Colors[j];
			Color color;
			Color color2;
			float num4 = shaderParametersColor.FindBlendParameters(pos, out color, out color2);
			this.properties.SetColor(shaderParametersColor.Name, Color.Lerp(color, color2, num4));
		}
		for (int k = 0; k < this.Textures.Length; k++)
		{
			MaterialConfig.ShaderParametersTexture shaderParametersTexture = this.Textures[k];
			Texture texture = shaderParametersTexture.FindBlendParameters(pos);
			if (texture)
			{
				this.properties.SetTexture(shaderParametersTexture.Name, texture);
			}
		}
		for (int l = 0; l < this.ScaleUV.Length; l++)
		{
			Vector4 vector = mat.GetVector(this.ScaleUV[l]);
			vector = new Vector4(vector.x * scale.y, vector.y * scale.y, vector.z, vector.w);
			this.properties.SetVector(this.ScaleUV[l], vector);
		}
		return this.properties;
	}

	// Token: 0x0400334E RID: 13134
	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersFloat[] Floats;

	// Token: 0x0400334F RID: 13135
	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersColor[] Colors;

	// Token: 0x04003350 RID: 13136
	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersTexture[] Textures;

	// Token: 0x04003351 RID: 13137
	public string[] ScaleUV;

	// Token: 0x04003352 RID: 13138
	private MaterialPropertyBlock properties;

	// Token: 0x02000EC3 RID: 3779
	public class ShaderParameters<T>
	{
		// Token: 0x0600534F RID: 21327 RVA: 0x001B2100 File Offset: 0x001B0300
		public float FindBlendParameters(Vector3 pos, out T src, out T dst)
		{
			if (TerrainMeta.BiomeMap == null)
			{
				src = this.Temperate;
				dst = this.Tundra;
				return 0f;
			}
			if (this.climates == null || this.climates.Length == 0)
			{
				this.climates = new T[] { this.Arid, this.Temperate, this.Tundra, this.Arctic };
			}
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
			int biomeMaxType2 = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, ~biomeMaxType);
			src = this.climates[TerrainBiome.TypeToIndex(biomeMaxType)];
			dst = this.climates[TerrainBiome.TypeToIndex(biomeMaxType2)];
			return TerrainMeta.BiomeMap.GetBiome(pos, biomeMaxType2);
		}

		// Token: 0x06005350 RID: 21328 RVA: 0x001B21E0 File Offset: 0x001B03E0
		public T FindBlendParameters(Vector3 pos)
		{
			if (TerrainMeta.BiomeMap == null)
			{
				return this.Temperate;
			}
			if (this.climates == null || this.climates.Length == 0)
			{
				this.climates = new T[] { this.Arid, this.Temperate, this.Tundra, this.Arctic };
			}
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
			return this.climates[TerrainBiome.TypeToIndex(biomeMaxType)];
		}

		// Token: 0x04004D4F RID: 19791
		public string Name;

		// Token: 0x04004D50 RID: 19792
		public T Arid;

		// Token: 0x04004D51 RID: 19793
		public T Temperate;

		// Token: 0x04004D52 RID: 19794
		public T Tundra;

		// Token: 0x04004D53 RID: 19795
		public T Arctic;

		// Token: 0x04004D54 RID: 19796
		private T[] climates;
	}

	// Token: 0x02000EC4 RID: 3780
	[Serializable]
	public class ShaderParametersFloat : MaterialConfig.ShaderParameters<float>
	{
	}

	// Token: 0x02000EC5 RID: 3781
	[Serializable]
	public class ShaderParametersColor : MaterialConfig.ShaderParameters<Color>
	{
	}

	// Token: 0x02000EC6 RID: 3782
	[Serializable]
	public class ShaderParametersTexture : MaterialConfig.ShaderParameters<Texture>
	{
	}
}
