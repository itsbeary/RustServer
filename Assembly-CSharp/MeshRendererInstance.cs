using System;
using UnityEngine;

// Token: 0x020002B6 RID: 694
public struct MeshRendererInstance
{
	// Token: 0x1700026C RID: 620
	// (get) Token: 0x06001DA2 RID: 7586 RVA: 0x000CC220 File Offset: 0x000CA420
	// (set) Token: 0x06001DA3 RID: 7587 RVA: 0x000CC22D File Offset: 0x000CA42D
	public Mesh mesh
	{
		get
		{
			return this.data.mesh;
		}
		set
		{
			this.data = MeshCache.Get(value);
		}
	}

	// Token: 0x04001669 RID: 5737
	public Renderer renderer;

	// Token: 0x0400166A RID: 5738
	public OBB bounds;

	// Token: 0x0400166B RID: 5739
	public Vector3 position;

	// Token: 0x0400166C RID: 5740
	public Quaternion rotation;

	// Token: 0x0400166D RID: 5741
	public Vector3 scale;

	// Token: 0x0400166E RID: 5742
	public MeshCache.Data data;
}
