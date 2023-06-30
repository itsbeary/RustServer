using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020002AD RID: 685
public class MeshColliderData
{
	// Token: 0x06001D82 RID: 7554 RVA: 0x000CAEDD File Offset: 0x000C90DD
	public void Alloc()
	{
		if (this.triangles == null)
		{
			this.triangles = Facepunch.Pool.GetList<int>();
		}
		if (this.vertices == null)
		{
			this.vertices = Facepunch.Pool.GetList<Vector3>();
		}
		if (this.normals == null)
		{
			this.normals = Facepunch.Pool.GetList<Vector3>();
		}
	}

	// Token: 0x06001D83 RID: 7555 RVA: 0x000CAF18 File Offset: 0x000C9118
	public void Free()
	{
		if (this.triangles != null)
		{
			Facepunch.Pool.FreeList<int>(ref this.triangles);
		}
		if (this.vertices != null)
		{
			Facepunch.Pool.FreeList<Vector3>(ref this.vertices);
		}
		if (this.normals != null)
		{
			Facepunch.Pool.FreeList<Vector3>(ref this.normals);
		}
	}

	// Token: 0x06001D84 RID: 7556 RVA: 0x000CAF53 File Offset: 0x000C9153
	public void Clear()
	{
		if (this.triangles != null)
		{
			this.triangles.Clear();
		}
		if (this.vertices != null)
		{
			this.vertices.Clear();
		}
		if (this.normals != null)
		{
			this.normals.Clear();
		}
	}

	// Token: 0x06001D85 RID: 7557 RVA: 0x000CAF90 File Offset: 0x000C9190
	public void Apply(UnityEngine.Mesh mesh)
	{
		mesh.Clear();
		if (this.vertices != null)
		{
			mesh.SetVertices(this.vertices);
		}
		if (this.triangles != null)
		{
			mesh.SetTriangles(this.triangles, 0);
		}
		if (this.normals != null)
		{
			if (this.normals.Count == this.vertices.Count)
			{
				mesh.SetNormals(this.normals);
				return;
			}
			if (this.normals.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping collider normals because some meshes were missing them.");
			}
		}
	}

	// Token: 0x06001D86 RID: 7558 RVA: 0x000CB01C File Offset: 0x000C921C
	public void Combine(MeshColliderGroup meshGroup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshColliderInstance meshColliderInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshColliderInstance.position, meshColliderInstance.rotation, meshColliderInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshColliderInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshColliderInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshColliderInstance.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x.MultiplyPoint3x4(meshColliderInstance.data.vertices[k]));
			}
			for (int l = 0; l < meshColliderInstance.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x.MultiplyVector(meshColliderInstance.data.normals[l]));
			}
		}
	}

	// Token: 0x06001D87 RID: 7559 RVA: 0x000CB130 File Offset: 0x000C9330
	public void Combine(MeshColliderGroup meshGroup, MeshColliderLookup colliderLookup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshColliderInstance meshColliderInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshColliderInstance.position, meshColliderInstance.rotation, meshColliderInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshColliderInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshColliderInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshColliderInstance.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x.MultiplyPoint3x4(meshColliderInstance.data.vertices[k]));
			}
			for (int l = 0; l < meshColliderInstance.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x.MultiplyVector(meshColliderInstance.data.normals[l]));
			}
			colliderLookup.Add(meshColliderInstance);
		}
	}

	// Token: 0x04001648 RID: 5704
	public List<int> triangles;

	// Token: 0x04001649 RID: 5705
	public List<Vector3> vertices;

	// Token: 0x0400164A RID: 5706
	public List<Vector3> normals;
}
