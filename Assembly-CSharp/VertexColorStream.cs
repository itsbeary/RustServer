using System;
using UnityEngine;

// Token: 0x020009AB RID: 2475
[ExecuteInEditMode]
public class VertexColorStream : MonoBehaviour
{
	// Token: 0x06003AE5 RID: 15077 RVA: 0x000063A5 File Offset: 0x000045A5
	private void OnDidApplyAnimationProperties()
	{
	}

	// Token: 0x06003AE6 RID: 15078 RVA: 0x0015C608 File Offset: 0x0015A808
	public void init(Mesh origMesh, bool destroyOld)
	{
		this.originalMesh = origMesh;
		this.paintedMesh = UnityEngine.Object.Instantiate<Mesh>(origMesh);
		if (destroyOld)
		{
			UnityEngine.Object.DestroyImmediate(origMesh);
		}
		this.paintedMesh.hideFlags = HideFlags.None;
		this.paintedMesh.name = "vpp_" + base.gameObject.name;
		this.meshHold = new MeshHolder();
		this.meshHold._vertices = this.paintedMesh.vertices;
		this.meshHold._normals = this.paintedMesh.normals;
		this.meshHold._triangles = this.paintedMesh.triangles;
		this.meshHold._TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.subMeshCount];
		for (int i = 0; i < this.paintedMesh.subMeshCount; i++)
		{
			this.meshHold._TrianglesOfSubs[i] = new trisPerSubmesh();
			this.meshHold._TrianglesOfSubs[i].triangles = this.paintedMesh.GetTriangles(i);
		}
		this.meshHold._bindPoses = this.paintedMesh.bindposes;
		this.meshHold._boneWeights = this.paintedMesh.boneWeights;
		this.meshHold._bounds = this.paintedMesh.bounds;
		this.meshHold._subMeshCount = this.paintedMesh.subMeshCount;
		this.meshHold._tangents = this.paintedMesh.tangents;
		this.meshHold._uv = this.paintedMesh.uv;
		this.meshHold._uv2 = this.paintedMesh.uv2;
		this.meshHold._uv3 = this.paintedMesh.uv3;
		this.meshHold._colors = this.paintedMesh.colors;
		this.meshHold._uv4 = this.paintedMesh.uv4;
		base.GetComponent<MeshFilter>().sharedMesh = this.paintedMesh;
		if (base.GetComponent<MeshCollider>())
		{
			base.GetComponent<MeshCollider>().sharedMesh = this.paintedMesh;
		}
	}

	// Token: 0x06003AE7 RID: 15079 RVA: 0x0015C818 File Offset: 0x0015AA18
	public void setWholeMesh(Mesh tmpMesh)
	{
		this.paintedMesh.vertices = tmpMesh.vertices;
		this.paintedMesh.triangles = tmpMesh.triangles;
		this.paintedMesh.normals = tmpMesh.normals;
		this.paintedMesh.colors = tmpMesh.colors;
		this.paintedMesh.uv = tmpMesh.uv;
		this.paintedMesh.uv2 = tmpMesh.uv2;
		this.paintedMesh.uv3 = tmpMesh.uv3;
		this.meshHold._vertices = tmpMesh.vertices;
		this.meshHold._triangles = tmpMesh.triangles;
		this.meshHold._normals = tmpMesh.normals;
		this.meshHold._colors = tmpMesh.colors;
		this.meshHold._uv = tmpMesh.uv;
		this.meshHold._uv2 = tmpMesh.uv2;
		this.meshHold._uv3 = tmpMesh.uv3;
	}

	// Token: 0x06003AE8 RID: 15080 RVA: 0x0015C914 File Offset: 0x0015AB14
	public Vector3[] setVertices(Vector3[] _deformedVertices)
	{
		this.paintedMesh.vertices = _deformedVertices;
		this.meshHold._vertices = _deformedVertices;
		this.paintedMesh.RecalculateNormals();
		this.paintedMesh.RecalculateBounds();
		this.meshHold._normals = this.paintedMesh.normals;
		this.meshHold._bounds = this.paintedMesh.bounds;
		base.GetComponent<MeshCollider>().sharedMesh = null;
		if (base.GetComponent<MeshCollider>())
		{
			base.GetComponent<MeshCollider>().sharedMesh = this.paintedMesh;
		}
		return this.meshHold._normals;
	}

	// Token: 0x06003AE9 RID: 15081 RVA: 0x0015C9B0 File Offset: 0x0015ABB0
	public Vector3[] getVertices()
	{
		return this.paintedMesh.vertices;
	}

	// Token: 0x06003AEA RID: 15082 RVA: 0x0015C9BD File Offset: 0x0015ABBD
	public Vector3[] getNormals()
	{
		return this.paintedMesh.normals;
	}

	// Token: 0x06003AEB RID: 15083 RVA: 0x0015C9CA File Offset: 0x0015ABCA
	public int[] getTriangles()
	{
		return this.paintedMesh.triangles;
	}

	// Token: 0x06003AEC RID: 15084 RVA: 0x0015C9D7 File Offset: 0x0015ABD7
	public void setTangents(Vector4[] _meshTangents)
	{
		this.paintedMesh.tangents = _meshTangents;
		this.meshHold._tangents = _meshTangents;
	}

	// Token: 0x06003AED RID: 15085 RVA: 0x0015C9F1 File Offset: 0x0015ABF1
	public Vector4[] getTangents()
	{
		return this.paintedMesh.tangents;
	}

	// Token: 0x06003AEE RID: 15086 RVA: 0x0015C9FE File Offset: 0x0015ABFE
	public void setColors(Color[] _vertexColors)
	{
		this.paintedMesh.colors = _vertexColors;
		this.meshHold._colors = _vertexColors;
	}

	// Token: 0x06003AEF RID: 15087 RVA: 0x0015CA18 File Offset: 0x0015AC18
	public Color[] getColors()
	{
		return this.paintedMesh.colors;
	}

	// Token: 0x06003AF0 RID: 15088 RVA: 0x0015CA25 File Offset: 0x0015AC25
	public Vector2[] getUVs()
	{
		return this.paintedMesh.uv;
	}

	// Token: 0x06003AF1 RID: 15089 RVA: 0x0015CA32 File Offset: 0x0015AC32
	public void setUV4s(Vector2[] _uv4s)
	{
		this.paintedMesh.uv4 = _uv4s;
		this.meshHold._uv4 = _uv4s;
	}

	// Token: 0x06003AF2 RID: 15090 RVA: 0x0015CA4C File Offset: 0x0015AC4C
	public Vector2[] getUV4s()
	{
		return this.paintedMesh.uv4;
	}

	// Token: 0x06003AF3 RID: 15091 RVA: 0x0015CA59 File Offset: 0x0015AC59
	public void unlink()
	{
		this.init(this.paintedMesh, false);
	}

	// Token: 0x06003AF4 RID: 15092 RVA: 0x0015CA68 File Offset: 0x0015AC68
	public void rebuild()
	{
		if (!base.GetComponent<MeshFilter>())
		{
			return;
		}
		this.paintedMesh = new Mesh();
		this.paintedMesh.hideFlags = HideFlags.HideAndDontSave;
		this.paintedMesh.name = "vpp_" + base.gameObject.name;
		if (this.meshHold == null || this.meshHold._vertices.Length == 0 || this.meshHold._TrianglesOfSubs.Length == 0)
		{
			this.paintedMesh.subMeshCount = this._subMeshCount;
			this.paintedMesh.vertices = this._vertices;
			this.paintedMesh.normals = this._normals;
			this.paintedMesh.triangles = this._triangles;
			this.meshHold._TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.subMeshCount];
			for (int i = 0; i < this.paintedMesh.subMeshCount; i++)
			{
				this.meshHold._TrianglesOfSubs[i] = new trisPerSubmesh();
				this.meshHold._TrianglesOfSubs[i].triangles = this.paintedMesh.GetTriangles(i);
			}
			this.paintedMesh.bindposes = this._bindPoses;
			this.paintedMesh.boneWeights = this._boneWeights;
			this.paintedMesh.bounds = this._bounds;
			this.paintedMesh.tangents = this._tangents;
			this.paintedMesh.uv = this._uv;
			this.paintedMesh.uv2 = this._uv2;
			this.paintedMesh.uv3 = this._uv3;
			this.paintedMesh.colors = this._colors;
			this.paintedMesh.uv4 = this._uv4;
			this.init(this.paintedMesh, true);
			return;
		}
		this.paintedMesh.subMeshCount = this.meshHold._subMeshCount;
		this.paintedMesh.vertices = this.meshHold._vertices;
		this.paintedMesh.normals = this.meshHold._normals;
		for (int j = 0; j < this.meshHold._subMeshCount; j++)
		{
			this.paintedMesh.SetTriangles(this.meshHold._TrianglesOfSubs[j].triangles, j);
		}
		this.paintedMesh.bindposes = this.meshHold._bindPoses;
		this.paintedMesh.boneWeights = this.meshHold._boneWeights;
		this.paintedMesh.bounds = this.meshHold._bounds;
		this.paintedMesh.tangents = this.meshHold._tangents;
		this.paintedMesh.uv = this.meshHold._uv;
		this.paintedMesh.uv2 = this.meshHold._uv2;
		this.paintedMesh.uv3 = this.meshHold._uv3;
		this.paintedMesh.colors = this.meshHold._colors;
		this.paintedMesh.uv4 = this.meshHold._uv4;
		this.init(this.paintedMesh, true);
	}

	// Token: 0x06003AF5 RID: 15093 RVA: 0x0015CD77 File Offset: 0x0015AF77
	private void Start()
	{
		if (!this.paintedMesh || this.meshHold == null)
		{
			this.rebuild();
		}
	}

	// Token: 0x040035B3 RID: 13747
	[HideInInspector]
	public Mesh originalMesh;

	// Token: 0x040035B4 RID: 13748
	[HideInInspector]
	public Mesh paintedMesh;

	// Token: 0x040035B5 RID: 13749
	[HideInInspector]
	public MeshHolder meshHold;

	// Token: 0x040035B6 RID: 13750
	[HideInInspector]
	public Vector3[] _vertices;

	// Token: 0x040035B7 RID: 13751
	[HideInInspector]
	public Vector3[] _normals;

	// Token: 0x040035B8 RID: 13752
	[HideInInspector]
	public int[] _triangles;

	// Token: 0x040035B9 RID: 13753
	[HideInInspector]
	public int[][] _Subtriangles;

	// Token: 0x040035BA RID: 13754
	[HideInInspector]
	public Matrix4x4[] _bindPoses;

	// Token: 0x040035BB RID: 13755
	[HideInInspector]
	public BoneWeight[] _boneWeights;

	// Token: 0x040035BC RID: 13756
	[HideInInspector]
	public Bounds _bounds;

	// Token: 0x040035BD RID: 13757
	[HideInInspector]
	public int _subMeshCount;

	// Token: 0x040035BE RID: 13758
	[HideInInspector]
	public Vector4[] _tangents;

	// Token: 0x040035BF RID: 13759
	[HideInInspector]
	public Vector2[] _uv;

	// Token: 0x040035C0 RID: 13760
	[HideInInspector]
	public Vector2[] _uv2;

	// Token: 0x040035C1 RID: 13761
	[HideInInspector]
	public Vector2[] _uv3;

	// Token: 0x040035C2 RID: 13762
	[HideInInspector]
	public Color[] _colors;

	// Token: 0x040035C3 RID: 13763
	[HideInInspector]
	public Vector2[] _uv4;
}
