using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009BA RID: 2490
	public static class GlobalMesh
	{
		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06003B3D RID: 15165 RVA: 0x0015E580 File Offset: 0x0015C780
		public static Mesh mesh
		{
			get
			{
				if (GlobalMesh.ms_Mesh == null)
				{
					GlobalMesh.ms_Mesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
					GlobalMesh.ms_Mesh.hideFlags = Consts.ProceduralObjectsHideFlags;
				}
				return GlobalMesh.ms_Mesh;
			}
		}

		// Token: 0x04003647 RID: 13895
		private static Mesh ms_Mesh;
	}
}
