using System;
using UnityEngine;

// Token: 0x020006AA RID: 1706
public class TerrainSplatMap : TerrainMap<byte>
{
	// Token: 0x060030E2 RID: 12514 RVA: 0x00125778 File Offset: 0x00123978
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.num = this.config.Splats.Length;
		this.src = (this.dst = new byte[this.num * this.res * this.res]);
		if (this.SplatTexture0 != null)
		{
			if (this.SplatTexture0.width == this.SplatTexture0.height && this.SplatTexture0.width == this.res)
			{
				Color32[] pixels = this.SplatTexture0.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						Color32 color = pixels[num];
						if (this.num > 0)
						{
							byte[] dst = this.dst;
							int res = this.res;
							dst[(0 + i) * this.res + j] = color.r;
						}
						if (this.num > 1)
						{
							this.dst[(this.res + i) * this.res + j] = color.g;
						}
						if (this.num > 2)
						{
							this.dst[(2 * this.res + i) * this.res + j] = color.b;
						}
						if (this.num > 3)
						{
							this.dst[(3 * this.res + i) * this.res + j] = color.a;
						}
						j++;
						num++;
					}
					i++;
				}
			}
			else
			{
				Debug.LogError("Invalid splat texture: " + this.SplatTexture0.name, this.SplatTexture0);
			}
		}
		if (this.SplatTexture1 != null)
		{
			if (this.SplatTexture1.width == this.SplatTexture1.height && this.SplatTexture1.width == this.res && this.num > 5)
			{
				Color32[] pixels2 = this.SplatTexture1.GetPixels32();
				int k = 0;
				int num2 = 0;
				while (k < this.res)
				{
					int l = 0;
					while (l < this.res)
					{
						Color32 color2 = pixels2[num2];
						if (this.num > 4)
						{
							this.dst[(4 * this.res + k) * this.res + l] = color2.r;
						}
						if (this.num > 5)
						{
							this.dst[(5 * this.res + k) * this.res + l] = color2.g;
						}
						if (this.num > 6)
						{
							this.dst[(6 * this.res + k) * this.res + l] = color2.b;
						}
						if (this.num > 7)
						{
							this.dst[(7 * this.res + k) * this.res + l] = color2.a;
						}
						l++;
						num2++;
					}
					k++;
				}
				return;
			}
			Debug.LogError("Invalid splat texture: " + this.SplatTexture1.name, this.SplatTexture1);
		}
	}

	// Token: 0x060030E3 RID: 12515 RVA: 0x00125AA8 File Offset: 0x00123CA8
	public void GenerateTextures()
	{
		this.SplatTexture0 = new Texture2D(this.res, this.res, TextureFormat.RGBA32, false, true);
		this.SplatTexture0.name = "SplatTexture0";
		this.SplatTexture0.wrapMode = TextureWrapMode.Clamp;
		Color32[] cols2 = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte b;
				if (this.num <= 0)
				{
					b = 0;
				}
				else
				{
					byte[] src = this.src;
					int res = this.res;
					b = src[(0 + z) * this.res + i];
				}
				byte b2 = b;
				byte b3 = ((this.num > 1) ? this.src[(this.res + z) * this.res + i] : 0);
				byte b4 = ((this.num > 2) ? this.src[(2 * this.res + z) * this.res + i] : 0);
				byte b5 = ((this.num > 3) ? this.src[(3 * this.res + z) * this.res + i] : 0);
				cols2[z * this.res + i] = new Color32(b2, b3, b4, b5);
			}
		});
		this.SplatTexture0.SetPixels32(cols2);
		this.SplatTexture1 = new Texture2D(this.res, this.res, TextureFormat.RGBA32, false, true);
		this.SplatTexture1.name = "SplatTexture1";
		this.SplatTexture1.wrapMode = TextureWrapMode.Clamp;
		Color32[] cols = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int j = 0; j < this.res; j++)
			{
				byte b6 = ((this.num > 4) ? this.src[(4 * this.res + z) * this.res + j] : 0);
				byte b7 = ((this.num > 5) ? this.src[(5 * this.res + z) * this.res + j] : 0);
				byte b8 = ((this.num > 6) ? this.src[(6 * this.res + z) * this.res + j] : 0);
				byte b9 = ((this.num > 7) ? this.src[(7 * this.res + z) * this.res + j] : 0);
				cols[z * this.res + j] = new Color32(b6, b7, b8, b9);
			}
		});
		this.SplatTexture1.SetPixels32(cols);
	}

	// Token: 0x060030E4 RID: 12516 RVA: 0x00125BBD File Offset: 0x00123DBD
	public void ApplyTextures()
	{
		this.SplatTexture0.Apply(true, true);
		this.SplatTexture1.Apply(true, true);
	}

	// Token: 0x060030E5 RID: 12517 RVA: 0x00125BDC File Offset: 0x00123DDC
	public float GetSplatMax(Vector3 worldPos, int mask = -1)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetSplatMax(num, num2, mask);
	}

	// Token: 0x060030E6 RID: 12518 RVA: 0x00125C0C File Offset: 0x00123E0C
	public float GetSplatMax(float normX, float normZ, int mask = -1)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		return this.GetSplatMax(num, num2, mask);
	}

	// Token: 0x060030E7 RID: 12519 RVA: 0x00125C34 File Offset: 0x00123E34
	public float GetSplatMax(int x, int z, int mask = -1)
	{
		byte b = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
			{
				byte b2 = this.src[(i * this.res + z) * this.res + x];
				if (b2 >= b)
				{
					b = b2;
				}
			}
		}
		return BitUtility.Byte2Float((int)b);
	}

	// Token: 0x060030E8 RID: 12520 RVA: 0x00125C88 File Offset: 0x00123E88
	public int GetSplatMaxIndex(Vector3 worldPos, int mask = -1)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetSplatMaxIndex(num, num2, mask);
	}

	// Token: 0x060030E9 RID: 12521 RVA: 0x00125CB8 File Offset: 0x00123EB8
	public int GetSplatMaxIndex(float normX, float normZ, int mask = -1)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		return this.GetSplatMaxIndex(num, num2, mask);
	}

	// Token: 0x060030EA RID: 12522 RVA: 0x00125CE0 File Offset: 0x00123EE0
	public int GetSplatMaxIndex(int x, int z, int mask = -1)
	{
		byte b = 0;
		int num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
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

	// Token: 0x060030EB RID: 12523 RVA: 0x00125D30 File Offset: 0x00123F30
	public int GetSplatMaxType(Vector3 worldPos, int mask = -1)
	{
		return TerrainSplat.IndexToType(this.GetSplatMaxIndex(worldPos, mask));
	}

	// Token: 0x060030EC RID: 12524 RVA: 0x00125D3F File Offset: 0x00123F3F
	public int GetSplatMaxType(float normX, float normZ, int mask = -1)
	{
		return TerrainSplat.IndexToType(this.GetSplatMaxIndex(normX, normZ, mask));
	}

	// Token: 0x060030ED RID: 12525 RVA: 0x00125D4F File Offset: 0x00123F4F
	public int GetSplatMaxType(int x, int z, int mask = -1)
	{
		return TerrainSplat.IndexToType(this.GetSplatMaxIndex(x, z, mask));
	}

	// Token: 0x060030EE RID: 12526 RVA: 0x00125D60 File Offset: 0x00123F60
	public float GetSplat(Vector3 worldPos, int mask)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetSplat(num, num2, mask);
	}

	// Token: 0x060030EF RID: 12527 RVA: 0x00125D90 File Offset: 0x00123F90
	public float GetSplat(float normX, float normZ, int mask)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int num6 = Mathf.Min(num4 + 1, num);
		int num7 = Mathf.Min(num5 + 1, num);
		float num8 = Mathf.Lerp(this.GetSplat(num4, num5, mask), this.GetSplat(num6, num5, mask), num2 - (float)num4);
		float num9 = Mathf.Lerp(this.GetSplat(num4, num7, mask), this.GetSplat(num6, num7, mask), num2 - (float)num4);
		return Mathf.Lerp(num8, num9, num3 - (float)num5);
	}

	// Token: 0x060030F0 RID: 12528 RVA: 0x00125E28 File Offset: 0x00124028
	public float GetSplat(int x, int z, int mask)
	{
		if (Mathf.IsPowerOfTwo(mask))
		{
			return BitUtility.Byte2Float((int)this.src[(TerrainSplat.TypeToIndex(mask) * this.res + z) * this.res + x]);
		}
		int num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
			{
				num += (int)this.src[(i * this.res + z) * this.res + x];
			}
		}
		return Mathf.Clamp01(BitUtility.Byte2Float(num));
	}

	// Token: 0x060030F1 RID: 12529 RVA: 0x00125EA8 File Offset: 0x001240A8
	public void SetSplat(Vector3 worldPos, int id)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetSplat(num, num2, id);
	}

	// Token: 0x060030F2 RID: 12530 RVA: 0x00125ED8 File Offset: 0x001240D8
	public void SetSplat(float normX, float normZ, int id)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.SetSplat(num, num2, id);
	}

	// Token: 0x060030F3 RID: 12531 RVA: 0x00125F00 File Offset: 0x00124100
	public void SetSplat(int x, int z, int id)
	{
		int num = TerrainSplat.TypeToIndex(id);
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

	// Token: 0x060030F4 RID: 12532 RVA: 0x00125F68 File Offset: 0x00124168
	public void SetSplat(Vector3 worldPos, int id, float v)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetSplat(num, num2, id, v);
	}

	// Token: 0x060030F5 RID: 12533 RVA: 0x00125F98 File Offset: 0x00124198
	public void SetSplat(float normX, float normZ, int id, float v)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.SetSplat(num, num2, id, v);
	}

	// Token: 0x060030F6 RID: 12534 RVA: 0x00125FC0 File Offset: 0x001241C0
	public void SetSplat(int x, int z, int id, float v)
	{
		this.SetSplat(x, z, id, this.GetSplat(x, z, id), v);
	}

	// Token: 0x060030F7 RID: 12535 RVA: 0x00125FD8 File Offset: 0x001241D8
	public void SetSplatRaw(int x, int z, Vector4 v1, Vector4 v2, float opacity)
	{
		if (opacity == 0f)
		{
			return;
		}
		float num = Mathf.Clamp01(v1.x + v1.y + v1.z + v1.w + v2.x + v2.y + v2.z + v2.w);
		if (num == 0f)
		{
			return;
		}
		float num2 = 1f - opacity * num;
		if (num2 == 0f && opacity == 1f)
		{
			byte[] dst = this.dst;
			int res = this.res;
			dst[(0 + z) * this.res + x] = BitUtility.Float2Byte(v1.x);
			this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(v1.y);
			this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v1.z);
			this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v1.w);
			this.dst[(4 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.x);
			this.dst[(5 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.y);
			this.dst[(6 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.z);
			this.dst[(7 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.w);
			return;
		}
		byte[] dst2 = this.dst;
		int res2 = this.res;
		int num3 = (0 + z) * this.res + x;
		byte[] src = this.src;
		int res3 = this.res;
		dst2[num3] = BitUtility.Float2Byte(BitUtility.Byte2Float(src[(0 + z) * this.res + x]) * num2 + v1.x * opacity);
		this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(this.res + z) * this.res + x]) * num2 + v1.y * opacity);
		this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(2 * this.res + z) * this.res + x]) * num2 + v1.z * opacity);
		this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(3 * this.res + z) * this.res + x]) * num2 + v1.w * opacity);
		this.dst[(4 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(4 * this.res + z) * this.res + x]) * num2 + v2.x * opacity);
		this.dst[(5 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(5 * this.res + z) * this.res + x]) * num2 + v2.y * opacity);
		this.dst[(6 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(6 * this.res + z) * this.res + x]) * num2 + v2.z * opacity);
		this.dst[(7 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(7 * this.res + z) * this.res + x]) * num2 + v2.w * opacity);
	}

	// Token: 0x060030F8 RID: 12536 RVA: 0x001263D0 File Offset: 0x001245D0
	public void SetSplat(Vector3 worldPos, int id, float opacity, float radius, float fade = 0f)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetSplat(num, num2, id, opacity, radius, fade);
	}

	// Token: 0x060030F9 RID: 12537 RVA: 0x00126404 File Offset: 0x00124604
	public void SetSplat(float normX, float normZ, int id, float opacity, float radius, float fade = 0f)
	{
		int idx = TerrainSplat.TypeToIndex(id);
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				float num = BitUtility.Byte2Float((int)this.dst[(idx * this.res + z) * this.res + x]);
				float num2 = Mathf.Lerp(num, 1f, lerp * opacity);
				this.SetSplat(x, z, id, num, num2);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x060030FA RID: 12538 RVA: 0x00126458 File Offset: 0x00124658
	public void AddSplat(Vector3 worldPos, int id, float delta, float radius, float fade = 0f)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddSplat(num, num2, id, delta, radius, fade);
	}

	// Token: 0x060030FB RID: 12539 RVA: 0x0012648C File Offset: 0x0012468C
	public void AddSplat(float normX, float normZ, int id, float delta, float radius, float fade = 0f)
	{
		int idx = TerrainSplat.TypeToIndex(id);
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				float num = BitUtility.Byte2Float((int)this.dst[(idx * this.res + z) * this.res + x]);
				float num2 = Mathf.Clamp01(num + lerp * delta);
				this.SetSplat(x, z, id, num, num2);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x060030FC RID: 12540 RVA: 0x001264E0 File Offset: 0x001246E0
	private void SetSplat(int x, int z, int id, float old_val, float new_val)
	{
		int num = TerrainSplat.TypeToIndex(id);
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

	// Token: 0x04002809 RID: 10249
	public Texture2D SplatTexture0;

	// Token: 0x0400280A RID: 10250
	public Texture2D SplatTexture1;

	// Token: 0x0400280B RID: 10251
	internal int num;
}
