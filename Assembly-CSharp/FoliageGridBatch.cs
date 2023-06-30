using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200093A RID: 2362
public class FoliageGridBatch : MeshBatch
{
	// Token: 0x17000470 RID: 1136
	// (get) Token: 0x0600388E RID: 14478 RVA: 0x0015099B File Offset: 0x0014EB9B
	public override int VertexCapacity
	{
		get
		{
			return Batching.renderer_capacity;
		}
	}

	// Token: 0x17000471 RID: 1137
	// (get) Token: 0x0600388F RID: 14479 RVA: 0x001509A2 File Offset: 0x0014EBA2
	public override int VertexCutoff
	{
		get
		{
			return Batching.renderer_vertices;
		}
	}

	// Token: 0x06003890 RID: 14480 RVA: 0x001509A9 File Offset: 0x0014EBA9
	protected void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshData = new FoliageGridMeshData();
		this.meshGroup = new MeshGroup();
	}

	// Token: 0x06003891 RID: 14481 RVA: 0x001509DC File Offset: 0x0014EBDC
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

	// Token: 0x06003892 RID: 14482 RVA: 0x00150A88 File Offset: 0x0014EC88
	public void Add(MeshInstance instance)
	{
		instance.position -= this.position;
		this.meshGroup.data.Add(instance);
		base.AddVertices(instance.mesh.vertexCount);
	}

	// Token: 0x06003893 RID: 14483 RVA: 0x00150AD5 File Offset: 0x0014ECD5
	protected override void AllocMemory()
	{
		this.meshGroup.Alloc();
		this.meshData.Alloc();
	}

	// Token: 0x06003894 RID: 14484 RVA: 0x00150AED File Offset: 0x0014ECED
	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	// Token: 0x06003895 RID: 14485 RVA: 0x00150B05 File Offset: 0x0014ED05
	protected override void RefreshMesh()
	{
		this.meshData.Clear();
		this.meshData.Combine(this.meshGroup);
	}

	// Token: 0x06003896 RID: 14486 RVA: 0x00150B23 File Offset: 0x0014ED23
	protected override void ApplyMesh()
	{
		if (!this.meshBatch)
		{
			this.meshBatch = AssetPool.Get<UnityEngine.Mesh>();
		}
		this.meshData.Apply(this.meshBatch);
		this.meshBatch.UploadMeshData(false);
	}

	// Token: 0x06003897 RID: 14487 RVA: 0x00150B5C File Offset: 0x0014ED5C
	protected override void ToggleMesh(bool state)
	{
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

	// Token: 0x06003898 RID: 14488 RVA: 0x00150BD8 File Offset: 0x0014EDD8
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
	}

	// Token: 0x040033A9 RID: 13225
	private Vector3 position;

	// Token: 0x040033AA RID: 13226
	private UnityEngine.Mesh meshBatch;

	// Token: 0x040033AB RID: 13227
	private MeshFilter meshFilter;

	// Token: 0x040033AC RID: 13228
	private MeshRenderer meshRenderer;

	// Token: 0x040033AD RID: 13229
	private FoliageGridMeshData meshData;

	// Token: 0x040033AE RID: 13230
	private MeshGroup meshGroup;
}
