using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200027B RID: 635
public class SocketMod_EntityType : SocketMod
{
	// Token: 0x06001D05 RID: 7429 RVA: 0x000C8E18 File Offset: 0x000C7018
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001D06 RID: 7430 RVA: 0x000C8E88 File Offset: 0x000C7088
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(vector, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		foreach (BaseEntity baseEntity in list)
		{
			bool flag = baseEntity.GetType().IsAssignableFrom(this.searchType.GetType());
			if (flag && this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
			if (flag && !this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return false;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return !this.wantsCollide;
	}

	// Token: 0x04001592 RID: 5522
	public float sphereRadius = 1f;

	// Token: 0x04001593 RID: 5523
	public LayerMask layerMask;

	// Token: 0x04001594 RID: 5524
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x04001595 RID: 5525
	public BaseEntity searchType;

	// Token: 0x04001596 RID: 5526
	public bool wantsCollide;
}
