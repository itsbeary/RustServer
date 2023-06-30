using System;
using UnityEngine;

// Token: 0x020004FA RID: 1274
public class DeployShell : PrefabAttribute
{
	// Token: 0x06002943 RID: 10563 RVA: 0x000FE3D0 File Offset: 0x000FC5D0
	public OBB WorldSpaceBounds(Transform transform)
	{
		return new OBB(transform.position, transform.lossyScale, transform.rotation, this.bounds);
	}

	// Token: 0x06002944 RID: 10564 RVA: 0x000FE3EF File Offset: 0x000FC5EF
	public float LineOfSightPadding()
	{
		return 0.025f;
	}

	// Token: 0x06002945 RID: 10565 RVA: 0x000FE3F6 File Offset: 0x000FC5F6
	protected override Type GetIndexedType()
	{
		return typeof(DeployShell);
	}

	// Token: 0x0400216A RID: 8554
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
}
