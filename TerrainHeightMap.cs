using System;
using UnityEngine;

// Token: 0x020006A6 RID: 1702
public class TerrainHeightMap : TerrainMap<short>
{
	// Token: 0x06003087 RID: 12423 RVA: 0x001236D8 File Offset: 0x001218D8
	public override void Setup()
	{
		this.res = this.terrain.terrainData.heightmapResolution;
		this.src = (this.dst = new short[this.res * this.res]);
		this.normY = TerrainMeta.Size.x / TerrainMeta.Size.y / (float)this.res;
		if (this.HeightTexture != null)
		{
			if (this.HeightTexture.width == this.HeightTexture.height && this.HeightTexture.width == this.res)
			{
				Color32[] pixels = this.HeightTexture.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						Color32 color = pixels[num];
						this.dst[i * this.res + j] = BitUtility.DecodeShort(color);
						j++;
						num++;
					}
					i++;
				}
				return;
			}
			Debug.LogError("Invalid height texture: " + this.HeightTexture.name);
		}
	}

	// Token: 0x06003088 RID: 12424 RVA: 0x001237F0 File Offset: 0x001219F0
	public void ApplyToTerrain()
	{
		float[,] heights = this.terrain.terrainData.GetHeights(0, 0, this.res, this.res);
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				heights[z, i] = this.GetHeight01(i, z);
			}
		});
		this.terrain.terrainData.SetHeights(0, 0, heights);
		TerrainCollider component = this.terrain.GetComponent<TerrainCollider>();
		if (component)
		{
			component.enabled = false;
			component.enabled = true;
		}
	}

	// Token: 0x06003089 RID: 12425 RVA: 0x00123880 File Offset: 0x00121A80
	public void GenerateTextures(bool heightTexture = true, bool normalTexture = true)
	{
		if (heightTexture)
		{
			Color32[] heights = new Color32[this.res * this.res];
			Parallel.For(0, this.res, delegate(int z)
			{
				for (int i = 0; i < this.res; i++)
				{
					heights[z * this.res + i] = BitUtility.EncodeShort(this.src[z * this.res + i]);
				}
			});
			this.HeightTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
			this.HeightTexture.name = "HeightTexture";
			this.HeightTexture.wrapMode = TextureWrapMode.Clamp;
			this.HeightTexture.SetPixels32(heights);
		}
		if (normalTexture)
		{
			int normalres = (this.res - 1) / 2;
			Color32[] normals = new Color32[normalres * normalres];
			Parallel.For(0, normalres, delegate(int z)
			{
				float num = ((float)z + 0.5f) / (float)normalres;
				for (int j = 0; j < normalres; j++)
				{
					float num2 = ((float)j + 0.5f) / (float)normalres;
					Vector3 vector = this.GetNormal(num2, num);
					float num3 = Vector3.Angle(Vector3.up, vector);
					float num4 = Mathf.InverseLerp(50f, 70f, num3);
					vector = Vector3.Slerp(vector, Vector3.up, num4);
					normals[z * normalres + j] = BitUtility.EncodeNormal(vector);
				}
			});
			this.NormalTexture = new Texture2D(normalres, normalres, TextureFormat.RGBA32, false, true);
			this.NormalTexture.name = "NormalTexture";
			this.NormalTexture.wrapMode = TextureWrapMode.Clamp;
			this.NormalTexture.SetPixels32(normals);
		}
	}

	// Token: 0x0600308A RID: 12426 RVA: 0x001239B4 File Offset: 0x00121BB4
	public void ApplyTextures()
	{
		this.HeightTexture.Apply(true, false);
		this.NormalTexture.Apply(true, false);
		this.NormalTexture.Compress(false);
		this.HeightTexture.Apply(false, true);
		this.NormalTexture.Apply(false, true);
	}

	// Token: 0x0600308B RID: 12427 RVA: 0x00123A01 File Offset: 0x00121C01
	public float GetHeight(Vector3 worldPos)
	{
		return TerrainMeta.Position.y + this.GetHeight01(worldPos) * TerrainMeta.Size.y;
	}

	// Token: 0x0600308C RID: 12428 RVA: 0x00123A20 File Offset: 0x00121C20
	public float GetHeight(float normX, float normZ)
	{
		return TerrainMeta.Position.y + this.GetHeight01(normX, normZ) * TerrainMeta.Size.y;
	}

	// Token: 0x0600308D RID: 12429 RVA: 0x00123A40 File Offset: 0x00121C40
	public float GetHeightFast(Vector2 uv)
	{
		int num = this.res - 1;
		float num2 = uv.x * (float)num;
		float num3 = uv.y * (float)num;
		int num4 = (int)num2;
		int num5 = (int)num3;
		float num6 = num2 - (float)num4;
		float num7 = num3 - (float)num5;
		num4 = ((num4 >= 0) ? num4 : 0);
		num5 = ((num5 >= 0) ? num5 : 0);
		num4 = ((num4 <= num) ? num4 : num);
		num5 = ((num5 <= num) ? num5 : num);
		int num8 = ((num2 < (float)num) ? 1 : 0);
		int num9 = ((num3 < (float)num) ? this.res : 0);
		int num10 = num5 * this.res + num4;
		int num11 = num10 + num8;
		int num12 = num10 + num9;
		int num13 = num12 + num8;
		float num14 = (float)this.src[num10] * 3.051944E-05f;
		float num15 = (float)this.src[num11] * 3.051944E-05f;
		float num16 = (float)this.src[num12] * 3.051944E-05f;
		float num17 = (float)this.src[num13] * 3.051944E-05f;
		float num18 = (num15 - num14) * num6 + num14;
		float num19 = ((num17 - num16) * num6 + num16 - num18) * num7 + num18;
		return TerrainMeta.Position.y + num19 * TerrainMeta.Size.y;
	}

	// Token: 0x0600308E RID: 12430 RVA: 0x00123B59 File Offset: 0x00121D59
	public float GetHeight(int x, int z)
	{
		return TerrainMeta.Position.y + this.GetHeight01(x, z) * TerrainMeta.Size.y;
	}

	// Token: 0x0600308F RID: 12431 RVA: 0x00123B7C File Offset: 0x00121D7C
	public float GetHeight01(Vector3 worldPos)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetHeight01(num, num2);
	}

	// Token: 0x06003090 RID: 12432 RVA: 0x00123BAC File Offset: 0x00121DAC
	public float GetHeight01(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int num6 = Mathf.Min(num4 + 1, num);
		int num7 = Mathf.Min(num5 + 1, num);
		float height = this.GetHeight01(num4, num5);
		float height2 = this.GetHeight01(num6, num5);
		float height3 = this.GetHeight01(num4, num7);
		float height4 = this.GetHeight01(num6, num7);
		float num8 = num2 - (float)num4;
		float num9 = num3 - (float)num5;
		float num10 = Mathf.Lerp(height, height2, num8);
		float num11 = Mathf.Lerp(height3, height4, num8);
		return Mathf.Lerp(num10, num11, num9);
	}

	// Token: 0x06003091 RID: 12433 RVA: 0x00123C50 File Offset: 0x00121E50
	public float GetTriangulatedHeight01(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int num6 = Mathf.Min(num4 + 1, num);
		int num7 = Mathf.Min(num5 + 1, num);
		float num8 = num2 - (float)num4;
		float num9 = num3 - (float)num5;
		float height = this.GetHeight01(num4, num5);
		float height2 = this.GetHeight01(num6, num7);
		if (num8 > num9)
		{
			float height3 = this.GetHeight01(num6, num5);
			return height + (height3 - height) * num8 + (height2 - height3) * num9;
		}
		float height4 = this.GetHeight01(num4, num7);
		return height + (height2 - height4) * num8 + (height4 - height) * num9;
	}

	// Token: 0x06003092 RID: 12434 RVA: 0x00123CFF File Offset: 0x00121EFF
	public float GetHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06003093 RID: 12435 RVA: 0x00123CFF File Offset: 0x00121EFF
	private float GetSrcHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06003094 RID: 12436 RVA: 0x00123D17 File Offset: 0x00121F17
	private float GetDstHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.dst[z * this.res + x]);
	}

	// Token: 0x06003095 RID: 12437 RVA: 0x00123D30 File Offset: 0x00121F30
	public Vector3 GetNormal(Vector3 worldPos)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetNormal(num, num2);
	}

	// Token: 0x06003096 RID: 12438 RVA: 0x00123D60 File Offset: 0x00121F60
	public Vector3 GetNormal(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int num6 = Mathf.Min(num4 + 1, num);
		int num7 = Mathf.Min(num5 + 1, num);
		Vector3 normal = this.GetNormal(num4, num5);
		Vector3 normal2 = this.GetNormal(num6, num5);
		Vector3 normal3 = this.GetNormal(num4, num7);
		Vector3 normal4 = this.GetNormal(num6, num7);
		float num8 = num2 - (float)num4;
		float num9 = num3 - (float)num5;
		Vector3 vector = Vector3.Slerp(normal, normal2, num8);
		Vector3 vector2 = Vector3.Slerp(normal3, normal4, num8);
		return Vector3.Slerp(vector, vector2, num9).normalized;
	}

	// Token: 0x06003097 RID: 12439 RVA: 0x00123E10 File Offset: 0x00122010
	public Vector3 GetNormal(int x, int z)
	{
		int num = this.res - 1;
		int num2 = Mathf.Clamp(x - 1, 0, num);
		int num3 = Mathf.Clamp(z - 1, 0, num);
		int num4 = Mathf.Clamp(x + 1, 0, num);
		int num5 = Mathf.Clamp(z + 1, 0, num);
		float num6 = (this.GetHeight01(num4, num3) - this.GetHeight01(num2, num3)) * 0.5f;
		float num7 = (this.GetHeight01(num2, num5) - this.GetHeight01(num2, num3)) * 0.5f;
		return new Vector3(-num6, this.normY, -num7).normalized;
	}

	// Token: 0x06003098 RID: 12440 RVA: 0x00123E9C File Offset: 0x0012209C
	private Vector3 GetNormalSobel(int x, int z)
	{
		int num = this.res - 1;
		Vector3 vector = new Vector3(TerrainMeta.Size.x / (float)num, TerrainMeta.Size.y, TerrainMeta.Size.z / (float)num);
		int num2 = Mathf.Clamp(x - 1, 0, num);
		int num3 = Mathf.Clamp(z - 1, 0, num);
		int num4 = Mathf.Clamp(x + 1, 0, num);
		int num5 = Mathf.Clamp(z + 1, 0, num);
		float num6 = this.GetHeight01(num2, num3) * -1f;
		num6 += this.GetHeight01(num2, z) * -2f;
		num6 += this.GetHeight01(num2, num5) * -1f;
		num6 += this.GetHeight01(num4, num3) * 1f;
		num6 += this.GetHeight01(num4, z) * 2f;
		num6 += this.GetHeight01(num4, num5) * 1f;
		num6 *= vector.y;
		num6 /= vector.x;
		float num7 = this.GetHeight01(num2, num3) * -1f;
		num7 += this.GetHeight01(x, num3) * -2f;
		num7 += this.GetHeight01(num4, num3) * -1f;
		num7 += this.GetHeight01(num2, num5) * 1f;
		num7 += this.GetHeight01(x, num5) * 2f;
		num7 += this.GetHeight01(num4, num5) * 1f;
		num7 *= vector.y;
		num7 /= vector.z;
		Vector3 vector2 = new Vector3(-num6, 8f, -num7);
		return vector2.normalized;
	}

	// Token: 0x06003099 RID: 12441 RVA: 0x00124048 File Offset: 0x00122248
	public float GetSlope(Vector3 worldPos)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(worldPos));
	}

	// Token: 0x0600309A RID: 12442 RVA: 0x0012405B File Offset: 0x0012225B
	public float GetSlope(float normX, float normZ)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(normX, normZ));
	}

	// Token: 0x0600309B RID: 12443 RVA: 0x0012406F File Offset: 0x0012226F
	public float GetSlope(int x, int z)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(x, z));
	}

	// Token: 0x0600309C RID: 12444 RVA: 0x00124083 File Offset: 0x00122283
	public float GetSlope01(Vector3 worldPos)
	{
		return this.GetSlope(worldPos) * 0.011111111f;
	}

	// Token: 0x0600309D RID: 12445 RVA: 0x00124092 File Offset: 0x00122292
	public float GetSlope01(float normX, float normZ)
	{
		return this.GetSlope(normX, normZ) * 0.011111111f;
	}

	// Token: 0x0600309E RID: 12446 RVA: 0x001240A2 File Offset: 0x001222A2
	public float GetSlope01(int x, int z)
	{
		return this.GetSlope(x, z) * 0.011111111f;
	}

	// Token: 0x0600309F RID: 12447 RVA: 0x001240B4 File Offset: 0x001222B4
	public void SetHeight(Vector3 worldPos, float height)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetHeight(num, num2, height);
	}

	// Token: 0x060030A0 RID: 12448 RVA: 0x001240E4 File Offset: 0x001222E4
	public void SetHeight(float normX, float normZ, float height)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.SetHeight(num, num2, height);
	}

	// Token: 0x060030A1 RID: 12449 RVA: 0x0012410A File Offset: 0x0012230A
	public void SetHeight(int x, int z, float height)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Short(height);
	}

	// Token: 0x060030A2 RID: 12450 RVA: 0x00124124 File Offset: 0x00122324
	public void SetHeight(Vector3 worldPos, float height, float opacity)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetHeight(num, num2, height, opacity);
	}

	// Token: 0x060030A3 RID: 12451 RVA: 0x00124154 File Offset: 0x00122354
	public void SetHeight(float normX, float normZ, float height, float opacity)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.SetHeight(num, num2, height, opacity);
	}

	// Token: 0x060030A4 RID: 12452 RVA: 0x0012417C File Offset: 0x0012237C
	public void SetHeight(int x, int z, float height, float opacity)
	{
		float num = Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity);
		this.SetHeight(x, z, num);
	}

	// Token: 0x060030A5 RID: 12453 RVA: 0x001241A4 File Offset: 0x001223A4
	public void AddHeight(Vector3 worldPos, float delta)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddHeight(num, num2, delta);
	}

	// Token: 0x060030A6 RID: 12454 RVA: 0x001241D4 File Offset: 0x001223D4
	public void AddHeight(float normX, float normZ, float delta)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.AddHeight(num, num2, delta);
	}

	// Token: 0x060030A7 RID: 12455 RVA: 0x001241FC File Offset: 0x001223FC
	public void AddHeight(int x, int z, float delta)
	{
		float num = Mathf.Clamp01(this.GetDstHeight01(x, z) + delta);
		this.SetHeight(x, z, num);
	}

	// Token: 0x060030A8 RID: 12456 RVA: 0x00124224 File Offset: 0x00122424
	public void LowerHeight(Vector3 worldPos, float height, float opacity)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.LowerHeight(num, num2, height, opacity);
	}

	// Token: 0x060030A9 RID: 12457 RVA: 0x00124254 File Offset: 0x00122454
	public void LowerHeight(float normX, float normZ, float height, float opacity)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.LowerHeight(num, num2, height, opacity);
	}

	// Token: 0x060030AA RID: 12458 RVA: 0x0012427C File Offset: 0x0012247C
	public void LowerHeight(int x, int z, float height, float opacity)
	{
		float num = Mathf.Min(this.GetDstHeight01(x, z), Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity));
		this.SetHeight(x, z, num);
	}

	// Token: 0x060030AB RID: 12459 RVA: 0x001242B0 File Offset: 0x001224B0
	public void RaiseHeight(Vector3 worldPos, float height, float opacity)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.RaiseHeight(num, num2, height, opacity);
	}

	// Token: 0x060030AC RID: 12460 RVA: 0x001242E0 File Offset: 0x001224E0
	public void RaiseHeight(float normX, float normZ, float height, float opacity)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.RaiseHeight(num, num2, height, opacity);
	}

	// Token: 0x060030AD RID: 12461 RVA: 0x00124308 File Offset: 0x00122508
	public void RaiseHeight(int x, int z, float height, float opacity)
	{
		float num = Mathf.Max(this.GetDstHeight01(x, z), Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity));
		this.SetHeight(x, z, num);
	}

	// Token: 0x060030AE RID: 12462 RVA: 0x0012433C File Offset: 0x0012253C
	public void SetHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		float num3 = TerrainMeta.NormalizeY(worldPos.y);
		this.SetHeight(num, num2, num3, opacity, radius, fade);
	}

	// Token: 0x060030AF RID: 12463 RVA: 0x0012437C File Offset: 0x0012257C
	public void SetHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.SetHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x060030B0 RID: 12464 RVA: 0x001243C0 File Offset: 0x001225C0
	public void LowerHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		float num3 = TerrainMeta.NormalizeY(worldPos.y);
		this.LowerHeight(num, num2, num3, opacity, radius, fade);
	}

	// Token: 0x060030B1 RID: 12465 RVA: 0x00124400 File Offset: 0x00122600
	public void LowerHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.LowerHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x060030B2 RID: 12466 RVA: 0x00124444 File Offset: 0x00122644
	public void RaiseHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		float num3 = TerrainMeta.NormalizeY(worldPos.y);
		this.RaiseHeight(num, num2, num3, opacity, radius, fade);
	}

	// Token: 0x060030B3 RID: 12467 RVA: 0x00124484 File Offset: 0x00122684
	public void RaiseHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.RaiseHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x060030B4 RID: 12468 RVA: 0x001244C8 File Offset: 0x001226C8
	public void AddHeight(Vector3 worldPos, float delta, float radius, float fade = 0f)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddHeight(num, num2, delta, radius, fade);
	}

	// Token: 0x060030B5 RID: 12469 RVA: 0x001244FC File Offset: 0x001226FC
	public void AddHeight(float normX, float normZ, float delta, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.AddHeight(x, z, lerp * delta);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x04002802 RID: 10242
	public Texture2D HeightTexture;

	// Token: 0x04002803 RID: 10243
	public Texture2D NormalTexture;

	// Token: 0x04002804 RID: 10244
	private float normY;
}
