using System;
using UnityEngine;

// Token: 0x020002AF RID: 687
public struct MeshColliderInstance
{
	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06001D8A RID: 7562 RVA: 0x000CB250 File Offset: 0x000C9450
	// (set) Token: 0x06001D8B RID: 7563 RVA: 0x000CB25D File Offset: 0x000C945D
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

	// Token: 0x0400164B RID: 5707
	public Transform transform;

	// Token: 0x0400164C RID: 5708
	public Rigidbody rigidbody;

	// Token: 0x0400164D RID: 5709
	public Collider collider;

	// Token: 0x0400164E RID: 5710
	public OBB bounds;

	// Token: 0x0400164F RID: 5711
	public Vector3 position;

	// Token: 0x04001650 RID: 5712
	public Quaternion rotation;

	// Token: 0x04001651 RID: 5713
	public Vector3 scale;

	// Token: 0x04001652 RID: 5714
	public MeshCache.Data data;
}
