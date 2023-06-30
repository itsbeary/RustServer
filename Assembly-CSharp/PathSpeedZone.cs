using System;
using UnityEngine;

// Token: 0x020001AE RID: 430
public class PathSpeedZone : MonoBehaviour, IAIPathSpeedZone
{
	// Token: 0x060018E4 RID: 6372 RVA: 0x000B890E File Offset: 0x000B6B0E
	public OBB WorldSpaceBounds()
	{
		return new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
	}

	// Token: 0x060018E5 RID: 6373 RVA: 0x000B893C File Offset: 0x000B6B3C
	public float GetMaxSpeed()
	{
		return this.maxVelocityPerSec;
	}

	// Token: 0x060018E6 RID: 6374 RVA: 0x000B8944 File Offset: 0x000B6B44
	public virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
		Gizmos.DrawCube(this.bounds.center, this.bounds.size);
		Gizmos.color = new Color(1f, 0.7f, 0f, 1f);
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x04001178 RID: 4472
	public Bounds bounds;

	// Token: 0x04001179 RID: 4473
	public OBB obbBounds;

	// Token: 0x0400117A RID: 4474
	public float maxVelocityPerSec = 5f;
}
