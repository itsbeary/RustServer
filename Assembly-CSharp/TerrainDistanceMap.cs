using System;
using UnityEngine;

// Token: 0x020006A5 RID: 1701
public class TerrainDistanceMap : TerrainMap<byte>
{
	// Token: 0x0600307F RID: 12415 RVA: 0x00123364 File Offset: 0x00121564
	public override void Setup()
	{
		this.res = this.terrain.terrainData.heightmapResolution;
		this.src = (this.dst = new byte[4 * this.res * this.res]);
		if (this.DistanceTexture != null)
		{
			if (this.DistanceTexture.width == this.DistanceTexture.height && this.DistanceTexture.width == this.res)
			{
				Color32[] pixels = this.DistanceTexture.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						this.SetDistance(j, i, BitUtility.DecodeVector2i(pixels[num]));
						j++;
						num++;
					}
					i++;
				}
				return;
			}
			Debug.LogError("Invalid distance texture: " + this.DistanceTexture.name, this.DistanceTexture);
		}
	}

	// Token: 0x06003080 RID: 12416 RVA: 0x00123454 File Offset: 0x00121654
	public void GenerateTextures()
	{
		this.DistanceTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
		this.DistanceTexture.name = "DistanceTexture";
		this.DistanceTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] cols = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				cols[z * this.res + i] = BitUtility.EncodeVector2i(this.GetDistance(i, z));
			}
		});
		this.DistanceTexture.SetPixels32(cols);
	}

	// Token: 0x06003081 RID: 12417 RVA: 0x001234E5 File Offset: 0x001216E5
	public void ApplyTextures()
	{
		this.DistanceTexture.Apply(true, true);
	}

	// Token: 0x06003082 RID: 12418 RVA: 0x001234F4 File Offset: 0x001216F4
	public Vector2i GetDistance(Vector3 worldPos)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetDistance(num, num2);
	}

	// Token: 0x06003083 RID: 12419 RVA: 0x00123524 File Offset: 0x00121724
	public Vector2i GetDistance(float normX, float normZ)
	{
		int num = this.res - 1;
		int num2 = Mathf.Clamp(Mathf.RoundToInt(normX * (float)num), 0, num);
		int num3 = Mathf.Clamp(Mathf.RoundToInt(normZ * (float)num), 0, num);
		return this.GetDistance(num2, num3);
	}

	// Token: 0x06003084 RID: 12420 RVA: 0x00123564 File Offset: 0x00121764
	public Vector2i GetDistance(int x, int z)
	{
		byte[] src = this.src;
		int res = this.res;
		byte b = src[(0 + z) * this.res + x];
		byte b2 = this.src[(this.res + z) * this.res + x];
		byte b3 = this.src[(2 * this.res + z) * this.res + x];
		byte b4 = this.src[(3 * this.res + z) * this.res + x];
		if (b == 255 && b2 == 255 && b3 == 255 && b4 == 255)
		{
			return new Vector2i(256, 256);
		}
		return new Vector2i((int)(b - b2), (int)(b3 - b4));
	}

	// Token: 0x06003085 RID: 12421 RVA: 0x00123618 File Offset: 0x00121818
	public void SetDistance(int x, int z, Vector2i v)
	{
		byte[] dst = this.dst;
		int res = this.res;
		dst[(0 + z) * this.res + x] = (byte)Mathf.Clamp(v.x, 0, 255);
		this.dst[(this.res + z) * this.res + x] = (byte)Mathf.Clamp(-v.x, 0, 255);
		this.dst[(2 * this.res + z) * this.res + x] = (byte)Mathf.Clamp(v.y, 0, 255);
		this.dst[(3 * this.res + z) * this.res + x] = (byte)Mathf.Clamp(-v.y, 0, 255);
	}

	// Token: 0x04002801 RID: 10241
	public Texture2D DistanceTexture;
}
