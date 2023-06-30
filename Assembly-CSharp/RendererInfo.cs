using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020002B8 RID: 696
public class RendererInfo : ComponentInfo<Renderer>
{
	// Token: 0x06001DA9 RID: 7593 RVA: 0x000CC2B8 File Offset: 0x000CA4B8
	public override void Reset()
	{
		this.component.shadowCastingMode = this.shadows;
		if (this.material)
		{
			this.component.sharedMaterial = this.material;
		}
		SkinnedMeshRenderer skinnedMeshRenderer;
		if ((skinnedMeshRenderer = this.component as SkinnedMeshRenderer) != null)
		{
			skinnedMeshRenderer.sharedMesh = this.mesh;
			return;
		}
		if (this.component is MeshRenderer)
		{
			this.meshFilter.sharedMesh = this.mesh;
		}
	}

	// Token: 0x06001DAA RID: 7594 RVA: 0x000CC330 File Offset: 0x000CA530
	public override void Setup()
	{
		this.shadows = this.component.shadowCastingMode;
		this.material = this.component.sharedMaterial;
		SkinnedMeshRenderer skinnedMeshRenderer;
		if ((skinnedMeshRenderer = this.component as SkinnedMeshRenderer) != null)
		{
			this.mesh = skinnedMeshRenderer.sharedMesh;
			return;
		}
		if (this.component is MeshRenderer)
		{
			this.meshFilter = base.GetComponent<MeshFilter>();
			this.mesh = this.meshFilter.sharedMesh;
		}
	}

	// Token: 0x04001671 RID: 5745
	public ShadowCastingMode shadows;

	// Token: 0x04001672 RID: 5746
	public Material material;

	// Token: 0x04001673 RID: 5747
	public Mesh mesh;

	// Token: 0x04001674 RID: 5748
	public MeshFilter meshFilter;
}
