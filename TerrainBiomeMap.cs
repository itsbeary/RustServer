using System;
using UnityEngine;

// Token: 0x020006A3 RID: 1699
public class TerrainBiomeMap : TerrainMap<byte>
{
	// Token: 0x0600305A RID: 12378 RVA: 0x00122720 File Offset: 0x00120920
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.num = 4;
		this.src = (this.dst = new byte[this.num * this.res * this.res]);
		if (this.BiomeTexture != null)
		{
			if (this.BiomeTexture.width == this.BiomeTexture.height && this.BiomeTexture.width == this.res)
			{
				Color32[] pixels = this.BiomeTexture.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						Color32 color = pixels[num];
						byte[] dst = this.dst;
						int res = this.res;
						dst[(0 + i) * this.res + j] = color.r;
						this.dst[(this.res + i) * this.res + j] = color.g;
						this.dst[(2 * this.res + i) * this.res + j] = color.b;
						this.dst[(3 * this.res + i) * this.res + j] = color.a;
						j++;
						num++;
					}
					i++;
				}
				return;
			}
			Debug.LogError("Invalid biome texture: " + this.BiomeTexture.name);
		}
	}

	// Token: 0x0600305B RID: 12379 RVA: 0x001228A4 File Offset: 0x00120AA4
	public void GenerateTextures()
	{
		this.BiomeTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
		this.BiomeTexture.name = "BiomeTexture";
		this.BiomeTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte[] src = this.src;
				int res = this.res;
				byte b = src[(0 + z) * this.res + i];
				byte b2 = this.src[(this.res + z) * this.res + i];
				byte b3 = this.src[(2 * this.res + z) * this.res + i];
				byte b4 = this.src[(3 * this.res + z) * this.res + i];
				col[z * this.res + i] = new Color32(b, b2, b3, b4);
			}
		});
		this.BiomeTexture.SetPixels32(col);
	}

	// Token: 0x0600305C RID: 12380 RVA: 0x00122935 File Offset: 0x00120B35
	public void ApplyTextures()
	{
		this.BiomeTexture.Apply(true, false);
		this.BiomeTexture.Compress(false);
		this.BiomeTexture.Apply(false, true);
	}

	// Token: 0x0600305D RID: 12381 RVA: 0x00122960 File Offset: 0x00120B60
	public float GetBiomeMax(Vector3 worldPos, int mask = -1)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiomeMax(num, num2, mask);
	}

	// Token: 0x0600305E RID: 12382 RVA: 0x00122990 File Offset: 0x00120B90
	public float GetBiomeMax(float normX, float normZ, int mask = -1)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		return this.GetBiomeMax(num, num2, mask);
	}

	// Token: 0x0600305F RID: 12383 RVA: 0x001229B8 File Offset: 0x00120BB8
	public float GetBiomeMax(int x, int z, int mask = -1)
	{
		byte b = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				byte b2 = this.src[(i * this.res + z) * this.res + x];
				if (b2 >= b)
				{
					b = b2;
				}
			}
		}
		return (float)b;
	}

	// Token: 0x06003060 RID: 12384 RVA: 0x00122A08 File Offset: 0x00120C08
	public int GetBiomeMaxIndex(Vector3 worldPos, int mask = -1)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiomeMaxIndex(num, num2, mask);
	}

	// Token: 0x06003061 RID: 12385 RVA: 0x00122A38 File Offset: 0x00120C38
	public int GetBiomeMaxIndex(float normX, float normZ, int mask = -1)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		return this.GetBiomeMaxIndex(num, num2, mask);
	}

	// Token: 0x06003062 RID: 12386 RVA: 0x00122A60 File Offset: 0x00120C60
	public int GetBiomeMaxIndex(int x, int z, int mask = -1)
	{
		byte b = 0;
		int num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				byte b2 = this.src[(i * this.res + z) * this.res + x];
				if (b2 >= b)
				{
					b = b2;
					num = i;
				}
			}
		}
		return num;
	}

	// Token: 0x06003063 RID: 12387 RVA: 0x00122AB0 File Offset: 0x00120CB0
	public int GetBiomeMaxType(Vector3 worldPos, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(worldPos, mask));
	}

	// Token: 0x06003064 RID: 12388 RVA: 0x00122ABF File Offset: 0x00120CBF
	public int GetBiomeMaxType(float normX, float normZ, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(normX, normZ, mask));
	}

	// Token: 0x06003065 RID: 12389 RVA: 0x00122ACF File Offset: 0x00120CCF
	public int GetBiomeMaxType(int x, int z, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(x, z, mask));
	}

	// Token: 0x06003066 RID: 12390 RVA: 0x00122AE0 File Offset: 0x00120CE0
	public float GetBiome(Vector3 worldPos, int mask)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiome(num, num2, mask);
	}

	// Token: 0x06003067 RID: 12391 RVA: 0x00122B10 File Offset: 0x00120D10
	public float GetBiome(float normX, float normZ, int mask)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		return this.GetBiome(num, num2, mask);
	}

	// Token: 0x06003068 RID: 12392 RVA: 0x00122B38 File Offset: 0x00120D38
	public float GetBiome(int x, int z, int mask)
	{
		if (Mathf.IsPowerOfTwo(mask))
		{
			return BitUtility.Byte2Float((int)this.src[(TerrainBiome.TypeToIndex(mask) * this.res + z) * this.res + x]);
		}
		int num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				num += (int)this.src[(i * this.res + z) * this.res + x];
			}
		}
		return Mathf.Clamp01(BitUtility.Byte2Float(num));
	}

	// Token: 0x06003069 RID: 12393 RVA: 0x00122BB8 File Offset: 0x00120DB8
	public void SetBiome(Vector3 worldPos, int id)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBiome(num, num2, id);
	}

	// Token: 0x0600306A RID: 12394 RVA: 0x00122BE8 File Offset: 0x00120DE8
	public void SetBiome(float normX, float normZ, int id)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.SetBiome(num, num2, id);
	}

	// Token: 0x0600306B RID: 12395 RVA: 0x00122C10 File Offset: 0x00120E10
	public void SetBiome(int x, int z, int id)
	{
		int num = TerrainBiome.TypeToIndex(id);
		for (int i = 0; i < this.num; i++)
		{
			if (i == num)
			{
				this.dst[(i * this.res + z) * this.res + x] = byte.MaxValue;
			}
			else
			{
				this.dst[(i * this.res + z) * this.res + x] = 0;
			}
		}
	}

	// Token: 0x0600306C RID: 12396 RVA: 0x00122C78 File Offset: 0x00120E78
	public void SetBiome(Vector3 worldPos, int id, float v)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBiome(num, num2, id, v);
	}

	// Token: 0x0600306D RID: 12397 RVA: 0x00122CA8 File Offset: 0x00120EA8
	public void SetBiome(float normX, float normZ, int id, float v)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.SetBiome(num, num2, id, v);
	}

	// Token: 0x0600306E RID: 12398 RVA: 0x00122CD0 File Offset: 0x00120ED0
	public void SetBiome(int x, int z, int id, float v)
	{
		this.SetBiome(x, z, id, this.GetBiome(x, z, id), v);
	}

	// Token: 0x0600306F RID: 12399 RVA: 0x00122CE8 File Offset: 0x00120EE8
	public void SetBiomeRaw(int x, int z, Vector4 v, float opacity)
	{
		if (opacity == 0f)
		{
			return;
		}
		float num = Mathf.Clamp01(v.x + v.y + v.z + v.w);
		if (num == 0f)
		{
			return;
		}
		float num2 = 1f - opacity * num;
		if (num2 == 0f && opacity == 1f)
		{
			byte[] dst = this.dst;
			int res = this.res;
			dst[(0 + z) * this.res + x] = BitUtility.Float2Byte(v.x);
			this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(v.y);
			this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v.z);
			this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v.w);
			return;
		}
		byte[] dst2 = this.dst;
		int res2 = this.res;
		int num3 = (0 + z) * this.res + x;
		byte[] src = this.src;
		int res3 = this.res;
		dst2[num3] = BitUtility.Float2Byte(BitUtility.Byte2Float(src[(0 + z) * this.res + x]) * num2 + v.x * opacity);
		this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(this.res + z) * this.res + x]) * num2 + v.y * opacity);
		this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(2 * this.res + z) * this.res + x]) * num2 + v.z * opacity);
		this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(3 * this.res + z) * this.res + x]) * num2 + v.w * opacity);
	}

	// Token: 0x06003070 RID: 12400 RVA: 0x00122F00 File Offset: 0x00121100
	private void SetBiome(int x, int z, int id, float old_val, float new_val)
	{
		int num = TerrainBiome.TypeToIndex(id);
		if (old_val >= 1f)
		{
			return;
		}
		float num2 = (1f - new_val) / (1f - old_val);
		for (int i = 0; i < this.num; i++)
		{
			if (i == num)
			{
				this.dst[(i * this.res + z) * this.res + x] = BitUtility.Float2Byte(new_val);
			}
			else
			{
				this.dst[(i * this.res + z) * this.res + x] = BitUtility.Float2Byte(num2 * BitUtility.Byte2Float((int)this.dst[(i * this.res + z) * this.res + x]));
			}
		}
	}

	// Token: 0x040027FE RID: 10238
	public Texture2D BiomeTexture;

	// Token: 0x040027FF RID: 10239
	internal int num;
}
