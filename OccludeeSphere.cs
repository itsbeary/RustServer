using System;
using UnityEngine;

// Token: 0x020009A1 RID: 2465
public struct OccludeeSphere
{
	// Token: 0x17000499 RID: 1177
	// (get) Token: 0x06003A78 RID: 14968 RVA: 0x00158FB4 File Offset: 0x001571B4
	public bool IsRegistered
	{
		get
		{
			return this.id >= 0;
		}
	}

	// Token: 0x06003A79 RID: 14969 RVA: 0x00158FC2 File Offset: 0x001571C2
	public void Invalidate()
	{
		this.id = -1;
		this.state = null;
		this.sphere = default(OcclusionCulling.Sphere);
	}

	// Token: 0x06003A7A RID: 14970 RVA: 0x00158FDE File Offset: 0x001571DE
	public OccludeeSphere(int id)
	{
		this.id = id;
		this.state = ((id < 0) ? null : OcclusionCulling.GetStateById(id));
		this.sphere = new OcclusionCulling.Sphere(Vector3.zero, 0f);
	}

	// Token: 0x06003A7B RID: 14971 RVA: 0x0015900F File Offset: 0x0015720F
	public OccludeeSphere(int id, OcclusionCulling.Sphere sphere)
	{
		this.id = id;
		this.state = ((id < 0) ? null : OcclusionCulling.GetStateById(id));
		this.sphere = sphere;
	}

	// Token: 0x0400353C RID: 13628
	public int id;

	// Token: 0x0400353D RID: 13629
	public OccludeeState state;

	// Token: 0x0400353E RID: 13630
	public OcclusionCulling.Sphere sphere;
}
