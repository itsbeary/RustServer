using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x0200027A RID: 634
public class SocketMod_EntityCheck : SocketMod
{
	// Token: 0x06001D02 RID: 7426 RVA: 0x000C8CA8 File Offset: 0x000C6EA8
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001D03 RID: 7427 RVA: 0x000C8D18 File Offset: 0x000C6F18
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(vector, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		using (List<BaseEntity>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				BaseEntity ent = enumerator.Current;
				bool flag = this.entityTypes.Any((BaseEntity x) => x.prefabID == ent.prefabID);
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
		}
		Pool.FreeList<BaseEntity>(ref list);
		return !this.wantsCollide;
	}

	// Token: 0x0400158D RID: 5517
	public float sphereRadius = 1f;

	// Token: 0x0400158E RID: 5518
	public LayerMask layerMask;

	// Token: 0x0400158F RID: 5519
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x04001590 RID: 5520
	public BaseEntity[] entityTypes;

	// Token: 0x04001591 RID: 5521
	public bool wantsCollide;
}
