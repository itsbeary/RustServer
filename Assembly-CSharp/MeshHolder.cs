using System;
using UnityEngine;

// Token: 0x020009A8 RID: 2472
[Serializable]
public class MeshHolder
{
	// Token: 0x06003ADA RID: 15066 RVA: 0x0015C27A File Offset: 0x0015A47A
	public void setAnimationData(Mesh mesh)
	{
		this._colors = mesh.colors;
	}

	// Token: 0x0400359F RID: 13727
	[HideInInspector]
	public Vector3[] _vertices;

	// Token: 0x040035A0 RID: 13728
	[HideInInspector]
	public Vector3[] _normals;

	// Token: 0x040035A1 RID: 13729
	[HideInInspector]
	public int[] _triangles;

	// Token: 0x040035A2 RID: 13730
	[HideInInspector]
	public trisPerSubmesh[] _TrianglesOfSubs;

	// Token: 0x040035A3 RID: 13731
	[HideInInspector]
	public Matrix4x4[] _bindPoses;

	// Token: 0x040035A4 RID: 13732
	[HideInInspector]
	public BoneWeight[] _boneWeights;

	// Token: 0x040035A5 RID: 13733
	[HideInInspector]
	public Bounds _bounds;

	// Token: 0x040035A6 RID: 13734
	[HideInInspector]
	public int _subMeshCount;

	// Token: 0x040035A7 RID: 13735
	[HideInInspector]
	public Vector4[] _tangents;

	// Token: 0x040035A8 RID: 13736
	[HideInInspector]
	public Vector2[] _uv;

	// Token: 0x040035A9 RID: 13737
	[HideInInspector]
	public Vector2[] _uv2;

	// Token: 0x040035AA RID: 13738
	[HideInInspector]
	public Vector2[] _uv3;

	// Token: 0x040035AB RID: 13739
	[HideInInspector]
	public Color[] _colors;

	// Token: 0x040035AC RID: 13740
	[HideInInspector]
	public Vector2[] _uv4;
}
