using System;
using UnityEngine;

// Token: 0x020006A9 RID: 1705
public class TerrainPlacementMap : TerrainMap<bool>
{
	// Token: 0x060030D1 RID: 12497 RVA: 0x001254B0 File Offset: 0x001236B0
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.src = (this.dst = new bool[this.res * this.res]);
		this.Enable();
	}

	// Token: 0x060030D2 RID: 12498 RVA: 0x001254FA File Offset: 0x001236FA
	public override void PostSetup()
	{
		this.res = 0;
		this.src = null;
		this.Disable();
	}

	// Token: 0x060030D3 RID: 12499 RVA: 0x00125510 File Offset: 0x00123710
	public void Enable()
	{
		this.isEnabled = true;
	}

	// Token: 0x060030D4 RID: 12500 RVA: 0x00125519 File Offset: 0x00123719
	public void Disable()
	{
		this.isEnabled = false;
	}

	// Token: 0x060030D5 RID: 12501 RVA: 0x00125524 File Offset: 0x00123724
	public void Reset()
	{
		for (int i = 0; i < this.res; i++)
		{
			for (int j = 0; j < this.res; j++)
			{
				this.dst[i * this.res + j] = false;
			}
		}
	}

	// Token: 0x060030D6 RID: 12502 RVA: 0x00125568 File Offset: 0x00123768
	public bool GetBlocked(Vector3 worldPos)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBlocked(num, num2);
	}

	// Token: 0x060030D7 RID: 12503 RVA: 0x00125598 File Offset: 0x00123798
	public bool GetBlocked(float normX, float normZ)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		return this.GetBlocked(num, num2);
	}

	// Token: 0x060030D8 RID: 12504 RVA: 0x001255BD File Offset: 0x001237BD
	public bool GetBlocked(int x, int z)
	{
		return this.isEnabled && this.res > 0 && this.src[z * this.res + x];
	}

	// Token: 0x060030D9 RID: 12505 RVA: 0x001255E4 File Offset: 0x001237E4
	public void SetBlocked(Vector3 worldPos)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBlocked(num, num2);
	}

	// Token: 0x060030DA RID: 12506 RVA: 0x00125614 File Offset: 0x00123814
	public void SetBlocked(float normX, float normZ)
	{
		int num = base.Index(normX);
		int num2 = base.Index(normZ);
		this.SetBlocked(num, num2);
	}

	// Token: 0x060030DB RID: 12507 RVA: 0x00125639 File Offset: 0x00123839
	public void SetBlocked(int x, int z)
	{
		this.dst[z * this.res + x] = true;
	}

	// Token: 0x060030DC RID: 12508 RVA: 0x00125650 File Offset: 0x00123850
	public bool GetBlocked(Vector3 worldPos, float radius)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBlocked(num, num2, radius);
	}

	// Token: 0x060030DD RID: 12509 RVA: 0x00125680 File Offset: 0x00123880
	public bool GetBlocked(float normX, float normZ, float radius)
	{
		float num = TerrainMeta.OneOverSize.x * radius;
		int num2 = base.Index(normX - num);
		int num3 = base.Index(normX + num);
		int num4 = base.Index(normZ - num);
		int num5 = base.Index(normZ + num);
		for (int i = num4; i <= num5; i++)
		{
			for (int j = num2; j <= num3; j++)
			{
				if (this.src[i * this.res + j])
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060030DE RID: 12510 RVA: 0x001256F8 File Offset: 0x001238F8
	public void SetBlocked(Vector3 worldPos, float radius, float fade = 0f)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBlocked(num, num2, radius, fade);
	}

	// Token: 0x060030DF RID: 12511 RVA: 0x00125728 File Offset: 0x00123928
	public void SetBlocked(float normX, float normZ, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if ((double)lerp > 0.5)
			{
				this.dst[z * this.res + x] = true;
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x04002808 RID: 10248
	private bool isEnabled;
}
