using System;
using UnityEngine;

// Token: 0x02000702 RID: 1794
public abstract class TerrainPlacement : PrefabAttribute
{
	// Token: 0x06003294 RID: 12948 RVA: 0x001382F8 File Offset: 0x001364F8
	public void Apply(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.ShouldHeight())
		{
			this.ApplyHeight(localToWorld, worldToLocal);
		}
		if (this.ShouldSplat(-1))
		{
			this.ApplySplat(localToWorld, worldToLocal);
		}
		if (this.ShouldAlpha())
		{
			this.ApplyAlpha(localToWorld, worldToLocal);
		}
		if (this.ShouldBiome(-1))
		{
			this.ApplyBiome(localToWorld, worldToLocal);
		}
		if (this.ShouldTopology(-1))
		{
			this.ApplyTopology(localToWorld, worldToLocal);
		}
		if (this.ShouldWater())
		{
			this.ApplyWater(localToWorld, worldToLocal);
		}
	}

	// Token: 0x06003295 RID: 12949 RVA: 0x00138368 File Offset: 0x00136568
	protected bool ShouldHeight()
	{
		return this.heightmap.isValid && this.HeightMap;
	}

	// Token: 0x06003296 RID: 12950 RVA: 0x0013837F File Offset: 0x0013657F
	protected bool ShouldSplat(int id = -1)
	{
		return this.splatmap0.isValid && this.splatmap1.isValid && (this.SplatMask & (TerrainSplat.Enum)id) > (TerrainSplat.Enum)0;
	}

	// Token: 0x06003297 RID: 12951 RVA: 0x001383A8 File Offset: 0x001365A8
	protected bool ShouldAlpha()
	{
		return this.alphamap.isValid && this.AlphaMap;
	}

	// Token: 0x06003298 RID: 12952 RVA: 0x001383BF File Offset: 0x001365BF
	protected bool ShouldBiome(int id = -1)
	{
		return this.biomemap.isValid && (this.BiomeMask & (TerrainBiome.Enum)id) > (TerrainBiome.Enum)0;
	}

	// Token: 0x06003299 RID: 12953 RVA: 0x001383DB File Offset: 0x001365DB
	protected bool ShouldTopology(int id = -1)
	{
		return this.topologymap.isValid && (this.TopologyMask & (TerrainTopology.Enum)id) > (TerrainTopology.Enum)0;
	}

	// Token: 0x0600329A RID: 12954 RVA: 0x001383F7 File Offset: 0x001365F7
	protected bool ShouldWater()
	{
		return this.watermap.isValid && this.WaterMap;
	}

	// Token: 0x0600329B RID: 12955
	protected abstract void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x0600329C RID: 12956
	protected abstract void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x0600329D RID: 12957
	protected abstract void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x0600329E RID: 12958
	protected abstract void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x0600329F RID: 12959
	protected abstract void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x060032A0 RID: 12960
	protected abstract void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x060032A1 RID: 12961 RVA: 0x0013840E File Offset: 0x0013660E
	protected override Type GetIndexedType()
	{
		return typeof(TerrainPlacement);
	}

	// Token: 0x0400295F RID: 10591
	[ReadOnly]
	public Vector3 size = Vector3.zero;

	// Token: 0x04002960 RID: 10592
	[ReadOnly]
	public Vector3 extents = Vector3.zero;

	// Token: 0x04002961 RID: 10593
	[ReadOnly]
	public Vector3 offset = Vector3.zero;

	// Token: 0x04002962 RID: 10594
	public bool HeightMap = true;

	// Token: 0x04002963 RID: 10595
	public bool AlphaMap = true;

	// Token: 0x04002964 RID: 10596
	public bool WaterMap;

	// Token: 0x04002965 RID: 10597
	[InspectorFlags]
	public TerrainSplat.Enum SplatMask;

	// Token: 0x04002966 RID: 10598
	[InspectorFlags]
	public TerrainBiome.Enum BiomeMask;

	// Token: 0x04002967 RID: 10599
	[InspectorFlags]
	public TerrainTopology.Enum TopologyMask;

	// Token: 0x04002968 RID: 10600
	[HideInInspector]
	public Texture2DRef heightmap;

	// Token: 0x04002969 RID: 10601
	[HideInInspector]
	public Texture2DRef splatmap0;

	// Token: 0x0400296A RID: 10602
	[HideInInspector]
	public Texture2DRef splatmap1;

	// Token: 0x0400296B RID: 10603
	[HideInInspector]
	public Texture2DRef alphamap;

	// Token: 0x0400296C RID: 10604
	[HideInInspector]
	public Texture2DRef biomemap;

	// Token: 0x0400296D RID: 10605
	[HideInInspector]
	public Texture2DRef topologymap;

	// Token: 0x0400296E RID: 10606
	[HideInInspector]
	public Texture2DRef watermap;

	// Token: 0x0400296F RID: 10607
	[HideInInspector]
	public Texture2DRef blendmap;
}
