using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200027F RID: 639
public class SocketMod_PlantCheck : SocketMod
{
	// Token: 0x06001D11 RID: 7441 RVA: 0x000C918C File Offset: 0x000C738C
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001D12 RID: 7442 RVA: 0x000C91FC File Offset: 0x000C73FC
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(vector, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		foreach (BaseEntity baseEntity in list)
		{
			GrowableEntity component = baseEntity.GetComponent<GrowableEntity>();
			if (component && this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
			if (component && !this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return false;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return !this.wantsCollide;
	}

	// Token: 0x0400159D RID: 5533
	public float sphereRadius = 1f;

	// Token: 0x0400159E RID: 5534
	public LayerMask layerMask;

	// Token: 0x0400159F RID: 5535
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x040015A0 RID: 5536
	public bool wantsCollide;
}
