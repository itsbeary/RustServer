using System;
using UnityEngine;

// Token: 0x020006AB RID: 1707
public class TerrainTopologyMap : TerrainMap<int>
{
	// Token: 0x060030FE RID: 12542 RVA: 0x00126588 File Offset: 0x00124788
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.src = (this.dst = new int[this.res * this.res]);
		if (this.TopologyTexture != null)
		{
			if (this.TopologyTexture.width == this.TopologyTexture.height && this.TopologyTexture.width == this.res)
			{
				Color32[] pixels = this.TopologyTexture.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						this.dst[i * this.res + j] = BitUtility.DecodeInt(pixels[num]);
						j++;
						num++;
					}
					i++;
				}
				return;
			}
			Debug.LogError("Invalid topology texture: " + this.TopologyTexture.name);
		}
	}

	// Token: 0x060030FF RID: 12543 RVA: 0x0012667C File Offset: 0x0012487C
	public void GenerateTextures()
	{
		this.TopologyTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, false, true);
		this.TopologyTexture.name = "TopologyTexture";
		this.TopologyTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				col[z * this.res + i] = BitUtility.EncodeInt(this.src[z * this.res + i]);
			}
		});
		this.TopologyTexture.SetPixels32(col);
	}

	// Token: 0x06003100 RID: 12544 RVA: 0x0012670D File Offset: 0x0012490D
	public void ApplyTextures()
	{
		this.TopologyTexture.Apply(false, true);
	}

	// Token: 0x06003101 RID: 12545 RVA: 0x0012671C File Offset: 0x0012491C
	public bool GetTopology(Vector3 worldPos, int mask)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(num, num2, mask);
	}

	// Token: 0x06003102 RID: 12546 RVA: 0x0012674C File Offset: 0x0012494C
	public bool GetTopology(float normX, float normZ, int mask)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		return this.GetTopology(num, num2, mask);
	}

	// Token: 0x06003103 RID: 12547 RVA: 0x00126772 File Offset: 0x00124972
	public bool GetTopology(int x, int z, int mask)
	{
		return (this.src[z * this.res + x] & mask) != 0;
	}

	// Token: 0x06003104 RID: 12548 RVA: 0x0012678C File Offset: 0x0012498C
	public int GetTopology(Vector3 worldPos)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(num, num2);
	}

	// Token: 0x06003105 RID: 12549 RVA: 0x001267BC File Offset: 0x001249BC
	public int GetTopology(float normX, float normZ)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		return this.GetTopology(num, num2);
	}

	// Token: 0x06003106 RID: 12550 RVA: 0x001267E4 File Offset: 0x001249E4
	public int GetTopologyFast(Vector2 uv)
	{
		int num = this.res - 1;
		int num2 = (int)(uv.x * (float)this.res);
		int num3 = (int)(uv.y * (float)this.res);
		num2 = ((num2 >= 0) ? num2 : 0);
		num3 = ((num3 >= 0) ? num3 : 0);
		num2 = ((num2 <= num) ? num2 : num);
		num3 = ((num3 <= num) ? num3 : num);
		return this.src[num3 * this.res + num2];
	}

	// Token: 0x06003107 RID: 12551 RVA: 0x0012684F File Offset: 0x00124A4F
	public int GetTopology(int x, int z)
	{
		return this.src[z * this.res + x];
	}

	// Token: 0x06003108 RID: 12552 RVA: 0x00126864 File Offset: 0x00124A64
	public void SetTopology(Vector3 worldPos, int mask)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetTopology(num, num2, mask);
	}

	// Token: 0x06003109 RID: 12553 RVA: 0x00126894 File Offset: 0x00124A94
	public void SetTopology(float normX, float normZ, int mask)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.SetTopology(num, num2, mask);
	}

	// Token: 0x0600310A RID: 12554 RVA: 0x001268BA File Offset: 0x00124ABA
	public void SetTopology(int x, int z, int mask)
	{
		this.dst[z * this.res + x] = mask;
	}

	// Token: 0x0600310B RID: 12555 RVA: 0x001268D0 File Offset: 0x00124AD0
	public void AddTopology(Vector3 worldPos, int mask)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddTopology(num, num2, mask);
	}

	// Token: 0x0600310C RID: 12556 RVA: 0x00126900 File Offset: 0x00124B00
	public void AddTopology(float normX, float normZ, int mask)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.AddTopology(num, num2, mask);
	}

	// Token: 0x0600310D RID: 12557 RVA: 0x00126926 File Offset: 0x00124B26
	public void AddTopology(int x, int z, int mask)
	{
		this.dst[z * this.res + x] |= mask;
	}

	// Token: 0x0600310E RID: 12558 RVA: 0x00126944 File Offset: 0x00124B44
	public void RemoveTopology(Vector3 worldPos, int mask)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.RemoveTopology(num, num2, mask);
	}

	// Token: 0x0600310F RID: 12559 RVA: 0x00126974 File Offset: 0x00124B74
	public void RemoveTopology(float normX, float normZ, int mask)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.RemoveTopology(num, num2, mask);
	}

	// Token: 0x06003110 RID: 12560 RVA: 0x0012699A File Offset: 0x00124B9A
	public void RemoveTopology(int x, int z, int mask)
	{
		this.dst[z * this.res + x] &= ~mask;
	}

	// Token: 0x06003111 RID: 12561 RVA: 0x001269B8 File Offset: 0x00124BB8
	public int GetTopology(Vector3 worldPos, float radius)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(num, num2, radius);
	}

	// Token: 0x06003112 RID: 12562 RVA: 0x001269E8 File Offset: 0x00124BE8
	public int GetTopology(float normX, float normZ, float radius)
	{
		int num = 0;
		float num2 = TerrainMeta.OneOverSize.x * radius;
		int num3 = base.Index(normX - num2);
		int num4 = base.Index(normX + num2);
		int num5 = base.Index(normZ - num2);
		int num6 = base.Index(normZ + num2);
		for (int i = num5; i <= num6; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				num |= this.src[i * this.res + j];
			}
		}
		return num;
	}

	// Token: 0x06003113 RID: 12563 RVA: 0x00126A64 File Offset: 0x00124C64
	public void SetTopology(Vector3 worldPos, int mask, float radius, float fade = 0f)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetTopology(num, num2, mask, radius, fade);
	}

	// Token: 0x06003114 RID: 12564 RVA: 0x00126A98 File Offset: 0x00124C98
	public void SetTopology(float normX, float normZ, int mask, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if ((double)lerp > 0.5)
			{
				this.dst[z * this.res + x] = mask;
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06003115 RID: 12565 RVA: 0x00126AD4 File Offset: 0x00124CD4
	public void AddTopology(Vector3 worldPos, int mask, float radius, float fade = 0f)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddTopology(num, num2, mask, radius, fade);
	}

	// Token: 0x06003116 RID: 12566 RVA: 0x00126B08 File Offset: 0x00124D08
	public void AddTopology(float normX, float normZ, int mask, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if ((double)lerp > 0.5)
			{
				this.dst[z * this.res + x] |= mask;
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x0400280C RID: 10252
	public Texture2D TopologyTexture;
}
