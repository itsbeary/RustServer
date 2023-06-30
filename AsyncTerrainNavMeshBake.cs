using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000660 RID: 1632
public class AsyncTerrainNavMeshBake : CustomYieldInstruction
{
	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x06002F5E RID: 12126 RVA: 0x0011E6A4 File Offset: 0x0011C8A4
	public override bool keepWaiting
	{
		get
		{
			return this.worker != null;
		}
	}

	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x06002F5F RID: 12127 RVA: 0x0011E6AF File Offset: 0x0011C8AF
	public bool isDone
	{
		get
		{
			return this.worker == null;
		}
	}

	// Token: 0x06002F60 RID: 12128 RVA: 0x0011E6BC File Offset: 0x0011C8BC
	public NavMeshBuildSource CreateNavMeshBuildSource()
	{
		return new NavMeshBuildSource
		{
			transform = Matrix4x4.TRS(this.pivot, Quaternion.identity, Vector3.one),
			shape = NavMeshBuildSourceShape.Mesh,
			sourceObject = this.mesh
		};
	}

	// Token: 0x06002F61 RID: 12129 RVA: 0x0011E704 File Offset: 0x0011C904
	public NavMeshBuildSource CreateNavMeshBuildSource(int area)
	{
		NavMeshBuildSource navMeshBuildSource = this.CreateNavMeshBuildSource();
		navMeshBuildSource.area = area;
		return navMeshBuildSource;
	}

	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x06002F62 RID: 12130 RVA: 0x0011E724 File Offset: 0x0011C924
	public Mesh mesh
	{
		get
		{
			Mesh mesh = new Mesh();
			if (this.vertices != null)
			{
				mesh.SetVertices(this.vertices);
				Pool.FreeList<Vector3>(ref this.vertices);
			}
			if (this.normals != null)
			{
				mesh.SetNormals(this.normals);
				Pool.FreeList<Vector3>(ref this.normals);
			}
			if (this.triangles != null)
			{
				mesh.SetTriangles(this.triangles, 0);
				Pool.FreeList<int>(ref this.triangles);
			}
			if (this.indices != null)
			{
				Pool.FreeList<int>(ref this.indices);
			}
			return mesh;
		}
	}

	// Token: 0x06002F63 RID: 12131 RVA: 0x0011E7AC File Offset: 0x0011C9AC
	public AsyncTerrainNavMeshBake(Vector3 pivot, int width, int height, bool normal, bool alpha)
	{
		this.pivot = pivot;
		this.width = width;
		this.height = height;
		this.normal = normal;
		this.alpha = alpha;
		this.indices = Pool.GetList<int>();
		this.vertices = Pool.GetList<Vector3>();
		this.normals = (normal ? Pool.GetList<Vector3>() : null);
		this.triangles = Pool.GetList<int>();
		this.Invoke();
	}

	// Token: 0x06002F64 RID: 12132 RVA: 0x0011E820 File Offset: 0x0011CA20
	private void DoWork()
	{
		Vector3 vector = new Vector3((float)(this.width / 2), 0f, (float)(this.height / 2));
		Vector3 vector2 = new Vector3(this.pivot.x - vector.x, 0f, this.pivot.z - vector.z);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainAlphaMap alphaMap = TerrainMeta.AlphaMap;
		int num = 0;
		for (int i = 0; i <= this.height; i++)
		{
			int j = 0;
			while (j <= this.width)
			{
				Vector3 vector3 = new Vector3((float)j, 0f, (float)i) + vector2;
				Vector3 vector4 = new Vector3((float)j, 0f, (float)i) - vector;
				float num2 = heightMap.GetHeight(vector3);
				if (num2 < -1f)
				{
					this.indices.Add(-1);
				}
				else if (this.alpha && alphaMap.GetAlpha(vector3) < 0.1f)
				{
					this.indices.Add(-1);
				}
				else
				{
					if (this.normal)
					{
						Vector3 vector5 = heightMap.GetNormal(vector3);
						this.normals.Add(vector5);
					}
					vector3.y = (vector4.y = num2 - this.pivot.y);
					this.indices.Add(this.vertices.Count);
					this.vertices.Add(vector4);
				}
				j++;
				num++;
			}
		}
		int num3 = 0;
		int k = 0;
		while (k < this.height)
		{
			int l = 0;
			while (l < this.width)
			{
				int num4 = this.indices[num3];
				int num5 = this.indices[num3 + this.width + 1];
				int num6 = this.indices[num3 + 1];
				int num7 = this.indices[num3 + 1];
				int num8 = this.indices[num3 + this.width + 1];
				int num9 = this.indices[num3 + this.width + 2];
				if (num4 != -1 && num5 != -1 && num6 != -1)
				{
					this.triangles.Add(num4);
					this.triangles.Add(num5);
					this.triangles.Add(num6);
				}
				if (num7 != -1 && num8 != -1 && num9 != -1)
				{
					this.triangles.Add(num7);
					this.triangles.Add(num8);
					this.triangles.Add(num9);
				}
				l++;
				num3++;
			}
			k++;
			num3++;
		}
	}

	// Token: 0x06002F65 RID: 12133 RVA: 0x0011EACC File Offset: 0x0011CCCC
	private void Invoke()
	{
		this.worker = new Action(this.DoWork);
		this.worker.BeginInvoke(new AsyncCallback(this.Callback), null);
	}

	// Token: 0x06002F66 RID: 12134 RVA: 0x0011EAF9 File Offset: 0x0011CCF9
	private void Callback(IAsyncResult result)
	{
		this.worker.EndInvoke(result);
		this.worker = null;
	}

	// Token: 0x04002714 RID: 10004
	private List<int> indices;

	// Token: 0x04002715 RID: 10005
	private List<Vector3> vertices;

	// Token: 0x04002716 RID: 10006
	private List<Vector3> normals;

	// Token: 0x04002717 RID: 10007
	private List<int> triangles;

	// Token: 0x04002718 RID: 10008
	private Vector3 pivot;

	// Token: 0x04002719 RID: 10009
	private int width;

	// Token: 0x0400271A RID: 10010
	private int height;

	// Token: 0x0400271B RID: 10011
	private bool normal;

	// Token: 0x0400271C RID: 10012
	private bool alpha;

	// Token: 0x0400271D RID: 10013
	private Action worker;
}
