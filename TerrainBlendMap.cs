using System;
using UnityEngine;

// Token: 0x020006A4 RID: 1700
public class TerrainBlendMap : TerrainMap<byte>
{
	// Token: 0x06003072 RID: 12402 RVA: 0x00122FA8 File Offset: 0x001211A8
	public override void Setup()
	{
		if (!(this.BlendTexture != null))
		{
			this.res = this.terrain.terrainData.alphamapResolution;
			this.src = (this.dst = new byte[this.res * this.res]);
			for (int i = 0; i < this.res; i++)
			{
				for (int j = 0; j < this.res; j++)
				{
					this.dst[i * this.res + j] = 0;
				}
			}
			return;
		}
		if (this.BlendTexture.width == this.BlendTexture.height)
		{
			this.res = this.BlendTexture.width;
			this.src = (this.dst = new byte[this.res * this.res]);
			Color32[] pixels = this.BlendTexture.GetPixels32();
			int k = 0;
			int num = 0;
			while (k < this.res)
			{
				int l = 0;
				while (l < this.res)
				{
					this.dst[k * this.res + l] = pixels[num].a;
					l++;
					num++;
				}
				k++;
			}
			return;
		}
		Debug.LogError("Invalid alpha texture: " + this.BlendTexture.name);
	}

	// Token: 0x06003073 RID: 12403 RVA: 0x001230FC File Offset: 0x001212FC
	public void GenerateTextures()
	{
		this.BlendTexture = new Texture2D(this.res, this.res, TextureFormat.Alpha8, true, true);
		this.BlendTexture.name = "BlendTexture";
		this.BlendTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte b = this.src[z * this.res + i];
				col[z * this.res + i] = new Color32(b, b, b, b);
			}
		});
		this.BlendTexture.SetPixels32(col);
	}

	// Token: 0x06003074 RID: 12404 RVA: 0x0012318D File Offset: 0x0012138D
	public void ApplyTextures()
	{
		this.BlendTexture.Apply(true, false);
		this.BlendTexture.Compress(false);
		this.BlendTexture.Apply(false, true);
	}

	// Token: 0x06003075 RID: 12405 RVA: 0x001231B8 File Offset: 0x001213B8
	public float GetAlpha(Vector3 worldPos)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetAlpha(num, num2);
	}

	// Token: 0x06003076 RID: 12406 RVA: 0x001231E8 File Offset: 0x001213E8
	public float GetAlpha(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int num6 = Mathf.Min(num4 + 1, num);
		int num7 = Mathf.Min(num5 + 1, num);
		float num8 = Mathf.Lerp(this.GetAlpha(num4, num5), this.GetAlpha(num6, num5), num2 - (float)num4);
		float num9 = Mathf.Lerp(this.GetAlpha(num4, num7), this.GetAlpha(num6, num7), num2 - (float)num4);
		return Mathf.Lerp(num8, num9, num3 - (float)num5);
	}

	// Token: 0x06003077 RID: 12407 RVA: 0x001225FA File Offset: 0x001207FA
	public float GetAlpha(int x, int z)
	{
		return BitUtility.Byte2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06003078 RID: 12408 RVA: 0x0012327C File Offset: 0x0012147C
	public void SetAlpha(Vector3 worldPos, float a)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(num, num2, a);
	}

	// Token: 0x06003079 RID: 12409 RVA: 0x001232AC File Offset: 0x001214AC
	public void SetAlpha(float normX, float normZ, float a)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.SetAlpha(num, num2, a);
	}

	// Token: 0x0600307A RID: 12410 RVA: 0x0012266A File Offset: 0x0012086A
	public void SetAlpha(int x, int z, float a)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Byte(a);
	}

	// Token: 0x0600307B RID: 12411 RVA: 0x001232D2 File Offset: 0x001214D2
	public void SetAlpha(int x, int z, float a, float opacity)
	{
		this.SetAlpha(x, z, Mathf.Lerp(this.GetAlpha(x, z), a, opacity));
	}

	// Token: 0x0600307C RID: 12412 RVA: 0x001232EC File Offset: 0x001214EC
	public void SetAlpha(Vector3 worldPos, float a, float opacity, float radius, float fade = 0f)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(num, num2, a, opacity, radius, fade);
	}

	// Token: 0x0600307D RID: 12413 RVA: 0x00123320 File Offset: 0x00121520
	public void SetAlpha(float normX, float normZ, float a, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			lerp *= opacity;
			if (lerp > 0f)
			{
				this.SetAlpha(x, z, a, lerp);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x04002800 RID: 10240
	public Texture2D BlendTexture;
}
