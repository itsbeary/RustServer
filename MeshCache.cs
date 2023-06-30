using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002AC RID: 684
public static class MeshCache
{
	// Token: 0x06001D80 RID: 7552 RVA: 0x000CAE20 File Offset: 0x000C9020
	public static MeshCache.Data Get(Mesh mesh)
	{
		MeshCache.Data data;
		if (!MeshCache.dictionary.TryGetValue(mesh, out data))
		{
			data = new MeshCache.Data();
			data.mesh = mesh;
			data.vertices = mesh.vertices;
			data.normals = mesh.normals;
			data.tangents = mesh.tangents;
			data.colors32 = mesh.colors32;
			data.triangles = mesh.triangles;
			data.uv = mesh.uv;
			data.uv2 = mesh.uv2;
			data.uv3 = mesh.uv3;
			data.uv4 = mesh.uv4;
			data.bounds = mesh.bounds;
			MeshCache.dictionary.Add(mesh, data);
		}
		return data;
	}

	// Token: 0x04001647 RID: 5703
	public static Dictionary<Mesh, MeshCache.Data> dictionary = new Dictionary<Mesh, MeshCache.Data>();

	// Token: 0x02000CAA RID: 3242
	[Serializable]
	public class Data
	{
		// Token: 0x040044CC RID: 17612
		public Mesh mesh;

		// Token: 0x040044CD RID: 17613
		public Vector3[] vertices;

		// Token: 0x040044CE RID: 17614
		public Vector3[] normals;

		// Token: 0x040044CF RID: 17615
		public Vector4[] tangents;

		// Token: 0x040044D0 RID: 17616
		public Color32[] colors32;

		// Token: 0x040044D1 RID: 17617
		public int[] triangles;

		// Token: 0x040044D2 RID: 17618
		public Vector2[] uv;

		// Token: 0x040044D3 RID: 17619
		public Vector2[] uv2;

		// Token: 0x040044D4 RID: 17620
		public Vector2[] uv3;

		// Token: 0x040044D5 RID: 17621
		public Vector2[] uv4;

		// Token: 0x040044D6 RID: 17622
		public Bounds bounds;
	}
}
