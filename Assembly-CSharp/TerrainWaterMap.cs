using System;
using UnityEngine;

// Token: 0x020006AC RID: 1708
public class TerrainWaterMap : TerrainMap<short>
{
	// Token: 0x06003118 RID: 12568 RVA: 0x00126B4C File Offset: 0x00124D4C
	public override void Setup()
	{
		this.res = this.terrain.terrainData.heightmapResolution;
		this.src = (this.dst = new short[this.res * this.res]);
		this.normY = TerrainMeta.Size.x / TerrainMeta.Size.y / (float)this.res;
		if (this.WaterTexture != null)
		{
			if (this.WaterTexture.width == this.WaterTexture.height && this.WaterTexture.width == this.res)
			{
				Color32[] pixels = this.WaterTexture.GetPixels32();
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
			Debug.LogError("Invalid water texture: " + this.WaterTexture.name);
		}
	}

	// Token: 0x06003119 RID: 12569 RVA: 0x00126C64 File Offset: 0x00124E64
	public void GenerateTextures()
	{
		Color32[] heights = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				heights[z * this.res + i] = BitUtility.EncodeShort(this.src[z * this.res + i]);
			}
		});
		this.WaterTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
		this.WaterTexture.name = "WaterTexture";
		this.WaterTexture.wrapMode = TextureWrapMode.Clamp;
		this.WaterTexture.SetPixels32(heights);
	}

	// Token: 0x0600311A RID: 12570 RVA: 0x00126CF5 File Offset: 0x00124EF5
	public void ApplyTextures()
	{
		this.WaterTexture.Apply(true, true);
	}

	// Token: 0x0600311B RID: 12571 RVA: 0x00126D04 File Offset: 0x00124F04
	public float GetHeight(Vector3 worldPos)
	{
		return Math.Max(TerrainMeta.Position.y + this.GetHeight01(worldPos) * TerrainMeta.Size.y, WaterSystem.OceanLevel);
	}

	// Token: 0x0600311C RID: 12572 RVA: 0x00126D2D File Offset: 0x00124F2D
	public float GetHeight(float normX, float normZ)
	{
		return Math.Max(TerrainMeta.Position.y + this.GetHeight01(normX, normZ) * TerrainMeta.Size.y, WaterSystem.OceanLevel);
	}

	// Token: 0x0600311D RID: 12573 RVA: 0x00126D58 File Offset: 0x00124F58
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
		return Math.Max(TerrainMeta.Position.y + num19 * TerrainMeta.Size.y, WaterSystem.OceanLevel);
	}

	// Token: 0x0600311E RID: 12574 RVA: 0x00126E7B File Offset: 0x0012507B
	public float GetHeight(int x, int z)
	{
		return Math.Max(TerrainMeta.Position.y + this.GetHeight01(x, z) * TerrainMeta.Size.y, WaterSystem.OceanLevel);
	}

	// Token: 0x0600311F RID: 12575 RVA: 0x00126EA8 File Offset: 0x001250A8
	public float GetHeight01(Vector3 worldPos)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetHeight01(num, num2);
	}

	// Token: 0x06003120 RID: 12576 RVA: 0x00126ED8 File Offset: 0x001250D8
	public float GetHeight01(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int num6 = Mathf.Min(num4 + 1, num);
		int num7 = Mathf.Min(num5 + 1, num);
		float num8 = Mathf.Lerp(this.GetHeight01(num4, num5), this.GetHeight01(num6, num5), num2 - (float)num4);
		float num9 = Mathf.Lerp(this.GetHeight01(num4, num7), this.GetHeight01(num6, num7), num2 - (float)num4);
		return Mathf.Lerp(num8, num9, num3 - (float)num5);
	}

	// Token: 0x06003121 RID: 12577 RVA: 0x00123CFF File Offset: 0x00121EFF
	public float GetHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06003122 RID: 12578 RVA: 0x00126F6C File Offset: 0x0012516C
	public Vector3 GetNormal(Vector3 worldPos)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetNormal(num, num2);
	}

	// Token: 0x06003123 RID: 12579 RVA: 0x00126F9C File Offset: 0x0012519C
	public Vector3 GetNormal(float normX, float normZ)
	{
		int num = this.res - 1;
		int num2 = (int)(normX * (float)num);
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp(num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int num6 = Mathf.Min(num4 + 1, num);
		int num7 = Mathf.Min(num5 + 1, num);
		float num8 = this.GetHeight01(num6, num5) - this.GetHeight01(num4, num5);
		float num9 = this.GetHeight01(num4, num7) - this.GetHeight01(num4, num5);
		return new Vector3(-num8, this.normY, -num9).normalized;
	}

	// Token: 0x06003124 RID: 12580 RVA: 0x00127024 File Offset: 0x00125224
	public Vector3 GetNormalFast(Vector2 uv)
	{
		int num = this.res - 1;
		int num2 = (int)(uv.x * (float)num);
		int num3 = (int)(uv.y * (float)num);
		num2 = ((num2 >= 0) ? num2 : 0);
		num3 = ((num3 >= 0) ? num3 : 0);
		num2 = ((num2 <= num) ? num2 : num);
		num3 = ((num3 <= num) ? num3 : num);
		int num4 = ((num2 < num) ? 1 : 0);
		int num5 = ((num3 < num) ? this.res : 0);
		int num6 = num3 * this.res + num2;
		int num7 = num6 + num4;
		int num8 = num6 + num5;
		short num9 = this.src[num6];
		float num10 = (float)this.src[num7];
		short num11 = this.src[num8];
		float num12 = (num10 - (float)num9) * 3.051944E-05f;
		float num13 = (float)(num11 - num9) * 3.051944E-05f;
		return new Vector3(-num12, this.normY, -num13);
	}

	// Token: 0x06003125 RID: 12581 RVA: 0x001270EC File Offset: 0x001252EC
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

	// Token: 0x06003126 RID: 12582 RVA: 0x00127178 File Offset: 0x00125378
	public float GetSlope(Vector3 worldPos)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(worldPos));
	}

	// Token: 0x06003127 RID: 12583 RVA: 0x0012718B File Offset: 0x0012538B
	public float GetSlope(float normX, float normZ)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(normX, normZ));
	}

	// Token: 0x06003128 RID: 12584 RVA: 0x0012719F File Offset: 0x0012539F
	public float GetSlope(int x, int z)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(x, z));
	}

	// Token: 0x06003129 RID: 12585 RVA: 0x001271B3 File Offset: 0x001253B3
	public float GetSlope01(Vector3 worldPos)
	{
		return this.GetSlope(worldPos) * 0.011111111f;
	}

	// Token: 0x0600312A RID: 12586 RVA: 0x001271C2 File Offset: 0x001253C2
	public float GetSlope01(float normX, float normZ)
	{
		return this.GetSlope(normX, normZ) * 0.011111111f;
	}

	// Token: 0x0600312B RID: 12587 RVA: 0x001271D2 File Offset: 0x001253D2
	public float GetSlope01(int x, int z)
	{
		return this.GetSlope(x, z) * 0.011111111f;
	}

	// Token: 0x0600312C RID: 12588 RVA: 0x001271E2 File Offset: 0x001253E2
	public float GetDepth(Vector3 worldPos)
	{
		return this.GetHeight(worldPos) - TerrainMeta.HeightMap.GetHeight(worldPos);
	}

	// Token: 0x0600312D RID: 12589 RVA: 0x001271F7 File Offset: 0x001253F7
	public float GetDepth(float normX, float normZ)
	{
		return this.GetHeight(normX, normZ) - TerrainMeta.HeightMap.GetHeight(normX, normZ);
	}

	// Token: 0x0600312E RID: 12590 RVA: 0x00127210 File Offset: 0x00125410
	public void SetHeight(Vector3 worldPos, float height)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetHeight(num, num2, height);
	}

	// Token: 0x0600312F RID: 12591 RVA: 0x00127240 File Offset: 0x00125440
	public void SetHeight(float normX, float normZ, float height)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.SetHeight(num, num2, height);
	}

	// Token: 0x06003130 RID: 12592 RVA: 0x0012410A File Offset: 0x0012230A
	public void SetHeight(int x, int z, float height)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Short(height);
	}

	// Token: 0x0400280D RID: 10253
	public Texture2D WaterTexture;

	// Token: 0x0400280E RID: 10254
	private float normY;
}
