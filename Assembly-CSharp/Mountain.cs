using System;
using UnityEngine;

// Token: 0x02000701 RID: 1793
public class Mountain : TerrainPlacement
{
	// Token: 0x0600328C RID: 12940 RVA: 0x00137C24 File Offset: 0x00135E24
	protected void OnDrawGizmosSelected()
	{
		Vector3 vector = Vector3.up * (0.5f * this.Fade);
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Gizmos.DrawCube(base.transform.position + vector, new Vector3(this.size.x, this.Fade, this.size.z));
		Gizmos.DrawWireCube(base.transform.position + vector, new Vector3(this.size.x, this.Fade, this.size.z));
	}

	// Token: 0x0600328D RID: 12941 RVA: 0x00137CD4 File Offset: 0x00135ED4
	protected override void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		Vector3 position = localToWorld.MultiplyPoint3x4(Vector3.zero);
		TextureData heightdata = new TextureData(this.heightmap.Get());
		Vector3 vector = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, -this.extents.z));
		Vector3 vector2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, -this.extents.z));
		Vector3 vector3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, this.extents.z));
		Vector3 vector4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, this.extents.z));
		TerrainMeta.HeightMap.ForEachParallel(vector, vector2, vector3, vector4, delegate(int x, int z)
		{
			float num = TerrainMeta.HeightMap.Coordinate(z);
			float num2 = TerrainMeta.HeightMap.Coordinate(x);
			Vector3 vector5 = new Vector3(TerrainMeta.DenormalizeX(num2), 0f, TerrainMeta.DenormalizeZ(num));
			Vector3 vector6 = worldToLocal.MultiplyPoint3x4(vector5) - this.offset;
			float num3 = position.y + this.offset.y + heightdata.GetInterpolatedHalf((vector6.x + this.extents.x) / this.size.x, (vector6.z + this.extents.z) / this.size.z) * this.size.y;
			float num4 = Mathf.InverseLerp(position.y, position.y + this.Fade, num3);
			if (num4 == 0f)
			{
				return;
			}
			float num5 = TerrainMeta.NormalizeY(num3);
			num5 = Mathx.SmoothMax(TerrainMeta.HeightMap.GetHeight01(x, z), num5, 0.1f);
			TerrainMeta.HeightMap.SetHeight(x, z, num5, num4);
		});
	}

	// Token: 0x0600328E RID: 12942 RVA: 0x00137E0C File Offset: 0x0013600C
	protected override void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
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
		Vector3 position = localToWorld.MultiplyPoint3x4(Vector3.zero);
		TextureData heightdata = new TextureData(this.heightmap.Get());
		TextureData splat0data = new TextureData(this.splatmap0.Get());
		TextureData splat1data = new TextureData(this.splatmap1.Get());
		Vector3 vector = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, -this.extents.z));
		Vector3 vector2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, -this.extents.z));
		Vector3 vector3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, this.extents.z));
		Vector3 vector4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, this.extents.z));
		TerrainMeta.SplatMap.ForEachParallel(vector, vector2, vector3, vector4, delegate(int x, int z)
		{
			float num = TerrainMeta.SplatMap.Coordinate(z);
			float num2 = TerrainMeta.SplatMap.Coordinate(x);
			Vector3 vector5 = new Vector3(TerrainMeta.DenormalizeX(num2), 0f, TerrainMeta.DenormalizeZ(num));
			Vector3 vector6 = worldToLocal.MultiplyPoint3x4(vector5) - this.offset;
			float num3 = position.y + this.offset.y + heightdata.GetInterpolatedHalf((vector6.x + this.extents.x) / this.size.x, (vector6.z + this.extents.z) / this.size.z) * this.size.y;
			float num4 = Mathf.InverseLerp(position.y, position.y + this.Fade, num3);
			if (num4 == 0f)
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
			TerrainMeta.SplatMap.SetSplatRaw(x, z, interpolatedVector, interpolatedVector2, num4);
		});
	}

	// Token: 0x0600328F RID: 12943 RVA: 0x000063A5 File Offset: 0x000045A5
	protected override void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
	}

	// Token: 0x06003290 RID: 12944 RVA: 0x00138020 File Offset: 0x00136220
	protected override void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		bool should0 = base.ShouldBiome(1);
		bool should1 = base.ShouldBiome(2);
		bool should2 = base.ShouldBiome(4);
		bool should3 = base.ShouldBiome(8);
		if (!should0 && !should1 && !should2 && !should3)
		{
			return;
		}
		Vector3 position = localToWorld.MultiplyPoint3x4(Vector3.zero);
		TextureData heightdata = new TextureData(this.heightmap.Get());
		TextureData biomedata = new TextureData(this.biomemap.Get());
		Vector3 vector = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, -this.extents.z));
		Vector3 vector2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, -this.extents.z));
		Vector3 vector3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, this.extents.z));
		Vector3 vector4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, this.extents.z));
		TerrainMeta.BiomeMap.ForEachParallel(vector, vector2, vector3, vector4, delegate(int x, int z)
		{
			float num = TerrainMeta.BiomeMap.Coordinate(z);
			float num2 = TerrainMeta.BiomeMap.Coordinate(x);
			Vector3 vector5 = new Vector3(TerrainMeta.DenormalizeX(num2), 0f, TerrainMeta.DenormalizeZ(num));
			Vector3 vector6 = worldToLocal.MultiplyPoint3x4(vector5) - this.offset;
			float num3 = position.y + this.offset.y + heightdata.GetInterpolatedHalf((vector6.x + this.extents.x) / this.size.x, (vector6.z + this.extents.z) / this.size.z) * this.size.y;
			float num4 = Mathf.InverseLerp(position.y, position.y + this.Fade, num3);
			if (num4 == 0f)
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
			TerrainMeta.BiomeMap.SetBiomeRaw(x, z, interpolatedVector, num4);
		});
	}

	// Token: 0x06003291 RID: 12945 RVA: 0x001381C0 File Offset: 0x001363C0
	protected override void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		TextureData topologydata = new TextureData(this.topologymap.Get());
		Vector3 vector = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, -this.extents.z));
		Vector3 vector2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, -this.extents.z));
		Vector3 vector3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.extents.x, 0f, this.extents.z));
		Vector3 vector4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.extents.x, 0f, this.extents.z));
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

	// Token: 0x06003292 RID: 12946 RVA: 0x000063A5 File Offset: 0x000045A5
	protected override void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
	}

	// Token: 0x0400295E RID: 10590
	public float Fade = 10f;
}
