using System;
using UnityEngine;

// Token: 0x02000700 RID: 1792
public class Monument : TerrainPlacement
{
	// Token: 0x06003284 RID: 12932 RVA: 0x0013749C File Offset: 0x0013569C
	protected void OnDrawGizmosSelected()
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		GizmosUtil.DrawWireCircleY(base.transform.position, this.Radius);
		GizmosUtil.DrawWireCircleY(base.transform.position, this.Radius - this.Fade);
	}

	// Token: 0x06003285 RID: 12933 RVA: 0x00137518 File Offset: 0x00135718
	protected override void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		bool useBlendMap = this.blendmap.isValid;
		Vector3 position = localToWorld.MultiplyPoint3x4(Vector3.zero);
		TextureData heightdata = new TextureData(this.heightmap.Get());
		TextureData blenddata = new TextureData(useBlendMap ? this.blendmap.Get() : null);
		float num = (useBlendMap ? this.extents.x : this.Radius);
		float num2 = (useBlendMap ? this.extents.z : this.Radius);
		Vector3 vector = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-num, 0f, -num2));
		Vector3 vector2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(num, 0f, -num2));
		Vector3 vector3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-num, 0f, num2));
		Vector3 vector4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(num, 0f, num2));
		TerrainMeta.HeightMap.ForEachParallel(vector, vector2, vector3, vector4, delegate(int x, int z)
		{
			float num3 = TerrainMeta.HeightMap.Coordinate(z);
			float num4 = TerrainMeta.HeightMap.Coordinate(x);
			Vector3 vector5 = new Vector3(TerrainMeta.DenormalizeX(num4), 0f, TerrainMeta.DenormalizeZ(num3));
			Vector3 vector6 = worldToLocal.MultiplyPoint3x4(vector5) - this.offset;
			float num5;
			if (useBlendMap)
			{
				num5 = blenddata.GetInterpolatedVector((vector6.x + this.extents.x) / this.size.x, (vector6.z + this.extents.z) / this.size.z).w;
			}
			else
			{
				num5 = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade, vector6.Magnitude2D());
			}
			if (num5 == 0f)
			{
				return;
			}
			float num6 = TerrainMeta.NormalizeY(position.y + this.offset.y + heightdata.GetInterpolatedHalf((vector6.x + this.extents.x) / this.size.x, (vector6.z + this.extents.z) / this.size.z) * this.size.y);
			num6 = Mathf.SmoothStep(TerrainMeta.HeightMap.GetHeight01(x, z), num6, num5);
			TerrainMeta.HeightMap.SetHeight(x, z, num6);
		});
	}

	// Token: 0x06003286 RID: 12934 RVA: 0x0013768C File Offset: 0x0013588C
	protected override void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		bool should0 = base.ShouldSplat(1);
		bool should1 = base.ShouldSplat(2);
		bool should2 = base.ShouldSplat(4);
		bool should3 = base.ShouldSplat(8);
		bool should4 = base.ShouldSplat(16);
		bool should5 = base.ShouldSplat(32);
		bool should6 = base.ShouldSplat(64);
		bool should7 = base.ShouldSplat(128);
		if (!should0 && !should1 && !should2 && !should3 && !should4 && !should5 && !should6 && !should7)
		{
			return;
		}
		TextureData splat0data = new TextureData(this.splatmap0.Get());
		TextureData splat1data = new TextureData(this.splatmap1.Get());
		Vector3 vector = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 vector2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 vector3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 vector4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.SplatMap.ForEachParallel(vector, vector2, vector3, vector4, delegate(int x, int z)
		{
			GenerateCliffSplat.Process(x, z);
			float num = TerrainMeta.SplatMap.Coordinate(z);
			float num2 = TerrainMeta.SplatMap.Coordinate(x);
			Vector3 vector5 = new Vector3(TerrainMeta.DenormalizeX(num2), 0f, TerrainMeta.DenormalizeZ(num));
			Vector3 vector6 = worldToLocal.MultiplyPoint3x4(vector5) - this.offset;
			float num3 = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade, vector6.Magnitude2D());
			if (num3 == 0f)
			{
				return;
			}
			Vector4 interpolatedVector = splat0data.GetInterpolatedVector((vector6.x + this.extents.x) / this.size.x, (vector6.z + this.extents.z) / this.size.z);
			Vector4 interpolatedVector2 = splat1data.GetInterpolatedVector((vector6.x + this.extents.x) / this.size.x, (vector6.z + this.extents.z) / this.size.z);
			if (!should0)
			{
				interpolatedVector.x = 0f;
			}
			if (!should1)
			{
				interpolatedVector.y = 0f;
			}
			if (!should2)
			{
				interpolatedVector.z = 0f;
			}
			if (!should3)
			{
				interpolatedVector.w = 0f;
			}
			if (!should4)
			{
				interpolatedVector2.x = 0f;
			}
			if (!should5)
			{
				interpolatedVector2.y = 0f;
			}
			if (!should6)
			{
				interpolatedVector2.z = 0f;
			}
			if (!should7)
			{
				interpolatedVector2.w = 0f;
			}
			TerrainMeta.SplatMap.SetSplatRaw(x, z, interpolatedVector, interpolatedVector2, num3);
		});
	}

	// Token: 0x06003287 RID: 12935 RVA: 0x0013786C File Offset: 0x00135A6C
	protected override void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		TextureData alphadata = new TextureData(this.alphamap.Get());
		Vector3 vector = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 vector2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 vector3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 vector4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.AlphaMap.ForEachParallel(vector, vector2, vector3, vector4, delegate(int x, int z)
		{
			float num = TerrainMeta.AlphaMap.Coordinate(z);
			float num2 = TerrainMeta.AlphaMap.Coordinate(x);
			Vector3 vector5 = new Vector3(TerrainMeta.DenormalizeX(num2), 0f, TerrainMeta.DenormalizeZ(num));
			Vector3 vector6 = worldToLocal.MultiplyPoint3x4(vector5) - this.offset;
			float num3 = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade, vector6.Magnitude2D());
			if (num3 == 0f)
			{
				return;
			}
			float w = alphadata.GetInterpolatedVector((vector6.x + this.extents.x) / this.size.x, (vector6.z + this.extents.z) / this.size.z).w;
			TerrainMeta.AlphaMap.SetAlpha(x, z, w, num3);
		});
	}

	// Token: 0x06003288 RID: 12936 RVA: 0x00137988 File Offset: 0x00135B88
	protected override void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		bool should0 = base.ShouldBiome(1);
		bool should1 = base.ShouldBiome(2);
		bool should2 = base.ShouldBiome(4);
		bool should3 = base.ShouldBiome(8);
		if (!should0 && !should1 && !should2 && !should3)
		{
			return;
		}
		TextureData biomedata = new TextureData(this.biomemap.Get());
		Vector3 vector = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 vector2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 vector3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 vector4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.BiomeMap.ForEachParallel(vector, vector2, vector3, vector4, delegate(int x, int z)
		{
			float num = TerrainMeta.BiomeMap.Coordinate(z);
			float num2 = TerrainMeta.BiomeMap.Coordinate(x);
			Vector3 vector5 = new Vector3(TerrainMeta.DenormalizeX(num2), 0f, TerrainMeta.DenormalizeZ(num));
			Vector3 vector6 = worldToLocal.MultiplyPoint3x4(vector5) - this.offset;
			float num3 = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade, vector6.Magnitude2D());
			if (num3 == 0f)
			{
				return;
			}
			Vector4 interpolatedVector = biomedata.GetInterpolatedVector((vector6.x + this.extents.x) / this.size.x, (vector6.z + this.extents.z) / this.size.z);
			if (!should0)
			{
				interpolatedVector.x = 0f;
			}
			if (!should1)
			{
				interpolatedVector.y = 0f;
			}
			if (!should2)
			{
				interpolatedVector.z = 0f;
			}
			if (!should3)
			{
				interpolatedVector.w = 0f;
			}
			TerrainMeta.BiomeMap.SetBiomeRaw(x, z, interpolatedVector, num3);
		});
	}

	// Token: 0x06003289 RID: 12937 RVA: 0x00137AF8 File Offset: 0x00135CF8
	protected override void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		TextureData topologydata = new TextureData(this.topologymap.Get());
		Vector3 vector = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 vector2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 vector3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 vector4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.TopologyMap.ForEachParallel(vector, vector2, vector3, vector4, delegate(int x, int z)
		{
			GenerateCliffTopology.Process(x, z);
			float num = TerrainMeta.TopologyMap.Coordinate(z);
			float num2 = TerrainMeta.TopologyMap.Coordinate(x);
			Vector3 vector5 = new Vector3(TerrainMeta.DenormalizeX(num2), 0f, TerrainMeta.DenormalizeZ(num));
			Vector3 vector6 = worldToLocal.MultiplyPoint3x4(vector5) - this.offset;
			int interpolatedInt = topologydata.GetInterpolatedInt((vector6.x + this.extents.x) / this.size.x, (vector6.z + this.extents.z) / this.size.z);
			if (this.ShouldTopology(interpolatedInt))
			{
				TerrainMeta.TopologyMap.AddTopology(x, z, interpolatedInt & (int)this.TopologyMask);
			}
		});
	}

	// Token: 0x0600328A RID: 12938 RVA: 0x000063A5 File Offset: 0x000045A5
	protected override void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
	}

	// Token: 0x0400295C RID: 10588
	public float Radius;

	// Token: 0x0400295D RID: 10589
	public float Fade = 10f;
}
