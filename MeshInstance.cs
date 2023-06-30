using System;
using UnityEngine;

// Token: 0x020002B3 RID: 691
public struct MeshInstance
{
	// Token: 0x1700026B RID: 619
	// (get) Token: 0x06001D98 RID: 7576 RVA: 0x000CB975 File Offset: 0x000C9B75
	// (set) Token: 0x06001D99 RID: 7577 RVA: 0x000CB982 File Offset: 0x000C9B82
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

	// Token: 0x0400165D RID: 5725
	public Vector3 position;

	// Token: 0x0400165E RID: 5726
	public Quaternion rotation;

	// Token: 0x0400165F RID: 5727
	public Vector3 scale;

	// Token: 0x04001660 RID: 5728
	public MeshCache.Data data;
}
