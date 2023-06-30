using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000949 RID: 2377
public class MeshRendererBatch : MeshBatch
{
	// Token: 0x1700047B RID: 1147
	// (get) Token: 0x060038E0 RID: 14560 RVA: 0x0015099B File Offset: 0x0014EB9B
	public override int VertexCapacity
	{
		get
		{
			return Batching.renderer_capacity;
		}
	}

	// Token: 0x1700047C RID: 1148
	// (get) Token: 0x060038E1 RID: 14561 RVA: 0x001509A2 File Offset: 0x0014EBA2
	public override int VertexCutoff
	{
		get
		{
			return Batching.renderer_vertices;
		}
	}

	// Token: 0x060038E2 RID: 14562 RVA: 0x00151C8C File Offset: 0x0014FE8C
	protected void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshData = new MeshRendererData();
		this.meshGroup = new MeshRendererGroup();
		this.meshLookup = new MeshRendererLookup();
	}

	// Token: 0x060038E3 RID: 14563 RVA: 0x00151CC8 File Offset: 0x0014FEC8
	public void Setup(Vector3 position, Material material, ShadowCastingMode shadows, int layer)
	{
		base.transform.position = position;
		this.position = position;
		base.gameObject.layer = layer;
		this.meshRenderer.sharedMaterial = material;
		this.meshRenderer.shadowCastingMode = shadows;
		if (shadows == ShadowCastingMode.ShadowsOnly)
		{
			this.meshRenderer.receiveShadows = false;
			this.meshRenderer.motionVectors = false;
			this.meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			this.meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			return;
		}
		this.meshRenderer.receiveShadows = true;
		this.meshRenderer.motionVectors = true;
		this.meshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
		this.meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
	}

	// Token: 0x060038E4 RID: 14564 RVA: 0x00151D74 File Offset: 0x0014FF74
	public void Add(MeshRendererInstance instance)
	{
		instance.position -= this.position;
		this.meshGroup.data.Add(instance);
		base.AddVertices(instance.mesh.vertexCount);
	}

	// Token: 0x060038E5 RID: 14565 RVA: 0x00151DC1 File Offset: 0x0014FFC1
	protected override void AllocMemory()
	{
		this.meshGroup.Alloc();
		this.meshData.Alloc();
	}

	// Token: 0x060038E6 RID: 14566 RVA: 0x00151DD9 File Offset: 0x0014FFD9
	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	// Token: 0x060038E7 RID: 14567 RVA: 0x00151DF1 File Offset: 0x0014FFF1
	protected override void RefreshMesh()
	{
		this.meshLookup.dst.Clear();
		this.meshData.Clear();
		this.meshData.Combine(this.meshGroup, this.meshLookup);
	}

	// Token: 0x060038E8 RID: 14568 RVA: 0x00151E28 File Offset: 0x00150028
	protected override void ApplyMesh()
	{
		if (!this.meshBatch)
		{
			this.meshBatch = AssetPool.Get<UnityEngine.Mesh>();
		}
		this.meshLookup.Apply();
		this.meshData.Apply(this.meshBatch);
		this.meshBatch.UploadMeshData(false);
	}

	// Token: 0x060038E9 RID: 14569 RVA: 0x00151E78 File Offset: 0x00150078
	protected override void ToggleMesh(bool state)
	{
		List<MeshRendererLookup.LookupEntry> data = this.meshLookup.src.data;
		for (int i = 0; i < data.Count; i++)
		{
			Renderer renderer = data[i].renderer;
			if (renderer)
			{
				renderer.enabled = !state;
			}
		}
		if (state)
		{
			if (this.meshFilter)
			{
				this.meshFilter.sharedMesh = this.meshBatch;
			}
			if (this.meshRenderer)
			{
				this.meshRenderer.enabled = true;
				return;
			}
		}
		else
		{
			if (this.meshFilter)
			{
				this.meshFilter.sharedMesh = null;
			}
			if (this.meshRenderer)
			{
				this.meshRenderer.enabled = false;
			}
		}
	}

	// Token: 0x060038EA RID: 14570 RVA: 0x00151F34 File Offset: 0x00150134
	protected override void OnPooled()
	{
		if (this.meshFilter)
		{
			this.meshFilter.sharedMesh = null;
		}
		if (this.meshBatch)
		{
			AssetPool.Free(ref this.meshBatch);
		}
		this.meshData.Free();
		this.meshGroup.Free();
		this.meshLookup.src.Clear();
		this.meshLookup.dst.Clear();
	}

	// Token: 0x040033BB RID: 13243
	private Vector3 position;

	// Token: 0x040033BC RID: 13244
	private UnityEngine.Mesh meshBatch;

	// Token: 0x040033BD RID: 13245
	private MeshFilter meshFilter;

	// Token: 0x040033BE RID: 13246
	private MeshRenderer meshRenderer;

	// Token: 0x040033BF RID: 13247
	private MeshRendererData meshData;

	// Token: 0x040033C0 RID: 13248
	private MeshRendererGroup meshGroup;

	// Token: 0x040033C1 RID: 13249
	private MeshRendererLookup meshLookup;
}
