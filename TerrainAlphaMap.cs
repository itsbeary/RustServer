using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006A2 RID: 1698
public class TerrainAlphaMap : TerrainMap<byte>
{
	// Token: 0x0600304D RID: 12365 RVA: 0x00122348 File Offset: 0x00120548
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.src = (this.dst = new byte[this.res * this.res]);
		for (int i = 0; i < this.res; i++)
		{
			for (int j = 0; j < this.res; j++)
			{
				this.dst[i * this.res + j] = byte.MaxValue;
			}
		}
		if (this.AlphaTexture != null)
		{
			if (this.AlphaTexture.width == this.AlphaTexture.height && this.AlphaTexture.width == this.res)
			{
				Color32[] pixels = this.AlphaTexture.GetPixels32();
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
			Debug.LogError("Invalid alpha texture: " + this.AlphaTexture.name);
		}
	}

	// Token: 0x0600304E RID: 12366 RVA: 0x0012247C File Offset: 0x0012067C
	public void GenerateTextures()
	{
		this.AlphaTexture = new Texture2D(this.res, this.res, TextureFormat.Alpha8, false, true);
		this.AlphaTexture.name = "AlphaTexture";
		this.AlphaTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte b = this.src[z * this.res + i];
				col[z * this.res + i] = new Color32(b, b, b, b);
			}
		});
		this.AlphaTexture.SetPixels32(col);
	}

	// Token: 0x0600304F RID: 12367 RVA: 0x0012250D File Offset: 0x0012070D
	public void ApplyTextures()
	{
		this.AlphaTexture.Apply(true, false);
		this.AlphaTexture.Compress(false);
		this.AlphaTexture.Apply(false, true);
	}

	// Token: 0x06003050 RID: 12368 RVA: 0x00122538 File Offset: 0x00120738
	public float GetAlpha(Vector3 worldPos)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetAlpha(num, num2);
	}

	// Token: 0x06003051 RID: 12369 RVA: 0x00122568 File Offset: 0x00120768
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

	// Token: 0x06003052 RID: 12370 RVA: 0x001225FA File Offset: 0x001207FA
	public float GetAlpha(int x, int z)
	{
		return BitUtility.Byte2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06003053 RID: 12371 RVA: 0x00122614 File Offset: 0x00120814
	public void SetAlpha(Vector3 worldPos, float a)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(num, num2, a);
	}

	// Token: 0x06003054 RID: 12372 RVA: 0x00122644 File Offset: 0x00120844
	public void SetAlpha(float normX, float normZ, float a)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.SetAlpha(num, num2, a);
	}

	// Token: 0x06003055 RID: 12373 RVA: 0x0012266A File Offset: 0x0012086A
	public void SetAlpha(int x, int z, float a)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Byte(a);
	}

	// Token: 0x06003056 RID: 12374 RVA: 0x00122683 File Offset: 0x00120883
	public void SetAlpha(int x, int z, float a, float opacity)
	{
		this.SetAlpha(x, z, Mathf.Lerp(this.GetAlpha(x, z), a, opacity));
	}

	// Token: 0x06003057 RID: 12375 RVA: 0x001226A0 File Offset: 0x001208A0
	public void SetAlpha(Vector3 worldPos, float a, float opacity, float radius, float fade = 0f)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(num, num2, a, opacity, radius, fade);
	}

	// Token: 0x06003058 RID: 12376 RVA: 0x001226D4 File Offset: 0x001208D4
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

	// Token: 0x040027FD RID: 10237
	[FormerlySerializedAs("ColorTexture")]
	public Texture2D AlphaTexture;
}
