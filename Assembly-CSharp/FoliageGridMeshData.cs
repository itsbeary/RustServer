using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000939 RID: 2361
public class FoliageGridMeshData
{
	// Token: 0x06003888 RID: 14472 RVA: 0x00150687 File Offset: 0x0014E887
	public void Alloc()
	{
		if (this.triangles == null)
		{
			this.triangles = Pool.GetList<int>();
		}
		if (this.vertices == null)
		{
			this.vertices = Pool.GetList<FoliageGridMeshData.FoliageVertex>();
		}
	}

	// Token: 0x06003889 RID: 14473 RVA: 0x001506AF File Offset: 0x0014E8AF
	public void Free()
	{
		if (this.triangles != null)
		{
			Pool.FreeList<int>(ref this.triangles);
		}
		if (this.vertices != null)
		{
			Pool.FreeList<FoliageGridMeshData.FoliageVertex>(ref this.vertices);
		}
	}

	// Token: 0x0600388A RID: 14474 RVA: 0x001506D7 File Offset: 0x0014E8D7
	public void Clear()
	{
		List<int> list = this.triangles;
		if (list != null)
		{
			list.Clear();
		}
		List<FoliageGridMeshData.FoliageVertex> list2 = this.vertices;
		if (list2 == null)
		{
			return;
		}
		list2.Clear();
	}

	// Token: 0x0600388B RID: 14475 RVA: 0x001506FC File Offset: 0x0014E8FC
	public void Combine(MeshGroup meshGroup)
	{
		if (meshGroup.data.Count == 0)
		{
			return;
		}
		this.bounds = new Bounds(meshGroup.data[0].position, Vector3.zero);
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshInstance meshInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshInstance.position, meshInstance.rotation, meshInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshInstance.data.vertices.Length; k++)
			{
				Vector4 vector = meshInstance.data.tangents[k];
				Vector3 vector2 = new Vector3(vector.x, vector.y, vector.z);
				Vector3 vector3 = matrix4x.MultiplyVector(vector2);
				FoliageGridMeshData.FoliageVertex foliageVertex = default(FoliageGridMeshData.FoliageVertex);
				foliageVertex.position = matrix4x.MultiplyPoint3x4(meshInstance.data.vertices[k]);
				foliageVertex.normal = matrix4x.MultiplyVector(meshInstance.data.normals[k]);
				foliageVertex.uv = meshInstance.data.uv[k];
				foliageVertex.uv2 = meshInstance.position;
				foliageVertex.tangent = new Vector4(vector3.x, vector3.y, vector3.z, vector.w);
				if (meshInstance.data.colors32.Length != 0)
				{
					foliageVertex.color = meshInstance.data.colors32[k];
				}
				this.vertices.Add(foliageVertex);
			}
			this.bounds.Encapsulate(new Bounds(meshInstance.position + meshInstance.data.bounds.center, meshInstance.data.bounds.size));
		}
		this.bounds.size = this.bounds.size + Vector3.one;
	}

	// Token: 0x0600388C RID: 14476 RVA: 0x00150940 File Offset: 0x0014EB40
	public void Apply(Mesh mesh)
	{
		mesh.SetVertexBufferParams(this.vertices.Count, FoliageGridMeshData.FoliageVertex.VertexLayout);
		mesh.SetVertexBufferData<FoliageGridMeshData.FoliageVertex>(this.vertices, 0, 0, this.vertices.Count, 0, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontRecalculateBounds);
		mesh.SetIndices(this.triangles, MeshTopology.Triangles, 0, false, 0);
		mesh.bounds = this.bounds;
	}

	// Token: 0x040033A6 RID: 13222
	public List<FoliageGridMeshData.FoliageVertex> vertices;

	// Token: 0x040033A7 RID: 13223
	public List<int> triangles;

	// Token: 0x040033A8 RID: 13224
	public Bounds bounds;

	// Token: 0x02000ECF RID: 3791
	public struct FoliageVertex
	{
		// Token: 0x04004D67 RID: 19815
		public Vector3 position;

		// Token: 0x04004D68 RID: 19816
		public Vector3 normal;

		// Token: 0x04004D69 RID: 19817
		public Vector4 tangent;

		// Token: 0x04004D6A RID: 19818
		public Color32 color;

		// Token: 0x04004D6B RID: 19819
		public Vector2 uv;

		// Token: 0x04004D6C RID: 19820
		public Vector4 uv2;

		// Token: 0x04004D6D RID: 19821
		public static readonly VertexAttributeDescriptor[] VertexLayout = new VertexAttributeDescriptor[]
		{
			new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 0),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord2, VertexAttributeFormat.Float32, 4, 0)
		};
	}
}
