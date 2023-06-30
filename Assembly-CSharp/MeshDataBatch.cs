using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000948 RID: 2376
public class MeshDataBatch : MeshBatch
{
	// Token: 0x17000479 RID: 1145
	// (get) Token: 0x060038D4 RID: 14548 RVA: 0x0015099B File Offset: 0x0014EB9B
	public override int VertexCapacity
	{
		get
		{
			return Batching.renderer_capacity;
		}
	}

	// Token: 0x1700047A RID: 1146
	// (get) Token: 0x060038D5 RID: 14549 RVA: 0x001509A2 File Offset: 0x0014EBA2
	public override int VertexCutoff
	{
		get
		{
			return Batching.renderer_vertices;
		}
	}

	// Token: 0x060038D6 RID: 14550 RVA: 0x00151A0C File Offset: 0x0014FC0C
	protected void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshData = new MeshData();
		this.meshGroup = new MeshGroup();
	}

	// Token: 0x060038D7 RID: 14551 RVA: 0x00151A3C File Offset: 0x0014FC3C
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

	// Token: 0x060038D8 RID: 14552 RVA: 0x00151AE8 File Offset: 0x0014FCE8
	public void Add(MeshInstance instance)
	{
		instance.position -= this.position;
		this.meshGroup.data.Add(instance);
		base.AddVertices(instance.mesh.vertexCount);
	}

	// Token: 0x060038D9 RID: 14553 RVA: 0x00151B35 File Offset: 0x0014FD35
	protected override void AllocMemory()
	{
		this.meshGroup.Alloc();
		this.meshData.Alloc();
	}

	// Token: 0x060038DA RID: 14554 RVA: 0x00151B4D File Offset: 0x0014FD4D
	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	// Token: 0x060038DB RID: 14555 RVA: 0x00151B65 File Offset: 0x0014FD65
	protected override void RefreshMesh()
	{
		this.meshData.Clear();
		this.meshData.Combine(this.meshGroup);
	}

	// Token: 0x060038DC RID: 14556 RVA: 0x00151B83 File Offset: 0x0014FD83
	protected override void ApplyMesh()
	{
		if (!this.meshBatch)
		{
			this.meshBatch = AssetPool.Get<UnityEngine.Mesh>();
		}
		this.meshData.Apply(this.meshBatch);
		this.meshBatch.UploadMeshData(false);
	}

	// Token: 0x060038DD RID: 14557 RVA: 0x00151BBC File Offset: 0x0014FDBC
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

	// Token: 0x060038DE RID: 14558 RVA: 0x00151C38 File Offset: 0x0014FE38
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

	// Token: 0x040033B5 RID: 13237
	private Vector3 position;

	// Token: 0x040033B6 RID: 13238
	private UnityEngine.Mesh meshBatch;

	// Token: 0x040033B7 RID: 13239
	private MeshFilter meshFilter;

	// Token: 0x040033B8 RID: 13240
	private MeshRenderer meshRenderer;

	// Token: 0x040033B9 RID: 13241
	private MeshData meshData;

	// Token: 0x040033BA RID: 13242
	private MeshGroup meshGroup;
}
