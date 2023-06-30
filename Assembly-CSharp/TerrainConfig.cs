using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200065F RID: 1631
[CreateAssetMenu(menuName = "Rust/Terrain Config")]
public class TerrainConfig : ScriptableObject
{
	// Token: 0x170003CE RID: 974
	// (get) Token: 0x06002F4D RID: 12109 RVA: 0x0011DEC6 File Offset: 0x0011C0C6
	public Texture AlbedoArray
	{
		get
		{
			return this.AlbedoArrays[Mathf.Clamp(QualitySettings.masterTextureLimit, 0, 2)];
		}
	}

	// Token: 0x170003CF RID: 975
	// (get) Token: 0x06002F4E RID: 12110 RVA: 0x0011DEDB File Offset: 0x0011C0DB
	public Texture NormalArray
	{
		get
		{
			return this.NormalArrays[Mathf.Clamp(QualitySettings.masterTextureLimit, 0, 2)];
		}
	}

	// Token: 0x06002F4F RID: 12111 RVA: 0x0011DEF0 File Offset: 0x0011C0F0
	public PhysicMaterial[] GetPhysicMaterials()
	{
		PhysicMaterial[] array = new PhysicMaterial[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].Material;
		}
		return array;
	}

	// Token: 0x06002F50 RID: 12112 RVA: 0x0011DF30 File Offset: 0x0011C130
	public Color[] GetAridColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].AridColor;
		}
		return array;
	}

	// Token: 0x06002F51 RID: 12113 RVA: 0x0011DF74 File Offset: 0x0011C174
	public void GetAridOverlayConstants(out Color[] color, out Vector4[] param)
	{
		color = new Color[this.Splats.Length];
		param = new Vector4[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			TerrainConfig.SplatOverlay aridOverlay = this.Splats[i].AridOverlay;
			color[i] = aridOverlay.Color.linear;
			param[i] = new Vector4(aridOverlay.Smoothness, aridOverlay.NormalIntensity, aridOverlay.BlendFactor, aridOverlay.BlendFalloff);
		}
	}

	// Token: 0x06002F52 RID: 12114 RVA: 0x0011DFF8 File Offset: 0x0011C1F8
	public Color[] GetTemperateColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].TemperateColor;
		}
		return array;
	}

	// Token: 0x06002F53 RID: 12115 RVA: 0x0011E03C File Offset: 0x0011C23C
	public void GetTemperateOverlayConstants(out Color[] color, out Vector4[] param)
	{
		color = new Color[this.Splats.Length];
		param = new Vector4[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			TerrainConfig.SplatOverlay temperateOverlay = this.Splats[i].TemperateOverlay;
			color[i] = temperateOverlay.Color.linear;
			param[i] = new Vector4(temperateOverlay.Smoothness, temperateOverlay.NormalIntensity, temperateOverlay.BlendFactor, temperateOverlay.BlendFalloff);
		}
	}

	// Token: 0x06002F54 RID: 12116 RVA: 0x0011E0C0 File Offset: 0x0011C2C0
	public Color[] GetTundraColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].TundraColor;
		}
		return array;
	}

	// Token: 0x06002F55 RID: 12117 RVA: 0x0011E104 File Offset: 0x0011C304
	public void GetTundraOverlayConstants(out Color[] color, out Vector4[] param)
	{
		color = new Color[this.Splats.Length];
		param = new Vector4[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			TerrainConfig.SplatOverlay tundraOverlay = this.Splats[i].TundraOverlay;
			color[i] = tundraOverlay.Color.linear;
			param[i] = new Vector4(tundraOverlay.Smoothness, tundraOverlay.NormalIntensity, tundraOverlay.BlendFactor, tundraOverlay.BlendFalloff);
		}
	}

	// Token: 0x06002F56 RID: 12118 RVA: 0x0011E188 File Offset: 0x0011C388
	public Color[] GetArcticColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].ArcticColor;
		}
		return array;
	}

	// Token: 0x06002F57 RID: 12119 RVA: 0x0011E1CC File Offset: 0x0011C3CC
	public void GetArcticOverlayConstants(out Color[] color, out Vector4[] param)
	{
		color = new Color[this.Splats.Length];
		param = new Vector4[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			TerrainConfig.SplatOverlay arcticOverlay = this.Splats[i].ArcticOverlay;
			color[i] = arcticOverlay.Color.linear;
			param[i] = new Vector4(arcticOverlay.Smoothness, arcticOverlay.NormalIntensity, arcticOverlay.BlendFactor, arcticOverlay.BlendFalloff);
		}
	}

	// Token: 0x06002F58 RID: 12120 RVA: 0x0011E250 File Offset: 0x0011C450
	public float[] GetSplatTiling()
	{
		float[] array = new float[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].SplatTiling;
		}
		return array;
	}

	// Token: 0x06002F59 RID: 12121 RVA: 0x0011E290 File Offset: 0x0011C490
	public float GetMaxSplatTiling()
	{
		float num = float.MinValue;
		for (int i = 0; i < this.Splats.Length; i++)
		{
			if (this.Splats[i].SplatTiling > num)
			{
				num = this.Splats[i].SplatTiling;
			}
		}
		return num;
	}

	// Token: 0x06002F5A RID: 12122 RVA: 0x0011E2D8 File Offset: 0x0011C4D8
	public float GetMinSplatTiling()
	{
		float num = float.MaxValue;
		for (int i = 0; i < this.Splats.Length; i++)
		{
			if (this.Splats[i].SplatTiling < num)
			{
				num = this.Splats[i].SplatTiling;
			}
		}
		return num;
	}

	// Token: 0x06002F5B RID: 12123 RVA: 0x0011E320 File Offset: 0x0011C520
	public Vector3[] GetPackedUVMIX()
	{
		Vector3[] array = new Vector3[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = new Vector3(this.Splats[i].UVMIXMult, this.Splats[i].UVMIXStart, this.Splats[i].UVMIXDist);
		}
		return array;
	}

	// Token: 0x06002F5C RID: 12124 RVA: 0x0011E384 File Offset: 0x0011C584
	public TerrainConfig.GroundType GetCurrentGroundType(bool isGrounded, RaycastHit hit)
	{
		if (string.IsNullOrEmpty(this.grassMatName))
		{
			this.dirtMatNames = new List<string>();
			this.stoneyMatNames = new List<string>();
			TerrainConfig.SplatType[] splats = this.Splats;
			int i = 0;
			while (i < splats.Length)
			{
				TerrainConfig.SplatType splatType = splats[i];
				string text = splatType.Name.ToLower();
				string name = splatType.Material.name;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 2296799147U)
				{
					if (num <= 1328097888U)
					{
						if (num != 1180566432U)
						{
							if (num == 1328097888U)
							{
								if (text == "forest")
								{
									goto IL_183;
								}
							}
						}
						else if (text == "dirt")
						{
							goto IL_183;
						}
					}
					else if (num != 2223183858U)
					{
						if (num == 2296799147U)
						{
							if (text == "stones")
							{
								goto IL_192;
							}
						}
					}
					else if (text == "snow")
					{
						this.snowMatName = name;
					}
				}
				else if (num <= 3000956154U)
				{
					if (num != 2993663101U)
					{
						if (num == 3000956154U)
						{
							if (text == "gravel")
							{
								goto IL_192;
							}
						}
					}
					else if (text == "grass")
					{
						this.grassMatName = name;
					}
				}
				else if (num != 3189014883U)
				{
					if (num == 3912378421U)
					{
						if (text == "tundra")
						{
							goto IL_183;
						}
					}
				}
				else if (text == "sand")
				{
					this.sandMatName = name;
				}
				IL_19F:
				i++;
				continue;
				IL_183:
				this.dirtMatNames.Add(name);
				goto IL_19F;
				IL_192:
				this.stoneyMatNames.Add(name);
				goto IL_19F;
			}
		}
		if (!isGrounded)
		{
			return TerrainConfig.GroundType.None;
		}
		if (hit.collider == null)
		{
			return TerrainConfig.GroundType.HardSurface;
		}
		PhysicMaterial materialAt = hit.collider.GetMaterialAt(hit.point);
		if (materialAt == null)
		{
			return TerrainConfig.GroundType.HardSurface;
		}
		string name2 = materialAt.name;
		if (name2 == this.grassMatName)
		{
			return TerrainConfig.GroundType.Grass;
		}
		if (name2 == this.sandMatName)
		{
			return TerrainConfig.GroundType.Sand;
		}
		if (name2 == this.snowMatName)
		{
			return TerrainConfig.GroundType.Snow;
		}
		for (int j = 0; j < this.dirtMatNames.Count; j++)
		{
			if (this.dirtMatNames[j] == name2)
			{
				return TerrainConfig.GroundType.Dirt;
			}
		}
		for (int k = 0; k < this.stoneyMatNames.Count; k++)
		{
			if (this.stoneyMatNames[k] == name2)
			{
				return TerrainConfig.GroundType.Gravel;
			}
		}
		return TerrainConfig.GroundType.HardSurface;
	}

	// Token: 0x040026FF RID: 9983
	public bool CastShadows = true;

	// Token: 0x04002700 RID: 9984
	public LayerMask GroundMask = 0;

	// Token: 0x04002701 RID: 9985
	public LayerMask WaterMask = 0;

	// Token: 0x04002702 RID: 9986
	public PhysicMaterial GenericMaterial;

	// Token: 0x04002703 RID: 9987
	public PhysicMaterial WaterMaterial;

	// Token: 0x04002704 RID: 9988
	public Material Material;

	// Token: 0x04002705 RID: 9989
	public Material MarginMaterial;

	// Token: 0x04002706 RID: 9990
	public Texture[] AlbedoArrays = new Texture[3];

	// Token: 0x04002707 RID: 9991
	public Texture[] NormalArrays = new Texture[3];

	// Token: 0x04002708 RID: 9992
	public float HeightMapErrorMin = 5f;

	// Token: 0x04002709 RID: 9993
	public float HeightMapErrorMax = 100f;

	// Token: 0x0400270A RID: 9994
	public float BaseMapDistanceMin = 100f;

	// Token: 0x0400270B RID: 9995
	public float BaseMapDistanceMax = 500f;

	// Token: 0x0400270C RID: 9996
	public float ShaderLodMin = 100f;

	// Token: 0x0400270D RID: 9997
	public float ShaderLodMax = 600f;

	// Token: 0x0400270E RID: 9998
	public TerrainConfig.SplatType[] Splats = new TerrainConfig.SplatType[8];

	// Token: 0x0400270F RID: 9999
	private string snowMatName;

	// Token: 0x04002710 RID: 10000
	private string grassMatName;

	// Token: 0x04002711 RID: 10001
	private string sandMatName;

	// Token: 0x04002712 RID: 10002
	private List<string> dirtMatNames;

	// Token: 0x04002713 RID: 10003
	private List<string> stoneyMatNames;

	// Token: 0x02000DCD RID: 3533
	[Serializable]
	public class SplatOverlay
	{
		// Token: 0x04004980 RID: 18816
		public Color Color = new Color(1f, 1f, 1f, 0f);

		// Token: 0x04004981 RID: 18817
		[Range(0f, 1f)]
		public float Smoothness;

		// Token: 0x04004982 RID: 18818
		[Range(0f, 1f)]
		public float NormalIntensity = 1f;

		// Token: 0x04004983 RID: 18819
		[Range(0f, 8f)]
		public float BlendFactor = 0.5f;

		// Token: 0x04004984 RID: 18820
		[Range(0.01f, 32f)]
		public float BlendFalloff = 0.5f;
	}

	// Token: 0x02000DCE RID: 3534
	[Serializable]
	public class SplatType
	{
		// Token: 0x04004985 RID: 18821
		public string Name = "";

		// Token: 0x04004986 RID: 18822
		[FormerlySerializedAs("WarmColor")]
		public Color AridColor = Color.white;

		// Token: 0x04004987 RID: 18823
		public TerrainConfig.SplatOverlay AridOverlay = new TerrainConfig.SplatOverlay();

		// Token: 0x04004988 RID: 18824
		[FormerlySerializedAs("Color")]
		public Color TemperateColor = Color.white;

		// Token: 0x04004989 RID: 18825
		public TerrainConfig.SplatOverlay TemperateOverlay = new TerrainConfig.SplatOverlay();

		// Token: 0x0400498A RID: 18826
		[FormerlySerializedAs("ColdColor")]
		public Color TundraColor = Color.white;

		// Token: 0x0400498B RID: 18827
		public TerrainConfig.SplatOverlay TundraOverlay = new TerrainConfig.SplatOverlay();

		// Token: 0x0400498C RID: 18828
		[FormerlySerializedAs("ColdColor")]
		public Color ArcticColor = Color.white;

		// Token: 0x0400498D RID: 18829
		public TerrainConfig.SplatOverlay ArcticOverlay = new TerrainConfig.SplatOverlay();

		// Token: 0x0400498E RID: 18830
		public PhysicMaterial Material;

		// Token: 0x0400498F RID: 18831
		public float SplatTiling = 5f;

		// Token: 0x04004990 RID: 18832
		[Range(0f, 1f)]
		public float UVMIXMult = 0.15f;

		// Token: 0x04004991 RID: 18833
		public float UVMIXStart;

		// Token: 0x04004992 RID: 18834
		public float UVMIXDist = 100f;
	}

	// Token: 0x02000DCF RID: 3535
	public enum GroundType
	{
		// Token: 0x04004994 RID: 18836
		None,
		// Token: 0x04004995 RID: 18837
		HardSurface,
		// Token: 0x04004996 RID: 18838
		Grass,
		// Token: 0x04004997 RID: 18839
		Sand,
		// Token: 0x04004998 RID: 18840
		Snow,
		// Token: 0x04004999 RID: 18841
		Dirt,
		// Token: 0x0400499A RID: 18842
		Gravel
	}
}
